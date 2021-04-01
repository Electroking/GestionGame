using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public static class Utils
{
    public const string LAYER_BUILDING = "Building", LAYER_VILLAGER = "Villager", LAYER_UI = "UI", LAYER_ENVIRONMENT = "Environment", LAYER_TERRAIN = "Terrain";
    public const string TAG_TREE = "Tree", TAG_MINE = "Mine", TAG_BUSH = "Bush";
    public static string[] firstNameList = new string[] {
        "Gabriel", "Léo", "Raphaël", "Arthur", "Louis", "Emma", "Jade", "Louise", "Lucas", "Adam", "Maël", "Jules", "Hugo", "Alice",
        "Liam", "Lina", "Chloé", "Noah", "Ethan", "Paul", "Mia", "Inès", "Léa", "Tiago", "Rose", "Mila", "Ambre", "Sacha", "Gabin",
        "Nathan", "Mohamed", "Anna", "Aaron", "Eden", "Julia", "Léna", "Tom", "Noé", "Théo", "Elena", "Léon", "Zoé", "Juliette",
        "Manon", "Martin", "Mathis", "Eva", "Timéo", "Nolan", "Agathe"
    };
    public static string[] lastNameList = new string[] {
        "Martin", "Bernard", "Thomas", "Petit", "Robert", "Richard", "Durand", "Dubois", "Moreau", "Laurent", "Simon", "Michel",
        "Lefèvre", "Leroy", "Roux", "David", "Bertrand", "Morel", "Fournier", "Girard", "Bonnet", "Dupont", "Lambert", "Fontaine",
        "Rousseau", "Vincent", "Muller", "Lefevre", "Faure", "Andre", "Mercier", "Blanc", "Guerin", "Boyer", "Garnier", "Chevalier",
        "Francois", "Legrand", "Gauthier", "Garcia", "Perrin", "Robin", "Clement", "Morin", "Nicolas", "Henry", "Roussel", "Mathieu",
        "Gautier", "Masson"
    };
    public static string[] fullNameList = new string[] {
        "Marion La Développeuse", "Prune Forge-Code"
    };

    public static string GetRandomName()
    {
        return Random.Range(0, 1f) <= 0.05f ?
            fullNameList[Random.Range(0, fullNameList.Length)] :
            firstNameList[Random.Range(0, firstNameList.Length)] + " " + lastNameList[Random.Range(0, lastNameList.Length)];
    }

    public static bool IsPointerOverUIElement()
    {
        // setup the pointer pos and event system
        var eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;
        var results = new List<RaycastResult>();
        // raycast the pos and get the results
        EventSystem.current.RaycastAll(eventData, results);
        // count number of raycast result having UI layer, return true if none have it
        return results.Where(r => r.gameObject.layer == LayerMask.NameToLayer(LAYER_UI)).Count() > 0;
    }

    // ++ EXTENSION METHODS ++ //

    public static int GetStoneCost(this Building.Type type)
    {
        PoolManager pm = PoolManager.instance;
        if (PoolManager.instance == null) return 0;
        switch (type)
        {
            case Building.Type.House:
                return pm.prefabHouse.resourceS;
            case Building.Type.School:
                return pm.prefabSchool.resourceS;
            case Building.Type.Farm:
                return pm.prefabFarm.resourceS;
            case Building.Type.Library:
                return pm.prefabLibrary.resourceS;
            case Building.Type.Museum:
                return pm.prefabMuseum.resourceS;
            default:
                return 0;
        }
    }

    public static int GetWoodCost(this Building.Type type)
    {
        PoolManager pm = PoolManager.instance;
        if (PoolManager.instance == null) return 0;
        switch (type)
        {
            case Building.Type.House:
                return pm.prefabHouse.resourceW;
            case Building.Type.School:
                return pm.prefabSchool.resourceW;
            case Building.Type.Farm:
                return pm.prefabFarm.resourceW;
            case Building.Type.Library:
                return pm.prefabLibrary.resourceW;
            case Building.Type.Museum:
                return pm.prefabMuseum.resourceW;
            default:
                return 0;
        }
    }
}