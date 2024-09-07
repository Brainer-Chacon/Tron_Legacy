using System;

namespace tron
{
    public class GridNode
    {
        public GridNode Up { get; set; }
        public GridNode Down { get; set; }
        public GridNode Left { get; set; }
        public GridNode Right { get; set; }

        public int X { get; set; } // Cambiado a X para coincidir con LightCycle
        public int Y { get; set; } // Cambiado a Y para coincidir con LightCycle
        public bool IsOccupied { get; set; } // Estado de ocupación del nodo

        public GridNode(int x, int y)
        {
            X = x;
            Y = y;
            IsOccupied = false; // Por defecto, el nodo no está ocupado
        }
    }

    public class GameGrid
    {
        public int Rows { get; private set; }
        public int Cols { get; private set; }
        public GridNode[,] Grid { get; private set; }

        public GameGrid(int rows, int cols)
        {
            Rows = rows;
            Cols = cols;
            Grid = new GridNode[rows, cols];
            InitializeGrid();
        }

        private void InitializeGrid()
        {
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Cols; j++)
                {
                    Grid[i, j] = new GridNode(i, j);

                    if (i > 0)
                    {
                        Grid[i, j].Up = Grid[i - 1, j];
                    }

                    if (i < Rows - 1)
                    {
                        Grid[i, j].Down = Grid[i + 1, j];
                    }

                    if (j > 0)
                    {
                        Grid[i, j].Left = Grid[i, j - 1];
                    }

                    if (j < Cols - 1)
                    {
                        Grid[i, j].Right = Grid[i, j + 1];
                    }
                }
            }
        }

        public GridNode GetNodeAtPosition(int row, int column)
        {
            if (row >= 0 && row < Rows && column >= 0 && column < Cols)
            {
                return Grid[row, column];
            }
            return null;
        }
    }
}
