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

    private Vector2[] directions = new Vector2[8] {
        new Vector2(-1, 0), new Vector2(1, 0), new Vector2(0, 1), new Vector2(1, 0), new Vector2(-1, -1), new Vector2(-1, 1), new Vector2(1, 1), new Vector2(1, -1)
        };

    // public AStarPoint[ , ] pointGrid;
    public AStarMap map;

    // 这个应该每个角色携带
    // 由终点到起点
    public List<AStarPoint> pathList = new List<AStarPoint>();
    private List<AStarPoint> pathHighList = new List<AStarPoint>();
    public List<AStarPoint> obstacles = new List<AStarPoint>();

    public AStar() {
        Init();
    }

    private void Init() {


        map = AStarMap.Instance;

        AddObstacle(map.lowMap[0, 0]);
    }

    private void AddObstacle(AStarPoint start) {

        for(int i = 0; i < 500; i++) {
            int w, h;
            w = Random.Range(0, AStarMap.Width);
            h = Random.Range(0, AStarMap.Height);
            if(map.lowMap[w, h] != start && !obstacles.Contains(map.lowMap[w, h])) {
                obstacles.Add(map.lowMap[w, h]);
            }
        }

        foreach(var obstacle in obstacles) {
            obstacle.IsObstacle = true;
            CreatePath(obstacle.position, Color.blue);
            // GetThePointInHighMap(obstacle).IsObstacle = true;
        }
    }

    public void UpdateObstacle(AStarPoint start) {
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


    public List<AStarPoint> FindPath(AStarPoint start, AStarPoint end) {
        
        // 如果起点终点在一个大格子内部
        // if(GetThePointInHighMap(start) == GetThePointInHighMap(end)) {
            return FindPathInLowMap(start, end, false, null);
        // }
        // else {
        //     return FindPathInHighMap(start, end);
        // }
    }

    /// <summary>
    /// 寻路底层实现
    /// </summary>
    /// <param name="start">起点</param>
    /// <param name="end">终点</param>
    /// <param name="isOverride">是否作用于大格</param>
    /// <param name="mask">掩码</param>
    /// <returns></returns>
    private List<AStarPoint> FindPathInLowMap(AStarPoint start, AStarPoint end, bool isOverride, List<AStarPoint> mask) {
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
            
            List<AStarPoint> surroundPoints = GetSurroundPoints(curPoint, closeList, isOverride, mask);

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

        // 目标不可达
        if(!openList.Contains(end)) {
            Debug.Log("target cannot get");
            pathList = new List<AStarPoint>();
            return pathList;
        }

        List<AStarPoint> path = GetPath(start, end, isOverride);
        if(!isOverride)
            pathList = path;
        return path;
    }

    // 寻路——大格
    private List<AStarPoint> FindPathInHighMap(AStarPoint start, AStarPoint end) {
        
        // 先找出大格怎么走
        AStarPoint s = GetThePointInHighMap(start), e = GetThePointInHighMap(end);
        List<AStarPoint> highPath = FindPathInLowMap(s, e, true, null);

        // 用大格当作掩码，小格必须处于大格中
        List<AStarPoint> path = FindPathInLowMap(start, end, false, highPath);
        return path;
    }

    private AStarPoint GetThePointInHighMap(AStarPoint point) {
        int x = (int)point.position.x / 10, y = (int)point.position.y / 10;
        return map.highMap[x, y];
    }

    // 获取周围点，有障碍和已加入关闭集合的除外
    private List<AStarPoint> GetSurroundPoints(AStarPoint point, List<AStarPoint> clostList, bool isHigh, List<AStarPoint> mask) {
        List<AStarPoint> surroundPoints = new List<AStarPoint>();
        foreach(var direction in directions) {

            int cur_x = (int)(point.position.x + direction.x);
            int cur_y = (int)(point.position.y + direction.y);
            int Width = (isHigh ? map.highMap.GetLength(0) : map.lowMap.GetLength(0));
            int Height = (isHigh ? map.highMap.GetLength(1) : map.lowMap.GetLength(1));

            // 防止越界
            if(cur_x >= 0 && cur_x < Width && cur_y >= 0 && cur_y < Height) {
                if(!isHigh && mask != null && !mask.Contains(GetThePointInHighMap(map.lowMap[cur_x, cur_y]))) {
                    continue;
                }

                AStarPoint surroundPoint = (isHigh ? map.highMap[cur_x, cur_y] : map.lowMap[cur_x, cur_y]);
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
    private List<AStarPoint> GetPath(AStarPoint start, AStarPoint end, bool isHigh) {
        List<AStarPoint> path = new List<AStarPoint>();

        AStarPoint cur = end;
        // 依靠parent由end回溯找到start
        while(true) {
            path.Add(cur);

            if(!isHigh) {
                Color color = Color.white;
                if(cur == start) color = Color.green;
                else if(cur == end) color = Color.red;

                CreatePath(cur.position, color);
            }
            
            if(cur == start) break;
            cur = cur.parent;
        }
        return path;
    }

    // 给抽象的AStarPoint添颜色
    private void CreatePath(Vector2 position, Color color) {
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.transform.position = new Vector3(position.x, position.y, 0);
        cube.GetComponent<Renderer>().material.color = color;
        cube.transform.SetParent(GameObject.Find("Path").transform);
        if(map.lowMap[(int)position.x, (int)position.y].cube != null)
            GameObject.Destroy(map.lowMap[(int)position.x, (int)position.y].cube);
        map.lowMap[(int)position.x, (int)position.y].cube = cube;
    }

    public void ClearGrid() {
        for(int i = 0; i < map.lowMap.GetLength(0); i++) {
            for(int j = 0; j < map.lowMap.GetLength(1); j++) {
                if(!map.lowMap[i, j].IsObstacle && map.lowMap[i, j].cube != null) {
                    GameObject.Destroy(map.lowMap[i, j].cube);
                    map.lowMap[i, j].cube = null;
                    map.lowMap[i, j].parent = null;
                }
            }
        }
    }

    #region calculate

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

    #endregion

}
