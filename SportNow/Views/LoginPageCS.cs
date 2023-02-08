using System;
using System.Threading.Tasks;
using System.Diagnostics;
using Xamarin.Forms;
using SportNow.Model;
using SportNow.Services.Data.JSON;
using System.Collections.Generic;
//using Android.Preferences;
//using Android.Content;
using SportNow.CustomViews;
using Acr.UserDialogs;
using Xamarin.Essentials;
using Plugin.DeviceOrientation;
using Plugin.DeviceOrientation.Abstractions;

namespace SportNow.Views
{
	public class LoginPageCS : ContentPage
	{

		protected async override void OnAppearing()
		{
			base.OnAppearing(); //On Test
			CrossDeviceOrientation.Current.LockOrientation(DeviceOrientations.Portrait);

			var mainDisplayInfo = DeviceDisplay.MainDisplayInfo;
			Constants.ScreenWidth = mainDisplayInfo.Width;
			Constants.ScreenHeight = mainDisplayInfo.Height;
			Debug.Print("AQUI Login - ScreenWidth = " + Constants.ScreenWidth + " ScreenHeight = " + Constants.ScreenHeight + "mainDisplayInfo.Density = " + mainDisplayInfo.Density);

			App.AdaptScreen();
			this.initSpecificLayout();
		}


		Label welcomeLabel;
		Button loginButton;
		FormEntry usernameEntry;
		FormEntryPassword passwordEntry;
		Label messageLabel;

		string password = "";
		string username = "";
		string message = "";

		private RelativeLayout relativeLayout;		

		public void initBaseLayout() {


			//NavigationPage.SetHasNavigationBar(this, false);
			//Title = "Login";			
			this.BackgroundColor = Color.FromRgb(25, 25, 25);

			relativeLayout = new RelativeLayout
			{
				Margin = new Thickness(20)
			};
			Content = relativeLayout;

			relativeLayout.Children.Add(new Image
			{
				Source = "boneco_karate.png",
			},
			xConstraint: Constraint.Constant(0),
			yConstraint: Constraint.Constant(0),
			widthConstraint: Constraint.RelativeToParent((parent) => { return parent.Width; }),
			heightConstraint: Constraint.RelativeToParent((parent) => { return parent.Height; }));

			NavigationPage.SetHasNavigationBar(this, false);

			

			/*var toolbarItem = new ToolbarItem {
				Text = "Sign Up",
			};
			toolbarItem.Clicked += OnSignUpButtonClicked;
			ToolbarItems.Add (toolbarItem);

			messageLabel = new Label ();*/

			NavigationPage.SetBackButtonTitle(this, "");
		}

		public void initSpecificLayout()
		{

			Grid gridLogin = new Grid { Padding = 10 };
			gridLogin.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
			gridLogin.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
			gridLogin.RowDefinitions.Add(new RowDefinition { Height = 40 });
			gridLogin.RowDefinitions.Add(new RowDefinition { Height = 45 });
			gridLogin.RowDefinitions.Add(new RowDefinition { Height = 45 });
			gridLogin.RowDefinitions.Add(new RowDefinition { Height = 60 });
			gridLogin.RowDefinitions.Add(new RowDefinition { Height = 80 });
//			gridLogin.RowDefinitions.Add(new RowDefinition { Height = GridLength.Star });
			gridLogin.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star}); //GridLength.Auto 


			welcomeLabel = new Label
			{
				Text = "BEM VINDO",
				TextColor = Color.White,
				FontSize = 30 * App.screenHeightAdapter,
				HorizontalOptions = LayoutOptions.Center
			};

			
			Image logo_aksl = new Image
			{
				Source = "logo_aksl_round.png",
				HorizontalOptions = LayoutOptions.Center,
				HeightRequest = 224 * App.screenHeightAdapter
			};

			if (App.Current.Properties.ContainsKey("EMAIL"))
			{
				username = App.Current.Properties["EMAIL"] as string;
			}


			//USERNAME ENTRY
			usernameEntry = new FormEntry(username, "EMAIL", 300);

			if (App.Current.Properties.ContainsKey("PASSWORD"))
			{
				password = App.Current.Properties["PASSWORD"] as string;
			}

			//PASSWORD ENTRY
			passwordEntry = new FormEntryPassword(password, "PASSWORD");

			//LOGIN BUTTON
			GradientBrush gradient = new LinearGradientBrush
			{
				StartPoint = new Point(0, 0),
				EndPoint = new Point(1, 1),
			};

			gradient.GradientStops.Add(new GradientStop(Color.FromRgb(180,143,86), Convert.ToSingle(0)));
			gradient.GradientStops.Add(new GradientStop(Color.FromRgb(246,220,178), Convert.ToSingle(0.5)));

			loginButton = new Button
			{
				Text = "LOGIN",
				Background = gradient,
                TextColor = Color.Black,
				HorizontalOptions = LayoutOptions.Center,
				WidthRequest = 200 * App.screenWidthAdapter,
				HeightRequest = 45 * App.screenHeightAdapter
			};
			loginButton.Clicked += OnLoginButtonClicked;


			Frame frame_loginButton = new Frame {
				BackgroundColor = Color.FromRgb(25, 25, 25),
				BorderColor = Color.LightGray,
				CornerRadius = 10 * (float) App.screenWidthAdapter,
				IsClippedToBounds = true,
				Padding = 0,
				HorizontalOptions = LayoutOptions.Center,
				VerticalOptions = LayoutOptions.Center,
				WidthRequest = 200 * App.screenWidthAdapter,
				HeightRequest = 45 * App.screenWidthAdapter
			};
			
