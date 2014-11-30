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
using Android.Graphics.Drawables;
using System.Text.RegularExpressions;
using RestSharp;

namespace SG50
{
    [Activity(Label = "SG50", Theme = "@android:style/Theme.Holo.Light")]
    public class Signup : Activity, GestureDetector.IOnGestureListener
    {
        private GestureDetector _gestureDetector;
        DateTime _date = DateTime.Today;
        Button btnDob;
        private ViewFlipper vf;
        private int page = 0;


        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            ActionBar.Hide(); 
            SetContentView(Resource.Layout.register);
            vf = FindViewById<ViewFlipper>(Resource.Id.ViewSwitcher);
            TextView btncontinue1 = this.FindViewById<TextView>(Resource.Id.BTN_Continue1);
            TextView btncontinue2 = this.FindViewById<TextView>(Resource.Id.BTN_Continue2);
            TextView btncontinue3 = this.FindViewById<TextView>(Resource.Id.BTN_Continue3);
            TextView btncontinue4 = this.FindViewById<TextView>(Resource.Id.BTN_Continue4);
            TextView btncontinue5 = this.FindViewById<TextView>(Resource.Id.BTN_Continue5);
            TextView btncontinue6 = this.FindViewById<TextView>(Resource.Id.BTN_Continue6);
            EditText et_NRIC = this.FindViewById<EditText>(Resource.Id.et_NRIC);
            EditText et_email = this.FindViewById<EditText>(Resource.Id.et_email);
            EditText et_phone = this.FindViewById<EditText>(Resource.Id.et_phone);
            EditText et_name = this.FindViewById<EditText>(Resource.Id.et_name);
            EditText et_password = this.FindViewById<EditText>(Resource.Id.et_password);
            EditText et_repassword = this.FindViewById<EditText>(Resource.Id.et_repassword);
            _gestureDetector = new GestureDetector(this);

            Drawable icon_error = Resources.GetDrawable(Resource.Drawable.popup_inline_error);//this should be your error image.

            btncontinue1.Click += (o, e) =>
            {

                if (et_NRIC.Text.Length <= 0)
                {
                    et_NRIC.SetError("Invalid NRIC", icon_error);
                }
                else if (!IsNRICValid(et_NRIC.Text))
                {
                    et_NRIC.SetError("Invalid NRIC", icon_error);
                }
                else
                {

                    vf.SetInAnimation(this, Resource.Animation.in_from_right);
                    vf.SetOutAnimation(this, Resource.Animation.out_to_left);
                    vf.ShowNext();
                    page++;

                }
            };
            btncontinue2.Click += (o, e) =>
            {
                if (et_phone.Text.Length < 8)
                {
                    et_phone.SetError("Please enter a Mobile number", icon_error);
                }
                else
                {

                    vf.SetInAnimation(this, Resource.Animation.in_from_right);
                    vf.SetOutAnimation(this, Resource.Animation.out_to_left);
                    vf.ShowNext();
                    page++;
                }
            };
            btncontinue3.Click += (o, e) =>
            {
                if (et_email.Text == null)
                {
                    et_email.SetError("Invalid Email Address", icon_error);
                }
                else if (isEmailValid(et_email.Text) == false)
                {
                    et_email.SetError("Invalid Email Address", icon_error);
                }
                else
                {
                    vf.SetInAnimation(this, Resource.Animation.in_from_right);
                    vf.SetOutAnimation(this, Resource.Animation.out_to_left);
                    vf.ShowNext();
                    page++;
                }
            };
            btncontinue4.Click += (o, e) =>
            {
                if (et_name.Text.Length <= 0)
                {
                    et_name.SetError("Please enter name", icon_error);
                }
                else
                {
                    vf.SetInAnimation(this, Resource.Animation.in_from_right);
                    vf.SetOutAnimation(this, Resource.Animation.out_to_left);
                    vf.ShowNext();
                    page++;
                }
            };
            btncontinue5.Click += (o, e) =>
            {
                if (et_password.Text == null || et_repassword.Text == null)
                {
                    et_password.SetError("Please enter password", icon_error);
                    et_repassword.SetError("Please enter password", icon_error);
                }
                else if (et_password.Text == null && et_repassword.Text == null)
                {
                    et_password.SetError("Please enter password", icon_error);
                    et_repassword.SetError("Please enter password", icon_error);
                }
                else if (et_password.Text != et_repassword.Text)
                {
                    et_password.SetError("Password doesn't match", icon_error);
                    et_repassword.SetError("Password doesn't match", icon_error);
                }
                else
                {
                    vf.SetInAnimation(this, Resource.Animation.in_from_right);
                    vf.SetOutAnimation(this, Resource.Animation.out_to_left);
                    vf.ShowNext();
                    page++;
                }
            };
            btncontinue6.Click += (o, e) =>
            {
             /*   APITask task = new APITask("user/register");

                APIArgs args = new APIArgs();
                args.Parameters.Add("name", et_name.Text);
                args.Parameters.Add("password", et_password.Text);
                args.Parameters.Add("email", et_email.Text);
                args.Parameters.Add("nric", et_NRIC.Text);
                args.Parameters.Add("handphone", et_phone.Text);
                 
                IRestResponse response = task.Call(args);

                if (response.ErrorException == null)
                {*/
                    Android.App.AlertDialog.Builder builder = new AlertDialog.Builder(this);
                    AlertDialog alertDialog = builder.Create();
                    alertDialog.SetTitle("");
                    alertDialog.SetIcon(Resource.Drawable.Icon);
                    alertDialog.SetMessage("Thank you for registering! Please proceed to login");

                    //Video
                    alertDialog.SetButton("Ok", (s, ev) =>
                    {
                        try
                        {
                            alertDialog.Dismiss();
                            Intent intent = new Intent(this, typeof(login));
                            this.StartActivity(intent);
                            this.Finish();
                        }
                        catch (Exception ex) { }
                    });

                    alertDialog.Show();

               /* }
                else
                {
                    Console.WriteLine(response.ErrorException.Message);
                }*/
            };

        }

