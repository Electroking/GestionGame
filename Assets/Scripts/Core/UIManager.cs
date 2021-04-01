using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager instance = null;

    public UIVillagerInfos uiVillager;

    [SerializeField] Text foodText = null, woodText = null, stoneText = null;
    [SerializeField] Slider sliderTime = null;
    [SerializeField] Slider prospSlider = null;
    [SerializeField] Text textPlayPause = null;
    [SerializeField] GameObject panelWin = null;
    [SerializeField] GameObject panelGO = null;
    GameManager gm;
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
    }
    void Start()
    {
        gm = GameManager.instance;
        UpdateResources();
        InitSliderOfTime();
        InitSliderOfProsp();
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
            if (GetTerrainPointHovered(out Vector3 terrainPoint, true))
            {
                MoveSelectedBuilding(terrainPoint);
                if (leftClick)
                {
                    if (!Utils.IsPointerOverUIElement())
                    {
                        PlaceSelectedBuilding(); //if current point is not over an UI element, place the building
                    }
                }
            }
        }
        if (leftClick)
        {
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                Villager villager = GetHoveredVillager(); //get the villager using raycast
                OnVillagerClick(villager);
            }
        }

    }
    void LateUpdate()
    {
        UpdateResources();
        UpdateSliderOfTime();
        UpdateSliderOfProsp();
    }

    public void SelectBuilding(int buildingType)
    {
        if (_selectedBuilding != null)
        {
            // Deselect the selected building.
            Building.Type lastBuildingType = _selectedBuilding.type;
            DeselectBuilding(true);
            if (lastBuildingType == (Building.Type)buildingType)
            {
                // Don't reselect the building when player clicks on this building button a second time.
                return;
            }
        }
        _selectedBuilding = PoolManager.instance.SpawnBuilding((Building.Type)buildingType); //spawn a building using buildingType
        _selectedBuilding.collider.isTrigger = false;

        // Add rigidbody to the selected building to allow for collision checks.
        Rigidbody rb = _selectedBuilding.gameObject.AddComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;
        rb.WakeUp();
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
        terrainPoint = gm.GetTerrainPos(terrainPoint, _selectedBuilding.transform.GetChild(0).localScale);
        _selectedBuilding.transform.position = terrainPoint;
    }

    public void UpdatePlayPause()
    {
        textPlayPause.text = gm.IsPaused ? "Play" : "Pause";
    }

    public void UpdateResources()
    {
        foodText.text = "Food : " + gm.Food;
        woodText.text = "Wood : " + gm.Wood;
        stoneText.text = "Stone : " + gm.Stone;
    }
    void PlaceSelectedBuilding() //place the current building and deselect it
    {
        if (_selectedBuilding.Place())
        {
            DeselectBuilding();
        }
    }
    /// <summary>
    /// Get terrain point hovered by cursor.
    /// </summary>
    /// <param name="impactPoint">The terrain point hovered if any.</param>
    /// <param name="ignoreUI"></param>
    /// <returns><b>True</b> if cursor is over terrain.</returns>
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
    void InitSliderOfTime() //initialise the time of night slider (its max value and its base value)
    {
        sliderTime.maxValue = 1;
        sliderTime.value = gm.timeOfDay;
    }
    void UpdateSliderOfTime() //update the night slider using the float "timeOfNight" in gameManager
    {
        sliderTime.value = gm.timeOfDay / gm.dayLength;
    }
    void InitSliderOfProsp() //initialise the prosperity slider (its max value and its base value)
    {
        prospSlider.maxValue = 1;
        prospSlider.value = gm.Prosperity / gm.maxProsperity;
    }
    void UpdateSliderOfProsp() //update the prosperity slider using the float "prosperity" in gameManager
    {
        prospSlider.value = gm.Prosperity / gm.maxProsperity;
    }

    /// <summary>
    /// Get the villager hovered by cursor if any.
    /// </summary>
    /// <returns>The villager hovered.</returns>
    Villager GetHoveredVillager()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.tag == "Villager")
            {
                return hit.collider.GetComponentInParent<Villager>();
            }
        }
        return null;
    }

    /// <summary>
    /// Open villager's UI if villager is not null, otherwise close panel.
    /// </summary>
    /// <param name="villager"></param>
    void OnVillagerClick(Villager villager)
    {
        if (villager == null)
        {
            uiVillager.ClosePanel();
        }
        else
        {
            uiVillager.gameObject.SetActive(true);
            uiVillager.AssignVillager(villager);
        }
    }

    public void SetActiveSwitch(GameObject go)
    {
        go.SetActive(!go.activeSelf);
    }

    public void ShowVictoryPanel()
    {
        panelWin.SetActive(true);
    }

    public void ShowGameOverPanel()
    {
        panelGO.SetActive(true);
    }
}