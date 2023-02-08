using System;
using System.Collections.Generic;
using Xamarin.Forms;
using SportNow.Model;
using SportNow.Services.Data.JSON;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Collections.ObjectModel;
using System.Globalization;
using Acr.UserDialogs;
using System.Linq;
using SportNow.ViewModel;
using Xamarin.Essentials;
using Plugin.DeviceOrientation;
using Plugin.DeviceOrientation.Abstractions;
//using Plugin.FirebaseAnalytics;

namespace SportNow.Views
{
	public class MainPageCS : ContentPage
	{

		protected async override void OnAppearing()
		{
			/*base.OnAppearing();
			CrossDeviceOrientation.Current.LockOrientation(DeviceOrientations.Portrait);*/


			var mainDisplayInfo = DeviceDisplay.MainDisplayInfo;

			Constants.ScreenWidth = mainDisplayInfo.Width;
			Constants.ScreenHeight = mainDisplayInfo.Height;
			//Debug.Print("AQUI 1 - ScreenWidth = " + Constants.ScreenWidth + " ScreenHeight = " + Constants.ScreenHeight + "mainDisplayInfo.Density = " + mainDisplayInfo.Density);

			Constants.ScreenWidth = Application.Current.MainPage.Width;
			Constants.ScreenHeight = Application.Current.MainPage.Height;
			//Debug.Print("AQUI 0 - ScreenWidth = " + Constants.ScreenWidth + " ScreenHeight = " + Constants.ScreenHeight);

			App.AdaptScreen();

			initSpecificLayout();
		}

		protected override void OnDisappearing()
		{
			this.CleanScreen();
		}

		
		private RelativeLayout relativeLayout;

		public List<MainMenuItem> MainMenuItems { get; set; }

		private Member member;

		Label msg;
		Button btn;

		private ObservableCollection<Class_Schedule> cleanClass_Schedule, importantClass_Schedule;

		private List<Class_Schedule> teacherClass_Schedules;

		private CollectionView importantClassesCollectionView;
		private CollectionView importantEventsCollectionView;
		private CollectionView teacherClassesCollectionView;

		ScheduleCollection scheduleCollection;

		Label usernameLabel, attendanceLabel, eventsLabel, teacherClassesLabel;
		Label currentFeeLabel;
		Label famousQuoteLabel;
		Label currentVersionLabel;

		private List<Event> importantEvents;
		private List<Competition> importantCompetitions;
		private List<Examination_Session> importantExaminationSessions;


		int classesY = 0;
		int eventsY = 0;
		int teacherClassesY = 0;
		int eventsHeight = 0;
		int feesOrQuoteY = 0;

		public void CleanScreen()
		{
			//Debug.Print("CleanScreen");
			//valida se os objetos já foram criados antes de os remover
			if (usernameLabel != null)
			{
				relativeLayout.Children.Remove(usernameLabel);
				usernameLabel = null;
			}

			if (importantClassesCollectionView != null)
			{
				//relativeLayout.Children.Clear();
				
				relativeLayout.Children.Remove(importantClassesCollectionView);
				relativeLayout.Children.Remove(attendanceLabel);
				relativeLayout.Children.Remove(importantEventsCollectionView);
				relativeLayout.Children.Remove(eventsLabel);

				importantClassesCollectionView = null;
				importantEventsCollectionView = null;
				attendanceLabel = null;
				eventsLabel = null;
			}
			if (teacherClassesCollectionView != null) {

				relativeLayout.Children.Remove(teacherClassesCollectionView);
				relativeLayout.Children.Remove(teacherClassesLabel);
				teacherClassesCollectionView = null;
				teacherClassesLabel = null;
			}
			if (currentFeeLabel != null)
			{
				relativeLayout.Children.Remove(currentFeeLabel);
				currentFeeLabel = null;
			}
			if (currentVersionLabel != null)
			{
				relativeLayout.Children.Remove(currentVersionLabel);
				currentVersionLabel = null;
			}

			if (famousQuoteLabel != null)
			{
				relativeLayout.Children.Remove(famousQuoteLabel);
				famousQuoteLabel = null;
			}

		}

		public void initLayout()
		{
			Title = "PRINCIPAL";
			this.BackgroundColor = Color.FromRgb(25, 25, 25);


			relativeLayout = new RelativeLayout
			{
				Margin = new Thickness(20)
			};
			ScrollView scrollView = new ScrollView { Content = relativeLayout, Orientation = ScrollOrientation.Vertical };

			Content = scrollView;

			relativeLayout.Children.Add(new Image
			{
				Source = "boneco_karate.png"
			},
			xConstraint: Constraint.Constant(0),
			yConstraint: Constraint.Constant(-13),
			widthConstraint: Constraint.RelativeToParent((parent) => { return parent.Width; }),
			heightConstraint: Constraint.RelativeToParent((parent) => { return parent.Height; }));

			var toolbarItem = new ToolbarItem
			{
				//Text = "Logout",
				IconImageSource = "perfil.png"

			};
			toolbarItem.Clicked += OnPerfilButtonClicked;
			ToolbarItems.Add(toolbarItem);

		}


