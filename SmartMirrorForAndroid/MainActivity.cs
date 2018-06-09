using Android.App;
using Android.Widget;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using System;
using System.Threading.Tasks;

namespace SmartMirrorForAndroid
{
    [Activity(Label = "@string/app_name", ScreenOrientation = Android.Content.PM.ScreenOrientation.Landscape, Theme = "@style/Theme.AppCompat.Light.NoActionBar", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {

        TextView clock;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            //Fullscreen
            this.Window.AddFlags(WindowManagerFlags.Fullscreen);
            SetContentView(Resource.Layout.activity_main);

            clock = FindViewById<TextView>(Resource.Id.textView2);
        }

        protected async override void OnResume()
        {
            base.OnResume();
        }


        public static void GetTemperature()
        {

        }
    }
}

