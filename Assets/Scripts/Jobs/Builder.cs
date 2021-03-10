using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Builder : Job
{
    public Builder() : base()
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