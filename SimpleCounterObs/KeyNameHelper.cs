using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace SimpleCounterObs
{
    public static class KeyNameHelper
    {
        private const uint MAPVK_VK_TO_VSC_EX = 4;
        private const int KF_EXTENDED = 0x0100;
        private const uint SC_PRINTSCREEN = 0xE037;
        private const uint SC_PAUSE = 0xE11D;
        private const uint SC_PAUSE__LEGACY = 0x0045;

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern uint MapVirtualKeyW(uint uCode, uint uMapType);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern int GetKeyNameTextW(int lParam, string lpBuffer, int nSize);

        public static string PrettyFromKeys(Keys key)
        {
            // Casos rápidos típicos:
            string s = key.ToString();
            if (s.Length == 2 && s[0] == 'D' && char.IsDigit(s[1])) return s[1].ToString(); // D1 → "1"
            if (s.StartsWith("NumPad", StringComparison.OrdinalIgnoreCase)) return "Num" + s.Substring(6); // NumPad1 → "Num1"
            if (s.Equals("OemMinus", StringComparison.OrdinalIgnoreCase)) return "-";
            if (s.Equals("Oemplus", StringComparison.OrdinalIgnoreCase)) return "=";

            // Fallback “pro” con Win32: MapVirtualKey + GetKeyNameText
            uint vk = (uint)key;
            // Keys.D0..D9 comparten VK 0x30..0x39; Keys.A..Z comparten 0x41..0x5A; funciona ok.
            uint extendedScanCode = MapVirtualKeyW(vk, MAPVK_VK_TO_VSC_EX);
            uint scanCode = (0xFF & extendedScanCode);

            // Marcar extendido si corresponde / casos especiales
            if (((extendedScanCode >> 8) & 0xFE) == 0xE0)
            {
                if (extendedScanCode == SC_PAUSE) scanCode = SC_PAUSE__LEGACY;
                else scanCode |= (uint)KF_EXTENDED;
            }
            else
            {
                switch (vk)
                {
                    case 0x2C: // PRINT SCREEN
                        scanCode = (uint)KF_EXTENDED | (SC_PRINTSCREEN & 0xFF);
                        break;
                    case 0x90: // NUMLOCK
                    case 0x21: // PRIOR (PageUp)
                    case 0x22: // NEXT (PageDown)
                    case 0x23: // END
                    case 0x24: // HOME
                    case 0x25: // LEFT
                    case 0x26: // UP
                    case 0x27: // RIGHT
                    case 0x28: // DOWN
                    case 0x2D: // INSERT
                    case 0x2E: // DELETE
                        scanCode |= (uint)KF_EXTENDED;
                        break;
                }
            }

            int lParam = (int)scanCode << 16;
            string buf = new('\0', 64);
            int len = GetKeyNameTextW(lParam, buf, buf.Length);
            if (len > 0) return buf[..len];

            // Si no se pudo, regresa el ToString() original
            return s;
        }
    }
}

