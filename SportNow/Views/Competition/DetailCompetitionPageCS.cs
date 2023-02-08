using System;
using System.Collections.Generic;
using Xamarin.Forms;
using SportNow.Model;
using SportNow.Services.Data.JSON;
using System.Threading.Tasks;
using System.Diagnostics;
using SportNow.CustomViews;
using Xamarin.Essentials;
using Acr.UserDialogs;

namespace SportNow.Views
{
	public class DetailCompetitionPageCS : ContentPage
	{

		protected override void OnAppearing()
		{
			refreshCompetitionStatus(competition);
			/*competition_participation = App.competition_participation;

			if (competition_participation != null) { 

				Debug.Print("competition_participation.estado=" + competition_participation.estado);

				if ((competition_participation.estado == "confirmado") & (competition.participationconfirmed != 1))
				{
					competition.participationconfirmed = 1;		
				}
			}*/
			//initSpecificLayout();
		}

		protected override void OnDisappearing()
		{
			CleanScreen();
		}


		FormValue estadoValue = new FormValue("");

		private RelativeLayout relativeLayout;

		private Competition competition;
		private List<Competition> competitions;

		private List<Payment> payments;

		Button registerButton = new Button();

		private Grid gridCompetiton;

		public void initLayout()
		{
			Title = competition.name;
			this.BackgroundColor = Color.FromRgb(25, 25, 25);

			

			relativeLayout = new RelativeLayout
			{
				Margin = new Thickness(5)
			};
			Content = relativeLayout;

			relativeLayout.Children.Add(new Image
			{
				Source = "boneco_karate.png"
			},
			xConstraint: Constraint.Constant(0),
			yConstraint: Constraint.Constant(0),
			widthConstraint: Constraint.RelativeToParent((parent) => { return parent.Width; }),
			heightConstraint: Constraint.RelativeToParent((parent) => { return parent.Height; }));

			NavigationPage.SetBackButtonTitle(this, "");
		}


		public void CleanScreen()
		{
			Debug.Print("CleanScreen AQUIIII");
			if (gridCompetiton != null)
			{
				relativeLayout.Children.Remove(gridCompetiton);
				gridCompetiton = null;

			}
			if (registerButton != null)
            {
				relativeLayout.Children.Remove(registerButton);
				registerButton = null;
			}
				
		}

