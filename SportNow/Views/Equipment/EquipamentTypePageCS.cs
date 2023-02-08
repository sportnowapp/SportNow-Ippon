using System;
using System.Collections.Generic;
using Xamarin.Forms;
using SportNow.Model;
using SportNow.Services.Data.JSON;
using System.Threading.Tasks;
using System.Diagnostics;
using SportNow.CustomViews;
using System.Collections.ObjectModel;
using Syncfusion.SfChart.XForms;
using SportNow.Model.Charts;

namespace SportNow.Views
{
	public class EquipamentTypePageCS : ContentPage
	{

		protected override void OnAppearing()
		{
			initSpecificLayout();
		}

		protected override void OnDisappearing()
		{
			this.CleanScreen();
		}

		private RelativeLayout equipamentosRelativeLayout;

		private RelativeLayout relativeLayout;
		private StackLayout stackButtons;

		private OptionButton karategiButton, protecoescintosButton, merchandisingButton;
	

		public void initLayout()
		{
			Title = "EQUIPAMENTOS";
			this.BackgroundColor = Color.FromRgb(25, 25, 25);
			NavigationPage.SetBackButtonTitle(this, "");

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

			var toolbarItem = new ToolbarItem
			{
				//Text = "Logout",
				IconImageSource = "perfil.png",

			};
			toolbarItem.Clicked += OnPerfilButtonClicked;
			ToolbarItems.Add(toolbarItem);

		}


		public void CleanScreen()
		{
			Debug.Print("CleanScreen");
			//valida se os objetos já foram criados antes de os remover
			if (stackButtons != null)
			{
				relativeLayout.Children.Remove(stackButtons);
				relativeLayout.Children.Remove(equipamentosRelativeLayout);

				stackButtons = null;
				equipamentosRelativeLayout = null;
			}

		}

		public async void initSpecificLayout()
		{
			CreateEquipamentos();
		}


		public void CreateEquipamentos()
		{
			equipamentosRelativeLayout = new RelativeLayout
			{
				Margin = new Thickness(5)
			};

			CreateEquipamentosOptionButtons();

			relativeLayout.Children.Add(equipamentosRelativeLayout,
				xConstraint: Constraint.Constant(0),
				yConstraint: Constraint.Constant(20),
				widthConstraint: Constraint.RelativeToParent((parent) =>
				{
					return (parent.Width);
				}),
				heightConstraint: Constraint.RelativeToParent((parent) =>
				{
					return (parent.Height) - 20;
				}));
		}

		public void CreateEquipamentosOptionButtons()
		{
			var width = Constants.ScreenWidth;
			var buttonWidth = (width) / 2;


			karategiButton = new OptionButton("KARATE GIs", "fotokarategis.png", buttonWidth, 60);
			//minhasGraduacoesButton.button.Clicked += OnMinhasGraduacoesButtonClicked;
			var karategiButton_tap = new TapGestureRecognizer();
			karategiButton_tap.Tapped += (s, e) =>
			{
				Navigation.PushAsync(new EquipamentsPageCS("karategis"));
			};
			karategiButton.GestureRecognizers.Add(karategiButton_tap);

			protecoescintosButton = new OptionButton("PROTEÇÕES E CINTOS", "fotoprotecoescintos.png", buttonWidth, 60);
			var protecoescintosButton_tap = new TapGestureRecognizer();
			protecoescintosButton_tap.Tapped += (s, e) =>
			{
				Navigation.PushAsync(new EquipamentsPageCS("protecoescintos"));
			};
			protecoescintosButton.GestureRecognizers.Add(protecoescintosButton_tap);

			merchandisingButton = new OptionButton("MERCHANDISING", "fotomerchandisingaksl.png", buttonWidth, 60);
			var merchandisingButton_tap = new TapGestureRecognizer();
			merchandisingButton_tap.Tapped += (s, e) =>
			{
				Navigation.PushAsync(new EquipamentsPageCS("merchandising"));
			};
			merchandisingButton.GestureRecognizers.Add(merchandisingButton_tap);


			StackLayout stackEquipamentosButtons = new StackLayout
			{
				//WidthRequest = 370,
				Margin = new Thickness(0),
				Spacing = 50,
				Orientation = StackOrientation.Vertical,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.FillAndExpand,
				HeightRequest = 350,
				Children =
				{
					karategiButton,
					protecoescintosButton,
					merchandisingButton,
				}
			};

			equipamentosRelativeLayout.Children.Add(stackEquipamentosButtons,
			xConstraint: Constraint.RelativeToParent((parent) =>
			{
				return (parent.Width / 4);
			}),
			yConstraint: Constraint.Constant(0),
			widthConstraint: Constraint.RelativeToParent((parent) =>
			{
				return (parent.Width/2);
			}),
			heightConstraint: Constraint.Constant(400));
		}




		public EquipamentTypePageCS()
		{

			this.initLayout();
			//this.initSpecificLayout();

		}

		async void OnPerfilButtonClicked(object sender, EventArgs e)
		{
			await Navigation.PushAsync(new ProfileCS());
		}

		async Task<List<Event_Participation>> GetPastEventParticipations()
		{
			Debug.WriteLine("GetPastCompetitionParticipations");
			EventManager eventManager = new EventManager();

			List<Event_Participation> pastEventParticipations = await eventManager.GetPastEventParticipations(App.member.id);
			if (pastEventParticipations == null)
			{
								Application.Current.MainPage = new NavigationPage(new LoginPageCS("Verifique a sua ligação à Internet e tente novamente."))
				{
					BarBackgroundColor = Color.FromRgb(15, 15, 15),
					BarTextColor = Color.White
				};
				return null;
			}
			return pastEventParticipations;
		}

	}
}
