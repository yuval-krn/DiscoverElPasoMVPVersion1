using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableHelp : MonoBehaviour
{
    [SerializeField] GameObject helpMenu;
    [SerializeField] GameObject disabledCamera;
    [SerializeField] GameObject enabledCamera;
    [SerializeField] GameObject helpButton;

    public void HelpButtonClick()
    {
        helpMenu.SetActive(true);

        disabledCamera.SetActive(false);
        enabledCamera.SetActive(false);
        helpButton.SetActive(false);
    }
}
