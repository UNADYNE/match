using System.Collections.Generic;
using UnityEngine;

namespace Tools
{
    /// <summary>
    /// This class will allow organizing anything into a grid system.
    /// You must call InitializeGrid() before using any other methods.
    /// The grid dimensions must be positive integers.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class GridSystem<T> : Singleton<GridSystem<T>>
    {
        private T[,] _data;
        private Vector2Int _dimensions = new Vector2Int(1, 1);
        
        public Vector2Int Dimensions => _dimensions;

        private bool _isReady;
        
        public bool IsReady => _isReady;


        // initialize the data array
        public void InitializeGrid(Vector2Int dimensions)
        {
            if(dimensions.x < 1 || dimensions.y < 1)
                Debug.LogError("Grid dimensions must be greater than 0");
            
            _dimensions = dimensions;
            _data = new T[dimensions.x, dimensions.y];
            _isReady = true;
        }
        
        // clear grid
        public void ClearGrid()
        {
            _data = new T[_dimensions.x, _dimensions.y];
        }
        // check bounds of grid

        public bool BoundsCheck(int x, int y)
        {
            if(!_isReady)
                Debug.LogError("Grid has not been initialized");
            return x >= 0 && x < _dimensions.x && y >= 0 && y < _dimensions.y;
        }
        
        public bool BoundsCheck(Vector2Int position)
        {
            return BoundsCheck(position.x, position.y);
        }
        
        // check if grid position is empty
        public bool IsEmpty(int x, int y)
        {
            if(!BoundsCheck(x, y))
                Debug.LogError($"(${x}, ${y}) are not on the grid.");
            return EqualityComparer<T>.Default.Equals(_data[x, y], default(T));
        }
        
        public bool IsEmpty(Vector2Int position)
        {
            return IsEmpty(position.x, position.y);
        }
        
        // put item on grid
        public bool PutItemAt(T item, int x, int y, bool allowOverwrite = false)
        {
           if(!BoundsCheck(x, y))
               Debug.LogError($"(${x}, ${y}) are not on the grid.");

           if (!allowOverwrite && !IsEmpty(x, y)) return false;
           
           _data[x, y] = item;
              return true;
        }
        
        public bool PutItemAt(T item, Vector2Int position, bool allowOverwrite = false)
        {
            return PutItemAt(item, position.x, position.y, allowOverwrite);
        }
        // get item from grid
        public T GetItemAt(int x, int y)
        {
            if(!BoundsCheck(x, y))
                Debug.LogError($"(${x}, ${y}) are not on the grid.");
            return _data[x, y];
        }
        
        public T GetItemAt(Vector2Int position)
        {
            return GetItemAt(position.x, position.y);
        }
        
        // remove item from grid
        public T RemoveItemAt(int x, int y)
        {
            if(!BoundsCheck(x, y))
                Debug.LogError($"(${x}, ${y}) are not on the grid.");
            T temp = _data[x, y];
            _data[x, y] = default(T);
            return temp;
        }
        
        public T RemoveItemAt(Vector2Int position)
        {
            return RemoveItemAt(position.x, position.y);
        }       
        
        public bool MoveItemTo(int x1, int y1, int x2, int y2, bool allowOverwrite = false)
        {
            if(!BoundsCheck(x1, y1))
                Debug.LogError($"(${x1}, ${y1}) are not on the grid.");
            
            if(!BoundsCheck(x2, y2))
                Debug.LogError($"(${x2}, ${y2}) are not on the grid.");

            if (!allowOverwrite && !IsEmpty(x2, y2)) return false;
           
            _data[x2, y2] = RemoveItemAt(x1, y1);
            return true;
        }
        
        public bool MoveItemTo(Vector2Int position1, Vector2Int position2, bool allowOverwrite = false)
        {
            return MoveItemTo(position1.x, position1.y, position2.x, position2.y, allowOverwrite);
        }
        
        // swap 2 items on grid

        public void SwapItemsAt(int x1, int y1, int x2, int y2)
        {
            BoundsCheck(x1, y1);
            BoundsCheck(x2, y2);

            T temp = _data[x1, y1];
            _data[x1, y1] = _data[x2, y2];
            _data[x2, y2] = temp;
        }
        
        // Move Item On Grid
   
        
        public void SwapItemsAt(Vector2Int position1, Vector2Int position2)
        {
            SwapItemsAt(position1.x, position1.y, position2.x, position2.y);
        }
        
        
        // convert the grid data to string[][]
        public override string ToString()
        {
            string s = "";
            for (int y = _dimensions.y - 1; y != -1; --y)
            {
                s += "[ ";
                for (int x = 0; x != _dimensions.x; ++x)
                {
                    if (IsEmpty(x, y))
                    {
                        s += "x";
                    } else
                    {
                        s += _data[x, y].ToString();
                    }
                    
                    if(x != _dimensions.x - 1)
                    {
                        s += ", ";
                    }
                }
                s += " ]\n";
            }
            return s;
        }
        
    }
}