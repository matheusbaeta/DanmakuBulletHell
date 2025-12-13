using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    private const string sceneName = "MainMenu";

    public static void Show()
    {
        SceneManager.LoadScene(sceneName);
    }

    public void OnClickStart()
    {
        SceneManager.LoadScene("Gameplay");
    }
}
