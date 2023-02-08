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
	public class DoPageCS : ContentPage
	{

		protected override void OnAppearing()
		{
			initSpecificLayout();
		}

		protected override void OnDisappearing()
		{
			this.CleanScreen();
		}

		List<Competition_Result> competition_results;

		MenuButton premiosButton, graduacoesButton, palmaresButton, participacoesEventosButton;
		private RelativeLayout premiosRelativeLayout, graduacoesRelativeLayout, palmaresRelativeLayout, participacoesEventosRelativeLayout;

		private RelativeLayout relativeLayout;
		private StackLayout stackButtons;

		private CollectionView collectionViewPremios, collectionViewParticipacoesCompeticoes, collectionViewParticipacoesEventos;
		private SfChart chart;

		private List<Award> awards;
		private List<Competition_Participation>  pastCompetitionParticipations;
		private List<Event_Participation> pastEventParticipations;

		private OptionButton minhasGraduacoesButton, programasExameButton;

		public void initLayout()
		{
			Title = "COMPETIÇÕES";
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

			var toolbarItem = new ToolbarItem
			{
				//Text = "Logout",
				IconImageSource = "perfil.png",

			};
			toolbarItem.Clicked += OnPerfilButtonClicked;
			ToolbarItems.Add(toolbarItem);

			NavigationPage.SetBackButtonTitle(this, "");
		}


		public void CleanScreen()
		{
			Debug.Print("CleanScreen");
			//valida se os objetos já foram criados antes de os remover
			if (stackButtons != null)
			{
				relativeLayout.Children.Remove(stackButtons);
				relativeLayout.Children.Remove(premiosRelativeLayout);
				relativeLayout.Children.Remove(graduacoesRelativeLayout);
				relativeLayout.Children.Remove(collectionViewParticipacoesCompeticoes);
				relativeLayout.Children.Remove(palmaresRelativeLayout);
				relativeLayout.Children.Remove(collectionViewParticipacoesEventos);

				stackButtons = null;
				collectionViewPremios = null;
				graduacoesRelativeLayout = null;
				collectionViewParticipacoesCompeticoes = null;
				palmaresRelativeLayout = null;
				collectionViewParticipacoesEventos = null;
			}
			if (chart != null)
			{
				relativeLayout.Children.Remove(chart);
				chart = null;
			}

		}

		public async void initSpecificLayout()
		{
			UserDialogs.Instance.ShowLoading("", MaskType.Clear);

			

			pastEventParticipations = await GetPastEventParticipations();
			pastCompetitionParticipations = await GetPastCompetitionParticipations();

			CreateStackButtons();
			CreatePremios();
			CreateGrades();
			CreateParticipacoesCompeticoesColletion();
			CreateParticipacoesEventosColletion();

			activateLastSelectedTab();

			UserDialogs.Instance.HideLoading();   //Hide loader
		}

		public void activateLastSelectedTab() {

			if (App.DO_activetab == "graduacoes")
			{
				OnGraduacoesButtonClicked(null, null);
			}
			else if (App.DO_activetab == "palmares")
			{
				OnPalmaresButtonClicked(null, null);
			}
			else if (App.DO_activetab == "participacoesevento")
			{
				OnParticipacoesEventosButtonClicked(null, null);
			}
			else if (App.DO_activetab == "premios")
			{
				OnPremiosButtonClicked(null, null);
			}
			else
			{
				OnGraduacoesButtonClicked(null, null);
			}
		}

		public void CreateStackButtons()
		{

			Debug.Print("CreateStackButtons Constants.ScreenWidth = "+ Constants.ScreenWidth);
			var width = Constants.ScreenWidth;
			var buttonWidth = (width - 50) / 4;

			graduacoesButton = new MenuButton("GRADUAÇÕES", buttonWidth, 60);
			graduacoesButton.button.Clicked += OnGraduacoesButtonClicked;


			palmaresButton = new MenuButton("PALMARÉS", buttonWidth, 60);
			palmaresButton.button.Clicked += OnPalmaresButtonClicked;

			participacoesEventosButton = new MenuButton("EVENTOS", buttonWidth, 60);
			participacoesEventosButton.button.Clicked += OnParticipacoesEventosButtonClicked;

			premiosButton = new MenuButton("PRÉMIOS", buttonWidth, 60);
			premiosButton.button.Clicked += OnPremiosButtonClicked;


			stackButtons = new StackLayout
			{
				Margin = new Thickness(0),
				Spacing = 5,
				Orientation = StackOrientation.Horizontal,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.FillAndExpand,
				HeightRequest = 60 * App.screenHeightAdapter,
				Children =
				{
					graduacoesButton,
					palmaresButton,
					participacoesEventosButton,
					premiosButton
				}
			};

			//Content = stackButtons;
			relativeLayout.Children.Add(stackButtons,
			xConstraint: Constraint.Constant(0),
			yConstraint: Constraint.Constant(0),
			widthConstraint: Constraint.RelativeToParent((parent) =>
			{
				return (parent.Width);
			}),
			heightConstraint: Constraint.Constant(60 * App.screenHeightAdapter));

			
			graduacoesButton.activate();
			palmaresButton.deactivate();
			participacoesEventosButton.deactivate();
			premiosButton.deactivate();
		}

		public void CreatePremios() {
			premiosRelativeLayout = new RelativeLayout
			{
				Margin = new Thickness(5)
			};

			CreatePremiosCollection();

			relativeLayout.Children.Add(premiosRelativeLayout,
				xConstraint: Constraint.Constant(0),
				yConstraint: Constraint.Constant(30 * App.screenHeightAdapter),
				widthConstraint: Constraint.RelativeToParent((parent) =>
				{
					return (parent.Width);
				}),
				heightConstraint: Constraint.RelativeToParent((parent) =>
				{
					return (parent.Height) - (80 * App.screenHeightAdapter);
				}));
		}


		public async void CreatePremiosCollection()
		{			
			var result = await GetAwards_Student(App.member);

			Label premiosLabel = new Label { BackgroundColor = Color.Transparent, VerticalTextAlignment = TextAlignment.Center, HorizontalTextAlignment = TextAlignment.Center, FontSize = App.titleFontSize, TextColor = Color.FromRgb(182, 145, 89), LineBreakMode = LineBreakMode.WordWrap };
			premiosLabel.Text = "PRÉMIOS";

			premiosRelativeLayout.Children.Add(premiosLabel,
				xConstraint: Constraint.Constant(0),
				yConstraint: Constraint.Constant(0 * App.screenHeightAdapter),
				widthConstraint: Constraint.RelativeToParent((parent) =>
				{
					return (parent.Width);
				}),
				heightConstraint: Constraint.Constant(50 * App.screenHeightAdapter));

			//COLLECTION PREMIOS
			collectionViewPremios = new CollectionView
			{
				SelectionMode = SelectionMode.None,
				ItemsSource = awards,
				ItemsLayout = new GridItemsLayout(3, ItemsLayoutOrientation.Vertical) { VerticalItemSpacing = 5, HorizontalItemSpacing = 5,  },
				EmptyView = new ContentView
				{
					Content = new StackLayout
					{
						Children =
							{
								new Label { Text = "Ainda não tens prémios anteriores. Continua a treinar e vais ver que vais conseguir!", HorizontalTextAlignment = TextAlignment.Center, TextColor = Color.White, FontSize = App.itemTitleFontSize },
							}
					}
				}
			};

			GradientBrush gradient = new LinearGradientBrush
			{
				StartPoint = new Point(0, 0),
				EndPoint = new Point(1, 1),
			};

			gradient.GradientStops.Add(new GradientStop(Color.FromRgb(180, 143, 86), Convert.ToSingle(0)));
			gradient.GradientStops.Add(new GradientStop(Color.FromRgb(246, 220, 178), Convert.ToSingle(0.5)));


			collectionViewPremios.ItemTemplate = new DataTemplate(() =>
			{
				RelativeLayout itemRelativeLayout = new RelativeLayout
				{
					HeightRequest=220 * App.screenHeightAdapter
				};


				Image premioImage= new Image {};
				premioImage.SetBinding(Image.SourceProperty, "imagem");

				itemRelativeLayout.Children.Add(premioImage,
					xConstraint: Constraint.Constant(10),
					yConstraint: Constraint.Constant(0),
					widthConstraint: Constraint.Constant(90 * App.screenHeightAdapter),
					heightConstraint: Constraint.Constant(90 * App.screenHeightAdapter));

				Label nameLabel = new Label { BackgroundColor = Color.Transparent, VerticalTextAlignment = TextAlignment.Start, HorizontalTextAlignment = TextAlignment.Center, FontSize = App.itemTextFontSize, TextColor = Color.White, LineBreakMode = LineBreakMode.WordWrap };
				nameLabel.SetBinding(Label.TextProperty, "name");

				itemRelativeLayout.Children.Add(nameLabel,
					xConstraint: Constraint.Constant(10),
					yConstraint: Constraint.Constant(100 * App.screenHeightAdapter),
					widthConstraint: Constraint.Constant(90 * App.screenHeightAdapter),
					heightConstraint: Constraint.Constant(90 * App.screenHeightAdapter));

				
				
				return itemRelativeLayout;
			});



			premiosRelativeLayout.Children.Add(collectionViewPremios,
				xConstraint: Constraint.Constant(0),
				yConstraint: Constraint.Constant(60 * App.screenHeightAdapter),
				widthConstraint: Constraint.RelativeToParent((parent) =>
				{
					return (parent.Width); // center of image (which is 40 wide)
				}),
				heightConstraint: Constraint.RelativeToParent((parent) =>
				{
					return (parent.Height) - (70 * App.screenHeightAdapter);
				}));

		}

		public void CreateGrades()
		{
			graduacoesRelativeLayout = new RelativeLayout
			{
				Margin = new Thickness(5)
			};

			CreateGraduacoesOptionButtons();

			relativeLayout.Children.Add(graduacoesRelativeLayout,
				xConstraint: Constraint.Constant(0),
				yConstraint: Constraint.Constant(80 * App.screenHeightAdapter),
				widthConstraint: Constraint.RelativeToParent((parent) =>
				{
					return (parent.Width);
				}),
				heightConstraint: Constraint.RelativeToParent((parent) =>
				{
					return (parent.Height) - (80 * App.screenHeightAdapter);
				}));
		}

		public void CreateGraduacoesOptionButtons()
		{
			var width = Constants.ScreenWidth;
			var buttonWidth = (width) / 2;


			minhasGraduacoesButton = new OptionButton("MINHAS GRADUAÇÕES", "mygrades.png", buttonWidth, 60);
			//minhasGraduacoesButton.button.Clicked += OnMinhasGraduacoesButtonClicked;
			var minhasGraduacoesButton_tap = new TapGestureRecognizer();
			minhasGraduacoesButton_tap.Tapped += (s, e) =>
			{
				Navigation.PushAsync(new myGradesPageCS("MinhasGraduaçoes"));
			};
			minhasGraduacoesButton.GestureRecognizers.Add(minhasGraduacoesButton_tap);

			programasExameButton = new OptionButton("PROGRAMAS EXAME", "examinationprograms.png", buttonWidth, 60);
			var programasExameButton_tap = new TapGestureRecognizer();
			programasExameButton_tap.Tapped += (s, e) =>
			{
				Navigation.PushAsync(new myGradesPageCS("ProgramasExame"));
			};
			programasExameButton.GestureRecognizers.Add(programasExameButton_tap);


			StackLayout stackGraduacaoesButtons = new StackLayout
			{
				//WidthRequest = 370,
				Margin = new Thickness(0),
				Spacing = 50,
				Orientation = StackOrientation.Vertical,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.FillAndExpand,
				HeightRequest = 280,
				Children =
				{
					minhasGraduacoesButton,
					programasExameButton,
				}
			};

			graduacoesRelativeLayout.Children.Add(stackGraduacaoesButtons,
			xConstraint: Constraint.RelativeToParent((parent) =>
			{
				return (parent.Width / 4);
			}),
			yConstraint: Constraint.Constant(0),
			widthConstraint: Constraint.RelativeToParent((parent) =>
			{
				return (parent.Width/2);
			}),
			heightConstraint: Constraint.Constant(280 * App.screenHeightAdapter));
		}


		public void CreateParticipacoesEventosColletion()
		{

			//COLLECTION PARTICIPAÇÕES EVENTOS
			collectionViewParticipacoesEventos = new CollectionView
			{
				SelectionMode = SelectionMode.Single,
				ItemsSource = pastEventParticipations,
				ItemsLayout = new GridItemsLayout(2, ItemsLayoutOrientation.Vertical) { VerticalItemSpacing = 5 * App.screenHeightAdapter, HorizontalItemSpacing = 5 * App.screenWidthAdapter },
				EmptyView = new ContentView
				{
					Content = new StackLayout
					{
						Children =
							{
								new Label { Text = "Não existem participações em eventos.", HorizontalTextAlignment = TextAlignment.Center, TextColor = Color.White, FontSize = App.itemTitleFontSize },
							}
					}
				}
			};

			foreach (Event_Participation event_participation in pastEventParticipations)
			{
				if ((event_participation.imagemNome == "") | (event_participation.imagemNome is null))
				{
					event_participation.imagemSource = "logo_aksl.png";
				}
				else
				{
					event_participation.imagemSource = Constants.images_URL + event_participation.evento_id + "_imagem_c";

				}
			}

			collectionViewParticipacoesEventos.SelectionChanged += OnCollectionViewParticipacoesEventosSelectionChanged;

			collectionViewParticipacoesEventos.ItemTemplate = new DataTemplate(() =>
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
					HeightRequest = App.ItemHeight,
					WidthRequest = App.ItemWidth,
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
						return (parent.Width - (5 * App.screenWidthAdapter));
					}),
					heightConstraint: Constraint.RelativeToParent((parent) =>
					{
						return (parent.Height);
					}));

				Label nameLabel = new Label { BackgroundColor = Color.Transparent, VerticalTextAlignment = TextAlignment.Center, HorizontalTextAlignment = TextAlignment.Center, FontSize = App.itemTitleFontSize, TextColor = Color.White, LineBreakMode = LineBreakMode.WordWrap };
				nameLabel.SetBinding(Label.TextProperty, "evento_name");

				itemRelativeLayout.Children.Add(nameLabel,
					xConstraint: Constraint.Constant(3 * App.screenWidthAdapter),
					yConstraint: Constraint.Constant(15 * App.screenHeightAdapter),
					widthConstraint: Constraint.RelativeToParent((parent) =>
					{
						return (parent.Width - (10 * App.screenWidthAdapter));
					}),
					heightConstraint: Constraint.Constant(((App.ItemHeight - (15 * App.screenHeightAdapter)) / 2)));

				Label dateLabel = new Label { VerticalTextAlignment = TextAlignment.Center, HorizontalTextAlignment = TextAlignment.Center, FontSize = App.itemTextFontSize, TextColor = Color.White, LineBreakMode = LineBreakMode.WordWrap };
				dateLabel.SetBinding(Label.TextProperty, "evento_detailed_date");

				itemRelativeLayout.Children.Add(dateLabel,
					xConstraint: Constraint.Constant(5 * App.screenHeightAdapter),
					yConstraint: Constraint.Constant((App.ItemHeight - (15 * App.screenHeightAdapter)) - ((App.ItemHeight - (15 * App.screenHeightAdapter)) / 3)),
					widthConstraint: Constraint.RelativeToParent((parent) =>
					{
						return (parent.Width-(10 * App.screenHeightAdapter)); // center of image (which is 40 wide)
					}),
					heightConstraint: Constraint.Constant(((App.ItemHeight - (15 * App.screenHeightAdapter)) / 3)));


				Image participationImagem = new Image { Aspect = Aspect.AspectFill }; //, HeightRequest = 60, WidthRequest = 60
				participationImagem.Source = "iconcheck.png";

				itemRelativeLayout.Children.Add(participationImagem,
					xConstraint: Constraint.RelativeToParent((parent) =>
					{
						return (parent.Width - (25 * App.screenHeightAdapter));
					}),
					yConstraint: Constraint.Constant(0),
					widthConstraint: Constraint.Constant(20 * App.screenHeightAdapter),
					heightConstraint: Constraint.Constant(20 * App.screenHeightAdapter));

				return itemRelativeLayout;

			});

		}


		public void CreateParticipacoesCompeticoesColletion()
		{

			palmaresRelativeLayout = new RelativeLayout
			{
				Margin = new Thickness(5)
			};


			//COLLECTION PARTICIPAÇÕES EVENTOS
			collectionViewParticipacoesCompeticoes = new CollectionView
			{
				SelectionMode = SelectionMode.Single,
				ItemsSource = pastCompetitionParticipations,
				ItemsLayout = new GridItemsLayout(2, ItemsLayoutOrientation.Vertical) { VerticalItemSpacing = 5 * App.screenHeightAdapter, HorizontalItemSpacing = 5 * App.screenWidthAdapter, },
				EmptyView = new ContentView
				{
					Content = new StackLayout
					{
						Children =
							{
								new Label { Text = "Não existem resultados de competições.", HorizontalTextAlignment = TextAlignment.Center, TextColor = Color.White, FontSize = App.itemTitleFontSize },
							}
					}
				}
			};

			foreach (Competition_Participation competition_participation in pastCompetitionParticipations)
			{
				if ((competition_participation.imagemNome == "") | (competition_participation.imagemNome is null))
				{
					competition_participation.imagemSource = "logo_aksl.png";
				}
				else
				{
					competition_participation.imagemSource = Constants.images_URL + competition_participation.competicao_id + "_imagem_c";
				}

				if (competition_participation.classificacao == "1")
				{
					competition_participation.classificacaoColor = Color.FromRgb(231, 188, 64);
				}
				else if (competition_participation.classificacao == "2")
				{
					competition_participation.classificacaoColor = Color.FromRgb(174, 174, 174);
				}
				else if (competition_participation.classificacao == "3")
				{
					competition_participation.classificacaoColor = Color.FromRgb(179, 144, 86);
				}
				else
				{
					competition_participation.classificacaoColor = Color.FromRgb(88, 191, 237);
					if ((competition_participation.classificacao == "") | (competition_participation.classificacao is null))
					{
						competition_participation.classificacao = "P";

					}
					else
					{
						competition_participation.classificacao = competition_participation.classificacao.ToUpper();
					}
				}
			}

			collectionViewParticipacoesCompeticoes.SelectionChanged += OnCollectionViewParticipacoesCompeticoesSelectionChanged;

			collectionViewParticipacoesCompeticoes.ItemTemplate = new DataTemplate(() =>
			{
				RelativeLayout itemRelativeLayout = new RelativeLayout
				{
					HeightRequest = App.ItemHeight,
				};

				Frame itemFrame = new Frame
				{
					CornerRadius = 5 * (float) App.screenHeightAdapter,
					IsClippedToBounds = true,
					BorderColor = Color.FromRgb(182, 145, 89),
					BackgroundColor = Color.Transparent,
					Padding = new Thickness(0, 0, 0, 0),
					HeightRequest = App.ItemHeight,
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
						return (parent.Width - (1 * App.screenWidthAdapter));
					}),
					heightConstraint: Constraint.RelativeToParent((parent) =>
					{
						return (parent.Height);
					}));

				Label nameLabel = new Label { BackgroundColor = Color.Transparent, VerticalTextAlignment = TextAlignment.Center, HorizontalTextAlignment = TextAlignment.Center, FontSize = App.itemTitleFontSize, TextColor = Color.White, LineBreakMode = LineBreakMode.WordWrap };
				nameLabel.SetBinding(Label.TextProperty, "competicao_name");

				itemRelativeLayout.Children.Add(nameLabel,
					xConstraint: Constraint.Constant(3 * App.screenWidthAdapter),
					yConstraint: Constraint.Constant(15 * App.screenHeightAdapter),
					widthConstraint: Constraint.RelativeToParent((parent) =>
					{
						return (parent.Width - (10 * App.screenWidthAdapter));
					}),
					heightConstraint: Constraint.Constant(((App.ItemHeight - (15 * App.screenHeightAdapter)) / 2)));


				Label categoryLabel = new Label { VerticalTextAlignment = TextAlignment.Center, HorizontalTextAlignment = TextAlignment.Center, FontSize = App.itemTextFontSize, TextColor = Color.White, LineBreakMode = LineBreakMode.WordWrap };
				categoryLabel.SetBinding(Label.TextProperty, "categoria");

				itemRelativeLayout.Children.Add(categoryLabel,
					xConstraint: Constraint.Constant(0),
					yConstraint: Constraint.Constant(((App.ItemHeight - (15 * App.screenHeightAdapter)) / 2)),
					widthConstraint: Constraint.RelativeToParent((parent) =>
					{
						return (parent.Width); // center of image (which is 40 wide)
					}),
					heightConstraint: Constraint.Constant(((App.ItemHeight - (15 * App.screenHeightAdapter)) / 4)));

				Label dateLabel = new Label { VerticalTextAlignment = TextAlignment.Center, HorizontalTextAlignment = TextAlignment.Center, FontSize = App.itemTextFontSize, TextColor = Color.White, LineBreakMode = LineBreakMode.NoWrap };
				dateLabel.SetBinding(Label.TextProperty, "competicao_detailed_date");

				itemRelativeLayout.Children.Add(dateLabel,
					xConstraint: Constraint.Constant(0),
					yConstraint: Constraint.Constant((App.ItemHeight - 15) - ((App.ItemHeight - 15) / 4)),
					widthConstraint: Constraint.RelativeToParent((parent) =>
					{
						return (parent.Width); // center of image (which is 40 wide)
					}),
					heightConstraint: Constraint.Constant(((App.ItemHeight - (15 * App.screenHeightAdapter)) / 4)));

				Label classificacaoLabel = new Label { VerticalTextAlignment = TextAlignment.Center, HorizontalTextAlignment = TextAlignment.Center, FontSize = App.itemTextFontSize, TextColor = Color.White, LineBreakMode = LineBreakMode.NoWrap };
				classificacaoLabel.SetBinding(Label.TextProperty, "classificacao");

				Frame classificacaoFrame = new Frame
				{
					CornerRadius = (float) (3 * App.screenHeightAdapter),
					IsClippedToBounds = true,
					Padding = 0
				};
				classificacaoFrame.SetBinding(Label.BackgroundColorProperty, "classificacaoColor");
				classificacaoFrame.Content = classificacaoLabel;

				/*
				Image participationImagem = new Image { Aspect = Aspect.AspectFill }; //, HeightRequest = 60, WidthRequest = 60
				participationImagem.Source = "iconcheck.png";
				*/
				itemRelativeLayout.Children.Add(classificacaoFrame,
					xConstraint: Constraint.RelativeToParent((parent) =>
					{
						return (parent.Width - (21 * App.screenHeightAdapter));
					}),
					yConstraint: Constraint.Constant(0),
					widthConstraint: Constraint.Constant(20 * App.screenHeightAdapter),
					heightConstraint: Constraint.Constant(20 * App.screenHeightAdapter));

				return itemRelativeLayout;

			});

			palmaresRelativeLayout.Children.Add(collectionViewParticipacoesCompeticoes,
			xConstraint: Constraint.Constant(0),
			yConstraint: Constraint.Constant(0),
			widthConstraint: Constraint.RelativeToParent((parent) =>
			{
				return (parent.Width); // center of image (which is 40 wide)
			}),
			heightConstraint: Constraint.RelativeToParent((parent) =>
			{
				return (parent.Height) - 200 * App.screenHeightAdapter; // 
			}));


			if (pastCompetitionParticipations.Count > 0) { 
				chart = createChart();

				palmaresRelativeLayout.Children.Add(chart,
				xConstraint: Constraint.Constant(0),
				yConstraint: Constraint.RelativeToParent((parent) =>
				{
					return (parent.Height) - 210 * App.screenHeightAdapter; // 
				}),
				widthConstraint: Constraint.RelativeToParent((parent) =>
				{
					return (parent.Width); // center of image (which is 40 wide)
				}),
				heightConstraint: Constraint.Constant(200 * App.screenHeightAdapter));
			}
		}

		public SfChart createChart() {
			SfChart chart = new SfChart();

			if (Device.RuntimePlatform == Device.Android)
			{
				chart.BackgroundColor = Color.FromRgb(25, 25, 25);
			}

			//Initializing Primary Axis
			CategoryAxis primaryAxis = new CategoryAxis();
			primaryAxis.Title.Text = "Classificacao";
			chart.PrimaryAxis = primaryAxis;

			NumericalAxis secondaryAxis = new NumericalAxis();
			secondaryAxis.Title.Text = "#";
			chart.SecondaryAxis = secondaryAxis;


			//this.BindingContext = competition_results;
			
			this.BindingContext = new Competition_Results(pastCompetitionParticipations);

			//Initializing column series
			PieSeries series = new PieSeries()
			{
				ColorModel = new ChartColorModel()
				{
					Palette = ChartColorPalette.Custom,
					CustomBrushes = new ChartColorCollection()
					 {
						 Color.FromRgb(231, 188, 64),
						 Color.FromRgb(174, 174, 174),
						 Color.FromRgb(179, 144, 86),
						 Color.FromRgb(88, 191, 237)
					 }
				}
			};
			series.SetBinding(ChartSeries.ItemsSourceProperty, "Data");
			series.XBindingPath = "classificacao";
			series.YBindingPath = "count";

			series.EnableSmartLabels = true;
			series.DataMarkerPosition = CircularSeriesDataMarkerPosition.OutsideExtended;
			series.ConnectorLineType = ConnectorLineType.Bezier;
			series.StartAngle = 75;
			series.EndAngle = 435;

			series.DataMarker = new ChartDataMarker();
			/*series.DataMarker.ShowLabel = true;
			series.DataMarker.LabelStyle.TextColor = Color.White;
			series.DataMarker.LabelStyle.Margin = 0;
			series.DataMarker.LabelStyle.FontSize = 12;

			series.DataMarker.LabelContent = LabelContent.YValue;*/

			//series.EnableDataPointSelection = true;
			//series.SelectedDataPointColor = Color.Red;

			chart.Series.Add(series);

			


			/*ChartLegend legend = new ChartLegend();
			legend.BackgroundColor = Color.FromRgb(245, 245, 240);
			legend.StrokeColor = Color.Black;
			legend.StrokeWidth = 2;
			legend.Margin = new Thickness(5);
			legend.CornerRadius = new ChartCornerRadius(5);
			legend.StrokeDashArray = new double[] { 3, 3 };

			chart.Legend = legend;*/

			return chart;
		}

		public DoPageCS()
		{

			this.initLayout();
			//this.initSpecificLayout();

		}

		async void OnPerfilButtonClicked(object sender, EventArgs e)
		{
			await Navigation.PushAsync(new ProfileCS());
		}

		async Task<List<Competition_Participation>> GetPastCompetitionParticipations()
		{
			Debug.WriteLine("GetPastCompetitionParticipations");
			CompetitionManager competitionManager = new CompetitionManager();

			List<Competition_Participation> pastCompetitionParticipations = await competitionManager.GetPastCompetitionParticipations(App.member.id);


			Debug.WriteLine("GetPastCompetitionParticipations pastCompetitionParticipations.count "+ pastCompetitionParticipations.Count);
			return pastCompetitionParticipations;
		}

		async Task<List<Event_Participation>> GetPastEventParticipations()
		{
			Debug.WriteLine("GetPastCompetitionParticipations");
			EventManager eventManager = new EventManager();

			List<Event_Participation> pastEventParticipations = await eventManager.GetPastEventParticipations(App.member.id);

			return pastEventParticipations;
		}


		async void OnPremiosButtonClicked(object sender, EventArgs e)
		{
			App.DO_activetab = "premios";
			premiosButton.activate();
			graduacoesButton.deactivate();
			palmaresButton.deactivate();
			participacoesEventosButton.deactivate();

			relativeLayout.Children.Add(premiosRelativeLayout,
				xConstraint: Constraint.Constant(0),
				yConstraint: Constraint.Constant(80),
				widthConstraint: Constraint.RelativeToParent((parent) =>
				{
					return (parent.Width);
				}),
				heightConstraint: Constraint.RelativeToParent((parent) =>
				{
					return (parent.Height) - 80;
				}));


			relativeLayout.Children.Remove(graduacoesRelativeLayout);
			relativeLayout.Children.Remove(palmaresRelativeLayout);
			relativeLayout.Children.Remove(collectionViewParticipacoesEventos);
		}

		async void OnGraduacoesButtonClicked(object sender, EventArgs e)
		{
			App.DO_activetab = "graduacoes";
			premiosButton.deactivate();
			graduacoesButton.activate();
			palmaresButton.deactivate();
			participacoesEventosButton.deactivate();

			relativeLayout.Children.Add(graduacoesRelativeLayout,
				xConstraint: Constraint.Constant(0),
				yConstraint: Constraint.Constant(80),
				widthConstraint: Constraint.RelativeToParent((parent) =>
				{
					return (parent.Width);
				}),
				heightConstraint: Constraint.RelativeToParent((parent) =>
				{
					return (parent.Height) - 80;
				}));

			relativeLayout.Children.Remove(premiosRelativeLayout);
			relativeLayout.Children.Remove(palmaresRelativeLayout);
			relativeLayout.Children.Remove(collectionViewParticipacoesEventos);
		}

		async void OnPalmaresButtonClicked(object sender, EventArgs e)
		{
			App.DO_activetab = "palmares";
			premiosButton.deactivate();
			graduacoesButton.deactivate();
			palmaresButton.activate();
			participacoesEventosButton.deactivate();

			relativeLayout.Children.Add(palmaresRelativeLayout,
				xConstraint: Constraint.Constant(0),
				yConstraint: Constraint.Constant(80),
				widthConstraint: Constraint.RelativeToParent((parent) =>
				{
					return (parent.Width);
				}),
				heightConstraint: Constraint.RelativeToParent((parent) =>
				{
					return (parent.Height-60);
				}));

			relativeLayout.Children.Remove(premiosRelativeLayout);
			relativeLayout.Children.Remove(graduacoesRelativeLayout);
			relativeLayout.Children.Remove(collectionViewParticipacoesEventos);
		}

		async void OnParticipacoesEventosButtonClicked(object sender, EventArgs e)
		{
			App.DO_activetab = "participacoesevento";
			premiosButton.deactivate();
			graduacoesButton.deactivate();
			palmaresButton.deactivate();
			participacoesEventosButton.activate();

			relativeLayout.Children.Add(collectionViewParticipacoesEventos,
					xConstraint: Constraint.Constant(0),
					yConstraint: Constraint.Constant(80),
					widthConstraint: Constraint.RelativeToParent((parent) =>
					{
						return (parent.Width);
					}),
					heightConstraint: Constraint.RelativeToParent((parent) =>
					{
						return (parent.Height) - 80;
					}));


			relativeLayout.Children.Remove(premiosRelativeLayout);
			relativeLayout.Children.Remove(graduacoesRelativeLayout);
			relativeLayout.Children.Remove(palmaresRelativeLayout);
		}


		async void OnCollectionViewParticipacoesEventosSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			Debug.WriteLine("OnCollectionViewParticipacoesEventosSelectionChanged");

			if ((sender as CollectionView).SelectedItem != null)
			{
				Event_Participation event_participation = (sender as CollectionView).SelectedItem as Event_Participation;
				await Navigation.PushAsync(new DetailEventParticipationPageCS(event_participation));
			}
		}

		async void OnCollectionViewParticipacoesCompeticoesSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			Debug.WriteLine("OnCollectionViewParticipacoesCompeticoesSelectionChanged");

			if ((sender as CollectionView).SelectedItem != null)
			{
				Competition_Participation competition_participation = (sender as CollectionView).SelectedItem as Competition_Participation;
				await Navigation.PushAsync(new DetailCompetitionResultPageCS(competition_participation));
			}
		}

		async Task<int> GetAwards_Student(Member member)
		{
			Debug.WriteLine("GetAwards_Student");
			AwardManager awardManager = new AwardManager();

			awards = await awardManager.GetAwards_Student(member.id);
			if (awards == null)
			{
				Application.Current.MainPage = new NavigationPage(new LoginPageCS("Verifique a sua ligação à Internet e tente novamente."))
				{
					BarBackgroundColor = Color.FromRgb(15, 15, 15),
					BarTextColor = Color.White
				};
				return -1;
			}

			foreach (Award award in awards)
			{
				if (award.tipo == "aluno_mes")
				{
					award.imagem = "premio_aluno_mes.png";
				}
				else
				{
					award.imagem = "premio_ippon_ouro.png";
				}
			}

			return 1;
		}

	}
}
