using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuButtonCommands : MonoBehaviour
{
    
    public void ExitApplication()
    {
        Application.Quit();
    }

    public void ChangeScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void SetObjectActive(GameObject obj)
    {
        obj.SetActive(true);
    }

    public void SetObjectInactive(GameObject obj)
    {
        obj.SetActive(false);
    }
}
