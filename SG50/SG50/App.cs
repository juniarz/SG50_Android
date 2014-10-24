using System;
using Android.App;
using Android.Runtime;
using Parse;

namespace SG50
{
	[Application]
	public class App : Application
	{

        public const String FB_APPID = "788126441251365";
        public const string FB_ExtendedPermissions = "user_about_me,read_stream,publish_stream";
        public static string FB_AccessToken;

		public App (IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
		{
		}

		public override void OnCreate ()
		{
			base.OnCreate ();

			// Initialize the Parse client with your Application ID and .NET Key found on
			// your Parse dashboard
            ParseClient.Initialize("sRP8PvTjYndZvdXoEqB0ZB9FYRSyS4YsW2elvPfZ", "gPr4zcn38aEtd6hSyaPUf5DEjRIlzXNVp0NTqUnf");
		}
	}
}
