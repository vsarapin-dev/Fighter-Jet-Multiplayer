using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PressCarOne()
    {
        PlayerPrefs.SetString("playerCar", "firstCar");
        LoadGame();
    }

    public void PressCarTwo()
    {
        PlayerPrefs.SetString("playerCar", "secondCar");
        LoadGame();
    }

    private void LoadGame()
    {
        int loadSceneCounter = SceneManager.GetActiveScene().buildIndex + 1;
        SceneManager.LoadScene(loadSceneCounter);
    }
}
