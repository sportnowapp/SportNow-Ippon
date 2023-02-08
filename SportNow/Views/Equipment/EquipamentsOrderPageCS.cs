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
using Acr.UserDialogs;

namespace SportNow.Views
{
	public class EquipamentsOrderPageCS : ContentPage
	{

		protected override void OnAppearing()
		{
			//initSpecificLayout();
		}

		private RelativeLayout relativeLayout;

		Equipment equipment;

		public void initLayout()
		{
			Title = "ENCOMENDA EQUIPAMENTO";
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
		}


		public void CleanScreen()
		{

		}

		public async void initSpecificLayout()
		{
			CreateEquipmentView();
		}

		

		public void CreateEquipmentView()
		{
			

			Label subtypeLabel = new Label { BackgroundColor = Color.Transparent, VerticalTextAlignment = TextAlignment.Center, HorizontalTextAlignment = TextAlignment.Start, FontSize = App.itemTitleFontSize, TextColor = Color.FromRgb(246, 220, 178), LineBreakMode = LineBreakMode.WordWrap };
			subtypeLabel.Text = equipment.type + " - " + equipment.subtype;

			relativeLayout.Children.Add(subtypeLabel,
				xConstraint: Constraint.Constant(0),
				yConstraint: Constraint.Constant(0),
				widthConstraint: Constraint.RelativeToParent((parent) =>
				{
					return (parent.Width / 5 * 4) - (10 * App.screenHeightAdapter); // center of image (which is 40 wide)
				}),
				heightConstraint: Constraint.Constant(30 * App.screenHeightAdapter));

			Label valueTitleLabel = new Label { Text = "VALOR", BackgroundColor = Color.Transparent, VerticalTextAlignment = TextAlignment.Center, HorizontalTextAlignment = TextAlignment.Center, FontSize = App.itemTitleFontSize, TextColor = Color.FromRgb(246, 220, 178), LineBreakMode = LineBreakMode.WordWrap };

			relativeLayout.Children.Add(valueTitleLabel,
				xConstraint: Constraint.RelativeToParent((parent) =>
				{
					return (parent.Width / 5 * 4); // center of image (which is 40 wide)
				}),
				yConstraint: Constraint.Constant(0),
				widthConstraint: Constraint.RelativeToParent((parent) =>
				{
					return (parent.Width / 5) - 10; // center of image (which is 40 wide)
				}),
				heightConstraint: Constraint.RelativeToParent((parent) =>
				{
					return 30 * App.screenHeightAdapter; // 
				}));


			Label nameLabel = new Label { BackgroundColor = Color.Transparent, VerticalTextAlignment = TextAlignment.Center, HorizontalTextAlignment = TextAlignment.Start, FontSize = App.itemTitleFontSize, TextColor = Color.White, LineBreakMode = LineBreakMode.WordWrap };
			nameLabel.Text = equipment.name;

			Frame nameFrame = new Frame
			{
				BorderColor = Color.FromRgb(246, 220, 178),
				BackgroundColor = Color.Transparent,
				CornerRadius = 10,
				IsClippedToBounds = true,
				Padding = new Thickness(5, 0, 0, 0)
			};
			nameFrame.Content = nameLabel;

			relativeLayout.Children.Add(nameFrame,
				xConstraint: Constraint.Constant(0),
				yConstraint: Constraint.Constant(40 * App.screenHeightAdapter),
				widthConstraint: Constraint.RelativeToParent((parent) =>
				{
					return (parent.Width / 5 * 4) - 10; // center of image (which is 40 wide)
				}),
				heightConstraint: Constraint.RelativeToParent((parent) =>
				{
					return 30 * App.screenHeightAdapter; // 
			}));

			Label valueLabel = new Label { BackgroundColor = Color.Transparent, VerticalTextAlignment = TextAlignment.Center, HorizontalTextAlignment = TextAlignment.Center, FontSize = App.itemTitleFontSize, TextColor = Color.White, LineBreakMode = LineBreakMode.WordWrap };
			valueLabel.Text = equipment.valueFormatted;

			Frame valueFrame = new Frame
			{
				BorderColor = Color.FromRgb(246, 220, 178),
				BackgroundColor = Color.Transparent,
				CornerRadius = 10,
				IsClippedToBounds = true,
				Padding = new Thickness(5, 0, 0, 0)
			};
			valueFrame.Content = valueLabel;

			relativeLayout.Children.Add(valueFrame,
				xConstraint: Constraint.RelativeToParent((parent) =>
				{
					return (parent.Width / 5 * 4); // center of image (which is 40 wide)
				}),
				yConstraint: Constraint.Constant(40 * App.screenHeightAdapter),
				widthConstraint: Constraint.RelativeToParent((parent) =>
				{
					return (parent.Width / 5) - 10; // center of image (which is 40 wide)
				}),
				heightConstraint: Constraint.RelativeToParent((parent) =>
				{
					return 30 * App.screenHeightAdapter; // 
			}));

			RoundButton orderButton = new RoundButton("SOLICITAR EQUIPAMENTO", 100, 40);
			//Button orderButton = new Button { BackgroundColor = Color.Transparent, VerticalOptions = LayoutOptions.Center, HorizontalOptions= LayoutOptions.Center, FontSize = 20, TextColor = Color.White};
			orderButton.button.Clicked += OnOrderButtonClicked;

			relativeLayout.Children.Add(orderButton,
				xConstraint: Constraint.Constant(0),
				yConstraint: Constraint.Constant(140 * App.screenHeightAdapter),
				widthConstraint: Constraint.RelativeToParent((parent) =>
				{
					return (parent.Width); // center of image (which is 40 wide)
				}),
				heightConstraint: Constraint.RelativeToParent((parent) =>
				{
					return 50 * App.screenHeightAdapter; // 
				}));


			Label orderdescLabel = new Label { BackgroundColor = Color.Transparent, VerticalTextAlignment = TextAlignment.Center, HorizontalTextAlignment = TextAlignment.Start, FontSize = App.itemTextFontSize, TextColor = Color.White, LineBreakMode = LineBreakMode.WordWrap };
			orderdescLabel.Text = "Ao solicitar este equipamento iremos efetuar uma validação do Stock disponível e o responsável do teu dojo levar-lhe a sua encomenda com a maior brevidade possível.";

			relativeLayout.Children.Add(orderdescLabel,
				xConstraint: Constraint.Constant(0),
				yConstraint: Constraint.Constant(200 * App.screenHeightAdapter),
				widthConstraint: Constraint.RelativeToParent((parent) =>
				{
					return (parent.Width); // center of image (which is 40 wide)
				}),
				heightConstraint: Constraint.RelativeToParent((parent) =>
				{
					return 80 * App.screenHeightAdapter; // 
			}));


		}


		
		public EquipamentsOrderPageCS(Equipment equipment)
		{
			this.equipment = equipment;
			this.initLayout();
			initSpecificLayout();
		}

		async void OnOrderButtonClicked(object sender, EventArgs e)
		{
			UserDialogs.Instance.ShowLoading("", MaskType.Clear);
			Debug.WriteLine("OnOrderButtonClicked");
			EquipmentManager equipmentManager = new EquipmentManager();

			var result = await equipmentManager.CreateEquipmentOrder(App.member.id, App.member.name, equipment.id, equipment.type + " - " + equipment.subtype + " - " + equipment.name);
			if ((result == "-1") | (result == "-2"))
			{
				Application.Current.MainPage = new NavigationPage(new LoginPageCS("Verifique a sua ligação à Internet e tente novamente."))
				{
					BarBackgroundColor = Color.FromRgb(15, 15, 15),
					BarTextColor = Color.White
				};
			}
			UserDialogs.Instance.HideLoading();   //Hide loader
			UserDialogs.Instance.Alert(new AlertConfig() { Title = "EQUIPAMENTO SOLICITADO", Message = "A tua encomenda foi realizada com sucesso. Fala com o teu instrutor para saber quando te conseguirá entregar a mesma.", OkText = "Ok" });



//			UserDialogs.Instance.ShowLoading("", MaskType.Clear);
//			UserDialogs.Instance.HideLoading();   //Hide loader

		}
	}
}
