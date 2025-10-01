using System.Drawing.Text;
using System.Linq;
using System.Windows.Forms;

namespace SimpleCounterObs
{
    public partial class ConfigForm : Form
    {
        private readonly CounterState _state;



        // el MainForm inyecta esto para suspender/reanudar el hook
        public Action<bool>? ToggleHotkeys;

        private enum HotkeyTarget { None, Increment, Decrement, Reset }
        private HotkeyTarget _captureTarget = HotkeyTarget.None;
        private string _prevText = "";
        private TextBox? _captureBox; // referencia al textbox que está capturando
        private bool _prevReadOnly = false;
        private bool _prevShortcutsEnabled = true;





        public ConfigForm(CounterState state)
        {
            _state = state;
            InitializeComponent();

            // Botones
            btnOk.Click += (s, e) => { this.DialogResult = DialogResult.OK; Close(); };
            btnCancel.Click += (s, e) => { this.DialogResult = DialogResult.Cancel; Close(); };

            // Hotkeys: al hacer click, entrar en captura
            txtInc.Click += (s, e) => BeginCapture(HotkeyTarget.Increment, txtInc);
            txtDec.Click += (s, e) => BeginCapture(HotkeyTarget.Decrement, txtDec);
            txtReset.Click += (s, e) => BeginCapture(HotkeyTarget.Reset, txtReset);

            // Captura con KeyDown, bloquear escritura con KeyPress
            txtInc.KeyDown += HotkeyBox_KeyDown;
            txtDec.KeyDown += HotkeyBox_KeyDown;
            txtReset.KeyDown += HotkeyBox_KeyDown;

            txtInc.KeyPress += HotkeyBox_KeyPress;
            txtDec.KeyPress += HotkeyBox_KeyPress;
            txtReset.KeyPress += HotkeyBox_KeyPress;

            LoadFonts();
            LoadFromState();
        }


        private void LoadFonts()
        {
            using var ifc = new System.Drawing.Text.InstalledFontCollection();
            var families = ifc.Families.Select(f => f.Name).OrderBy(n => n).ToArray();
            cboFont.Items.Clear();
            cboFont.Items.AddRange(families);

            // si prefieres permitir escribir a mano:
            // cboFont.DropDownStyle = ComboBoxStyle.DropDown;
        }

        private void LoadFromState()
        {
            txtInc.Text = _state.Hotkeys.Increment;
            txtDec.Text = _state.Hotkeys.Decrement;
            txtReset.Text = _state.Hotkeys.Reset;

            // Seleccionar fuente en el combo (si está en la lista)
            var idx = cboFont.Items.IndexOf(_state.Style.FontFamily);
            if (idx >= 0) cboFont.SelectedIndex = idx;
            else
            {
                // si no está listada, coloca el texto igualmente
                cboFont.Text = _state.Style.FontFamily ?? "Segoe UI";
            }

            nudSize.Value = Math.Max(nudSize.Minimum, Math.Min(nudSize.Maximum, _state.Style.FontSize));
            chkMinimal.Checked = _state.Style.Minimal;
            chkShowTitle.Checked = _state.Style.ShowTitle;
        }

        public bool ApplyChanges()
        {
            bool changed = false;

            // Hotkeys
            string inc = txtInc.Text.Trim();
            string dec = txtDec.Text.Trim();
            string res = txtReset.Text.Trim();

            if (_state.Hotkeys.Increment != inc) { _state.Hotkeys.Increment = inc; changed = true; }
            if (_state.Hotkeys.Decrement != dec) { _state.Hotkeys.Decrement = dec; changed = true; }
            if (_state.Hotkeys.Reset != res) { _state.Hotkeys.Reset = res; changed = true; }

            // Style
            // si usas DropDownList, SelectedItem; si permites texto libre, usa Text
            string fontName = cboFont.SelectedItem?.ToString() ?? cboFont.Text?.Trim() ?? "Segoe UI";
            int fontSize = (int)nudSize.Value;
            bool minimal = chkMinimal.Checked;
            bool showTitle = chkShowTitle.Checked;

            if (_state.Style.FontFamily != fontName) { _state.Style.FontFamily = fontName; changed = true; }
            if (_state.Style.FontSize != fontSize) { _state.Style.FontSize = fontSize; changed = true; }
            if (_state.Style.Minimal != minimal) { _state.Style.Minimal = minimal; changed = true; }
            if (_state.Style.ShowTitle != showTitle) { _state.Style.ShowTitle = showTitle; changed = true; }

            return changed;
        }

