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
	public class MonthFeeStudentListPageCS : ContentPage
	{

		protected override void OnAppearing()
		{
			initSpecificLayout();
		}

		protected override void OnDisappearing()
		{
			this.CleanScreen();
		}
		private RelativeLayout quotasRelativeLayout;

		private RelativeLayout relativeLayout;
		private StackLayout stackButtons;

		private CollectionView monthFeesCollectionView;

		public Label currentMonth;

		public StackLayout stackMonthSelector;

		Image estadoQuotaImage;

		DateTime selectedTime;

		ObservableCollection<MonthFee> monthFees;

		Button approveSelectedButton, approveAllButton;

		public void initLayout()
		{
			Title = "MENSALIDADES";
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
			if (quotasRelativeLayout != null)
			{
				relativeLayout.Children.Remove(quotasRelativeLayout);
				monthFeesCollectionView = null;
			}
			if (stackMonthSelector != null)
			{
				relativeLayout.Children.Remove(stackMonthSelector);
				stackMonthSelector = null;
			}
		}

		public async void initSpecificLayout()
		{
			UserDialogs.Instance.ShowLoading("", MaskType.Clear);

			CreateMonthSelector();
			monthFees = await GetMonthFeesbyStudent();
			CreateMonthFeesColletion();

			UserDialogs.Instance.HideLoading();   //Hide loader
		}

		public void CreateMonthSelector()
		{
			var width = Constants.ScreenWidth;
			var buttonWidth = (width - 50) / 3;

			//DateTime currentTime = DateTime.Now;
			selectedTime = DateTime.Now;



			Button previousMonthButton = new Button();
			Button nextMonthButton = new Button();

			if (Device.RuntimePlatform == Device.iOS)
			{
				previousMonthButton = new Button()
				{
					Text = "<",
					FontSize = App.titleFontSize,
					TextColor = Color.FromRgb(246, 220, 178),
					VerticalOptions = LayoutOptions.Center
				};
			}
			else if (Device.RuntimePlatform == Device.Android)
			{
				previousMonthButton = new Button()
				{
					Text = "<",
					FontSize = App.titleFontSize,
					TextColor = Color.FromRgb(246, 220, 178),
					BackgroundColor = Color.FromRgb(25, 25, 25),
					VerticalOptions = LayoutOptions.Center
				};
			}


			previousMonthButton.Clicked += OnPreviousButtonClicked;

			currentMonth = new Label()
			{
				Text = selectedTime.Year.ToString(),
				FontSize = App.titleFontSize,
				TextColor = Color.FromRgb(246, 220, 178),
				WidthRequest = 150,
				VerticalTextAlignment = TextAlignment.Center,
				HorizontalTextAlignment = TextAlignment.Center
			};

			if (Device.RuntimePlatform == Device.iOS)
			{
				nextMonthButton = new Button()
				{
					Text = ">",
					FontSize = App.titleFontSize,
					TextColor = Color.FromRgb(246, 220, 178),
					VerticalOptions = LayoutOptions.Center
				};
			}
			else if (Device.RuntimePlatform == Device.Android)
			{
				nextMonthButton = new Button()
				{
					Text = ">",
					FontSize = App.titleFontSize,
					TextColor = Color.FromRgb(246, 220, 178),
					BackgroundColor = Color.FromRgb(25, 25, 25),
					VerticalOptions = LayoutOptions.Center
				};
			}

			nextMonthButton.Clicked += OnNextButtonClicked;

			stackMonthSelector = new StackLayout
			{
				//WidthRequest = 370,
				Margin = new Thickness(0),
				Spacing = 5 * App.screenHeightAdapter,
				Orientation = StackOrientation.Horizontal,
				HorizontalOptions = LayoutOptions.Center,
				VerticalOptions = LayoutOptions.FillAndExpand,
				HeightRequest = 40 * App.screenHeightAdapter,
				Children =
				{
					previousMonthButton,
					currentMonth,
					nextMonthButton
				}
			};

			relativeLayout.Children.Add(stackMonthSelector,
			xConstraint: Constraint.Constant(0),
			yConstraint: Constraint.Constant(40 * App.screenHeightAdapter),
			widthConstraint: Constraint.RelativeToParent((parent) =>
			{
				return (parent.Width);
			}),
			heightConstraint: Constraint.Constant(40 * App.screenHeightAdapter));

		}

		public void CreateMonthFeesColletion()
		{
			//COLLECTION GRADUACOES
			monthFeesCollectionView = new CollectionView
			{
				SelectionMode = SelectionMode.Single,
				ItemsSource = monthFees,
				ItemsLayout = new GridItemsLayout(1, ItemsLayoutOrientation.Vertical) { VerticalItemSpacing = 5, HorizontalItemSpacing = 5, },
				EmptyView = new ContentView
				{
					Content = new StackLayout
					{
						Children =
							{
								new Label { Text = "Não existem Mensalidades relativas a este ano.", HorizontalTextAlignment = TextAlignment.Center, TextColor = Color.White, FontSize = 20 },
							}
					}
				}
			};

			monthFeesCollectionView.SelectionChanged += OncollectionViewMonthFeesSelectionChangedAsync;

			monthFeesCollectionView.ItemTemplate = new DataTemplate(() =>
			{
				RelativeLayout itemRelativeLayout = new RelativeLayout
				{
					HeightRequest = 100
				};

				Frame itemFrame = new Frame
				{
					CornerRadius = 5,
					IsClippedToBounds = true,
					BorderColor = Color.FromRgb(182, 145, 89),
					BackgroundColor = Color.Transparent,
					Padding = new Thickness(2, 2, 2, 2),
					HeightRequest = 30 * App.screenHeightAdapter,
					VerticalOptions = LayoutOptions.Center,
				};

				itemFrame.Content = itemRelativeLayout;

				Label nameLabel = new Label { BackgroundColor = Color.Transparent, VerticalTextAlignment = TextAlignment.Center, HorizontalTextAlignment = TextAlignment.Start, FontSize = App.itemTitleFontSize, TextColor = Color.White, LineBreakMode = LineBreakMode.WordWrap };
				nameLabel.SetBinding(Label.TextProperty, "membernickname");

				itemRelativeLayout.Children.Add(nameLabel,
					xConstraint: Constraint.Constant(5),
					yConstraint: Constraint.Constant(0),
					widthConstraint: Constraint.RelativeToParent((parent) =>
					{
						return (parent.Width - (255 * App.screenWidthAdapter)); // center of image (which is 40 wide)
					}),
					heightConstraint: Constraint.Constant(30 * App.screenHeightAdapter));

				Label monthLabel = new Label { BackgroundColor = Color.Transparent, VerticalTextAlignment = TextAlignment.Center, HorizontalTextAlignment = TextAlignment.Start, FontSize = App.itemTitleFontSize, TextColor = Color.White, LineBreakMode = LineBreakMode.WordWrap };
				monthLabel.SetBinding(Label.TextProperty, "month");

				itemRelativeLayout.Children.Add(monthLabel,
					xConstraint: Constraint.RelativeToParent((parent) =>
					{
						return (parent.Width - (250 * App.screenWidthAdapter)); // center of image (which is 40 wide)
					}),
					yConstraint: Constraint.Constant(0),
					widthConstraint: Constraint.Constant(40 * App.screenWidthAdapter),
					heightConstraint: Constraint.Constant(30 * App.screenHeightAdapter));

				Label valueLabel = new Label { BackgroundColor = Color.Transparent, VerticalTextAlignment = TextAlignment.Center, HorizontalTextAlignment = TextAlignment.Start, FontSize = App.itemTitleFontSize, TextColor = Color.White, LineBreakMode = LineBreakMode.WordWrap };
				valueLabel.SetBinding(Label.TextProperty, "value");

				itemRelativeLayout.Children.Add(valueLabel,
					xConstraint: Constraint.RelativeToParent((parent) =>
					{
						return (parent.Width - (205 * App.screenWidthAdapter)); // center of image (which is 40 wide)
					}),
					yConstraint: Constraint.Constant(0),
					widthConstraint: Constraint.Constant(50 * App.screenWidthAdapter),
					heightConstraint: Constraint.Constant(30 * App.screenHeightAdapter));

				Label statusLabel = new Label { BackgroundColor = Color.Transparent, VerticalTextAlignment = TextAlignment.Center, HorizontalTextAlignment = TextAlignment.Start, FontSize = App.itemTitleFontSize, TextColor = Color.White, LineBreakMode = LineBreakMode.WordWrap };
				statusLabel.SetBinding(Label.TextProperty, "status");
				statusLabel.SetBinding(Label.TextColorProperty, "selectedColor");

				itemRelativeLayout.Children.Add(statusLabel,
					xConstraint: Constraint.RelativeToParent((parent) =>
					{
						return (parent.Width - (150 * App.screenWidthAdapter)); // center of image (which is 40 wide)
					}),
					yConstraint: Constraint.Constant(0),
					widthConstraint: Constraint.Constant(150 * App.screenWidthAdapter),
					heightConstraint: Constraint.Constant(30 * App.screenHeightAdapter));

				return itemFrame;
			});

			relativeLayout.Children.Add(monthFeesCollectionView,
				xConstraint: Constraint.Constant(0),
				yConstraint: Constraint.Constant(100 * App.screenHeightAdapter),
				widthConstraint: Constraint.RelativeToParent((parent) =>
				{
					return (parent.Width); // center of image (which is 40 wide)
				}),
				heightConstraint: Constraint.RelativeToParent((parent) =>
				{
					return (parent.Height) - (170 * App.screenHeightAdapter); // 
				}));


		}

		public MonthFeeStudentListPageCS()
		{

			this.initLayout();
			//this.initSpecificLayout();

		}


		async void OnPreviousButtonClicked(object sender, EventArgs e)
		{
			UserDialogs.Instance.ShowLoading("", MaskType.Clear);
			selectedTime = selectedTime.AddYears(-1);
			currentMonth.Text = selectedTime.Year.ToString();

			monthFees = await GetMonthFeesbyStudent();

			monthFeesCollectionView.ItemsSource = monthFees;
			
			UserDialogs.Instance.HideLoading();   //Hide loader
		}

		async void OnNextButtonClicked(object sender, EventArgs e)
		{
			UserDialogs.Instance.ShowLoading("", MaskType.Clear);
			Debug.Print("selectedTime antes = " + selectedTime.ToShortDateString());
			selectedTime = selectedTime.AddYears(1);
			Debug.Print("selectedTime = " + selectedTime.ToShortDateString());
			currentMonth.Text = selectedTime.Year.ToString();
			monthFees = await GetMonthFeesbyStudent();

			monthFeesCollectionView.ItemsSource = monthFees;

			
			UserDialogs.Instance.HideLoading();   //Hide loader


		}

		async Task<ObservableCollection<MonthFee>> GetMonthFeesbyStudent()
		{
			MonthFeeManager monthFeeManager = new MonthFeeManager();
			ObservableCollection<MonthFee> monthFees = await monthFeeManager.GetMonthFeesbyStudent(App.member.id, selectedTime.Year.ToString());
			if (monthFees == null)
			{
				Application.Current.MainPage = new NavigationPage(new LoginPageCS("Verifique a sua ligação à Internet e tente novamente."))
				{
					BarBackgroundColor = Color.FromRgb(15, 15, 15),
					BarTextColor = Color.White
				};
				return null;
			}

			monthFees = correctMonthFees(monthFees);

			return monthFees;
		}


		public ObservableCollection<MonthFee> correctMonthFees(ObservableCollection<MonthFee> monthFees)
		{

			foreach (MonthFee monthFee in monthFees)
			{
				monthFee.selected = false;
				monthFee.selectedColor = Color.White;

                if (monthFee.status == "emitida")
                {
                    monthFee.selectedColor = Color.LightBlue;
                    monthFee.status = "Emitida";
                }
				if (monthFee.status == "emitida_a_pagamento")
				{
					monthFee.selectedColor = Color.LightBlue;
					monthFee.status = "Em pagamento";
				}
				else if (monthFee.status == "emitida_pagamento_em_atraso")
				{
					monthFee.selectedColor = Color.IndianRed;
					monthFee.status = "Pagamento em Atraso";
				}
				else if (monthFee.status == "paga")
				{
					monthFee.selectedColor = Color.LightGreen;
					monthFee.status = "Paga";
				}
			}

			return monthFees;
		}

		void OncollectionViewMonthFeesSelectionChangedAsync(object sender, SelectionChangedEventArgs e)
		{
			Debug.Print("OncollectionViewMonthFeesSelectionChanged");

			if ((sender as CollectionView).SelectedItem != null)
			{
				MonthFee selectedMonthFee = (sender as CollectionView).SelectedItem as MonthFee;

				Debug.WriteLine("OncollectionViewExaminationSessionCallSelectionChanged selected item = " + selectedMonthFee.membernickname);
				
				if ((selectedMonthFee.status == "Em pagamento") | (selectedMonthFee.status == "Pagamento em Atraso") | (selectedMonthFee.status == "Emitida"))
				{
					payMonthFee(selectedMonthFee);
				}
				else if (selectedMonthFee.status == "Paga")
				{
					InvoiceDocument(selectedMonthFee);
				}
				(sender as CollectionView).SelectedItem = null;
			}
			else
			{
				Debug.WriteLine("OncollectionViewMonthFeesSelectionChanged selected item = nulll");
			}
		}

		public async void InvoiceDocument(MonthFee monthFee)
		{
			Debug.Print("InvoiceDocument");
			Payment payment = await GetMonthFeePayment(monthFee);
			if (payment.invoiceid != null)
			{
				Debug.Print("InvoiceDocument != null");
				if (payment.value != 0)
                {
					Debug.Print("InvoiceDocument != 0");
					await Navigation.PushAsync(new InvoiceDocumentPageCS(payment));
				}
					
			}
		}

		public async Task<bool> checkPreviusUnpaidMonthFeeAsync(MonthFee selectedMonthFee)
		{
			Debug.Print("checkPreviusUnPaidMonthFee");
			foreach (MonthFee monthFee in monthFees)
			{
				Debug.Print("selectedMonthFee = " + selectedMonthFee.name + " " + selectedMonthFee.status + " " + selectedMonthFee.year + " " + selectedMonthFee.month);
				Debug.Print("MonthFee = " + monthFee.name + " " + monthFee.status + " " + monthFee.year + " " + monthFee.month);
				if (((monthFee.status == "Em pagamento") | (monthFee.status == "Pagamento em Atraso") | (monthFee.status == "Emitida"))
					& ((Convert.ToInt32(selectedMonthFee.year) == Convert.ToInt32(monthFee.year)) & (Convert.ToInt32(selectedMonthFee.month) > Convert.ToInt32(monthFee.month)))
						| (Convert.ToInt32(selectedMonthFee.year) > Convert.ToInt32(monthFee.year)))
                {
					Debug.Print("Tem mensalidades anteriores sem pagamento!");

					var result = await UserDialogs.Instance.ConfirmAsync("Tens mensalidades anteriores em atraso. Confirmas que queres pagar esta mensalidade?", "MENSALIDADES EM ATRASO", "Sim", "Não");
					return result;
					/*if (result)
					{
						return true;
					}
					else
					{
						return false;
					}*/
				}
			}
			return true;
		}
		

		async void payMonthFee(MonthFee monthFee)
		{
			Debug.Print("payMonthFee");

			bool unpaid = await checkPreviusUnpaidMonthFeeAsync(monthFee);
			if (unpaid == true)
			{
				UserDialogs.Instance.ShowLoading("", MaskType.Clear);
				await Navigation.PushAsync(new MonthFeePaymentPageCS(monthFee));
				UserDialogs.Instance.HideLoading();
			}
		}

		async void changeMonthFeeStatusPrompt(MonthFee monthFee)
		{
			UserDialogs.Instance.ShowLoading("", MaskType.Clear);
			var result = await UserDialogs.Instance.ConfirmAsync("Confirmas que pretendes colocar esta mensalidade como paga?", "Confirmar Pagamento", "Sim", "Não");
			if (result)
			{
				MonthFeeManager monthFeeManager = new MonthFeeManager();
				int i = await monthFeeManager.Update_MonthFee_Status_byID(monthFee.id, "paga");
				monthFee.status = "Paga";
				monthFeesCollectionView.ItemsSource = null;
				monthFeesCollectionView.ItemsSource = monthFees;
			}
			else
			{
			}

			UserDialogs.Instance.HideLoading();
		}

		public async Task<Payment> GetMonthFeePayment(MonthFee monthFee)
		{
			Debug.WriteLine("GetMonthFeePayment");
			MonthFeeManager monthFeeManager = new MonthFeeManager();

			List<Payment> result = await monthFeeManager.GetMonthFee_Payment(monthFee.id);
			if (result == null)
			{
				Application.Current.MainPage = new NavigationPage(new LoginPageCS("Verifique a sua ligação à Internet e tente novamente."))
				{
					BarBackgroundColor = Color.FromRgb(15, 15, 15),
					BarTextColor = Color.White
				};
				return result[0];
			}
			return result[0];
		}
	}
}
