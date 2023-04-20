using System.Collections;
using System.Collections.Generic;
using Steamworks;
using UnityEngine;

public class MenuChatUserList : MonoBehaviour
{
    [SerializeField] private GameObject userListPrefab;
    [SerializeField] private int timeBeforeListUpdate = 5;

    private List<GameObject> _usersGoList = new List<GameObject>();
    private List<int> _newUsersGoListFromSteam = new List<int>();
    private bool _showPlayersInChat = false;
    private bool _coroutineStarted = false;
    private GameObject _myDefaultChatItemInList;

    void Update()
    {
        if (_showPlayersInChat == true)
        {
            StartCoroutine(UpdateList());
        }
        else
        {
            _coroutineStarted = false;
        }
    }
    
    public void ShowPlayersInChat(bool show)
    {
        DestroyAllUsersInList();
        if (show == false && _myDefaultChatItemInList != null) Destroy(_myDefaultChatItemInList);
        _showPlayersInChat = show;
    }

    private IEnumerator UpdateList()
    {
        if (_coroutineStarted == true) yield break;
        _myDefaultChatItemInList = CreateMyItem();
        while (true)
        {
            _coroutineStarted = true;
            _newUsersGoListFromSteam.Clear();
            
            
            int friendCount = SteamFriends.GetFriendCount(EFriendFlags.k_EFriendFlagImmediate);
            for (int i = 0; i < friendCount; i++)
            {
                CSteamID friendId = new CSteamID(SteamFriends.GetFriendByIndex(i, EFriendFlags.k_EFriendFlagImmediate).m_SteamID);
                SteamFriendsFunctionality steamFriendsFunctionality = new SteamFriendsFunctionality();

                if (steamFriendsFunctionality.FriendIsInTheSameGame(friendId))
                {
                    _newUsersGoListFromSteam.Add(i);
                }
            }
            CreateUsersItems();
            RemoveSomeUserItems();
            yield return new WaitForSeconds(timeBeforeListUpdate);
        }
    }
    
    private void DestroyAllUsersInList()
    {
        if (_usersGoList.Count == 0) return;
        
        foreach (GameObject userGo in _usersGoList)
        {
            Destroy(userGo);
        }
        _usersGoList.Clear();
    }
    
    private void RemoveSomeUserItems()
    {
        foreach (GameObject user in new List<GameObject>(_usersGoList))
        {
            if (_newUsersGoListFromSteam.Contains(user.GetComponent<FriendsItem>().PlayerFriendId) == false)
            {
                Destroy(user);
                _usersGoList.Remove(user);
            }
        }
    }
    
    private void CreateUsersItems()
    {
        foreach (int userId in _newUsersGoListFromSteam)
        {
            bool needCreate = true;
            foreach (GameObject user in _usersGoList)
            {
                if (user.GetComponent<FriendsItem>().PlayerFriendId == userId)
                {
                    needCreate = false;
                    break;
                }
            }
            if (needCreate == true)
            {
                CreateNewItem(userId);
            }
        }
    }

    private void CreateNewItem(int friendId)
    {
        CSteamID friendSteamId = SteamFriends.GetFriendByIndex(friendId, EFriendFlags.k_EFriendFlagImmediate);
        SteamPlayerAvatar steamPlayerAvatar = new SteamPlayerAvatar(friendSteamId);
        Texture2D friendAvatar = steamPlayerAvatar.GetPlayerIcon();

        GameObject friendItem = Instantiate(userListPrefab, transform);
        friendItem.transform.localScale = Vector3.one;
        
        FriendsItem friendItemScript = friendItem.GetComponent<FriendsItem>();
        friendItemScript.PlayerSteamId = friendSteamId;
        friendItemScript.PlayerName = SteamFriends.GetFriendPersonaName(friendSteamId);
        friendItemScript.PlayerFriendId = friendId;
        friendItemScript.PlayerState = "In Chat";
        friendItemScript.SetFriendValues();
                
        _usersGoList.Add(friendItem);
    }
    
    private GameObject CreateMyItem()
    {
        CSteamID mySteamId = SteamUser.GetSteamID();
        SteamPlayerAvatar steamPlayerAvatar = new SteamPlayerAvatar(mySteamId);
        Texture2D myAvatar = steamPlayerAvatar.GetPlayerIcon();

        GameObject myUserListItem = Instantiate(userListPrefab, transform);
        myUserListItem.transform.localScale = Vector3.one;
        FriendsItem myUserItemScript = myUserListItem.GetComponent<FriendsItem>();
        myUserItemScript.PlayerSteamId = mySteamId;
        myUserItemScript.PlayerName = SteamFriends.GetFriendPersonaName(mySteamId);
        myUserItemScript.PlayerState = "In Chat";
        myUserItemScript.SetFriendValues();

        return myUserListItem;
    }
}
