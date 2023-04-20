using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using UnityEngine;
using UnityEngine.UI;
using Steamworks;
using TMPro;

public class PlayerListItem : NetworkBehaviour
{
    public int ConnectionID;
    public string PlayerName;
    public ulong PlayerSteamID;
    public int PlayerIdNumber;
    public TMP_Text PlayerNameText;
    public TMP_Text PlayerStatusText;
    public RawImage PlayerIcon;
    
    private bool _playerLobbyReadyStatus;
    private bool _avatarReceived;

    public bool PlayerLobbyReadyStatus
    {
        get => _playerLobbyReadyStatus;
        set => _playerLobbyReadyStatus = value;
    }
    
    public void SetPLayerValues()
    {
        PlayerNameText.text = PlayerName;
        if (!_avatarReceived) GetPlayerIcon();
    }

    public void SetPlayerStatusText(bool ready)
    {
        PlayerStatusText.text = ready ? "Ready" : "Not Ready";
        PlayerStatusText.color = ready ? Color.green : Color.red;
    }

    private void GetPlayerIcon()
    {
        SteamPlayerAvatar steamPlayerAvatar = new SteamPlayerAvatar((CSteamID) PlayerSteamID);
        Texture2D userAvatar = steamPlayerAvatar.GetPlayerIcon();
        if (userAvatar != null)
        {
            PlayerIcon.texture = userAvatar;
        }
    }
}
