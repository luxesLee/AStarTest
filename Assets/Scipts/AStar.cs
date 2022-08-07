using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStar
{
    private static AStar instance;
    public static AStar Instance {
        get {
            if(instance == null) {
                instance = new AStar();
            }
            return instance;
        }
    }

    public int Width = 10, Height = 10;
    private Vector2[] directions = new Vector2[8] {
        new Vector2(-1, 0), new Vector2(1, 0), new Vector2(0, 1), new Vector2(1, 0), new Vector2(-1, -1), new Vector2(-1, 1), new Vector2(1, 1), new Vector2(1, -1)
        };

    public AStarPoint[ , ] pointGrid;

    public List<AStarPoint> pathList = new List<AStarPoint>();
    public List<AStarPoint> obstacles = new List<AStarPoint>();

    public AStar() {
        Init();
    }

    private void Init() {

        pointGrid = new AStarPoint[10 , 10];
        for(int i = 0; i < 10; i++) {
            for(int j = 0; j < 10; j++) {
                pointGrid[i, j] = new AStarPoint(i, j);
            }
        }
        AddObstacle();
    }

    private void AddObstacle() {
        obstacles.Add(pointGrid[4, 2]);
        obstacles.Add(pointGrid[4, 3]);
        obstacles.Add(pointGrid[4, 4]);
        obstacles.Add(pointGrid[4, 5]);
        obstacles.Add(pointGrid[4, 6]);

        foreach(var obstacle in obstacles) {
            obstacle.IsObstacle = true;
            CreatePath(obstacle.position, Color.blue);
        }
    }



    // 寻路
    public List<AStarPoint> FindPath(AStarPoint start, AStarPoint end) {
        if(end.IsObstacle || start == end) {
            return null;
        }

        List<AStarPoint> openList = new List<AStarPoint>();
        List<AStarPoint> closeList = new List<AStarPoint>();

        openList.Add(start);
        while(openList.Count > 0) {
            AStarPoint curPoint = GetPointWithMinF(openList);
            openList.Remove(curPoint);
            closeList.Add(curPoint);
            
            List<AStarPoint> surroundPoints = GetSurroundPoints(curPoint, closeList);

            foreach(var surroundPoint in surroundPoints) {
                if(openList.Contains(surroundPoint)) {
                    float new_G = calcG(surroundPoint, curPoint);
                    // 如果由curPoint到surroundPoint的G更小
                    if(new_G < surroundPoint.G) {
                        surroundPoint.G = new_G;
                        surroundPoint.F = surroundPoint.G + surroundPoint.H;
                        surroundPoint.parent = curPoint;
                    }
                }
                else {
                    openList.Add(surroundPoint);
                    surroundPoint.parent = curPoint;
                    calcF(surroundPoint, end);
                }
            }
            
            if(openList.Contains(end)) {
                break;
            }
        }

        GetPath(start, end);

        return pathList;
    }

    // 获取周围点，有障碍和已加入关闭集合的除外
    private List<AStarPoint> GetSurroundPoints(AStarPoint point, List<AStarPoint> clostList) {
        List<AStarPoint> surroundPoints = new List<AStarPoint>();
        foreach(var direction in directions) {
            int cur_x = (int)(point.position.x + direction.x);
            int cur_y = (int)(point.position.y + direction.y);
            if(cur_x >= 0 && cur_x < Width && cur_y >= 0 && cur_y < Height) {
                AStarPoint surroundPoint = pointGrid[cur_x, cur_y];
                if(!surroundPoint.IsObstacle && !clostList.Contains(surroundPoint)) {
                    surroundPoints.Add(surroundPoint);
                }
            }
        }
        return surroundPoints;
    }

    private AStarPoint GetPointWithMinF(List<AStarPoint> openList) {
        float val = float.MaxValue;
        AStarPoint ans = null;
        foreach(AStarPoint point in openList) {
            if(point.F < val) {
                val = point.F;
                ans = point;
            }
        }
        return ans;
    }

    // 依靠parent由end回溯找到start，结果保存在pathList中
    private void GetPath(AStarPoint start, AStarPoint end) {
        pathList.Clear();

        AStarPoint cur = end;
        // 依靠parent由end回溯找到start
        while(true) {
            pathList.Add(cur);

            Color color = Color.white;
            if(cur == start) color = Color.green;
            else if(cur == end) color = Color.red;

            CreatePath(cur.position, color);
            
            if(cur == start) break;
            cur = cur.parent;
        }
    }

    private void CreatePath(Vector2 position, Color color) {
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.transform.position = new Vector3(position.x, position.y, 0);
        cube.GetComponent<Renderer>().material.color = color;
        cube.transform.SetParent(GameObject.Find("Path").transform);
        if(pointGrid[(int)position.x, (int)position.y].cube != null)
            GameObject.Destroy(pointGrid[(int)position.x, (int)position.y].cube);
        pointGrid[(int)position.x, (int)position.y].cube = cube;
    }


    // G
    private float calcG(AStarPoint surroundPoint, AStarPoint curPoint) {
        return Vector3.Distance(surroundPoint.position, curPoint.position) + curPoint.G;
    }

    // H
    private float calcH(AStarPoint now, AStarPoint end) {
        return Mathf.Abs(end.position.x - now.position.x) + Mathf.Abs(end.position.y - now.position.y);
    }

    // F
    private void calcF(AStarPoint now, AStarPoint end) {
        now.H = calcH(now, end);
        if(now.parent == null) {    // start无parent
            now.G = 0;
        }
        else {
            now.G = calcG(now, now.parent);
        }
        now.F = now.H + now.G;
    }


}
