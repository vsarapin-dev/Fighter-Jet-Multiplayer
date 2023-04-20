using TMPro;
using UnityEngine;

public class MainMenuUi : MonoBehaviour
{
    public static MainMenuUi Instance;

    [SerializeField] private GameObject musicText;

    private void Awake()
    {
        if (Instance != this && Instance != null)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    public void ShowSongName(string songName)
    {
        musicText.SetActive(true);
        musicText.GetComponent<TMP_Text>().text = songName;
    }
    
    public void RemoveSongItem()
    {
        musicText.GetComponent<TMP_Text>().text = "";
        musicText.SetActive(false);
    }

}
