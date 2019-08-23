using Android.App;
using Android.Widget;
using Android.OS;
using Android.Support.V7.App;
using Camera = Android.Hardware.Camera;

namespace camerapictureTaking
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = false)]
    public class MainActivity : AppCompatActivity
    {
        private static Button button;
        private Camera camera;
        static ImageView image;
        static TextView text;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }
    }
}