		public async void initSpecificLayout()
		{
			UserDialogs.Instance.ShowLoading("", MaskType.Clear);

			var textWelcome = "";

			textWelcome = "Olá " + App.member.nickname;

			//USERNAME LABEL
			usernameLabel = new Label
			{
				Text = textWelcome,
				TextColor = Color.White,
				HorizontalTextAlignment = TextAlignment.End,
				FontSize = 18 * App.screenWidthAdapter
			};
			relativeLayout.Children.Add(usernameLabel,
			xConstraint: Constraint.RelativeToParent((parent) =>
			{
				return (parent.Width) - (300 * App.screenWidthAdapter); // center of image (which is 40 wide)
			}),
			yConstraint: Constraint.Constant(2 * App.screenHeightAdapter),
			widthConstraint: Constraint.Constant(300 * App.screenWidthAdapter),
			heightConstraint: Constraint.Constant(30 * App.screenHeightAdapter));




			if (App.member.students_count > 0)
            {
				teacherClassesY = (int) (40 * App.screenHeightAdapter);
				classesY = (int) ((teacherClassesY + App.ItemHeight ) + (50 * App.screenHeightAdapter));
				eventsY = (int) (classesY + App.ItemHeight  + (50 * App.screenHeightAdapter));
				eventsHeight = (int)(App.ItemHeight  + 10);
				feesOrQuoteY = (int)((eventsY + eventsHeight) + (50 * App.screenHeightAdapter));
			}
			else if (App.member.students_count == 0)
			{
				classesY = (int) (40 * App.screenHeightAdapter);
				eventsY = (int) ((classesY + App.ItemHeight  + 50) * App.screenHeightAdapter);
				eventsHeight = (int)(2 * (App.ItemHeight  + 10));
				feesOrQuoteY = (int) ((eventsY + eventsHeight) + (50 * App.screenHeightAdapter));
			}


			createImportantClasses();

			createImportantEvents();

			Debug.Print("App.member.students_count = " + App.member.students_count);
			if (App.member.students_count > 0)
			{
				createImportantTeacherClasses();
			}

			createCurrentFee();

			//createVersion();

			UserDialogs.Instance.HideLoading();   //Hide loader
		}


		public async void createImportantTeacherClasses()
		{
			DateTime currentTime = DateTime.Now.Date;
			DateTime currentTime_add7 = DateTime.Now.AddDays(7).Date;

			string firstDay = currentTime.ToString("yyyy-MM-dd");
			string lastday = currentTime_add7.AddDays(6).ToString("yyyy-MM-dd");

			teacherClass_Schedules = await GetAllClass_Schedules(firstDay, lastday);
			CompleteTeacherClass_Schedules();

			//AULAS LABEL
			teacherClassesLabel = new Label
			{
				Text = "PRÓXIMAS AULAS COMO INSTRUTOR/MONITOR",
				TextColor = Color.FromRgb(246, 220, 178),
				HorizontalTextAlignment = TextAlignment.Start,
				FontSize = App.titleFontSize
			};
			relativeLayout.Children.Add(teacherClassesLabel,
			xConstraint: Constraint.Constant(0),
			yConstraint: Constraint.Constant(teacherClassesY),
			widthConstraint: Constraint.RelativeToParent((parent) =>
			{
				return (parent.Width); // center of image (which is 40 wide)
			}),
			heightConstraint: Constraint.Constant(30 * App.screenWidthAdapter));

			CreateTeacherClassesColletion();
		}

		public void CompleteTeacherClass_Schedules()
		{
			foreach (Class_Schedule class_schedule in teacherClass_Schedules)
			{
				DateTime class_schedule_date = DateTime.Parse(class_schedule.date).Date;

				class_schedule.datestring = Constants.daysofWeekPT[class_schedule_date.DayOfWeek.ToString()] + " - "
					+ class_schedule_date.Day + " "
					+ Constants.months[class_schedule_date.Month] + "\n"
					+ class_schedule.begintime + " às " + class_schedule.endtime;

				if (class_schedule.imagesource == null)
				{
					class_schedule.imagesourceObject = "logo_aksl.png";
				}
				else
				{
					class_schedule.imagesourceObject = new UriImageSource
					{
						Uri = new Uri(Constants.images_URL + class_schedule.classid + "_imagem_c"),
						CachingEnabled = true,
						CacheValidity = new TimeSpan(5, 0, 0, 0)
					};
				}
			}
		}


