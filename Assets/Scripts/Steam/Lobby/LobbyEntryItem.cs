using System;
using UnityEngine;
using Steamworks;
using TMPro;

public class LobbyEntryItem : MonoBehaviour
{
    public CSteamID LobbyId;
    public string LobbyName;
    public string LobbyCurrentPlayers;
    public string LobbyMaxPlayers;
    public string LobbyPing;
    public TMP_Text LobbyNameText;
    public TMP_Text LobbyPingText;
    public TMP_Text LobbyPlayersCountText;

    public void SetLobbyData()
    {
        if (LobbyName == "")
        {
            LobbyPingText.text = LobbyPing;
            LobbyNameText.text = "Empty";
        }
        else
        {
            LobbyPingText.text = LobbyPing;
            LobbyNameText.text = LobbyName;
            LobbyPlayersCountText.text = $"{LobbyCurrentPlayers}/{LobbyMaxPlayers}";
        }
    }

    public void JoinLobby()
    {
        if (CanEntryToLobby() == false) return;
        
        Lobby.Instance.LeaveLobby(false);
        Lobby.Instance.GameIsJoined = true;
        Lobby.Instance.NeedChangeButtonsAfterJoin = true;
        Lobby.Instance.HideStartHostButton();
        Lobby.Instance.ButtonsNotAvailableTimer();
        SteamLobby.Instance.JoinLobby(LobbyId);
    }

    private bool CanEntryToLobby()
    {
        string lobbyMaxPlayers = SteamMatchmaking.GetLobbyData(LobbyId, "lobbyMaxPlayers");
        int maxUsersNumInLobby = Int32.Parse(lobbyMaxPlayers.Length > 0 ? lobbyMaxPlayers : "0");
        int currentUsersNumInLobby = SteamMatchmaking.GetNumLobbyMembers(LobbyId);
        
        if (currentUsersNumInLobby >= maxUsersNumInLobby) return false;
        
        return true;
    }
    
}
