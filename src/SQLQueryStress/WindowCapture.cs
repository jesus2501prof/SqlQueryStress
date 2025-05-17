using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLQueryStress
{
    using System;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.Runtime.InteropServices;
    using System.Windows.Forms;

    internal class WindowCapture
    {
        // Importaciones de funciones de Windows API
        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        private static extern IntPtr GetWindowRect(IntPtr hWnd, ref Rect rect);

        [StructLayout(LayoutKind.Sequential)]
        private struct Rect
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        /// <summary>
        /// Captura la ventana actualmente activa de la aplicación
        /// </summary>
        /// <returns>Imagen Bitmap con la captura de pantalla</returns>
        public static Bitmap CaptureActiveWindow()
        {
            return CaptureWindow(GetForegroundWindow());
        }

        /// <summary>
        /// Captura una ventana específica por su handle
        /// </summary>
        /// <param name="handle">Handle de la ventana a capturar</param>
        /// <returns>Imagen Bitmap con la captura de pantalla</returns>
        public static Bitmap CaptureWindow(IntPtr handle)
        {
            try
            {
                Rect rect = new Rect();
                GetWindowRect(handle, ref rect);

                int width = rect.Right - rect.Left;
                int height = rect.Bottom - rect.Top;

                if (width <= 0 || height <= 0)
                    throw new InvalidOperationException("La ventana no tiene un tamaño válido para capturar.");

                Bitmap bmp = new Bitmap(width, height, PixelFormat.Format32bppArgb);
                using (Graphics graphics = Graphics.FromImage(bmp))
                {
                    graphics.CopyFromScreen(rect.Left, rect.Top, 0, 0, new Size(width, height), CopyPixelOperation.SourceCopy);
                }

                return bmp;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al capturar la ventana: " + ex.Message, ex);
            }
        }

        /// <summary>
        /// Guarda la captura de la ventana activa en un archivo
        /// </summary>
        /// <param name="filePath">Ruta completa del archivo donde guardar la imagen</param>
        /// <param name="format">Formato de la imagen (por defecto: PNG)</param>
        public static void SaveActiveWindowCapture(string filePath, ImageFormat format = null)
        {
            if (format == null)
                format = ImageFormat.Png;

            using (Bitmap bmp = CaptureActiveWindow())
            {
                bmp.Save(filePath, format);
            }
        }

        /// <summary>
        /// Captura la ventana actual del formulario proporcionado
        /// </summary>
        /// <param name="form">Formulario a capturar</param>
        /// <returns>Imagen Bitmap con la captura de pantalla</returns>
        public static Bitmap CaptureCurrentForm(Form form)
        {
            if (form == null)
                throw new ArgumentNullException(nameof(form));

            try
            {
                Bitmap bmp = new Bitmap(form.Width, form.Height);
                form.DrawToBitmap(bmp, new Rectangle(0, 0, form.Width, form.Height));
                return bmp;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al capturar el formulario: " + ex.Message, ex);
            }
        }
    }
}