		public void CreateTeacherClassesColletion()
		{
			//COLLECTION TEACHER CLASSES
			teacherClassesCollectionView = new CollectionView
			{
				SelectionMode = SelectionMode.Single,
				ItemsSource = teacherClass_Schedules,
				ItemsLayout = new GridItemsLayout(1, ItemsLayoutOrientation.Horizontal) { VerticalItemSpacing = 5 * App.screenHeightAdapter, HorizontalItemSpacing = 5 * App.screenWidthAdapter, },
				EmptyView = new ContentView
				{
					Content = new StackLayout
					{
						Children =
							{
								new Label { Text = "Não existem Aulas agendadas nesta semana.", HorizontalTextAlignment = TextAlignment.Center, TextColor = Color.White, FontSize = App.itemTitleFontSize },
							}
					}
				}
			};

			teacherClassesCollectionView.SelectionChanged += OnTeacherClassesCollectionViewSelectionChanged;

			teacherClassesCollectionView.ItemTemplate = new DataTemplate(() =>
			{
				RelativeLayout itemRelativeLayout = new RelativeLayout
				{
					HeightRequest = App.ItemHeight ,
					WidthRequest = App.ItemWidth
				};

				Debug.Print("App.ItemHeight  = " + (App.ItemHeight  - 10) * App.screenHeightAdapter);

				Frame itemFrame = new Frame
				{
					CornerRadius = 5 * (float)App.screenWidthAdapter,
					IsClippedToBounds = true,
					BorderColor = Color.FromRgb(182, 145, 89),
					BackgroundColor = Color.Transparent,
					Padding = new Thickness(0, 0, 0, 0),
					HeightRequest = App.ItemHeight,
					VerticalOptions = LayoutOptions.Center,
				};

				Image eventoImage = new Image { Aspect = Aspect.AspectFill, Opacity = 0.5 }; //, HeightRequest = 60, WidthRequest = 60
				eventoImage.SetBinding(Image.SourceProperty, "imagesourceObject");

				itemFrame.Content = eventoImage;

				/*var itemFrame_tap = new TapGestureRecognizer();
				itemFrame_tap.Tapped += (s, e) =>
				{
					Navigation.PushAsync(new EquipamentsPageCS("protecoescintos"));
				};
				itemFrame.GestureRecognizers.Add(itemFrame_tap);*/

				itemRelativeLayout.Children.Add(itemFrame,
					xConstraint: Constraint.Constant(0),
					yConstraint: Constraint.Constant(0),
					widthConstraint: Constraint.RelativeToParent((parent) =>
					{
						return (parent.Width);
					}),
					heightConstraint: Constraint.RelativeToParent((parent) =>
					{
						return (parent.Height);
					}));

				Label dateLabel = new Label { VerticalTextAlignment = TextAlignment.Center, HorizontalTextAlignment = TextAlignment.Center, FontSize = App.itemTextFontSize, TextColor = Color.White, LineBreakMode = LineBreakMode.WordWrap };
				dateLabel.SetBinding(Label.TextProperty, "datestring");

				itemRelativeLayout.Children.Add(dateLabel,
					xConstraint: Constraint.Constant(25 * App.screenWidthAdapter),
					yConstraint: Constraint.Constant(App.ItemHeight - (45 * App.screenHeightAdapter)),
					widthConstraint: Constraint.RelativeToParent((parent) =>
					{
						return (parent.Width - (50 * App.screenWidthAdapter)); // center of image (which is 40 wide)
					}),
					heightConstraint: Constraint.Constant((40 * App.screenHeightAdapter)));

				Label nameLabel = new Label { BackgroundColor = Color.Transparent, VerticalTextAlignment = TextAlignment.Start, HorizontalTextAlignment = TextAlignment.Center, FontSize = App.itemTitleFontSize, TextColor = Color.White, LineBreakMode = LineBreakMode.WordWrap };
				nameLabel.SetBinding(Label.TextProperty, "name");

				itemRelativeLayout.Children.Add(nameLabel,
					xConstraint: Constraint.Constant(3 * App.screenWidthAdapter),
					yConstraint: Constraint.Constant(25 * App.screenHeightAdapter),
					widthConstraint: Constraint.RelativeToParent((parent) =>
					{
						return (parent.Width - (6 * App.screenWidthAdapter));
					}),
					heightConstraint: Constraint.Constant(50 * App.screenHeightAdapter));

				Image participationImagem = new Image { Aspect = Aspect.AspectFill }; //, HeightRequest = 60, WidthRequest = 60
				participationImagem.SetBinding(Image.SourceProperty, "participationimage");

				itemRelativeLayout.Children.Add(participationImagem,
					xConstraint: Constraint.RelativeToParent((parent) =>
					{
						return (parent.Width - (25 * App.screenHeightAdapter));
					}),
					yConstraint: Constraint.Constant(5),
					widthConstraint: Constraint.Constant((20 * App.screenHeightAdapter)),
					heightConstraint: Constraint.Constant((20 * App.screenHeightAdapter)));

				return itemRelativeLayout;
			});

			//teacherClassesCollectionView.ScrollTo(5);

			relativeLayout.Children.Add(teacherClassesCollectionView,
				xConstraint: Constraint.Constant(0),
				yConstraint: Constraint.Constant(teacherClassesY + (30 * App.screenHeightAdapter)),
				widthConstraint: Constraint.RelativeToParent((parent) =>
				{
					return (parent.Width); // center of image (which is 40 wide)
				}),
				heightConstraint: Constraint.Constant(App.ItemHeight  + (10 * App.screenHeightAdapter)));

			Debug.Print("(App.ItemHeight  + 10) * App.screenHeightAdapter) = " + (App.ItemHeight  + 10) * App.screenHeightAdapter);
		}

		public async void createImportantClasses()
		{
			int result = await getClass_DetailData();

			//AULAS LABEL
			attendanceLabel = new Label
			{
				Text = "PRÓXIMAS AULAS COMO ALUNO(A)",
				TextColor = Color.FromRgb(246, 220, 178),
				HorizontalTextAlignment = TextAlignment.Start,
				FontSize = App.titleFontSize
			};
			relativeLayout.Children.Add(attendanceLabel,
			xConstraint: Constraint.Constant(0),
			yConstraint: Constraint.Constant(classesY),
			widthConstraint: Constraint.RelativeToParent((parent) =>
			{
				return (parent.Width); // center of image (which is 40 wide)
			}),
			heightConstraint: Constraint.Constant(30 * App.screenHeightAdapter));


			scheduleCollection = new ScheduleCollection();
			scheduleCollection.Items = importantClass_Schedule;
			createClassesCollection();
		}

