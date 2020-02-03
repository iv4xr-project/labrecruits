/*
This program has been developed by students from the bachelor Computer Science
at Utrecht University within the Software and Game project course.

©Copyright Utrecht University (Department of Information and Computing Sciences)
*/

using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject aboutPanel;

    public void StartApplication()
    {
        SceneManager.LoadScene("Main");
    }

    public void ActivateAboutPanel(bool state)
    {
        aboutPanel.SetActive(state);
    }

    public void Exit()
    {
        Application.Quit();
    }
}
