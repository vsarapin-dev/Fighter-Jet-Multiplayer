using System.Collections;
using System.Collections.Generic;
using Steamworks;
using UnityEngine;

public class LobbyButtons : MonoBehaviour
{
    public void ReadyButtonClick()
    {
        PlayerObjectController playerObjectController = Lobby.Instance.FindLocalPlayer();
        playerObjectController.CmdUpdatePlayerReadyStatus(SteamFriends.GetPersonaName());
    }
    
    public void LeaveButtonClick()
    {
        Lobby.Instance.LeaveLobby();
    }

    public void StartGameButtonClick()
    {
        PlayerObjectController playerObjectController = Lobby.Instance.FindLocalPlayer();
        playerObjectController.CmdStartGame();
    }
}
