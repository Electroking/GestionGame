using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public static class Utils {
    public const string LAYER_BUILDING = "Building", LAYER_VILLAGER = "Villager", LAYER_UI = "UI", LAYER_ENVIRONMENT = "Environment", LAYER_TERRAIN = "Terrain";
    public const string TAG_TREE = "Tree", TAG_MINE = "Mine", TAG_BUSH = "Bush";
    public static List<string> firstnameList = new List<string>() { "Gabriel", "Léo", "Raphaël", "Arthur", "Louis", "Emma", "Jade", "Louise", "Lucas",
    "Adam", "Maël", "Jules", "Hugo", "Alice", "Liam", "Lina", "Chloé", "Noah", "Ethan", "Paul", "Mia", "Inès", "Léa", "Tiago", "Rose", "Mila", "Ambre", "Sacha",
    "Gabin", "Nathan", "Mohamed", "Anna", "Aaron", "Eden", "Julia", "Léna", "Tom", "Noé", "Théo", "Elena", "Léon", "Zoé", "Juliette", "Manon", "Martin", "Mathis", "Eva", "Timéo", "Nolan", "Agathe"};
    public static List<string> nameList = new List<string>() { "Martin", "Bernard", "Thomas", "Petit", "Robert", "Richard", "Durand", "Dubois", "Moreau", "Laurent",
        "Simon", "Michel", "Lefèvre", "Leroy", "Roux", "David", "Bertrand", "Morel", "Fournier", "Girard", "Bonnet", "Dupont", "Lambert", "Fontaine", "Rousseau", 
        "Vincent", "Muller", "Lefevre", "Faure", "Andre", "Mercier", "Blanc", "Guerin", "Boyer", "Garnier", "Chevalier", "Francois", "Legrand", "Gauthier", 
        "Garcia", "Perrin", "Robin", "Clement", "Morin", "Nicolas", "Henry", "Roussel", "Mathieu", "Gautier", "Masson"};
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