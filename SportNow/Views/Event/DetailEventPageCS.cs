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
	public class DetailEventPageCS : ContentPage
	{

		protected override void OnAppearing()
		{
			Debug.Print("DetailEventPageCS - OnAppearing");
			refreshEventStatus();
			
		}

		protected override void OnDisappearing()
		{
			this.CleanScreen();
		}


		FormValue estadoValue = new FormValue("");

		private RelativeLayout relativeLayout;

		private Event event_v;

		//private Event_Participation event_participation;

		private List<Payment> payments;

		Button registerButton;

		private Grid gridEvent;

		public void initLayout()
		{
			Title = event_v.name;
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
			Debug.Print("CleanScreen");
			//valida se os objetos já foram criados antes de os remover
			if (gridEvent != null)
			{
				relativeLayout.Children.Remove(gridEvent);
				gridEvent = null;
			}
			if (registerButton != null)
			{
				relativeLayout.Children.Remove(registerButton);
				registerButton = null;
			}

		}


		public async void initSpecificLayout()
		{
			gridEvent = new Grid { Padding = 0, HorizontalOptions = LayoutOptions.FillAndExpand };
			gridEvent.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
			gridEvent.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
			gridEvent.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
			gridEvent.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
			gridEvent.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
			gridEvent.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
			gridEvent.RowDefinitions.Add(new RowDefinition { Height = 60 * App.screenHeightAdapter });
			gridEvent.RowDefinitions.Add(new RowDefinition { Height = GridLength.Star });
			//gridGeral.RowDefinitions.Add(new RowDefinition { Height = 1 });
			gridEvent.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto }); //GridLength.Auto
			gridEvent.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star }); //GridLength.Auto 

			Label dateLabel = new FormLabel { Text = "DATA" };
			FormValue dateValue = new FormValue(event_v.detailed_date);

			FormLabel placeLabel = new FormLabel { Text = "LOCAL" };
			FormValue placeValue = new FormValue(event_v.place);

			FormLabel typeLabel = new FormLabel { Text = "TIPO" };
			FormValue typeValue = new FormValue(Constants.event_type[event_v.type]);

			FormLabel valueLabel = new FormLabel { Text = "VALOR" };
			FormValue valueValue = new FormValue(String.Format("{0:0.00}", event_v.value + "€"));

			FormLabel websiteLabel = new FormLabel { Text = "WEBSITE" };
			FormValue websiteValue = new FormValue(event_v.website);

			websiteValue.GestureRecognizers.Add(new TapGestureRecognizer
			{
				Command = new Command(async () => {
					try
					{
						await Browser.OpenAsync(event_v.website, BrowserLaunchMode.SystemPreferred);
					}
					catch (Exception ex)
					{
						// An unexpected error occured. No browser may be installed on the device.
					}
				})
			});

			FormLabel estadoLabel = new FormLabel { Text = "ESTADO" }; ;
			estadoValue = new FormValue("");

			DateTime currentTime = DateTime.Now.Date;
			DateTime registrationbegindate_datetime = new DateTime();
			DateTime registrationlimitdate_datetime = new DateTime();

			Debug.Print("event_v.registrationbegindate = " + event_v.registrationbegindate);

			if ((event_v.registrationbegindate != "") & (event_v.registrationbegindate != null))
			{	
				registrationbegindate_datetime = DateTime.Parse(event_v.registrationbegindate).Date;
			}	
			if ((event_v.registrationlimitdate != "") &(event_v.registrationlimitdate != null))
			{
				registrationlimitdate_datetime = DateTime.Parse(event_v.registrationlimitdate).Date;
			}
			

			bool registrationOpened = false;
			string limitDateLabelText = "";

			if ((event_v.registrationbegindate == "") | (event_v.registrationbegindate == null))
			{
				limitDateLabelText = "Este evento não tem inscrições ou as inscrições ainda não estão abertas.";
			}
			else if ((currentTime - registrationbegindate_datetime).Days < 0)
			{
				limitDateLabelText = "As inscrições abrem no dia " + event_v.registrationbegindate + ".";
			}
			else
			{
				if ((registrationlimitdate_datetime - currentTime).Days < 0)
				{
					limitDateLabelText = "Ohhh...As inscrições já terminaram.";
				}
				else
				{
					registrationOpened = true;
					limitDateLabelText = "As inscrições estão abertas e terminam no dia " + event_v.registrationlimitdate+". Contamos contigo!";
				}
			}

			registerButton = new Button();

			if (event_v.participationconfirmed == "inscrito")
			{
				estadoValue = new FormValue("INSCRITO");
				estadoValue.label.TextColor = Color.FromRgb(96, 182, 89);
				limitDateLabelText = "BOA SORTE!";

			}
			else if ((event_v.participationconfirmed == null) | (event_v.participationconfirmed == "nao_inscrito"))
			{
				estadoValue = new FormValue("NÃO INSCRITO");
				estadoValue.label.TextColor = Color.Red;

				if (registrationOpened == true)
                {

					//registerButton = new RoundButton("INSCREVER", 100, 40);

					registerButton = new Button
					{
						Text = "INSCREVER",
						BackgroundColor = Color.FromRgb(96, 182, 89),
						TextColor = Color.White,
						FontSize = App.itemTitleFontSize,
						WidthRequest = 100,
						HeightRequest = 50 * App.screenHeightAdapter,
						VerticalOptions = LayoutOptions.End
					};

					Frame frame_registerButton = new Frame
					{
						BackgroundColor = Color.Transparent,
						//BorderColor = Color.FromRgb(96, 182, 89),
						WidthRequest = 100,
						HeightRequest = 50 * App.screenHeightAdapter,
						CornerRadius = 10,
						IsClippedToBounds = true,
						VerticalOptions = LayoutOptions.End,
						Padding = 0
					};

					frame_registerButton.Content = registerButton;
					registerButton.Clicked += OnRegisterButtonClicked;

					

					/*gridEvent.RowDefinitions.Add(new RowDefinition { Height = GridLength.Star });
					gridEvent.Children.Add(frame_registerButton, 0, 7);
					Grid.SetColumnSpan(frame_registerButton, 2);*/
				}

				
			}

			Label limitDateLabel = new Label
			{
				Text = limitDateLabelText,
				TextColor = Color.FromRgb(246, 220, 178),
				WidthRequest = 200,
				HeightRequest = 50,
				FontSize = App.itemTitleFontSize,
				HorizontalTextAlignment = TextAlignment.Center,
				VerticalOptions = LayoutOptions.Center
			};


			Image eventoImage = new Image { Aspect = Aspect.AspectFill, Opacity = 0.25 };
			eventoImage.Source = event_v.imagemSource;

			relativeLayout.Children.Add(eventoImage,
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

			gridEvent.Children.Add(dateLabel, 0, 0);
			gridEvent.Children.Add(dateValue, 1, 0);

			gridEvent.Children.Add(placeLabel, 0, 1);
			gridEvent.Children.Add(placeValue, 1, 1);

			gridEvent.Children.Add(typeLabel, 0, 2);
			gridEvent.Children.Add(typeValue, 1, 2);

			gridEvent.Children.Add(valueLabel, 0, 3);
			gridEvent.Children.Add(valueValue, 1, 3);

			gridEvent.Children.Add(websiteLabel, 0, 4);
			gridEvent.Children.Add(websiteValue, 1, 4);

			gridEvent.Children.Add(estadoLabel, 0, 5);
			gridEvent.Children.Add(estadoValue, 1, 5);

			gridEvent.Children.Add(limitDateLabel, 0, 6);
			Grid.SetColumnSpan(limitDateLabel, 2);

			relativeLayout.Children.Add(gridEvent,
				xConstraint: Constraint.Constant(5),
				yConstraint: Constraint.Constant(5),
				widthConstraint: Constraint.RelativeToParent((parent) =>
				{
					return (parent.Width-10); // center of image (which is 40 wide)
				}),
				heightConstraint: Constraint.RelativeToParent((parent) =>
				{
					return (parent.Height-(60 * App.screenHeightAdapter)); // center of image (which is 40 wide)
				})
			);

			relativeLayout.Children.Add(registerButton,
						xConstraint: Constraint.Constant(10 * App.screenWidthAdapter),
						yConstraint: Constraint.RelativeToParent((parent) =>
						{
							return (parent.Height - (60 * App.screenHeightAdapter));
						}),
						widthConstraint: Constraint.RelativeToParent((parent) =>
						{
							return (parent.Width - (20 * App.screenWidthAdapter)); // center of image (which is 40 wide)
						}),
						heightConstraint: Constraint.Constant(50 * App.screenHeightAdapter)
					);

		}

		public DetailEventPageCS(Event event_v)
		{
			App.notification = App.notification + " DetailEventPageCS ";
			//UserDialogs.Instance.Alert(new AlertConfig() { Title = "", Message = App.notification, OkText = "Ok" });
			this.event_v = event_v;
			Debug.Print("event_v.id = " + event_v.id);
			//this.event_participation = event_participation;
			//App.event_participation = event_participation;

			this.initLayout();
			//this.initSpecificLayout();
		}

		async void refreshEventStatus()
		{
			if (event_v.participationid != null)
            {
				EventManager eventManager = new EventManager();
				Event_Participation event_participation = await eventManager.GetEventParticipation(event_v.participationid);
				if (event_participation == null)
				{
					Application.Current.MainPage = new NavigationPage(new LoginPageCS("Verifique a sua ligação à Internet e tente novamente."))
					{
						BarBackgroundColor = Color.FromRgb(15, 15, 15), BarTextColor = Color.White
					};
					return;
				}
				event_v.participationconfirmed = event_participation.estado;
				//Debug.Print("refreshEventStatus event_v.participationconfirmed=" + event_v.participationconfirmed);
			}
			initSpecificLayout();
		}


		async void OnRegisterButtonClicked(object sender, EventArgs e)
		{
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
                UserDialogs.Instance.ShowLoading("", MaskType.Clear);
                registerButton.IsEnabled = false;

                Event_Participation event_participation = new Event_Participation();

                EventManager eventManager = new EventManager();

                //O Event Participation ainda não existe, temos de criar. Caso contrário não é necessário
                if (event_v.participationconfirmed == null)
                {
                    string new_event_participationid = await eventManager.CreateEventParticipation(App.member.id, event_v.id);
                    if ((new_event_participationid != "0") & (new_event_participationid != "-1"))
                    {
                        event_participation = await eventManager.GetEventParticipation(new_event_participationid);
                    }

                    event_v.participationconfirmed = event_participation.estado;
                }
                else
                {
                    event_participation = await eventManager.GetEventParticipation(event_v.participationid);
                }

                if (event_participation != null)
                {
                    Debug.WriteLine("OnRegisterButtonClicked event_participation.name " + event_participation.name);
                    //await Navigation.PushAsync(new EventMBPageCS(event_participation));
                    await Navigation.PushAsync(new EventPaymentPageCS(event_v, event_participation));

                }

                UserDialogs.Instance.HideLoading();
            }
            else
            {
                bool answer = await DisplayAlert("Inscrição Evento", "Para poderes efetuar a tua inscrição no Evento tens de ter a Quota Associativa em dia.", "Pagar Quota", "Cancelar");
                if (answer == true)
                {
                    await Navigation.PushAsync(new QuotasPageCS());
                }
            }

            
			//registerButton.IsEnabled = true;
		}

		async Task<List<Event_Participation>> GetFutureEventParticipations()
		{
			Debug.WriteLine("GetFutureCompetitionParticipation");
			EventManager eventManager = new EventManager();

			List<Event_Participation> futureEventParticipations = await eventManager.GetFutureEventParticipations(App.member.id);

			if (futureEventParticipations == null)
			{
				Application.Current.MainPage = new NavigationPage(new LoginPageCS("Verifique a sua ligação à Internet e tente novamente."))
				{
					BarBackgroundColor = Color.FromRgb(15, 15, 15),
					BarTextColor = Color.White
				};
				return null;
			}
			return futureEventParticipations;
		}




	}
}

