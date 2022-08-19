using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ButtonHandler : MonoBehaviour
{
    // this should contain the OnboardingCanvas GameObject
    [SerializeField] GameObject tutorialMessage;

    // open onboarding message
    public void OpenMessage() 
    {
        if (tutorialMessage != null)
        {
            tutorialMessage.SetActive(true);
        }
    }
}
