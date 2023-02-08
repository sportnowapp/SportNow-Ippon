﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Acr.UserDialogs;
using SportNow.Model;
using SportNow.Services.Data.JSON;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration.AndroidSpecific;

namespace SportNow.Views
{
    public class MainTabbedPageCS : Xamarin.Forms.TabbedPage
    {

        public async void initSpecificLayout(string actiontype, string actionid) {

            NavigationPage.SetBackButtonTitle(this, "");

            //

            //NavigationPage.SetHasNavigationBar(this, false);

            //Children.Add(item: new IntroPageCS() { Title = "PERFIL", IconImageSource = "perfil.png" });
            //Children.Add(new IntroPageCS() { Title = "PRESENÇAS", IconImageSource = "presencasicon.png" });
            Children.Add(new IntroPageCS() { Title = "AGUARDE", IconImageSource = "logo_aksl_icon.png" });
            //Children.Add(new IntroPageCS() { Title = "EVENTOS", IconImageSource = "eventos.png" });
            //Children.Add(new IntroPageCS() { Title = "EQUIPAMENTOS", IconImageSource = "kimono.png" });
            CurrentPage = Children[0];


            On<Xamarin.Forms.PlatformConfiguration.Android>().SetToolbarPlacement(ToolbarPlacement.Bottom);

            this.UnselectedTabColor = Color.White;
            this.BackgroundColor = Color.FromRgb(15, 15, 15);
            this.BarBackgroundColor = Color.FromRgb(15, 15, 15);
            this.BarTextColor = Color.White;//FromRgb(75, 75, 75); ;
            this.SelectedTabColor = Color.FromRgb(180, 143, 86);

            //public static double ScreenWidth; = Application.Current.MainPage.Width;
            //public static double ScreenHeight; = Application.Current.MainPage.Height;

            App.screenWidthAdapterCalculator();
            App.screenHeightAdapterCalculator();

            await checkMinimumVersion();

            if (App.members == null) //não foi feito login ainda
            {
                Debug.Print("não foi feito login ainda");
                App.members = await GetMembers();
                if (App.members == null)//com os dados que temos de login não retornou nenhum membro, logo vai para Login
                {
                    Debug.Print("App members é null");
                    Navigation.InsertPageBefore(new LoginPageCS(""), this);
                    await Navigation.PopAsync();
                    return;
                }
                else if (App.members.Count == 0) //com os dados que temos de login não retornou nenhum membro, logo vai para Login
                {
                    Debug.Print("App members é null");
                    Navigation.InsertPageBefore(new LoginPageCS(""), this);
                    await Navigation.PopAsync();
                    return;
                }
                else if (App.members.Count == 1) //foi feito login automático e retornou apenas 1 socio pelo que pode entrar para a página principal
                {
                    Debug.Print("App.members.Count = 1");
                    App.original_member = App.members[0];
                    App.member = App.original_member;

                }
                else if (App.members.Count > 1) //foi feito login automático e retornou mais do que 1 socio
                {
                    Debug.Print("App.members.Count > 1");
                    if (App.Current.Properties.ContainsKey("SELECTEDUSER")) //vê se tem selected user gravado
                    {
                        
                        string selectedUser = App.Current.Properties["SELECTEDUSER"] as string;
                        Debug.Print("App.members.Count > 1 e selectedUser = "+ selectedUser);
                        foreach (Member member in App.members)
                        {
                            if (selectedUser == member.id)
                            {
                                App.original_member = member;
                                App.member = member;
                            }
                        }
                        if (App.member == null) //não encontrou o selected user nos membros do login pelo que tem de ir para a página de select member
                        {
                            Debug.Print("App.members.Count > 1 e selectedUser não foi encontrado");
                            Navigation.InsertPageBefore(new SelectMemberPageCS(), this);
                            await Navigation.PopAsync();
                            return;
                        }
                    }
                    else //não tem o selected user gravado pelo que tem de ir para a página de select member
                    {
                        Debug.Print("App.members.Count > 1 e selectedUser não existe");
                        Navigation.InsertPageBefore(new SelectMemberPageCS(), this);
                        await Navigation.PopAsync();
                        return;
                    }
                }
                //se chegou aqui é pq sabe quem é o member
                if ((App.token != null) & (App.original_member.id == App.member.id))
                {
                    MemberManager memberManager = new MemberManager();
                    var result = await memberManager.updateToken(App.original_member.id, App.token);
                }

                //result = await GetCurrentFees(App.member);
                //App.original_member.students_count = await GetMemberStudents_Count(App.original_member.id);
                //App.member.students_count = await GetMemberStudents_Count(App.member.id);

                //App.notification = App.notification + " createTabs 0 ";
                //App.notification = App.notification + " createTabs 1 "+ actiontype;
                if (actiontype != "")
                {
                    App.member = App.original_member;
                    App.original_member.students_count = await GetMemberStudents_Count(App.original_member.id);
                    App.member.students_count = App.original_member.students_count;

                    App.notification = App.notification + " actiontype != empty ";
                    await navigateToPageAsync(actiontype, actionid);

                }
                else
                {
                    App.original_member.students_count = await GetMemberStudents_Count(App.original_member.id);
                    if (App.original_member.id != App.member.id)
                    {
                        App.member.students_count = await GetMemberStudents_Count(App.member.id);
                    }
                    else
                    {
                        App.member.students_count = App.original_member.students_count;
                    }
                }
                await createTabs();
                

            }
            else if (App.members.Count == 0) //os dados do login anterior não retornaram membros pelo que volta para o login
            {
                Debug.Print("App.members.Count = 0 volta para Login");
                App.Current.MainPage = new NavigationPage(new LoginPageCS(""))
                {
                    BarBackgroundColor = Color.FromRgb(15, 15, 15),
                    BarTextColor = Color.White
                };
            }
            else if (App.members.Count > 0) //veio do login ou de select member e retornou vários membros mas já preencheu o que tinha a preencher.
            {
                //var result = await GetCurrentFees(App.member);
                //App.original_member.students_count = await GetMemberStudents_Count(App.original_member.id);
                //App.member.students_count = await GetMemberStudents_Count(App.member.id);

                if ((App.token != null) & (App.original_member.id == App.member.id))
                {
                    MemberManager memberManager = new MemberManager();
                    var result = await memberManager.updateToken(App.original_member.id, App.token);
                }
                App.notification = App.notification + " createTabs 0.1 "+actiontype;
                if (actiontype != "")
                {
                    App.member = App.original_member;
                    App.original_member.students_count = await GetMemberStudents_Count(App.original_member.id);
                    App.member.students_count = App.original_member.students_count;

                    App.notification = App.notification + " actiontype != empty ";
                    await navigateToPageAsync(actiontype, actionid);

                }
                else
                {
                    App.original_member.students_count = await GetMemberStudents_Count(App.original_member.id);
                    if (App.original_member.id != App.member.id)
                    {
                        App.member.students_count = await GetMemberStudents_Count(App.member.id);
                    }
                    else
                    {
                        App.member.students_count = App.original_member.students_count;
                    }
                    
                }
                await createTabs();
                App.notification = App.notification + " createTabs 2 ";

            }
        }

