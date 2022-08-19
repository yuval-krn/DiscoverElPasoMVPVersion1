using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class artManager : MonoBehaviour
{
    public static List<GameObject> objectsToPlace = new List<GameObject>();
    public static List<GameObject> placedObjects = new List<GameObject>();

    // Start is called before the first frame update
    public void Start() 
    {
        // load all prefabs (pictures) as gameObjects: note that all the prefab names should be identical to the corresponding hotspot name 
        Object[] subListObjects = Resources.LoadAll("Prefabs/ctbgallery", typeof(GameObject));
        foreach (GameObject subListObject in subListObjects) 
        {    
            GameObject obj = (GameObject)subListObject;
            objectsToPlace.Add(obj);
        }
        Input.location.Start();
    }
}
