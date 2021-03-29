using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField] float speed = 10f, panSpeed = 1f, rotateSpeed = 1f, zMinZoom = -20, zMaxZoom = -80, camAngleMin = 30, camAngleMax = 90;
    bool _rotateIsUsed = false, _translateIsUsed = false, _panIsUsed = false;
    //float _angleBeforeRotate = 0f;
    Vector3 _lastMousePos, _posBeforePan, _angleBeforeRotate;
    Transform _hinge, _stick;

    private void Awake() {
        _hinge = transform.GetChild(0);
        _stick = _hinge.GetChild(0);
    }

    // Start is called before the first frame update
    void Start()
    {
        Mathf.Clamp(_stick.position.z, zMaxZoom, zMinZoom);
        //_lastMousePos = Input.mousePosition;
        UpdateCamAngles();
    }

    // Update is called once per frame
    void Update()
    {
        MoveCam();
    }

    void MoveCam() {
        if(!_panIsUsed && !_translateIsUsed) RotationMoveCam();
        if(!_rotateIsUsed && !_translateIsUsed) PanMoveCam();
        if(!_rotateIsUsed && !_panIsUsed) LateralMoveCam();
        if(!_panIsUsed && !_translateIsUsed) ZoomCam();
    }

    void PanMoveCam() {
        //lmpIsUsed = false;
        if(Input.GetMouseButtonDown(2)) {
            _lastMousePos = Input.mousePosition;
            _posBeforePan = transform.position;
            _panIsUsed = true;
        }
        if(Input.GetMouseButton(2)) {
            //lmpIsUsed = true;
            //transform.rotation = Quaternion.Euler(Vector3.up * 0.01f * rotateSpeed * (Input.mousePosition.x - _lastMousePos.x + _lastRotateY));
            transform.position = _posBeforePan - transform.rotation * new Vector3(Input.mousePosition.x - _lastMousePos.x, 0, Input.mousePosition.y - _lastMousePos.y) * panSpeed * 0.01f;
        }
        if(Input.GetMouseButtonUp(2)) {
            _panIsUsed = false;
        }
    }

    void LateralMoveCam() {
        Vector3 translation = Vector3.zero;
        if(Input.mousePosition.x >= (Screen.width - 1f)) //verify if mouseX >= to the screen width
        {
            translation = new Vector3(2, 0, 0); //Move the cam right 2 by 2
        } else if(Input.mousePosition.x <= 1f) //verify if mouseX <= to 0 which is the min of screen width
        {
            translation = new Vector3(-2, 0, 0); //Move the cam left 2 by 2
        }

        if(Input.mousePosition.y >= (Screen.height - 1f)) //verify if mouseY >= to the screen width
        {
            translation = new Vector3(0, 0, 2); //Move the cam up 2 by 2
        } else if(Input.mousePosition.y <= 1f) //verify if mouseY <= to 0 which is the min of screen height
        {
            translation = new Vector3(0, 0, -2); //Move the cam down 2 by 2
        }

        if(translation != Vector3.zero) {
            _translateIsUsed = true;
            transform.Translate(translation * Time.deltaTime * speed);
            transform.position = GameManager.instance.GetTerrainPos(transform.position);
        } else {
            _translateIsUsed = false;
        }
    }

    void RotationMoveCam()
    {
        //lmpIsUsed = false;
        if (Input.GetMouseButtonDown(1))
        {
            _lastMousePos = Input.mousePosition;
            _rotateIsUsed = true;
        }
        if (Input.GetMouseButton(1))
        {
            //lmpIsUsed = true;
            Debug.Log(_angleBeforeRotate.x + " | " + _angleBeforeRotate.y);
            Vector3 swivelRotate = Vector3.up * (0.01f * rotateSpeed * (Input.mousePosition.x - _lastMousePos.x) + _angleBeforeRotate.y);
            Vector3 hingeRotate = Vector3.right * Mathf.Clamp(0.01f * rotateSpeed * -(Input.mousePosition.y - _lastMousePos.y) + _angleBeforeRotate.x, camAngleMin, camAngleMax);
            Debug.Log(hingeRotate + " | " + swivelRotate);
            transform.rotation = Quaternion.Euler(swivelRotate);
            _hinge.localRotation = Quaternion.Euler(hingeRotate);
            Debug.Log(_hinge.rotation.eulerAngles + " | " + transform.rotation.eulerAngles);
        }
        if(Input.GetMouseButtonUp(1))
        {
            _rotateIsUsed = false;
            UpdateCamAngles();
        }
    }

    void ZoomCam()
    {
        //zoomIsUsed = false;
        if (!_rotateIsUsed)
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

    void UpdateCamAngles() {
        _angleBeforeRotate.y = transform.rotation.eulerAngles.y % 360;
        _angleBeforeRotate.x = _hinge.rotation.eulerAngles.x % 360;
    }
}
