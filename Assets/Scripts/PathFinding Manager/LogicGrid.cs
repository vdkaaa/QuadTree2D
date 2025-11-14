
using System;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace IsometricGame.Logic
{
    public class LogicGrid
    {

       public struct CellIndex
        {
            public int i;
            public int j;

            // Constructor: te permite crear una instancia con valores iniciales
            public CellIndex(int i, int j)
            {
                this.i = i;
                this.j = j;
            }
        }

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
            public bool InBounds(CellIndex c)
            {
                return c.i >= 0 && c.i < grid.width
                    && c.j >= 0 && c.j < grid.height;
            }
            #endregion

            #region Consultas
            public bool IsWalkable(CellIndex c)
            {
                if (!InBounds(c)) return false;
                return _tiles[c.i, c.j].walkable;
            }
            public float GetCost(CellIndex c)
            {
                if (!InBounds(c)) return float.PositiveInfinity;
                return _tiles[c.i, c.j].cost;

            }
            public int GetArea(CellIndex c)
            {
                if (!InBounds(c)) return -1;
                return _tiles[c.i, c.j].areaId;
            }
            #endregion

            #region Edición

            public void SetTile(CellIndex c, TileData t)
            {
                if (InBounds(c))
                {
                    _tiles[c.i, c.j] = t;
                }
            }

            public void SetWalkable(CellIndex c, bool value)
            {
                if (InBounds(c))
                {
                    _tiles[c.i, c.j].walkable = value;
                }
            }

            public void SetCost(CellIndex c, float value)
            {
                if (InBounds(c))
                {
                    _tiles[c.i, c.j].cost = Mathf.Max(0, value);
                }
            }

            public void SetArea(CellIndex c, int id)
            {
                if (InBounds(c))
                {
                    _tiles[c.i, c.j].areaId = id;
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

