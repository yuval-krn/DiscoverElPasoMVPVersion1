using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsButtons : MonoBehaviour
{
    [SerializeField] GameObject clearMessage;
    public void Start() 
    {
        clearMessage.SetActive(false);
    }

    // when player clears, display message and reset all playerprefs
    public void ClearButton()
    {
        clearMessage.SetActive(true);
        PlayerPrefs.DeleteAll(); 
        PlayerPrefs.SetInt("ClearAll", 1);
        PlayerPrefs.SetInt("seenMapOnboarding", 1);
        PlayerPrefs.SetInt("seenAROnboarding", 1);
        StartCoroutine(ClearAll());
    }

    // display "all hotspots have been reset" message
    IEnumerator ClearAll()
    {
        yield return new WaitForSecondsRealtime(1);
        clearMessage.SetActive(false);
    }

    // links
    public void openAboutCIC() 
    {
        Application.OpenURL("https://www.codingitforward.com/");
    }

    public void openAboutElPaso() 
    {
        Application.OpenURL("https://www.elpasotexas.gov/information-technology/");
    }

    public void openAboutProject() 
    {
        Application.OpenURL("https://docs.google.com/presentation/d/1TTGM1U8mnO3mkOMJcp6RdbOMpcgrZ5y3U31hP9eSVd8/edit#slide=id.g143e96c2937_0_5");
    }
}
