using System.Collections;
using System.Linq;
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

        // keep track of camera position to place image accordingly
        public GameObject cam;

        // keep track of object to spawn
        private GameObject spawnedObject;

        // number of hotspots
        [SerializeField] public int maxObjectsToPlace;

        // object count
        private int placedObjectCount = 0;
        
        // manage raycasts and detected planes
        public ARRaycastManager arRaycastManager;
        public ARPlaneManager arPlaneManager;

        // manage spawned objects
        private List<GameObject> placedGameObjects = new List<GameObject>();

        // store information relevant to current hotspot to place correct image
        string hotspotName;
        int objectIndex;

        // keep track of where user touches screen and list of all raycast hits
        Vector2 touchPosition;
        static List<ARRaycastHit> hits = new List<ARRaycastHit>();

        // Awake is called after scene is loaded when object is actived/enabled
        // since AR Session is always enabled within the Ar Scene this function is called once everytime the scene is loaded
        private void Awake() 
        {
            // check if AROnboarding has been seen before, set onboarding to false if so
            if (PlayerPrefs.GetInt("seenAROnboarding") > 0)
            {
                OnboardingHandler.onboardingCanvas.SetActive(false);
                CanvasHandler.ARCanvas.SetActive(true);
            }


            // when user clears all placed images
            if (PlayerPrefs.GetInt("ClearAll") > 0)
            {
                ResetAll();
                PlayerPrefs.DeleteKey("ClearAll");
            }
            
            // set the third child of the ARCanvas to be false
            CanvasHandler.ARCanvas.transform.GetChild(2).gameObject.SetActive(false);
            
            arRaycastManager = GetComponent<ARRaycastManager>();
            arPlaneManager = GetComponent<ARPlaneManager>();
        }
        
        void Start() 
        {
            //get activated hotspotName and index 
            foreach (var x in artManager.objectsToPlace)
            {
                //use magic number to keep track of currently activated hotspot
                if (PlayerPrefs.GetInt(x.name) == 7)
                {
                    hotspotName = x.name;
                }
            }

            // find the corresponding prefab for the hotspot the user is currently in
            objectIndex = artManager.objectsToPlace.FindIndex(x => x.name.Equals(hotspotName));
            
        }

        void Update() 
        {
            // only activate once onboarding is finished
            if (OnboardingHandler.onboardingCanvas.activeSelf == false) 
            {
                // check to make sure user is within a hotspot or the hotspot hasn't been activated
                if (objectIndex == -1 || PlayerPrefs.GetInt(hotspotName) == 1) {
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
                    // disable message telling user to tap to place the object
                    CanvasHandler.ARCanvas.transform.GetChild(1).gameObject.SetActive(false);
                    
                    var imageLocation = arPlaneManager.GetPlane(hits[0].trackableId);

                    var hitPose = hits[0].pose;

                    if (placedObjectCount < maxObjectsToPlace) 
                    {
                        // spawn picture at user's tapped location 
                        spawnedObject = Instantiate(artManager.objectsToPlace[objectIndex], hitPose.position, hitPose.rotation);
                        // face picture towards camera (a little finnicky right now)
                        spawnedObject.transform.LookAt(new Vector3(spawnedObject.transform.position.x, spawnedObject.transform.position.y, cam.transform.position.z));
                        // set spawnedObject name = hotspot name = prefab name
                        spawnedObject.name = hotspotName;
                        
                        // currently clear all isn't working with this but can fix and use this to retain across scenes
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

                        placedObjectCount++;

                        // set next stop ui element active (note that this hard codes which child element it is)
                        CanvasHandler.ARCanvas.transform.GetChild(2).gameObject.SetActive(true);
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

        // when user clicks the reset button, reset hotspot
        public void ResetHotspot()
        {
            // get most recently placed prefab and destroy
            GameObject hotspot = placedGameObjects.LastOrDefault();
            Destroy(hotspot);
            PlayerPrefs.DeleteKey(hotspot.name);

            placedObjectCount--;
        }

        // called when user tries to clear all hotspots
        public void ResetAll()
        {
            // destroy all instantiated gameobjects (only works if spawnedObjects remain between scenes)
            // foreach (GameObject art in placedGameObjects)
            // {
            //     Destroy(art);
            // }
            // reset placed object count
            placedObjectCount = 0;
        }
    }
}
