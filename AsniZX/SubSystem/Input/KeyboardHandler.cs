using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SharpDX.DirectInput;
using System.Threading;
using SharpDX;
using System.Runtime.Serialization;

namespace AsniZX.SubSystem.Input
{
    public class KeyboardHandler
    {
        public static KeyboardHandler Instance { get; private set; }
        readonly Thread UpdateThread;
        public static bool AbortThread { private get; set; }

        [Flags]
        public enum ModifierKey
        {
            // Summary:
            //     The bitmask to extract modifiers from a key value.
            Modifiers = -65536,
            //
            // Summary:
            //     No key pressed.
            None = 0,
            //
            // Summary:
            //     The SHIFT modifier key.
            Shift = 65536,
            //
            // Summary:
            //     The CTRL modifier key.
            Control = 131072,
            //
            // Summary:
            //     The ALT modifier key.
            Alt = 262144,
        }

        public KeyboardHandler()
        {
            AbortThread = false;

            UpdateThread = new Thread(UpdateThreadProc)
            {
                IsBackground = true,
                Priority = ThreadPriority.AboveNormal
            };

            UpdateThread.Start();
        }

        public static void Initialize(ZXForm form)
        {

            KeyInput.Init(form);

            Instance = new KeyboardHandler();
        }

        public void Dispose()
        {
            AbortThread = true;
        }

        public enum InputEventType
        {
            Press,
            Release
        }

        void HandleButton(string button, bool newState)
        {
            bool isModifier = IgnoreKeys.Contains(button);
            if (EnableIgnoreModifiers && isModifier) return;
            if (LastState[button] && newState) return;
            if (!LastState[button] && !newState) return;

            //apply 
            //NOTE: this is not quite right. if someone held leftshift+rightshift it would be broken. seems unlikely, though.
            if (button == "LeftShift")
            {
                _Modifiers &= ~ModifierKey.Shift;
                if (newState)
                    _Modifiers |= ModifierKey.Shift;
            }
            if (button == "RightShift") { _Modifiers &= ~ModifierKey.Shift; if (newState) _Modifiers |= ModifierKey.Shift; }
            if (button == "LeftControl") { _Modifiers &= ~ModifierKey.Control; if (newState) _Modifiers |= ModifierKey.Control; }
            if (button == "RightControl") { _Modifiers &= ~ModifierKey.Control; if (newState) _Modifiers |= ModifierKey.Control; }
            if (button == "LeftAlt") { _Modifiers &= ~ModifierKey.Alt; if (newState) _Modifiers |= ModifierKey.Alt; }
            if (button == "RightAlt") { _Modifiers &= ~ModifierKey.Alt; if (newState) _Modifiers |= ModifierKey.Alt; }

            if (UnpressState.ContainsKey(button))
            {
                if (newState) return;
                Console.WriteLine("Removing Unpress {0} with newState {1}", button, newState);
                UnpressState.Remove(button);
                LastState[button] = false;
                return;
            }


            //dont generate events for things like Ctrl+LeftControl
            ModifierKey mods = _Modifiers;
            if (button == "LeftShift") mods &= ~ModifierKey.Shift;
            if (button == "RightShift") mods &= ~ModifierKey.Shift;
            if (button == "LeftControl") mods &= ~ModifierKey.Control;
            if (button == "RightControl") mods &= ~ModifierKey.Control;
            if (button == "LeftAlt") mods &= ~ModifierKey.Alt;
            if (button == "RightAlt") mods &= ~ModifierKey.Alt;

            var ie = new InputEvent
            {
                EventType = newState ? InputEventType.Press : InputEventType.Release,
                LogicalButton = new LogicalButton(button, mods)
            };
            LastState[button] = newState;

            //track the pressed events with modifiers that we send so that we can send corresponding unpresses with modifiers
            //this is an interesting idea, which we may need later, but not yet.
            //for example, you may see this series of events: press:ctrl+c, release:ctrl, release:c
            //but you might would rather have press:ctr+c, release:ctrl+c
            //this code relates the releases to the original presses.
            //UPDATE - this is necessary for the frame advance key, which has a special meaning when it gets stuck down
            //so, i am adding it as of 11-sep-2011
            if (newState)
            {
                ModifierState[button] = ie.LogicalButton;
            }
            else
            {
                if (ModifierState[button] != null)
                {
                    LogicalButton alreadyReleased = ie.LogicalButton;
                    var ieModified = new InputEvent
                    {
                        LogicalButton = (LogicalButton)ModifierState[button],
                        EventType = InputEventType.Release
                    };
                    if (ieModified.LogicalButton != alreadyReleased)
                        _NewEvents.Add(ieModified);
                }
                ModifierState[button] = null;
            }

            _NewEvents.Add(ie);
        }

