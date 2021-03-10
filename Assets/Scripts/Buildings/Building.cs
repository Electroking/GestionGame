using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Building : MonoBehaviour
{
    public static int nbHouses;

    public new Collider collider;
    public Type type;

    bool canbePlaced = false;
    public bool isPlaced = false;
    public bool isBuilt = false;

    MeshRenderer[] renderers;
    float colorAlpha = 1;

    public enum Type
    {
        House, School, Farm, Museum, Library
    }

    private void Awake()
    {
        renderers = GetComponentsInChildren<MeshRenderer>();
        collider = GetComponentInChildren<Collider>();
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.tag = tag;
        }
    }

    protected virtual void Start()
    {

    }

    protected virtual void Update()
    {
        if (!isPlaced)
        {
            CheckIfCanBePlaced();
            ChangeAlpha(canbePlaced ? 1 : 0.5f);
        }
    }

    public bool Place()
    {
        if (!CheckIfCanBePlaced()) { return false; }
        return isPlaced = true;
    }

    public void Build()
    {
        StartCoroutine(nameof(Construct), 6f);
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
        OnBuilt();
    }

    protected virtual void OnBuilt()
    {

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

