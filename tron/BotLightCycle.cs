using System;
using System.Windows.Media;
using System.Windows.Threading;

namespace tron
{
    public class BotLightCycle : LightCycle
    {
        private static readonly Random random = new Random();
        private DispatcherTimer directionChangeTimer;
        private int changeDirectionInterval = 1500; // Intervalo para cambiar dirección (en milisegundos)
        private int directionChangeCounter = 0; // Contador para el número de cambios de dirección

        public Brush Color { get; private set; }

        public BotLightCycle(GridNode startNode, int width, int height, GameGrid gameGrid, Brush botColor)
            : base(startNode, width, height, gameGrid)
        {
            Color = botColor;
            InitializeDirectionChangeTimer();
        }

        private void InitializeDirectionChangeTimer()
        {
            directionChangeTimer = new DispatcherTimer();
            directionChangeTimer.Interval = TimeSpan.FromMilliseconds(changeDirectionInterval);
            directionChangeTimer.Tick += (s, e) => ChangeDirectionRandomly();
            directionChangeTimer.Start();
        }

        private void ChangeDirectionRandomly()
        {
            // Cambia la dirección aleatoriamente, pero evita el retroceso
            Direction newDirection;
            do
            {
                newDirection = (Direction)random.Next(4); // Asigna una dirección aleatoria
            } while (!CanChangeDirection(newDirection)); // Asegúrate de que la dirección sea válida

            CurrentDirection = newDirection;
        }

        public override void Move()
        {
            base.Move();
            directionChangeCounter++;
            if (directionChangeCounter >= 3)
            {
                ChangeDirectionRandomly();
                directionChangeCounter = 0;
            }
        }

        public void RandomChangeDirection()
        {
            // Obtén una dirección aleatoria, pero evita que sea la dirección opuesta a la actual
            Direction newDirection;
            do
            {
                newDirection = (Direction)random.Next(4); // Obtén una dirección aleatoria
            }
            while (!CanChangeDirection(newDirection)); // Asegúrate de que la nueva dirección sea válida

            CurrentDirection = newDirection;
        }

    }

}