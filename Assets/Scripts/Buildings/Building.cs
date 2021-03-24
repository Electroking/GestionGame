using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    float _progression = 0;
    MeshRenderer[] renderers;
    GameObject model;
    bool canbePlaced = false;
    float colorAlpha = 1;

    public enum Type
    {
        House, School, Farm, Museum, Library
    }

    private void Awake()
    {
        model = transform.GetChild(0).gameObject;
        renderers = GetComponentsInChildren<MeshRenderer>();
        collider = GetComponentInChildren<Collider>();
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.tag = tag;
        }
    }

    protected virtual void Start()
    {
        transform.localScale *= 0.5f;
    }

    protected virtual void Update()
    {
        if (!isPlaced)
        {
            CheckIfCanBePlaced();
            //ChangeAlpha(canbePlaced ? 1 : 0.5f);
        }
    }

    public bool Place()
    {
        if (!CheckIfCanBePlaced()) { return false; }
        if (!CheckifEnoughResources()) { return false; }
        ExpendResources();
        unbuiltList.Add(this);
        return isPlaced = true;
    }

    public void Build(float amount)
    {
        if (isBuilt) { return; }
        _progression = Mathf.Clamp01(_progression + amount);
        transform.localScale = Vector3.one * 0.5f * (_progression + 1);
        if (_progression >= 1)
        {
            OnBuilt();
        }
        //StartCoroutine(nameof(Construct), 6f);
    }


    private void ExpendResources()
    {
        GameManager.instance.Stone -= resourceS;
        GameManager.instance.Wood -= resourceW;
        //Build();
    }

    private bool CheckifEnoughResources()
    {
        return GameManager.instance.Stone >= resourceS && GameManager.instance.Wood >= resourceW;
    }

    IEnumerator Construct(float nbSeconds)
    {

        float seconds = 0;
        while (seconds < nbSeconds)
        {
            seconds += Time.deltaTime;
            yield return null;

        }
        // at this point, the building has been built
        transform.localScale *= 2.5f;
        Debug.Log("Construct");
        OnBuilt();
    }

    protected virtual void OnBuilt()
    {
        isBuilt = true;
        unbuiltList.Remove(this);
        Debug.Log(name + " has been built!");
    }

    void ChangeAlpha(float newAlpha)
    {
        if (newAlpha == colorAlpha) { return; }
        Color matColor;
        for (int i = 0; i < renderers.Length; i++)
        {
            matColor = renderers[i].material.color;
            matColor.a = newAlpha;
            renderers[i].material.color = matColor;
        }
        colorAlpha = newAlpha;
    }

    bool CheckIfCanBePlaced()
    {
        Collider[] colliders = Physics.OverlapBox(transform.position, collider.transform.localScale * 0.5f, transform.rotation);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].tag == "Village" && colliders[i] != collider)
            {
                return canbePlaced = false;
            }
            else if (colliders[i].tag == "Villager")
            {
                return canbePlaced = false;
            }
        }
        return canbePlaced = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Village")
        {
            //canbeBuilt = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Village")
        {
            //canbeBuilt = false;
        }
    }
}

