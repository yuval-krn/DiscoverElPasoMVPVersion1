using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasHandler : MonoBehaviour
{
    // keep track of AR canvas for later reference
    public static GameObject ARCanvas;

    void Awake()
    {
        if (ARCanvas == null)
        {
            ARCanvas = this.gameObject;
        }
        else if (ARCanvas != this.gameObject)
        {
            Destroy (gameObject);
        }
    }

    void Start()
    {
        // set the tap to place image object as false
        this.transform.GetChild(1).gameObject.SetActive(false);

        // set the next stop object as false
        this.transform.GetChild(3).gameObject.SetActive(false);
    }
}