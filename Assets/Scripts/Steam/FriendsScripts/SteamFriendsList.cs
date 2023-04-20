using System;
using System.Collections;
using System.Collections.Generic;
using Steamworks;
using UnityEngine;

public class SteamFriendsList : MonoBehaviour
{
    [SerializeField] private Transform friendsList;
    [SerializeField] private GameObject friendItemPrefab;

    private List<GameObject> _friendsGoList = new List<GameObject>();
    private bool _isFriendListOpen;

    private void Start()
    {
        if (!SteamManager.Initialized) return;
        
        SteamUtils.SetOverlayNotificationInset(0, 0);
        SteamFriends.SetListenForFriendsMessages(true);
    }
    
    public void RequestFriendListButtonClick()
    {
        _isFriendListOpen = !_isFriendListOpen;
        
        if (_isFriendListOpen == true)
        {
            RequestFriendList();
        }
        else
        {
            DestroyFriendsInList();
        }
    }

    public void CloseFriendListButtonClick()
    {
        _isFriendListOpen = false;
        DestroyFriendsInList();
    }
    
    private void RequestFriendList()
    {
        DestroyFriendsInList();
        
        int friendCount = SteamFriends.GetFriendCount(EFriendFlags.k_EFriendFlagImmediate);
        for (int i = 0; i < friendCount; i++)
        {
            CreateFriendItem(i);
        }
    }

    private void DestroyFriendsInList()
    {
        if (_friendsGoList.Count == 0) return;
        
        foreach (GameObject friendGo in _friendsGoList)
        {
            Destroy(friendGo);
        }
        _friendsGoList.Clear();
    }

    private void CreateFriendItem(int counter)
    {
        CSteamID friendSteamId = SteamFriends.GetFriendByIndex(counter, EFriendFlags.k_EFriendFlagImmediate);

        SteamFriendsFunctionality steamFriendsFunctionality = new SteamFriendsFunctionality();
        
        string friendState = steamFriendsFunctionality.GetFriendsStatus(friendSteamId);
        GameObject friendItem = Instantiate(friendItemPrefab);
        FriendsItem friendItemScript = friendItem.GetComponent<FriendsItem>();
        friendItemScript.PlayerSteamId = friendSteamId;
        friendItemScript.PlayerName = SteamFriends.GetFriendPersonaName(friendSteamId);
        friendItemScript.PlayerState = friendState;
        friendItemScript.SetFriendValues();

        InstantiateItemInProperPlace(friendItem, friendState);
    }

    private void InstantiateItemInProperPlace(GameObject friendItem, string friendState)
    {
        friendItem.transform.SetParent(friendsList);
        friendItem.transform.localScale = Vector3.one;
            
        if (friendState.Contains("Offline") == false)
        {
            friendItem.transform.SetSiblingIndex(1);
        }
        else
        {
            friendItem.transform.SetAsLastSibling();
        }
        
        _friendsGoList.Add(friendItem);
    }
}
