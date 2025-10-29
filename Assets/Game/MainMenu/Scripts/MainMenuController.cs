using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public void OnClickStart()
    {
        SceneManager.LoadScene("Gameplay");
    }
}
