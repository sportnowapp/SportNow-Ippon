using System;
using System.Diagnostics;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace SportNow.CustomViews
{
    public class FormValue : Frame
    {

        public Label label;
        //public string Text {get; set; }

        public FormValue(string Text) {

            this.CornerRadius = 5;
            this.IsClippedToBounds = true;
            BorderColor = Color.FromRgb(246, 220, 178);
			BackgroundColor = Color.Transparent;
            Padding = new Thickness(2, 2, 2, 2);
            //this.MinimumHeightRequest = 50;
            this.HeightRequest = 45 * App.screenHeightAdapter;
            this.VerticalOptions = LayoutOptions.Center;


            label = new Label
            {
                Padding = new Thickness(5,0,5,0),
                Text = Text,
                HorizontalTextAlignment = TextAlignment.Start,
                VerticalTextAlignment = TextAlignment.Center,
                TextColor = Color.White,
                BackgroundColor = Color.FromRgb(25, 25, 25),
                FontSize = App.formValueFontSize,
            }; 

            this.Content = label; // relativeLayout_Button;

        }
    }
}
