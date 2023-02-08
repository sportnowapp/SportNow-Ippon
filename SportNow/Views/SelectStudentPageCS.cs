using System;
using System.Collections.Generic;
using Xamarin.Forms;
using SportNow.Model;
using SportNow.Services.Data.JSON;
using System.Threading.Tasks;
using System.Diagnostics;
using SportNow.CustomViews;
using System.Collections.ObjectModel;

namespace SportNow.Views
{
	public class SelectStudentPageCS : ContentPage
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

		//private List<Member> members;

		public void initLayout()
		{
			Debug.Print("SelectStudentPageCS.initLayout");
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

		}


		public void CleanScreen()
		{
			Debug.Print("SelectMemberPageCS.CleanScreen");
			//valida se os objetos já foram criados antes de os remover
			if (stackButtons != null)
            {
				relativeLayout.Children.Remove(stackButtons);
				relativeLayout.Children.Remove(collectionViewMembers);

				stackButtons = null;
				collectionViewMembers = null;
			}

		}

		public async void initSpecificLayout()
		{
			Label titleLabel = new Label { BackgroundColor = Color.Transparent, VerticalTextAlignment = TextAlignment.Center, HorizontalTextAlignment = TextAlignment.Center, FontSize = App.itemTitleFontSize, TextColor = Color.FromRgb(246, 220, 178), LineBreakMode = LineBreakMode.WordWrap };
			titleLabel.Text = "Podes também utilizar a aplicação com a conta de um dos teus alunos:";


			relativeLayout.Children.Add(titleLabel,
			xConstraint: Constraint.Constant(0),
			yConstraint: Constraint.Constant(0),
			widthConstraint: Constraint.RelativeToParent((parent) =>
			{
				return (parent.Width);
			}),
			heightConstraint: Constraint.Constant(60 * App.screenHeightAdapter));

			App.member.students = await GetMemberStudents();

			CreateStudentsColletion();
		}

		public void CreateStudentsColletion()
		{
			

			Debug.Print("SelectMemberPageCS.CreateStudentsColletion");
			//COLLECTION GRADUACOES
			collectionViewStudents = new CollectionView
			{
				SelectionMode = SelectionMode.Single,
				ItemsSource = App.member.students,
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
			yConstraint: Constraint.Constant(50 * App.screenHeightAdapter),
			widthConstraint: Constraint.RelativeToParent((parent) =>
			{
				return (parent.Width); // center of image (which is 40 wide)
			}),
			heightConstraint: Constraint.RelativeToParent((parent) =>
			{
				return (parent.Height- (50 * App.screenHeightAdapter)); // 
			}));

		}

		public SelectStudentPageCS()
		{
			Debug.WriteLine("SelectStudentPageCS");
			this.initLayout();
			//this.initSpecificLayout();

		}


		async Task<List<Member>> GetMemberStudents()
		{
			MemberManager memberManager = new MemberManager();
			List<Member> students = await memberManager.GetMemberStudents(App.original_member.id);

			return students;
		}

		async void OnCollectionViewStudentsSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			Debug.WriteLine("SelectMemberPageCS.OnCollectionViewMembersSelectionChanged");

			if ((sender as CollectionView).SelectedItem != null)
			{

				Member member = (sender as CollectionView).SelectedItem as Member;
				
				App.member = member;
				App.member.students_count = await GetMemberStudents_Count(App.member.id);

				App.Current.MainPage = new NavigationPage(new MainTabbedPageCS("", ""))
				{
					BarBackgroundColor = Color.FromRgb(15, 15, 15),
					BarTextColor = Color.White//FromRgb(75, 75, 75)
				};


				/*Navigation.InsertPageBefore(new MainTabbedPageCS("", ""), this);
				await Navigation.PopToRootAsync();*/

				//await Navigation.PopAsync();
			}
		}

		async Task<int> GetMemberStudents_Count(string memberid)
		{
			Debug.WriteLine("MainTabbedPageCS.GetMemberStudents_Count");
			MemberManager memberManager = new MemberManager();

			var result = await memberManager.GetMemberStudents_Count(memberid);

			return result;
		}
	}
}
