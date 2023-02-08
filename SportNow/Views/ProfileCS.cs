using System;
using System.Collections.Generic;
using Xamarin.Forms;
using SportNow.Model;
using SportNow.Services.Data.JSON;
using System.Threading.Tasks;
using System.Diagnostics;
using SportNow.CustomViews;
using Xamarin.Essentials;
using Acr.UserDialogs;
using System.Text.RegularExpressions;

namespace SportNow.Views
{
    public class ProfileCS : ContentPage
	{

		protected async override void OnAppearing()
		{
			var result = await GetCurrentFees(member);

			estadoQuotaImage.Source = "iconinativo.png";

			if (App.member != null)
			{ 
				if (App.member.currentFee != null)
				{
					if ((App.member.currentFee.estado == "fechado") | (App.member.currentFee.estado == "recebido") | (App.member.currentFee.estado == "confirmado"))
					{
						estadoQuotaImage.Source = "iconcheck.png";
					}
				}
			}
		}


		protected async override void OnDisappearing()
		{
			if (changeMember == false)
            {
				await UpdateMemberInfo();
			} 
			
		}

		Image estadoQuotaImage;

		private RelativeLayout relativeLayout;
		private ScrollView scrollView;

		private Member member;

		MenuButton geralButton;
		MenuButton identificacaoButton;
		MenuButton moradaButton;
		MenuButton encEducacaoButton;


		StackLayout stackButtons;
		private Grid gridGeral;
		private Grid gridIdentificacao;
		private Grid gridMorada;
		private Grid gridEncEducacao;
		private Grid gridButtons;

		FormValueEdit nameValue;
		FormValue emailValue;
		FormValueEdit phoneValue;
		FormValueEdit addressValue;
		FormValueEdit cityValue;
		FormValueEdit postalcodeValue;
		FormValueEdit EncEducacao1NomeValue;
		FormValueEdit EncEducacao1PhoneValue;
		FormValueEdit EncEducacao1MailValue;
		FormValueEdit EncEducacao2NomeValue;
		FormValueEdit EncEducacao2PhoneValue;
		FormValueEdit EncEducacao2MailValue;

		bool changeMember = false;

		bool enteringPage = true;

		Button activateButton;

		Label currentVersionLabel;

		public void initLayout()
		{
			Title = "PERFIL";
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
				Text = "Logout"
			};
			toolbarItem.Clicked += OnLogoutButtonClicked;
			ToolbarItems.Add(toolbarItem);

		}


		public async void initSpecificLayout()
		{

			member = App.member;

			scrollView = new ScrollView { Orientation = ScrollOrientation.Vertical };

			relativeLayout.Children.Add(scrollView,
				xConstraint: Constraint.Constant(0),
				yConstraint: Constraint.Constant(260 * App.screenHeightAdapter),
				widthConstraint: Constraint.RelativeToParent((parent) =>
				{
					return (parent.Width); // center of image (which is 40 wide)
				}),
				heightConstraint: Constraint.RelativeToParent((parent) =>
				{
					return (parent.Height) - 320 * App.screenHeightAdapter; // center of image (which is 40 wide)
				})
			);


			int countStudents = App.original_member.students_count;

			CreatePhoto();
			CreateQuota();
			CreateGraduacao();
			CreateStackButtons();
			CreateGridGeral();
			CreateGridIdentificacao();
			CreateGridMorada();
			CreateGridEncEducacao();
			CreateGridButtons();

			/*gridIdentificacao.IsVisible = false;
			gridMorada.IsVisible = false;
			gridEncEducacao.IsVisible = false;*/
			OnGeralButtonClicked(null, null);
		}

		public void CreatePhoto()
		{
			Debug.Print("CreatePhoto " + Constants.images_URL + App.member.id + "_photo");
			Image memberPhotoImage = new Image
			{
				Source = new UriImageSource
				{
					Uri = new Uri(Constants.images_URL + App.member.id + "_photo"),
					CachingEnabled = true,
					CacheValidity = new TimeSpan(5, 0, 0, 0)
				}
			};

			relativeLayout.Children.Add(memberPhotoImage,
			xConstraint: Constraint.Constant(0),
			yConstraint: Constraint.Constant(0),
			widthConstraint: Constraint.Constant(180 * App.screenHeightAdapter),
			heightConstraint: Constraint.Constant(180 * App.screenHeightAdapter) // size of screen -80
			);

		}

