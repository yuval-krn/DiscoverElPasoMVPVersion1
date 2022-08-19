using System.Collections;
using TMPro;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.UI;

[RequireComponent(typeof(ARRaycastManager))]

public class ARDetectionatImage : MonoBehaviour
{
    // keep track of objects to spawn and placed object
    public static GameObject[] myObjects;
    public GameObject spawnedObject;

    // manage raycasts to place objects
    public ARRaycastManager _arRaycastManager;

    // detected plane size
    float xsize;
    float ysize;

    public TMP_Text touches;
    public TMP_Text finalWords;
    public TMP_Text planeSize;
    public TMP_Text objectSize;

    // keep track of where user touches screen and list of all raycast hits
    Vector2 touchPosition;
    static List<ARRaycastHit> hits = new List<ARRaycastHit>();

    // manage gallery size + objects
    int gallerySize = 10;
    int objectsSpawned = 0;

    // keep track of visited planes
    List<string> visitedplanes = new List<string>();

    private void Awake() {
        _arRaycastManager = GetComponent<ARRaycastManager>();
        arPlaneManager = GetComponent<ARPlaneManager>();
    }

    ARPlaneManager arPlaneManager;

    void start() {
        myObjects = Resources.LoadAll<GameObject>("Prefabs/galleryart");
    }


    void Update() {
        // check if user has touched screen
        if (Input.touchCount > 0) {
            // get position of touch 
            var touch = Input.GetTouch(0);
            touchPosition = Input.GetTouch(0).position;
            // distinguish between different types of touches
            if (touch.phase == TouchPhase.Ended) {
                touches.text = Input.touchCount.ToString();
                // check if hit falls within detected plane range
                if (_arRaycastManager.Raycast(touchPosition, hits, TrackableType.PlaneWithinPolygon)) {
                    var imageLocation = arPlaneManager.GetPlane(hits[0].trackableId);
                    VisitingPlanes(imageLocation);
                    xsize = imageLocation.size.x;
                    ysize = imageLocation.size.y;
                    planeSize.text = imageLocation.size.x.ToString() + " " + imageLocation.size.y.ToString();
                    // get position of hit
                    var hitPose = hits[0].pose;
                    if (gallerySize > objectsSpawned) {
                        GameObject spawnedObject = Instantiate((myObjects[0] as GameObject), hitPose.position, hitPose.rotation);
                        objectSize.text = spawnedObject.GetComponent<Renderer>().bounds.size.x.ToString() + " " + spawnedObject.GetComponent<Renderer>().bounds.size.y.ToString();
                        spawnedObject.transform.Rotate(-90, 180, 0, Space.Self);
                        objectsSpawned++;
                        imageLocation.gameObject.SetActive(false);
                        //SpawnObjects(spawnedObject, hitPose);
                    }
                    if (gallerySize == objectsSpawned) {
                        finalWords.text = "You've Placed All the Pictures!";
                    }
                }
            }
        }
    }

    void SpawnObjects(GameObject objectToPlace, Pose hitPose) {
        // use raycast hit to place object 
        //GameObject spawnedObject = Instantiate(objectToPlace, hitPose.position, hitPose.rotation);
        // rotate object to align with detected plane (may be different for android but 180 degree rotation for ios)
        //spawnedObject.transform.Rotate(-90, 180, 0, Space.Self);
        //spawnedObject.transform.localScale = new Vector3(xsize * 10, ysize * 10, 0);
        //objectsSpawned++;
    }

    void VisitingPlanes(ARPlane planeId) {
        if (!visitedplanes.Contains(planeId.ToString())) {
            visitedplanes.Add(planeId.ToString());
        }
    }

    int GetFactor(ARPlane planeId, GameObject Spawnedobject) {
        float planeArea = planeId.size.x * planeId.size.y;
        float objectArea = Spawnedobject.GetComponent<Renderer>().bounds.size.x * Spawnedobject.GetComponent<Renderer>().bounds.size.y;
        int factor = (int)(planeArea/objectArea);
        return factor;
    }

}