		public async Task<int> getClass_DetailData()
		{
			DateTime currentTime = DateTime.Now.Date;
			DateTime firstDayWeek = currentTime.AddDays(-Constants.daysofWeekInt[currentTime.DayOfWeek.ToString()]);

			importantClass_Schedule = await GetStudentClass_Schedules(currentTime.ToString("yyyy-MM-dd"), currentTime.AddDays(7).ToString("yyyy-MM-dd"));//  new List<Class_Schedule>();
			cleanClass_Schedule = new ObservableCollection<Class_Schedule>();

			CompleteClass_Schedules();

			return 1;
		}


		public void CompleteClass_Schedules()
		{
			foreach (Class_Schedule class_schedule in importantClass_Schedule)
			{
				/*if (class_schedule.classattendancestatus == "confirmada")
				{
					class_schedule.participationimage = "iconcheck.png";
				}*/
				DateTime class_schedule_date = DateTime.Parse(class_schedule.date).Date;

				class_schedule.datestring = Constants.daysofWeekPT[class_schedule_date.DayOfWeek.ToString()] + " - "
					+ class_schedule_date.Day + " "
					+ Constants.months[class_schedule_date.Month] + "\n"
					+ class_schedule.begintime + " às " + class_schedule.endtime;
				if (class_schedule.imagesource == null)
				{
					class_schedule.imagesourceObject = "logo_aksl.png";
				}
				else
				{
					class_schedule.imagesourceObject = new UriImageSource
					{
							Uri = new Uri(Constants.images_URL + class_schedule.classid + "_imagem_c"),
							CachingEnabled = true,
							CacheValidity = new TimeSpan(5, 0, 0, 0)
					};
				}
				

				if ((class_schedule.classattendancestatus == "confirmada") | (class_schedule.classattendancestatus == "fechada"))
				{
					class_schedule.participationimage = "iconcheck.png";
				}
				else
				{
					class_schedule.participationimage = "iconinativo.png";
				}

			}

		}

		public void createClassesCollection()
		{
			//COLLECTION GRADUACOES
			importantClassesCollectionView = new CollectionView
			{
				SelectionMode = SelectionMode.Multiple,
				//ItemsSource = importantClass_Schedule,
				ItemsLayout = new GridItemsLayout(1, ItemsLayoutOrientation.Horizontal) { VerticalItemSpacing = 5 * App.screenHeightAdapter, HorizontalItemSpacing = 5 * App.screenWidthAdapter, },
				EmptyView = new ContentView
				{
					Content = new StackLayout
					{
						Children =
							{
								new Label { Text = "Não existem aulas agendadas.", HorizontalTextAlignment = TextAlignment.Start, TextColor = Color.White, FontSize = App.itemTitleFontSize },
							}
					}
				}
			};
			this.BindingContext = scheduleCollection;
			importantClassesCollectionView.SetBinding(ItemsView.ItemsSourceProperty, "Items");


			importantClassesCollectionView.SelectionChanged += OnClassScheduleCollectionViewSelectionChanged;

			importantClassesCollectionView.ItemTemplate = new DataTemplate(() =>
			{
				RelativeLayout itemRelativeLayout = new RelativeLayout
				{
					HeightRequest = App.ItemHeight,
					WidthRequest = App.ItemWidth,
				};

				Frame itemFrame = new Frame
				{
					CornerRadius = 5 * (float) App.screenHeightAdapter,
					IsClippedToBounds = true,
					BorderColor = Color.FromRgb(182, 145, 89),
					BackgroundColor = Color.Transparent,
					Padding = new Thickness(0, 0, 0, 0),
					HeightRequest = App.ItemHeight,// -(10 * App.screenHeightAdapter),
					VerticalOptions = LayoutOptions.Center,
				};

				Image eventoImage = new Image {
					Aspect = Aspect.AspectFill,
					Opacity = 0.5,
				};
				eventoImage.SetBinding(Image.SourceProperty, "imagesourceObject");

				itemFrame.Content = eventoImage;

				/*var itemFrame_tap = new TapGestureRecognizer();
				itemFrame_tap.Tapped += (s, e) =>
				{
					Navigation.PushAsync(new EquipamentsPageCS("protecoescintos"));
				};
				itemFrame.GestureRecognizers.Add(itemFrame_tap);*/

				itemRelativeLayout.Children.Add(itemFrame,
					xConstraint: Constraint.Constant(0),
					yConstraint: Constraint.Constant(0),
					widthConstraint: Constraint.RelativeToParent((parent) =>
					{
					return (parent.Width);// - (5 * App.screenWidthAdapter));
					}),
					heightConstraint: Constraint.RelativeToParent((parent) =>
					{
						return (parent.Height);
					}));

				Label nameLabel = new Label { BackgroundColor = Color.Transparent, VerticalTextAlignment = TextAlignment.Start, HorizontalTextAlignment = TextAlignment.Center, FontSize = App.itemTitleFontSize, TextColor = Color.White, LineBreakMode = LineBreakMode.WordWrap };
				nameLabel.SetBinding(Label.TextProperty, "name");

				itemRelativeLayout.Children.Add(nameLabel,
					xConstraint: Constraint.Constant(3 * App.screenWidthAdapter),
					yConstraint: Constraint.Constant(25 * App.screenHeightAdapter),
					widthConstraint: Constraint.RelativeToParent((parent) =>
					{
						return (parent.Width - (6 * App.screenWidthAdapter));
					}),
					heightConstraint: Constraint.Constant(50 * App.screenHeightAdapter));

				Label dateLabel = new Label { VerticalTextAlignment = TextAlignment.Center, HorizontalTextAlignment = TextAlignment.Center, FontSize = App.itemTextFontSize, TextColor = Color.White, LineBreakMode = LineBreakMode.WordWrap };
				dateLabel.SetBinding(Label.TextProperty, "datestring");

				itemRelativeLayout.Children.Add(dateLabel,
					xConstraint: Constraint.Constant(25 * App.screenWidthAdapter),
					yConstraint: Constraint.Constant(App.ItemHeight - (45 * App.screenHeightAdapter)),
					widthConstraint: Constraint.RelativeToParent((parent) =>
					{
						return (parent.Width - (50 * App.screenWidthAdapter)); // center of image (which is 40 wide)
					}),
					heightConstraint: Constraint.Constant((40 * App.screenHeightAdapter)));


				Image participationImagem = new Image { Aspect = Aspect.AspectFill }; //, HeightRequest = 60, WidthRequest = 60
				participationImagem.SetBinding(Image.SourceProperty, "participationimage");

				itemRelativeLayout.Children.Add(participationImagem,
					xConstraint: Constraint.RelativeToParent((parent) =>
					{
						return (parent.Width - (20 * App.screenHeightAdapter));
					}),
					yConstraint: Constraint.Constant(5),
					widthConstraint: Constraint.Constant((20 * App.screenHeightAdapter)),
					heightConstraint: Constraint.Constant((20 * App.screenHeightAdapter)));

				return itemRelativeLayout;
			});
			relativeLayout.Children.Add(importantClassesCollectionView,
				xConstraint: Constraint.Constant(0),
				yConstraint: Constraint.Constant(classesY + (30 * App.screenHeightAdapter)),
				widthConstraint: Constraint.RelativeToParent((parent) =>
				{
					return (parent.Width); // center of image (which is 40 wide)
				}),
				heightConstraint: Constraint.Constant(App.ItemHeight  + (10 * App.screenHeightAdapter)));


		}

