using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Building : MonoBehaviour
{

    public static int nbHouses;
    public Type type;
    public bool canbeBuilt;
    public bool isBuilt;

    public enum Type
    {
        House, School, Farm, Museum, Library
    }

    protected virtual void Start()
    {
        
    }

    protected virtual void Update()
    {
        
    }

    protected void OnBuilt()
    {

    }

    public void Place()
    {
        isBuilt = true;
    }

    public void Build()
    {
        StartCoroutine(nameof(Construct), 6f);
    }

    IEnumerator Construct(float nbSeconds)
    {
        float seconds = 0;
        while (seconds<nbSeconds)
        {
            seconds += Time.deltaTime;
            yield return null;
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Village")
        {
            canbeBuilt = true;
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Village")
        {
            canbeBuilt = false;
        }
    }
}
