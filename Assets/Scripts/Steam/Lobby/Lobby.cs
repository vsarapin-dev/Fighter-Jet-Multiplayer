using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using Michsky.UI.Shift;
using Mirror;
using UnityEngine;
using Steamworks;
using TMPro;
using UnityEngine.UI;

public class Lobby : MonoBehaviour
{
    [SerializeField] private LobbyUiManager lobbyUiManager;
    [SerializeField] private TMP_Text lobbyNameText;
    [SerializeField] private GameObject playerListItemPrefab;
    [SerializeField] private GameObject playerListViewContent;
    [SerializeField] private GameObject bottomPlayerListViewContent;
    [SerializeField] private GameObject bottomPanelListItemPrefab;
    [SerializeField] private GameObject buttonHostGame;
    [SerializeField] private GameObject buttonReturnToLobby;
    [SerializeField] private Button buttonReturnHome;
    [SerializeField] private Button closeLobbyWindowOnLeaveJoinButton;
    [SerializeField] private Button openLobbyWindowOnJoinButton;
    [SerializeField] private LobbyChat lobbyChat;
    [SerializeField] private TMP_Dropdown resolutionDropdown;

    private ulong _currentLobbyId;
    private List<PlayerListItem> _playerListItems = new List<PlayerListItem>();
    private List<SteamBottomPanelLobbyMember> _playerBottomListItems = new List<SteamBottomPanelLobbyMember>();
    private GameNetworkManager _manager;
    private PlayerObjectController _localPlayerController;
    private float _waitTimeBeforeCanGoBack = 2f;
    private bool _gameIsHosted;
    private bool _gameIsJoined;
    private bool _needChangeButtonsAfterHost;
    private bool _needChangeButtonsAfterJoin;
    private bool _playerItemCreated;
    private bool _canPressButtons;
    
    public static Lobby Instance;

    public List<PlayerListItem> PlayerListItems
    {
        get => _playerListItems;
        set => _playerListItems = value;
    }
    public bool GameIsJoined
    {
        set => _gameIsJoined = value;
    }
    public bool NeedChangeButtonsAfterJoin
    {
        set => _needChangeButtonsAfterJoin = value;
    }
    
    private GameNetworkManager Manager
    {
        get
        {
            if (_manager != null)
            {
                return _manager;
            }

            return _manager = GameNetworkManager.singleton as GameNetworkManager;
        }
    }

    private void Awake()
    {
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

        StopNetworkAndLobbyOnStart();
        ResetLobbyValues();
        lobbyUiManager.UnlockCursor();
        LobbiesListManager.Instance.GetListOfLobbies();
    }
    
    private void Update()
    {
        ProcessGameButtons();
        if (Input.GetKeyDown(KeyCode.Escape)) ProcessCollapseLobby();
    }

    public void HostLobby()
    {
        SteamLobby.Instance.HostLobby();
        HideStartHostButton();
        _gameIsHosted = true;
        _needChangeButtonsAfterHost = true;
        ButtonsNotAvailableTimer();
    }
    
    public void LeaveLobby(bool shouldCheckCanGoBack = true)
    {
        if (shouldCheckCanGoBack && _canPressButtons == false) return;
        
        if (_gameIsHosted || _gameIsJoined)
        {
            StopClientOrHostOnButtonClick();
            ResetLobbyValues();
        }
    }

    public void HideStartHostButton()
    {
        buttonHostGame.SetActive(false);
        buttonReturnToLobby.SetActive(true);
    }

    private void ShowStartHostButton()
    {
        buttonHostGame.SetActive(true);
        buttonReturnToLobby.SetActive(false);
    }
    
    public void UpdateLobbyName()
    {
        _currentLobbyId = (ulong)NetworkLobbyIdentificator.Instance.CurrentLobbyId;
        lobbyNameText.text = SteamMatchmaking.GetLobbyData(new CSteamID(_currentLobbyId), "name");
    }

    public void UpdatePlayerList()
    {
        if (!_playerItemCreated) CreateHostPlayerItem();
        if (_playerListItems.Count < Manager.GamePlayers.Count) CreateClientPlayerItem();
        if (_playerListItems.Count > Manager.GamePlayers.Count) RemovePlayerItem();
        if (_playerListItems.Count == Manager.GamePlayers.Count) UpdatePlayerItem();
    }

    public PlayerObjectController FindLocalPlayer()
    {
        if (_localPlayerController) return _localPlayerController;

        GameObject localPlayerObject = GameObject.Find("LocalGamePlayer");
        _localPlayerController = localPlayerObject.GetComponent<PlayerObjectController>();

        return _localPlayerController;
    }

    private void CreateHostPlayerItem()
    {
        foreach (PlayerObjectController player in Manager.GamePlayers)
        {
            SetNewPlayerListItem(player);
            _playerItemCreated = true;
        }
    }
    
    private void CreateClientPlayerItem()
    {
        foreach (PlayerObjectController player in Manager.GamePlayers)
        {
            if (!_playerListItems.Any(b => b.ConnectionID == player.ConnectionID))
            {
                SetNewPlayerListItem(player);
            }
        }
    }
    
    private void UpdatePlayerItem()
    {
        foreach (PlayerObjectController player in Manager.GamePlayers)
        {
            foreach (PlayerListItem playerListItemScript in _playerListItems)
            {
                if (playerListItemScript.ConnectionID == player.ConnectionID)
                {
                    playerListItemScript.PlayerName = player.PlayerName;
                    playerListItemScript.SetPLayerValues();
                }
            }
        }
    }
    
