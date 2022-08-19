using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnboardingHandler : MonoBehaviour
{
    // keep track of onboardingCanvas for later use
    public static GameObject onboardingCanvas;

    void Awake()
    {
        if (onboardingCanvas == null)
        {
            onboardingCanvas = this.gameObject;
        }
        else if (onboardingCanvas != this.gameObject)
        {
            Destroy (gameObject);
        }
    }
}
