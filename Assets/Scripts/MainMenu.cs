using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {
        RegionLevelMenu.Open();
    }

    public void QuitGame()
    {
        Debug.Log("Quit!");
        Application.Quit();
    }

}
