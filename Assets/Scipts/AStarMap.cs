using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStarMap
{
    private static AStarMap instance;
    public static AStarMap Instance {
        get {
            if(instance == null) {
                instance = new AStarMap();
            }
            return instance;
        }
    }

    public AStarPoint[,] highMap;
    public AStarPoint[,] lowMap;

    public const int Width = 50;
    public const int Height = 50;

    public AStarMap() {
        InitMap();
    }

    private void InitMap() {
        lowMap = new AStarPoint[Width, Height];
        for(int i = 0; i < Width; i++) {
            for(int j = 0; j < Height; j++) {
                lowMap[i, j] = new AStarPoint(i, j);
            }
        }

        highMap = new AStarPoint[Width/10, Height/10];
        for(int i = 0; i < Width/10; i++) {
            for(int j = 0; j < Height/10; j++) {
                highMap[i, j] = new AStarPoint(i, j);
            }
        }
    }



}
