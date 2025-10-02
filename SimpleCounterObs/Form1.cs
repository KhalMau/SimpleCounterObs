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
        private NotifyIcon _tray;
        private ContextMenuStrip _trayMenu;
        //private bool _trayTipShownOnce = false; //pop up notify

        public Form1()
        {
            InitializeComponent();
            _state = _config.Load();

            
            _trayMenu = new ContextMenuStrip();
            _trayMenu.Items.Add("Mostrar", null, (s, e) => RestoreFromTray());
            _trayMenu.Items.Add(new ToolStripSeparator());
            _trayMenu.Items.Add("Incrementar", null, (s, e) => btnInc_Click(s, e));
            _trayMenu.Items.Add("Decrementar", null, (s, e) => btnDec_Click(s, e));
            _trayMenu.Items.Add("Resetear", null, (s, e) => btnReset_Click(s, e));
            _trayMenu.Items.Add(new ToolStripSeparator());
            _trayMenu.Items.Add("Salir", null, (s, e) => ExitApplication());

            
            _tray = new NotifyIcon
            {
                Text = "SimpleCounterObs",
                Visible = false,                  
                ContextMenuStrip = _trayMenu
            };

            // Carga del icono (elige una de las dos opciones)
            string icoPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "app.ico");
            if (File.Exists(icoPath))
            {
                _tray.Icon = new Icon(icoPath);  
            }
            else
            {
                // fallback a icono del formulario si no hay .ico
                _tray.Icon = this.Icon ?? SystemIcons.Application;
            }

            // Doble clic restaura
            _tray.MouseDoubleClick += (s, e) => { if (e is MouseEventArgs me && me.Button == MouseButtons.Left) RestoreFromTray(); };

            // Maneja minimizar
            this.Resize += MainForm_Resize;

            // (opcional) si quieres “cerrar” con la X, que vaya a tray:
            

        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            txtTitle.Text = _state.Title;
            lblValue.Text = _state.Value.ToString();

            txtOverlay.Text = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Designs", "overlay.html");

            BumpAndSave();

            _hotkeys = new HotkeyManager();
            _hotkeys.OnHotkey += HandleHotkey;
            _hotkeys.Register(_state.Hotkeys, avoidRepeat: false); // ← autorepeat
            _hotkeys.SetEdgeMode(false); // ← explícito: modo continuo
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

            // mientras el usuario captura una tecla, suspende el hook global (evita acciones accidentales)
            dlg.ToggleHotkeys = (enable) =>
            {
                if (enable) _hotkeys?.Resume();
                else _hotkeys?.Suspend();
            };

            // ← Aquí forzamos modo “WasPressed” mientras está abierto el config
            _hotkeys?.SetEdgeMode(true);   // NO repetir (WasPressed)

            var dr = dlg.ShowDialog(this);

            // al cerrar, volvemos a modo continuo para Form1
            _hotkeys?.SetEdgeMode(false);  // repetir (autorepeat)

            if (dr == DialogResult.OK && dlg.ApplyChanges())
            {
                _hotkeys?.Register(_state.Hotkeys, avoidRepeat: false); // mantener autorepeat activo para Form1
                BumpAndSave();
            }
        }

        public void btnCopy_Click(object sender, EventArgs e) { Clipboard.SetText(txtOverlay.Text); }

        private void BumpAndSave()
        {
            //_state.Version++;
            _config.Save(_state); // escribe overlay-data.js
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            // Si haces clic en el fondo del formulario (no sobre otro control)
            this.ActiveControl = null;
        }


        private void MainForm_Resize(object? sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                GoToTray();
            }
        }

        

        private void GoToTray()
        {
            // Oculta la ventana y saca el icono
            _tray.Visible = true;
            this.ShowInTaskbar = false;
            this.Hide();

            /*if (!_trayTipShownOnce)
            {
                _trayTipShownOnce = true;
                try
                {
                    _tray.BalloonTipTitle = "SimpleCounterObs";
                    _tray.BalloonTipText = "Still working here!.";
                    _tray.ShowBalloonTip(2000);
                }
                catch { }
            }*/
        }

        private void RestoreFromTray()
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;
            this.ShowInTaskbar = true;
            this.Activate();
            _tray.Visible = false;
        }

        private void ExitApplication()
        {
            // Cierra realmente la app (útil desde el menú del tray)
            _tray.Visible = false;
            _tray.Dispose();
            Application.Exit();
        }

    }
}
