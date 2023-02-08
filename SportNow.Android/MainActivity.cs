using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

using Android.Util;
using Android.Content;
using Xamarin.Forms;
using Plugin.FirebasePushNotification;
using Acr.UserDialogs;
using Plugin.CurrentActivity;
using System.Collections.Generic;
using SportNow.Views;
using Android.Content.Res;
using Plugin.DeviceOrientation;
using System.Net;

namespace SportNow.Droid
{


    [Activity(Label = "My IPPON", Icon = "@mipmap/logoaksl", Theme = "@style/MainTheme", Exported = true, LaunchMode = LaunchMode.SingleTask, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize)]//, ScreenOrientation = ScreenOrientation.Portrait)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        private bool IsNotification = false;
        private object NotificationData;

        protected override void OnCreate(Bundle savedInstanceState)
        {

            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            LoadApplication(new App());
            RequestedOrientation = ScreenOrientation.Portrait;

            UserDialogs.Init(this);

            FirebasePushNotificationManager.ProcessIntent(this, Intent);

            /* //Set the default notification channel for your app when running Android Oreo
             if (Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.O)
             {
                 //Change for your default notification channel id here
                 FirebasePushNotificationManager.DefaultNotificationChannelId = "FirebasePushNotificationChannel";

                 //Change for your default notification channel name here
                 FirebasePushNotificationManager.DefaultNotificationChannelName = "General";
             }

             FirebasePushNotificationManager.NotificationActivityType = typeof(MainActivity);
             //If debug you should reset the token each time.
 #if DEBUG
             FirebasePushNotificationManager.Initialize(this, false);
 #else
               FirebasePushNotificationManager.Initialize(this,false);
 #endif
            */
            /*//Handle notification when app is closed here - ELES DIZEM ISTO MAS EU SÒ APANHO COISAS AQUI QD A APP ESTÁ ABERTA
            CrossFirebasePushNotification.Current.OnNotificationReceived += (s, p) =>
            {
                Log.Debug("HUGO", "OnNotificationReceived Activity");

            };


            //Handle notification when app is closed here - ELES DIZEM ISTO MAS EU SÒ APANHO COISAS AQUI QD A APP ESTÁ ABERTA
            CrossFirebasePushNotification.Current.OnNotificationOpened += (s, p) =>
            {
                Log.Debug("HUGO", "OnNotificationOpened Activity");

            };

            FirebasePushNotificationManager.ProcessIntent(this, Intent);*/

            ServicePointManager.ServerCertificateValidationCallback += (o, cert, chain, errors) => true;

            CrossCurrentActivity.Current.Init(Application);

        }

        protected override void OnNewIntent(Intent intent)
       {
           base.OnNewIntent(intent);
           FirebasePushNotificationManager.ProcessIntent(this,intent);
       }

        public override void OnConfigurationChanged(Configuration newConfig)
        {
            base.OnConfigurationChanged(newConfig);

            DeviceOrientationImplementation.NotifyOrientationChange(newConfig.Orientation);
        }

    }
}