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
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using Android.Media;
using RestSharp;

namespace SG50
{
    class CustomFeedsAdapter : BaseAdapter<JObject>
    {

        List<JObject> items;
        Activity context;


        public CustomFeedsAdapter(Activity context, List<JObject> items)
            : base()
        {
            this.context = context;
            this.items = items;

        }
        public override long GetItemId(int position)
        {
            return position;
        }
        public override JObject this[int position]
        {
            get { return items[position]; }
        }
        public override int Count
        {
            get { return items.Count; }
        }
        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            MyViewHolder holder = null;
            int likes;
            View view = convertView; // re-use an existing view, if one is supplied

            if (view != null)
            {
                holder = view.Tag as MyViewHolder;

            }
            if (holder == null)
            {
                holder = new MyViewHolder();
                view = context.LayoutInflater.Inflate(Resource.Layout.FeedsItem, null);
                holder.Title = view.FindViewById<TextView>(Resource.Id.TV_TITLE);
                holder.User = view.FindViewById<TextView>(Resource.Id.TV_POSTED);
                holder.Likes = view.FindViewById<TextView>(Resource.Id.TV_LIKES);
                holder.Video = view.FindViewById<VideoView>(Resource.Id.VideoView1);
                view.Tag = holder;
            }
            holder.Title.Text = items[position]["title"].ToString();
            holder.User.Text = "Uploaded by: " + items[position]["user"].ToString();
            likes = int.Parse(items[position]["likes"].ToString());
            holder.Likes.Text = likes.ToString() + " Likes";
            var uri = Android.Net.Uri.Parse(items[position]["video"]["url"].ToString());
            holder.Video.SetVideoURI(uri);

            /* VideoView videoView = view.FindViewById<VideoView>(Resource.Id.VideoView1);
             view.FindViewById<TextView>(Resource.Id.TV_TITLE).Text = items[position]["title"].ToString();
             view.FindViewById<TextView>(Resource.Id.TV_POSTED).Text = items[position]["user"].ToString();
             view.FindViewById<TextView>(Resource.Id.TV_LIKES).Text = items[position]["likes"].ToString() + " Likes";
             Console.WriteLine(items[position]["video"]["url"].ToString());
             var uri = Android.Net.Uri.Parse(items[position]["video"]["url"].ToString());
             videoView.SetVideoURI(uri);*/

            Button btn = view.FindViewById<Button>(Resource.Id.button1);
            var liked = items[position]["liked"].ToString();
            Button LikeVideo = view.FindViewById<Button>(Resource.Id.BTN_LIKE);
            if (Convert.ToBoolean(liked) == true)
            {
                LikeVideo.SetTextColor(Android.Graphics.Color.Red);
                
            }
            else
            {
                LikeVideo.SetTextColor(Android.Graphics.Color.DarkGray);
            }
            Button FlagVideo = view.FindViewById<Button>(Resource.Id.BTN_FLAG);
            btn.Click += (o, e) =>
            {
                btn.Visibility = ViewStates.Gone;
                holder.Video.Start();
                MediaController mc = new MediaController(context);
                mc.SetAnchorView(holder.Video);
                mc.SetMinimumWidth(holder.Video.Width);
                mc.RequestFocus();

            };
            LikeVideo.Click += (o, e) =>
            {
                
                if (Convert.ToBoolean(liked))
                {
                    APITask task = new APITask("feed/" + items[position]["present_id"] + "/unlike");
                    APIArgs args = new APIArgs();
                    IRestResponse response = task.Call(args);

                    if (response.ErrorException == null)
                    {
                        context.RunOnUiThread(() =>
                        {
                            LikeVideo.SetTextColor(Android.Graphics.Color.DarkGray);
                            likes -= 1;
                            holder.Likes.Text = likes.ToString() + " Likes";
                            items[position]["liked"] = false;
                            liked = items[position]["liked"].ToString();
                            Console.WriteLine("Status:" +items[position]["liked"].ToString() + "Likes:"+items[position]["likes"].ToString());
                        });
                    }
                    else
                    {
                        Console.WriteLine(response.ErrorException.Message);
                    }
                }
                else
                {
                    APITask task = new APITask("feed/" + items[position]["present_id"] + "/like");
                    APIArgs args = new APIArgs();
                    IRestResponse response = task.Call(args);

                    if (response.ErrorException == null)
                    {
                        context.RunOnUiThread(() =>
                        {
                            LikeVideo.SetTextColor(Android.Graphics.Color.Red);
                            likes += 1;
                            holder.Likes.Text = likes.ToString() + " Likes";
                            items[position]["liked"] = true;
                            liked = items[position]["liked"].ToString();
                            Console.WriteLine("Status:" +items[position]["liked"].ToString() + "Likes:"+items[position]["likes"].ToString());
                        });
                    }
                    else
                    {
                        Console.WriteLine(response.ErrorException.Message);

                    }
                }
            }; 

            FlagVideo.Click += (o, e) =>
            {
                Android.App.AlertDialog.Builder builder = new AlertDialog.Builder(context);
                AlertDialog alertDialog = builder.Create();
                alertDialog.SetTitle("Are you sure?");
                alertDialog.SetIcon(Resource.Drawable.Icon);
                alertDialog.SetMessage("Flag as inappropriate?");

                //YES
                alertDialog.SetButton("YES", (s, ev) =>
                {
                    APITask task = new APITask("feed/" + items[position]["present_id"] + "/flag");
                    APIArgs args = new APIArgs();
                    IRestResponse response = task.Call(args);

                    if (response.ErrorException == null)
                   { 
                        context.RunOnUiThread(() =>
                        {
                            Console.WriteLine("Flag:");
                        });
                    }
                    else
                    {
                        Console.WriteLine(response.ErrorException.Message);
                    }
                });
                
                //NO
                alertDialog.SetButton2("NO", (s, ev) =>
                {
                    alertDialog.Dispose();
                });

                alertDialog.Show();
            };
            
            return view;
        }

    }
}