		public async void CreateQuota()
		{

			var result = await GetCurrentFees(member);

			bool hasQuotaPayed = false;

			if (App.member.currentFee != null)
			{
				if ((App.member.currentFee.estado == "fechado") | (App.member.currentFee.estado == "recebido"))
				{
					hasQuotaPayed = true;
				}
			}

			Frame quotasFrame = new Frame
			{
				CornerRadius = 5 * (float) App.screenHeightAdapter,
				IsClippedToBounds = true,
				BorderColor = Color.FromRgb(182, 145, 89),
				BackgroundColor = Color.Transparent,
				Padding = new Thickness(2, 2, 2, 2),
				HeightRequest = 70 * App.screenHeightAdapter,
				VerticalOptions = LayoutOptions.Center,
			};

			var tapGestureRecognizer_quotasFrame = new TapGestureRecognizer();
			tapGestureRecognizer_quotasFrame.Tapped += async (s, e) => {
				await Navigation.PushAsync(new QuotasListPageCS());
			};
			quotasFrame.GestureRecognizers.Add(tapGestureRecognizer_quotasFrame);

			RelativeLayout quotasRelativeLayout = new RelativeLayout
			{
				Margin = new Thickness(0)
			};
			quotasFrame.Content = quotasRelativeLayout;

			string logoFeeFileName = "", estadoImageFileName = "";


			if (hasQuotaPayed == true)
			{
				logoFeeFileName = "fnkpikp.png";
				estadoImageFileName = "iconcheck.png";
			}
			else if (hasQuotaPayed == false)
            {
				logoFeeFileName = "fnkpikp.png";
				estadoImageFileName = "iconinativo.png"; 
			}

			Image LogoFee = new Image
			{
				Source = logoFeeFileName,
				//WidthRequest = 100,
				HorizontalOptions = LayoutOptions.Center
			};
			quotasRelativeLayout.Children.Add(LogoFee,
				xConstraint: Constraint.RelativeToParent((parent) =>
				{
					return (parent.Width/2) - 39 * App.screenHeightAdapter;
				}),
				yConstraint: Constraint.Constant(5 * App.screenHeightAdapter),
				heightConstraint: Constraint.Constant(30 * App.screenHeightAdapter));

			estadoQuotaImage = new Image
			{
				Source = estadoImageFileName,
				WidthRequest = 20 * App.screenWidthAdapter
			};

			quotasRelativeLayout.Children.Add(estadoQuotaImage,
				xConstraint: Constraint.RelativeToParent((parent) =>
				{
					return (parent.Width) - 20 * App.screenHeightAdapter;
				}),
				yConstraint: Constraint.Constant(0),
				widthConstraint: Constraint.Constant(20 * App.screenHeightAdapter),
				heightConstraint: Constraint.Constant(20 * App.screenHeightAdapter));

			Label feeLabel = new Label
			{
				Text = "QUOTA "+ DateTime.Now.Year,
				VerticalTextAlignment = TextAlignment.Center,
				HorizontalTextAlignment = TextAlignment.Center,
				TextColor = Color.White,
				LineBreakMode = LineBreakMode.NoWrap,
				FontSize = App.itemTitleFontSize
			};
			quotasRelativeLayout.Children.Add(feeLabel,
				xConstraint: Constraint.Constant(0),
				yConstraint: Constraint.RelativeToParent((parent) =>
				{
					return (parent.Height) - 30 * App.screenHeightAdapter; // center of image (which is 40 wide)
				}),
				widthConstraint: Constraint.RelativeToParent((parent) =>
				{
					return (parent.Width); // center of image (which is 40 wide)
				}),
				heightConstraint: Constraint.Constant(30 * App.screenHeightAdapter));

			relativeLayout.Children.Add(quotasFrame,
				xConstraint: Constraint.Constant(200 * App.screenHeightAdapter),
				yConstraint: Constraint.Constant(10 * App.screenHeightAdapter),
				widthConstraint: Constraint.RelativeToParent((parent) =>
				{
					return (parent.Width) - 220 * App.screenHeightAdapter; // center of image (which is 40 wide)
				}),
				heightConstraint: Constraint.Constant(70 * App.screenHeightAdapter));
		}