        public override bool OnTouchEvent(MotionEvent e)
        {
            _gestureDetector.OnTouchEvent(e);
            return false;
        }

        public bool OnDown(MotionEvent e)
        {
            return false;
        }

        public bool OnFling(MotionEvent e1, MotionEvent e2, float velocityX, float velocityY)
        {
            float sensitivity = 50;

            if ((e2.GetX() - e1.GetX()) > sensitivity)
            {
                if (page == 0)
                {
                    Intent intent = new Intent(this, typeof(login));
                    this.StartActivity(intent);
                    this.Finish();
                    return true;
                }
                vf.SetInAnimation(this, Resource.Animation.in_from_left);
                vf.SetOutAnimation(this, Resource.Animation.out_to_right);
                vf.ShowPrevious();
                page--;
            }

            return true;
        }

        public void OnLongPress(MotionEvent e) { }

        public bool OnScroll(MotionEvent e1, MotionEvent e2, float distanceX, float distanceY)
        {
            return false;
        }

        public void OnShowPress(MotionEvent e) { }

        public bool OnSingleTapUp(MotionEvent e)
        {
            return false;
        }

        bool isEmailValid(String email)
        {
            return Android.Util.Patterns.EmailAddress.Matcher(email).Matches();
        } // end of email matcher
        private static readonly int[] Multiples = { 2, 7, 6, 5, 4, 3, 2 };

        public static bool IsNRICValid(string nric)
        {
            if (string.IsNullOrEmpty(nric))
            {
                return false;
            }

            //	check length must be 9 digits
            if (nric.Length != 9)
            {
                return false;
            }

            int total = 0
                , count = 0
                , numericNric;
            char first = nric[0]
                , last = nric[nric.Length - 1];

            // first chat alwatas T or S
            if (first != 'S' && first != 'T')
            {
                return false;
            }

            // ensure first chars is char and last

            if (!int.TryParse(nric.Substring(1, nric.Length - 2), out numericNric))
            {
                return false;
            }

            while (numericNric != 0)
            {
                total += numericNric % 10 * Multiples[Multiples.Length - (1 + count++)];

                numericNric /= 10;
            }

            char[] outputs;
            // first S, pickup different array( read specification)
            if (first == 'S')
            {
                outputs = new char[] { 'J', 'Z', 'I', 'H', 'G', 'F', 'E', 'D', 'C', 'B', 'A' };
            }
            // T pickup different arrary ( read specification)
            else
            {
                outputs = new char[] { 'G', 'F', 'E', 'D', 'C', 'B', 'A', 'J', 'Z', 'I', 'H' };
            }

            return last == outputs[total % 11];

        }
    }
}