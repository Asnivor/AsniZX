using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SharpDX.DirectInput;
using AsniZX.Emulation.Interfaces.Devices;

namespace AsniZX.Emulation.Hardware.Keyboard
{
    public class ZXSpectrum48_Keys : IKeyMapping
    {
        public KeyboardBase _keyboardBase;

        public ZXSpectrum48_Keys(KeyboardBase keyBase)
        {
            _keyboardBase = keyBase;
        }

        /// <summary>
        /// spectrum keyboard mapping
        /// </summary>
        public enum KeyCode
        {
            // 0xfefe
            CAPSHIFT = 0, Z = 1, X = 2, C = 3, V = 4,

            // 0xfdfe
            A = 5, S = 6, D = 7, F = 8, G = 9,

            // 0xfbfe
            Q = 10,  W = 11, E = 12,  R = 13, T = 14,

            // 0xf7fe
            D1 = 15, D2 = 16, D3 = 17, D4 = 18, D5 = 19,

            // 0xeffe
            D0 = 20, D9 = 21, D8 = 22, D7 = 23, D6 = 24,

            // 0xdffe
            P = 25, O = 26, I = 27, U = 28, Y = 29,

            // 0xbffe
            ENTER = 30, L = 31, K = 32, J = 33, H = 34,

            // 0x7ffe
            SPACE = 35, SYMSHIFT = 36, M = 37, N = 38, B = 39
        }

        /// <summary>
        /// Map DirectInput key codes to spectrum keys
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static KeyCode DX2Spec(SharpDX.DirectInput.Key key)
        {
            KeyCode r;
            bool t = System.Enum.TryParse(key.ToString(), true, out r);

            if (!t)
            {
                // handle the keys that were not parsed
                switch (key)
                {
                    case Key.LeftControl: r = KeyCode.CAPSHIFT; break;
                    case Key.Return: r = KeyCode.ENTER; break;
                    case Key.RightControl: r = KeyCode.SYMSHIFT; break;
                }
            }

            return r;
        }

        /// <summary>
        /// Sets the spectrum key status
        /// </summary>
        /// <param name="key"></param>
        /// <param name="isPressed"></param>
        public void SetStatus(KeyCode key, bool isPressed)
        {
            var lineIndex = (byte)key / 5;
            var lineMask = 1 << (byte)key % 5;
            _keyboardBase._lineStatus[lineIndex] = isPressed ? (byte)(_keyboardBase._lineStatus[lineIndex] | lineMask)
                : (byte)(_keyboardBase._lineStatus[lineIndex] & ~lineMask);
        }

        /// <summary>
        /// Gets the status of a spectrum key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool GetKeyStatus(KeyCode key)
        {
            var lineIndex = (byte)key / 5;
            var lineMask = 1 << (byte)key % 5;
            return (_keyboardBase._lineStatus[lineIndex] & lineMask) != 0;
        }

        /// <summary>
        /// Returns the query byte
        /// </summary>
        /// <param name="lines"></param>
        /// <returns></returns>
        public byte GetLineStatus(byte lines)
        {
            byte status = 0;
            lines = (byte)~lines;

            var lineIndex = 0;
            while (lines > 0)
            {
                if ((lines & 0x01) != 0)
                {
                    status |= _keyboardBase._lineStatus[lineIndex];
                }
                lineIndex++;
                lines >>= 1;
            }
            return (byte)~status;
        }
    }

    
}
