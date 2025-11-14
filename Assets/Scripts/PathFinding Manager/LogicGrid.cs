
using System;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace IsometricGame.Logic
{
    public class LogicGrid
    {
        public struct TileData
        {
            public  bool walkable;
            public  float cost;
            public  int areaId;

            public TileData(bool Walkable, float Cost, int AreaId)
            {
                this.walkable = Walkable;
                this.cost = Cost;
                this.areaId = AreaId;
            }
        }

        public struct  GridConfig
        {
            public GridConfig(int Width, int Height)
            {
                this.width = Width;
                this.height = Height;
            }
            public int width;
            public int height;
        }


        public class PathGrid
        {
            GridConfig grid = new GridConfig();

            TileData[,] _tiles;


            public void Initialize(GridConfig gridConfig)
            {
                grid = gridConfig;
                _tiles = new TileData[gridConfig.width, gridConfig.height];
                ClearToDefaults();
            }

            public void ClearToDefaults()
            {
                for (int i = 0; i < grid.width; i++)
                {
                    for (int j = 0; j < grid.height; j++)
                    {
                        _tiles[i, j] = new TileData(true,0.0f,0);
                    }
                }
            }


            #region Verificación
            public bool InBounds(Vector2Int c)
            {
                return c.x >= 0 && c.x < grid.width
                    && c.y >= 0 && c.y < grid.height;
            }
            #endregion

            #region Consultas
            public bool IsWalkable(Vector2Int c)
            {
                if (!InBounds(c)) return false;
                return _tiles[c.x, c.y].walkable;
            }
            public float GetCost(Vector2Int c)
            {
                if (!InBounds(c)) return float.PositiveInfinity;
                return _tiles[c.x, c.y].cost;

            }
            public int GetArea(Vector2Int c)
            {
                if (!InBounds(c)) return -1;
                return _tiles[c.x, c.y].areaId;
            }
            #endregion

            #region Edición

            public void SetTile(Vector2Int c, TileData t)
            {
                if (InBounds(c))
                {
                    _tiles[c.x, c.y] = t;
                }
            }

            public void SetWalkable(Vector2Int c, bool value)
            {
                if (InBounds(c))
                {
                    _tiles[c.x, c.y].walkable = value;
                }
            }

            public void SetCost(Vector2Int c, float value)
            {
                if (InBounds(c))
                {
                    _tiles[c.x, c.y].cost = Mathf.Max(0, value);
                }
            }

            public void SetArea(Vector2Int c, int id)
            {
                if (InBounds(c))
                {
                    _tiles[c.x, c.y].areaId = id;
                }
            }
            #endregion

            #region Utilidades

            public int CountWalkables()
            {
                int total = 0;

                for (int i = 0; i < grid.width; i++)
                {
                    for (int j = 0; j < grid.height; j++)
                    {
                        if (_tiles[i, j].walkable)
                        {
                            total += 1;
                        }
                    }
                }
                return total;
            }

            public TileData SampleCell(int i, int j)
            {
                if (i < 0 || i >= grid.width) return new TileData(false, float.PositiveInfinity, -1);
                if (j < 0 || j >= grid.height) return new TileData(false, float.PositiveInfinity, -1);

                return _tiles[i, j];
            }

            #endregion 

            #region ImprimirMap
            
            public void  GetAllMap()
            {
                for (int i = 0; i < grid.width; i++)
                {
                    for (int j = 0; j < grid.height; j++)
                    {
                        Debug.Log("Casilla i: " + i + " j: " + j);
                    }
                }
            }
            #endregion

        }    
        
    }
}

