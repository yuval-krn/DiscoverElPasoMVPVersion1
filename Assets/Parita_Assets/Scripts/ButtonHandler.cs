using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ButtonHandler : MonoBehaviour
{
    [SerializeField] GameObject tutorialMessage;

    public void OpenMessage() 
    {
        if (tutorialMessage != null)
        {
            tutorialMessage.SetActive(true);
        }
    }
}
