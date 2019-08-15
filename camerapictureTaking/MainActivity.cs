using Android.App;
using Android.Widget;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using System;
using Android.Runtime;
using Camera = Android.Hardware.Camera;
using Android.Graphics;
using static Android.Hardware.Camera;
using Java.IO;
using Android.Hardware;
using System.IO;
using System.Threading.Tasks;

namespace camerapictureTaking
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        private static Button button;
        private Camera camera;
        static ImageView image;
        static TextView text;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);


            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);
            if (null == camera)
            {
                camera = getCustomCamera();
            }
            image = FindViewById<ImageView>(Resource.Id.image);
            button = FindViewById<Button>(Resource.Id.bt);

            text = FindViewById<TextView>(Resource.Id.text);

            button.Click += picture;

            button.TextChanged += Button_TextChanged;
        }

        private void Button_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            text.Text = "Megina Atlikti fotografavima";
            if (!button.Text.Contains("Eimantas"))
            {
                try
                {
                    Task.Run(() =>
                    {
                        camera.StartPreview();
                        camera.TakePicture(null, null, new MyPictureCallback());
                    });

                }
                catch
                {
                    button.Text = "Kamera nepasiekiama";
                }
            }
        }

        protected override void OnResume()
        {
            base.OnResume();
            if (null == camera)
            {
                camera = getCustomCamera();
            }
        }

        public async Task takingPhotos()
        {
            await Task.Delay(1000);
            Application.SynchronizationContext.Post(_ => { text.Text = "fokinama"; }, null);
            
            //button.Text = "Tikrinama";
            try
            {
                camera.StartPreview();
                camera.TakePicture(null, null, new MyPictureCallback());
                takingPhotos();
                //camera.StartPreview();
                //camera.TakePicture(null, null, new MyPictureCallback());
            }
            catch
            {
                takingPhotos();
                Application.SynchronizationContext.Post(_ => { button.Text = "Kamera nepasiekiama"; }, null);
                
            }
        }

        protected override void OnStop()
        {
            base.OnStop();
            if (null != camera)
            {
                camera.SetPreviewCallback(null);
                camera.StopPreview();
                camera.Release();
                camera = null;
            }

        }

        private Camera getCustomCamera()
        {
            if (null == camera)
            {
                var cameraInfo = new Camera.CameraInfo();
                for (int i = 0; i < Camera.NumberOfCameras; i++)
                {
                    Camera.GetCameraInfo(i, cameraInfo);

                    if ((cameraInfo.Facing == CameraFacing.Front))
                    {
                        camera = Camera.Open(i);
                        //camera.SetDisplayOrientation(90);
                        //camera.FaceDetection += Camera_FaceDetection;
                        //camera.StartFaceDetection();
                        var parameters = camera.GetParameters();
                        parameters.PictureFormat = ImageFormatType.Jpeg;
                        parameters.SetPreviewSize(50, 50);
                        parameters.FocusMode = Camera.Parameters.FocusModeAuto;
                    }
                }
            }
            return camera;

        }

        private void Camera_FaceDetection(object sender, FaceDetectionEventArgs e)
        {
            button.Text = "face detected";
        }

        private void picture(object sender, EventArgs e)
        {
            //Task.Run(() => { takingPhotos(); });

            text.Text = "fokinama";
            //button.Text = "Tikrinama";
            try
            {
                Task.Run(() =>
                {
                    camera.StartPreview();
                    camera.TakePicture(null, null, new MyPictureCallback());
                });
                //camera.StartPreview();
                //camera.TakePicture(null, null, new MyPictureCallback());
            }
            catch
            {
                button.Text = "Kamera nepasiekiama";
            }
        }
        public void SurfaceChanged(ISurfaceHolder holder, [GeneratedEnum] Format format, int width, int height)
        {
            camera.StopPreview();
        }

        public class MyPictureCallback : Java.Lang.Object, IPictureCallback
        {
            public void OnPictureTaken(byte[] data, Camera camera)
            {
                dealWithCameraDataAsync(data);
            }

            private void dealWithCameraDataAsync(byte[] data)
            {
                //Application.SynchronizationContext.Post(_ => { button.Text = button.Text+ " " + data.Length; }, null);
                Task.Run(() =>
                {
                    Application.SynchronizationContext.Post(_ => { text.Text = "gauti duomenys dydis " + data.Length; }, null);

                    byte[] flippedImageByteArray = null;
                    try
                    {
                        Bitmap bitmap = BitmapFactory.DecodeByteArray(data, 0, data.Length);
                        bitmap = rotateImage(180, bitmap);
                        MemoryStream stream = new MemoryStream();
                        bitmap.Compress(Bitmap.CompressFormat.Jpeg, 100, stream);
                        flippedImageByteArray = stream.ToArray();
                        //Application.SynchronizationContext.Post(_ => { image.SetImageBitmap(bitmap); }, null);

                    }
                    catch { }

                    if (flippedImageByteArray == null)
                    {
                        Application.SynchronizationContext.Post(_ => { button.Text = "Nepavyko apversti nuotraukos"; }, null);

                        return;
                    }
                    Application.SynchronizationContext.Post(_ => { text.Text = "flip atliktas"; }, null);

                    //Here to save your photo
                    FileOutputStream fos = null;
                    string tempStr = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
                    string fileName = tempStr + "Eimantas" + ".jpg";
                    Java.IO.File tempFile = new Java.IO.File(fileName);
                    fos = new FileOutputStream(fileName);
                    fos.Write(flippedImageByteArray);
                    fos.Close();

                    var classs = new FaceRecognition.FaceRecognition();
                    var result = classs.RecognitionFace("1", fileName).Result;
                    Application.SynchronizationContext.Post(_ => { button.Text = result; }, null);
                });
            }

            public Bitmap rotateImage(int angle, Bitmap bitmapSrc)
            {
                Matrix matrix = new Matrix();
                matrix.PostRotate(angle);
                return Bitmap.CreateBitmap(bitmapSrc, 0, 0,
                    bitmapSrc.Width, bitmapSrc.Height, matrix, true);
            }
        }
    }
}

