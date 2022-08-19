using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActivityUI : MonoBehaviour
{
    public RectTransform activityPanel;
    public GameObject backToDescButton;
    // Start is called before the first frame update
    void Start()
    {
        
    }

   public void HideActivityPanel()
    {
        activityPanel.gameObject.SetActive(false);
        backToDescButton.SetActive(true);
    }

    public void ShowActivityPanel()
    {
        activityPanel.gameObject.SetActive(true);
        backToDescButton.SetActive(false);
    }


}
