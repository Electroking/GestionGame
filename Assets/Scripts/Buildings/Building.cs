using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class Building : MonoBehaviour
{
    public static List<Building> unbuiltList = new List<Building>();

    public new Collider collider;
    public Type type;

    public List<Villager> builders = new List<Villager>();
    public bool isPlaced = false;
    public bool isBuilt = false;
    public int resourceS;
    public int resourceW;
    public float nbBuilder;

    float Progression {
        get { return _progression; }
        set {
            _progression = Mathf.Clamp(value, 0, maxProgression);
            _model.transform.localPosition = Vector3.up * (2 * _progression / maxProgression - 1);
        }
    }
    public float maxProgression = 10;
    float _progression = 0;
    MeshRenderer[] _renderers;
    NavMeshObstacle _obstacle;
    GameObject _model;
    bool _canbePlaced = false;
    float _colorAlpha = 1;

    public enum Type
    {
        House = 0, School = 1, Farm = 2, Museum = 4, Library = 3
    }

    private void Awake()
    {
        _model = transform.GetChild(1).gameObject;
        _obstacle = GetComponent<NavMeshObstacle>();
        _obstacle.enabled = false;
        _renderers = GetComponentsInChildren<MeshRenderer>();
        collider = GetComponentInChildren<Collider>();
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.tag = tag;
        }
    }

    protected virtual void Start()
    {
        //transform.localScale *= 0.5f;
    }

    protected virtual void Update()
    {
        if (!isPlaced)
        {
            CheckIfCanBePlaced();
            //ChangeAlpha(canbePlaced ? 1 : 0.5f);
        }
    }

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
        GameManager.instance.terrain.UpdateNavMesh();
        return isPlaced = true;
    }

    public void Build(float amount) {
        if(isBuilt) { return; }
        Progression += amount;
        if(Progression >= maxProgression) {
            isBuilt = true;
            unbuiltList.Remove(this);
            //Debug.Log(name + " has been built!");
            OnBuilt();
        }
    }

    private void ExpendResources()
    {
        GameManager.instance.Stone -= resourceS;
        GameManager.instance.Wood -= resourceW;
    }

    private bool CheckifEnoughResources()
    {
        return GameManager.instance.Stone >= resourceS && GameManager.instance.Wood >= resourceW;
    }

    /*IEnumerator Construct(float nbSeconds)
    {

        float seconds = 0;
        while (seconds < nbSeconds)
        {
            seconds += Time.deltaTime;
            yield return null;

        }
        // at this point, the building has been built
        Debug.Log("Construct");
        OnBuilt();
    }*/

    protected abstract void OnBuilt();

    void ChangeAlpha(float newAlpha) {
        if(newAlpha == _colorAlpha) { return; }
        Color matColor;
        for(int i = 0; i < _renderers.Length; i++) {
            matColor = _renderers[i].material.color;
            matColor.a = newAlpha;
            _renderers[i].material.color = matColor;
        }
        _colorAlpha = newAlpha;
    }

    bool CheckIfCanBePlaced()
    {
        Collider[] colliders = Physics.OverlapBox(transform.position, collider.transform.localScale * 0.5f, transform.rotation);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].tag == "Village" && colliders[i] != collider)
            {
                return _canbePlaced = false;
            }
            else if (colliders[i].tag == "Villager")
            {
                return _canbePlaced = false;
            }
        }
        return _canbePlaced = true;
    }
}

