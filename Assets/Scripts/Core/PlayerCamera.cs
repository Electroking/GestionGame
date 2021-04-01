using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField] float speed = 10f, panSpeed = 1f, rotateSpeed = 1f, zMinZoom = -20, zMaxZoom = -80, camAngleMin = 30, camAngleMax = 90;
    bool _rotateIsUsed = false, _translateIsUsed = false, _panIsUsed = false;
    [SerializeField] bool allowCameraMovByScreenBorders = false;
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
        //UpdateCamAngles();
    }

    // Update is called once per frame
    void Update()
    {
        MoveCam();
    }

    void MoveCam() { //define all the if to miss different bug between rotate and translate
        if(!_panIsUsed && !_translateIsUsed) RotationMoveCam();
        if(!_rotateIsUsed && !_translateIsUsed) PanMoveCam();
        if(allowCameraMovByScreenBorders && !_rotateIsUsed && !_panIsUsed) LateralMoveCam();
        if(!_panIsUsed && !_translateIsUsed) ZoomCam();
    }

    void RotationMoveCam() {
        if(Input.GetMouseButtonDown(1)) { //if right click was just pressed
            _rotateIsUsed = true;
            _lastMousePos = Input.mousePosition;
            _angleBeforeRotate.y = transform.rotation.eulerAngles.y % 360; //catch the angle and get the value staying from -360 to 360
            _angleBeforeRotate.x = _hinge.rotation.eulerAngles.x % 360; //catch the angle and get the value staying from -360 to 360
        }
        if(Input.GetMouseButton(1)) { //if right click is being pressed
            float speed = 0.01f * rotateSpeed;
            Vector3 swivelRotate = Vector3.up * (speed * (Input.mousePosition.x - _lastMousePos.x) + _angleBeforeRotate.y); //set the swivel rotate
            Vector3 hingeRotate = Vector3.right * Mathf.Clamp(-speed * (Input.mousePosition.y - _lastMousePos.y) + _angleBeforeRotate.x, camAngleMin, camAngleMax); //set the hinge rotate and clamp it
            transform.rotation = Quaternion.Euler(swivelRotate); //use the swivel rotate
            _hinge.localRotation = Quaternion.Euler(hingeRotate); //use the hinge rotate
        }
        if(Input.GetMouseButtonUp(1)) { //if right click is released
            _rotateIsUsed = false;
            
        }
    }

    void PanMoveCam() {
        if(Input.GetMouseButtonDown(2)) { //if mousewheel is clicked
            _lastMousePos = Input.mousePosition;
            _posBeforePan = transform.position;
            _panIsUsed = true;
        }
        if(Input.GetMouseButton(2)) { //if mousewheel is clicked
            //transform.rotation = Quaternion.Euler(Vector3.up * 0.01f * rotateSpeed * (Input.mousePosition.x - _lastMousePos.x + _lastRotateY));
            transform.position = _posBeforePan - transform.rotation * new Vector3(Input.mousePosition.x - _lastMousePos.x, 0, Input.mousePosition.y - _lastMousePos.y) * panSpeed * 0.01f; //Move the camera position 
            transform.position = GameManager.instance.GetTerrainPos(transform.position); //set the position inside the terrain position
        }
        if(Input.GetMouseButtonUp(2)) { //if mousewheel is clicked
            _panIsUsed = false;
        }
    }

    void LateralMoveCam() {
        Vector3 translation = Vector3.zero;
        if(Input.mousePosition.x >= (Screen.width - 1f)) //verify if mouseX >= to the screen width -1
        {
            translation = new Vector3(2, 0, 0); //Move the cam right 2 by 2
        } else if(Input.mousePosition.x <= 1f) //verify if mouseX <= to 0 which is the min of screen width -1
        {
            translation = new Vector3(-2, 0, 0); //Move the cam left 2 by 2
        }

        if(Input.mousePosition.y >= (Screen.height - 1f)) //verify if mouseY >= to the screen width -1
        {
            translation = new Vector3(0, 0, 2); //Move the cam up 2 by 2
        } else if(Input.mousePosition.y <= 1f) //verify if mouseY <= to 0 which is the min of screen height -1
        {
            translation = new Vector3(0, 0, -2); //Move the cam down 2 by 2
        }

        if(translation != Vector3.zero) { //if it has a translation
            _translateIsUsed = true;
            transform.Translate(translation * Time.unscaledDeltaTime * speed); //Translate the camera using tranlation * speed * deltatime (unscale because of pause)
            transform.position = GameManager.instance.GetTerrainPos(transform.position); //set the position inside the terrain position
        } else {
            _translateIsUsed = false;
        }
    }

    void ZoomCam()
    {
        //zoomIsUsed = false;
        if (!_rotateIsUsed)
        {
            if (Input.mouseScrollDelta.y > 0f) //When mouseWheel is scrolled up
            {
                //zoomIsUsed = true;
                if (_stick.localPosition.z < zMinZoom)
                {
                    _stick.localPosition += new Vector3(0, 0, 5); //zoom down using stick GameObject
                }
            }
            if (Input.mouseScrollDelta.y < 0f) //When mouseWheel is scrolled down
            {
                //zoomIsUsed = true;
                if (_stick.localPosition.z > zMaxZoom)
                {
                    _stick.localPosition -= new Vector3(0, 0, 5); //zoom up using stick GameObject
                }
            }
        }
    }
}
