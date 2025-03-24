using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using WinForms = System.Windows.Forms;
using System.Reflection;
using System.ComponentModel;

namespace LFFSSK
{
    public class KeyboardHook
    {
        const string TraceCategory = "LFFSSK.Helper.KeyboardHook";
        #region Delegates

        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

        #endregion

        #region Constants

        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0100;
        private const int WM_SYSKEYDOWN = 0x104;

        #endregion

        #region Variables

        private static LowLevelKeyboardProc _proc = HookCallback;
        private static IntPtr _hookID = IntPtr.Zero;

        #endregion

        #region Public Methods

        public void SetKeyboardHook()
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                _hookID = SetWindowsHookEx(WH_KEYBOARD_LL, _proc, GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        public void UnhookKeyboard()
        {
            if (_hookID != IntPtr.Zero)
                UnhookWindowsHookEx(_hookID);
        }

        #endregion

        #region Private Methods

        private void EnsureSubscribedToGlobalKeyboardEvents()
        {
            if (_hookID == IntPtr.Zero)
            {
                _proc = HookCallback;

                SetKeyboardHook();

                if (_hookID == IntPtr.Zero)
                {
                    int errorCode = Marshal.GetLastWin32Error();
                    throw new Win32Exception(errorCode);
                }
            }
        }

        private void TryUnsubscribeFromGlobalKeyboardEvents()
        {
            if (s_KeyDown == null)
            {
                ForceUnsunscribeFromGlobalKeyboardEvents();
            }
        }

        private void ForceUnsunscribeFromGlobalKeyboardEvents()
        {
            try
            {
                if (_hookID != IntPtr.Zero)
                {
                    bool canUnhook = UnhookWindowsHookEx(_hookID);
                    _hookID = IntPtr.Zero;
                    _proc = null;

                    if (!canUnhook)
                    {
                        int errorCode = Marshal.GetLastWin32Error();
                        throw new Win32Exception(errorCode);
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceError, string.Format("[ERROR] ForceUnsunscribeFromGlobalKeyboardEvents: {0}", ex.ToString()), TraceCategory);
            }
        }

        private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {

            //if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN || wParam == (IntPtr)WM_SYSKEYDOWN)
            //{
            //    int vkCode = Marshal.ReadInt32(lParam);

            //    KeyEventArgs e = new KeyEventArgs((Keys)vkCode);
            //    s_KeyDown.Invoke(null, e);
            //}
            if (nCode >= 0)
            {
                bool Alt = (WinForms.Control.ModifierKeys & Keys.Alt) != 0;
                bool Control = (WinForms.Control.ModifierKeys & Keys.Control) != 0;

                //Prevent ALT-TAB and CTRL-ESC by eating TAB and ESC. Also kill Windows Keys.
                int vCode = Marshal.ReadInt32(lParam);
                Keys key = (Keys)vCode;
                if (key == Keys.CapsLock)
                    Debug.WriteLine("Capital");
                if (Alt && key == Keys.F4) return (IntPtr)1; //handled
                if (Alt && key == Keys.Tab) return (IntPtr)1; //handled
                if (Alt && key == Keys.Space) return (IntPtr)1; //handled
                if (Alt && key == Keys.Escape) return (IntPtr)1; //handled
                if (Control && key == Keys.Escape) return (IntPtr)1; //handled
                if (Control && key == Keys.V) return (IntPtr)1; //handled
                if (key == Keys.LWin || key == Keys.RWin) return (IntPtr)1; //handled
                if (key == Keys.None) return (IntPtr)1; //handled
                if (key == Keys.Menu) return (IntPtr)1; //handled
                if (key == Keys.Pause) return (IntPtr)1; //handled
                if (key == Keys.Help) return (IntPtr)1; //handled
                if (key == Keys.Sleep) return (IntPtr)1; //handled
                if (key == Keys.Apps) return (IntPtr)1; //handled
                if (key >= Keys.KanaMode && key <= Keys.HanjaMode) return (IntPtr)1; //handled
                if (key >= Keys.IMEConvert && key <= Keys.IMEModeChange) return (IntPtr)1; //handled
                if (key >= Keys.BrowserBack && key <= Keys.BrowserHome) return (IntPtr)1; //handled
                //if (key >= Keys.MediaNextTrack && key <= Keys.OemClear) return (IntPtr)1; //handled

                if (s_KeyDown != null && (wParam == (IntPtr)WM_KEYDOWN || wParam == (IntPtr)WM_SYSKEYDOWN))
                {
                    KeyEventArgs e = new KeyEventArgs(key);
                    s_KeyDown.Invoke(null, e);
                }
            }

            return CallNextHookEx(_hookID, nCode, wParam, lParam);
        }

        #endregion

        #region Native Methods

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        #endregion

        #region Event Handler

        private static event KeyEventHandler s_KeyDown;

        public event KeyEventHandler KeyDown
        {
            add
            {
                EnsureSubscribedToGlobalKeyboardEvents();
                s_KeyDown += value;
            }
            remove
            {
                s_KeyDown -= value;
                TryUnsubscribeFromGlobalKeyboardEvents();
            }
        }

        #endregion
    }
}
