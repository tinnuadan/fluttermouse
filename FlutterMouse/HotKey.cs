using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Mime;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;

namespace FlutterMouse
{
  [Flags]
  public enum KeyModifier
  {
    None = 0x0000,
    Alt = 0x0001,
    Ctrl = 0x0002,
    NoRepeat = 0x4000,
    Shift = 0x0004,
    Win = 0x0008
  }

  class HotKey : IDisposable
  {

    #region Members
    private static Dictionary<int, HotKey> HotKeyToCallBackProc;

    public Key Key { get; private set; }
    public KeyModifier KeyModifiers { get; private set; }
    public Action<HotKey> Action { get; private set; }
    public bool Registered { get; private set; }

    private const int WmHotKey = 0x0312;
    private int Id { get; set; }
    #endregion

    #region Public Methods
    public HotKey(Key k, KeyModifier keyModifiers, Action<HotKey> action, bool register = true)
    {
      Key = k;
      KeyModifiers = keyModifiers;
      Action = action;
      Registered = false;
      if (register)
      {
        Register();
      }
    }

    public bool Register()
    {
      if(Registered)
      {
        Debug.Print("Has to be unregistered first");
        return false;
      }
      int virtKeyCode = KeyInterop.VirtualKeyFromKey(Key);
      Id = virtKeyCode + ((int)KeyModifiers << 16);
      bool result = RegisterHotKey(IntPtr.Zero, Id, (UInt32)KeyModifiers, (UInt32)virtKeyCode);

      if (HotKeyToCallBackProc == null)
      {
        // initialise static hotkey mapping if it doesn't exist yet
        HotKeyToCallBackProc = new Dictionary<int, HotKey>();
        // add dispatcher method for windows messages
        ComponentDispatcher.ThreadFilterMessage += new ThreadMessageEventHandler(OnWindowsMessage);
      }

      if(result)
      {
        HotKeyToCallBackProc.Add(Id, this);
        Registered = true;
      }
      
      return result;
    }

    public bool Unregister()
    {
      bool result = false;
      if (Registered)
      {
        Debug.Print("Is not registered");
      }
      else
      {
        HotKey hotKey;
        if (HotKeyToCallBackProc.TryGetValue(Id, out hotKey))
        {
          result = UnregisterHotKey(IntPtr.Zero, Id);
          if (result)
          {
            Registered = false;
          }
        }
      }
      return result;
    }
    #endregion

    #region Private Methods
    private static void OnWindowsMessage(ref MSG msg, ref bool handled)
    {
      if(!handled && msg.message == WmHotKey)
      {
        HotKey hotKey;
        if(HotKeyToCallBackProc.TryGetValue((int)msg.wParam, out hotKey))
        {
          if(hotKey.Action != null)
          {
            hotKey.Action.Invoke(hotKey);
          }
          handled = true;
        }
      }
    }

    [DllImport("user32.dll")]
    private static extern bool RegisterHotKey(IntPtr hWnd, int id, UInt32 fsModifiers, UInt32 vlc);

    [DllImport("user32.dll")]
    private static extern bool UnregisterHotKey(IntPtr hWnd, int id);
    #endregion

    #region IDisposable Support
    private bool disposedValue = false; // To detect redundant calls

    protected virtual void Dispose(bool disposing)
    {
      if (!disposedValue)
      {
        if (disposing && Registered)
        {
          Unregister();
        }
        disposedValue = true;
      }
    }

    // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
    // ~HotKey() {
    //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
    //   Dispose(false);
    // }

    // This code added to correctly implement the disposable pattern.
    public void Dispose()
    {
      Dispose(true);
      GC.SuppressFinalize(this);
    }
    #endregion
  }
}
