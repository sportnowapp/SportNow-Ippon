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
	public class EquipamentsPageCS : ContentPage
	{

		protected override void OnAppearing()
		{
			
		}

		protected override void OnDisappearing()
		{
			//this.CleanScreen();
		}

		private RelativeLayout equipamentosRelativeLayout;
		private RelativeLayout relativeLayout;
		private StackLayout stackButtons;

		List<Equipment> equipments, equipmentsKarategi, equipmentsProtecoesCintos, equipmentsMerchandising;
		public List<EquipmentGroup> equipmentsGroupSelected, equipmentsGroupKarategi, equipmentsGroupProtecoesCintos, equipmentsGroupMerchandising;

		private MenuButton karategiButton, protecoescintosButton, merchandisingButton;

		private CollectionView collectionViewEquipments;

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

			NavigationPage.SetBackButtonTitle(this, "");
		}


		public void CleanScreen()
		{
			Debug.Print("CleanScreen");
			//valida se os objetos já foram criados antes de os remover
			if (stackButtons != null)
			{
				relativeLayout.Children.Remove(stackButtons);
				//relativeLayout.Children.Remove(equipamentosRelativeLayout);

				stackButtons = null;
				equipamentosRelativeLayout = null;
			}

		}

		public async void initSpecificLayout()
		{
			await GetEquipmentsData();
			
			CreateStackButtons();
			CreateEquipmentColletionView();

			if (App.EQUIPAMENTOS_activetab == "karategis")
            {
				OnKarategiButtonClicked(null, null);
            }
			else if (App.EQUIPAMENTOS_activetab == "protecoescintos")
			{
				OnProtecoesCintosButtonClicked(null, null);
			}
			else if (App.EQUIPAMENTOS_activetab == "merchandising")
			{
				OnMerchandisingButtonClicked(null, null);
			}
		}


		public async Task GetEquipmentsData()
		{
			equipments = await GetEquipments();
			equipmentsKarategi = new List<Equipment>();
			equipmentsProtecoesCintos = new List<Equipment>();
			equipmentsMerchandising = new List<Equipment>();
			equipmentsGroupKarategi = new List<EquipmentGroup>();
			equipmentsGroupProtecoesCintos= new List<EquipmentGroup>();
			equipmentsGroupMerchandising = new List<EquipmentGroup>();

			foreach (Equipment equipment in equipments)
			{
				equipment.valueFormatted = String.Format("{0:0.00}", equipment.value) + "€";

				if (equipment.type == "karategi")
				{
					equipmentsKarategi.Add(equipment);
					EquipmentGroup equipmentGroup = getSubTypeEquipmentGroup(equipmentsGroupKarategi, equipment.subtype);

					if (equipmentGroup == null)
					{
						List<Equipment> equipments = new List<Equipment>();
						equipments.Add(equipment);
						equipmentsGroupKarategi.Add(new EquipmentGroup(equipment.subtype.ToUpper(), equipments));
					}
					else
					{
						equipmentGroup.Add(equipment);
					}
				}
				else if ((equipment.type == "protecao") | (equipment.type == "cinto"))
				{
					equipmentsProtecoesCintos.Add(equipment);
					EquipmentGroup equipmentGroup = getSubTypeEquipmentGroup(equipmentsGroupProtecoesCintos, equipment.subtype);

					if (equipmentGroup == null)
					{
						List<Equipment> equipments = new List<Equipment>();
						equipments.Add(equipment);
						equipmentsGroupProtecoesCintos.Add(new EquipmentGroup(equipment.subtype.ToUpper(), equipments));
					}
					else
					{
						equipmentGroup.Add(equipment);
					}
				}
				else if (equipment.type == "merchandising")
				{
					equipmentsMerchandising.Add(equipment);
					EquipmentGroup equipmentGroup = getSubTypeEquipmentGroup(equipmentsGroupMerchandising, equipment.subtype);

					if (equipmentGroup == null)
					{
						List<Equipment> equipments = new List<Equipment>();
						equipments.Add(equipment);
						equipmentsGroupMerchandising.Add(new EquipmentGroup(equipment.subtype.ToUpper(), equipments));
					}
					else
					{
						equipmentGroup.Add(equipment);
					}
				}
			}

			/*foreach (EquipmentGroup equipmentGroup in equipmentsGroupKarategi) {
				equipmentGroup.Print();
			}*/
		}

		public EquipmentGroup getSubTypeEquipmentGroup(List<EquipmentGroup> equipmentGroups, string subtype)
		{
			foreach (EquipmentGroup equipmentGroup in equipmentGroups)
			{
				if (equipmentGroup.Name.ToUpper() == subtype.ToUpper())
				{
					return equipmentGroup;
				}
			}
			return null;
		}

		public void CreateEquipmentColletionView()
		{
			collectionViewEquipments = new CollectionView
			{
				SelectionMode = SelectionMode.Single,
				//ItemsSource = equipments,
				IsGrouped = true,
				ItemsLayout = new GridItemsLayout(1, ItemsLayoutOrientation.Vertical),
				ItemSizingStrategy = ItemSizingStrategy.MeasureFirstItem,
				EmptyView = new ContentView
				{
					Content = new StackLayout
					{
						Children =
						{
							new Label { Text = "Não existem equipamentos disponíveis de mometo.", HorizontalTextAlignment = TextAlignment.Center, TextColor = Color.Red, FontSize = 20 },
						}
					}
				}
			};

			
			collectionViewEquipments.SelectionChanged += OnCollectionViewEquipmentsSelectionChanged;

			collectionViewEquipments.GroupHeaderTemplate = new DataTemplate(() =>
			{
				RelativeLayout itemRelativeLayout = new RelativeLayout
				{
					Margin = new Thickness(3)
				};

				Label nameLabel = new Label { BackgroundColor = Color.Transparent, VerticalTextAlignment = TextAlignment.Center, HorizontalTextAlignment = TextAlignment.Start, FontSize = App.titleFontSize, TextColor = Color.FromRgb(246, 220, 178), LineBreakMode = LineBreakMode.WordWrap };
				nameLabel.SetBinding(Label.TextProperty, "Name");
				//nameLabel.SetBinding(Label.TextColorProperty, "estadoTextColor");

				itemRelativeLayout.Children.Add(nameLabel,
					xConstraint: Constraint.Constant(0),
					yConstraint: Constraint.Constant(0),
					widthConstraint: Constraint.RelativeToParent((parent) =>
					{
						return (parent.Width / 5 * 4) - 10; // center of image (which is 40 wide)
					}),
					heightConstraint: Constraint.Constant(30 * App.screenHeightAdapter));

				Label valueLabel = new Label { Text = "VALOR", BackgroundColor = Color.Transparent, VerticalTextAlignment = TextAlignment.Center, HorizontalTextAlignment = TextAlignment.Center, FontSize = App.titleFontSize, TextColor = Color.FromRgb(246, 220, 178), LineBreakMode = LineBreakMode.WordWrap };

				itemRelativeLayout.Children.Add(valueLabel,
					xConstraint: Constraint.RelativeToParent((parent) =>
					{
						return (parent.Width / 5 * 4); // center of image (which is 40 wide)
					}),
					yConstraint: Constraint.Constant(0),
					widthConstraint: Constraint.RelativeToParent((parent) =>
					{
						return (parent.Width / 5) - 10; // center of image (which is 40 wide)
					}),
					heightConstraint: Constraint.Constant(30 * App.screenHeightAdapter));

				
				return itemRelativeLayout;
			});
			
			collectionViewEquipments.ItemTemplate = new DataTemplate(() =>
			{
				RelativeLayout itemRelativeLayout = new RelativeLayout
				{
					Margin = new Thickness(3)
				};

				Label nameLabel = new Label { BackgroundColor = Color.Transparent, VerticalTextAlignment = TextAlignment.Center, HorizontalTextAlignment = TextAlignment.Start, FontSize = App.itemTitleFontSize, TextColor = Color.White, LineBreakMode = LineBreakMode.WordWrap };
				nameLabel.SetBinding(Label.TextProperty, "name");
				//nameLabel.SetBinding(Label.TextColorProperty, "estadoTextColor");

				Frame nameFrame = new Frame
				{
					BorderColor = Color.FromRgb(246, 220, 178),
					BackgroundColor = Color.Transparent,
					CornerRadius = 10,
					IsClippedToBounds = true,
					Padding = new Thickness(5, 0, 0, 0)
				};
				nameFrame.Content = nameLabel;

				itemRelativeLayout.Children.Add(nameFrame,
					xConstraint: Constraint.Constant(0),
					yConstraint: Constraint.Constant(0),
					widthConstraint: Constraint.RelativeToParent((parent) =>
					{
						return (parent.Width / 5 * 4) - 10; // center of image (which is 40 wide)
					}),
					heightConstraint: Constraint.Constant(30));
				
				Label valueLabel = new Label { BackgroundColor = Color.Transparent, VerticalTextAlignment = TextAlignment.Center, HorizontalTextAlignment = TextAlignment.Center, FontSize = App.itemTitleFontSize, TextColor = Color.White, LineBreakMode = LineBreakMode.WordWrap };
				valueLabel.SetBinding(Label.TextProperty, "valueFormatted");

				Frame valueFrame = new Frame
				{
					BorderColor = Color.FromRgb(246, 220, 178),
					BackgroundColor = Color.Transparent,
					CornerRadius = 10,
					IsClippedToBounds = true,
					Padding = new Thickness(5, 0, 0, 0)
				};
				valueFrame.Content = valueLabel;

				itemRelativeLayout.Children.Add(valueFrame,
					xConstraint: Constraint.RelativeToParent((parent) =>
					{
						return (parent.Width / 5 * 4); // center of image (which is 40 wide)
					}),
					yConstraint: Constraint.Constant(0),
					widthConstraint: Constraint.RelativeToParent((parent) =>
					{
						return (parent.Width / 5) - 10; // center of image (which is 40 wide)
					}),
					heightConstraint: Constraint.Constant(30));

				
				return itemRelativeLayout;
			});

			relativeLayout.Children.Add(collectionViewEquipments,
				xConstraint: Constraint.Constant(0),
				yConstraint: Constraint.Constant(80),
				widthConstraint: Constraint.RelativeToParent((parent) =>
				{
					return (parent.Width); // center of image (which is 40 wide)
				}),
				heightConstraint: Constraint.RelativeToParent((parent) =>
				{
					return (parent.Height) - 80; // 
				})
			);
		}


		public void CreateStackButtons()
		{
			var width = Constants.ScreenWidth;
			var buttonWidth = (width-50) / 3;


			karategiButton = new MenuButton("KARATE GIs", buttonWidth, 60);
			karategiButton.button.Clicked += OnKarategiButtonClicked;

			protecoescintosButton = new MenuButton("PROTEÇÕES E CINTOS", buttonWidth, 60);
			protecoescintosButton.button.Clicked += OnProtecoesCintosButtonClicked;

			merchandisingButton = new MenuButton("MERCHANDISING", buttonWidth, 60);
			merchandisingButton.button.Clicked += OnMerchandisingButtonClicked;

			stackButtons = new StackLayout
			{
				//WidthRequest = 370,
				Margin = new Thickness(0),
				Spacing = 5,
				Orientation = StackOrientation.Horizontal,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.FillAndExpand,
				HeightRequest = 40,
				Children =
				{
					karategiButton,
					protecoescintosButton,
					merchandisingButton
				}
			};

			relativeLayout.Children.Add(stackButtons,
			xConstraint: Constraint.Constant(0),
			yConstraint: Constraint.Constant(0),
			widthConstraint: Constraint.RelativeToParent((parent) =>
			{
				return (parent.Width);
			}),
			heightConstraint: Constraint.Constant(40));

		}



		public EquipamentsPageCS(string type)
		{
			App.EQUIPAMENTOS_activetab = type;
			this.initLayout();
			initSpecificLayout();
		}

		async Task<List<Equipment>> GetEquipments()
		{
			EquipmentManager equipmentManager = new EquipmentManager();
			List<Equipment> equipments = await equipmentManager.GetEquipments();
			if (equipments == null)
			{
								Application.Current.MainPage = new NavigationPage(new LoginPageCS("Verifique a sua ligação à Internet e tente novamente."))
				{
					BarBackgroundColor = Color.FromRgb(15, 15, 15),
					BarTextColor = Color.White
				};
				return null;
			}
			return equipments;
		}

		async void OnKarategiButtonClicked(object sender, EventArgs e)
		{
			karategiButton.activate();
			protecoescintosButton.deactivate();
			merchandisingButton.deactivate();

			collectionViewEquipments.ItemsSource = equipmentsGroupKarategi;
			App.EQUIPAMENTOS_activetab = "karategis";

		}

		async void OnProtecoesCintosButtonClicked(object sender, EventArgs e)
		{
			karategiButton.deactivate();
			protecoescintosButton.activate();
			merchandisingButton.deactivate();

			collectionViewEquipments.ItemsSource = equipmentsGroupProtecoesCintos;

			App.EQUIPAMENTOS_activetab = "protecoescintos";
		}

		async void OnMerchandisingButtonClicked(object sender, EventArgs e)
		{
			karategiButton.deactivate();
			protecoescintosButton.deactivate();
			merchandisingButton.activate();

			collectionViewEquipments.ItemsSource = equipmentsGroupMerchandising;
			App.EQUIPAMENTOS_activetab = "merchandising";
		}

		async void OnCollectionViewEquipmentsSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			Debug.WriteLine("OnCollectionViewEquipmentsSelectionChanged");
			if ((sender as CollectionView).SelectedItem != null)
			{

				Equipment equipment = (sender as CollectionView).SelectedItem as Equipment;

				
				await Navigation.PushAsync(new EquipamentsOrderPageCS(equipment));
				

				Debug.WriteLine("OnCollectionViewEquipmentsSelectionChanged equipment = " + equipment.name);

			}
		}

	}
}
