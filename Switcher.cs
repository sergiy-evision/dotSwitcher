using System;
using System.Collections.Generic;
using System.Linq;

namespace dotSwitcher
{
    public class Switcher : IDisposable
    {
        public event EventHandler<SwitcherErrorArgs> Error;
        private HookId _keyboardHook = HookId.Empty;
        private HookId _mouseHook = HookId.Empty;
        private readonly List<HookEventData> _currentWord = new List<HookEventData>();

        public bool IsStarted()
        {
            return !_keyboardHook.IsEmpty();
        }

        public void Start()
        {
            if (IsStarted())
            {
                return;
            }
            _keyboardHook = LowLevelAdapter.SetKeyboardHook(ProcessKeyPress);
            _mouseHook = LowLevelAdapter.SetMouseHook(ProcessMousePress);
        }

        public void Stop()
        {
            if (!IsStarted())
            {
                return;
            }
            LowLevelAdapter.ReleaseKeyboardHook(_keyboardHook);
            LowLevelAdapter.ReleaseKeyboardHook(_mouseHook);
            _mouseHook = HookId.Empty;
            _keyboardHook = HookId.Empty;
        }

        private void ProcessKeyPress(HookEventData evtData)
        {
            try
            {
                OnKeyPress(evtData);
            }
            catch (Exception ex)
            {
                OnError(ex);
            }
        }

        private void ProcessMousePress(HookEventData evtData)
        {
            try
            {
                BeginNewWord();
            }
            catch (Exception ex)
            {
                OnError(ex);
            }
        }

        private void OnKeyPress(HookEventData evtData)
        {
            var vkCode = evtData.KeyData.vkCode;
            var ctrl = evtData.CtrlIsPressed;
            var alt = evtData.AltIsPressed;
            var notModified = !ctrl && !alt;
            var shift = evtData.ShiftIsPressed;

            if (vkCode == VirtualKeyStates.VK_CONTROL ||
                vkCode == VirtualKeyStates.VK_LCONTROL ||
                vkCode == VirtualKeyStates.VK_RCONTROL ||
                vkCode == VirtualKeyStates.VK_SNAPSHOT ||
                vkCode == VirtualKeyStates.VK_SHIFT ||
                vkCode == VirtualKeyStates.VK_RSHIFT ||
                vkCode == VirtualKeyStates.VK_LSHIFT)
            {
                return;
            }
            if (vkCode == VirtualKeyStates.VK_SPACE && notModified)
            {
                AddToCurrentWord(evtData);
                return;
            }
            if (vkCode == VirtualKeyStates.VK_BACK && notModified)
            {
                RemoveLast();
                return;
            }
            if (VirtualKeyStates.IsPrintable(evtData))
            {
                if (GetPreviousVkCode() == VirtualKeyStates.VK_SPACE)
                {
                    BeginNewWord();
                }
                AddToCurrentWord(evtData);
                return;
            }
            // todo make it global hotkey someday
            if (vkCode == VirtualKeyStates.VK_PAUSE)
            {
                if (shift)
                {
                    ConvertSelection();
                }
                else
                {
                    ConvertLast();
                }
                return;
            }
            // default: 
            BeginNewWord();

        }

        private void OnError(Exception ex)
        {
            if (Error != null)
            {
                Error(this, new SwitcherErrorArgs(ex));
            }
        }

        #region word manipulation

        // returns 0 if currentWord is empty
        private uint GetPreviousVkCode()
        {
            if (_currentWord.Count == 0)
            {
                return 0;
            }
            return _currentWord.Last().KeyData.vkCode;
        }

        private void BeginNewWord()
        {
            _currentWord.Clear();
        }

        private void AddToCurrentWord(HookEventData data)
        {
            _currentWord.Add(data);
        }

        private void RemoveLast()
        {
            if (_currentWord.Count == 0)
            {
                return;
            }
            _currentWord.RemoveAt(_currentWord.Count - 1);
        }

        #endregion

        private void ConvertSelection()
        {
            throw new NotImplementedException();
        }

        private void ConvertLast()
        {
            var word = _currentWord.ToList();
            var backspaces = Enumerable.Repeat<uint>(VirtualKeyStates.VK_BACK, word.Count);

            LowLevelAdapter.SetNextKeyboardLayout();
            foreach (var vkCode in backspaces)
            {
                LowLevelAdapter.SendKeyPress(vkCode);
            }
            foreach (var data in word)
            {
                LowLevelAdapter.SendKeyPress(data.KeyData.vkCode, data.ShiftIsPressed);
            }
        }

        public void Dispose()
        {
            Stop();
        }
    }

    public class SwitcherErrorArgs : EventArgs
    {
        public Exception Error { get; private set; }

        public SwitcherErrorArgs(Exception ex)
        {
            Error = ex;
        }
    }
}