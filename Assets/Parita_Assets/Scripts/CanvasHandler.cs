using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasHandler : MonoBehaviour
{

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
        this.transform.GetChild(1).gameObject.SetActive(false);
        this.transform.GetChild(3).gameObject.SetActive(false);
    }
}