using System;
using System.Runtime.InteropServices;

namespace dotSwitcher
{
    [StructLayout(LayoutKind.Sequential)]
    public struct KeyData
    {
        public UInt32 vkCode;
        public UInt32 scanCode;
        public KeyDataFlags flags;
        public UInt32 time;
        public IntPtr dwExtraInfo;
    }

    [Flags()]
    public enum KeyDataFlags
    {
        LLKHF_EXTENDED = 0x01,
        LLKHF_INJECTED = 0x10,
        LLKHF_ALTDOWN = 0x20,
        LLKHF_UP = 0x80,
        KEYEVENTF_KEYUP = 0x02
    }

    public class HookEventData
    {
        public KeyData KeyData { get; private set; }
        public bool CtrlIsPressed { get; private set; }
        public bool AltIsPressed { get; private set; }
        public bool ShiftIsPressed { get; private set; }

        public HookEventData(KeyData data, bool ctrl, bool alt, bool shift)
        {
            KeyData = data;
            CtrlIsPressed = ctrl;
            AltIsPressed = alt;
            ShiftIsPressed = shift;
        }
    }

    public class DummyHookEventData : HookEventData
    {
        public new KeyData KeyData
        {
            get
            {
                WrongUsage();
                return new KeyData();
            }
        }

        public new bool CtrlIsPressed
        {
            get
            {
                WrongUsage();
                return false;
            }
        }

        public new bool AltIsPressed
        {
            get
            {
                WrongUsage();
                return false;
            }
        }

        public new bool ShiftIsPressed
        {
            get
            {
                WrongUsage();
                return false;
            }
        }

        public DummyHookEventData() : base(new KeyData(), false, false, false)
        {
        }

        public void WrongUsage()
        {
            throw new NotImplementedException("This is a DummyHookEventData");
        }
    }

    public class HookId
    {
        public IntPtr HookResult { get; set; }
        public static HookId Empty = new HookId {HookResult = IntPtr.Zero};

        public bool IsEmpty()
        {
            return HookResult == IntPtr.Zero;
        }
    }

    internal struct INPUT
    {
        public UInt32 Type;
        public MOUSEKEYBDHARDWAREINPUT Data;
    }

    [StructLayout(LayoutKind.Explicit)]
    internal struct MOUSEKEYBDHARDWAREINPUT
    {
        [FieldOffset(0)] public MOUSEINPUT Mouse;
        [FieldOffset(0)] public KEYBDINPUT Keyboard;
        [FieldOffset(0)] public HARDWAREINPUT Hardware;
    }

#pragma warning disable 649
    internal struct MOUSEINPUT
    {
        public Int32 X;
        public Int32 Y;
        public UInt32 MouseData;
        public UInt32 Flags;
        public UInt32 Time;
        public IntPtr ExtraInfo;
    }

    internal struct KEYBDINPUT
    {
        public UInt16 Vk;
        public UInt16 Scan;
        public UInt32 Flags;
        public UInt32 Time;
        public IntPtr ExtraInfo;
    }

    internal struct HARDWAREINPUT
    {
        public UInt32 Msg;
        public UInt16 ParamL;
        public UInt16 ParamH;
    }
#pragma warning restore 649

}