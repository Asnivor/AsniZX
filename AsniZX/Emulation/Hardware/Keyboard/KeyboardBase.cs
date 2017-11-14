using AsniZX.Emulation.Hardware.Machine;
using AsniZX.Emulation.Interfaces;
using AsniZX.Emulation.Interfaces.Devices;
using AsniZX.SubSystem.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsniZX.Emulation.Hardware.Keyboard
{
    public abstract class KeyboardBase : IZXBoundDevice, IDevice
    {
        /// <summary>
        /// Instance of the DirectInput KeyboardHandler
        /// </summary>
        public KeyboardHandler _keyHandler = KeyboardHandler.Instance;

        /// <summary>
        /// The key mapping used for this model of spectrum
        /// </summary>
        public IKeyMapping KeyMapping;


        public byte[] _lineStatus = new byte[8];

        /// <summary>
        /// Holds the current KeyQueue
        /// </summary>
        protected List<KeyEvent> KeyQueue = new List<KeyEvent>();


        protected List<KeyEvent> toPurge = new List<KeyEvent>();

        /// <summary>
        /// Processes the currently pressed and unpressed keys
        /// </summary>
        public virtual void ProcessKeyboardInput()
        {
            // get the latest keyboard input events
            var keyResult = KeyInput.Update(); // KeyboardHandler.Instance._NewEvents;
            if (keyResult.Count() == 0)
            {
                // no key events detected
                return;
            }
            lock (this)
            {
                foreach (var k in keyResult)
                {
                    // add to the KeyQueue
                    KeyQueue.Add(k);

                    // remove duplicates
                    KeyQueue = KeyQueue.Distinct().ToList();
                }
               
                SetKeys();
            }

            // pass the pressed keys to the spectrum
            //PressKeys();
        }

        /// <summary>
        /// Removes old events from the KeyQueue
        /// whilst sending keypresses to the spectrum
        /// </summary>
        protected virtual void SetKeys()
        {
            throw new NotImplementedException();
        }

        public virtual void PressKeys()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns the byte signifying which keys are pressed
        /// </summary>
        /// <param name="addr"></param>
        /// <returns></returns>
        public virtual byte ReadKeyboardByte(ushort addr)
        {
            throw new NotImplementedException();
        }

        



        #region Construction

        #endregion

        #region IZXBoundDevice

        public ZXBase HostZX { get; set; }
        public virtual void OnAttached(ZXBase hostZX)
        {
            HostZX = hostZX;
        }

        #endregion

        #region IDevice

        /// <summary>
        /// Resets this device
        /// </summary>
        public void Reset()
        {
        }

        #endregion
    }
}
