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
            holder.User.Text = "Uploaded by: "+items[position]["user"].ToString();
            holder.Likes.Text = items[position]["likes"].ToString() + " Likes";
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
            Button LikeVideo = view.FindViewById<Button>(Resource.Id.BTN_LIKE);
            btn.Click += (o, e) =>
            {
                btn.Visibility = ViewStates.Gone;
                holder.Video.Start();
                MediaController mc = new MediaController(context);
                mc.SetAnchorView(holder.Video);
                mc.SetMinimumWidth(holder.Video.Width);
                mc.RequestFocus();

            };
            LikeVideo.Click += async (o, e) =>
            {
                var liked = items[position]["liked"].ToString();
                Console.WriteLine("Before:" +items[position]["liked"].ToString());
                if (Convert.ToBoolean(liked))
                {
                    APITask task = new APITask("/feed/:" + items[position]["present_id"] + "/unlike");
                    APIArgs args = new APIArgs();
                    args.Parameters.Add("accesstoken", "acdcb58208f767fc204f36ecd74afc30");
                    IRestResponse response = await task.CallAsync(args);

                    if (response.ErrorException == null)
                    {
                        context.RunOnUiThread(() =>
                        {
                            int likes = Convert.ToInt32(items[position]["likes"]);
                            items[position]["likes"] = likes - 1;
                            Console.WriteLine("After:" + items[position]["liked"].ToString());
                        });
                    }
                    else
                    {
                        Console.WriteLine(response.ErrorException.Message);
                        Console.WriteLine("After:" + items[position]["liked"].ToString());
                    }
                }
                else
                {
                    APITask task = new APITask("/feed/:" + items[position]["present_id"] + "/like");
                    APIArgs args = new APIArgs();
                    args.Parameters.Add("accesstoken", "acdcb58208f767fc204f36ecd74afc30");
                    IRestResponse response = await task.CallAsync(args);

                    if (response.ErrorException == null)
                    {
                        context.RunOnUiThread(() =>
                        {
                            Console.WriteLine("After:" + items[position]["liked"].ToString());
                        });
                    }
                    else
                    {
                        Console.WriteLine(response.ErrorException.Message);
                        Console.WriteLine("After:" + items[position]["liked"].ToString());
                    }
                }
            };

               return view;           
        }

    }
}
