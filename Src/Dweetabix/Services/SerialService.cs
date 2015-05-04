using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Dweetabix.Helpers;

namespace Dweetabix.Services
{
    public class SerialService : IDisposable
    {
        public event EventHandler<EventArgs> Connected;
        public event EventHandler<EventArgs> Disconnected;

        private SerialPort _port;
        private bool _isConnected;

        public SerialService(string port, int baudRate, int dataBits, StopBits stopBits, Parity parity, int timeout)
        {
            _port = new SerialPort(port, baudRate, parity, dataBits, stopBits)
            {
                ReadTimeout = timeout,
                WriteTimeout = timeout
            };
        }

        public void Connect(int retryDelay = 5)
        {
            while (!_port.IsOpen)
            {
                try
                {
                    LogHelper.Info("Connecting to com port '" + _port.PortName + "'...");

                    _port.Open();
                }
                catch (IOException ex)
                {
                    LogHelper.Error("Error opening port: " + ex.Message);
                    LogHelper.Info("Retrying in 5 seconds...");

                    Thread.Sleep(retryDelay*1000);
                }
            }

            NotifyConnected();
        }

        public void Disconnect(bool notify = true)
        {
            _port.Close();
            _isConnected = false;

            if (notify)
                NotifyDisconnected();
        }

        public void Update()
        {
            if (_port.IsOpen && !_isConnected)
                NotifyConnected();

            if (!_port.IsOpen && _isConnected)
                NotifyDisconnected();
        }

        public void Write(string msg)
        {
            if (_port.IsOpen)
                _port.Write(msg + '\n');
            else
                LogHelper.Error("Unable to write message, serial port is not open.");
        }

        private void NotifyConnected()
        {
            LogHelper.Success("Connected to com port '" + _port.PortName + "'...");

            _isConnected = true;
            if (Connected != null)
                Connected(this, new EventArgs());
        }

        private void NotifyDisconnected()
        {
            LogHelper.Error("Disconnected from com port '" + _port.PortName + "'...");

            _isConnected = false;
            if (Disconnected != null)
                Disconnected(this, new EventArgs());
        }

        public void Dispose()
        {
            if (_port.IsOpen)
                _port.Close();

            _port.Dispose();
        }
    }
}