        public async Task<int> createTabs()
        {
            Children.RemoveAt(0);
            Children.Add(new DoPageCS() { Title = "HISTÓRICO", IconImageSource = "icondo.png" });
            Children.Add(new AttendanceOptionsPageCS() { Title = "AULAS", IconImageSource = "presencasicon.png" });
            Children.Add(new MainPageCS() { Title = "PRINCIPAL", IconImageSource = "logo_aksl_icon.png" });
            Children.Add(new AllEventsPageCS() { Title = "EVENTOS", IconImageSource = "eventos.png" });
            Children.Add(new EquipamentTypePageCS() { Title = "EQUIPAMENTOS", IconImageSource = "kimono.png" });
            CurrentPage = Children[2];
            Title = "PRINCIPAL";
            CurrentPageChanged += CurrentPageHasChanged;

            return 0;
        }

        public MainTabbedPageCS(string actiontype, string actionid)
        {
            Debug.Print("MainTabbedPageCS");

            initSpecificLayout(actiontype, actionid);
        }

        private async Task<int> navigateToPageAsync(string actiontype, string actionid)
        {
            //App.member = App.original_member; //QUANDO VEM DE NOTIFICAÇÃO QUERO QUE ABRA SEMPRE NO UTILIZADOR ORIGINAL

            App.notification = App.notification + " navigateToPageAsync ";
            if (actiontype == "openevent")
            {
                App.notification = App.notification + " actiontype == openevent ";
                Event event_i = await GetEvent_byID(App.member.id, actionid);
                App.notification = App.notification + " event_i = "+event_i.name;
                await Navigation.PushAsync(new DetailEventPageCS(event_i));
            }
            else if (actiontype == "opencompetition")
            {
                App.notification = App.notification + " actiontype == opencompetition ";
                await Navigation.PushAsync(new DetailCompetitionPageCS(actionid));
            }
            else if (actiontype == "openexaminationsession")
            {
                App.notification = App.notification + " actiontype == openexaminationsession ";
                await Navigation.PushAsync(new ExaminationSessionPageCS(actionid));
            }
            else if (actiontype == "openweekclass")
            {
                App.notification = App.notification + " actiontype == openweekclass ";
                await Navigation.PushAsync(new AttendancePageCS("next"));
            }
            else if (actiontype == "opentodayclass")
            {
                App.notification = App.notification + " actiontype == opentodayclass ";
                await Navigation.PushAsync(new AttendancePageCS());
            }
            else if (actiontype == "opentodayclassinstructor")
            {
                App.notification = App.notification + " actiontype == opentodayclassinstructor ";
                await Navigation.PushAsync(new AttendanceManagePageCS());
            }
            else if (actiontype == "opentodayclassinstructor")
            {
                App.notification = App.notification + " actiontype == opentodayclassinstructor ";
                await Navigation.PushAsync(new AttendanceManagePageCS());
            }


            return 0;
        }

