using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Parse;
using System.Threading.Tasks;
using Android.Util;
using System.Collections.Generic;
using System.Collections;
using Facebook;
using Android.Graphics;
using System.Net;
using System.IO;
using System.Text;
using RestSharp;
using Newtonsoft.Json.Linq;

namespace SG50
{
    using System.Threading;

    using Android.App;
    using Android.OS;
    using Android.Preferences;
    [Activity(Label = "SG50", MainLauncher = true, NoHistory = true, Theme = "@android:style/Theme.Holo.Light")]
    public class MainActivity : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.splash);
            ActionBar.Hide(); 
            ISharedPreferences prefs = this.GetSharedPreferences("settings", FileCreationMode.Private);
            Boolean showTutorial = prefs.GetBoolean("showTutorial", true);

            if (showTutorial)
            {
                Intent intent = new Intent(this, typeof(Intro));
                this.StartActivity(intent);
            }
            else
            {
                if (GetAccessToken() != null)
                {
                    Intent intent = new Intent(this, typeof(Feed));
                    this.StartActivity(intent);
                }
                else
                {
                    Intent intent = new Intent(this, typeof(login));
                    this.StartActivity(intent);
                }
            }

            this.Finish();
        }


        public static String GetAccessToken()
        {
            ISharedPreferences prefs = Application.Context.GetSharedPreferences("settings", FileCreationMode.Private);
            return prefs.GetString("X-Accesstoken", null);
        }
    }
}