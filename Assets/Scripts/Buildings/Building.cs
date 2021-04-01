using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class Building : MonoBehaviour
{
    public static List<Building> unbuiltList = new List<Building>();

    [HideInInspector] public new Collider collider;
    [HideInInspector] public List<Villager> builders = new List<Villager>();
    [HideInInspector] public bool isPlaced = false;
    [HideInInspector] public bool isBuilt = false;
    public Type type;
    public int resourceS;
    public int resourceW;

    float Progression
    {
        get { return _progression; }
        set
        {
            _progression = Mathf.Clamp(value, 0, maxProgression);
            _model.transform.localPosition = Vector3.up * (2 * _progression / maxProgression - 1);
        }
    }
    [SerializeField] float maxProgression = 10;
    float _progression = 0;
    NavMeshObstacle _obstacle;
    GameObject _model;

    public enum Type
    {
        House = 0, School = 1, Farm = 2, Museum = 4, Library = 3
    }

    private void Awake()
    {
        _model = transform.GetChild(1).gameObject;
        _obstacle = GetComponent<NavMeshObstacle>();
        _obstacle.enabled = false;
        collider = GetComponentInChildren<Collider>();
        for (int i = 0; i < transform.childCount; i++) // set the tag of a game object to all his children
        {
            transform.GetChild(i).gameObject.tag = tag;
        }
    }

    /// <summary>
    /// Place the Building on the terrain.
    /// </summary>
    /// <param name="ignoreConditions">If the Building needs to be placed without checking for the condition nor expending resources.</param>
    /// <returns><b>True</b> if the building was placed successfully.</returns>
    public bool Place(bool ignoreConditions = false)
    {
        if (!ignoreConditions)
        {
            if (!CheckIfCanBePlaced()) { return false; }
            if (!CheckifEnoughResources()) { return false; }
            ExpendResources();
        }
        unbuiltList.Add(this);
        _model.transform.localPosition = -Vector3.up;
        _obstacle.enabled = true;
        return isPlaced = true;
    }

    public void BuildInstant() => Build(maxProgression);

    /// <summary>
    /// Progress the contruction of a building.
    /// </summary>
    /// <param name="amount"> Amount of progression.</param>
    public void Build(float amount)
    {
        if (isBuilt) { return; }
        Progression += amount;
        if (Progression >= maxProgression)
        {
            isBuilt = true;
            unbuiltList.Remove(this);
            OnBuilt();
        }
    }

    /// <summary>
    /// Spend the resources needed to build the building.
    /// </summary>
    private void ExpendResources()
    {
        GameManager.instance.Stone -= resourceS;
        GameManager.instance.Wood -= resourceW;
    }

    private bool CheckifEnoughResources()
    {
        return GameManager.instance.Stone >= resourceS && GameManager.instance.Wood >= resourceW;
    }

    protected abstract void OnBuilt();

    bool CheckIfCanBePlaced()
    {
        Collider[] colliders = Physics.OverlapBox(transform.position, collider.transform.localScale * 0.5f, transform.rotation);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].tag == "Village" && colliders[i] != collider)
            {
                return false;
            }
            else if (colliders[i].tag == "Villager")
            {
                return false;
            }
        }
        return true;
    }
}

