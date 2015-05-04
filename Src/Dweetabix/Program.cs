using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Dweetabix.Models;
using Dweetabix.Extensions;
using System.Threading;
using Dweetabix.Helpers;
using Dweetabix.Services;
using System.Runtime.InteropServices;

namespace Dweetabix
{
    class Program
    {
        [DllImport("Kernel32")]
        private static extern bool SetConsoleCtrlHandler(EventHandler handler, bool add);

        private delegate bool EventHandler(int eventType);
        static EventHandler _handler;

        private static SerialService _serial;
        private static DweetService _dweet;

        static void Main(string[] args)
        {
            var version = typeof(Program).Assembly.GetName().Version;

            // Set console title
            Console.Title = "Dweetabix v" + version;

            // Display header
            Console.WriteLine(@"                                                                      ");
            Console.WriteLine(@"      #####  ##   ## ###### ###### ######  ####  #####  ###### ##  ## ");
            Console.WriteLine(@"     ##  ## ##   ## ##     ##       ##   ##  ## ##  ##   ##    ####   ");
            Console.WriteLine(@"    ##  ## ## # ## ####   ####     ##   ###### #####    ##     ##     ");
            Console.WriteLine(@"   ##  ## ####### ##     ##       ##   ##  ## ##  ##   ##    ####     ");
            Console.WriteLine(@"  #####   ## ##  ###### ######   ##   ##  ## #####  ###### ##  ##     ");
            Console.WriteLine(@"                                                                      ");
            Console.WriteLine(@"                   --== Version " + version + " ==--                  ");
            Console.WriteLine(@"                                                                      ");
            Console.WriteLine(@"                                                                      ");

            // Setup close handler
            _handler += new EventHandler(Handler);
            SetConsoleCtrlHandler(_handler, true);

            // Setup serial port
            _serial = new SerialService(Config.Instance.Serial.Port,
                Config.Instance.Serial.BuadRate,
                Config.Instance.Serial.DataBits,
                Config.Instance.Serial.StopBits,
                Config.Instance.Serial.Parity,
                Config.Instance.General.Timeout);

            _serial.Connected += (o, e) =>
            {
                _dweet.ListenForDweets();
            };

            _serial.Disconnected += (o, e) =>
            {
                _dweet.StopListeningForDweets();
                _serial.Connect(5);
            };

            // Setup dweet service
            _dweet = new DweetService(Config.Instance.Dweet.Thing, 
                Config.Instance.General.Timeout);

            _dweet.DweetReceived += (o, e) =>
            {
                var msg = e.Dweet.Content.As<Message>().Msg;
                LogHelper.Info("Dweet received: " + msg);
                _serial.Write(msg);
            };

            // Start everything off
            _serial.Connect(5);

            // Loop
            while(true)
            {
                _serial.Update();
                _dweet.Update();
            }
        }

        // Cleanup
        private static bool Handler(int eventType)
        {
            if(_dweet != null)
            {
                _dweet.StopListeningForDweets();
                _dweet.Dispose();
            }

            if (_serial != null)
            {
                _serial.Disconnect(false);
                _serial.Dispose();
            }

            return true;
        }
    }
}
