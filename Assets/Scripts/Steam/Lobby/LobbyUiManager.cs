using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUiManager : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private GameObject buttonReady;
    [SerializeField] private GameObject buttonStartGame;
    [SerializeField] private GameObject buttonLeaveGame;
    
    [Header("Text")]
    [SerializeField] private TMP_Text buttonReadyTextNormal;
    [SerializeField] private TMP_Text buttonReadyTextHighighted;
    [SerializeField] private TMP_Text buttonReadyTextPressed;

    public void EnableNecessaryButtons(bool gameIsHosted = false, bool isHost = false)
    {
        buttonReady.SetActive(!isHost);
        buttonStartGame.SetActive(gameIsHosted);
        buttonLeaveGame.SetActive(true);
    }
    
    public void ResetButtonsToStartValues()
    {
        buttonStartGame.SetActive(false);
        buttonReady.SetActive(false);
        buttonLeaveGame.SetActive(false);
    }

    public void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ChangeReadyButtonText(bool playerReady)
    {
        buttonReadyTextNormal.text = playerReady ? "NOT READY" : "GET READY";
        buttonReadyTextHighighted.text = playerReady ? "NOT READY" : "GET READY";
        buttonReadyTextPressed.text = playerReady ? "NOT READY" : "GET READY";
    }
    
    
}
