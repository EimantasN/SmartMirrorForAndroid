using System;
using System.Threading.Tasks;
using Android.Gms.Vision;
using Android.Gms.Vision.Faces;
using camerapictureTaking.BluetoothControl;
using camerapictureTaking.CustomControls;
using camerapictureTaking.Helpers;
using FaceRecognitionService.ServiceHelpers;

namespace camerapictureTaking
{
    public class GraphicFaceTracker : Tracker, CameraSource.IPictureCallback
    {
        private GraphicOverlay mOverlay;
        private FaceGraphic mFaceGraphic;
        private CameraSource mCameraSource = null;
        private bool isProcessing = false;
        private BlueToothConnectionPortable ArduinoConnection;

        public GraphicFaceTracker(GraphicOverlay overlay, CameraSource cameraSource = null, BlueToothConnectionPortable bTArduino = null)
        {
            mOverlay = overlay;
            mFaceGraphic = new FaceGraphic(overlay);
            mCameraSource = cameraSource;
            ArduinoConnection = bTArduino;
        }

        public override void OnNewItem(int id, Java.Lang.Object item)
        {
            mFaceGraphic.SetId(id);
            // if (mCameraSource != null && !isProcessing)
            //mCameraSource.TakePicture(null, this);
        }

        public override void OnUpdate(Detector.Detections detections, Java.Lang.Object item)
        {
            var face = item as Face;
            mOverlay.Add(mFaceGraphic);
            mFaceGraphic.UpdateFace(face);
            if (detections.DetectedItems.Size() != 0)
            {
                if (ArduinoConnection != null && ArduinoConnection.State != true)
                    ArduinoConnection.On();
            }
        }

        public override void OnMissing(Detector.Detections detections)
        {
            mOverlay.Remove(mFaceGraphic);
            if (detections.DetectedItems.Size() == 0)
            {
                if (ArduinoConnection != null)
                {
                    ArduinoConnection.Off();
                }
            }
        }

        public override void OnDone()
        {
            mOverlay.Remove(mFaceGraphic);
        }

        public void OnPictureTaken(byte[] data)
        {
            Task.Run(async () =>
            {
                try
                {
                    isProcessing = true;
                    Console.WriteLine("face detected: ");
                    //var imageAnalyzer = new ImageAnalyzer(data);
                    //await LiveCamHelper.ProcessCameraCapture(imageAnalyzer);
                }
                finally
                {
                    isProcessing = false;
                }
            });
        }

    }
}