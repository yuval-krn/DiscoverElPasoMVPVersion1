using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableHelp : MonoBehaviour
{
    [SerializeField] GameObject helpMenu;
    [SerializeField] GameObject disabledCamera;
    [SerializeField] GameObject helpButton;

    public void Resume()
    {
        helpMenu.SetActive(false);
        PlayerPrefs.SetInt("seenMapOnboarding", 1);
        disabledCamera.SetActive(true);
        helpButton.SetActive(true);
    }
}
