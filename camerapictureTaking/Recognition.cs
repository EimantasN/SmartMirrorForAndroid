using Android.App;
using Android.OS;
using Android.Gms.Vision;
using Android.Support.V4.App;
using Android.Util;
using Android;
using Android.Support.Design.Widget;
using Android.Gms.Vision.Faces;
using Android.Runtime;
using static Android.Gms.Vision.MultiProcessor;
using Android.Content.PM;
using Android.Gms.Common;
using camerapictureTaking.CustomControls;
using camerapictureTaking.Helpers;
using camerapictureTaking.BluetoothControl;

namespace camerapictureTaking
{
    [Activity(Label = "Recognition", MainLauncher = true, Theme = "@style/Theme.AppCompat.Light.NoActionBar", ScreenOrientation = ScreenOrientation.FullSensor)]
    public class Recognition : SupportActivity, IFactory
    {
        private static readonly string TAG = "FaceRecognition&Tracker";

        BlueToothConnectionPortable BTArduino { get; set; }

        private CameraSource mCameraSource = null;
        private CameraSourcePreview mPreview;
        private GraphicOverlay mGraphicOverlay;

        public static string GreetingsText { get; set; }

        private static readonly int RC_HANDLE_GMS = 9001;
        private static readonly int RC_HANDLE_CAMERA_PERM = 2;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.face_recognition);

            BTArduino = new BlueToothConnectionPortable();

            mPreview = FindViewById<CameraSourcePreview>(Resource.Id.preview);
            mGraphicOverlay = FindViewById<GraphicOverlay>(Resource.Id.faceOverlay);

            if (ActivityCompat.CheckSelfPermission(this, Manifest.Permission.Camera) == Permission.Granted)
            {
                CreateCameraSource();
                LiveCamHelper.Init();
                LiveCamHelper.GreetingsCallback = (s) => { RunOnUiThread(() => GreetingsText = s); };
                //Task.Run(() => LiveCamHelper.RegisterFaces());
            }
            else { RequestCameraPermission(); }

        }

        protected override void OnResume()
        {
            base.OnResume();
            StartCameraSource();
        }

        protected override void OnPause()
        {
            base.OnPause();
            mPreview.Stop();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (mCameraSource != null)
            {
                mCameraSource.Release();
            }
        }

        private void RequestCameraPermission()
        {
            Log.Warn(TAG, "Camera permission is not granted. Requesting permission");

            var permissions = new string[] { Manifest.Permission.Camera };

            if (!ActivityCompat.ShouldShowRequestPermissionRationale(this,
                    Manifest.Permission.Camera))
            {
                ActivityCompat.RequestPermissions(this, permissions, RC_HANDLE_CAMERA_PERM);
                return;
            }

            Snackbar.Make(mGraphicOverlay, "Camera will be used for face detection",
                    Snackbar.LengthIndefinite)
                    .SetAction("Ok", (o) => { ActivityCompat.RequestPermissions(this, permissions, RC_HANDLE_CAMERA_PERM); })
                    .Show();
        }

        private void CreateCameraSource()
        {
            var context = Application.Context;
            FaceDetector detector = new FaceDetector.Builder(context)
                    .SetClassificationType(ClassificationType.All)
                    .Build();

            detector.SetProcessor(new MultiProcessor.Builder(this).Build());

            if (!detector.IsOperational)
            {
                Log.Warn(TAG, "Face detector dependencies are not yet available.");
            }

            mCameraSource = new CameraSource.Builder(context, detector)
                    .SetFacing(CameraFacing.Back)
                    .SetRequestedFps(60.0f)
                    .SetAutoFocusEnabled(true)
                    .Build();
        }

        public Tracker Create(Java.Lang.Object item)
        {
            return new GraphicFaceTracker(mGraphicOverlay, mCameraSource, BTArduino);
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            if (requestCode != RC_HANDLE_CAMERA_PERM)
            {
                Log.Debug(TAG, "Got unexpected permission result: " + requestCode);
                base.OnRequestPermissionsResult(requestCode, permissions, grantResults);

                return;
            }

            if (grantResults.Length != 0 && grantResults[0] == Permission.Granted)
            {
                Log.Debug(TAG, "Camera permission granted - initialize the camera source");
                // we have permission, so create the camerasource
                CreateCameraSource();
                return;
            }

            Log.Error(TAG, "Permission not granted: results len = " + grantResults.Length +
                    " Result code = " + (grantResults.Length > 0 ? grantResults[0].ToString() : "(empty)"));


            var builder = new Android.Support.V7.App.AlertDialog.Builder(this);
            builder.SetTitle("FaceRecognition")
                    .SetMessage("We dont have camera premissions")
                    .SetPositiveButton("Ok", (o, e) => Finish())
                    .Show();

        }

        private void StartCameraSource()
        {

            // check that the device has play services available.
            int code = GoogleApiAvailability.Instance.IsGooglePlayServicesAvailable(this.ApplicationContext);
            if (code != ConnectionResult.Success)
            {
                Dialog dlg = GoogleApiAvailability.Instance.GetErrorDialog(this, code, RC_HANDLE_GMS);
                dlg.Show();
            }

            if (mCameraSource != null)
            {
                try
                {
                    mPreview.Start(mCameraSource, mGraphicOverlay);
                }
                catch (System.Exception e)
                {
                    Log.Error(TAG, "Unable to start camera source.", e);
                    mCameraSource.Release();
                    mCameraSource = null;
                }
            }
        }
    }
}