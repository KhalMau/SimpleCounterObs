using static System.Windows.Forms.AxHost;

using System;
using System.IO;
using System.Windows.Forms;


namespace SimpleCounterObs
{
    public partial class Form1 : Form
    {
        private readonly ConfigService _config = new();
        private CounterState _state;
        private HotkeyManager? _hotkeys;

        public Form1()
        {
            InitializeComponent();
            _state = _config.Load();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            txtTitle.Text = _state.Title;
            lblValue.Text = _state.Value.ToString();

            txtOverlay.Text = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Designs", "overlay.html");

            BumpAndSave();

            _hotkeys = new HotkeyManager();
            _hotkeys.OnHotkey += HandleHotkey;
            _hotkeys.Register(_state.Hotkeys, avoidRepeat: true);
        }

        private void HandleHotkey(string action)
        {
            if (action == "increment") BtnInc();
            else if (action == "decrement") BtnDec();
            else if (action == "reset") BtnReset();
        }

        private void txtTitle_TextChanged(object sender, EventArgs e)
        {
            _state.Title = string.IsNullOrWhiteSpace(txtTitle.Text) ? "Contador" : txtTitle.Text.Trim();
            BumpAndSave();
        }

        private void btnInc_Click(object sender, EventArgs e) => BtnInc();
        private void btnDec_Click(object sender, EventArgs e) => BtnDec();
        private void btnReset_Click(object sender, EventArgs e) => BtnReset();

        private void BtnInc() { _state.Value++; lblValue.Text = _state.Value.ToString(); BumpAndSave(); }
        private void BtnDec() { _state.Value--; lblValue.Text = _state.Value.ToString(); BumpAndSave(); }
        private void BtnReset() { _state.Value = 0; lblValue.Text = _state.Value.ToString(); BumpAndSave(); }

        private void btnCfg_Click(object sender, EventArgs e)
        {
            using var dlg = new ConfigForm(_state);
            dlg.ToggleHotkeys = (enable) => { if (enable) _hotkeys?.Resume(); else _hotkeys?.Suspend(); };

            if (dlg.ShowDialog(this) == DialogResult.OK && dlg.ApplyChanges())
            {
                _hotkeys?.Register(_state.Hotkeys, avoidRepeat: true); // o false si quieres autorepetición
                BumpAndSave(); // sube Version y guarda overlay-data.js
            }

        }
        public void btnCopy_Click(object sender, EventArgs e) { Clipboard.SetText(txtOverlay.Text); }

        private void BumpAndSave()
        {
            //_state.Version++;
            _config.Save(_state); // escribe overlay-data.js
        }
    }
}
