using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace SimpleCounterObs
{
    /// <summary>
    /// Hotkeys globales por hook WH_KEYBOARD_LL (no bloquea las teclas).
    /// - API pública: Register(HotkeyConfig), Suspend/Resume, evento OnHotkey("increment"|"decrement"|"reset")
    /// - Detección de borde (edge): una acción por pulsación (sin repetir por autorepetición)
    /// </summary>
    public class HotkeyManager : IDisposable
    {
        // --- Win32 consts ---
        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0100;
        private const int WM_SYSKEYDOWN = 0x0104;

        // Máscara de modificadores (coincide con lo que ya usabas)
        private const uint MOD_ALT = 0x0001;
        private const uint MOD_CTRL = 0x0002;
        private const uint MOD_SHIFT = 0x0004;
        private const uint MOD_WIN = 0x0008;

        // --- Hook state ---
        private IntPtr _hookId = IntPtr.Zero;
        private LowLevelKeyboardProc _proc;
        private bool _suspended = false;

        // --- Combos preparseados (increment/decrement/reset) ---
        private ComboKey _inc = new();
        private ComboKey _dec = new();
        private ComboKey _res = new();

        // Si quieres permitir autorepetición manteniendo la tecla, pon esto en false con Register(..., edgeOnly:false)
        private bool _edgeOnly = true;

        public event Action<string>? OnHotkey;

        public HotkeyManager()
        {
            _proc = HookCallback;
            _hookId = SetHook(_proc);
        }

        /// <summary>
        /// Registra/actualiza las combinaciones desde tu configuración.
        /// edgeOnly=true => solo dispara en el "primer down" (sin auto-repeat)
        /// edgeOnly=false => disparará también con el autorepeat del SO
        /// </summary>
        public void Register(HotkeyConfig cfg, bool avoidRepeat = true)
        {
            _edgeOnly = avoidRepeat;

            _inc.SetFromString(cfg?.Increment);
            _dec.SetFromString(cfg?.Decrement);
            _res.SetFromString(cfg?.Reset);
        }

        public void Suspend() => _suspended = true;
        public void Resume() => _suspended = false;

        public void Dispose()
        {
            if (_hookId != IntPtr.Zero)
            {
                UnhookWindowsHookEx(_hookId);
                _hookId = IntPtr.Zero;
            }
        }

        // dentro de HotkeyManager
        public void SetEdgeMode(bool edgeOnly)
        {
            _edgeOnly = edgeOnly;
        }


        // -------------------- Hook --------------------
        private IntPtr SetHook(LowLevelKeyboardProc proc)
        {
            using var curProcess = Process.GetCurrentProcess();
            using var curModule = curProcess.MainModule!;
            // OJO: GetModuleHandle es de kernel32.dll
            return SetWindowsHookEx(WH_KEYBOARD_LL, proc, GetModuleHandle(curModule.ModuleName), 0);
        }

        private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            try
            {
                if (nCode >= 0 && !_suspended &&
                    (wParam == (IntPtr)WM_KEYDOWN || wParam == (IntPtr)WM_SYSKEYDOWN))
                {
                    var kb = Marshal.PtrToStructure<KBDLLHOOKSTRUCT>(lParam);
                    int vk = (int)kb.vkCode;

                    // Modificadores actuales
                    bool ctrl = IsDown(Keys.ControlKey);
                    bool alt = IsDown(Keys.Menu);
                    bool shift = IsDown(Keys.ShiftKey);
                    bool win = IsDown(Keys.LWin) || IsDown(Keys.RWin);

                    // Intentos en orden
                    if (_inc.TryFire(vk, ctrl, alt, shift, win, _edgeOnly))
                        OnHotkey?.Invoke("increment");
                    else if (_dec.TryFire(vk, ctrl, alt, shift, win, _edgeOnly))
                        OnHotkey?.Invoke("decrement");
                    else if (_res.TryFire(vk, ctrl, alt, shift, win, _edgeOnly))
                        OnHotkey?.Invoke("reset");
                }
            }
            catch
            {
                // swallow: no queremos romper el hook si algo falla
            }

            // CLAVE: NO bloqueamos la tecla → pasa a otras apps
            return CallNextHookEx(IntPtr.Zero, nCode, wParam, lParam);
        }

        private static bool IsDown(Keys key)
        {
            short s = GetKeyState((int)key);
            return (s & 0x8000) != 0;
        }

        // -------------------- ComboKey --------------------
        /// <summary>
        /// Representa una hotkey preparseada (vk + modificadores).
        /// Implementa "detección de borde": dispara una vez por pulsación y se rearma al soltar.
        /// </summary>
        private sealed class ComboKey
        {
            public string Raw = "";
            public uint Vk = 0;         // VK_* sin modificadores
            public uint Mods = 0;       // bits MOD_* (ALT/CTRL/SHIFT/WIN)
            private bool _armed = true; // true: listo para disparar cuando coincida

            /// <summary>
            /// Setea desde cadena tipo "Ctrl+Alt+F9", "W", "-", "OemMinus"…
            /// </summary>
            public void SetFromString(string? combo)
            {
                Raw = combo?.Trim() ?? "";
                (Vk, Mods) = ParseCombo(Raw);
                _armed = true; // cada vez que cambie la hotkey, rearmamos
            }

            /// <summary>
            /// Devuelve true SOLO en el "flanco de bajada" (primer keydown coincidente)
            /// Si edgeOnly=false, devuelve true para todos los keydown coincidentes (incluye autorepeat)
            /// </summary>
            public bool TryFire(int vk, bool ctrl, bool alt, bool shift, bool win, bool edgeOnly)
            {
                if (Vk == 0) return false;

                uint modsActual = 0;
                if (alt) modsActual |= MOD_ALT;
                if (ctrl) modsActual |= MOD_CTRL;
                if (shift) modsActual |= MOD_SHIFT;
                if (win) modsActual |= MOD_WIN;

                bool active = (vk == Vk) && (modsActual == Mods);

                if (!edgeOnly)
                {
                    // Sin detección de borde: siempre que active, dispara
                    return active;
                }

                // Con detección de borde:
                if (active)
                {
                    if (_armed)
                    {
                        _armed = false; // desarma hasta soltar
                        return true;
                    }
                    return false; // repetición mientras mantenés la tecla
                }
                else
                {
                    // combinación no activa → rearmar
                    _armed = true;
                    return false;
                }
            }

            // ----- parsing -----
            private static (uint vk, uint mods) ParseCombo(string? combo)
            {
                if (string.IsNullOrWhiteSpace(combo)) return (0, 0);

                uint vk = 0;
                uint mods = 0;

                var parts = combo.Split('+', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                foreach (var p in parts)
                {
                    var t = p.ToUpperInvariant();
                    if (t is "CTRL" or "CONTROL") mods |= MOD_CTRL;
                    else if (t is "ALT") mods |= MOD_ALT;
                    else if (t is "SHIFT") mods |= MOD_SHIFT;
                    else if (t is "WIN" or "LWIN" or "RWIN" or "WINDOWS") mods |= MOD_WIN;
                    else if (t.StartsWith("F") && int.TryParse(t[1..], out int n) && n is >= 1 and <= 24)
                        vk = (uint)(0x70 + (n - 1)); // VK_F1=0x70
                    else
                        vk = MapKeyTokenToVk(t);
                }

                return (vk, mods);
            }

            private static uint MapKeyTokenToVk(string token)
            {
                // Mapea algunos nombres comunes y símbolos. Amplía según necesites.
                return token switch
                {
                    "UP" => 0x26,
                    "DOWN" => 0x28,
                    "LEFT" => 0x25,
                    "RIGHT" => 0x27,
                    "ADD" => 0x6B,
                    "SUBTRACT" => 0x6D,
                    "MULTIPLY" => 0x6A,
                    "DIVIDE" => 0x6F,
                    "SPACE" => 0x20,
                    "ENTER" => 0x0D,
                    "ESC" => 0x1B,
                    "OEMMINUS" or "-" => 0xBD,  // VK_OEM_MINUS
                    "OEMPLUS" or "=" => 0xBB,  // VK_OEM_PLUS
                    _ => token.Length == 1 ? (uint)char.ToUpperInvariant(token[0]) : 0
                };
            }
        }

        // -------------------- WinAPI --------------------
        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

        [StructLayout(LayoutKind.Sequential)]
        private struct KBDLLHOOKSTRUCT
        {
            public uint vkCode;
            public uint scanCode;
            public uint flags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        [DllImport("user32.dll")] private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);
        [DllImport("user32.dll")] private static extern bool UnhookWindowsHookEx(IntPtr hhk);
        [DllImport("user32.dll")] private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        // ¡Correcto!: GetModuleHandle es de kernel32.dll
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string? lpModuleName);

        [DllImport("user32.dll")] private static extern short GetKeyState(int nVirtKey);
    }
}
