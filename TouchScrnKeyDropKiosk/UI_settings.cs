using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices; //IMPORTANT FOR FULL SCREEN FUNCTIONALITY


namespace KeyCabinetKiosk
{
    public static class UI_settings
    {


        // Full Screen Form Code
        [DllImport("user32.dll")]
        private static extern int FindWindow(string lpszClassName, string lpszWindowName);
        [DllImport("user32.dll")]
        private static extern int ShowWindow(int hWnd, int nCmdShow);
        private const int SW_HIDE = 0;
        private const int SW_SHOW = 1;

        /// <summary>
        /// Hides windows taskbar and makes program fullscreen
        /// </summary>
        public static void StartApp()
        {
            int hWnd = FindWindow("Shell_TrayWnd", "");
            ShowWindow(hWnd, SW_HIDE);
        }
        
        /// <summary>
        /// Restores taskbar
        /// </summary>
        public static void Restore()
        {
            int hwnd = FindWindow("Shell_TrayWnd", "");
            ShowWindow(hwnd, SW_SHOW);
        }
    }
}
