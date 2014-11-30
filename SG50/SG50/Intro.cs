using System;
using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using Android.App;
using Android.Content;

namespace SG50
{
    [Activity(Label = "SG50", Theme = "@android:style/Theme.Holo.Light")]
    public class Intro : Activity, GestureDetector.IOnGestureListener
    {
        private GestureDetector _gestureDetector;
        private ViewFlipper vf;
        private int page = 0;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            ActionBar.Hide(); 
            SetContentView(Resource.Layout.Intro);
            vf = FindViewById<ViewFlipper>(Resource.Id.ViewSwitcher);
            _gestureDetector = new GestureDetector(this);
            Button BTN_Finish = FindViewById<Button>(Resource.Id.BTN_FINISH);
            BTN_Finish.Click += (o, e) =>
                {
                    var prefs = this.GetSharedPreferences("settings", 0);
                    var editor = prefs.Edit();
                    editor.PutBoolean("showTutorial", false);
                    editor.Commit();
                    Intent intent = new Intent(this, typeof(login));
                    this.StartActivity(intent);
                    this.Finish();
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

            if ((e1.GetX() - e2.GetX()) > sensitivity)
            {
                if (page == 3)
                {
                    return true;
                }
                vf.SetInAnimation(this, Resource.Animation.in_from_right);
                vf.SetOutAnimation(this, Resource.Animation.out_to_left);
                vf.ShowNext();
                page++;

            }
            else if ((e2.GetX() - e1.GetX()) > sensitivity)
            {
                if (page == 0)
                {
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
    }
}