using Android.App;
using Android.Widget;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using System;
using System.Threading.Tasks;
using Library;
using Square.Picasso;

namespace SmartMirrorForAndroid
{
    [Activity(Label = "@string/app_name", ScreenOrientation = Android.Content.PM.ScreenOrientation.Landscape, Theme = "@style/Theme.AppCompat.Light.NoActionBar", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        DataParsing parsing;
        TemperatureNow temperatureData;
        TextClock clock;
        TextView console;
        TextView Temperature;

        ImageView TempImage;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            //Fullscreen
            this.Window.AddFlags(WindowManagerFlags.Fullscreen);
            SetContentView(Resource.Layout.activity_main);

            console = FindViewById<TextView>(Resource.Id.console);
            string a = DateTime.Now.ToLocalTime().ToString();
            clock = FindViewById<TextClock>(Resource.Id.textView2);
            Temperature = FindViewById<TextView>(Resource.Id.textView3);
            TempImage = FindViewById<ImageView>(Resource.Id.imageView);
            

            parsing = new DataParsing();
        }

        public void WriteToConsole(string newtext, TextView console)
        {
            Application.SynchronizationContext.Post(_ => { console.Text = newtext + "  time - " + DateTime.Now.ToLocalTime().ToString() + "\n" + console.Text; }, null);
            
        }

        protected async override void OnResume()
        {
            base.OnResume();
            
            await Task.Run(() => temperatureData = parsing.ParseTemperatureData("Kaunas", console));
            Temperature.Text = temperatureData.Temperature;

            string a = "";
        }


        public static void GetTemperature()
        {

        }
    }
}

