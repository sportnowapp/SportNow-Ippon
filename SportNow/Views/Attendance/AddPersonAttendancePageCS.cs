using System;
using System.Collections.Generic;
using Xamarin.Forms;
using SportNow.Model;
using SportNow.Services.Data.JSON;
using System.Threading.Tasks;
using System.Diagnostics;
using SportNow.CustomViews;
using System.Collections.ObjectModel;
using Acr.UserDialogs;

namespace SportNow.Views
{
	public class AddPersonAttendancePageCS : ContentPage
	{

		protected override void OnAppearing()
		{
			initSpecificLayout();
		}

		protected override void OnDisappearing()
		{
			this.CleanScreen();
		}

		MenuButton proximosEventosButton;
		MenuButton participacoesEventosButton;

		private RelativeLayout relativeLayout;
		private StackLayout stackButtons;

		private CollectionView collectionViewMembers, collectionViewStudents;

		Class_Schedule class_Schedule;
		List<Member> students;

		//private List<Member> members;

		public void initLayout()
		{
			Title = "ESCOLHER ALUNO";
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
			Debug.Print("AddPersonAttendancePageCS.CleanScreen");
			//valida se os objetos já foram criados antes de os remover
			/*if (collectionViewMembers != null)
            {
				relativeLayout.Children.Remove(collectionViewMembers);
				collectionViewMembers = null;
			}*/

		}

		public async void initSpecificLayout()
		{

			UserDialogs.Instance.ShowLoading("", MaskType.Clear);

			Label titleLabel = new Label { BackgroundColor = Color.Transparent, VerticalTextAlignment = TextAlignment.Center, HorizontalTextAlignment = TextAlignment.Center, FontSize = App.itemTitleFontSize, TextColor = Color.FromRgb(246, 220, 178), LineBreakMode = LineBreakMode.WordWrap };
			titleLabel.Text = "Escolhe o aluno para o qual pretendes adicionar uma presença:";


			relativeLayout.Children.Add(titleLabel,
			xConstraint: Constraint.Constant(0),
			yConstraint: Constraint.Constant(0),
			widthConstraint: Constraint.RelativeToParent((parent) =>
			{
				return (parent.Width);
			}),
			heightConstraint: Constraint.Constant(40 * App.screenHeightAdapter));


			CreateDojoPicker();

			students = await GetStudentsDojo(class_Schedule.dojo);

			

			CreateStudentsColletion();

			UserDialogs.Instance.HideLoading();   //Hide loader
		}

		public async void CreateDojoPicker()
		{
			List<string> dojoList = new List<string>();
			List<Dojo> dojos= await GetAllDojos(); 
			int selectedIndex = 0;
			int selectedIndex_temp = 0;

			foreach (Dojo dojo in dojos)
			{
				dojoList.Add(dojo.name);
				Debug.Print("dojo.name = " + dojo.name + " class_Schedule.dojo=" + class_Schedule.dojo);
				if (dojo.name == class_Schedule.dojo)
                {
					selectedIndex = selectedIndex_temp;
				}
				selectedIndex_temp++;
			}

			
			Debug.Print("selectedIndex = "+ selectedIndex);

			var dojoPicker = new Picker
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
				students = await GetStudentsDojo(dojoPicker.SelectedItem.ToString());
				relativeLayout.Children.Remove(collectionViewStudents);
				collectionViewStudents = null;
				CreateStudentsColletion();

				UserDialogs.Instance.HideLoading();

			};



			relativeLayout.Children.Add(dojoPicker,
				xConstraint: Constraint.Constant(0),
				yConstraint: Constraint.Constant(40 * App.screenHeightAdapter),
				widthConstraint: Constraint.RelativeToParent((parent) =>
				{
					return (parent.Width); // center of image (which is 40 wide)
				}),
				heightConstraint: Constraint.Constant(50));
		}


