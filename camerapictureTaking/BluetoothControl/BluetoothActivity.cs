using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Bluetooth;
using System.Linq;
using System.Threading;

namespace camerapictureTaking.BluetoothControl
{
    [Activity(Label = "camerapictureTaking", MainLauncher = false)]
    public class MainActivity : Activity
    {
        BluetoothConnection myConnection = new BluetoothConnection();
        BluetoothSocket _socket { get; set; }

        Thread ArduinoOutputReader { get; set; }

        Button Connect;
        Button On;
        Button Off;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.bluetooth);

            Connect = FindViewById<Button>(Resource.Id.connect);
            Connect.Click += Connect_Click;

            On = FindViewById<Button>(Resource.Id.on);
            On.Click += On_Click;

            Off = FindViewById<Button>(Resource.Id.off);
            Off.Click += Off_Click;

            BluetoothSocket _socket = null;
        }

        private void Off_Click(object sender, EventArgs e)
        {
            ControlLed(false);
        }

        private void On_Click(object sender, EventArgs e)
        {
            ControlLed(true);
        }

        private void Connect_Click(object sender, EventArgs e)
        {
            try
            {
                myConnection = new BluetoothConnection();

                myConnection.getAdapter();
                myConnection.thisAdapter.StartDiscovery();

                myConnection.getDevice();
                //myConnection.thisDevice.SetPairingConfirmation(false);
                //myConnection.thisDevice.SetPairingConfirmation(true);
                myConnection.thisDevice.CreateBond();

                myConnection.thisAdapter.CancelDiscovery();
                _socket = myConnection.thisDevice.CreateRfcommSocketToServiceRecord(Java.Util.UUID.FromString("00001101-0000-1000-8000-00805f9b34fb"));
                myConnection.thisSocket = _socket;

                myConnection.thisSocket.Connect();

                Message("Connected!");
                Connect.Enabled = false;
            }
            catch (Exception ex)
            {
                Message("Connection Error");
            }
        }

        private void Message(string message, bool mainThread = true)
        {
            if (mainThread)
            {
                Toast.MakeText(this.ApplicationContext, message, ToastLength.Short).Show();
            }
            else
            {
                RunOnUiThread(() =>
                {
                    Toast.MakeText(this.ApplicationContext, message, ToastLength.Short).Show();
                });
            }
        }

        private void ReaderThread()
        {
            ArduinoOutputReader = new Thread(() =>
            {
                byte[] read = new byte[1];
                string ReadedText = string.Empty;
                var message = Toast.MakeText(this.ApplicationContext, read[0], ToastLength.Short);

                while (true)
                {
                    try
                    {
                        myConnection.thisSocket.InputStream.Read(read, 0, 1);
                        myConnection.thisSocket.InputStream.Close();
                        RunOnUiThread(() =>
                        {

                            if (read[0] == 1)
                            {
                                message.SetText(read[0]);
                                Application.SynchronizationContext.Post(_ => { message.Show(); }, null);
                                ReadedText += read[0];
                            }
                        });
                    }
                    catch (Exception e)
                    {
                    }
                }
            });
            ArduinoOutputReader.Start();
        }

        private void ControlLed(bool state)
        {
            if (state)
            {
                myConnection.thisSocket.OutputStream.WriteByte(72);
            }
            else
            {
                myConnection.thisSocket.OutputStream.WriteByte(71);
            }
        }
    }
}