			frame_loginButton.Content = loginButton;

			messageLabel = new Label {
				HorizontalOptions = LayoutOptions.Center,
				VerticalOptions = LayoutOptions.End,
				TextColor = Color.Red,
				FontSize = App.itemTitleFontSize
			};

			messageLabel.Text = this.message;

			//RECOVER PASSWORD LABEL
			Label recoverPasswordLabel = new Label
			{
				Text = "Recuperar palavra-passe",
				TextColor = Color.White,
				FontSize = 20,
				HorizontalTextAlignment = TextAlignment.Center,
				VerticalTextAlignment = TextAlignment.Center
			};
			var recoverPasswordLabel_tap = new TapGestureRecognizer();
			recoverPasswordLabel_tap.Tapped += (s, e) =>
			{
				/*Navigation.InsertPageBefore(new RecoverPasswordPageCS(), this);
				Navigation.PopAsync();*/

				 Navigation.PushAsync(new RecoverPasswordPageCS());

			};
			recoverPasswordLabel.GestureRecognizers.Add(recoverPasswordLabel_tap);

			gridLogin.Children.Add(welcomeLabel, 0, 0);
			gridLogin.Children.Add(logo_aksl, 0, 1);
			gridLogin.Children.Add(messageLabel, 0, 2);
			gridLogin.Children.Add(usernameEntry, 0, 3);
			gridLogin.Children.Add(passwordEntry, 0, 4);
			gridLogin.Children.Add(frame_loginButton, 0, 5);
			gridLogin.Children.Add(recoverPasswordLabel, 0, 6);

			relativeLayout.Children.Add(gridLogin,
				xConstraint: Constraint.Constant(0),
				yConstraint: Constraint.Constant(0),
				widthConstraint: Constraint.RelativeToParent((parent) =>
				{
					return (parent.Width); // center of image (which is 40 wide)
				}),
				heightConstraint: Constraint.RelativeToParent((parent) =>
				{
					return (parent.Height) ; // center of image (which is 40 wide)
				})
			);

		}


		public LoginPageCS (string message)
		{
			if (message != "")
			{
				this.message = message;
				//UserDialogs.Instance.Alert(new AlertConfig() { Title = "Erro", Message = message, OkText = "Ok" });
			}
			
			this.initBaseLayout();
			

		}

		async void OnSignUpButtonClicked (object sender, EventArgs e)
		{
			//await Navigation.PushAsync (new SignUpPageCS ());
		}

		async void OnLoginButtonClicked (object sender, EventArgs e)
		{
			Debug.WriteLine("OnLoginButtonClicked");

			loginButton.IsEnabled = false;

			var user = new User {
				Username = usernameEntry.entry.Text,
				Password = passwordEntry.entry.Text
			};


			MemberManager memberManager = new MemberManager();

			UserDialogs.Instance.ShowLoading("", MaskType.Clear);

			var loginResult = await memberManager.Login(user);

			if (loginResult == "1")
			{
				Debug.WriteLine("login ok");

				App.members = await GetMembers(user);

				this.saveUserPassword(user.Username, user.Password);

				if (App.members.Count == 1)
                {
					App.original_member = App.members[0];
					App.member = App.original_member;

					Navigation.InsertPageBefore(new MainTabbedPageCS("",""), this);
					await Navigation.PopAsync();
				}
				else if (App.members.Count > 1)
				{
					Navigation.InsertPageBefore(new SelectMemberPageCS(), this);
					await Navigation.PopAsync();
				}
			}
			else {
				Debug.WriteLine("login nok");
				loginButton.IsEnabled = true;
				passwordEntry.entry.Text = string.Empty;

				if (loginResult == "0")
				{
					Debug.WriteLine("Login falhou. O Utilizador não existe");
					messageLabel.Text = "Login falhou. O Utilizador não existe.";
				}
				else if (loginResult == "-1")
				{
					Debug.WriteLine("Login falhou. A Password está incorreta");
					messageLabel.Text = "Login falhou. A Password está incorreta.";
				}
				else if (loginResult == "-2")
				{
					Debug.WriteLine("Ocorreu um erro. Volte a tentar mais tarde.");
					messageLabel.Text = "Ocorreu um erro. Volte a tentar mais tarde.";
				}
				else
				{
					Debug.WriteLine("Ocorreu um erro. Volte a tentar mais tarde.");
					messageLabel.Text = "Ocorreu um erro. Volte a tentar mais tarde.";
					UserDialogs.Instance.Alert(new AlertConfig() { Title = "Erro", Message = loginResult, OkText = "Ok" });
				}

				this.saveUserPassword(user.Username, user.Password);
			}
			UserDialogs.Instance.HideLoading();   //Hide loader
		}


		async Task<List<Member>> GetMembers(User user)
		{
			Debug.WriteLine("GetMembers");
			MemberManager memberManager = new MemberManager();

			List<Member> members;

			members = await memberManager.GetMembers(user);

			return members;
			
		}

		protected void saveUserPassword(string email, string password)
		{
			Application.Current.Properties.Remove("EMAIL");
			Application.Current.Properties.Remove("PASSWORD");

			Application.Current.Properties.Add("EMAIL", email);
			Application.Current.Properties.Add("PASSWORD", password);
			Application.Current.SavePropertiesAsync();

			username = App.Current.Properties["EMAIL"] as string;
		}
	}
}


