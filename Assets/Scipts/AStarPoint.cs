using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStarPoint {

    public AStarPoint parent;
    public bool IsObstacle;
    public Vector2 position;
    public GameObject cube;
    

    /// <summary>
    /// G从起点到指定方格移动花费
    /// H从指定方格到终点的花费
    /// F = G + H
    /// </summary>
    public float G, H, F;

    public AStarPoint(int x, int y) {
        position = new Vector2(x, y);
        parent = null;
        IsObstacle = false;
    }
}