        private readonly WorkingDictionary<string, object> ModifierState = new WorkingDictionary<string, object>();
        private readonly WorkingDictionary<string, bool> LastState = new WorkingDictionary<string, bool>();
        private readonly WorkingDictionary<string, bool> UnpressState = new WorkingDictionary<string, bool>();
        private readonly HashSet<string> IgnoreKeys = new HashSet<string>(new[] { "LeftShift", "RightShift", "LeftControl", "RightControl", "LeftAlt", "RightAlt" });

        public class InputEvent
        {
            public LogicalButton LogicalButton;
            public InputEventType EventType;
            public override string ToString()
            {
                return string.Format("{0}:{1}", EventType.ToString(), LogicalButton.ToString());
            }
        }

        ModifierKey _Modifiers;
        public readonly List<InputEvent> _NewEvents = new List<InputEvent>();

        //do we need this?
        public void ClearEvents()
        {
            lock (this)
            {
                InputEvents.Clear();
            }
        }

        public readonly Queue<InputEvent> InputEvents = new Queue<InputEvent>();
        public InputEvent DequeueEvent()
        {
            lock (this)
            {
                if (InputEvents.Count == 0) return null;
                else return InputEvents.Dequeue();
            }
        }
        void EnqueueEvent(InputEvent ie)
        {
            lock (this)
            {
                InputEvents.Enqueue(ie);
            }
        }

        void UpdateThreadProc()
        {
            for (;;)
            {
                if (AbortThread == true)
                {
                    break;
                }
                /*

                var keyEvents = KeyInput.Update();
                
                lock (this)
                {
                    _NewEvents.Clear();

                    //analyze keys
                    foreach (var ke in keyEvents)
                        HandleButton(ke.Key.ToString(), ke.Pressed);

                    foreach (var ie in _NewEvents)
                    {
                        //events are swallowed in some cases:
                        if (ie.LogicalButton.Alt) // && !GlobalWin.MainForm.AllowInput(true))
                        { }
                        else if (ie.EventType == InputEventType.Press) // && swallow)
                        { }
                        else
                            EnqueueEvent(ie);
                    }
                } //lock(this)

                //arbitrary selection of polling frequency:
                */
                Thread.Sleep(10);
            }
        }

        public struct LogicalButton
        {
            public LogicalButton(string button, ModifierKey modifiers)
            {
                Button = button;
                Modifiers = modifiers;
            }
            public readonly string Button;
            public readonly ModifierKey Modifiers;

            public bool Alt { get { return ((Modifiers & ModifierKey.Alt) != 0); } }
            public bool Control { get { return ((Modifiers & ModifierKey.Control) != 0); } }
            public bool Shift { get { return ((Modifiers & ModifierKey.Shift) != 0); } }


            public override string ToString()
            {
                string ret = "";
                /*
                if (Control) ret += "Ctrl+";
                if (Alt) ret += "Alt+";
                if (Shift) ret += "Shift+";
                */
                ret += Button;
                return ret;
            }


            public override bool Equals(object obj)
            {
                var other = (LogicalButton)obj;
                return other == this;
            }
            public override int GetHashCode()
            {
                return Button.GetHashCode() ^ Modifiers.GetHashCode();
            }
            public static bool operator ==(LogicalButton lhs, LogicalButton rhs)
            {
                return lhs.Button == rhs.Button && lhs.Modifiers == rhs.Modifiers;
            }
            public static bool operator !=(LogicalButton lhs, LogicalButton rhs)
            {
                return !(lhs == rhs);
            }
        }

        public void Update()
        {
            //TODO - for some reason, we may want to control when the next event processing step happens
            //so i will leave this method here for now..
        }

