using System;
using System.Diagnostics;
using Xamarin.Forms;

namespace SportNow.CustomViews
{
    public class MenuButton: StackLayout
    {
        public Button button;
        BoxView line;

        public MenuButton(string text, double width, double height)
        {
            if (Device.RuntimePlatform == Device.iOS)
            {
                button = new Button
                {
                    Text = text,
                    TextColor = Color.FromRgb(200, 200, 200),
                    FontSize = App.menuButtonFontSize,
                    WidthRequest = width,
                    HeightRequest = 50 * App.screenHeightAdapter
                };
            }
            else if (Device.RuntimePlatform == Device.Android)
            {
                //this.BackgroundColor = Color.FromRgb(200, 200, 200);
                button = new Button
                {
                    Text = text,
                    BackgroundColor = Color.FromRgb(25, 25, 25),
                    TextColor = Color.FromRgb(200, 200, 200),
                    FontSize = App.menuButtonFontSize,
                    WidthRequest = width,
                    HeightRequest = 50 * App.screenHeightAdapter
                };
            }

            this.Spacing = 0;
            this.Orientation = StackOrientation.Vertical;
            this.MinimumHeightRequest = height * App.screenHeightAdapter;


            //BUTTON


            /*this.BackgroundColor = Color.FromRgb(246, 220, 178);
            this.CornerRadius = 0;
            this.Padding = new Thickness(0,0,0,1);
            this.WidthRequest = width;
            this.HeightRequest = height;
            this.Content = button; // relativeLayout_Button;*/

            line = new BoxView
            {
                Color = Color.FromRgb(246, 220, 178),
                WidthRequest = width,
                HeightRequest = 2 * App.screenHeightAdapter,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.CenterAndExpand
            };
            
            this.Children.Add(button);
            this.Children.Add(line);
        }

        public void activate() {

            this.button.TextColor = Color.FromRgb(246, 220, 178);
            this.line.Color = Color.FromRgb(246, 220, 178);
        }

        public void deactivate()
        {

            this.button.TextColor = Color.FromRgb(200, 200, 200);
            this.line.Color = Color.FromRgb(200, 200, 200);
        }
    }
}