		public async void createImportantEvents()
		{
			importantEvents = await GetImportantEvents();

			foreach (Event event_i in importantEvents)
			{
				if ((event_i.imagemNome == "") | (event_i.imagemNome is null))
				{
					event_i.imagemSource = "logo_aksl.png";
				}
				else
				{
					event_i.imagemSource = Constants.images_URL + event_i.id + "_imagem_c";

				}

				if ((event_i.participationconfirmed == "inscrito") | (event_i.participationconfirmed == "confirmado"))
				{
					event_i.participationimage = "iconcheck.png";
				}
			}

			//AULAS LABEL
			eventsLabel = new Label
			{
				Text = "PRÓXIMOS EVENTOS",
				TextColor = Color.FromRgb(246, 220, 178),
				HorizontalTextAlignment = TextAlignment.Start,
				VerticalTextAlignment = TextAlignment.End,
				FontSize = App.titleFontSize
			};
			relativeLayout.Children.Add(eventsLabel,
			xConstraint: Constraint.Constant(0),
			yConstraint: Constraint.Constant(eventsY),
			widthConstraint: Constraint.RelativeToParent((parent) =>
			{
				return (parent.Width); // center of image (which is 40 wide)
			}),
			heightConstraint: Constraint.Constant(30 * App.screenHeightAdapter));

			CreateProximosEventosColletion();
		}

