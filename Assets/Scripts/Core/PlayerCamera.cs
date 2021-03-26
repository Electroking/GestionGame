using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField] float speed = 10f, rotateSpeed = 1f, zMinZoom = -20, zMaxZoom = -80;
    bool rotateIsUsed = false, translateIsUsed = false;
    float lastMousePosX, lastRotateY = 0f;
    Transform _stick;

    private void Awake() {
        _stick = transform.GetChild(0).GetChild(0).transform;
    }

    // Start is called before the first frame update
    void Start()
    {
        Mathf.Clamp(_stick.position.z, zMaxZoom, zMinZoom);
        lastMousePosX = Input.mousePosition.x;
    }

    // Update is called once per frame
    void Update()
    {
        MoveCam();
    }

    void MoveCam()
    {
        LateralMoveCam();
        RotationMoveCam();
        ZoomCam();
    }
    void LateralMoveCam()
    {
        translateIsUsed = false;
        if (!rotateIsUsed)
        {
            Vector3 translation = Vector3.zero;
            if (Input.mousePosition.x >= (Screen.width - 1f)) //verify if mouseX >= to the screen width
            {
                translation = new Vector3(2, 0, 0); //Move the cam right 2 by 2
            }
            else if (Input.mousePosition.x <= 1f) //verify if mouseX <= to 0 which is the min of screen width
            {
                translation = new Vector3(-2, 0, 0); //Move the cam left 2 by 2
            }
            if (Input.mousePosition.y >= (Screen.height - 1f)) //verify if mouseY >= to the screen width
            {
                translation = new Vector3(0, 0, 2); //Move the cam up 2 by 2
            }
            else if (Input.mousePosition.y <= 1f) //verify if mouseY <= to 0 which is the min of screen height
            {
                translation = new Vector3(0, 0, -2); //Move the cam down 2 by 2
            }
            if(translation != Vector3.zero)
            {
                translateIsUsed = true;
                transform.Translate(translation * Time.deltaTime * speed);
                transform.position = GameManager.instance.GetTerrainPos(transform.position);
            }
        }
    }
    void RotationMoveCam()
    {
        //lmpIsUsed = false;
        if (Input.GetMouseButtonDown(1))
        {
            lastMousePosX = Input.mousePosition.x;
            rotateIsUsed = true;
        }
        if (Input.GetMouseButton(1) && !translateIsUsed)
        {
            //lmpIsUsed = true;
            transform.rotation = Quaternion.Euler(Vector3.up * 0.01f * rotateSpeed * (Input.mousePosition.x - lastMousePosX + lastRotateY));
        }
        if(Input.GetMouseButtonUp(1))
        {
            rotateIsUsed = false;
            lastRotateY += Input.mousePosition.x - lastMousePosX;
        }
    }
    void ZoomCam()
    {
        //zoomIsUsed = false;
        if (!rotateIsUsed)
        {
            if (Input.mouseScrollDelta.y > 0f)
            {
                //zoomIsUsed = true;
                if (_stick.localPosition.z < zMinZoom)
                {
                    _stick.localPosition += new Vector3(0, 0, 5);
                }
            }
            if (Input.mouseScrollDelta.y < 0f)
            {
                //zoomIsUsed = true;
                if (_stick.localPosition.z > zMaxZoom)
                {
                    _stick.localPosition -= new Vector3(0, 0, 5);
                }
            }
        }
    }
}
