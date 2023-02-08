using System;
using System.Collections.Generic;
using Xamarin.Forms;
using SportNow.Model;
using SportNow.Services.Data.JSON;
using System.Threading.Tasks;
using System.Diagnostics;

namespace SportNow.Views
{
	public class IntroPageCS : ContentPage
	{

		private RelativeLayout relativeLayout;

		public List<MainMenuItem> MainMenuItems { get; set; }

		private Member member;

		Label msg;
		Button btn;


		protected override void OnAppearing()
		{
			Constants.ScreenWidth = Application.Current.MainPage.Width;//DeviceDisplay.MainDisplayInfo.Width;
			Constants.ScreenHeight = Application.Current.MainPage.Height; //DeviceDisplay.MainDisplayInfo.Height;
			//Debug.Print("ScreenWidth = "+ Constants.ScreenWidth + " ScreenHeight = " + Constants.ScreenHeight);
		}

		public void initLayout()
		{
			Title = "Home";
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



		public IntroPageCS ()
		{
			this.initLayout();
			
		}
	}
}