		public void CreateProximosEventosColletion()
		{
			int gridLines = 2;
			if (App.member.students_count > 0) {
				gridLines = 1;
			}

			//COLLECTION EVENTOS IMPORTANTES
			importantEventsCollectionView = new CollectionView
			{
				SelectionMode = SelectionMode.Single,
				ItemsSource = importantEvents,
				ItemsLayout = new GridItemsLayout(gridLines, ItemsLayoutOrientation.Horizontal) { VerticalItemSpacing = 5 * App.screenHeightAdapter, HorizontalItemSpacing = 5 * App.screenWidthAdapter, },
				EmptyView = new ContentView
				{
					Content = new StackLayout
					{
						Children =
							{
								new Label { Text = "Não existem Eventos agendados.", HorizontalTextAlignment = TextAlignment.Start, TextColor = Color.White, FontSize = App.itemTitleFontSize },
							}
					}
				}
			};

			importantEventsCollectionView.SelectionChanged += OnProximosEventosCollectionViewSelectionChanged;

			importantEventsCollectionView.ItemTemplate = new DataTemplate(() =>
			{
				RelativeLayout itemRelativeLayout = new RelativeLayout
				{
					HeightRequest = App.ItemHeight,
					WidthRequest = App.ItemWidth
				};

				Frame itemFrame = new Frame
				{
					CornerRadius = 5 * (float)App.screenHeightAdapter,
					IsClippedToBounds = true,
					BorderColor = Color.FromRgb(182, 145, 89),
					BackgroundColor = Color.Transparent,
					Padding = new Thickness(0, 0, 0, 0),
					HeightRequest = (App.ItemHeight),
					VerticalOptions = LayoutOptions.Center,
				};

				Image eventoImage = new Image { Aspect = Aspect.AspectFill, Opacity = 0.25 }; //, HeightRequest = 60, WidthRequest = 60
				eventoImage.SetBinding(Image.SourceProperty, "imagemSource");

				itemFrame.Content = eventoImage;

				itemRelativeLayout.Children.Add(itemFrame,
					xConstraint: Constraint.Constant(0),
					yConstraint: Constraint.Constant(0),
					widthConstraint: Constraint.RelativeToParent((parent) =>
					{
						return (parent.Width);
					}),
					heightConstraint: Constraint.RelativeToParent((parent) =>
					{
						return (parent.Height);
					}));

				Label nameLabel = new Label { BackgroundColor = Color.Transparent, VerticalTextAlignment = TextAlignment.Center, HorizontalTextAlignment = TextAlignment.Center, FontSize = App.itemTitleFontSize, TextColor = Color.White, LineBreakMode = LineBreakMode.WordWrap, MaxLines = 2 };
				nameLabel.SetBinding(Label.TextProperty, "name");

				itemRelativeLayout.Children.Add(nameLabel,
					xConstraint: Constraint.Constant(3 * App.screenWidthAdapter),
					yConstraint: Constraint.Constant(15 * App.screenHeightAdapter),
					widthConstraint: Constraint.RelativeToParent((parent) =>
					{
						return (parent.Width - (10 * App.screenWidthAdapter));
					}),
					heightConstraint: Constraint.Constant(((App.ItemHeight - (15 * App.screenHeightAdapter)) /2)));

				Label categoryLabel = new Label { VerticalTextAlignment = TextAlignment.Center, HorizontalTextAlignment = TextAlignment.Center, FontSize = App.itemTextFontSize, TextColor = Color.White, LineBreakMode = LineBreakMode.WordWrap };
				categoryLabel.SetBinding(Label.TextProperty, "category");

				itemRelativeLayout.Children.Add(categoryLabel,
					xConstraint: Constraint.Constant(0),
					yConstraint: Constraint.Constant(((App.ItemHeight - (15 * App.screenHeightAdapter)) / 2) ),
					widthConstraint: Constraint.RelativeToParent((parent) =>
					{
						return (parent.Width); // center of image (which is 40 wide)
					}),
					heightConstraint: Constraint.Constant(((App.ItemHeight - (15 * App.screenHeightAdapter)) / 4)));

				Label dateLabel = new Label { VerticalTextAlignment = TextAlignment.Center, HorizontalTextAlignment = TextAlignment.Center, FontSize = App.itemTextFontSize, TextColor = Color.White, LineBreakMode = LineBreakMode.WordWrap };
				dateLabel.SetBinding(Label.TextProperty, "detailed_date");

				itemRelativeLayout.Children.Add(dateLabel,
					xConstraint: Constraint.Constant(0),
					yConstraint: Constraint.Constant((App.ItemHeight -15) - ((App.ItemHeight - 15) / 4)),
					widthConstraint: Constraint.RelativeToParent((parent) =>
					{
						return (parent.Width); // center of image (which is 40 wide)
					}),
					heightConstraint: Constraint.Constant(((App.ItemHeight - (15 * App.screenHeightAdapter)) / 4)));


				Image participationImagem = new Image { Aspect = Aspect.AspectFill }; //, HeightRequest = 60, WidthRequest = 60
				participationImagem.SetBinding(Image.SourceProperty, "participationimage");

				itemRelativeLayout.Children.Add(participationImagem,
					xConstraint: Constraint.RelativeToParent((parent) =>
					{
						return (parent.Width - (20 * App.screenHeightAdapter));
					}),
					yConstraint: Constraint.Constant(5),
					widthConstraint: Constraint.Constant(20 * App.screenHeightAdapter),
					heightConstraint: Constraint.Constant(20 * App.screenHeightAdapter));

				return itemRelativeLayout;
			});

			 relativeLayout.Children.Add(importantEventsCollectionView,
				xConstraint: Constraint.Constant(0),
				yConstraint: Constraint.Constant(eventsY + (30 * App.screenHeightAdapter)),
				widthConstraint: Constraint.RelativeToParent((parent) =>
				{
					return (parent.Width); // center of image (which is 40 wide)
				}),
				heightConstraint: Constraint.Constant(eventsHeight));
		}




		public void createFamousQuote()
		{
			Random random = new Random();
			int random_number = random.Next(Constants.famousQuotes.Count);

			famousQuoteLabel = new Label
			{
				Text = Constants.famousQuotes[random_number],
				TextColor = Color.White,
				HorizontalTextAlignment = TextAlignment.Center,
				FontSize = App.titleFontSize,
				FontAttributes = FontAttributes.Italic
			};

			relativeLayout.Children.Add(famousQuoteLabel,
			xConstraint: Constraint.Constant(0),
			yConstraint: Constraint.Constant(feesOrQuoteY),
			widthConstraint: Constraint.RelativeToParent((parent) =>
			{
				return parent.Width; // size of screen -80
			}),
			heightConstraint: Constraint.Constant(90 * App.screenHeightAdapter));
		}

