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
	public class MonthFeeListPageCS : ContentPage
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

		private CollectionView monthFeesCollectionView;

		public Label currentMonth;

		public StackLayout stackMonthSelector;

		Image estadoQuotaImage;

		DateTime selectedTime;

		ObservableCollection<MonthFee> monthFees;

		Picker dojoPicker;

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

			relativeLayout.Children.Remove(monthFeesCollectionView);
			monthFeesCollectionView = null;
		}

		public async void initSpecificLayout()
		{
			UserDialogs.Instance.ShowLoading("", MaskType.Clear);

			CreateMonthSelector();
			_ = await CreateDojoPicker();
			monthFees = await GetMonthFeesbyDojo();
			CreateMonthFeesColletion();

			UserDialogs.Instance.HideLoading();   //Hide loader
		}


		public async Task<int> CreateDojoPicker()
		{
			Debug.Print("CreateDojoPicker");
			List<string> dojoList = new List<string>();
			List<Dojo> dojos = await GetAllDojos();
			int selectedIndex = 0;
			int selectedIndex_temp = 0;

			foreach (Dojo dojo in dojos)
			{
				dojoList.Add(dojo.name);
				if (dojo.name == App.member.dojo)
				{
					selectedIndex = selectedIndex_temp;
				}
				selectedIndex_temp++;
			}

			dojoPicker = new Picker
			{
				Title = "",
				TitleColor = Color.White,
				BackgroundColor = Color.Transparent,
				TextColor = Color.FromRgb(246, 220, 178),
				HorizontalTextAlignment = TextAlignment.Center,
				FontSize = App.titleFontSize

			};
			dojoPicker.ItemsSource = dojoList;
			dojoPicker.SelectedIndex = selectedIndex;

			dojoPicker.SelectedIndexChanged += async (object sender, EventArgs e) =>
			{

				UserDialogs.Instance.ShowLoading("", MaskType.Clear);

				Debug.Print("DojoPicker selectedItem = " + dojoPicker.SelectedItem.ToString());

				monthFees = await GetMonthFeesbyDojo();

				monthFeesCollectionView.ItemsSource = monthFees;

				/*students = await GetStudentsDojo(dojoPicker.SelectedItem.ToString());
				relativeLayout.Children.Remove(collectionViewStudents);
				collectionViewStudents = null;
				CreateStudentsColletion();*/

				UserDialogs.Instance.HideLoading();

			};



			relativeLayout.Children.Add(dojoPicker,
				xConstraint: Constraint.Constant(0),
				yConstraint: Constraint.Constant(0 * App.screenHeightAdapter),
				widthConstraint: Constraint.RelativeToParent((parent) =>
				{
					return (parent.Width); // center of image (which is 40 wide)
				}),
				heightConstraint: Constraint.Constant(50));

			return 1;
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
				Text = selectedTime.Year + " - " + selectedTime.Month,
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
								new Label { Text = "Não existem Mensalidades deste Dojo para este mês.", HorizontalTextAlignment = TextAlignment.Center, TextColor = Color.White, FontSize = 20 },
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

				Label nameLabel = new Label { BackgroundColor = Color.Transparent, VerticalTextAlignment = TextAlignment.Center, HorizontalTextAlignment = TextAlignment.Start, FontSize = App.titleFontSize, TextColor = Color.White, LineBreakMode = LineBreakMode.WordWrap };
				nameLabel.SetBinding(Label.TextProperty, "membernickname");

				itemRelativeLayout.Children.Add(nameLabel,
					xConstraint: Constraint.Constant(5),
					yConstraint: Constraint.Constant(0),
					widthConstraint: Constraint.RelativeToParent((parent) =>
					{
						return (parent.Width - (280 * App.screenWidthAdapter)); // center of image (which is 40 wide)
					}),
					heightConstraint: Constraint.Constant(30 * App.screenHeightAdapter));

				Label valueLabel = new Label { BackgroundColor = Color.Transparent, VerticalTextAlignment = TextAlignment.Center, HorizontalTextAlignment = TextAlignment.Start, FontSize = App.titleFontSize, TextColor = Color.White, LineBreakMode = LineBreakMode.WordWrap };
				valueLabel.SetBinding(Label.TextProperty, "value");

				itemRelativeLayout.Children.Add(valueLabel,
					xConstraint: Constraint.RelativeToParent((parent) =>
					{
						return (parent.Width - (215 * App.screenWidthAdapter)); // center of image (which is 40 wide)
					}),
					yConstraint: Constraint.Constant(0),
					widthConstraint: Constraint.Constant(60 * App.screenWidthAdapter),
					heightConstraint: Constraint.Constant(30 * App.screenHeightAdapter));

				Label statusLabel = new Label { BackgroundColor = Color.Transparent, VerticalTextAlignment = TextAlignment.Center, HorizontalTextAlignment = TextAlignment.Start, FontSize = App.titleFontSize, TextColor = Color.White, LineBreakMode = LineBreakMode.WordWrap };
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

		public void createApproveButtons()
		{
			/*approveSelectedButton = new Button
			{
				Text = "APROVAR SELECCIONADOS",
				BackgroundColor = Color.FromRgb(96, 182, 89),
				TextColor = Color.White,
				FontSize = App.itemTitleFontSize,
				WidthRequest = 100,
				HeightRequest = 50
			};

			Frame frame_approveSelectedButton = new Frame
			{
				BorderColor = Color.FromRgb(96, 182, 89),
				WidthRequest = 100,
				HeightRequest = 50,
				CornerRadius = 10,
				IsClippedToBounds = true,
				Padding = 0
			};

			frame_approveSelectedButton.Content = approveSelectedButton;
			approveSelectedButton.Clicked += approveSelectedButtonClicked;

			relativeLayout.Children.Add(frame_approveSelectedButton,
				xConstraint: Constraint.Constant(0),
				yConstraint: Constraint.RelativeToParent((parent) =>
				{
					return (parent.Height) - 60; // 
				}),
				widthConstraint: Constraint.RelativeToParent((parent) =>
				{
					return (parent.Width/2)-5; // center of image (which is 40 wide)
				}),
				heightConstraint: Constraint.Constant(50)
			);*/

			approveAllButton = new Button
			{
				Text = "APROVAR TODOS",
				BackgroundColor = Color.FromRgb(96, 182, 89),
				TextColor = Color.White,
				FontSize = App.itemTitleFontSize,
				WidthRequest = 100,
				HeightRequest = 50
			};

			Frame frame_approveAllButton = new Frame
			{
				BorderColor = Color.FromRgb(96, 182, 89),
				WidthRequest = 100,
				HeightRequest = 50,
				CornerRadius = 10,
				IsClippedToBounds = true,
				Padding = 0
			};

			frame_approveAllButton.Content = approveAllButton;
			approveAllButton.Clicked += approveAllButtonClicked;

			relativeLayout.Children.Add(frame_approveAllButton,
				xConstraint: Constraint.Constant(5),
				yConstraint: Constraint.RelativeToParent((parent) =>
				{
					return (parent.Height) - 60; // 
				}),
				widthConstraint: Constraint.RelativeToParent((parent) =>
				{
					return (parent.Width) - 10; // center of image (which is 40 wide)
				}),
				heightConstraint: Constraint.Constant(50)
			);

		}

		public MonthFeeListPageCS()
		{

			this.initLayout();
			//this.initSpecificLayout();

		}

		async Task<List<Dojo>> GetAllDojos()
		{
			DojoManager dojoManager = new DojoManager();
			List<Dojo> dojos = await dojoManager.GetAllDojos();

			return dojos;
		}


		async void OnPreviousButtonClicked(object sender, EventArgs e)
		{
			UserDialogs.Instance.ShowLoading("", MaskType.Clear);
			selectedTime = selectedTime.AddMonths(-1);
			currentMonth.Text = selectedTime.Year + " - " + selectedTime.Month;

			monthFees = await GetMonthFeesbyDojo();

			monthFeesCollectionView.ItemsSource = monthFees;
			
			UserDialogs.Instance.HideLoading();   //Hide loader
		}

		async void OnNextButtonClicked(object sender, EventArgs e)
		{
			UserDialogs.Instance.ShowLoading("", MaskType.Clear);
			Debug.Print("selectedTime antes = " + selectedTime.ToShortDateString());
			selectedTime = selectedTime.AddMonths(1);
			Debug.Print("selectedTime = " + selectedTime.ToShortDateString());
			currentMonth.Text = selectedTime.Year + " - " + selectedTime.Month;
			monthFees = await GetMonthFeesbyDojo();

			monthFeesCollectionView.ItemsSource = monthFees;

			
			UserDialogs.Instance.HideLoading();   //Hide loader


		}

		async Task<ObservableCollection<MonthFee>> GetMonthFeesbyDojo()
		{
			Debug.WriteLine("GetMonthFeesbyDojo " + dojoPicker.SelectedItem.ToString());
			MonthFeeManager monthFeeManager = new MonthFeeManager();
			ObservableCollection<MonthFee> monthFees = await monthFeeManager.GetMonthFeesbyDojo(dojoPicker.SelectedItem.ToString(), selectedTime.Year.ToString(), selectedTime.Month.ToString());
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
			bool hasMonthFeesToApprove = false;

			Debug.Print("correctMonthFees");

			foreach (MonthFee monthFee in monthFees)
			{
				monthFee.selected = false;
				monthFee.selectedColor = Color.White;
				Debug.Print("monthFee.status = " + monthFee.status);

				if (monthFee.status == "por_aprovar")
				{
					monthFee.selectedColor = Color.LightYellow;
					monthFee.status = "Por Aprovar";
					hasMonthFeesToApprove = true;
				}
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

			if (hasMonthFeesToApprove == true)
			{
				createApproveButtons();
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
				if (selectedMonthFee.status == "Por Aprovar")
				{
					changeMonthFeeValuePrompt(selectedMonthFee);
				}
				else if ((selectedMonthFee.status == "Em pagamento") | (selectedMonthFee.status == "Pagamento em Atraso") | (selectedMonthFee.status == "Emitida"))
				{
					changeMonthFeeStatusPrompt(selectedMonthFee);
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


		async void changeMonthFeeValuePrompt(MonthFee monthFee)
		{
			UserDialogs.Instance.ShowLoading("", MaskType.Clear);
			var promptConfig = new PromptConfig();
			promptConfig.InputType = InputType.Default;
			promptConfig.Text = monthFee.value;
			promptConfig.IsCancellable = true;
			promptConfig.Message = "Indica o valor da mensalidade a aplicar a este aluno";
			promptConfig.Title = "Mensalidade";
			promptConfig.OkText = "Ok";
			promptConfig.CancelText = "Cancelar";
			var input = await UserDialogs.Instance.PromptAsync(promptConfig);

			if (input.Ok)
			{
				string new_value = input.Text;
				var charsToRemove = new string[] { "$", "€"};
				foreach (var c in charsToRemove)
				{
					new_value = new_value.Replace(c, string.Empty);
				}

				Debug.Print("O Valor da Mensalidade é " + new_value);


				MonthFeeManager monthFeeManager = new MonthFeeManager();
				int i = await monthFeeManager.Update_MonthFee_Value_byID(monthFee.id, new_value);
				monthFee.value = input.Text;
				relativeLayout.Children.Remove(monthFeesCollectionView);
				monthFeesCollectionView = null;
				CreateMonthFeesColletion();
				/*string global_evaluation = await UpdateExamination_Result(examination_Result.id, input.Text);
				examination_Result.description = input.Text;*/
			}

			UserDialogs.Instance.HideLoading();
		}

		async void changeMonthFeeStatusPrompt(MonthFee monthFee)
		{
			UserDialogs.Instance.ShowLoading("", MaskType.Clear);
			var result = await UserDialogs.Instance.ConfirmAsync("Confirmas que pretendes colocar esta mensalidade como paga?", "Confirmar Pagamento", "Sim", "Não");
			if (result)
			{
				MonthFeeManager monthFeeManager = new MonthFeeManager();
				int i = await monthFeeManager.Update_MonthFee_Status_byID(monthFee.id, "paga");
				monthFee.selectedColor = Color.LightGreen;
				monthFee.status = "Paga";
				relativeLayout.Children.Remove(monthFeesCollectionView);
				monthFeesCollectionView = null;
				CreateMonthFeesColletion();
			}
			else
			{
			}

			UserDialogs.Instance.HideLoading();
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

		async void approveSelectedButtonClicked(object sender, EventArgs e)
		{
			Debug.Print("approveSelectedButtonClicked");
			UserDialogs.Instance.ShowLoading("", MaskType.Clear);
			
			//approveSelectedButton.IsEnabled = false;
			//approveAllButton.IsEnabled = false;


			int selectedMonthFeeCount = 0;

			foreach (MonthFee monthFee in monthFees)
			{
				if (monthFee.selected == true)
				{
					selectedMonthFeeCount++;
					MonthFeeManager monthFeeManager = new MonthFeeManager();
					//int i = await monthFeeManager.Update_MonthFee_Status_byID(monthFee.id, "emitida_a_pagamento");
                    int i = await monthFeeManager.Update_MonthFee_Status_byID(monthFee.id, "emitida");
                }
			}

			if (selectedMonthFeeCount == 0)
			{
				UserDialogs.Instance.Alert(new AlertConfig() { Title = "ESCOLHA VAZIA", Message = "Tens de escolher pelo menos uma mensalidade para aprovar.", OkText = "Ok" });
			}

			//approveSelectedButton.IsEnabled = true;
			//approveAllButton.IsEnabled = true;

			UserDialogs.Instance.HideLoading();
		}

		async void approveAllButtonClicked(object sender, EventArgs e)
		{
			Debug.Print("approveAllButtonClicked");
			UserDialogs.Instance.ShowLoading("", MaskType.Clear);


			foreach (MonthFee monthFee in monthFees)
			{
				Debug.Print("monthFee "+ monthFee.name + " status = "+ monthFee.status);
				if (monthFee.status == "Por Aprovar")
				{
					MonthFeeManager monthFeeManager = new MonthFeeManager();

					int currentMonth = Convert.ToInt32(DateTime.Now.Month.ToString());
					int currentYear = Convert.ToInt32(DateTime.Now.Year.ToString());

					if (((Convert.ToInt32(monthFee.year) == currentYear) & (Convert.ToInt32(monthFee.month) < currentMonth)) | (Convert.ToInt32(monthFee.year) < currentYear))
					{
                        //int i = await monthFeeManager.Update_MonthFee_Status_byID(monthFee.id, "emitida_pagamento_em_atraso");
                        //monthFee.status = "Pagamento em Atraso";
                        
						int i = await monthFeeManager.Update_MonthFee_Status_byID(monthFee.id, "emitida");
                        monthFee.status = "Emitida";
                        monthFee.selectedColor = Color.IndianRed;
                    }
					else
					{
						/*int i = await monthFeeManager.Update_MonthFee_Status_byID(monthFee.id, "emitida_a_pagamento");
						monthFee.status = "Em pagamento";*/
                        int i = await monthFeeManager.Update_MonthFee_Status_byID(monthFee.id, "emitida");
                        monthFee.status = "Emitida";
                        monthFee.selectedColor = Color.LightBlue;
					}
				}
			}
			relativeLayout.Children.Remove(monthFeesCollectionView);
			monthFeesCollectionView = null;
			CreateMonthFeesColletion();


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
