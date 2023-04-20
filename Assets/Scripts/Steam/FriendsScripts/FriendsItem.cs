using System.Collections;
using System.Collections.Generic;
using Steamworks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FriendsItem : MonoBehaviour
{
    [SerializeField] private TMP_Text playerNameText;
    [SerializeField] private TMP_Text playerStatusText;
    [SerializeField] private RawImage playerAvatarImage;
    
    private string _playerName;
    private string _playerStatus;
    private CSteamID _playerSteamId;
    private int _playerFriendId;

    public CSteamID PlayerSteamId
    {
        get => _playerSteamId;
        set => _playerSteamId = value;
    }
    public string PlayerName
    {
        get => _playerName;
        set => _playerName = value;
    }
    public string PlayerState
    {
        set => _playerStatus = value;
    }
    
    public int PlayerFriendId
    {
        get => _playerFriendId;
        set => _playerFriendId = value;
    }

    public void SetFriendValues()
    {
        playerNameText.text = _playerName;
        playerStatusText.text = _playerStatus;
        SetAvatar();
    }

    public void OnFriendClick()
    {
        SteamFriends.ActivateGameOverlayToUser("steamid", _playerSteamId);
    }

    private void SetAvatar()
    {
        SteamPlayerAvatar steamPlayerAvatar = new SteamPlayerAvatar(_playerSteamId);
        Texture2D friendAvatar = steamPlayerAvatar.GetPlayerIcon();
        if (friendAvatar != null)
        {
            playerAvatarImage.texture = friendAvatar;
        }
    }
}
