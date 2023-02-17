using System;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Threading;

namespace SysTimeSynchronizer
{
    class Program
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct SYSTEMTIME
        {
            public short wYear;
            public short wMonth;
            public short wDayOfWeek;
            public short wDay;
            public short wHour;
            public short wMinute;
            public short wSecond;
            public short wMilliseconds;
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool SetSystemTime(ref SYSTEMTIME st);

        static void Main(string[] args)
        {
            new ManualResetEvent(false).WaitOne(5000);
            var time = GetNetworkTime();
            
            SYSTEMTIME st = new SYSTEMTIME();
            st.wYear = Convert.ToInt16(time.Year);
            st.wSecond = Convert.ToInt16(time.Second);
            st.wMonth = Convert.ToInt16(time.Month);
            st.wMinute = Convert.ToInt16(time.Minute);
            st.wMilliseconds = Convert.ToInt16(time.Millisecond);
            st.wHour = Convert.ToInt16(time.Hour);
            st.wDayOfWeek = Convert.ToInt16(time.DayOfWeek);
            st.wDay = Convert.ToInt16(time.Day);

            SetSystemTime(ref st);
            
        }

        public static DateTime GetNetworkTime()
        {
            try
            {
                const string ntpServer = "0.ru.pool.ntp.org";
                var data = new byte[48];
                data[0] = 0x1B;

                var address = Dns.GetHostEntry(ntpServer).AddressList[0];
                var iep = new IPEndPoint(address, 123);
                var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            
                socket.Connect(iep);
                socket.Send(data);
                socket.Receive(data);
                socket.Close();

                ulong intPart = (ulong)data[40] << 24 | (ulong)data[41] << 16 | (ulong)data[42] << 8 | (ulong)data[43];
                ulong fractPart = (ulong)data[44] << 24 | (ulong)data[45] << 16 | (ulong)data[46] << 8 | (ulong)data[47];

                var milliseconds = (intPart * 1000) + ((fractPart * 1000) / 0x100000000L);
                var target = (new DateTime(1900, 1, 1)).AddMilliseconds((long)milliseconds);

                return target;
            } catch
            {
                Environment.Exit(1);
                return new DateTime();
            }
        }
    }
}
