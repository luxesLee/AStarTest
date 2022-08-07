using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    public float speed;

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(KeyCode.A)) {
            Camera.main.transform.position -= speed * Time.deltaTime * Vector3.right;
        }
        if(Input.GetKey(KeyCode.D)) {
            Camera.main.transform.position += speed * Time.deltaTime * Vector3.right;
        }
        if(Input.GetKey(KeyCode.W)) {
            Camera.main.transform.position += speed * Time.deltaTime * Vector3.up;
        }
        if(Input.GetKey(KeyCode.S)) {
            Camera.main.transform.position -= speed * Time.deltaTime * Vector3.up;
        }

        if(Input.GetKey(KeyCode.Q)) {
            Camera.main.transform.position -= speed * Time.deltaTime * Vector3.forward;
        }

        if(Input.GetKey(KeyCode.E)) {
            Camera.main.transform.position += speed * Time.deltaTime * Vector3.forward;
        }
    }
}
