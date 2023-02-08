﻿using System;
using System.Collections.Generic;
using Xamarin.Forms;
using SportNow.Model;
using SportNow.Services.Data.JSON;
using System.Threading.Tasks;
using System.Diagnostics;


namespace SportNow.Views
{
	public class EventMBPageCS : ContentPage
	{

		protected override void OnDisappearing()
		{
		}

		private RelativeLayout relativeLayout;

		private Event_Participation event_participation;

		private List<Payment> payments;

		private Grid gridMBPayment;

		public void initLayout()
		{
			Title = "INSCRIÇÃO";
			this.BackgroundColor = Color.FromRgb(25, 25, 25);

			relativeLayout = new RelativeLayout
			{
				Margin = new Thickness(20)
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


		public async void initSpecificLayout()
		{

			payments = await GetEventParticipationPayment(event_participation);

			if ((payments == null) | (payments.Count == 0))
			{
				createRegistrationConfirmed();
			}
			else {
				createMBPaymentLayout();
			}
		}

		public async void createRegistrationConfirmed()
		{
			Label inscricaoOKLabel = new Label
			{
                Text = "A tua Inscrição no Evento \n " + event_participation.evento_name + " \n está Confirmada. \n\n BOA SORTE\n e nunca te esqueças de te divertir!",
				VerticalTextAlignment = TextAlignment.Center,
				HorizontalTextAlignment = TextAlignment.Center,
				TextColor = Color.White,
				//LineBreakMode = LineBreakMode.NoWrap,
				HeightRequest = 300,
				FontSize = 30
			};

			relativeLayout.Children.Add(inscricaoOKLabel,
				xConstraint: Constraint.Constant(0),
				yConstraint: Constraint.Constant(10),
				widthConstraint: Constraint.RelativeToParent((parent) =>
				{
					return (parent.Width); // center of image (which is 40 wide)
				}),
				heightConstraint: Constraint.Constant(300)
			);

			EventManager eventManager = new EventManager();

			await eventManager.Update_Event_Participation_Status(event_participation.id, "inscrito");
			event_participation.estado = "inscrito";

		}

		public void createMBPaymentLayout() {
			gridMBPayment= new Grid { Padding = 10, HorizontalOptions = LayoutOptions.FillAndExpand, VerticalOptions = LayoutOptions.FillAndExpand };
			gridMBPayment.RowDefinitions.Add(new RowDefinition { Height = 100 });
			gridMBPayment.RowDefinitions.Add(new RowDefinition { Height = 20 });
			gridMBPayment.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
			gridMBPayment.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star }); //GridLength.Auto
			gridMBPayment.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star }); //GridLength.Auto 

			Label competitionParticipationNameLabel = new Label
			{
				Text = "Para confirmares a tua presença no\n " + event_participation.evento_name + "\n efetua o pagamento no MB com os dados apresentados em baixo",
				VerticalTextAlignment = TextAlignment.Center,
				HorizontalTextAlignment = TextAlignment.Center,
				TextColor = Color.White,
				//LineBreakMode = LineBreakMode.NoWrap,
				FontSize = App.bigTitleFontSize
			};

			Image MBLogoImage = new Image
			{
				Source = "logomultibanco.png",
				WidthRequest = 184 * App.screenHeightAdapter,
				HeightRequest = 115 * App.screenHeightAdapter

			};

			Label referenciaMBLabel = new Label
			{
				Text = "Pagamento por\n Multibanco",
				VerticalTextAlignment = TextAlignment.Center,
				HorizontalTextAlignment = TextAlignment.Center,
				TextColor = Color.White,
				//LineBreakMode = LineBreakMode.NoWrap,
				HeightRequest = 115 * App.screenHeightAdapter,
				FontSize = App.bigTitleFontSize
			};

			/*gridMBDataPayment.Children.Add(referenceLabel, 0, 1);
			gridMBDataPayment.Children.Add(valueLabel, 0, 2);
			*/
			gridMBPayment.Children.Add(competitionParticipationNameLabel, 0, 0);
			Grid.SetColumnSpan(competitionParticipationNameLabel, 2);

			gridMBPayment.Children.Add(MBLogoImage, 0, 2);
			gridMBPayment.Children.Add(referenciaMBLabel, 1, 2);

			createMBGrid(payments[0]);

