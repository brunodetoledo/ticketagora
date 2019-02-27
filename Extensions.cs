using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace TA.Location
{
    internal static class Extensions
    {
        internal static bool IsInternal(this IPAddress toTest)
        {
            if (IPAddress.IsLoopback(toTest))
            {
                return true;
            }

            if (toTest.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
            {
                byte[] bytes = toTest.GetAddressBytes();
                switch (bytes[0])
                {
                    case 10:
                        return true;
                    case 172:
                        return bytes[1] < 32 && bytes[1] >= 16;
                    case 192:
                        return bytes[1] == 168;
                    default:
                        return false;
                }
            }

            return false;
        }
    }
}
