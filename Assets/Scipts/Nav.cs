using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 挂载在每个单位身上的组件，封装寻路逻辑
/// </summary>
public class Nav : MonoBehaviour
{

    public float moveSpeed;
    // 路径坐标集合
    public List<AStarPoint> pathList;
    private AStarPoint curPoint;
    private AStarPoint endPoint;
    public bool isPathPending;

    private void Awake() {
        isPathPending = false;
    }

    private void Update() {
        Walk();
        
    }

    // 应该分散调用
    public void SetPosition(Vector2 pos) {
        pathList.Clear();

        curPoint = AStarMap.Instance.lowMap[(int)transform.position.x, (int)transform.position.y];
        endPoint = AStarMap.Instance.lowMap[(int)pos.x, (int)pos.y];
        AStar.Instance.FindPath(curPoint, endPoint);
    }

    // 每帧调用
    public void Walk() {
        if(pathList != null && pathList.Count > 0) {
            
            // 被阻挡，重新规划
            if(pathList[pathList.Count - 1].IsObstacle) {
                SetPosition(endPoint.position);
                return;
            }

            curPoint = pathList[pathList.Count - 1];
            // 组件挂载的对象移动
            // gameObject.transform.position


        }
    }



}
