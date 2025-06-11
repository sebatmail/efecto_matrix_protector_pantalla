using System;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Text;
using System.Threading.Tasks;


namespace efectomatrix
{
    public partial class Form1 : Form
    {
        // Declaración de variables:
        private System.Windows.Forms.Timer? gameTimer;
        private char[] matrixChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!@#$%^&*()_+-=[]{};:'\",.<>/?`~".ToCharArray();
        private int[]? yPositions; // Se inicializa en InitializeMatrixEffect()
        private Random random = new Random();

        private Graphics? offScreenGraphics;
        private Bitmap? offScreenBitmap;

        private int fontSize = 16;
        private int charHeight = 1; // ¡Inicializamos a 1 para evitar división por cero!

        private Font matrixFont; // Declaramos la fuente aquí y la inicializamos en el constructor. No necesita '?' ni 'null!' porque siempre se inicializa.

        // Constructor del formulario (aquí se inicializa todo al inicio de la aplicación)
        public Form1()
        {
            InitializeComponent(); // Este método es generado por Visual Studio y es esencial para el formulario

            // Configuración del formulario
            this.Text = "Matrix Rain"; // Título de la ventana
            this.BackColor = Color.Black; // Fondo negro
            this.WindowState = FormWindowState.Maximized; // Inicia la ventana maximizada
            this.FormBorderStyle = FormBorderStyle.None; // Sin bordes ni botones de cerrar/minimizar

            // Suscripción de eventos: Esto conecta las funciones a los eventos del formulario
            // Se usa la sintaxis de "method group" que es la forma moderna y limpia de suscribirse a eventos.
            this.Paint += Form1_Paint; // Para dibujar en el formulario
            this.Resize += Form1_Resize; // Para reajustar si la ventana cambia de tamaño
            this.KeyDown += Form1_KeyDown; // Para detectar la tecla ESC y cerrar

            // Inicializamos la fuente aquí. Esto es mejor que en GameTimer_Tick para rendimiento.
            matrixFont = new Font("Consolas", fontSize);

            // Calculamos charHeight con un valor de respaldo
            using (Graphics g = this.CreateGraphics())
            {
                // Aseguramos que charHeight sea al menos 1 para evitar división por cero
                charHeight = Math.Max(1, (int)g.MeasureString("M", matrixFont).Height);
            }

            // Iniciar el efecto Matrix
            InitializeMatrixEffect();
        }

        // --- A partir de aquí, TODO el código del efecto Matrix ---

        // Método para inicializar o reiniciar el efecto Matrix (llamado en el constructor y al redimensionar)
        private void InitializeMatrixEffect()
        {
            // Detener el temporizador y liberar recursos gráficos si ya existen (al redimensionar)
            // Se utiliza el operador ?. (null-conditional) para llamar a Stop()/Dispose() solo si el objeto no es nulo.
            gameTimer?.Stop();
            offScreenGraphics?.Dispose();
            offScreenBitmap?.Dispose();

            // Asegurarse de que la ventana tenga un tamaño válido antes de intentar dibujar
            if (this.ClientSize.Width <= 0 || this.ClientSize.Height <= 0) return;

            // Aseguramos que fontSize no sea 0 para evitar posible división por cero en numColumns
            if (fontSize <= 0) fontSize = 16;

            // Calcular el número de columnas de código basándose en el ancho del formulario y el tamaño de la fuente
            int numColumns = this.ClientSize.Width / (fontSize / 2);
            if (numColumns <= 0) numColumns = 1; // Asegura que siempre haya al menos una columna

            yPositions = new int[numColumns];

            // Asignar una posición Y inicial aleatoria a cada columna (algunas empezarán fuera de la pantalla)
            for (int i = 0; i < numColumns; i++)
            {
                // Ahora charHeight tiene un valor mínimo de 1, evitando la división por cero
                yPositions[i] = random.Next(-(this.ClientSize.Height / charHeight), 0);
            }

            // Inicializar el bitmap y los gráficos off-screen
            offScreenBitmap = new Bitmap(this.ClientSize.Width, this.ClientSize.Height);
            offScreenGraphics = Graphics.FromImage(offScreenBitmap);

            // Configurar y arrancar el temporizador que controlará la animación
            gameTimer = new System.Windows.Forms.Timer();
            gameTimer.Interval = 40; // Intervalo en milisegundos (25 fotogramas por segundo)
            gameTimer.Tick += GameTimer_Tick; // El método GameTimer_Tick se llamará en cada 'tick' del temporizador
            gameTimer.Start();
        }

