using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapOnboardingHandler : MonoBehaviour
{
    public static GameObject mapOnboardingCanvas;
    [SerializeField] private GameObject buttMan;

    void Awake()
    {
        if (mapOnboardingCanvas == null)
        {
            mapOnboardingCanvas = this.gameObject;
        }
        else if (mapOnboardingCanvas != this.gameObject)
        {
            Destroy (gameObject);
        }

        
    }

    private void Start()
    {
        if (PlayerPrefs.GetInt("seenMapOnboarding") > 0)
        {
            DisableHelp _disableHelp = buttMan.GetComponent<DisableHelp>();
            _disableHelp.Resume();
        }
    }
}
