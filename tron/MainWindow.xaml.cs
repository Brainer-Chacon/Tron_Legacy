using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;     
using System.Diagnostics;
using System.Windows.Threading;

namespace tron
{
    public partial class MainWindow : Window
    {
        private const int CellSize = 20;
        private GameGrid gameGrid;
        private LightCycle playerLightCycle;
        private BotLightCycle[] botLightCycles;
        private DispatcherTimer gameTimer;
        private DispatcherTimer botMovementTimer;

        private const int Separation = 3; // Número de celdas para separar las motos
        private static readonly Color[] BotColors = { Colors.Red, Colors.Green, Colors.Yellow };

        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;
            this.SizeChanged += Window_SizeChanged; // Asegúrate de que este evento esté asociado
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            InitializeGame();
            StartGame();
        }

        private void InitializeGame()
        {
            if (gameCanvas.ActualHeight > 0 && gameCanvas.ActualWidth > 0)
            {
                int rows = (int)(gameCanvas.ActualHeight / CellSize);
                int cols = (int)(gameCanvas.ActualWidth / CellSize);
                gameGrid = new GameGrid(rows, cols);

                Random random = new Random();
                int centerRow = rows / 2;
                int centerCol = cols / 2;

                // Coloca la moto del jugador en el centro
                var playerStartNode = gameGrid.Grid[centerRow, centerCol];
                playerLightCycle = new LightCycle(playerStartNode, 3, 3, gameGrid);

                // Coloca las motos de los bots alrededor del centro
                botLightCycles = new BotLightCycle[3];
                for (int i = 0; i < botLightCycles.Length; i++)
                {
                    int botRow = centerRow + (i + 1) * Separation;
                    int botCol = centerCol + (i + 1) * Separation;
                    botRow = Math.Clamp(botRow, 0, rows - 1);
                    botCol = Math.Clamp(botCol, 0, cols - 1);
                    var botStartNode = gameGrid.Grid[botRow, botCol];

                    // Asigna un color diferente a cada bot
                    Color botColor = i switch
                    {
                        0 => Colors.Red,
                        1 => Colors.Green,
                        2 => Colors.Yellow,
                        _ => Colors.Gray
                    };

                    botLightCycles[i] = new BotLightCycle(botStartNode, 3, 3, gameGrid, new SolidColorBrush(botColor));
                }

                DrawGrid();
                DrawLightCycles();
            }
            else
            {
                MessageBox.Show("El tamaño del canvas es inválido.", "Error de Inicialización", MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void DrawGrid()
        {
            gameCanvas.Children.Clear();

            if (gameCanvas.ActualWidth > 0 && gameCanvas.ActualHeight > 0)
            {
                for (int i = 0; i < gameGrid.Rows; i++)
                {
                    for (int j = 0; j < gameGrid.Cols; j++)
                    {
                        var node = gameGrid.Grid[i, j];

                        Rectangle rect = new Rectangle
                        {
                            Width = CellSize,
                            Height = CellSize,
                            Stroke = Brushes.Black,
                            Fill = node.IsOccupied ? Brushes.Red : Brushes.White
                        };

                        Canvas.SetLeft(rect, j * CellSize);
                        Canvas.SetTop(rect, i * CellSize);

                        gameCanvas.Children.Add(rect);
                    }
                }
            }
        }

        private void DrawLightCycles()
        {
            // Dibuja la moto del jugador
            foreach (var node in playerLightCycle.Trail)
            {
                Rectangle rect = new Rectangle
                {
                    Width = CellSize,
                    Height = CellSize,
                    Fill = Brushes.Blue
                };

                Canvas.SetLeft(rect, node.Y * CellSize);
                Canvas.SetTop(rect, node.X * CellSize);

                gameCanvas.Children.Add(rect);
            }

            // Dibuja las motos de los bots
            foreach (var bot in botLightCycles)
            {
                foreach (var node in bot.Trail)
                {
                    Rectangle rect = new Rectangle
                    {
                        Width = CellSize,
                        Height = CellSize,
                        Fill = bot.Color // Usa el Brush aquí
                    };

                    Canvas.SetLeft(rect, node.Y * CellSize);
                    Canvas.SetTop(rect, node.X * CellSize);

                    gameCanvas.Children.Add(rect);
                }
            }
        }


        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            Direction newDirection;

            // Imprime la tecla que se presionó
            Debug.WriteLine($"Tecla presionada: {e.Key}");

            switch (e.Key)
            {
                case Key.Up:
                    newDirection = Direction.Up;
                    break;
                case Key.Down:
                    newDirection = Direction.Down;
                    break;
                case Key.Left:
                    newDirection = Direction.Left;
                    break;
                case Key.Right:
                    newDirection = Direction.Right;
                    break;
                default:
                    return;
            }

            // Imprime la dirección actual antes de intentar cambiarla
            Debug.WriteLine($"Dirección actual antes de cambiar: {playerLightCycle.CurrentDirection}");

            // Verifica que la nueva dirección no sea opuesta a la actual
            if (playerLightCycle.CanChangeDirection(newDirection))
            {
                playerLightCycle.CurrentDirection = newDirection;
            }

            // Imprime la dirección actual después de intentar cambiarla
            Debug.WriteLine($"Dirección actual después de cambiar: {playerLightCycle.CurrentDirection}");
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (gameCanvas != null && gameCanvas.ActualHeight > 0 && gameCanvas.ActualWidth > 0)
            {
                int newRows = (int)(gameCanvas.ActualHeight / CellSize);
                int newCols = (int)(gameCanvas.ActualWidth / CellSize);

                if (gameGrid == null || gameGrid.Rows != newRows || gameGrid.Cols != newCols)
                {
                    gameGrid = new GameGrid(newRows, newCols);
                    InitializeGame();
                }
            }
            else
            {
                Debug.WriteLine("Canvas no está inicializado o tiene un tamaño inválido.");
            }
        }

        private void StartGame()
        {
            gameTimer = new DispatcherTimer();
            gameTimer.Interval = TimeSpan.FromMilliseconds(100); // Ajusta el intervalo según sea necesario
            gameTimer.Tick += (s, e) =>
            {
                playerLightCycle.Move();
                foreach (var bot in botLightCycles)
                {
                    bot.Move();
                }
                DrawGrid();
                DrawLightCycles();
            };
            gameTimer.Start();

            botMovementTimer = new DispatcherTimer();
            botMovementTimer.Interval = TimeSpan.FromSeconds(1.5);
            botMovementTimer.Tick += (s, e) =>
            {
                Random random = new Random();
                foreach (var bot in botLightCycles)
                {
                    bot.RandomChangeDirection();
                }
            };
            botMovementTimer.Start();
        }
    }
}
