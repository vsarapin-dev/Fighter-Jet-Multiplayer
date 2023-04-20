using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DebugLobbyId : MonoBehaviour
{
    public static DebugLobbyId Instance;
    
    [SerializeField] private TMP_Text lobbyIdText;
    
    private bool _shouldFindDebugLobbyId = true;
    private DebugLobbyId _debugLobbyId;


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }
    
    private void Update()
    {
        CheckLobbyScene();
        CheckOtherScenes();
        ProcessUpdateLobbyId();
    }

    public void SetDebugLobbyIdText(string lobbyId)
    {
        lobbyIdText.text = lobbyId;
    }

    private void CheckLobbyScene()
    {
        if (_shouldFindDebugLobbyId &&
            SceneManager.GetActiveScene().name == "Lobby")
        {
            _shouldFindDebugLobbyId = false;
            _debugLobbyId = FindObjectOfType<DebugLobbyId>();
        } 
    }
    
    private void CheckOtherScenes()
    {
        if (_shouldFindDebugLobbyId && SceneManager.GetActiveScene().name != "Lobby")
        {
            _shouldFindDebugLobbyId = true;
            _debugLobbyId = null;
        }
    }
    
    private void ProcessUpdateLobbyId()
    {
        if (_debugLobbyId != null)
        {
            _debugLobbyId.SetDebugLobbyIdText(NetworkLobbyIdentificator.Instance.CurrentLobbyId.ToString());
        }
    }
}
