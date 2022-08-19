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
    public void ClearButton()
    {
        clearMessage.SetActive(true);
        PlayerPrefs.DeleteAll(); //new line - yuval 8/16
        PlayerPrefs.SetInt("ClearAll", 1);
        Debug.Log("when i clikc the clear button this should be 1" + PlayerPrefs.GetInt("ClearAll").ToString());
        PlayerPrefs.SetInt("seenMapOnboarding", 1);
        PlayerPrefs.SetInt("seenAROnboarding", 1);
        StartCoroutine(ClearAll());
    }

    IEnumerator ClearAll()
    {
        yield return new WaitForSecondsRealtime(1);
        clearMessage.SetActive(false);
    }

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