        async Task<Event> GetEvent_byID(string memberid, string eventid)
        {
            Debug.WriteLine("GetFutureExaminationSessions");
            EventManager eventManager = new EventManager();

            Event event_i = await eventManager.GetEvent_byID(memberid, eventid);
            if (event_i == null)
            {
                Xamarin.Forms.Application.Current.MainPage = new NavigationPage(new LoginPageCS("Verifique a sua ligação à Internet e tente novamente."))
                {
                    BarBackgroundColor = Color.FromRgb(15, 15, 15),
                    BarTextColor = Color.White
                };
            }

            return event_i;
        }


        private void CurrentPageHasChanged(object sender, EventArgs e)
        {
            Title = CurrentPage.Title;
            NavigationPage.SetBackButtonTitle(this, "");
        }


        public async Task<List<Member>> GetMembers()
        {
            Debug.WriteLine("GetMembers");

            string username = "", password = "";

            if (App.Current.Properties.ContainsKey("EMAIL"))
            {
                username = App.Current.Properties["EMAIL"] as string;
            }

            if (App.Current.Properties.ContainsKey("PASSWORD"))
            {
                password = App.Current.Properties["PASSWORD"] as string;
            }
            var user = new User
            {
                Username = username,
                Password = password
            };

            MemberManager memberManager = new MemberManager();

            List<Member> members;

            members = await memberManager.GetMembers(user);

            return members;

        }

        async Task<int> GetCurrentFees(Member member)
        {
            Debug.WriteLine("MainTabbedPageCS.GetCurrentFees");
            MemberManager memberManager = new MemberManager();

            var result = await memberManager.GetCurrentFees(member);

            return result;
        }


        async Task<int> checkMinimumVersion()
        {
            Debug.Print("MainTabbedPageCS.checkMinimumVersion");
            AppManager appManager = new AppManager();

            MinimumVersion minimumVersion = await appManager.GetMinimumVersion();

            if (Convert.ToInt32(minimumVersion.build) > Convert.ToInt32(App.BuildNumber))
            {
                await DisplayAlert("ATUALIZAR APP", "Para continuar a utilizar a nossa App deverá efetuar a atualização para uma versão mais recente.", "Ok");

                Xamarin.Forms.Application.Current.MainPage = new NavigationPage(new LoginPageCS("Efetue a atualização da App para uma versão mais recente."))
                {
                    BarBackgroundColor = Color.Black,
                    BarTextColor = Color.White
                };
            }
            Debug.Print("MainTabbedPageCS.checkMinimumVersion is OK!");
            return 1;
        }

        async Task<int> GetMemberStudents_Count(string memberid)
        {
            Debug.WriteLine("MainTabbedPageCS.GetMemberStudents_Count");
            MemberManager memberManager = new MemberManager();

            var result = await memberManager.GetMemberStudents_Count(memberid);

            return result;
        }
    }
}
