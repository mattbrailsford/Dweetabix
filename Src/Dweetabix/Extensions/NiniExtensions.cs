using Nini.Config;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dweetabix.Extensions
{
    public static class NiniExtensions
    {
        public static StopBits GetStopBits(this IConfig cfg, string key, StopBits defaultValue = StopBits.One)
        {
            var value = cfg.GetFloat(key, -1);
            if (value == 0) return StopBits.None;
            if (value == 1) return StopBits.One;
            if (value == 1.5) return StopBits.OnePointFive;
            if (value == 2) return StopBits.Two;
            return defaultValue;
        }

        public static TEnum GetEnum<TEnum>(this IConfig cfg, string key, TEnum defaultValue = default(TEnum))
        {
            var value = cfg.Get(key, defaultValue.ToString());
            try
            {
                return (TEnum)Enum.Parse(typeof(TEnum), value, true);
            }
            catch
            {
                return defaultValue;
            }
        }
    }
}
