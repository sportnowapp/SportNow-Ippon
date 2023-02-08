using System;
using System.Diagnostics;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace SportNow.CustomViews
{
    /*public class FormValueEdit : Frame
    {

        public Entry entry;
        //public string Text {get; set; }

        public FormValueEdit(string text)
        {

            //this.BackgroundColor = Color.FromRgb(25, 25, 25);
            this.BackgroundColor = Color.FromRgb(25, 25, 25);
            this.BorderColor = Color.LightGray;

            this.CornerRadius = 10;
            this.IsClippedToBounds = true;
            this.Padding = new Thickness(10, 2, 10, 2);
            //this.WidthRequest = width;
            this.HeightRequest = 30;

            //USERNAME ENTRY
            entry = new Entry
            {
                //Text = "tete@hotmail.com",
                Text = text,
                TextColor = Color.White,
                BackgroundColor = Color.FromRgb(25, 25, 25),
                //Placeholder = placeholder,
                HorizontalOptions = LayoutOptions.Start,
                //WidthRequest = width,
                FontSize = 18,
            };
            this.Content = entry;

        }
    }*/

     public class FormValueEdit : Frame
     {

         public Entry entry;
         //public string Text {get; set; }

         public FormValueEdit(string Text) {

             this.CornerRadius = 5 * (float) App.screenHeightAdapter;
             this.IsClippedToBounds = true;
             BorderColor = Color.FromRgb(246, 220, 178);
             BackgroundColor = Color.Transparent;
             this.Padding = new Thickness(1, 2, 2, 2);
             //this.MinimumHeightRequest = 50;
             this.HeightRequest = 45 * App.screenHeightAdapter;
             this.VerticalOptions = LayoutOptions.Center;


             entry = new Entry
             {
                 //Padding = new Thickness(5,0,5,0),
                 Text = Text,
                 HorizontalTextAlignment = TextAlignment.Start,
                 //VerticalTextAlignment = TextAlignment.Center,
                 TextColor = Color.White,
                 BackgroundColor = Color.FromRgb(25, 25, 25),
                 FontSize = App.formValueFontSize,
                 //HeightRequest = 30
             };
            
             this.Content = entry; // relativeLayout_Button;

         }
     }
}
