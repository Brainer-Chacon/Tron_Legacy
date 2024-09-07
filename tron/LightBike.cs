using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace tron
{
    public enum Direction
    {
        Up,
        Down,
        Left,
        Right
    }

    public class LightCycle
    {
        public LinkedList<GridNode> Trail { get; private set; }
        public Direction CurrentDirection { get; set; }
        public GridNode CurrentNode => Trail.First.Value;

        private int _maxTrailLength;
        protected GameGrid _gameGrid;

        public LightCycle(GridNode startNode, int initialTrailLength, int maxTrailLength, GameGrid gameGrid)
        {
            Trail = new LinkedList<GridNode>();
            _maxTrailLength = maxTrailLength;
            _gameGrid = gameGrid;

            for (int i = 0; i < initialTrailLength; i++)
            {
                Trail.AddLast(startNode);
            }

            foreach (var node in Trail)
            {
                node.IsOccupied = true;
            }
        }

        public virtual void Move()
        {
            GridNode nextNode = GetNextNode(CurrentDirection);
       
            if (nextNode != null)
            {
                if (!nextNode.IsOccupied || nextNode == Trail.Last.Value)
                {
                    Trail.AddFirst(nextNode);
                    nextNode.IsOccupied = true;
       
                    if (Trail.Count > _maxTrailLength)
                    {
                        GridNode oldestNode = Trail.Last.Value;
                        oldestNode.IsOccupied = false;
                        Trail.RemoveLast();
                        Debug.WriteLine($"Nodo eliminado: ({oldestNode.X}, {oldestNode.Y})");
                    }
       
                    Debug.WriteLine($"Nodo añadido: ({nextNode.X}, {nextNode.Y})");
                }
            }
        }
       
        public bool CanChangeDirection(Direction newDirection)
        {
            // Previene que la dirección cambie a la opuesta
            return !(CurrentDirection == Direction.Up && newDirection == Direction.Down ||
                     CurrentDirection == Direction.Down && newDirection == Direction.Up ||
                     CurrentDirection == Direction.Left && newDirection == Direction.Right ||
                     CurrentDirection == Direction.Right && newDirection == Direction.Left);
        }

        public GridNode GetNextNode(Direction direction)
        {
            // Obtiene la posición actual
            var currentNode = Trail.First.Value;
            int currentRow = currentNode.X; // Cambiado a X para coincidir con GridNode
            int currentColumn = currentNode.Y; // Cambiado a Y para coincidir con GridNode

            // Calcula el siguiente nodo basado en la dirección
            GridNode nextNode = null;

            switch (direction)
            {
                case Direction.Up:
                    nextNode = _gameGrid.GetNodeAtPosition(currentRow - 1, currentColumn);
                    break;
                case Direction.Down:
                    nextNode = _gameGrid.GetNodeAtPosition(currentRow + 1, currentColumn);
                    break;
                case Direction.Left:
                    nextNode = _gameGrid.GetNodeAtPosition(currentRow, currentColumn - 1);
                    break;
                case Direction.Right:
                    nextNode = _gameGrid.GetNodeAtPosition(currentRow, currentColumn + 1);
                    break;
            }

            // Imprime el siguiente nodo en la dirección dada
            Debug.WriteLine($"Nodo siguiente en dirección {direction}: ({nextNode?.X}, {nextNode?.Y})");

            return nextNode;
        }
    }
}