		public async void CreateGraduacao()
		{
			Frame graduacaoFrame = new Frame
			{
				CornerRadius = 5,
				IsClippedToBounds = true,
				BorderColor = Color.FromRgb(182, 145, 89),
				BackgroundColor = Color.Transparent,
				Padding = new Thickness(2, 2, 2, 2),
				HeightRequest = 70 * App.screenHeightAdapter,
				VerticalOptions = LayoutOptions.Center,
			};

			var tapGestureRecognizer_graduacaoFrame = new TapGestureRecognizer();
			tapGestureRecognizer_graduacaoFrame.Tapped += async (s, e) => {
				await Navigation.PushAsync(new myGradesPageCS("MinhasGraduaçoes"));
			};
			graduacaoFrame.GestureRecognizers.Add(tapGestureRecognizer_graduacaoFrame);

			RelativeLayout graduacaoRelativeLayout = new RelativeLayout
			{
				Margin = new Thickness(0)
			};
			graduacaoFrame.Content = graduacaoRelativeLayout;

			string gradeBeltFileName = "belt_" + App.member.grade + ".png";

			Image gradeBeltImage = new Image
			{
				Source = gradeBeltFileName
			};
			graduacaoRelativeLayout.Children.Add(gradeBeltImage,
				xConstraint: Constraint.RelativeToParent((parent) =>
				{
					return (parent.Width / 2) - 38 * App.screenHeightAdapter;
				}),
				yConstraint: Constraint.Constant(5 * App.screenHeightAdapter),
				heightConstraint: Constraint.Constant(30 * App.screenHeightAdapter));

			Label gradeLabel = new Label
			{
				Text = Constants.grades[member.grade],
				VerticalTextAlignment = TextAlignment.Center,
				HorizontalTextAlignment = TextAlignment.Center,
				TextColor = Color.White,
				LineBreakMode = LineBreakMode.NoWrap,
				FontSize = App.itemTitleFontSize
			};
			graduacaoRelativeLayout.Children.Add(gradeLabel,
				xConstraint: Constraint.Constant(0),
				yConstraint: Constraint.RelativeToParent((parent) =>
				{
					return (parent.Height) - 30 * App.screenHeightAdapter; // center of image (which is 40 wide)
				}),
				widthConstraint: Constraint.RelativeToParent((parent) =>
				{
					return (parent.Width); // center of image (which is 40 wide)
				}),
				heightConstraint: Constraint.Constant(30 * App.screenHeightAdapter));

			relativeLayout.Children.Add(graduacaoFrame,
				xConstraint: Constraint.Constant(200 * App.screenHeightAdapter),
				yConstraint: Constraint.Constant(100 * App.screenHeightAdapter),
				widthConstraint: Constraint.RelativeToParent((parent) =>
				{
					return (parent.Width) - 220 * App.screenHeightAdapter; // center of image (which is 40 wide)
				}),
				heightConstraint: Constraint.Constant(70 * App.screenHeightAdapter));
		}
		

		public ProfileCS()
		{
			Debug.WriteLine("ProfileCS");
			NavigationPage.SetBackButtonTitle(this, "");
			this.initLayout();
			this.initSpecificLayout();

		}

		public void CreateStackButtons() {
			var width = Constants.ScreenWidth;
			Debug.WriteLine("Constants.ScreenWidth " + width);
			var buttonWidth = (width - (50 * App.screenHeightAdapter)) / 4;

			geralButton = new MenuButton("GERAL", buttonWidth, 60);
			geralButton.button.Clicked += OnGeralButtonClicked;
			identificacaoButton = new MenuButton("ID",buttonWidth, 60);
			identificacaoButton.button.Clicked += OnIdentificacaoButtonClicked;
			moradaButton = new MenuButton("CONTATOS", buttonWidth, 60);
			moradaButton.button.Clicked += OnMoradaButtonClicked;
			encEducacaoButton = new MenuButton("E. EDUCAÇÃO",buttonWidth, 60);
			encEducacaoButton.button.Clicked += OnEncEducacaoButtonClicked;

			stackButtons = new StackLayout
			{
				Orientation = StackOrientation.Horizontal,
				HorizontalOptions = LayoutOptions.CenterAndExpand,
				Children =
				{
					geralButton,
					identificacaoButton,
					moradaButton,
					encEducacaoButton
				}
			};

			relativeLayout.Children.Add(stackButtons,
			xConstraint: Constraint.Constant(0),
			yConstraint: Constraint.Constant(185 * App.screenHeightAdapter),
			widthConstraint: Constraint.RelativeToParent((parent) =>
			{
				return (parent.Width);
			}),
			heightConstraint: Constraint.Constant(60 * App.screenHeightAdapter) // size of screen -80
			);

			geralButton.activate();
			identificacaoButton.deactivate();
			moradaButton.deactivate();
			encEducacaoButton.deactivate();
		}