        // ---- captura ----
        private void BeginCapture(HotkeyTarget target, TextBox box)
        {
            _captureTarget = target;
            _captureBox = box;

            _prevText = box.Text;
            _prevReadOnly = box.ReadOnly;
            _prevShortcutsEnabled = box.ShortcutsEnabled;

            box.ReadOnly = true;             // evita que “escriba”
            box.ShortcutsEnabled = false;    // evita Ctrl+V, etc.
            box.ImeMode = ImeMode.Disable;   // opcional

            box.Text = "Presione la tecla… (ESC para cancelar)";
            box.SelectAll();
            box.Focus();

            // si cambia foco a otro control → cancelar
            box.Leave += CaptureBox_Leave;

            // si clicas en el “fondo” → cancelar
            HookClickAwayHandlers();

            // no dispares macros mientras capturas
            ToggleHotkeys?.Invoke(false);
        }

        private void CancelCapture()
        {
            if (_captureTarget == HotkeyTarget.None || _captureBox == null) return;

            _captureBox.Text = _prevText;
            _captureBox.ReadOnly = _prevReadOnly;
            _captureBox.ShortcutsEnabled = _prevShortcutsEnabled;

            _captureBox.Leave -= CaptureBox_Leave;
            UnhookClickAwayHandlers();
            _captureBox = null;

            _captureTarget = HotkeyTarget.None;
            ToggleHotkeys?.Invoke(true);
        }

        private void FinishCapture()
        {
            if (_captureBox == null) return;

            _captureBox.ReadOnly = _prevReadOnly;
            _captureBox.ShortcutsEnabled = _prevShortcutsEnabled;

            _captureBox.Leave -= CaptureBox_Leave;
            UnhookClickAwayHandlers();
            _captureBox = null;

            _captureTarget = HotkeyTarget.None;
            ToggleHotkeys?.Invoke(true);
        }

        private void CaptureBox_Leave(object? sender, EventArgs e) => CancelCapture();

        private void HookClickAwayHandlers()
        {
            this.MouseDown += BackgroundMouseDown;
            tabs.MouseDown += BackgroundMouseDown;
            tabHotkeys.MouseDown += BackgroundMouseDown;
            tabStyle.MouseDown += BackgroundMouseDown;
            // si tienes Panel/GroupBox dentro de tabs, engánchalos aquí también
        }
        private void UnhookClickAwayHandlers()
        {
            this.MouseDown -= BackgroundMouseDown;
            tabs.MouseDown -= BackgroundMouseDown;
            tabHotkeys.MouseDown -= BackgroundMouseDown;
            tabStyle.MouseDown -= BackgroundMouseDown;
        }
        private void BackgroundMouseDown(object? sender, MouseEventArgs e)
        {
            if (_captureTarget != HotkeyTarget.None) CancelCapture();
        }


        public void HotkeyBox_KeyDown(object? sender, KeyEventArgs e)
        {
            if (_captureTarget == HotkeyTarget.None) return;
            var box = (TextBox)sender!;
            e.SuppressKeyPress = true;

            if (e.KeyCode == Keys.Escape)
            {
                box.Text = _prevText;
                _captureTarget = HotkeyTarget.None;
                ToggleHotkeys?.Invoke(true);
                box.FindForm()?.Focus(); // 👈 pierde el foco
                return;
            }

            var parts = new List<string>();
            if (e.Control) parts.Add("Ctrl");
            if (e.Alt) parts.Add("Alt");
            if (e.Shift) parts.Add("Shift");

            var key = e.KeyCode;
            if (key is Keys.ControlKey or Keys.ShiftKey or Keys.Menu or Keys.LWin or Keys.RWin)
                return;

            parts.Add(KeyNameHelper.PrettyFromKeys(key));
            box.Text = string.Join("+", parts);

            _captureTarget = HotkeyTarget.None;
            ToggleHotkeys?.Invoke(true);

            // 👇 perder el foco para evitar repetición de letras
            box.FindForm()?.Focus();
        }


        private void HotkeyBox_KeyPress(object? sender, KeyPressEventArgs e)
        {
            // mientras capturas, no dejes que “escriba” caracteres
            if (_captureTarget != HotkeyTarget.None && sender == _captureBox)
                e.Handled = true;
        }

        


        private static string BuildCombo(KeyEventArgs e)
        {
            var parts = new System.Collections.Generic.List<string>();
            if (e.Control) parts.Add("Ctrl");
            if (e.Alt) parts.Add("Alt");
            if (e.Shift) parts.Add("Shift");
            // Para Win: if ((e.Modifiers & Keys.LWin) == Keys.LWin || (e.Modifiers & Keys.RWin) == Keys.RWin) parts.Add("Win");

            var key = e.KeyCode;

            // si solo presionó modificadores, seguimos esperando tecla final
            if (key is Keys.ControlKey or Keys.ShiftKey or Keys.Menu or Keys.LWin or Keys.RWin)
                return "…";

            string keyName;
            if (key >= Keys.F1 && key <= Keys.F24) keyName = key.ToString();
            else
            {
                keyName = key.ToString(); // “A”, “W”, “D1”, “OemMinus”, etc.
            }

            parts.Add(keyName);
            return string.Join("+", parts);
        }
    }
}
