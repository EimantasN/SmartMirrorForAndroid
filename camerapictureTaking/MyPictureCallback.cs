using System.IO;
using System.Threading.Tasks;
using Android.Graphics;
using Java.IO;
using static Android.Hardware.Camera;

namespace FaceDetectDroid
{
    public class MyPictureCallback : Java.Lang.Object, IPictureCallback
    {
        public void OnPictureTaken(byte[] data, Android.Hardware.Camera camera)
        {
            dealWithCameraDataAsync(data);
        }

        private void dealWithCameraDataAsync(byte[] data)
        {
            Task.Run(() =>
            {
                //Application.SynchronizationContext.Post(_ => { text.Text = "gauti duomenys dydis " + data.Length; }, null);

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
                    //Application.SynchronizationContext.Post(_ => { button.Text = "Nepavyko apversti nuotraukos"; }, null);

                    return;
                }
                //Application.SynchronizationContext.Post(_ => { text.Text = "flip atliktas"; }, null);


                FileOutputStream fos = null;
                string tempStr = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
                string fileName = tempStr + "Eimantas" + ".jpg";
                Java.IO.File tempFile = new Java.IO.File(fileName);
                fos = new FileOutputStream(fileName);
                fos.Write(flippedImageByteArray);
                fos.Close();

                //var classs = new FaceRecognition.FaceRecognition();
                //var result = classs.RecognitionFace("1", fileName).Result;
                //Application.SynchronizationContext.Post(_ => { button.Text = result; }, null);
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