using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Student : Job
{
    public Student() : base()
    {

    }

    public override IEnumerator DoTheWork()
    {
        yield return null;
    }

    public override Vector3 GetWorkplacePos()
    {
        return Vector3.zero;
    }
}