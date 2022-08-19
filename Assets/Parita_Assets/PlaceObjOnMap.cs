using GoShared;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceObjOnMap : MonoBehaviour{
   public  double lat = 31.765569;

    public double lng = -106.275815;

    public GameObject prefab;
    // Start is called before the first frame update
    void Start()
    {
        Coordinates coordinates = new Coordinates(lat, lng);

        GameObject place = GameObject.Instantiate(prefab);

        Vector3 position = coordinates.convertCoordinateToVector(place.transform.position.y);

        place.transform.localPosition = position;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