		public async void createCurrentFee()
		{

			if (App.member.currentFee == null)
			{
				Debug.Print("Current Fee NULL não devia acontecer!");
				var result = await GetCurrentFees(App.member);
			}

			bool hasQuotaPayed = false;

			if (App.member.currentFee != null)
			{
				if ((App.member.currentFee.estado == "fechado") | (App.member.currentFee.estado == "recebido") | (App.member.currentFee.estado == "confirmado"))
				{
					hasQuotaPayed = true;
					createFamousQuote();
					return;
				}
			}

			if (hasQuotaPayed == false)
            {

                bool answer = await DisplayAlert("A TUA QUOTA NÃO ESTÁ ATIVA.", "A tua quota para este ano não está ativa. Queres efetuar o pagamento?", "Sim", "Não");
                Debug.WriteLine("Answer: " + answer);
				if (answer == true)
				{
                    await Navigation.PushAsync(new QuotasPageCS());
                }

                currentFeeLabel = new Label
				{
					Text = "A TUA QUOTA PARA ESTE ANO NÃO ESTÁ ATIVA. \n DESTA FORMA NÃO PODERÁS PARTICIPAR NOS NOSSOS EVENTOS :(. \n ATIVA AQUI A TUA QUOTA.",
					TextColor = Color.Red,
					HorizontalTextAlignment = TextAlignment.Center,
					FontSize = App.itemTextFontSize
				};

				var currentFeeLabel_tap = new TapGestureRecognizer();
				currentFeeLabel_tap.Tapped += async (s, e) =>
				{
					await Navigation.PushAsync(new QuotasPageCS());
				};
				currentFeeLabel.GestureRecognizers.Add(currentFeeLabel_tap);


				relativeLayout.Children.Add(currentFeeLabel,
					xConstraint: Constraint.Constant(0),
					yConstraint: Constraint.Constant(feesOrQuoteY),
					widthConstraint: Constraint.RelativeToParent((parent) =>
					{
						return parent.Width; // size of screen -80
					}),
					heightConstraint: Constraint.Constant(50));
			}
		}

        public async void createDelayedMonthFee()
        {


            bool answer = await DisplayAlert("A TUA QUOTA NÃO ESTÁ ATIVA.", "A tua quota para este ano não está ativa. Queres efetuar o pagamento?", "Sim", "Não");
            Debug.WriteLine("Answer: " + answer);
                
        }

        public async void createVersion()
		{
			currentVersionLabel = new Label
			{
				Text = "Version 1.2(23)",
				TextColor = Color.White,
				HorizontalTextAlignment = TextAlignment.End,
				FontSize = 10
			};

			relativeLayout.Children.Add(currentVersionLabel,
			xConstraint: Constraint.Constant(0),
			yConstraint: Constraint.Constant(feesOrQuoteY+90),
			widthConstraint: Constraint.RelativeToParent((parent) =>
			{
				return parent.Width; // size of screen -80
			}),
			heightConstraint: Constraint.Constant(30));
		}
		

		public MainPageCS ()
		{

			this.initLayout();
			//this.initSpecificLayout(App.members);

		}

		void OnSendClick(object sender, EventArgs e)
		{
			/*notificationNumber++;
			string title = $"Local Notification #{notificationNumber}";
			string message = $"You have now received {notificationNumber} notifications!";
			notificationManager.SendNotification(title, message);*/
		}

		void OnScheduleClick(object sender, EventArgs e)
		{
			/*notificationNumber++;
			string title = $"Local Notification #{notificationNumber}";
			string message = $"You have now received {notificationNumber} notifications!";
			notificationManager.SendNotification(title, message, DateTime.Now.AddSeconds(10));*/
		}

		void ShowNotification(string title, string message)
		{
			Device.BeginInvokeOnMainThread(() =>
			{
				msg.Text = $"Notification Received:\nTitle: {title}\nMessage: {message}";
			});
		}

		async void OnPerfilButtonClicked(object sender, EventArgs e)
		{
			await Navigation.PushAsync(new ProfileCS());
		}

		async Task<ObservableCollection<Class_Schedule>> GetStudentClass_Schedules(string begindate, string enddate)
		{
			Debug.WriteLine("GetStudentClass_Schedules");
			ClassManager classManager = new ClassManager();
			ObservableCollection<Class_Schedule> class_schedules_i = await classManager.GetStudentClass_Schedules_obs(App.member.id, begindate, enddate);
			if (class_schedules_i == null)
			{
				Application.Current.MainPage = new NavigationPage(new LoginPageCS("Verifique a sua ligação à Internet e tente novamente."))
				{
					BarBackgroundColor = Color.FromRgb(15, 15, 15),
					BarTextColor = Color.White
				};
				return null;
			}
			return class_schedules_i;
		}