			relativeLayout.Children.Add(gridMBPayment,
				xConstraint: Constraint.Constant(0),
				yConstraint: Constraint.Constant(10),
				widthConstraint: Constraint.RelativeToParent((parent) =>
				{
					return (parent.Width); // center of image (which is 40 wide)
				}),
				heightConstraint: Constraint.RelativeToParent((parent) =>
				{
					return (parent.Height) - 10; // center of image (which is 40 wide)
				})
			);
		}

		public void createMBGrid(Payment payment)
		{
			Grid gridMBDataPayment = new Grid { Padding = 10, HorizontalOptions = LayoutOptions.FillAndExpand, VerticalOptions = LayoutOptions.FillAndExpand };
			gridMBDataPayment.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
			gridMBDataPayment.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
			gridMBDataPayment.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
			gridMBDataPayment.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star }); //GridLength.Auto
			gridMBDataPayment.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star }); //GridLength.Auto

			Label entityLabel = new Label
			{
				Text = "Entidade:",
				VerticalTextAlignment = TextAlignment.Center,
				HorizontalTextAlignment = TextAlignment.Start,
				TextColor = Color.White,
				FontSize = App.titleFontSize
			};
			Label referenceLabel = new Label
			{
				Text = "Referência:",
				VerticalTextAlignment = TextAlignment.Center,
				HorizontalTextAlignment = TextAlignment.Start,
				TextColor = Color.White,
				FontSize = App.titleFontSize
			};
			Label valueLabel = new Label
			{
				Text = "Valor:",
				VerticalTextAlignment = TextAlignment.Center,
				HorizontalTextAlignment = TextAlignment.Start,
				TextColor = Color.White,
				FontSize = App.titleFontSize
			};

			Label entityValue = new Label
			{
				Text = payments[0].entity,
				VerticalTextAlignment = TextAlignment.Center,
				HorizontalTextAlignment = TextAlignment.End,
				TextColor = Color.White,
				FontSize = App.titleFontSize
			};
			Label referenceValue = new Label
			{
				Text = payments[0].reference,
				VerticalTextAlignment = TextAlignment.Center,
				HorizontalTextAlignment = TextAlignment.End,
				TextColor = Color.White,
				FontSize = App.titleFontSize
			};
			Label valueValue = new Label
			{
				Text = String.Format("{0:0.00}", payments[0].value) + "€",
				VerticalTextAlignment = TextAlignment.Center,
				HorizontalTextAlignment = TextAlignment.End,
				TextColor = Color.White,
				FontSize = App.titleFontSize
			};

			Frame MBDataFrame = new Frame { BackgroundColor = Color.FromRgb(25, 25, 25), BorderColor = Color.Yellow, CornerRadius = 10, IsClippedToBounds = true, Padding = 0 };
			MBDataFrame.Content = gridMBDataPayment;

			gridMBDataPayment.Children.Add(entityLabel, 0, 0);
			gridMBDataPayment.Children.Add(entityValue, 1, 0);
			gridMBDataPayment.Children.Add(referenceLabel, 0, 1);
			gridMBDataPayment.Children.Add(referenceValue, 1, 1);
			gridMBDataPayment.Children.Add(valueLabel, 0, 2);
			gridMBDataPayment.Children.Add(valueValue, 1, 2);

			gridMBPayment.RowDefinitions.Add(new RowDefinition { Height = 20 });
			gridMBPayment.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

			gridMBPayment.Children.Add(MBDataFrame, 0, 4);
			Grid.SetColumnSpan(MBDataFrame, 2);
		}


		public EventMBPageCS(Event_Participation event_participation)
		{

			this.event_participation = event_participation;
			//App.event_participation = event_participation;

			this.initLayout();
			this.initSpecificLayout();

		}

		async Task<List<Payment>> GetEventParticipationPayment(Event_Participation event_participation)
		{
			Debug.WriteLine("GetCompetitionParticipationPayment");
			EventManager eventManager = new EventManager();

			List<Payment> payments = await eventManager.GetEventParticipation_Payment(event_participation.id);
			if (payments == null)
			{
								Application.Current.MainPage = new NavigationPage(new LoginPageCS("Verifique a sua ligação à Internet e tente novamente."))
				{
					BarBackgroundColor = Color.FromRgb(15, 15, 15),
					BarTextColor = Color.White
				};
				return null;
			}
			return payments;
		}

	}
}

