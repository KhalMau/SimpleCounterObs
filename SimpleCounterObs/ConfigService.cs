using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;



namespace SimpleCounterObs
{
    public class ConfigService
    {
        public string JsPath { get; }

        public ConfigService()
        {
            var baseDir = AppDomain.CurrentDomain.BaseDirectory;
            JsPath = Path.Combine(baseDir, "overlay-data.js");
        }

        public CounterState Load()
        {
            try
            {
                if (File.Exists(JsPath))
                {
                    var text = File.ReadAllText(JsPath);

                    // Quitar el prefijo "window.__STATE__ =" y el ";" final si existen
                    var json = text.Replace("window.__STATE__ =", "")
                                   .Trim();

                    if (json.EndsWith(";"))
                        json = json[..^1].Trim();

                    var state = JsonSerializer.Deserialize<CounterState>(json);
                    if (state != null) return state;
                }
            }
            catch
            {
                // Ignora errores de lectura/parseo y devuelve valores por defecto
            }

            return new CounterState();
        }

        public void Save(CounterState state)
        {
            try
            {
                //state.Version++;

                var json = JsonSerializer.Serialize(state, new JsonSerializerOptions
                {
                    WriteIndented = true
                });

                var js = $"window.__STATE__ = {json};{Environment.NewLine}";
                WriteAllTextAtomic(JsPath, js);
            }
            catch
            {
                // Ignora errores de IO (opcional: log)
            }
        }

        // Escritura atómica (escribe a .tmp y luego reemplaza)
        private static void WriteAllTextAtomic(string path, string contents)
        {
            var dir = Path.GetDirectoryName(path)!;
            Directory.CreateDirectory(dir);

            var tmp = Path.Combine(dir, $"overlay-data.{Guid.NewGuid():N}.tmp");
            File.WriteAllText(tmp, contents);
            if (File.Exists(path))
            {
                // En Windows, Replace mueve de forma atómica
                var bak = path + ".bak";
                File.Replace(tmp, path, bak, ignoreMetadataErrors: true);
                try { File.Delete(bak); } catch { /* ignore */ }
            }
            else
            {
                File.Move(tmp, path);
            }
        }
    }
}