        // Este método se llama en cada 'tick' del temporizador para actualizar la animación
        private void GameTimer_Tick(object? sender, EventArgs e) // 'sender' puede ser nulo, por eso 'object?'
        {
            // Asegurarse de que offScreenGraphics y offScreenBitmap no son nulos antes de usarlos.
            // Esto debería estar garantizado por InitializeMatrixEffect(), pero es buena práctica en el contexto de '?'
            if (offScreenGraphics == null || offScreenBitmap == null) return;

            offScreenGraphics.FillRectangle(new SolidBrush(Color.FromArgb(10, 0, 0, 0)), 0, 0, offScreenBitmap.Width, offScreenBitmap.Height);

            // Usamos la fuente ya inicializada (matrixFont, declarada como campo de clase)

            // Itera sobre cada columna de código
            if (yPositions != null) // Asegurarse de que yPositions no sea nulo antes de iterar
            {
                for (int i = 0; i < yPositions.Length; i++)
                {
                    int columnX = i * (fontSize / 2);
                    int currentY = yPositions[i] * charHeight;

                    // Dibuja la "cabeza" del rastro (blanca)
                    if (currentY >= 0 && currentY < offScreenBitmap.Height)
                    {
                        offScreenGraphics.DrawString(matrixChars[random.Next(matrixChars.Length)].ToString(),
                                                     matrixFont, // Usamos la variable de clase matrixFont
                                                     Brushes.White, // Color blanco para la cabeza
                                                     columnX,
                                                     currentY);
                    }

                    // Dibuja la "cola" del rastro (verde que se desvanece)
                    for (int j = 1; j < 20; j++) // Empieza desde 1 para no sobrescribir la cabeza
                    {
                        int tailY = currentY - (j * charHeight);
                        if (tailY >= 0 && tailY < offScreenBitmap.Height)
                        {
                            int greenValue = 255 - (j * 15);
                            if (greenValue < 0) greenValue = 0;

                            Color fadeColor = Color.FromArgb(0, greenValue, 0);

                            offScreenGraphics.DrawString(matrixChars[random.Next(matrixChars.Length)].ToString(),
                                                         matrixFont, // Usamos la variable de clase matrixFont
                                                         new SolidBrush(fadeColor),
                                                         columnX,
                                                         tailY);
                        }
                    }

                    // Mueve la posición Y de la columna hacia abajo
                    yPositions[i]++;

                    // Si la columna sale por la parte inferior, la reinicia en la parte superior (aleatoriamente)
                    if (yPositions[i] * charHeight > this.ClientSize.Height + random.Next(100, 500))
                    {
                        yPositions[i] = random.Next(-(this.ClientSize.Height / charHeight), 0); // Reinicia en una posición Y aleatoria en la parte superior (incluso fuera de la pantalla)
                    }
                }
            }

            this.Invalidate(); // Fuerza al formulario a redibujarse, lo que llamará a Form1_Paint
        }

        // Este método se llama cada vez que el formulario necesita ser dibujado o redibujado
        private void Form1_Paint(object? sender, PaintEventArgs e) // 'sender' puede ser nulo
        {
            // Dibuja el contenido del bitmap off-screen (donde hemos estado dibujando todo) en el formulario
            if (offScreenBitmap != null)
            {
                e.Graphics.DrawImage(offScreenBitmap, 0, 0);
            }
        }

        // Este método se llama cuando el formulario cambia de tamaño
        private void Form1_Resize(object? sender, EventArgs e) // 'sender' puede ser nulo
        {
            // Antes de reinicializar el efecto, liberar la fuente antigua si existe
            // matrixFont no necesita '?' porque se garantiza que se inicializa en el constructor.
            // Sin embargo, Dispose es una buena práctica para recursos IDisposable.
            matrixFont.Dispose(); // Libera los recursos de la fuente

            // Recalcular la altura del carácter y reiniciar la fuente para el nuevo tamaño
            matrixFont = new Font("Consolas", fontSize);
            using (Graphics g = this.CreateGraphics())
            {
                charHeight = Math.Max(1, (int)g.MeasureString("M", matrixFont).Height);
            }
            InitializeMatrixEffect();
        }

        // Este método se llama cuando se presiona una tecla en el formulario
        private void Form1_KeyDown(object? sender, KeyEventArgs e) // 'sender' puede ser nulo
        {
            // Si la tecla presionada es ESC, cierra la aplicación
            if (e.KeyCode == Keys.Escape)
            {
                this.Close();
            }
        }
    } // <--- Esta llave cierra la clase Form1
} // <--- Esta llave cierra el namespace