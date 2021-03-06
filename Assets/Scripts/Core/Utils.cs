using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public static class Utils {
    public const string LAYER_BUILDING = "Building", LAYER_VILLAGER = "Villager", LAYER_UI = "UI", LAYER_ENVIRONMENT = "Environment", LAYER_TERRAIN = "Terrain";

    public static bool IsPointerOverUIElement() {
        // setup the pointer pos and event system
        var eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;
        var results = new List<RaycastResult>();
        // raycast the pos and get the results
        EventSystem.current.RaycastAll(eventData, results);
        // count number of raycast result having UI layer, return true if none have it
        return results.Where(r => r.gameObject.layer == LayerMask.NameToLayer(LAYER_UI)).Count() > 0;
    }
}