using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class GetWall : MonoBehaviour
{
    int gridSizeX, gridSizeY;
    public Tilemap _wallTilemap;
    List<Pair<float, float>> walls;

    void Awake()
    {
        walls = new List<Pair<float, float>>();

        gridSizeY = Mathf.RoundToInt(Camera.main.orthographicSize);
        gridSizeX = Mathf.RoundToInt(gridSizeY * Camera.main.aspect);


        for (int x = -gridSizeX; x < gridSizeX; x++)
        {
            for (int y = -gridSizeY; y < gridSizeY; y++)
            {
                if (_wallTilemap.GetTile(new Vector3Int(x, y, 0)) == null) continue;

                float wall_x = x;
                float wall_y = y;

                //중심점 
                //0.5 -> nodeHalfSize
                wall_x += 0.5f;
                wall_y += 0.5f;

                //벽에 해당하는 좌표값 저장 
                WallList(wall_x, wall_y);
            }
        }
    }

  
   void WallList(float x,float y)
    {
        walls.Add(new Pair<float,float>(x,y));
 
    }

    public List<Pair<float, float>> getVal
    {
        get{
            return walls;
        }
    }
}


//pair 형식 생성 
[Serializable]
public class Pair<T, U>
{
    public T First { get; set; }
    public U Second { get; set; }

    public Pair(T first, U second)
    {
        this.First = first;
        this.Second = second;
    }

    //public override string ToString()
    //{
    //    return $"({First}, {Second})";
    //}
};
