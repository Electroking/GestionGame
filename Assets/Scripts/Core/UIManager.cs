using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager instance = null;

    //Canvas _canvas;
    Building _selectedBuilding = null;
    [SerializeField] Text foodText = null, woodText = null, stoneText = null;
    GameManager gm;
    //[SerializeField] PopUpInfo[] popUpPanel;
    [SerializeField] Slider sliderTime;
    [SerializeField] Slider prospSlider;
    public UIVillagerInfos uiVillager;

    [SerializeField]
    GameObject panelWin;

    [SerializeField]
    GameObject panelGO;
    float dayLenght, timeOfDay, prosp, maxProsp=100;
    //int i = 0;

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

        //_canvas = FindObjectOfType<Canvas>();
    }
    void Start()
    {
        gm = GameManager.instance;
        UpdateUI();
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
                        PlaceSelectedBuilding();
                    }
                }
            }
        }
        if (leftClick)
        {
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                Villager villager = CheckIfOverVillager();
                OnVillagerClick(villager);
                /*if ((villager = CheckIfOverVillager()) != null)
                {
                    OnVillagerClick(villager);
                }
                else
                {
                    DeselectVillager();
                }*/
            }
        }

    }
    void LateUpdate()
    {
        UpdateUI();
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
                return;
            }
        }
        _selectedBuilding = PoolManager.instance.SpawnBuilding((Building.Type)buildingType);
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
        //_selectedBuilding.transform.Rotate(0, Input.mouseScrollDelta.y * 45, 0);
    }

    public void UpdateUI()
    {
        foodText.text = "Food : " + gm.Food;
        woodText.text = "Wood : " + gm.Wood;
        stoneText.text = "Stone : " + gm.Stone;
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

    /*public void SpawnPopUpInfo(int numType)
    {
        switch (numType)
        {
            case 0:
                i = 0; //House
                break;
            case 1:
                i = 1; //School
                break;
            case 2:
                i = 2; //farm
                break;
            case 3:
                i = 3; //Museum
                break;
            case 4:
                i = 4; //Library
                break;
        }
        popUpPanel[i].gameObject.SetActive(true);
        popUpPanel[i].SetText(i);
    }
    public void ClosePopUpInfo()
    {
        popUpPanel[i].gameObject.SetActive(false);
    }*/

    void InitSliderOfTime()
    {
        //dayLenght = gm.dayLength;
        //timeOfDay = GameManager.instance.timeOfDay;
        sliderTime.maxValue = 1;
        sliderTime.value = gm.timeOfDay;
    }
    void UpdateSliderOfTime()
    {
        //timeOfDay = GameManager.instance.timeOfDay;
        sliderTime.value = gm.timeOfDay / gm.dayLength;
    }
    void InitSliderOfProsp()
    {
        //prosp = GameManager.instance.Prosperity;
        //maxProsp = GameManager.instance.maxProsperity;
        prospSlider.maxValue = 1;
        prospSlider.value = gm.Prosperity / gm.maxProsperity;
    }
    void UpdateSliderOfProsp()
    {
        //prosp = GameManager.instance.Prosperity;
        prospSlider.value = gm.Prosperity / gm.maxProsperity;
    }

    Villager CheckIfOverVillager()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.tag == "Villager")
            {
                //jobMenu.SetActive(true);
                return hit.collider.GetComponentInParent<Villager>();
            }
        }
        return null;
    }

    void OnVillagerClick(Villager villager)
    {
        if (villager == null) {
            uiVillager.ClosePanel();
        } else {
            uiVillager.gameObject.SetActive(true);
            uiVillager.AssignVillager(villager);
        }
    }

    /*void DeselectVillager()
    {
        if (uiVillager.villager != null) {
            uiVillager.villager.OnSelect(false);
        }
        uiVillager.gameObject.SetActive(false);
        uiVillager.gameObject.transform.GetChild(2).gameObject.SetActive(false);
    }*/

    public void SetActiveSwitch(GameObject go)
    {
        go.SetActive(!go.activeSelf);
    }

   public void Victory()
    {
        if (GameManager.instance.Prosperity >= maxProsp)
        {
            panelWin.SetActive(true);
            Time.timeScale = 0;
        }
    }

   public void GameOver()
    {
        if (Villager.list.Count == 0)
        {
            panelGO.SetActive(true);
            Time.timeScale = 0;
        }
    }

    public void ReloadScene()
    {
        SceneManager.LoadScene(0);
        Time.timeScale = 1;
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}