		async Task<List<Event>> GetImportantEvents()
		{
			Debug.WriteLine("GetImportantEvents");
			EventManager eventManager = new EventManager();
			List<Event> events = await eventManager.GetImportantEvents(App.member.id);
			if (events == null)
			{
				Application.Current.MainPage = new NavigationPage(new LoginPageCS("Verifique a sua ligação à Internet e tente novamente."))
				{
					BarBackgroundColor = Color.FromRgb(15, 15, 15),
					BarTextColor = Color.White
				};
				return null;
			}
			return events;
		}

		async Task<int> GetCurrentFees(Member member)
		{
			Debug.WriteLine("MainTabbedPageCS.GetCurrentFees");
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

		async Task<List<Class_Schedule>> GetAllClass_Schedules(string begindate, string enddate)
		{
			ClassManager classManager = new ClassManager();
			List<Class_Schedule> class_schedules_i = await classManager.GetAllClass_Schedules(App.member.id, begindate, enddate);

			if (class_schedules_i == null)
			{
				Application.Current.MainPage = new NavigationPage(new LoginPageCS("Verifique a sua ligação à Internet e tente novamente."))
				{
					BarBackgroundColor = Color.FromRgb(15, 15, 15),
					BarTextColor = Color.White
				};
				return null;
			}
			return class_schedules_i;
		}

		async void OnClassScheduleCollectionViewSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			UserDialogs.Instance.ShowLoading("", MaskType.Clear);
			Debug.WriteLine("MainPageCS.OnClassScheduleCollectionViewSelectionChanged");

			if ((sender as CollectionView).SelectedItems.Count != 0)
			{
				ClassManager classmanager = new ClassManager();

				Class_Schedule class_schedule = (sender as CollectionView).SelectedItems[0] as Class_Schedule;
				if (class_schedule.classattendanceid == null)
				{
                    Task.Run(async () =>
                    {
                        string class_attendance_id = await classmanager.CreateClass_Attendance(App.member.id, class_schedule.classid, "confirmada", class_schedule.date);
                        class_schedule.classattendanceid = class_attendance_id;
                        return true;
                    });
                    //string class_attendance_id = await classmanager.CreateClass_Attendance(App.member.id, class_schedule.classid, "confirmada", class_schedule.date);
                    /*                    string class_attendance_id =  classmanager.CreateClass_Attendance_sync(App.member.id, class_schedule.classid, "confirmada", class_schedule.date);
                                        */
                    class_schedule.classattendancestatus = "confirmada";
                    class_schedule.participationimage = "iconcheck.png";


                }
                else
				{
					if (class_schedule.classattendancestatus == "anulada")
					{
						class_schedule.classattendancestatus = "confirmada";
						class_schedule.participationimage = "iconcheck.png";
						int result = await classmanager.UpdateClass_Attendance(class_schedule.classattendanceid, class_schedule.classattendancestatus);
					}
					else if (class_schedule.classattendancestatus == "confirmada")
					{
						class_schedule.classattendancestatus = "anulada";
						class_schedule.participationimage = "iconinativo.png";
						int result = await classmanager.UpdateClass_Attendance(class_schedule.classattendanceid, class_schedule.classattendancestatus);
					}
					else if (class_schedule.classattendancestatus == "fechada")
					{
						UserDialogs.Instance.Alert(new AlertConfig() { Title = "PRESENÇA EM AULA", Message = "A tua presença nesta aula já foi validada pelo instrutor pelo que não é possível alterar o seu estado.", OkText = "Ok" });
					}
					//int result = await classmanager.UpdateClass_Attendance(class_schedule.classattendanceid, class_schedule.classattendancestatus);
				}

				((CollectionView)sender).SelectedItems.Clear();
				/*importantClassesCollectionView.ItemsSource = cleanClass_Schedule;
				importantClassesCollectionView.ItemsSource = importantClass_Schedule;*/

				UserDialogs.Instance.HideLoading();   //Hide loader
			}
		}

		async void OnProximosEventosCollectionViewSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			Debug.WriteLine("OnCollectionViewProximosEstagiosSelectionChanged " + (sender as CollectionView).SelectedItem.GetType().ToString());

			if ((sender as CollectionView).SelectedItem != null)
			{
				Event event_v = (sender as CollectionView).SelectedItem as Event;

				if (event_v.type == "estagio")
				{
					await Navigation.PushAsync(new DetailEventPageCS(event_v));
				}
				else if (event_v.type == "competicao")
				{

					if (event_v.participationid == null)
					{
                        await Navigation.PushAsync(new DetailCompetitionPageCS(event_v.id));
                    }
					else
					{
                        await Navigation.PushAsync(new DetailCompetitionPageCS(event_v.id, event_v.participationid));
                    }
					
				}
				else if (event_v.type == "sessaoexame")
				{
					await Navigation.PushAsync(new ExaminationSessionPageCS(event_v.id));
				}

			}
		}

		async void OnTeacherClassesCollectionViewSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			Debug.WriteLine("MainPageCS.OnClassAttendanceCollectionViewSelectionChanged");

			if ((sender as CollectionView).SelectedItem != null)
			{
				Class_Schedule class_schedule = (sender as CollectionView).SelectedItem as Class_Schedule;
				(sender as CollectionView).SelectedItem = null;
				await Navigation.PushAsync(new AttendanceClassPageCS(class_schedule));

			}
		}
	}
}
