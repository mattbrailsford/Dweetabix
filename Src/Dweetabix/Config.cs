using Nini.Config;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dweetabix.Extensions;

namespace Dweetabix
{
    public class Config
    {
        private static Config _instance;

        public DweetConfig Dweet { get; private set; }
        public SerialConfig Serial { get; private set; }
        public GeneralConfig General { get; set; }

        static Config()
        {
            var cfg = new IniConfigSource("Dweetabix.ini");

            _instance = new Config
            {
                Dweet = new DweetConfig
                {
                    Thing = cfg.Configs["Dweet"].Get("dweet_thing")
                },
                Serial = new SerialConfig
                {
                    Port = cfg.Configs["Serial"].Get("com_port"),
                    BuadRate = cfg.Configs["Serial"].GetInt("com_baud", 9660),
                    DataBits = cfg.Configs["Serial"].GetInt("com_databits", 8),
                    StopBits = cfg.Configs["Serial"].GetStopBits("com_stopbits", StopBits.One),
                    Parity = cfg.Configs["Serial"].GetEnum<Parity>("parity", Parity.None)
                },
                General = new GeneralConfig
                {
                    Timeout = cfg.Configs["General"].GetInt("timeout", 3000)
                }
            };
        }

        public static Config Instance
        {
            get { return _instance; }
        }
    }

    #region ConfigModels

    public class DweetConfig
    {
        public string Thing { get; set; }
    }

    public class SerialConfig
    {
        public string Port { get; set; }
        public int BuadRate { get; set; }
        public int DataBits { get; set; }
        public StopBits StopBits { get; set; }
        public Parity Parity { get; set; }
    }

    public class GeneralConfig
    {
        public int Timeout { get; set; }
    }

    #endregion
}
