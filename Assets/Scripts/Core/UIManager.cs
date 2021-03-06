using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {
    Canvas _canvas;
    Building _selectedBuilding = null;
    Vector3 _mousePos;

    private void Awake() {
        _canvas = FindObjectOfType<Canvas>();
    }

    private void Update() {
        //_mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        bool leftClick = Input.GetMouseButtonDown(0);
        bool rightClick = Input.GetMouseButtonDown(1);

        if (_selectedBuilding != null) {
            if(GetTerrainPointHovered(out Vector3 terrainPoint)) {
                MoveSelectedBuilding(terrainPoint);
                if(leftClick) {
                    if (PlaceSelectedBuilding(terrainPoint)) {
                        // building placed
                    } else {
                        // building couldn't be placed
                    }
                }
            }
        }
    }

    public void SelectBuilding(Building.Type buildingType) {
        if (_selectedBuilding != null) {
            DeselectBuilding(true);
            if (_selectedBuilding.type == buildingType) {
                return;
            }
        }
        _selectedBuilding = PoolManager.instance.SpawnBuilding(buildingType);
        UpdateUI();
    }

    public void DeselectBuilding(bool discard = false) {
        if(_selectedBuilding != null && discard) {
            Destroy(_selectedBuilding.gameObject);
        }
        _selectedBuilding = null;
    }

    public void MoveSelectedBuilding(Vector3 terrainPoint) {
        _selectedBuilding.transform.position = terrainPoint;
        _selectedBuilding.transform.Rotate(0, Input.mouseScrollDelta.y * 45, 0);
    }

    public void UpdateUI() {
        // change which building is selected, which are available, etc
    }

    bool PlaceSelectedBuilding(Vector3 position) {
        bool buildSuccessful = _selectedBuilding.Build();
        DeselectBuilding();
        return buildSuccessful;
    }

    bool GetTerrainPointHovered(out Vector3 impactPoint, bool ignoreUI = false) {
        impactPoint = Vector3.zero;
        if(!ignoreUI && Utils.IsPointerOverUIElement()) { return false; }
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 200, 1 << LayerMask.NameToLayer(Utils.LAYER_TERRAIN))) {
            impactPoint = hit.point;
            return true;
        }
        return false;
    }
}