        //returns the next Press event, if available. should be useful
        public string GetNextBindEvent()
        {
            //this whole process is intimately involved with the data structures, which can conflict with the input thread.
            lock (this)
            {
                if (InputEvents.Count == 0) return null;
                //if (!GlobalWin.MainForm.AllowInput(false)) return null;

                //we only listen to releases for input binding, because we need to distinguish releases of pure modifierkeys from modified keys
                //if you just pressed ctrl, wanting to bind ctrl, we'd see: pressed:ctrl, unpressed:ctrl
                //if you just pressed ctrl+c, wanting to bind ctrl+c, we'd see: pressed:ctrl, pressed:ctrl+c, unpressed:ctrl+c, unpressed:ctrl
                //so its the first unpress we need to listen for

                while (InputEvents.Count != 0)
                {
                    var ie = DequeueEvent();

                    //as a special perk, we'll accept escape immediately
                    if (ie.EventType == InputEventType.Press && ie.LogicalButton.Button == "Escape")
                        goto ACCEPT;

                    if (ie.EventType == InputEventType.Press) continue;

                    ACCEPT:
                    Console.WriteLine("Bind Event: {0} ", ie);

                    foreach (var kvp in LastState)
                        if (kvp.Value)
                        {
                            Console.WriteLine("Unpressing " + kvp.Key);
                            UnpressState[kvp.Key] = true;
                        }

                    InputEvents.Clear();

                    return ie.LogicalButton.ToString();
                }

                return null;
            }
        }

        //controls whether modifier keys will be ignored as key press events
        //this should be used by hotkey binders, but we may want modifier key events
        //to get triggered in the main form
        public bool EnableIgnoreModifiers = false;

        //sets a key as unpressed for the binding system
        /*
        public void BindUnpress(System.Windows.Forms.Keys key)
        {
            //only validated for Return
            string keystr = key.ToString();
            UnpressState[keystr] = true;
            LastState[keystr] = true;
        }
        */
    }

    /// <summary>
	/// A dictionary that creates new values on the fly as necessary so that any key you need will be defined. 
	/// </summary>
	/// <typeparam name="K">dictionary keys</typeparam>
	/// <typeparam name="V">dictionary values</typeparam>
	[Serializable]
    public class WorkingDictionary<K, V> : Dictionary<K, V> where V : new()
    {
        public new V this[K key]
        {
            get
            {
                V temp;
                if (!TryGetValue(key, out temp))
                {
                    temp = this[key] = new V();
                }

                return temp;
            }

            set
            {
                base[key] = value;
            }
        }

        public WorkingDictionary() { }

        protected WorkingDictionary(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }


    public struct KeyEvent
    {
        public Key Key;
        public bool Pressed;
    }

    public static class KeyInput
    {
        private static DirectInput dinput;
        private static Keyboard keyboard;
        private static KeyboardState state = new KeyboardState();

        public static void Dispose()
        {

        }

        public static void Init(ZXForm form)
        {
            IntPtr handle = form.Handle;

            if (dinput == null)
                dinput = new DirectInput();

            if (keyboard == null || keyboard.IsDisposed)
            {
                keyboard = new Keyboard(dinput);
                keyboard.SetCooperativeLevel(handle, CooperativeLevel.Background | CooperativeLevel.NonExclusive);
                keyboard.Properties.BufferSize = 8;
            }
        }

        static List<KeyEvent> EmptyList = new List<KeyEvent>();
        static List<KeyEvent> EventList = new List<KeyEvent>();

        public static IEnumerable<KeyEvent> Update()
        {
            EventList.Clear();

            /*
            if (keyboard.Acquire().IsFailure)
                return EmptyList;
            if (keyboard.Poll().IsFailure)
                return EmptyList;
                */

            keyboard.Acquire();
            keyboard.Poll();

            for (;;)
            {
                var events = keyboard.GetBufferedData();
                if (events.Length == 0)
                    break;

                foreach (var e in events)
                {

                    if (e.IsPressed)
                        EventList.Add(new KeyEvent { Key = e.Key, Pressed = true });
                    if (e.IsReleased)
                        EventList.Add(new KeyEvent { Key = e.Key, Pressed = false });
                }
            }

            return EventList;
        }
    }
}
