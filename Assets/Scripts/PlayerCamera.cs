using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField] float speed = 10f, zoomSpeed = 5f, panSpeed = 1f, rotateSpeed = 1f, nearestZoom = -20, farthestZoom = -80, camAngleMin = 30, camAngleMax = 90;
    [SerializeField] bool allowCameraMovByScreenBorders = false;

    bool _rotateIsUsed = false, _translateIsUsed = false, _panIsUsed = false;
    Vector3 _lastMousePos, _posBeforePan, _angleBeforeRotate;
    Transform _hinge, _stick;

    private void Awake()
    {
        _hinge = transform.GetChild(0);
        _stick = _hinge.GetChild(0);
    }

    void Start()
    {
        Mathf.Clamp(_stick.position.z, farthestZoom, nearestZoom);
    }

    void Update()
    {
        MoveCam();
    }

    void MoveCam()
    {
        if (!_panIsUsed && !_translateIsUsed) RotationMoveCam();
        if (!_rotateIsUsed && !_translateIsUsed) PanMoveCam();
        if (allowCameraMovByScreenBorders && !_rotateIsUsed && !_panIsUsed) LateralMoveCam();
        if (!_panIsUsed && !_translateIsUsed) ZoomCam();
    }

    void RotationMoveCam()
    {
        if (Input.GetMouseButtonDown(1))
        {
            _rotateIsUsed = true;
            _lastMousePos = Input.mousePosition;
            _angleBeforeRotate.y = transform.rotation.eulerAngles.y % 360; //store the angle and make the value stay between -360 and 360.
            _angleBeforeRotate.x = _hinge.rotation.eulerAngles.x % 360; //store the angle and make the value stay between -360 and 360.
        }
        if (Input.GetMouseButton(1))
        {
            float speed = 0.01f * rotateSpeed;
            Vector3 swivelRotation = Vector3.up * (speed * (Input.mousePosition.x - _lastMousePos.x) + _angleBeforeRotate.y); //set the swivel rotation
            Vector3 hingeRotation = Vector3.right * Mathf.Clamp(-speed * (Input.mousePosition.y - _lastMousePos.y) + _angleBeforeRotate.x, camAngleMin, camAngleMax); //set the hinge rotation and clamp it
            transform.rotation = Quaternion.Euler(swivelRotation);
            _hinge.localRotation = Quaternion.Euler(hingeRotation);
        }
        if (Input.GetMouseButtonUp(1))
        {
            _rotateIsUsed = false;
        }
    }

    void PanMoveCam()
    {
        if (Input.GetMouseButtonDown(2))
        {
            _lastMousePos = Input.mousePosition;
            _posBeforePan = transform.position;
            _panIsUsed = true;
        }
        if (Input.GetMouseButton(2))
        {
            transform.position = _posBeforePan - transform.rotation * new Vector3(Input.mousePosition.x - _lastMousePos.x, 0, Input.mousePosition.y - _lastMousePos.y) * panSpeed * 0.01f; //Move the camera position 
            transform.position = GameManager.instance.GetTerrainPos(transform.position); //set the position inside the terrain position
        }
        if (Input.GetMouseButtonUp(2))
        {
            _panIsUsed = false;
        }
    }

    void LateralMoveCam()
    {
        Vector3 translation = Vector3.zero;
        if (Input.mousePosition.x >= (Screen.width - 1f)) //verify if mouseX >= to the screen width -1
        {
            translation = Vector3.right;
        }
        else if (Input.mousePosition.x <= 1f) //verify if mouseX <= to 0 which is the min of screen width -1
        {
            translation = Vector3.left;
        }

        if (Input.mousePosition.y >= (Screen.height - 1f)) //verify if mouseY >= to the screen width -1
        {
            translation = Vector3.forward;
        }
        else if (Input.mousePosition.y <= 1f) //verify if mouseY <= to 0 which is the min of screen height -1
        {
            translation = Vector3.back;
        }

        if (translation != Vector3.zero)
        { //if it has a translation
            _translateIsUsed = true;
            transform.Translate(translation * Time.unscaledDeltaTime * speed); //Translate the camera using tranlation * speed * deltatime (unscale because of pause)
            transform.position = GameManager.instance.GetTerrainPos(transform.position); //set the position inside the terrain position
        }
        else
        {
            _translateIsUsed = false;
        }
    }

    void ZoomCam()
    {
        if (Input.mouseScrollDelta.y != 0f) //When mouseWheel is used.
        {
            float zoom = _stick.localPosition.z + Input.mouseScrollDelta.y * zoomSpeed * Time.deltaTime; //zoom down using stick GameObject
            _stick.localPosition = new Vector3(_stick.localPosition.x, _stick.localPosition.y, Mathf.Clamp(zoom, farthestZoom, nearestZoom));
        }
    }
}
