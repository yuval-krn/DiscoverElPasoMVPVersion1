using System.Collections;
using TMPro;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.UI;

// reference https://docs.unity3d.com/2019.2/Documentation/ScriptReference/GameObject.SetActive.html
namespace ARLocation
{
    [RequireComponent(typeof(ARRaycastManager))]

    public class PlaceImage : MonoBehaviour
    {

        public GameObject cam;
        public artManager artScript;

        // keep track of object to spawn
        private GameObject spawnedObject;

        [SerializeField]
        public int maxObjectsToPlace;

        // object count
        private int placedObjectCount = 0;
        
        // manage raycasts and detected planes
        public ARRaycastManager arRaycastManager;
        public ARPlaneManager arPlaneManager;

        List<GameObject> placedGameObjects = new List<GameObject>();

        // store information relevant to current hotspot to place correct image
        string hotspotName;
        int objectIndex;
        string imagePlane;

        // keep track of where user touches screen and list of all raycast hits
        Vector2 touchPosition;
        static List<ARRaycastHit> hits = new List<ARRaycastHit>();

        // Awake is called after scene is loaded when object is actived/enabled
        // since AR Session is always enabled within the Ar Scene this function is called once everytime the scene is loaded
        private void Awake() 
        {
            arRaycastManager = GetComponent<ARRaycastManager>();
            arPlaneManager = GetComponent<ARPlaneManager>();

            if (PlayerPrefs.GetInt("seenAROnboarding") > 0)
            {
                OnboardingHandler.onboardingCanvas.SetActive(false);
                CanvasHandler.ARCanvas.SetActive(true);
            }
            else {
                OnboardingHandler.onboardingCanvas.SetActive(true);
                CanvasHandler.ARCanvas.SetActive(false);
            }

            if (PlayerPrefs.GetInt("ClearAll") == 1)
            {
                ResetAll();
                PlayerPrefs.DeleteKey("ClearAll");
                arPlaneManager.enabled = true;
            }
            
            CanvasHandler.ARCanvas.transform.GetChild(3).gameObject.SetActive(false);
        }

        void Update() 
        {
            // only activate once onboarding is finished
            if (OnboardingHandler.onboardingCanvas.activeSelf == false) 
            {
                //get activated hotspotName and index 
                foreach (var x in artManager.objectsToPlace)
                {
                    //use magic number to keep track of currently activated hotspot
                    if (PlayerPrefs.GetInt(x.name, 0) == 7)
                    {
                        hotspotName = x.name;
                        Debug.Log("this should only be called once at a time" + hotspotName);
                    }
                }

                Debug.Log("this is the hotspot name of the thing that is activated right now" + hotspotName);

                // find the corresponding prefab for the hotspot the user is currently in
                objectIndex = artManager.objectsToPlace.FindIndex(x => x.name.Equals(hotspotName));

                // check to make sure user is within a hotspot or the hotspot hasn't been activated
                if (objectIndex == -1 || PlayerPrefs.GetInt(hotspotName) == 1) {
                    Debug.Log("this is the object index" + objectIndex.ToString());
                    Debug.Log("this is the playerpref" +  PlayerPrefs.GetInt(hotspotName).ToString());
                    Debug.Log("I shouldn't get here if there's a hotsopt");
                    arPlaneManager.enabled = false;
                    return;
                }

                // sanity check
                if (arPlaneManager.enabled == false) 
                {
                    arPlaneManager.enabled = true;
                }

                // check if user detects a plane
                if (arPlaneManager.trackables.count > 0)
                {
                    // enable message informing user that a plane was detected and they need to tap to place the object
                    CanvasHandler.ARCanvas.transform.GetChild(1).gameObject.SetActive(true);
                }

                // if user hasn't tapped to place object, wait for update
                if (!GetTouchPosition()) 
                {
                    return;
                }

                // check if user taps on a detected plane
                if (arRaycastManager.Raycast(touchPosition, hits, TrackableType.PlaneWithinPolygon)) 
                {  
                    Debug.Log("I should make it here both times");
                    // disable message telling user to tap to place the object
                    CanvasHandler.ARCanvas.transform.GetChild(1).gameObject.SetActive(false);
                    
                    var imageLocation = arPlaneManager.GetPlane(hits[0].trackableId);

                    var hitPose = hits[0].pose;

                    if (placedObjectCount < maxObjectsToPlace) 
                    {
                        Debug.Log("this is the object that i'm placing" + artManager.objectsToPlace[objectIndex].name);
                        // spawn picture at user's tapped location 
                        spawnedObject = Instantiate(artManager.objectsToPlace[objectIndex], hitPose.position, hitPose.rotation);
                        spawnedObject.transform.LookAt(new Vector3(spawnedObject.transform.position.x, spawnedObject.transform.position.y, cam.transform.position.z));
                        // DontDestroyOnLoad(spawnedObject);

                        placedGameObjects.Add(spawnedObject);

                        // deactivate the place the picture was instantiated on and turn off the arPlaneManager
                        imageLocation.gameObject.SetActive(false);
                        foreach (var plane in arPlaneManager.trackables)
                        {
                            plane.gameObject.SetActive(false);
                        }
                        arPlaneManager.enabled = false;
                        
                        // move hotspot from list of hotspots that need to be visited
                        PlayerPrefs.SetInt(hotspotName, 1);

                        // move placed picture from temp list to final list 
                        artManager.objectsToPlace.Remove(artManager.objectsToPlace[objectIndex]);
                        artManager.placedObjects.Add(artManager.objectsToPlace[objectIndex]);
                        placedObjectCount++;

                        // set next stop ui element active
                        CanvasHandler.ARCanvas.transform.GetChild(3).gameObject.SetActive(true);
                    }
                }
            }
            else 
            {
                arPlaneManager.enabled = false;
            }
        }

        // get position of user tap to place object 
        bool GetTouchPosition() 
        {
            if (Input.touchCount > 0) 
            {
                if (Input.GetTouch(0).phase == TouchPhase.Began) 
                {
                    touchPosition = Input.GetTouch(0).position; 
                    return true;
                }
            }

            touchPosition = default;
            return false;
        }
        public void ResetHotspot()
        {
            GameObject hotspotToReset = placedGameObjects[placedGameObjects.Count - 1];
            Destroy(GameObject.Find(hotspotToReset.name));
            artManager.placedObjects.Remove(hotspotToReset);
            artManager.objectsToPlace.Add(hotspotToReset);
            placedObjectCount--;
            PlayerPrefs.DeleteKey(hotspotName);
            arPlaneManager.enabled = true;
        }

        public void ResetAll()
        {
            foreach (GameObject art in artManager.placedObjects)
            {
                artManager.placedObjects.Remove(art);
                artManager.objectsToPlace.Add(art);
            }

            foreach (GameObject art in placedGameObjects)
            {
                Destroy(art);
            }
            placedObjectCount = 0;
        }
    }
}
