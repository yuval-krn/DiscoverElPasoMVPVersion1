using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionScript : MonoBehaviour
{

    [SerializeField] GameObject disableCamera;
    [SerializeField] GameObject enableCamera;
    [SerializeField] GameObject introScreen;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "hotspot" && !introScreen.activeInHierarchy)
        {
            disableCamera.SetActive(false);

            if (PlayerPrefs.GetInt(other.name) != 1)
            {
                PlayerPrefs.SetInt(other.name, 7);
            }

            enableCamera.SetActive(true);
        }
    }

    //experimental function, we'll see
    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "hotspot" && !introScreen.activeInHierarchy)
        {
            disableCamera.SetActive(false);

            if (PlayerPrefs.GetInt(other.name) != 1)
            {
                PlayerPrefs.SetInt(other.name, 7);
            }

            enableCamera.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "hotspot" && !introScreen.activeInHierarchy)
        {
            disableCamera.SetActive(true);
            enableCamera.SetActive(false);
        }
    }
}
