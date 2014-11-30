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
using Android.Graphics;
using Android.Text;
using Android.Text.Style;
using RestSharp;

namespace SG50
{
    [Activity(Label = "SG50", Theme = "@android:style/Theme.Holo.Light")]
    public class login : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.login);
            ActionBar.Hide();
            Button btnLogin = this.FindViewById<Button>(Resource.Id.BTN_LOGIN);
            Button btnSignup = this.FindViewById<Button>(Resource.Id.BTN_SIGNUP);
            TextView btnForgotPass = this.FindViewById<TextView>(Resource.Id.TV_FORGOTPASSWORD);
            String text = "<u>Forgot Password?</u>";
            btnForgotPass.TextFormatted = Html.FromHtml(text);


            btnLogin.Click += (o, e) =>
            {
                EditText tbuser = this.FindViewById<EditText>(Resource.Id.ed_loginUser);
                EditText tbpass = this.FindViewById<EditText>(Resource.Id.ed_loginpass);
                if (tbuser.Text != null && tbpass.Text != null)
                {
                    APITask task = new APITask("user/auth");

                    APIArgs args = new APIArgs();
                    args.Parameters.Add("email", tbuser);
                    args.Parameters.Add("password", tbpass);

                    IRestResponse response = task.Call(args);
                    if (response.ErrorException == null)
                    {
                        var prefs = this.GetSharedPreferences("settings", FileCreationMode.Private);
                        var editor = prefs.Edit();
                        editor.PutString("X-Accesstoken", response.Content);
                        editor.Commit();
                        Console.WriteLine("STORED:" + response.Content);

                        Intent intent = new Intent(this, typeof(Feed));
                        this.StartActivity(intent);
                        this.Finish();
                    }
                    else
                    {
                        Android.App.AlertDialog.Builder builder = new AlertDialog.Builder(this);
                        AlertDialog alertDialog = builder.Create();
                        alertDialog.SetTitle("");
                        alertDialog.SetIcon(Resource.Drawable.Icon);
                        alertDialog.SetMessage("Invalid username/password");

                        //Video
                        alertDialog.SetButton("Ok", (s, ev) =>
                        {
                            try
                            {
                                alertDialog.Dismiss();
                            }
                            catch (Exception ex) { }
                        });
                        alertDialog.Show();
                    }
                }
            };
            btnSignup.Click += (o, e) =>
            {
                Intent intent = new Intent(this, typeof(Signup));
                this.StartActivity(intent);
                this.Finish();
            };
        }
    }
}