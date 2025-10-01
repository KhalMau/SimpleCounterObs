using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleCounterObs
{
    public class CounterState
    {
        public string Title { get; set; } = "Contador";
        public int Value { get; set; } = 0;

        // Subir versión cada cambio para que el overlay detecte actualizaciones
       // public long Version { get; set; } = 0;

        public HotkeyConfig Hotkeys { get; set; } = new();
        public StyleConfig Style { get; set; } = new();
    }

    public class HotkeyConfig
    {
        // Ejemplos: "F9", "Ctrl+Alt+Up", "Ctrl+Shift+Subtract"
        public string Increment { get; set; } = "F9";
        public string Decrement { get; set; } = "F10";
        public string Reset { get; set; } = "F11";
    }

    public class StyleConfig
    {
        public string FontFamily { get; set; } = "Segoe UI";
        public int FontSize { get; set; } = 64;
        public bool Minimal { get; set; } = false; // true = solo número (sin fondo/box/título)
        public bool ShowTitle { get; set; } = true;  // por si quieres ocultar también el título
    }
}

