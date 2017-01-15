using UnityEngine;
using System.Collections;
using System;

public class TopDownFreeCamera : MonoBehaviour
{
    public float moveSpeed = 1f;
    public float turnSpeed = 1f;
    public float zoomSpeed = 1f;

    private Vector2 mousePos;
    private Vector2 screenSize;

    private GameObject pivot;

    // Use this for initialization
    void Start ()
    {
        mousePos = Vector2.zero;
        screenSize.x = Screen.width;
        screenSize.y = Screen.height;

        pivot = new GameObject("Camera Pivot");

        Ray ray = Camera.main.ScreenPointToRay(screenSize/2);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
            pivot.transform.position = hit.point;

        transform.parent = pivot.transform;
    }
	
	// Update is called once per frame
	void Update ()
    {
        getMousePos();

        if (!GameManager.menuOpen) // dominogame specific line
        {
            moveCamera();
            turnCamera();
            zoomCamera();
        }
    }

    private void getMousePos()
    {
        mousePos = Input.mousePosition;
    }

    private void moveCamera()
    {
        if(mousePos.x < 5)
        {
            transform.parent.Translate(-1f * moveSpeed, 0, 0);
        }
        else if(mousePos.x > screenSize.x -5)
        {
            transform.parent.Translate(1f * moveSpeed, 0, 0);
        }

        if (mousePos.y < 5)
        {
            transform.parent.Translate(0, 0, -1f * moveSpeed);
        }
        else if (mousePos.y > screenSize.y - 5)
        {
            transform.parent.Translate(0, 0, 1f * moveSpeed);
        }
    }

    private void turnCamera()
    {
        if(GetInput.middleMousePressed && !GetInput.shiftKeyPressed && !GetInput.ctrlKeyPressed)
        {
            float rotationAmount = Input.GetAxisRaw("Mouse X");
            Vector3 rotation = transform.parent.rotation.eulerAngles;

            transform.parent.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(rotation.x, rotationAmount * turnSpeed + rotation.y, rotation.z), 1);
        }
    }

    private void zoomCamera()
    {
        if (!GetInput.shiftKeyPressed && !GetInput.ctrlKeyPressed)
        {
            float zoomAmount = Input.GetAxisRaw("Mouse ScrollWheel");
            if (zoomAmount != 0)
            {
                if (zoomAmount < 0)
                {
                    transform.Translate(0, 0, -1.0f + zoomAmount * zoomSpeed);
                }
                else if (zoomAmount > 0)
                {
                    transform.Translate(0, 0, 1.0f + zoomAmount * zoomSpeed);
                }
            }
        }
    }
}
