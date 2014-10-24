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
    [Activity(Label = "SG50", MainLauncher = true)]
    public class MainActivity : Activity
    {
        protected async override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            ListView listView = FindViewById<ListView>(Resource.Id.listView1);

            APITask task = new APITask("/feed");

            APIArgs args = new APIArgs();
            args.Parameters.Add("accesstoken", "acdcb58208f767fc204f36ecd74afc30");

            IRestResponse response = await task.CallAsync(args);
             
            if (response.ErrorException == null)
            {
                List<JObject> items = new List<JObject>();
                var data = JObject.Parse(response.Content);

                foreach (JObject obj in data.Values())
                {
                    items.Add(obj);
                }
                Console.WriteLine("Feeds Count: " + items.Count);

                RunOnUiThread(() => {
                    listView.Adapter = new CustomFeedsAdapter(this, items);
                });
            }
            else
            {
                Console.WriteLine(response.ErrorException.Message);
            }
        }
    }
}