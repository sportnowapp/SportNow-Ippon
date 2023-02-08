﻿using System;
using System.Collections.Generic;
using Xamarin.Forms;
using SportNow.Model;
using SportNow.Services.Data.JSON;
using System.Threading.Tasks;
using System.Diagnostics;
using Acr.UserDialogs;

namespace SportNow.Views
{
	public class QuotasPageCS : ContentPage
	{

		protected async override void OnAppearing()
		{
			initSpecificLayout();
		}

		protected override void OnDisappearing()
		{
			this.CleanScreen();
		}

		private RelativeLayout relativeLayout;

		private CollectionView collectionViewQuotas;

		private Member member;

		private Grid gridInactiveFee, gridActiveFee;

		Button activateButton;


		public void initLayout()
		{
			Title = "QUOTAS";
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

		public void CleanScreen()
		{
			Debug.Print("CleanScreen");
			//valida se os objetos já foram criados antes de os remover
			if (gridInactiveFee != null)
			{
				relativeLayout.Children.Remove(gridInactiveFee);
				gridInactiveFee = null;
			}
			if (gridActiveFee != null)
			{
				relativeLayout.Children.Remove(gridActiveFee);
				gridActiveFee = null;
			}
			

		}

		public async void initSpecificLayout()
		{

			member = App.member;

			var result = await GetCurrentFees(member);

			bool hasQuotaPayed = false;

			if (App.member.currentFee != null)
			{
				if ((App.member.currentFee.estado == "fechado") | (App.member.currentFee.estado == "recebido") | (App.member.currentFee.estado == "confirmado"))
				{
					hasQuotaPayed = true;
				}
			}

			if (hasQuotaPayed) {
				createActiveFeeLayout();
			}
			else {
				createInactiveFeeLayout();
			}
		}

		public void createInactiveFeeLayout() {
			gridInactiveFee = new Grid { Padding = 10, HorizontalOptions = LayoutOptions.FillAndExpand, VerticalOptions = LayoutOptions.FillAndExpand };
			gridInactiveFee.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
			gridInactiveFee.RowDefinitions.Add(new RowDefinition { Height = 100 });
			gridInactiveFee.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
			gridInactiveFee.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
			gridInactiveFee.RowDefinitions.Add(new RowDefinition { Height = GridLength.Star });
			gridInactiveFee.RowDefinitions.Add(new RowDefinition { Height = 50 });
			gridInactiveFee.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star }); //
			gridInactiveFee.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star }); //GridLength.Auto 

			Label feeYearLabel = new Label
			{
				Text = DateTime.Now.ToString("yyyy"),
				VerticalTextAlignment = TextAlignment.Center,
				HorizontalTextAlignment = TextAlignment.Center,
				TextColor = Color.White,
				LineBreakMode = LineBreakMode.NoWrap,
				FontSize = 50
			};

			Image akslLogoFee = new Image
			{
				Source = "logo_ikp_inactive.png",
				WidthRequest = 100
			};

			Image fnkpLogoFee = new Image
			{
				Source = "logo_fnkp_inactive.png",
				WidthRequest = 100
			};

			Label feeInactiveLabel = new Label
			{
				Text = "Quotas Inativas",
				VerticalTextAlignment = TextAlignment.Center,
				HorizontalTextAlignment = TextAlignment.Center,
				TextColor = Color.Red,
				LineBreakMode = LineBreakMode.NoWrap,
				FontSize = 40
			};

			Label feeInactiveCommentLabel = new Label
			{
				Text = "Atenção: Com as quotas inativas o aluno não poderá participar em eventos e poderá não ter acesso a seguro desportivo em caso de lesão (caso treine num dojo sem seguro desportivo).",
				VerticalTextAlignment = TextAlignment.Center,
				HorizontalTextAlignment = TextAlignment.Center,
				TextColor = Color.White,
				FontSize = 20
			};

			activateButton = new Button
			{
				Text = "ATIVAR",
				BackgroundColor = Color.FromRgb(96, 182, 89),
				TextColor = Color.White,
				WidthRequest = 100,
				HeightRequest = 50
			};
			activateButton.Clicked += OnActivateButtonClicked;


			Frame frame_activateButton = new Frame {
				BorderColor = Color.FromRgb(96, 182, 89),
				WidthRequest = 100,
				HeightRequest = 50,
				CornerRadius = 10,
				IsClippedToBounds = true,
				Padding = 0 };

			frame_activateButton.Content = activateButton;

			gridInactiveFee.Children.Add(feeYearLabel, 0, 0);
			Grid.SetColumnSpan(feeYearLabel, 2);

			gridInactiveFee.Children.Add(fnkpLogoFee, 0, 1);
			gridInactiveFee.Children.Add(akslLogoFee, 1, 1);

			gridInactiveFee.Children.Add(feeInactiveLabel, 0, 2);
			Grid.SetColumnSpan(feeInactiveLabel, 2);

			gridInactiveFee.Children.Add(feeInactiveCommentLabel, 0, 3);
			Grid.SetColumnSpan(feeInactiveCommentLabel, 2);

			gridInactiveFee.Children.Add(frame_activateButton, 0, 5);
			Grid.SetColumnSpan(frame_activateButton, 2);
			

			relativeLayout.Children.Add(gridInactiveFee,
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


		public void createActiveFeeLayout()
		{
			gridActiveFee = new Grid { Padding = 30, HorizontalOptions = LayoutOptions.FillAndExpand, VerticalOptions = LayoutOptions.FillAndExpand };
			gridActiveFee.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
			gridActiveFee.RowDefinitions.Add(new RowDefinition { Height = 100 });
			gridActiveFee.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
			gridActiveFee.RowDefinitions.Add(new RowDefinition { Height = GridLength.Star });
			gridActiveFee.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
			gridActiveFee.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star }); //GridLength.Auto
			gridActiveFee.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star }); //GridLength.Auto 

			Label feeYearLabel = new Label
			{
				Text = DateTime.Now.ToString("yyyy"),
				VerticalTextAlignment = TextAlignment.Center,
				HorizontalTextAlignment = TextAlignment.Center,
				TextColor = Color.White,
				LineBreakMode = LineBreakMode.NoWrap,
				FontSize = 50
			};

			Image akslLogoFee = new Image
			{
				Source = "logo_ikp.png",
				WidthRequest = 100
			};

			Image fnkpLogoFee = new Image
			{
				Source = "logo_fnkp.png",
				WidthRequest = 100

			};

			Label feeActiveLabel = new Label
			{
				Text = "Quotas Ativas",
				VerticalTextAlignment = TextAlignment.Center,
				HorizontalTextAlignment = TextAlignment.Center,
				TextColor = Color.FromRgb(96, 182, 89),
				LineBreakMode = LineBreakMode.NoWrap,
				FontSize = 40
			};

			Label feeActiveDueDateLabel = new Label
			{
				Text = "Válida até 31-12-"+DateTime.Now.ToString("yyyy"),
				VerticalTextAlignment = TextAlignment.Center,
				HorizontalTextAlignment = TextAlignment.Center,
				TextColor = Color.White,
				LineBreakMode = LineBreakMode.NoWrap,
				FontSize = 35
			};


			gridActiveFee.Children.Add(feeYearLabel, 0, 0);
			Grid.SetColumnSpan(feeYearLabel, 2);

			gridActiveFee.Children.Add(fnkpLogoFee, 0, 1);
			gridActiveFee.Children.Add(akslLogoFee, 1, 1);

			gridActiveFee.Children.Add(feeActiveLabel, 0, 2);
			Grid.SetColumnSpan(feeActiveLabel, 2);

			gridActiveFee.Children.Add(feeActiveDueDateLabel, 0, 3);
			Grid.SetColumnSpan(feeActiveDueDateLabel, 2);

			relativeLayout.Children.Add(gridActiveFee,
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

		public QuotasPageCS()
		{
			Debug.Print("App.member.dojo_faturavel = " + App.member.dojo_faturavel);
			Debug.Print("App.member.dojo_seguro = " + App.member.dojo_seguro);
			Debug.Print("App.member.member_type = " + App.member.member_type);

			this.initLayout();
			//this.initSpecificLayout();

		}

		async void OnPerfilButtonClicked(object sender, EventArgs e)
		{
			await Navigation.PushAsync(new ProfileCS());
		}

		async void OnActivateButtonClicked(object sender, EventArgs e)
		{

			Debug.Print("App.member.member_type = "+App.member.member_type);
			UserDialogs.Instance.ShowLoading("", MaskType.Clear);
			activateButton.IsEnabled = false;

			MemberManager memberManager = new MemberManager();


			if (App.member.currentFee is null) {

				var result_create = 0;
				
				result_create = await memberManager.CreateFee(member, DateTime.Now.ToString("yyyy"));
				if (result_create == -1)
				{
				Application.Current.MainPage = new NavigationPage(new LoginPageCS("Verifique a sua ligação à Internet e tente novamente."))
				{
					BarBackgroundColor = Color.FromRgb(15, 15, 15),
					BarTextColor = Color.White
				};
				}

				var result_get = await GetCurrentFees(member);
				if (result_create == -1)
				{
				Application.Current.MainPage = new NavigationPage(new LoginPageCS("Verifique a sua ligação à Internet e tente novamente."))
				{
					BarBackgroundColor = Color.FromRgb(15, 15, 15),
					BarTextColor = Color.White
				};
				}
			}
			
			await Navigation.PushAsync(new QuotasPaymentPageCS(member));
			UserDialogs.Instance.HideLoading();
		}
		

		async Task<int> GetCurrentFees(Member member)
		{
			Debug.WriteLine("GetCurrentFees");
			MemberManager memberManager = new MemberManager();

			var result = await memberManager.GetCurrentFees(member);
			if (result == -1)
			{
				Application.Current.MainPage = new NavigationPage(new LoginPageCS("Verifique a sua ligação à Internet e tente novamente."))
				{
					BarBackgroundColor = Color.FromRgb(15, 15, 15),
					BarTextColor = Color.White
				};
				return result;
			}
			return result;
		}

	}
}