		public void CreateStudentsColletion()
		{
			Debug.Print("AddPersonAttendancePageCS.CreateStudentsColletion");
			//COLLECTION GRADUACOES
			collectionViewStudents = new CollectionView
			{
				SelectionMode = SelectionMode.Single,
				ItemsSource = students,
				ItemsLayout = new GridItemsLayout(1, ItemsLayoutOrientation.Vertical) { VerticalItemSpacing = 10, HorizontalItemSpacing = 5, },
				EmptyView = new ContentView
				{
					Content = new StackLayout
					{
						Children =
							{
								new Label { Text = "Não tem membros associados.", HorizontalTextAlignment = TextAlignment.Start, TextColor = Color.White, FontSize = App.itemTitleFontSize },
							}
					}
				}
			};

			collectionViewStudents.SelectionChanged += OnCollectionViewStudentsSelectionChanged;

			collectionViewStudents.ItemTemplate = new DataTemplate(() =>
			{
				RelativeLayout itemRelativeLayout = new RelativeLayout
				{
					HeightRequest = 30 * App.screenHeightAdapter
				};

				FormValue numberLabel = new FormValue("");
				numberLabel.label.SetBinding(Label.TextProperty, "number_member");


				itemRelativeLayout.Children.Add(numberLabel,
					xConstraint: Constraint.Constant(0),
					yConstraint: Constraint.Constant(0),
					widthConstraint: Constraint.Constant(50 * App.screenWidthAdapter),
					heightConstraint: Constraint.Constant(30 * App.screenHeightAdapter));

				FormValue nicknameLabel = new FormValue("");
				nicknameLabel.label.SetBinding(Label.TextProperty, "nickname");


				itemRelativeLayout.Children.Add(nicknameLabel,
					xConstraint: Constraint.Constant(55 * App.screenWidthAdapter),
					yConstraint: Constraint.Constant(0),
					widthConstraint: Constraint.RelativeToParent((parent) =>
					{
						return ((parent.Width - (55 * App.screenWidthAdapter)) / 2) - (5 * App.screenWidthAdapter);
					}),
					heightConstraint: Constraint.Constant(30 * App.screenHeightAdapter));

				FormValue dojoLabel = new FormValue("");
				dojoLabel.label.SetBinding(Label.TextProperty, "dojo");

				itemRelativeLayout.Children.Add(dojoLabel,
					xConstraint: Constraint.RelativeToParent((parent) =>
					{
						return (((parent.Width) - ((parent.Width - (55 * App.screenWidthAdapter)) / 2)));
					}),
					yConstraint: Constraint.Constant(0),
					widthConstraint: Constraint.RelativeToParent((parent) =>
					{
						return ((parent.Width - (55 * App.screenWidthAdapter)) / 2) - (5 * App.screenWidthAdapter);
					}),
					heightConstraint: Constraint.Constant(30 * App.screenHeightAdapter));

				return itemRelativeLayout;
			});

			relativeLayout.Children.Add(collectionViewStudents,
			xConstraint: Constraint.Constant(0),
			yConstraint: Constraint.Constant(90 * App.screenHeightAdapter),
			widthConstraint: Constraint.RelativeToParent((parent) =>
			{
				return (parent.Width); // center of image (which is 40 wide)
			}),
			heightConstraint: Constraint.RelativeToParent((parent) =>
			{
				return (parent.Height- (90 * App.screenHeightAdapter)); // 
			}));

		}

		public AddPersonAttendancePageCS(Class_Schedule class_Schedule)
		{
			Debug.WriteLine("AddPersonAttendancePageCS");
			this.class_Schedule = class_Schedule;
			this.initLayout();
			//this.initSpecificLayout();

		}


		async Task<List<Member>> GetStudentsDojo(string dojo)
		{
			MemberManager memberManager = new MemberManager();
			List<Member> students = await memberManager.GetStudentsDojo(App.original_member.id, dojo);

			return students;
		}

		async Task<List<Dojo>> GetAllDojos()
		{
			DojoManager dojoManager = new DojoManager();
			List<Dojo> dojos = await dojoManager.GetAllDojos();

			return dojos;
		}

		async void OnCollectionViewStudentsSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			Debug.WriteLine("AddPersonAttendancePageCS.OnCollectionViewMembersSelectionChanged");

			UserDialogs.Instance.ShowLoading("", MaskType.Clear);

			if ((sender as CollectionView).SelectedItem != null)
			{

				Member member = (sender as CollectionView).SelectedItem as Member;

				ClassManager classmanager = new ClassManager();
				string class_attendance_id = await classmanager.CreateClass_Attendance(member.id, class_Schedule.classid, "fechada", class_Schedule.date);
				Debug.Print("class_attendance_id=" + class_attendance_id);

				await Navigation.PopAsync();

				/*Navigation.InsertPageBefore(new MainTabbedPageCS("", ""), this);
				await Navigation.PopToRootAsync();*/

				//await Navigation.PopAsync();
			}

			UserDialogs.Instance.HideLoading();   //Hide loader
		}
	}
}
