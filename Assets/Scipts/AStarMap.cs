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

    public List<AStarPoint> obstacles = new List<AStarPoint>();

    public const int Width = 50;
    public const int Height = 50;
    public int obstacle_num;

    public AStarMap() {
        obstacle_num = 500;
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

        AddObstacle(lowMap[0, 0]);
    }

    public void AddObstacle(AStarPoint start) {


        // obstacles.Add(lowMap[4, 5]);
        // obstacles.Add(lowMap[4, 6]);
        // obstacles.Add(lowMap[4, 7]);
        // obstacles.Add(lowMap[5, 7]);
        // obstacles.Add(lowMap[6, 7]);
        // obstacles.Add(lowMap[6, 6]);
        // obstacles.Add(lowMap[6, 5]);
        // obstacles.Add(lowMap[5, 5]);

        for(int i = 0; i < obstacle_num; i++) {
            int w, h;
            w = Random.Range(0, AStarMap.Width);
            h = Random.Range(0, AStarMap.Height);
            if(lowMap[w, h] != start && !obstacles.Contains(lowMap[w, h])) {
                obstacles.Add(lowMap[w, h]);
                lowMap[w, h].IsObstacle = true;
                CreatePath(lowMap[w, h].position, Color.blue);
            }
        }

        // foreach(var obstacle in obstacles) {
        //     obstacle.IsObstacle = true;
        //     CreatePath(obstacle.position, Color.blue);
        // }
    }

    
    public void UpdateObstacle(AStarPoint start, List<AStarPoint> pathList) {
        foreach(var obstacle in obstacles) {
            obstacle.IsObstacle = false;
            GameObject.Destroy(obstacle.cube);
            obstacle.cube = null;
            if(pathList.Contains(obstacle)) {
                CreatePath(obstacle.position, Color.white);
            }
        }
        obstacles = new List<AStarPoint>();

        AddObstacle(start);
    }

    private void CreatePath(Vector2 position, Color color) {
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.transform.position = new Vector3(position.x, position.y, 0);
        cube.GetComponent<Renderer>().material.color = color;
        cube.transform.SetParent(GameObject.Find("Path").transform);
        if(lowMap[(int)position.x, (int)position.y].cube != null)
            GameObject.Destroy(lowMap[(int)position.x, (int)position.y].cube);
        lowMap[(int)position.x, (int)position.y].cube = cube;
    }
}
