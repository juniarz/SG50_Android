using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using RestSharp;
using Newtonsoft.Json.Linq;
using Android.Provider;

namespace SG50
{
    [Activity(Label = "SG50", Theme = "@android:style/Theme.Holo.Light")]
    public class Feed : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            ActionBar.Hide(); 
            SetContentView(Resource.Layout.Main);
            GridView GridView = FindViewById<GridView>(Resource.Id.GridHolder);
            APITask task = new APITask("feeds");
            APIArgs args = new APIArgs();
            args.Parameters.Add("page", 0);

            IRestResponse response = task.Call(args, Method.GET);
            if (response.ErrorException == null)
            {
                List<JToken> items = new List<JToken>();

                Console.WriteLine("Res: " + response.Content);
                var data = JArray.Parse(response.Content);

                foreach (JToken obj in data)
                {
                    items.Add(obj);
                }
                RunOnUiThread(() =>
                {
                    GridView.Adapter = new CustomFeedsAdapter(this, items);
                });
            }
            else
            {
                Android.App.AlertDialog.Builder builder = new AlertDialog.Builder(this);
                AlertDialog alertDialog = builder.Create();
                alertDialog.SetTitle("");
                alertDialog.SetIcon(Resource.Drawable.Icon);
                alertDialog.SetMessage("Could not connect to Network");
                Console.WriteLine(response.ErrorException.Message);

                alertDialog.SetButton("Ok", (s, ev) =>
                {
                    alertDialog.Dismiss();
                });
                alertDialog.Show();
            }

            ImageButton btnCamera = this.FindViewById<ImageButton>(Resource.Id.btnCamera);

            btnCamera.Click += (o, e) =>
            {
                Android.App.AlertDialog.Builder builder = new AlertDialog.Builder(this);
                AlertDialog alertDialog = builder.Create();
                alertDialog.SetTitle("Select action");
                alertDialog.SetIcon(Resource.Drawable.Icon);
                alertDialog.SetMessage("");

                //Video
                alertDialog.SetButton("Record a Video", (s, ev) =>
                {
                    try
                    {
                        Intent intent = new Intent(Android.Provider.MediaStore.ActionVideoCapture);
                        StartActivityForResult(intent, 0);
                    }
                    catch (Exception ex) { }
                });

                //Upload
                alertDialog.SetButton2("Upload a Video", (s, ev) =>
                {
                    try
                    {
                        Intent intent = new Intent(Intent.ActionGetContent);
                        intent.SetType("video/mp4");
                        StartActivityForResult(intent, 1);
                    }
                    catch (Exception ex)
                    {
                    }
                });

                alertDialog.Show();
            };
        }
        private enum Operation
        {
            RECORD = 0,
            SELECT_FILE = 1
        }

        protected override void OnActivityResult(int requestCode, Result result, Intent data)
        {
            //RECORDED VIDEO
            if (requestCode == (int)Operation.RECORD && result == Result.Ok) // User canceled operation
            {
                var VideoUri = data.Data;
                // Build the dialog.
                Android.App.AlertDialog.Builder builder = new AlertDialog.Builder(this);

                AlertDialog alertDialog = builder.Create();
                alertDialog.SetIcon(Resource.Drawable.Icon);
                alertDialog.SetTitle("Upload a Video");
                alertDialog.SetMessage("Title of the Video: ");
                EditText inputTitle = new EditText(this);
                alertDialog.SetButton("Upload", (s, ev) =>
                {
                    // Get the URI that points to the selected Video

                    inputTitle.Text = VideoUri.Path.ToString();
                    APITask task = new APITask("feed/upload");

                    APIArgs args = new APIArgs();
                    args.Files.Add("video", getRealPathFromURI(data.Data));
                    args.Parameters.Add("title", inputTitle.Text);

                    IRestResponse response = task.Call(args);

                    Console.WriteLine(response.StatusCode.ToString());
                });
                alertDialog.SetButton2("Cancel", (s, ev) =>
                {
                    alertDialog.Cancel();
                });
                alertDialog.SetView(inputTitle);

                // Create empty event handlers, we will override them manually instead of letting the builder handling the clicks.
                alertDialog.Show();
            }

            //UPLOAD VIDEO
            if (requestCode == (int)Operation.SELECT_FILE && result == Result.Ok) // Upload a video & User selected a video
            {
                // Build the dialog.
                Android.App.AlertDialog.Builder builder = new AlertDialog.Builder(this);

                AlertDialog alertDialog = builder.Create();
                alertDialog.SetIcon(Resource.Drawable.Icon);
                alertDialog.SetTitle("Upload a Video");
                alertDialog.SetMessage("Title of the Video: ");
                EditText inputTitle = new EditText(this);
                alertDialog.SetButton("Upload", (s, ev) =>
                    {
                        // Get the URI that points to the selected Video
                        var VideoUri = data.Data;

                        APITask task = new APITask("feed/upload");

                        APIArgs args = new APIArgs();
                        args.Parameters.Add("title", inputTitle.Text);
                        args.Files.Add("video", getRealPathFromURI(data.Data));

                        task.CallAsync(args, OnSuccess, OnError);
                    });
                alertDialog.SetButton2("Cancel", (s, ev) =>
                    {
                        alertDialog.Cancel();
                    });
                alertDialog.SetView(inputTitle);

                // Create empty event handlers, we will override them manually instead of letting the builder handling the clicks.
                alertDialog.Show();
            }
        }

        public void OnSuccess(IRestResponse response)
        {
            Console.WriteLine("SUCCESS!" + response.StatusCode);
        }
        public void OnError(IRestResponse response)
        {
            Console.WriteLine("ERROR!");
        }

        public String getRealPathFromURI(Android.Net.Uri contentUri)
        {
            String path = null;
            String[] proj = { MediaStore.MediaColumns.Data };
            Android.Database.ICursor cursor = ContentResolver.Query(contentUri, proj, null, null, null);
            if (cursor.MoveToFirst())
            {
                int column_index = cursor.GetColumnIndexOrThrow(MediaStore.MediaColumns.Data);
                path = cursor.GetString(column_index);
            }
            cursor.Close();
            return path;
        }
    }
}