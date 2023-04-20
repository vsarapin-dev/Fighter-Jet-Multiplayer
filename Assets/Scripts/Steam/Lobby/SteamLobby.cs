using System;
using System.Collections;
using System.Collections.Generic;
using Michsky.UI.Shift;
using UnityEngine;
using Mirror;
using Steamworks;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SteamLobby : MonoBehaviour
{
    public static SteamLobby Instance;

    //Callbacks
    protected Callback<LobbyCreated_t> lobbyCreated;
    protected Callback<GameLobbyJoinRequested_t> joinRequest;
    protected Callback<LobbyEnter_t> lobbyEnter;
    
    //Lobbies callbacks
    protected Callback<LobbyMatchList_t> lobbyList;
    protected Callback<LobbyDataUpdate_t> lobbyDataUpdated;
    
    public List<CSteamID> lobbyIds = new List<CSteamID>();
    
    //Variables
    private ulong _currentLobbyId;
    private const string HostAddressKey = "HostAddress";
    private GameNetworkManager _manager;

    //GameObject
    private Button _openLobbyWindowOnJoinButton;
    private TMP_Text _lobbyNameText;
    
    private void Awake() 
    { 
        // If there is an instance, and it's not me, delete myself.
    
        if (Instance != null && Instance != this) 
        { 
            Destroy(this); 
        } 
        else 
        { 
            Instance = this; 
        } 
    }

    private void Start()
    {
        if (!SteamManager.Initialized) return;
        _manager = FindObjectOfType<GameNetworkManager>();

        lobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
        joinRequest = Callback<GameLobbyJoinRequested_t>.Create(OnJoinRequest);
        lobbyEnter = Callback<LobbyEnter_t>.Create(OnLobbyEnter);
        lobbyList = Callback<LobbyMatchList_t>.Create(OnLobbyMatchList);
        lobbyDataUpdated = Callback<LobbyDataUpdate_t>.Create(OnLobbyDataUpdated);
    }

    public void ClearLobbyId()
    {
        _currentLobbyId = 0;
    }

    public void HostLobby()
    {
        if (!SteamManager.Initialized) return;
        
        SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypePublic, _manager.maxConnections);
    }

    private void OnLobbyCreated(LobbyCreated_t callback)
    {
        if (callback.m_eResult != EResult.k_EResultOK) return;
        

        _manager.StartHost();

        SteamMatchmaking.SetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), HostAddressKey, SteamUser.GetSteamID().ToString());
        SteamMatchmaking.SetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), "name", SteamFriends.GetPersonaName() + "`s LOBBY");
        SteamMatchmaking.SetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), "hostName", SteamFriends.GetPersonaName());
        SteamMatchmaking.SetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), "lobbyMaxPlayers", _manager.maxConnections.ToString());
    }
    
    private void OnJoinRequest(GameLobbyJoinRequested_t callback)
    {
        SteamMatchmaking.JoinLobby(callback.m_steamIDLobby);
    }
    
    private void OnLobbyEnter(LobbyEnter_t callback)
    {
        //Everyone
        _currentLobbyId = callback.m_ulSteamIDLobby;
        NetworkLobbyIdentificator.Instance.CurrentLobbyId = new CSteamID(callback.m_ulSteamIDLobby);
        _lobbyNameText = GameObject.FindGameObjectWithTag("LobbyName").GetComponent<TMP_Text>();
        _lobbyNameText.text = SteamMatchmaking.GetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), "name");
        
        //Clients
        if (NetworkServer.active) return;
        _manager.networkAddress = SteamMatchmaking.GetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), HostAddressKey);
        _manager.StartClient();
    }

    public void GetLobbiesList()
    {
        if (lobbyIds.Count > 0) lobbyIds.Clear();

        SteamMatchmaking.RequestLobbyList();
    }
    
    private void OnLobbyMatchList(LobbyMatchList_t result)
    {
        if (LobbiesListManager.Instance.listOfLobbies.Count > 0) LobbiesListManager.Instance.DestroyLobbies();

        int cFriends = SteamFriends.GetFriendCount(EFriendFlags.k_EFriendFlagImmediate);
        for ( int i = 0; i < cFriends; i++ )
        {
            FriendGameInfo_t friendGameInfo;
            CSteamID steamIDFriend = SteamFriends.GetFriendByIndex(i, EFriendFlags.k_EFriendFlagImmediate);
            
            if (SteamFriends.GetFriendGamePlayed( steamIDFriend, out friendGameInfo) && friendGameInfo.m_steamIDLobby.IsValid())
            {
                if (new CSteamID(_currentLobbyId) != friendGameInfo.m_steamIDLobby)
                {
                    CSteamID lobbyId = friendGameInfo.m_steamIDLobby;
                    lobbyIds.Add(lobbyId);
                    SteamMatchmaking.RequestLobbyData(lobbyId);
                }
            }
        }
    }
    
    private void OnLobbyDataUpdated(LobbyDataUpdate_t result)
    {
        if (SceneManager.GetActiveScene().name != "Lobby") return;
        if (LobbiesListManager.Instance.listOfLobbies.Count > 0) LobbiesListManager.Instance.DestroyLobbies();
        LobbiesListManager.Instance.DisplayLobbies(lobbyIds, result);
    }

    public void JoinLobby(CSteamID lobbyId)
    {
        _openLobbyWindowOnJoinButton = GameObject.FindWithTag("OpenLobbyWindowOnJoinButton").GetComponent<Button>();
        _openLobbyWindowOnJoinButton.onClick.Invoke();
        if (LobbiesListManager.Instance.listOfLobbies.Count > 0) LobbiesListManager.Instance.DestroyLobbies();
        SteamMatchmaking.JoinLobby(lobbyId);
    }
}
