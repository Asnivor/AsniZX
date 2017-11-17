
using AsniZX.Emulation.Hardware.Keyboard;
using System.Linq;

namespace AsniZX.Emulation.Models.ZXSpectrum48
{
    public partial class ZXSpectrum48
    {
        /// <summary>
        /// Represents the Spectrum keyboard
        /// </summary>
        public class Keyboard : KeyboardBase
        {
            public Keyboard()
            {
                KeyMapping = new ZXSpectrum48_Keys(this);
            }

            public override byte ReadKeyboardByte(ushort addr)
            {
                return ((ZXSpectrum48_Keys)KeyMapping as ZXSpectrum48_Keys).GetLineStatus((byte)(addr >> 8));
            }

            public override void PressKeys()
            {
                var km = (ZXSpectrum48_Keys)KeyMapping as ZXSpectrum48_Keys;
                foreach (var k in KeyQueue)
                {
                    km.SetStatus(ZXSpectrum48_Keys.DX2Spec(k.Key), true);
                }
            }

            /// <summary>
            /// Removes old events from the KeyQueue
            /// whilst sending keypresses to the spectrum
            /// </summary>
            protected override void SetKeys()
            {
                var km = (ZXSpectrum48_Keys)KeyMapping as ZXSpectrum48_Keys;

                KeyQueue = KeyQueue.Distinct().ToList();
                foreach (var k in KeyQueue)
                {
                    if (k.Pressed)
                    {
                        if (km.GetKeyStatus(ZXSpectrum48_Keys.DX2Spec(k.Key)))
                        {
                            // key is already pressed
                        }
                        else
                        {
                            // key is not pressed
                            km.SetStatus(ZXSpectrum48_Keys.DX2Spec(k.Key), true);
                        }
                    }
                    else
                    {
                        if (km.GetKeyStatus(ZXSpectrum48_Keys.DX2Spec(k.Key)))
                        {
                            // key is already pressed
                            km.SetStatus(ZXSpectrum48_Keys.DX2Spec(k.Key), false);
                        }
                        else
                        {
                            // key is not pressed
                        }

                    }
                }


                // Key presses have been processed. Purge everything except the keys that are currently pressed              
                for (int i = 0; i < KeyQueue.Count; i++)
                {
                    var key = KeyQueue[i].Key;
                    bool isPressed = KeyQueue[i].Pressed;

                    if (isPressed)
                    {
                        // seek forward and try and locate an unpress event
                        for (int s = i + 1; s < KeyQueue.Count; s++)
                        {
                            if (KeyQueue[s].Key == key && !KeyQueue[s].Pressed)
                            {
                                // an unpress event has been found after the initial press event
                                // mark the unpress and the original press for removal
                                toPurge.Add(KeyQueue[i]);
                                toPurge.Add(KeyQueue[s]);
                            }
                        }
                    }
                }

                // remove all the unpress events
                foreach (var u in KeyQueue.Where(a => a.Pressed == false))
                {
                    toPurge.Add(u);
                }



                // distinct
                toPurge = toPurge.Distinct().ToList();

                // finally purge the list
                foreach (var p in toPurge)
                {
                    KeyQueue.Remove(p);
                }

                toPurge.Clear();
            }
        }
    }
}
