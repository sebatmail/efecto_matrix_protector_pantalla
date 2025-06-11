using System;
using System.Windows.Forms;
using System.Runtime.InteropServices; // Necesario para DllImport

namespace efectomatrix
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static int Main() // Asegúrate de que sea 'static int Main()'
        {
            // Obtener los argumentos de la línea de comandos
            string[] args = Environment.GetCommandLineArgs();

            // Inicialización de la aplicación (para .NET 6+ Windows Forms)
            ApplicationConfiguration.Initialize();

            // Lógica para manejar los argumentos de la línea de comandos (modo salvapantallas)
            if (args.Length > 1)
            {
                string firstRealArgument = args[1].ToLowerInvariant();
                switch (firstRealArgument)
                {
                    case "/c": // Modo configuración
                        MessageBox.Show("Este salvapantallas no tiene opciones de configuración.", "Matrix Rain", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return 0; // Terminar la aplicación
                    case "/p": // Modo vista previa
                        // Asegurarse de que haya un handle de ventana padre
                        if (args.Length > 2)
                        {
                            IntPtr previewHwnd = new IntPtr(long.Parse(args[2])); // Obtener el handle de la ventana padre
                            Form1 previewForm = new Form1(); // Crear una nueva instancia del formulario

                            // Establecer el formulario como hijo de la ventana de vista previa
                            NativeMethods.SetParent(previewForm.Handle, previewHwnd);

                            // Ajustar el estilo para que se comporte como un salvapantallas hijo
                            NativeMethods.SetWindowLong(previewForm.Handle, NativeMethods.GWL_STYLE,
                                NativeMethods.GetWindowLong(previewForm.Handle, NativeMethods.GWL_STYLE) | NativeMethods.WS_CHILD);

                            // Ajustar el tamaño del formulario para que coincida con la ventana de vista previa
                            // AQUÍ ES DONDE ESTABA EL ERROR CS0266, RESUELTO CON (int) CASTS
                            previewForm.Size = new System.Drawing.Size((int)NativeMethods.GetWindowWidth(previewHwnd), (int)NativeMethods.GetWindowHeight(previewHwnd));
                            previewForm.Location = new System.Drawing.Point(0, 0); // Posición en 0,0 dentro de la ventana padre

                            // Quitar bordes y controles no deseados para un salvapantallas
                            previewForm.FormBorderStyle = FormBorderStyle.None;
                            previewForm.ControlBox = false;
                            previewForm.MaximizeBox = false;
                            previewForm.MinimizeBox = false;
                            previewForm.WindowState = FormWindowState.Normal; // Mantener normal para que el tamaño coincida con la vista previa
                            previewForm.ShowInTaskbar = false; // No mostrar en la barra de tareas

                            Application.Run(previewForm); // Ejecutar el formulario en modo vista previa
                        }
                        return 0; // Terminar la aplicación
                    case "/s": // Modo pantalla completa (salvapantallas)
                        Application.Run(new Form1()); // Ejecutar el formulario principal en pantalla completa
                        return 0; // Terminar la aplicación
                    default: // Argumento no reconocido, ejecutar en modo pantalla completa por defecto
                        Application.Run(new Form1());
                        return 0; // Terminar la aplicación
                }
            }
            else // Sin argumentos, ejecutar en modo pantalla completa por defecto (o depuración)
            {
                Application.Run(new Form1());
                return 0; // Terminar la aplicación
            }
        }
    }

    // Clase auxiliar para las llamadas a la API nativa de Windows (P/Invoke)
    internal static class NativeMethods
    {
        // Importación de funciones de la API de Windows (user32.dll)
        [DllImport("user32.dll")]
        internal static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

        [DllImport("user32.dll")]
        internal static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        internal static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        // Constantes para GetWindowLong/SetWindowLong
        internal const int GWL_STYLE = -16;
        internal const int WS_CHILD = 0x40000000;
        internal const int WS_POPUP = unchecked((int)0x80000000); // Añadido unchecked para evitar advertencia si se usa como int

        // Estructura RECT para almacenar las coordenadas de un rectángulo
        [StructLayout(LayoutKind.Sequential)]
        internal struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        // Importación de GetClientRect para obtener las dimensiones del área cliente de una ventana
        // Se asegura que el tipo de retorno sea bool y los parámetros sean int.
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetClientRect(IntPtr hWnd, out RECT lpRect); // Hacerla privada ya que la envolvemos

        // Métodos auxiliares para obtener el ancho y alto de la ventana
        internal static int GetWindowWidth(IntPtr hWnd)
        {
            RECT rect;
            GetClientRect(hWnd, out rect);
            return rect.Right - rect.Left;
        }

        internal static int GetWindowHeight(IntPtr hWnd)
        {
            RECT rect;
            GetClientRect(hWnd, out rect);
            return rect.Bottom - rect.Top;
        }
    }
}