using System.Collections;
using System.Collections.Generic;
using Steamworks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BottomPanelUserData : MonoBehaviour
{
    [SerializeField] private TMP_Text playerNicknameText;
    [SerializeField] private TMP_Text playerSteamLevelText;
    [SerializeField] private RawImage playerIcon;

    private CSteamID _playerSteamID;
    
    private bool _avatarReceived;

    void Start()
    {
        if (!SteamManager.Initialized) return;
        
        SetPLayerValues();
    }

    public void SetPLayerValues()
    {
        _playerSteamID = SteamUser.GetSteamID();
        int playerSteamLevel = SteamUser.GetPlayerSteamLevel();
        playerSteamLevelText.text = $"Steam level {playerSteamLevel.ToString()}";
        playerNicknameText.text = SteamFriends.GetPersonaName();
        if (!_avatarReceived) GetPlayerIcon();
    }

    private void GetPlayerIcon()
    {
        SteamPlayerAvatar steamPlayerAvatar = new SteamPlayerAvatar(_playerSteamID);
        Texture2D userAvatar = steamPlayerAvatar.GetPlayerIcon();
        if (userAvatar != null)
        {
            playerIcon.texture = userAvatar;
        }
    }
}
