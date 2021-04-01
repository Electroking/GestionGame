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

    //Canvas _canvas;
    [SerializeField] Text foodText = null, woodText = null, stoneText = null;
    //[SerializeField] PopUpInfo[] popUpPanel;
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
        if (leftClick) //if left mouse button is clicked
        {
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                Villager villager = CheckIfOverVillager(); //get the villager using raycast
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
            Building.Type lastBuildingType = _selectedBuilding.type;
            DeselectBuilding(true);
            if (lastBuildingType == (Building.Type)buildingType)
            {
                return; //change the building type
            }
        }
        _selectedBuilding = PoolManager.instance.SpawnBuilding((Building.Type)buildingType); //spawn a building using type changed before
        _selectedBuilding.collider.isTrigger = false;
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
                Destroy(_selectedBuilding.gameObject); //destroy the selected building
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

    public void UpdatePlayPause() {
        textPlayPause.text = gm.IsPaused ? "Play" : "Pause";
    }

    public void UpdateResources()
    {
        foodText.text = "Food : " + gm.Food;
        woodText.text = "Wood : " + gm.Wood;
        stoneText.text = "Stone : " + gm.Stone;
    }
    void PlaceSelectedBuilding() //place the current building with deselect function
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
            impactPoint = hit.point; //get the clicked point on the terrain using raycast
            return true; //and ignore ui if there is an impact point
        }
        return false;
    }
    void InitSliderOfTime() //initialise the time of night slider (it's max value and it's base value)
    {
        sliderTime.maxValue = 1;
        sliderTime.value = gm.timeOfDay;
    }
    void UpdateSliderOfTime() //update the time of the night using the float "timeOfNight" in gameManager
    {
        sliderTime.value = gm.timeOfDay / gm.dayLength;
    }
    void InitSliderOfProsp() //initialise the prosperity slider (it's max value and it's base value)
    {
        prospSlider.maxValue = 1;
        prospSlider.value = gm.Prosperity / gm.maxProsperity;
    }
    void UpdateSliderOfProsp() //update the prosperity value using the float "prosperity" in gameManager
    {
        prospSlider.value = gm.Prosperity / gm.maxProsperity;
    }

    Villager CheckIfOverVillager()
    {
        RaycastHit hit; // use a raycast to get a villager
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.tag == "Villager")
            {
                return hit.collider.GetComponentInParent<Villager>(); //return the clicked villager
            }
        }
        return null; //if miss return null
    }

    void OnVillagerClick(Villager villager)
    {
        if (villager == null) { //if it's the second click, close the panel
            uiVillager.ClosePanel();
        } else {
            uiVillager.gameObject.SetActive(true); //if it's the first, open it
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