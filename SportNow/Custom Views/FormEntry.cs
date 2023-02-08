using System;
using Xamarin.Forms;

namespace SportNow.CustomViews
{
    public class FormEntry: Frame
    {

        public Entry entry;
        //public string Text {get; set; }


        public FormEntry(string text, string placeholder, double width)
        {
            createFormEntry(text, placeholder, width, Keyboard.Text);
        }

        public FormEntry(string text, string placeholder, double width, Keyboard keyboard)
        {
            createFormEntry(text, placeholder, width, keyboard);
        }

        public void createFormEntry(string text, string placeholder, double width, Keyboard keyboard)
        {

            //this.BackgroundColor = Color.FromRgb(25, 25, 25);
            this.BackgroundColor = Color.FromRgb(25, 25, 25);
            this.BorderColor = Color.LightGray;

            this.CornerRadius = 10;
            this.IsClippedToBounds = true;
            this.Padding = new Thickness(10,2,10,2);
            this.WidthRequest = width;
            this.WidthRequest = width;
            this.HeightRequest = 45 * App.screenHeightAdapter;
            this.HasShadow = false;

            //USERNAME ENTRY
            entry = new Entry
            {
                //Text = "tete@hotmail.com",
                Text = text,
                TextColor = Color.White,
                BackgroundColor = Color.FromRgb(25, 25, 25),
                Placeholder = placeholder,
                PlaceholderColor = Color.Gray,
                HorizontalOptions = LayoutOptions.Start,
                WidthRequest = width,
                FontSize = App.formValueFontSize,
                Keyboard = keyboard
            };
            this.Content = entry;

        }
    }
}