		public void CreateGridButtons()
		{
			var width = Constants.ScreenWidth;
			var buttonWidth = (width - (50 * App.screenHeightAdapter)) / 3;

			gridButtons = new Grid { Padding = 0, HorizontalOptions = LayoutOptions.FillAndExpand };
			gridButtons.RowDefinitions.Add(new RowDefinition { Height = 45 });
			//gridGeral.RowDefinitions.Add(new RowDefinition { Height = 1 });
			gridButtons.ColumnDefinitions.Add(new ColumnDefinition { Width = buttonWidth - 5 });

			/*Button changePasswordButton = new Button { HorizontalOptions = LayoutOptions.Center, BackgroundColor = Color.Transparent, ImageSource = "botaoalterarpass.png", HeightRequest = 30 };
			changePasswordButton.Clicked += OnChangePasswordButtonClicked;*/
			Image changePasswordImage = new Image
			{
				Source = "botaoalterarpass.png",
				WidthRequest = width - 5 * App.screenHeightAdapter,
				HeightRequest = 30 * App.screenHeightAdapter,
				Aspect = Aspect.AspectFit
			};

			TapGestureRecognizer changePasswordImage_tapEvent = new TapGestureRecognizer();
			changePasswordImage_tapEvent.Tapped += OnChangePasswordButtonClicked;
			changePasswordImage.GestureRecognizers.Add(changePasswordImage_tapEvent);
			gridButtons.Children.Add(changePasswordImage, 0, 0);

			int numberOfButtons = 1;

			if (App.members.Count > 1)
			{
				
				Button changeMemberButton = new Button { HorizontalOptions = LayoutOptions.Center, BackgroundColor = Color.Transparent, ImageSource = "botaoalmudarconta.png", HeightRequest = 30 };

				/*gridButtons.ColumnDefinitions.Add(new ColumnDefinition { Width = buttonWidth });
				//RoundButton changeMemberButton = new RoundButton("Login Outro Sócio", buttonWidth-5, 40);
				changeMemberButton.Clicked += OnChangeMemberButtonClicked;*/

				Image changeMemberImage = new Image
				{
					Source = "botaoalmudarconta.png",
					WidthRequest = width - (5 * App.screenHeightAdapter),
					HeightRequest = 30 * App.screenHeightAdapter,
					Aspect = Aspect.AspectFit
				};

				TapGestureRecognizer changeMemberImage_tapEvent = new TapGestureRecognizer();
				changeMemberImage_tapEvent.Tapped += OnChangeMemberButtonClicked;
				changeMemberImage.GestureRecognizers.Add(changeMemberImage_tapEvent);

				gridButtons.Children.Add(changeMemberImage, numberOfButtons, 0);
				numberOfButtons++;

			}

			if (App.original_member.id != App.member.id)
			{
				gridButtons.ColumnDefinitions.Add(new ColumnDefinition { Width = buttonWidth });

				Image originalMemberImage = new Image
				{
					Source = "botaologininstrutor.png",
					WidthRequest = width - (5 * App.screenHeightAdapter),
					HeightRequest = 30 * App.screenHeightAdapter,
					Aspect = Aspect.AspectFit
				};

				TapGestureRecognizer originalMemberImage_tapEvent = new TapGestureRecognizer();
				originalMemberImage_tapEvent.Tapped += OnBackOriginalButtonClicked;
				originalMemberImage.GestureRecognizers.Add(originalMemberImage_tapEvent);


				gridButtons.Children.Add(originalMemberImage, numberOfButtons, 0);
				numberOfButtons++;
			}
			else if (App.original_member.students_count > 1)
			{
				gridButtons.ColumnDefinitions.Add(new ColumnDefinition { Width = buttonWidth });

				Image changeStudentImage = new Image
				{
					Source = "botaologinaluno.png",
					WidthRequest = width - (5 * App.screenHeightAdapter),
					HeightRequest = 30 * App.screenHeightAdapter,
					Aspect = Aspect.AspectFit
				};

				TapGestureRecognizer changeStudentImage_tapEvent = new TapGestureRecognizer();
				changeStudentImage_tapEvent.Tapped += OnChangeStudentButtonClicked;
				changeStudentImage.GestureRecognizers.Add(changeStudentImage_tapEvent);

				gridButtons.Children.Add(changeStudentImage, numberOfButtons, 0);
				numberOfButtons++;
			}

			gridButtons.ColumnDefinitions.Add(new ColumnDefinition { Width = buttonWidth });
			currentVersionLabel = new Label
			{
                Text = "Version " + App.VersionNumber + " " + App.BuildNumber,
                TextColor = Color.White,
				HorizontalTextAlignment = TextAlignment.End,
				VerticalTextAlignment = TextAlignment.End,
				FontSize = 10 * App.screenHeightAdapter
			};

			gridButtons.Children.Add(currentVersionLabel, numberOfButtons, 0);


			relativeLayout.Children.Add(gridButtons,
				xConstraint: Constraint.Constant(0),
				yConstraint: Constraint.RelativeToParent((parent) =>
				{
					return (parent.Height) - 50 * App.screenHeightAdapter; // center of image (which is 40 wide)
				}),
				widthConstraint: Constraint.RelativeToParent((parent) =>
				{
					return (parent.Width); // center of image (which is 40 wide)
				}),
				heightConstraint: Constraint.Constant(40 * App.screenHeightAdapter)
			);

		}