    private void RemovePlayerItem()
    {
        List<PlayerListItem> playerListItemsToRemove = new List<PlayerListItem>();
        
        foreach (PlayerListItem playerListItem in _playerListItems)
        {
            if (!Manager.GamePlayers.Any(b => b.ConnectionID == playerListItem.ConnectionID))
            {
                playerListItemsToRemove.Add(playerListItem);
            }
        }

        if (playerListItemsToRemove.Count > 0)
        {
            foreach (PlayerListItem playerListItemToRemove in playerListItemsToRemove)
            {
                GameObject objectToRemove = playerListItemToRemove.gameObject;
                _playerListItems.Remove(playerListItemToRemove);
                Destroy(objectToRemove);
                objectToRemove = null;
            }
        }
    }

    public void ButtonsNotAvailableTimer()
    {
        StartCoroutine(CanPressButtonsTimer());
    }

    private void StopNetworkAndLobbyOnStart()
    {
        if (NetworkServer.active)
        {
            Manager.StopHost();
        }
        else
        {
            Manager.StopClient();
        }
        
        if (LobbiesListManager.Instance.listOfLobbies.Count > 0) LobbiesListManager.Instance.DestroyLobbies();
        SteamMatchmaking.LeaveLobby(NetworkLobbyIdentificator.Instance.CurrentLobbyId);
        NetworkLobbyIdentificator.Instance.ClearLobbyId();

    }

    private void ProcessCollapseLobby()
    {
        closeLobbyWindowOnLeaveJoinButton.onClick.Invoke();
        buttonReturnHome.onClick.Invoke();
    }

    private IEnumerator CanPressButtonsTimer()
    {
        _canPressButtons = false;
        yield return new WaitForSeconds(_waitTimeBeforeCanGoBack);
        _canPressButtons = true;
    }

    private void ResetBoolValues()
    {
        _needChangeButtonsAfterHost = false;
        _needChangeButtonsAfterJoin = false;
        _gameIsHosted = false;
        _gameIsJoined = false;
    }

    private void StopClientOrHostOnButtonClick()
    {
        if (_gameIsHosted)
        {
            Manager.StopHost();
        }
        else if (_gameIsJoined)
        {
            Manager.StopClient();
        }
        
        if (LobbiesListManager.Instance.listOfLobbies.Count > 0) LobbiesListManager.Instance.DestroyLobbies();
        SteamMatchmaking.LeaveLobby(NetworkLobbyIdentificator.Instance.CurrentLobbyId);
        NetworkLobbyIdentificator.Instance.ClearLobbyId();
    }

    private void ResetLobbyValues()
    {
        lobbyUiManager.ResetButtonsToStartValues();
        lobbyChat.RemoveMessages();
        ShowStartHostButton();
        ResetBoolValues();
        ProcessCollapseLobby();
    }
    
    private void ProcessGameButtons()
    {
        if (_canPressButtons == false) return;
        
        if (_gameIsHosted && _needChangeButtonsAfterHost)
        {
            lobbyUiManager.EnableNecessaryButtons(_gameIsHosted, true);
            _needChangeButtonsAfterHost = false;
        }
        
        if (_gameIsJoined && _needChangeButtonsAfterJoin)
        {
            lobbyUiManager.EnableNecessaryButtons();
            _needChangeButtonsAfterJoin = false;
        }
    }
    
    private void SetNewPlayerListItem(PlayerObjectController player)
    {
        CreatePlayerItemForLobbyWindow(player);
        //CreatePlayerItemForBottomPanel(player);
    }

    private void CreatePlayerItemForLobbyWindow(PlayerObjectController player)
    {
        if (playerListViewContent == null)
        {
            playerListViewContent = GameObject.FindWithTag("PlayerListViewContent");
        }

        GameObject newPlayerItem = Instantiate(playerListItemPrefab);
        PlayerListItem newPlayerListItemScript = newPlayerItem.GetComponent<PlayerListItem>();

        newPlayerListItemScript.PlayerName = player.PlayerName;
        newPlayerListItemScript.ConnectionID = player.ConnectionID;
        newPlayerListItemScript.PlayerSteamID = player.PlayerSteamID;
        newPlayerListItemScript.PlayerIdNumber = player.PlayerIdNumber;
        newPlayerListItemScript.SetPLayerValues();
            
        newPlayerItem.transform.SetParent(playerListViewContent.transform);
        newPlayerItem.transform.localScale = Vector3.one;
        newPlayerItem.transform.SetSiblingIndex(1);
        
        _playerListItems.Add(newPlayerListItemScript);
    }
    
    private void CreatePlayerItemForBottomPanel(PlayerObjectController player)
    {
        if (bottomPlayerListViewContent == null)
        {
            bottomPlayerListViewContent = GameObject.FindWithTag("BottomPlayerListViewContent");
        }
        
        SteamPlayerAvatar steamPlayerAvatar = new SteamPlayerAvatar((CSteamID) player.PlayerSteamID);
        Texture2D userAvatar = steamPlayerAvatar.GetPlayerIcon();
        if (userAvatar != null)
        {
            GameObject newPlayerItem = Instantiate(bottomPanelListItemPrefab);
            SteamBottomPanelLobbyMember steamBottomPanelLobbyMemberScript =
                newPlayerItem.GetComponent<SteamBottomPanelLobbyMember>();
            steamBottomPanelLobbyMemberScript.SetPlayerIcon(userAvatar);
            
            newPlayerItem.transform.SetParent(bottomPlayerListViewContent.transform);
            newPlayerItem.transform.localScale = Vector3.one;
            newPlayerItem.transform.SetSiblingIndex(1);
        
            _playerBottomListItems.Add(steamBottomPanelLobbyMemberScript);
        }
    }
}