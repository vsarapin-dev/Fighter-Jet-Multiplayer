using System;
using Steamworks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkLobbyIdentificator : MonoBehaviour
{
    public static NetworkLobbyIdentificator Instance;

    private CSteamID _currentLobbyId;

    public CSteamID CurrentLobbyId
    {
        get => _currentLobbyId;
        set
        {
            if (value.m_SteamID > 0)
            {
                _currentLobbyId = value;
            }
        }
    }

    public void ClearLobbyId()
    {
        _currentLobbyId.m_SteamID = 0;
        SteamLobby.Instance.ClearLobbyId();
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(Instance);
        }
        else
        {
            Instance = this;
        }
    }
}
