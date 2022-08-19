using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class test : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //commnting as parita !
        Debug.Log("begin");

        foreach (var x in ARLocation.ModifiedWebMapLoader.hotspotsToVisit)
        {
            Debug.Log(x.name);
        }
        Debug.Log("end");
    }
}
