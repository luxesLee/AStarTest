using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cube : MonoBehaviour
{
    public delegate void pathFind(Vector2 pos);
    public pathFind FindPath;

    private void OnMouseDown() {
        if(FindPath != null) {
            FindPath(new Vector2(transform.position.x, transform.position.y));
        }    
    }
}
