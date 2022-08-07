using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class TestAStar : MonoBehaviour
{
    private Transform cubeParent;
    private AStarPoint[,] pointGrid;
    private AStarPoint startPoint;
    private AStarPoint endPoint;

    private float mTime = 0.7f;
    private float mTimer = 0f;

    private float oTime = 2f;
    private float oTimer = 0f;

    // Start is called before the first frame update
    void Start()
    {
        cubeParent = GameObject.Find("Root").transform;
        pointGrid = AStar.Instance.map.lowMap;
        startPoint = pointGrid[0, 0];
        InitBackGround();
    }

    // Update is called once per frame
    void Update()
    {
        mTimer += Time.deltaTime;
        oTimer += Time.deltaTime;
        if(oTimer >= oTime) {
            AStar.Instance.UpdateObstacle(startPoint);
            oTimer = 0;
        }

        if(mTimer >= mTime) {
            mTimer = 0;
            Walk();
        }
    }

    private void InitBackGround() {
        for(int i = 0; i < AStarMap.Width; i++) {
            for(int j = 0; j < AStarMap.Height; j++) {
                CreateGrid(new Vector2(i, j), Color.gray);
            }
        }

    }


    private void CreateGrid(Vector2 position, Color color) {
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.transform.position = new Vector3(position.x, position.y, 0);
        cube.transform.localScale = new Vector3(0.9f, 0.9f, 0.9f);
        cube.GetComponent<Renderer>().material.color = color;
        cube.transform.SetParent(cubeParent);
        cube.AddComponent<Cube>().FindPath = FindPath;
    }

    private void Walk() {
        if(AStar.Instance.pathList != null && AStar.Instance.pathList.Count > 1) {

            startPoint = AStar.Instance.pathList[AStar.Instance.pathList.Count - 1];
            Color color = startPoint.cube.GetComponent<Renderer>().material.color;
            AStar.Instance.pathList.Remove(startPoint);
            Destroy(startPoint.cube);
            startPoint.cube = null;

            // 如果行走过程中路径被挡住，则重新规划
            if(AStar.Instance.pathList[AStar.Instance.pathList.Count - 1].IsObstacle) {
                UnityEngine.Debug.Log("reFindPath");
                FindPath(endPoint.position);
                return;
            }

            startPoint = AStar.Instance.pathList[AStar.Instance.pathList.Count - 1];
            startPoint.cube.GetComponent<Renderer>().material.color = color;
        }
    }

    public void FindPath(Vector2 target) {
        // AStar.Instance.

        AStar.Instance.ClearGrid();

        endPoint = pointGrid[(int)target.x, (int)target.y];

        Stopwatch sw = new Stopwatch();
        sw.Start();
        AStar.Instance.FindPath(startPoint, endPoint);
        sw.Stop();
        UnityEngine.Debug.Log(sw.ElapsedMilliseconds);
    }
}