		public void CreateGridGeral() {

			gridGeral = new Grid { Padding = 0, HorizontalOptions = LayoutOptions.FillAndExpand };
			gridGeral.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
			gridGeral.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
			gridGeral.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
			gridGeral.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
			gridGeral.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
			gridGeral.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
			//gridGeral.RowDefinitions.Add(new RowDefinition { Height = 1 });
			gridGeral.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto }); //GridLength.Auto
			gridGeral.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star }); //GridLength.Auto 

			Label number_memberLabel = new FormLabel { Text = "Nº SÓCIO" };
			FormValue number_memberValue = new FormValue(member.number_member);

			FormLabel nameLabel = new FormLabel { Text = "NOME", HorizontalTextAlignment = TextAlignment.Start };
			nameValue = new FormValueEdit(member.name);

			FormLabel dojoLabel = new FormLabel { Text = "DOJO"};
			//FormValue dojoValue = new FormValue (Constants.dojos[member.dojo]);
			FormValue dojoValue = new FormValue(member.dojo);

			FormLabel birthdateLabel = new FormLabel { Text = "NASCIMENTO"};
			FormValue birthdateValue = new FormValue (member.birthdate?.ToString("yyyy-MM-dd"));

			FormLabel registrationdateLabel = new FormLabel { Text = "INSCRIÇÃO"};
			FormValue registrationdateValue = new FormValue (member.registrationdate?.ToString("yyyy-MM-dd"));

			gridGeral.Children.Add(number_memberLabel, 0, 0);
			gridGeral.Children.Add(number_memberValue, 1, 0);

			gridGeral.Children.Add(nameLabel, 0, 1);
			gridGeral.Children.Add(nameValue, 1, 1);

			gridGeral.Children.Add(dojoLabel, 0, 2);
			gridGeral.Children.Add(dojoValue, 1, 2);

			gridGeral.Children.Add(birthdateLabel, 0, 3);
			gridGeral.Children.Add(birthdateValue, 1, 3);

			gridGeral.Children.Add(registrationdateLabel, 0, 4);
			gridGeral.Children.Add(registrationdateValue, 1, 4);
			
			/*relativeLayout.Children.Add(gridGeral,
				xConstraint: Constraint.Constant(0),
				yConstraint: Constraint.Constant(240),
				widthConstraint: Constraint.RelativeToParent((parent) =>
				{
					return (parent.Width); // center of image (which is 40 wide)
				}),
				heightConstraint: Constraint.RelativeToParent((parent) =>
				{
					return (parent.Height) - 230; // center of image (which is 40 wide)
				})
			);*/
		}

		public void CreateGridIdentificacao()
		{

			gridIdentificacao = new Grid { Padding = 10 };
			gridIdentificacao.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
			gridIdentificacao.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
			gridIdentificacao.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
			gridIdentificacao.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto }); //GridLength.Auto
			gridIdentificacao.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star }); //GridLength.Auto 

			FormLabel cc_numberLabel = new FormLabel { Text = "CC" };
			FormValue cc_numberValue = new FormValue (member.cc_number);

			FormLabel nifLabel = new FormLabel { Text = "NIF"};
			FormValue nifValue = new FormValue (member.nif);

			FormLabel fnkpLabel = new FormLabel { Text = "FNKP" };
			FormValue fnkpValue = new FormValue (member.number_fnkp);

			gridIdentificacao.Children.Add(cc_numberLabel, 0, 0);
			gridIdentificacao.Children.Add(cc_numberValue, 1, 0);

			gridIdentificacao.Children.Add(nifLabel, 0, 1);
			gridIdentificacao.Children.Add(nifValue, 1, 1);

			gridIdentificacao.Children.Add(fnkpLabel, 0, 2);
			gridIdentificacao.Children.Add(fnkpValue, 1, 2);

			/*relativeLayout.Children.Add(gridIdentificacao,
				xConstraint: Constraint.Constant(0),
				yConstraint: Constraint.Constant(230),
				widthConstraint: Constraint.RelativeToParent((parent) =>
				{
					return (parent.Width); // center of image (which is 40 wide)
				}),
				heightConstraint: Constraint.RelativeToParent((parent) =>
				{
					return (parent.Height) - 230; // center of image (which is 40 wide)
				})
			);*/
		}

		public void CreateGridMorada()
		{

			gridMorada = new Grid { Padding = 10 };
			gridMorada.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
			gridMorada.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
			gridMorada.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
			gridMorada.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
			gridMorada.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
			gridMorada.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto }); //GridLength.Auto
			gridMorada.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star }); //GridLength.Auto 


			FormLabel emailLabel = new FormLabel { Text = "EMAIL" };
			emailValue = new FormValue(member.email);

			FormLabel phoneLabel = new FormLabel { Text = "TELEFONE" };
			phoneValue = new FormValueEdit(member.phone);

			FormLabel addressLabel = new FormLabel { Text = "MORADA" };
			addressValue = new FormValueEdit(member.address);

			FormLabel cityLabel = new FormLabel { Text = "CIDADE" };
			cityValue = new FormValueEdit(member.city);

			FormLabel postalcodeLabel = new FormLabel { Text = "CÓDIGO POSTAL" };
			postalcodeValue = new FormValueEdit(member.postalcode);


			gridMorada.Children.Add(emailLabel, 0, 0);
			gridMorada.Children.Add(emailValue, 1, 0);

			gridMorada.Children.Add(phoneLabel, 0, 1);
			gridMorada.Children.Add(phoneValue, 1, 1);

			gridMorada.Children.Add(addressLabel, 0, 2);
			gridMorada.Children.Add(addressValue, 1, 2);

			gridMorada.Children.Add(cityLabel, 0, 3);
			gridMorada.Children.Add(cityValue, 1, 3);

			gridMorada.Children.Add(postalcodeLabel, 0, 4);
			gridMorada.Children.Add(postalcodeValue, 1, 4);

			/*relativeLayout.Children.Add(gridMorada,
				xConstraint: Constraint.Constant(0),
				yConstraint: Constraint.Constant(230),
				widthConstraint: Constraint.RelativeToParent((parent) =>
				{
					return (parent.Width); // center of image (which is 40 wide)
				}),
				heightConstraint: Constraint.RelativeToParent((parent) =>
				{
					return (parent.Height) - 230; // center of image (which is 40 wide)
				})
			);*/
		}

		public void CreateGridEncEducacao()
		{

			gridEncEducacao = new Grid { Padding = 10 };
			gridEncEducacao.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
			gridEncEducacao.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
			gridEncEducacao.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
			gridEncEducacao.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
			gridEncEducacao.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
			gridEncEducacao.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
			gridEncEducacao.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
			gridEncEducacao.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
			gridEncEducacao.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto }); 
			gridEncEducacao.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });


			FormLabel EncEducacao1Label = new FormLabel { Text = "ENCARREGADO DE EDUCACAO 1", FontSize = App.itemTitleFontSize };

			FormLabel EncEducacao1NomeLabel = new FormLabel { Text = "NOME" };
			EncEducacao1NomeValue = new FormValueEdit(member.name_enc1);

			FormLabel EncEducacao1PhoneLabel = new FormLabel { Text = "TELEFONE" };
			EncEducacao1PhoneValue = new FormValueEdit(member.phone_enc1);

			FormLabel EncEducacao1MailLabel = new FormLabel { Text = "MAIL" };
			EncEducacao1MailValue = new FormValueEdit(member.mail_enc1);

			FormLabel EncEducacao2Label = new FormLabel { Text = "ENCARREGADO DE EDUCACAO 2", FontSize = App.itemTitleFontSize };

			FormLabel EncEducacao2NomeLabel = new FormLabel { Text = "NOME" };
			EncEducacao2NomeValue = new FormValueEdit(member.name_enc2);

			FormLabel EncEducacao2PhoneLabel = new FormLabel { Text = "TELEFONE" };
			EncEducacao2PhoneValue = new FormValueEdit(member.phone_enc2);

			FormLabel EncEducacao2MailLabel = new FormLabel { Text = "MAIL" };
			EncEducacao2MailValue = new FormValueEdit(member.mail_enc2);


			gridEncEducacao.Children.Add(EncEducacao1Label, 0, 0);
			Grid.SetColumnSpan(EncEducacao1Label, 2);

			gridEncEducacao.Children.Add(EncEducacao1NomeLabel, 0, 1);
			gridEncEducacao.Children.Add(EncEducacao1NomeValue, 1, 1);

			gridEncEducacao.Children.Add(EncEducacao1PhoneLabel, 0, 2);
			gridEncEducacao.Children.Add(EncEducacao1PhoneValue, 1, 2);

			gridEncEducacao.Children.Add(EncEducacao1MailLabel, 0, 3);
			gridEncEducacao.Children.Add(EncEducacao1MailValue, 1, 3);

			gridEncEducacao.Children.Add(EncEducacao2Label, 0, 4);
			Grid.SetColumnSpan(EncEducacao2Label, 2);

			gridEncEducacao.Children.Add(EncEducacao2NomeLabel, 0, 5);
			gridEncEducacao.Children.Add(EncEducacao2NomeValue, 1, 5);

			gridEncEducacao.Children.Add(EncEducacao2PhoneLabel, 0, 6);
			gridEncEducacao.Children.Add(EncEducacao2PhoneValue, 1, 6);

			gridEncEducacao.Children.Add(EncEducacao2MailLabel, 0, 7);
			gridEncEducacao.Children.Add(EncEducacao2MailValue, 1, 7);

			/*relativeLayout.Children.Add(gridEncEducacao,
				xConstraint: Constraint.Constant(0),
				yConstraint: Constraint.Constant(230),
				widthConstraint: Constraint.RelativeToParent((parent) =>
				{
					return (parent.Width); // center of image (which is 40 wide)
				}),
				heightConstraint: Constraint.RelativeToParent((parent) =>
				{
					return (parent.Height) - 230; // center of image (which is 40 wide)
				})
			);*/
		}


		async void OnGeralButtonClicked(object sender, EventArgs e)
		{
			Debug.WriteLine("OnLoginButtonClicked");
			geralButton.activate();
			identificacaoButton.deactivate();
			moradaButton.deactivate();
			encEducacaoButton.deactivate();

			scrollView.Content = gridGeral;

			/*gridGeral.IsVisible = true;
			gridIdentificacao.IsVisible = false;
			gridMorada.IsVisible = false;
			gridEncEducacao.IsVisible = false;*/

			Debug.Print("AQUIIII1");
			if (enteringPage == false) {
				await UpdateMemberInfo();
				enteringPage = false;
			}
			

		}

		async void OnIdentificacaoButtonClicked(object sender, EventArgs e)
		{
			Debug.WriteLine("OnIdentificacaoButtonClicked");
			geralButton.deactivate();
			identificacaoButton.activate();
			moradaButton.deactivate();
			encEducacaoButton.deactivate();

			scrollView.Content = gridIdentificacao;

			/*gridGeral.IsVisible = false;
			gridIdentificacao.IsVisible = true;
			gridMorada.IsVisible = false;
			gridEncEducacao.IsVisible = false;*/

			await UpdateMemberInfo();
		}


		async void OnMoradaButtonClicked(object sender, EventArgs e)
		{
			Debug.WriteLine("OnMoradaButtonClicked");

			geralButton.deactivate();
			identificacaoButton.deactivate();
			moradaButton.activate();
			encEducacaoButton.deactivate();

			scrollView.Content = gridMorada;

			/*gridGeral.IsVisible = false;
			gridIdentificacao.IsVisible = false;
			gridMorada.IsVisible = true;
			gridEncEducacao.IsVisible = false;*/

			await UpdateMemberInfo();
		}

		async void OnEncEducacaoButtonClicked(object sender, EventArgs e)
		{
			Debug.WriteLine("OnEncEducacaoButtonClicked");

			geralButton.deactivate();
			identificacaoButton.deactivate();
			moradaButton.deactivate();
			encEducacaoButton.activate();

			scrollView.Content = gridEncEducacao;

			/*gridGeral.IsVisible = false;
			gridIdentificacao.IsVisible = false;
			gridMorada.IsVisible = false;
			gridEncEducacao.IsVisible = true;*/

			await UpdateMemberInfo();
		}

		async void OnLogoutButtonClicked(object sender, EventArgs e)
		{
			Debug.WriteLine("OnLogoutButtonClicked");

			Application.Current.Properties.Remove("EMAIL");
			Application.Current.Properties.Remove("PASSWORD");
			Application.Current.Properties.Remove("SELECTEDUSER");
			App.member = null;
			App.members = null;



			Application.Current.MainPage = new NavigationPage(new LoginPageCS(""))
			{
				BarBackgroundColor = Color.FromRgb(15, 15, 15),
				BarTextColor = Color.White//FromRgb(75, 75, 75)
			};
		}

		async void OnChangePasswordButtonClicked(object sender, EventArgs e)
		{
			Navigation.PushAsync(new ChangePasswordPageCS(member));
		}

		async void OnChangeMemberButtonClicked(object sender, EventArgs e)
		{
			changeMember = true;

			//Navigation.PushAsync(new SelectMemberPageCS());
			Navigation.InsertPageBefore(new SelectMemberPageCS(), this);
			await Navigation.PopAsync();
			await Navigation.PopAsync();
		}

		async void OnChangeStudentButtonClicked(object sender, EventArgs e)
		{
			changeMember = true;
			Navigation.InsertPageBefore(new SelectStudentPageCS(), this);
			await Navigation.PopAsync();
			await Navigation.PopAsync();

			//Navigation.PushAsync(new SelectStudentPageCS());
		}

		async void OnBackOriginalButtonClicked(object sender, EventArgs e)
		{
			changeMember = true;
			App.member = App.original_member;
			//Navigation.PushAsync(new MainTabbedPageCS(""));
			Navigation.InsertPageBefore(new MainTabbedPageCS("",""), this);
			await Navigation.PopAsync();
			await Navigation.PopAsync();

		}



		async void OnActivateButtonClicked(object sender, EventArgs e)
		{

			activateButton.IsEnabled = false;

			MemberManager memberManager = new MemberManager();

			if (App.member.currentFee is null)
			{
				var result_create = await memberManager.CreateFee(member, DateTime.Now.ToString("yyyy"));
				if (result_create == -1)
				{
					Application.Current.MainPage = new NavigationPage(new LoginPageCS("Verifique a sua ligação à Internet e tente novamente."))
					{
						BarBackgroundColor = Color.FromRgb(15, 15, 15),
						BarTextColor = Color.White
					};
					return;
				}
				var result_get = await GetCurrentFees(member);
				if (result_get == -1)
				{
					Application.Current.MainPage = new NavigationPage(new LoginPageCS("Verifique a sua ligação à Internet e tente novamente."))
					{
						BarBackgroundColor = Color.FromRgb(15, 15, 15),
						BarTextColor = Color.White
					};
					return;
				}
			}

			await Navigation.PushAsync(new QuotasMBPageCS(member));
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
				return -1;
			}
			return result;
		}


		async Task<string> UpdateMemberInfo()
		{
			Debug.Print("AQUIIII UpdateMemberInfo");



			if (App.member != null)
			{
                if (string.IsNullOrEmpty(postalcodeValue.entry.Text)) {
					postalcodeValue.entry.Text = "";
				}

				Debug.WriteLine("UpdateMemberInfo " + nameValue.entry.Text);
				if (nameValue.entry.Text == "")
				{
					nameValue.entry.Text = App.member.name;
					UserDialogs.Instance.Alert(new AlertConfig() { Title = "DADOS INVÁLIDOS", Message = "O nome introduzido não é válido.", OkText = "Ok" });
					return "-1";
				}
				else if (!Regex.IsMatch(phoneValue.entry.Text, @"^\+?(\d[\d-. ]+)?(\([\d-. ]+\))?[\d-. ]+\d$"))
				{
					phoneValue.entry.Text = App.member.phone;
					UserDialogs.Instance.Alert(new AlertConfig() { Title = "DADOS INVÁLIDOS", Message = "O telefone introduzido não é válido.", OkText = "Ok" });
					return "-1";
				}
				else if (!Regex.IsMatch((postalcodeValue.entry.Text), @"^\d{4}-\d{3}$"))
				{
					postalcodeValue.entry.Text = App.member.postalcode;
					UserDialogs.Instance.Alert(new AlertConfig() { Title = "DADOS INVÁLIDOS", Message = "O código postal introduzido não é válido.", OkText = "Ok" });
					return "-1";
				}
				
				Debug.WriteLine("UpdateMemberInfo "+ App.member.name);
				App.member.name = nameValue.entry.Text;
				App.member.email = emailValue.label.Text;
				App.member.phone = phoneValue.entry.Text;
				App.member.address = addressValue.entry.Text;
				App.member.city = cityValue.entry.Text;
				App.member.postalcode = postalcodeValue.entry.Text;
				App.member.name_enc1 = EncEducacao1NomeValue.entry.Text;
				App.member.phone_enc1 = EncEducacao1PhoneValue.entry.Text;
				App.member.mail_enc1 = EncEducacao1MailValue.entry.Text;
				App.member.name_enc2 = EncEducacao2NomeValue.entry.Text;
				App.member.phone_enc2 = EncEducacao2PhoneValue.entry.Text;
				App.member.mail_enc2 = EncEducacao2MailValue.entry.Text;


				MemberManager memberManager = new MemberManager();

				var result = await memberManager.UpdateMemberInfo(App.member);
				if (result == "-1")
				{
					Application.Current.MainPage = new NavigationPage(new LoginPageCS("Verifique a sua ligação à Internet e tente novamente."))
					{
						BarBackgroundColor = Color.FromRgb(15, 15, 15),
						BarTextColor = Color.White
					};
					return "-1";
				}
				return result;
			}
			return "";
		}
	}
}