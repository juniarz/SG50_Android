using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Android.Media;
using RestSharp;
using Java.Net;
using Android.Views;
using Android.Widget;
using Android.App;
using Android.Content;
using Android.Graphics;
using System.Threading.Tasks;
using System.Threading;

namespace SG50
{
    class CustomFeedsAdapter : BaseAdapter<JToken>
    {
        List<JToken> items;
        Activity context;
        Bitmap imageBitmap;

        public CustomFeedsAdapter(Activity context, List<JToken> items)
            : base()
        {
            this.context = context;
            this.items = items;

        }
        public override long GetItemId(int position)
        {
            return position;
        }
        public override JToken this[int position]
        {
            get { return items[position]; }
        }
        public override int Count
        {
            get { return items.Count; }
        }
        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View view = context.LayoutInflater.Inflate(Resource.Layout.FeedsItem, null);

            TextView titleView = view.FindViewById<TextView>(Resource.Id.TV_TITLE);
            TextView userView = view.FindViewById<TextView>(Resource.Id.TV_POSTED);
            TextView likesView = view.FindViewById<TextView>(Resource.Id.TV_LIKES);
            VideoView videoView = view.FindViewById<VideoView>(Resource.Id.VideoPlayer);
            JToken feedData = items[position];


            ImageView PlayBtn = view.FindViewById<ImageView>(Resource.Id.BTN_play);

            videoView.Visibility = ViewStates.Gone;

            PlayBtn.Click += (o, e) =>
            {
                videoView.Visibility = ViewStates.Visible;
                PlayBtn.Visibility = ViewStates.Gone;

                MediaController mc = new MediaController(context);
                videoView.SetMediaController(new MediaController(context));
                mc.SetMediaPlayer(videoView);
                videoView.Start();
                mc.SetAnchorView(videoView);

                videoView.Completion += delegate
                {
                    videoView.Visibility = ViewStates.Gone;
                    PlayBtn.Visibility = ViewStates.Visible;
                };
            };

            Button LikeBtn = view.FindViewById<Button>(Resource.Id.BTN_LIKE);
            if (Convert.ToBoolean(feedData["liked"].ToString()) == true)
            {
                LikeBtn.SetTextColor(Android.Graphics.Color.WhiteSmoke);
            }
            else
            {
                LikeBtn.SetTextColor(Android.Graphics.Color.DarkGray);
            }

            LikeBtn.Click += (o, e) =>
            {
                if (Convert.ToBoolean(feedData["liked"].ToString()))
                {
                    APITask task = new APITask("feed/" + feedData["present_id"] + "/unlike");
                    APIArgs args = new APIArgs();
                    IRestResponse response = task.Call(args);

                    if (response.ErrorException == null)
                    {
                        context.RunOnUiThread(() =>
                        {
                            feedData["likes"] = int.Parse(feedData["likes"].ToString()) - 1;
                            likesView.Text = int.Parse(feedData["likes"].ToString()) + " Likes";
                            feedData["liked"] = false;
                            LikeBtn.SetTextColor(Android.Graphics.Color.DarkGray);
                        });
                    }
                    else
                    {
                        Console.WriteLine(response.ErrorException.Message);
                    }
                }
                else
                {
                    APITask task = new APITask("feed/" + feedData["present_id"] + "/like");
                    APIArgs args = new APIArgs();
                    IRestResponse response = task.Call(args);

                    if (response.ErrorException == null)
                    {
                        context.RunOnUiThread(() =>
                        {
                            feedData["likes"] = int.Parse(feedData["likes"].ToString()) + 1;
                            likesView.Text = int.Parse(feedData["likes"].ToString()) + " Likes";
                            feedData["liked"] = true;
                            LikeBtn.SetTextColor(Android.Graphics.Color.WhiteSmoke);
                        });
                    }
                    else
                    {
                        Console.WriteLine(response.ErrorException.Message);
                    }
                }
            };

            ImageView FlagBtn = view.FindViewById<ImageView>(Resource.Id.BTN_FLAG);
            FlagBtn.Click += (o, e) =>
            {
                // Build the dialog.
                Android.App.AlertDialog.Builder builder = new AlertDialog.Builder(context);
                builder.SetPositiveButton("Yes", (EventHandler<DialogClickEventArgs>)null);
                builder.SetNegativeButton("No", (EventHandler<DialogClickEventArgs>)null);
                AlertDialog alertDialog = builder.Create();
                alertDialog.SetMessage("Flag as inappropriate?");
                alertDialog.SetTitle("Are you sure?");

                alertDialog.Show();

                // Get the buttons.
                var yesBtn = alertDialog.GetButton((int)DialogButtonType.Positive);
                var noBtn = alertDialog.GetButton((int)DialogButtonType.Negative);

                // Assign our handlers.
                yesBtn.Click += (sender, ev) =>
                {
                    // Don't dismiss dialog.
                    APITask task = new APITask("feed/" + feedData["present_id"] + "/flag");
                    APIArgs args = new APIArgs();
                    IRestResponse response = task.Call(args);

                    if (response.ErrorException == null)
                    {
                        context.RunOnUiThread(() =>
                        {
                            Console.WriteLine("Flag:" + feedData["present_id"].ToString());
                            alertDialog.Dismiss();
                        });
                    }
                    else
                    {
                        Console.WriteLine(response.ErrorException.Message);
                    }
                };

                noBtn.Click += (sender, ev) =>
                {
                    alertDialog.Dismiss();
                };
            };

            titleView.Text = feedData["title"].ToString();
            userView.Text = "Uploaded by: " + feedData["user"].ToString();
            likesView.Text = int.Parse(feedData["likes"].ToString()) + " Likes";
            videoView.SetVideoURI(Android.Net.Uri.Parse(feedData["video"]["url"].ToString()));

            return view;
        }
    }
}
