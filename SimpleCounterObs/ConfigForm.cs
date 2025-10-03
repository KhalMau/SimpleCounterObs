using System.Drawing.Text;

namespace SimpleCounterObs
{
    public partial class ConfigForm : Form
    {
        private readonly CounterState _state;

        // Inyectado desde el MainForm para suspender/reanudar el hook global
        public Action<bool>? ToggleHotkeys;

        private enum HotkeyTarget { None, Increment, Decrement, Reset }

        private HotkeyTarget _captureTarget = HotkeyTarget.None;
        private TextBox? _captureBox;        // textbox que está capturando
        private string _prevText = "";       // texto previo para restaurar en cancelar

        public ConfigForm(CounterState state)
        {
            _state = state;
            InitializeComponent();

            // Botones
            btnOk.Click += (_, __) => { DialogResult = DialogResult.OK; Close(); };
            btnCancel.Click += (_, __) => { DialogResult = DialogResult.Cancel; Close(); };

            // Entrar a captura al hacer clic
            txtInc.Click += (_, __) => BeginCapture(HotkeyTarget.Increment, txtInc);
            txtDec.Click += (_, __) => BeginCapture(HotkeyTarget.Decrement, txtDec);
            txtReset.Click += (_, __) => BeginCapture(HotkeyTarget.Reset, txtReset);

            // Captura por teclado y bloqueo de escritura
            txtInc.KeyDown += HotkeyBox_KeyDown;
            txtDec.KeyDown += HotkeyBox_KeyDown;
            txtReset.KeyDown += HotkeyBox_KeyDown;

            txtInc.KeyPress += HotkeyBox_KeyPress;
            txtDec.KeyPress += HotkeyBox_KeyPress;
            txtReset.KeyPress += HotkeyBox_KeyPress;

            // Asegura que los textboxes no acepten edición directa (por si no lo dejaste en el diseñador)
            ConfigureHotkeyTextBox(txtInc);
            ConfigureHotkeyTextBox(txtDec);
            ConfigureHotkeyTextBox(txtReset);

            LoadFonts();
            LoadFromState();
        }

        private void ConfigureHotkeyTextBox(TextBox box)
        {
            box.ReadOnly = true;          // sin edición directa
            box.ShortcutsEnabled = false; // sin Ctrl+V, etc.
            box.ImeMode = ImeMode.Disable;
            box.Cursor = Cursors.Hand;    // indicativo visual
        }

        // ------------------------
        // Carga de UI
        // ------------------------
        private void LoadFonts()
        {
            using var ifc = new InstalledFontCollection();
            var families = ifc.Families.Select(f => f.Name).OrderBy(n => n).ToArray();

            cboFont.Items.Clear();
            cboFont.Items.AddRange(families);
        }

        private void LoadFromState()
        {
            txtInc.Text = _state.Hotkeys.Increment;
            txtDec.Text = _state.Hotkeys.Decrement;
            txtReset.Text = _state.Hotkeys.Reset;

            var idx = cboFont.Items.IndexOf(_state.Style.FontFamily);
            if (idx >= 0) cboFont.SelectedIndex = idx;
            else cboFont.Text = _state.Style.FontFamily ?? "Segoe UI";

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

            // Estilo
            string fontName = cboFont.SelectedItem?.ToString() ?? (cboFont.Text?.Trim() ?? "Segoe UI");
            int fontSize = (int)nudSize.Value;
            bool minimal = chkMinimal.Checked;
            bool showTitle = chkShowTitle.Checked;

            if (_state.Style.FontFamily != fontName) { _state.Style.FontFamily = fontName; changed = true; }
            if (_state.Style.FontSize != fontSize) { _state.Style.FontSize = fontSize; changed = true; }
            if (_state.Style.Minimal != minimal) { _state.Style.Minimal = minimal; changed = true; }
            if (_state.Style.ShowTitle != showTitle) { _state.Style.ShowTitle = showTitle; changed = true; }

            return changed;
        }

        // ------------------------
        // Captura de hotkeys (simplificada)
        // ------------------------
        private void BeginCapture(HotkeyTarget target, TextBox box)
        {
            // Si ya hay otra captura activa, cancélala primero
            if (_captureTarget != HotkeyTarget.None)
                CancelCapture();

            _captureTarget = target;
            _captureBox = box;

            _prevText = box.Text;
            box.Text = "Press key… (ESC to cancel)";
            box.SelectAll();
            box.Focus();

            // Cancelar si pierde foco o clicas fuera
            box.Leave += CaptureBox_Leave;
            ToggleClickAwayHandlers(true);

            // Pausar hooks globales durante la captura
            ToggleHotkeys?.Invoke(false);
        }

        private void EndCapture(bool commit)
        {
            if (_captureBox == null) return;

            if (!commit)
                _captureBox.Text = _prevText;

            _captureBox.Leave -= CaptureBox_Leave;
            ToggleClickAwayHandlers(false);

            _captureBox = null;
            _captureTarget = HotkeyTarget.None;

            ToggleHotkeys?.Invoke(true);
        }

        private void CancelCapture() => EndCapture(commit: false);
        private void FinishCapture() => EndCapture(commit: true);

        private void CaptureBox_Leave(object? sender, EventArgs e) => CancelCapture();

        private void ToggleClickAwayHandlers(bool on)
        {
            if (on)
            {
                this.MouseDown += BackgroundMouseDown;
                tabs.MouseDown += BackgroundMouseDown;
                tabHotkeys.MouseDown += BackgroundMouseDown;
                tabStyle.MouseDown += BackgroundMouseDown;
            }
            else
            {
                this.MouseDown -= BackgroundMouseDown;
                tabs.MouseDown -= BackgroundMouseDown;
                tabHotkeys.MouseDown -= BackgroundMouseDown;
                tabStyle.MouseDown -= BackgroundMouseDown;
            }
        }

        private void BackgroundMouseDown(object? sender, MouseEventArgs e)
        {
            if (_captureTarget != HotkeyTarget.None)
                CancelCapture();
        }

        // Tecla presionada durante captura
        public void HotkeyBox_KeyDown(object? sender, KeyEventArgs e)
        {
            e.SuppressKeyPress = true; // nunca escribir caracteres

            if (_captureTarget == HotkeyTarget.None || _captureBox == null) return;

            // ESC → cancelar
            if (e.KeyCode == Keys.Escape)
            {
                CancelCapture();
                // Quitar foco para evitar autorepeat
                return;
            }

            // Construir combinación (modificadores + tecla final)
            var parts = new List<string>();
            if (e.Control) parts.Add("Ctrl");
            if (e.Alt) parts.Add("Alt");
            if (e.Shift) parts.Add("Shift");

            var key = e.KeyCode;

            if (key is Keys.ControlKey or Keys.ShiftKey or Keys.Menu or Keys.LWin or Keys.RWin)
                return;

            string keyName = KeyNameHelper.PrettyFromKeys(key);
            if (string.IsNullOrWhiteSpace(keyName))
                keyName = key.ToString();

            _captureBox.Text = string.Join("+", parts.Append(keyName));

            FinishCapture();
        }

        private void HotkeyBox_KeyPress(object? sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            ActiveControl = null;
        }
    }
}
