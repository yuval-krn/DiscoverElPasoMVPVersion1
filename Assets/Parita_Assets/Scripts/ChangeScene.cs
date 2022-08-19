using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
    [SerializeField] private string ARScene = "ARView";
    [SerializeField] private string MapScene = "MapView";
    [SerializeField] private string HomeScene = "HomeView";
    [SerializeField] private string SettingsScene = "SettingsPage";

    [SerializeField] private string CollectionScene = "ArtCollection";
    
    [SerializeField] private string StudentScene = "StudentsPage";

    public void ARButton()
    {
        SceneManager.LoadScene(ARScene);
    }

    public void MapViewButton()
    {
        SceneManager.LoadScene(MapScene);
    }

    public void HomeButton()
    {
        SceneManager.LoadScene(HomeScene);
    }

    public void SettingsButton()
    {
        SceneManager.LoadScene(SettingsScene);
    }

    public void CollectionsButton()
    {
        SceneManager.LoadScene(CollectionScene);
    }

    public void StudentsButton()
    {
        SceneManager.LoadScene(StudentScene);
    }
}