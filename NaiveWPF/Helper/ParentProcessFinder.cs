using System;
using System.Diagnostics;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace NaiveGUI.Helper
{
    // Another useful paste
    // https://stackoverflow.com/a/3346055
    [StructLayout(LayoutKind.Sequential)]
    public struct ParentProcessFinder
    {
        // These members must match PROCESS_BASIC_INFORMATION
        internal IntPtr Reserved1;
        internal IntPtr PebBaseAddress;
        internal IntPtr Reserved2_0;
        internal IntPtr Reserved2_1;
        internal IntPtr UniqueProcessId;
        internal IntPtr InheritedFromUniqueProcessId;

        [DllImport("ntdll.dll")]
        private static extern int NtQueryInformationProcess(IntPtr processHandle, int processInformationClass, ref ParentProcessFinder processInformation, int processInformationLength, out int returnLength);

        /// <summary>
        /// Gets the parent process of a specified process.
        /// </summary>
        public static Process GetParentProcess(IntPtr handle)
        {
            var pbi = new ParentProcessFinder();
            int status = NtQueryInformationProcess(handle, 0, ref pbi, Marshal.SizeOf(pbi), out int returnLength);
            if(status != 0)
            {
                throw new Win32Exception(status);
            }
            try
            {
                return Process.GetProcessById(pbi.InheritedFromUniqueProcessId.ToInt32());
            }
            catch
            {
                return null;
            }
        }
    }
}