		public async void initSpecificLayout()
		{
			gridCompetiton = new Grid { Padding = 0, HorizontalOptions = LayoutOptions.FillAndExpand };
			gridCompetiton.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
			gridCompetiton.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
			gridCompetiton.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
			gridCompetiton.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
			gridCompetiton.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
			gridCompetiton.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
			gridCompetiton.RowDefinitions.Add(new RowDefinition { Height = 80 });
			gridCompetiton.RowDefinitions.Add(new RowDefinition { Height = 120 });
			gridCompetiton.RowDefinitions.Add(new RowDefinition { Height = GridLength.Star });
			//gridGeral.RowDefinitions.Add(new RowDefinition { Height = 1 });
			gridCompetiton.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto }); //GridLength.Auto
			gridCompetiton.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star }); //GridLength.Auto 

			Label dateLabel = new FormLabel { Text = "DATA" };
			FormValue dateValue = new FormValue(competition.detailed_date);

			FormLabel placeLabel = new FormLabel { Text = "LOCAL" };
			FormValue placeValue = new FormValue(competition.place);

			FormLabel typeLabel = new FormLabel { Text = "TIPO" };
			FormValue typeValue = new FormValue(Constants.competition_type[competition.type]);

			FormLabel valueLabel = new FormLabel { Text = "VALOR" };
			FormValue valueValue = new FormValue(String.Format("{0:0.00}", competition.value + "€"));

			FormLabel websiteLabel = new FormLabel { Text = "WEBSITE" };
			FormValue websiteValue = new FormValue(competition.website);

			websiteValue.GestureRecognizers.Add(new TapGestureRecognizer
			{
				Command = new Command(async () => {
					try
					{
						await Browser.OpenAsync(competition.website, BrowserLaunchMode.SystemPreferred);
					}
					catch (Exception ex)
					{
						// An unexpected error occured. No browser may be installed on the device.
					}
				})
			});

			FormLabel estadoLabel = new FormLabel { Text = "ESTADO" }; ;
			estadoValue = new FormValue("");

			List<Competition_Participation> competitionCall = await GetCompetitionCall();

			Debug.Print("examination_session.registrationbegindate = " + competition.registrationbegindate);
			DateTime currentTime = DateTime.Now.Date;
			DateTime registrationbegindate_datetime = new DateTime();
			DateTime registrationlimitdate_datetime = new DateTime();


			if ((competition.registrationbegindate != "") & (competition.registrationbegindate != null))
			{
				registrationbegindate_datetime = DateTime.Parse(competition.registrationbegindate).Date;
			}
			if ((competition.registrationlimitdate != "") & (competition.registrationlimitdate != null))
			{
				registrationlimitdate_datetime = DateTime.Parse(competition.registrationlimitdate).Date;
			}
			Debug.Print("event_v.registrationbegindate = " + competition.registrationbegindate + " " + registrationbegindate_datetime);
			Debug.Print("event_v.registrationlimitdate = " + competition.registrationlimitdate + " " + registrationlimitdate_datetime);

			int registrationOpened = -1;
			string limitDateLabelText = "";

			if ((competition.registrationbegindate == "") | (competition.registrationbegindate == null))
			{
				Debug.Print("Data início de inscrições ainda não está definida");
				limitDateLabelText = "As inscrições ainda não estão abertas.";
			}
			else if ((currentTime - registrationbegindate_datetime).Days < 0)
			{
				Debug.Print("Inscrições ainda não abriram");
				limitDateLabelText = "As inscrições abrem no dia " + competition.registrationbegindate + ".";
			}
			else
			{

				Debug.Print("Inscrições já abriram " + (registrationlimitdate_datetime - currentTime).Days);
				if ((registrationlimitdate_datetime - currentTime).Days < 0)
				{
					Debug.Print("Inscrições já fecharam");
					limitDateLabelText = "Ohhh...As inscrições já terminaram.";
					registrationOpened = 0;
				}
				else
				{
					registrationOpened = 1;
					Debug.Print("Inscrições estão abertas!");
					limitDateLabelText = "As inscrições estão abertas e terminam no dia " + competition.registrationlimitdate + ".";
				}
			}

			if (competition.participationconfirmed == "confirmado")
			{
				estadoValue = new FormValue("INSCRITO");
				estadoValue.label.TextColor = Color.FromRgb(96, 182, 89);
				limitDateLabelText = "BOA SORTE!";
			}
			else if (competition.participationconfirmed == "convocado")
			{
				estadoValue = new FormValue("NÃO INSCRITO");
				estadoValue.label.TextColor = Color.Red;

				if (registrationOpened == 1) { 
					registerButton = new Button
					{
						Text = "INSCREVER",
						BackgroundColor = Color.FromRgb(96, 182, 89),
						TextColor = Color.White,
						FontSize = App.itemTitleFontSize,
						WidthRequest = 100,
						HeightRequest = 50,
						VerticalOptions = LayoutOptions.End
					};

					Frame frame_registerButton = new Frame
					{
						BackgroundColor = Color.Transparent,
						//BorderColor = Color.FromRgb(96, 182, 89),
						WidthRequest = 100,
						HeightRequest = 50,
						CornerRadius = 10,
						IsClippedToBounds = true,
						Padding = 0
					};

					frame_registerButton.Content = registerButton;
					registerButton.Clicked += OnRegisterButtonClicked;

					gridCompetiton.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
					gridCompetiton.Children.Add(frame_registerButton, 0, 8);
					Grid.SetColumnSpan(frame_registerButton, 2);
				}
				
			}
			else if (competition.participationconfirmed == null)
			{
				if ((competitionCall == null) | (competitionCall.Count == 0))
				{
					estadoValue = new FormValue("-");
				}

				else
				{
					estadoValue = new FormValue("NÃO CONVOCADO");
				}
				estadoValue.label.TextColor = Color.White;
			}


			Label limitDateLabel = new Label
			{
				Text = limitDateLabelText,
				TextColor = Color.FromRgb(246, 220, 178),
				WidthRequest = 200,
				HeightRequest = 30,
				FontSize = 20,
				HorizontalTextAlignment = TextAlignment.Center
			};


			if ((competitionCall == null) | (competitionCall.Count == 0) | (registrationOpened == -1)) 
			{
				Label convocatoriaLabel = new Label
				{
					Text = "Ainda não existe Convocatória para esta Competição.",
					TextColor = Color.White,
					FontSize = 20,
					VerticalTextAlignment = TextAlignment.Center,
					HorizontalTextAlignment = TextAlignment.Center
				};
				gridCompetiton.Children.Add(convocatoriaLabel, 0, 7);
				Grid.SetColumnSpan(convocatoriaLabel, 2);
			}
			else
			{
				Image convocatoriaImage = new Image
				{
					Source = "iconconvocatoria.png",
					HorizontalOptions = LayoutOptions.End
				};

				Label convocatoriaLabel = new Label
				{
					Text = "Convocatória",
					TextColor = Color.White,
					FontSize = 20,
					VerticalTextAlignment = TextAlignment.Center,
					HorizontalTextAlignment = TextAlignment.Start
				};

				StackLayout convocatoriaStackLayout = new StackLayout
				{
					Orientation = StackOrientation.Horizontal,
					HorizontalOptions = LayoutOptions.Center,
					VerticalOptions = LayoutOptions.Center,
					Spacing = 20,
					Children =
				{
					convocatoriaImage,
					convocatoriaLabel
				}
				};

				gridCompetiton.Children.Add(convocatoriaStackLayout, 0, 7);
				Grid.SetColumnSpan(convocatoriaStackLayout, 2);

				var convocatoriaStackLayout_tap = new TapGestureRecognizer();
				convocatoriaStackLayout_tap.Tapped += (s, e) =>
				{
					Navigation.PushAsync(new CompetitionCallPageCS(competitions, null));
				};
				convocatoriaStackLayout.GestureRecognizers.Add(convocatoriaStackLayout_tap);

			}



			gridCompetiton.Children.Add(dateLabel, 0, 0);
			gridCompetiton.Children.Add(dateValue, 1, 0);

			gridCompetiton.Children.Add(placeLabel, 0, 1);
			gridCompetiton.Children.Add(placeValue, 1, 1);

			gridCompetiton.Children.Add(typeLabel, 0, 2);
			gridCompetiton.Children.Add(typeValue, 1, 2);

			gridCompetiton.Children.Add(valueLabel, 0, 3);
			gridCompetiton.Children.Add(valueValue, 1, 3);

			gridCompetiton.Children.Add(websiteLabel, 0, 4);
			gridCompetiton.Children.Add(websiteValue, 1, 4);

			gridCompetiton.Children.Add(estadoLabel, 0, 5);
			gridCompetiton.Children.Add(estadoValue, 1, 5);

			gridCompetiton.Children.Add(limitDateLabel, 0, 6);
			Grid.SetColumnSpan(limitDateLabel, 2);


			Image competitionImage = new Image { Aspect = Aspect.AspectFill, Opacity = 0.25 };
			Debug.Print("competition image = " + competition.imagemSource);
			competitionImage.Source = competition.imagemSource;

			relativeLayout.Children.Add(competitionImage,
				xConstraint: Constraint.Constant(0),
				yConstraint: Constraint.Constant(0),
				widthConstraint: Constraint.RelativeToParent((parent) =>
				{
					return (parent.Width); // center of image (which is 40 wide)
				}),
				heightConstraint: Constraint.RelativeToParent((parent) =>
				{
					return (parent.Height); // center of image (which is 40 wide)
				})
			);


			relativeLayout.Children.Add(gridCompetiton,
				xConstraint: Constraint.Constant(5),
				yConstraint: Constraint.Constant(5),
				widthConstraint: Constraint.RelativeToParent((parent) =>
				{
					return (parent.Width - 10); // center of image (which is 40 wide)
				}),
				heightConstraint: Constraint.RelativeToParent((parent) =>
				{
					return (parent.Height-10); // center of image (which is 40 wide)
				})
			);
		}



		public DetailCompetitionPageCS(Competition competition)
		{
			this.competition = competition;
			//Debug.Print("AQUI 2 competition ImageSource = " + competition.imagemSource);
			this.initLayout();
			//this.initSpecificLayout();
		}

		public DetailCompetitionPageCS(string competitionid)
		{
			this.competition = new Competition();
			competition.id = competitionid;
			this.initLayout();
			//this.initSpecificLayout();
		}

        public DetailCompetitionPageCS(string competitionid, string competitionparticipationid)
        {
            this.competition = new Competition();
            competition.id = competitionid;
            competition.participationid = competitionparticipationid;
            this.initLayout();
            //this.initSpecificLayout();
        }

        async void refreshCompetitionStatus(Competition competition)
		{
			if (competition != null)
			{
				Debug.Print("competition.participationid=" + competition.participationid);
				CompetitionManager competitionManager = new CompetitionManager();
				if (competition.participationid != null)

				{
					competitions = await competitionManager.GetCompetitionByParticipationID(App.member.id, competition.participationid);
					this.competition = competitions[0];

				}
				else
				{
					competitions = await competitionManager.GetCompetitionByID(App.member.id, competition.id);
					this.competition = competitions[0];
				}
				
				if ((competition.imagemNome == "") | (competition.imagemNome is null))
				{
					competition.imagemSource = "logo_aksl.png";
				}
				else
				{
					competition.imagemSource = Constants.images_URL + competition.id + "_imagem_c";
					Debug.Print("ANTES competition ImageSource = " + competition.imagemSource);
				}
			}
			initSpecificLayout();
		}

		async Task<List<Competition_Participation>> GetCompetitionCall()
		{
			Debug.WriteLine("GetCompetitionCall");
			CompetitionManager competitionManager = new CompetitionManager();

			List<Competition_Participation> futureCompetitionParticipations = await competitionManager.GetCompetitionCall(competition.id);
			if (futureCompetitionParticipations == null)
			{
								Application.Current.MainPage = new NavigationPage(new LoginPageCS("Verifique a sua ligação à Internet e tente novamente."))
				{
					BarBackgroundColor = Color.FromRgb(15, 15, 15),
					BarTextColor = Color.White
				};
				return null;
			}
			return futureCompetitionParticipations;
		}

		async void OnRegisterButtonClicked(object sender, EventArgs e)
		{

			registerButton.IsEnabled = false;

            bool hasQuotaPayed = false;

            if (App.member.currentFee != null)
            {
                if ((App.member.currentFee.estado == "fechado") | (App.member.currentFee.estado == "recebido") | (App.member.currentFee.estado == "confirmado"))
                {
                    hasQuotaPayed = true;
                }
            }

            if (hasQuotaPayed == true)
            {
                if (competition.participationconfirmed == "convocado")
                {
                    await Navigation.PushAsync(new CompetitionPaymentPageCS(competition));
                }
            }
            else
            {
                bool answer = await DisplayAlert("Inscrição Competição", "Para poderes efetuar a tua inscrição na Competição tens de ter a Quota Associativa em dia.", "Pagar Quota", "Cancelar");
                if (answer == true)
                {
                    await Navigation.PushAsync(new QuotasPageCS());
                }
            }

            

		}




	}
}

