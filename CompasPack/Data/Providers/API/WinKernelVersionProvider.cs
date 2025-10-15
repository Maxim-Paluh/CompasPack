using System;
using System.Runtime.InteropServices;

/* 
 * .NET Framework використовує для заповнення Environment.OSVersion функцію GetVersionEx() яка без маніфесту та пункту compatibility (сумісність) видає для сумісності версію Windows 8 (6.2.9200) починаючи з Windows 8.1
 * .NET Core / .NET 5+ / 6+ використовує для заповнення Environment.OSVersion функцію RtlGetVersion внутрішня функція з ntdll.dll яка дає точну інформацію без маніфесту, і працює стабільно на всіх NT-платформах незалежно від маніфесту
 * Тому краще реалізувати ось таку функцію щоб не залежати він реалізацій .NET
 */
public static class WinKernelVersionProvider
{
    [DllImport("ntdll.dll", SetLastError = true)]
    private static extern int RtlGetVersion(ref OSVERSIONINFOEX lpVersionInformation);

    [StructLayout(LayoutKind.Sequential)]
    private struct OSVERSIONINFOEX
    {
        public int dwOSVersionInfoSize;
        public int dwMajorVersion;
        public int dwMinorVersion;
        public int dwBuildNumber;
        public int dwPlatformId;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        public string szCSDVersion;
    }

    public static Version GetKernelVersion()
    {
        var osVersionInfo = new OSVERSIONINFOEX
        {
            dwOSVersionInfoSize = Marshal.SizeOf(typeof(OSVERSIONINFOEX))
        };

        int result = RtlGetVersion(ref osVersionInfo);
        if (result != 0)
            throw new InvalidOperationException("RtlGetVersion failed");

        return new Version(osVersionInfo.dwMajorVersion, osVersionInfo.dwMinorVersion, osVersionInfo.dwBuildNumber);
    }
}