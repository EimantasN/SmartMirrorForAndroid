using System;
using System.Threading;
using Android.Bluetooth;

namespace camerapictureTaking.BluetoothControl
{
    public class BlueToothConnectionPortable
    {
        BluetoothConnection Connection = new BluetoothConnection();

        BluetoothSocket _socket { get; set; }

        public bool Connected { get; set; }

        Thread ArduinoOutputReader { get; set; }

        public bool State { get; set; }

        public BlueToothConnectionPortable()
        {
            _socket = null;
            Connect();
        }

        public void Reset()
        {
            _socket = null;
            Connect();
        }

        public void Connect()
        {
            if (Connected || _socket != null)
                return;
            try
            {
                Connection = new BluetoothConnection();

                Connection.getAdapter();
                Connection.thisAdapter.StartDiscovery();

                Connection.getDevice();
                //myConnection.thisDevice.SetPairingConfirmation(false);
                //myConnection.thisDevice.SetPairingConfirmation(true);
                Connection.thisDevice.CreateBond();

                Connection.thisAdapter.CancelDiscovery();
                _socket = Connection.thisDevice.CreateRfcommSocketToServiceRecord(Java.Util.UUID.FromString("00001101-0000-1000-8000-00805f9b34fb"));
                Connection.thisSocket = _socket;

                Connection.thisSocket.Connect();
                Connected = true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void Off()
        {
            ControlLed(false);
        }

        public void On()
        {
            ControlLed(true);
        }

        private void ControlLed(bool state)
        {
            State = state;

            if (Connection == null)
                return;

            if (state)
            {
                Connection.thisSocket.OutputStream.WriteByte(72);
            }
            else
            {
                Connection.thisSocket.OutputStream.WriteByte(71);
            }
        }

        private void ReaderThread()
        {
            ArduinoOutputReader = new Thread(() =>
            {
                //byte[] read = new byte[1];
                //string ReadedText = string.Empty;
                //var message = Toast.MakeText(this.ApplicationContext, read[0], ToastLength.Short);

                //while (true)
                //{
                //    try
                //    {
                //        Connection.thisSocket.InputStream.Read(read, 0, 1);
                //        Connection.thisSocket.InputStream.Close();
                //        RunOnUiThread(() =>
                //        {
                //            if (read[0] == 1)
                //            {
                //                message.SetText(read[0]);
                //                Application.SynchronizationContext.Post(_ => { message.Show(); }, null);
                //                ReadedText += read[0];
                //            }
                //        });
                //    }
                //    catch (Exception e)
                //    {
                //    }
                //}
            });
            ArduinoOutputReader.Start();
        }
    }
}