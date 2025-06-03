using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PlayGame1()
    {
        SceneManager.LoadScene("Shop"); // Наприклад: "GameScene"
    }

    public void QuitGame2()
    {
        Debug.Log("Вихід з гри...");
        Application.Quit();
    }
}
