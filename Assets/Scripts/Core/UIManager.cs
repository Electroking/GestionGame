using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance = null;

    Canvas _canvas;
    Building _selectedBuilding = null;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);

        _canvas = FindObjectOfType<Canvas>();
    }
    private void Update()
    {
        bool leftClick = Input.GetMouseButtonDown(0);
        bool rightClick = Input.GetMouseButtonDown(1);

        if (rightClick)
        {
            DeselectBuilding(true);
        }
        if (_selectedBuilding != null)
        {
            if (GetTerrainPointHovered(out Vector3 terrainPoint))
            {
                terrainPoint = GameManager.instance.GetBoundedPos(terrainPoint, _selectedBuilding.transform.localScale);
                MoveSelectedBuilding(terrainPoint);
                if (leftClick)
                {
                    PlaceSelectedBuilding();
                }
            }
        }
    }

    public void SelectBuilding(int buildingType)
    {
        if (_selectedBuilding != null)
        {
            Building.Type lastBuildingType = _selectedBuilding.type;
            DeselectBuilding(true);
            if (lastBuildingType == (Building.Type)buildingType)
            {
                UpdateUI();
                return;
            }
        }
        _selectedBuilding = PoolManager.instance.SpawnBuilding((Building.Type)buildingType);
        _selectedBuilding.collider.isTrigger = false;
        Rigidbody rb = _selectedBuilding.gameObject.AddComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;
        rb.WakeUp();
        UpdateUI();
    }

    public void DeselectBuilding(bool destroy = false)
    {
        if (_selectedBuilding != null)
        {
            if (destroy)
            {
                Destroy(_selectedBuilding.gameObject);
            }
            else if (_selectedBuilding.TryGetComponent(out Rigidbody rb))
            {
                _selectedBuilding.collider.isTrigger = true;
                Destroy(rb);
            }
            _selectedBuilding = null;
        }
    }

    public void MoveSelectedBuilding(Vector3 terrainPoint)
    {
        _selectedBuilding.transform.position = terrainPoint;
        _selectedBuilding.transform.Rotate(0, Input.mouseScrollDelta.y * 45, 0);
    }

    public void UpdateUI()
    {
        // change which building is selected, which are available, etc
    }
    void PlaceSelectedBuilding()
    {
        if (_selectedBuilding.Place())
        {
            DeselectBuilding();
        }
    }
    bool GetTerrainPointHovered(out Vector3 impactPoint, bool ignoreUI = false)
    {
        impactPoint = Vector3.zero;
        if (!ignoreUI && Utils.IsPointerOverUIElement()) { return false; }
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 200, 1 << LayerMask.NameToLayer(Utils.LAYER_TERRAIN)))
        {
            impactPoint = hit.point;
            return true;
        }
        return false;
    }
}