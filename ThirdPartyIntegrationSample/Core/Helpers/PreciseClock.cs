using System;
using System.Runtime.InteropServices;

namespace Core.Helpers
{
    public static class PreciseClock
    {
#if Windows
        [SuppressUnmanagedCodeSecurity, DllImport("kernel32.dll")]
        static extern void GetSystemTimePreciseAsFileTime(out FILE_TIME lpSystemTimeAsFileTime);
#endif
        [StructLayout(LayoutKind.Sequential)]
        struct FILE_TIME
        {
            public uint ftTimeLow;
            public uint ftTimeHigh;
        }

        private static readonly DateTime FileSystemBaseDate = new DateTime(1601, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        static DateTime GetTimePrecise()
        {
#if Windows
            GetSystemTimePreciseAsFileTime(out var fileTime);
            var ticks = ((ulong)fileTime.ftTimeHigh << 32) + fileTime.ftTimeLow;
            return FileSystemBaseDate.AddTicks((long)ticks);
#else
            return DateTime.UtcNow;
#endif
        }

        static Func<DateTime> Init()
        {
            try
            {
                GetTimePrecise();
                return GetTimePrecise;
            }
            catch (EntryPointNotFoundException)
            {
                return () => DateTime.UtcNow;
            }
        }

        private static readonly Func<DateTime> GetUtcTime = Init();

        public static DateTime UtcNow => GetUtcTime();
        public static bool IsPrecise => GetUtcTime == GetTimePrecise;
    }
}