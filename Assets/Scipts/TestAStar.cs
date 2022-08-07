using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestAStar : MonoBehaviour
{
    private Transform cubeParent;
    private AStarPoint[,] pointGrid;
    private AStarPoint startPoint;

    private float mTime = 0.7f;
    private float mTimer = 0f;

    // Start is called before the first frame update
    void Start()
    {
        cubeParent = GameObject.Find("Root").transform;
        pointGrid = AStar.Instance.pointGrid;
        startPoint = pointGrid[0, 0];
        InitBackGround();
    }

    // Update is called once per frame
    void Update()
    {
        mTimer += Time.deltaTime;
        if(mTimer >= mTime) {
            mTimer = 0;
            Walk();
        }
    }

    private void InitBackGround() {
        for(int i = 0; i < 10; i++) {
            for(int j = 0; j < 10; j++) {
                CreatePath(new Vector2(i, j), Color.gray);
            }
        }
    }

    private void CreatePath(Vector2 position, Color color) {
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.transform.position = new Vector3(position.x, position.y, 0);
        cube.transform.localScale = new Vector3(0.9f, 0.9f, 0.9f);
        cube.GetComponent<Renderer>().material.color = color;
        cube.transform.SetParent(cubeParent);
        
    }

    private void Walk() {
        
    }

    public void FindPath() {

    }
}
