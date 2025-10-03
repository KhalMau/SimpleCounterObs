using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace SimpleCounterObs
{
    public partial class Form1 : Form
    {
        private const string AppName = "SimpleCounterObs";

        private readonly ConfigService _config = new();
        private CounterState _state;

        private HotkeyManager? _hotkeys;

        private readonly NotifyIcon _tray;
        private readonly ContextMenuStrip _trayMenu;

        public Form1()
        {
            InitializeComponent();

            // Carga estado inicial
            _state = _config.Load();

            // --- Menú del tray
            _trayMenu = CreateTrayMenu();

            // --- Icono del tray
            _tray = new NotifyIcon
            {
                Text = AppName,
                Visible = false,
                ContextMenuStrip = _trayMenu
            };

            // Carga icono desde Assets\app.ico o usa el del formulario
            TryLoadTrayIcon();

            // Doble clic izquierdo restaura
            _tray.MouseDoubleClick += (s, e) =>
            {
                if (e is MouseEventArgs me && me.Button == MouseButtons.Left)
                    RestoreFromTray();
            };

            // Minimizar → va al tray
            this.Resize += MainForm_Resize;

            // Cierre con “X” → cierra de verdad. (No se intercepta, solo limpiamos recursos)
            this.FormClosing += (_, __) => _tray.Dispose();
        }

        // Cargado del form (asume que el diseñador llama a este método)
        private void MainForm_Load(object? sender, EventArgs e)
        {
            txtTitle.Text = _state.Title;
            lblValue.Text = _state.Value.ToString();

            // Ruta del overlay (solo se muestra en el textbox para copiar)
            txtOverlay.Text = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Designs", "overlay.html");

            // Persistimos a overlay-data.js
            BumpAndSave();

            // Hotkeys en modo continuo (autorepeat) para el Form1
            _hotkeys = new HotkeyManager();
            _hotkeys.OnHotkey += HandleHotkey;
            _hotkeys.Register(_state.Hotkeys, avoidRepeat: false);
            _hotkeys.SetEdgeMode(false);
        }

        // --- Eventos UI básicos
        private void txtTitle_TextChanged(object? sender, EventArgs e)
        {
            _state.Title = string.IsNullOrWhiteSpace(txtTitle.Text)
                ? "Deaths: "
                : txtTitle.Text.Trim();

            BumpAndSave();
        }

        private void btnInc_Click(object? sender, EventArgs e) => BtnInc();
        private void btnDec_Click(object? sender, EventArgs e) => BtnDec();
        private void btnReset_Click(object? sender, EventArgs e) => BtnReset();

        // Evita que el TextBox quede con foco si haces clic en el fondo del form
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            this.ActiveControl = null;
        }

        // --- Acciones del contador
        private void BtnInc()
        {
            _state.Value++;
            lblValue.Text = _state.Value.ToString();
            BumpAndSave();
        }

        private void BtnDec()
        {
            _state.Value--;
            lblValue.Text = _state.Value.ToString();
            BumpAndSave();
        }

        private void BtnReset()
        {
            _state.Value = 0;
            lblValue.Text = _state.Value.ToString();
            BumpAndSave();
        }

        // --- Configuración
        private void btnCfg_Click(object? sender, EventArgs e)
        {
            using var dlg = new ConfigForm(_state);

            // Mientras se captura una tecla en ConfigForm, se suspende el hook global
            dlg.ToggleHotkeys = (enable) =>
            {
                if (enable) _hotkeys?.Resume();
                else _hotkeys?.Suspend();
            };

            // En Config: modo “WasPressed” (NO repetir)
            _hotkeys?.SetEdgeMode(true);

            var dr = dlg.ShowDialog(this);

            // Al cerrar Config: volver a modo continuo (autorepeat) en el Form1
            _hotkeys?.SetEdgeMode(false);

            if (dr == DialogResult.OK && dlg.ApplyChanges())
            {
                _hotkeys?.Register(_state.Hotkeys, avoidRepeat: false);
                BumpAndSave();
            }
        }

        // Copiar la ruta del overlay sin mostrarla en mensajes (solo aviso)
        public void btnCopy_Click(object? sender, EventArgs e)
        {
            try
            {
                Clipboard.SetText(txtOverlay.Text);
                // Si el tray está visible, usa globo; si no, un MessageBox corto.
                if (_tray.Visible)
                {
                    _tray.BalloonTipTitle = AppName;
                    _tray.BalloonTipText = "Copied URL";
                    _tray.ShowBalloonTip(1500);
                }
                else
                {
                    MessageBox.Show(this, "Copied URL", AppName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch
            {
                MessageBox.Show(this, "ERROR copying URL", AppName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // --- Persistencia
        private void BumpAndSave()
        {
            // _state.Version++; // Si usas versionado incremental, deja esta línea
            _config.Save(_state); // Escribe overlay-data.js (o lo que defina tu servicio)
        }

        // --- Tray / Minimizar
        private void MainForm_Resize(object? sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
                GoToTray();
        }

        private void GoToTray()
        {
            _tray.Visible = true;
            this.ShowInTaskbar = false;
            this.Hide();
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
            // Útil desde el menú del tray → cierra realmente la app
            _tray.Visible = false;
            _tray.Dispose();
            Application.Exit();
        }

        // --- Utilidades privadas
        private ContextMenuStrip CreateTrayMenu()
        {
            var menu = new ContextMenuStrip();
            menu.Items.Add("Show", null, (_, __) => RestoreFromTray());
            menu.Items.Add(new ToolStripSeparator());
            menu.Items.Add("Incrementar", null, (_, __) => BtnInc());
            menu.Items.Add("Decrementar", null, (_, __) => BtnDec());
            menu.Items.Add("Resetear", null, (_, __) => BtnReset());
            menu.Items.Add(new ToolStripSeparator());
            menu.Items.Add("Salir", null, (_, __) => ExitApplication());
            return menu;
        }

        private void TryLoadTrayIcon()
        {
            try
            {
                string icoPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "app.ico");
                if (File.Exists(icoPath))
                {
                    _tray.Icon = new Icon(icoPath);
                }
                else
                {
                    // Fallback: icono del formulario o del sistema
                    _tray.Icon = this.Icon ?? SystemIcons.Application;
                }
            }
            catch
            {
                _tray.Icon = SystemIcons.Application;
            }
        }

        private void HandleHotkey(string action)
        {
            switch (action)
            {
                case "increment": BtnInc(); break;
                case "decrement": BtnDec(); break;
                case "reset": BtnReset();   break;
            }
        }

        // Handlers vacíos (si el diseñador los generó y no los usas, puedes quitarlos del .Designer)
        private void lblOverlay_Click(object? sender, EventArgs e) { /* intencionalmente vacío */ }
        private void txtOverlay_TextChanged(object? sender, EventArgs e) { /* intencionalmente vacío */ }
    }
}
