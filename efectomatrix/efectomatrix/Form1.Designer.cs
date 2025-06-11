// Este es un ejemplo de cómo debería verse Form1.Designer.cs
// ¡NO REEMPLACES TODO EL CONTENIDO DE TU Form1.Designer.cs CON ESTO!
// Solo busca el método 'Dispose' y modifica su contenido.

namespace efectomatrix
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing) // ESTE ES EL BLOQUE QUE DEBES MODIFICAR
            {
                // AQUI DEBES PEGAR EL CÓDIGO QUE QUITASTE DE Form1.cs
                // (la lógica de gameTimer, offScreenGraphics, offScreenBitmap, matrixFont)

                gameTimer?.Dispose(); // Añadido desde tu Form1.cs
                offScreenGraphics?.Dispose(); // Añadido desde tu Form1.cs
                offScreenBitmap?.Dispose(); // Añadido desde tu Form1.cs
                matrixFont.Dispose(); // Añadido desde tu Form1.cs

                // Asegúrate de que el bloque original del diseñador también esté aquí:
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container(); // Esto puede existir o no, dependiendo de tu form
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Text = "Form1"; // Este es el texto por defecto, ya lo cambias en Form1.cs
        }

        #endregion
    }
}