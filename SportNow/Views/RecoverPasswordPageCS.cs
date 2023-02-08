
using System;
using System.Threading.Tasks;
using System.Diagnostics;
using Xamarin.Forms;
using SportNow.Model;
using SportNow.Services.Data.JSON;
using System.Collections.Generic;
using System.Net.Mail;

namespace SportNow.Views
{
	public class RecoverPasswordPageCS : ContentPage
	{

		Button recoverPasswordButton;
		Entry usernameEntry;
		Label messageLabel;

		private RelativeLayout relativeLayout;

		public void initBaseLayout() {
			Title = "RECUPERAR PASSWORD";			
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

			/*var toolbarItem = new ToolbarItem {
				Text = "Sign Up",
			};
			toolbarItem.Clicked += OnSignUpButtonClicked;
			ToolbarItems.Add (toolbarItem);

			messageLabel = new Label ();*/

		}

		public void initSpecificLayout()
		{

			Grid gridLogin = new Grid { Padding = 10 };
			gridLogin.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
			gridLogin.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
			gridLogin.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
			gridLogin.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
			gridLogin.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star}); //GridLength.Auto 


			Label recoverPasswordLabel = new Label
			{
				Text = "Introduza o email para o qual pretende recuperar a Password.",
				HorizontalOptions = LayoutOptions.Center,
				TextColor = Color.White,
				FontSize = App.itemTitleFontSize
			};

			string username = "";

			if (App.Current.Properties.ContainsKey("EMAIL"))
			{
				username = App.Current.Properties["EMAIL"] as string;
			}

			//USERNAME ENTRY
			usernameEntry = new Entry
			{
				//Text = "tete@hotmail.com",
				Text = username,
				TextColor = Color.White,
				BackgroundColor = Color.FromRgb(25, 25, 25),
				Placeholder = "EMAIL",
				HorizontalOptions = LayoutOptions.Start,
				WidthRequest = 300 * App.screenWidthAdapter
			};
			Frame frame_usernameEntry = new Frame {
				BackgroundColor = Color.FromRgb(25, 25, 25),
				BorderColor = Color.LightGray,
				CornerRadius = 10 * (float) App.screenHeightAdapter,
				IsClippedToBounds = true,
				Padding = 0,
				HorizontalOptions = LayoutOptions.Center,
				WidthRequest = 300 * App.screenWidthAdapter
			};
			frame_usernameEntry.Content = usernameEntry;

			//LOGIN BUTTON
			GradientBrush gradient = new LinearGradientBrush
			{
				StartPoint = new Point(0, 0),
				EndPoint = new Point(1, 1),
			};

			gradient.GradientStops.Add(new GradientStop(Color.FromRgb(180,143,86), Convert.ToSingle(0)));
			gradient.GradientStops.Add(new GradientStop(Color.FromRgb(246,220,178), Convert.ToSingle(0.5)));

			recoverPasswordButton = new Button
			{
				Text = "ENVIAR EMAIL",
				Background = gradient,
                TextColor = Color.Black,
				HorizontalOptions = LayoutOptions.Center,
				FontSize = App.itemTitleFontSize,
				WidthRequest = 200 * App.screenWidthAdapter
			};
			recoverPasswordButton.Clicked += OnrecoverPasswordButtonClicked;


			Frame frame_recoverPasswordButton = new Frame {
				BackgroundColor = Color.FromRgb(25, 25, 25),
				BorderColor = Color.LightGray,
				CornerRadius = 10 * (float) App.screenHeightAdapter,
				IsClippedToBounds = true,
				Padding = 0,
				HorizontalOptions = LayoutOptions.Center,
				WidthRequest = 200 * App.screenWidthAdapter
			};

			frame_recoverPasswordButton.Content = recoverPasswordButton;

			messageLabel = new Label {
				HorizontalOptions = LayoutOptions.Center,
				TextColor = Color.Green,
				FontSize = App.itemTitleFontSize
			};

			//RECOVER PASSWORD LABEL

			gridLogin.Children.Add(recoverPasswordLabel, 0, 0);
			gridLogin.Children.Add(frame_usernameEntry, 0, 1);
			gridLogin.Children.Add(frame_recoverPasswordButton, 0, 2);
			gridLogin.Children.Add(messageLabel, 0, 3);

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


		public RecoverPasswordPageCS()
		{

			this.initBaseLayout();
			this.initSpecificLayout();

		}


		async void OnrecoverPasswordButtonClicked(object sender, EventArgs e)
		{
			Debug.WriteLine("OnrecoverPasswordButtonClicked");

			recoverPasswordButton.IsEnabled = false;
			messageLabel.Text = "";
			messageLabel.TextColor = Color.Green;

			MemberManager memberManager = new MemberManager();


			if (IsValidEmail(usernameEntry.Text))
			{
				int result = await memberManager.RecoverPassword(usernameEntry.Text);

				if (result == 1)
				{
					messageLabel.TextColor = Color.Green;
					messageLabel.Text = "Enviámos um email para o endereço indicado em cima com os dados para recuperar a sua password.";
				}
				else if (result == -1)
				{
					messageLabel.TextColor = Color.Red;
					messageLabel.Text = "Houve um erro. O email introduzido não é válido.";
				}
				else 
				{
					messageLabel.TextColor = Color.Red;
					messageLabel.Text = "Houve um erro. Verifique a sua ligação à Internet ou tente novamente mais tarde.";
				}
			}
			else
			{
				messageLabel.TextColor = Color.Red;
				messageLabel.Text = "Houve um erro. O email introduzido não é válido.";
			}
			recoverPasswordButton.IsEnabled = true;
		}

		async Task <string> AreCredentialsCorrect (User user)
		{
			Debug.WriteLine("AreCredentialsCorrect");
			MemberManager memberManager = new MemberManager();

			string loginOk = await memberManager.Login(user);

			return loginOk;
		}

		async Task<List<Member>> GetMembers(User user)
		{
			Debug.WriteLine("AreCredentialsCorrect");
			UserManager userManager = new UserManager();

			List<Member> members;

			members = await userManager.GetMembers(user);

			return members;
			
		}

		public bool IsValidEmail(string emailaddress)
		{
			try
			{
				MailAddress m = new MailAddress(emailaddress);

				return true;
			}
			catch (FormatException)
			{
				return false;
			}
		}

	}
}


