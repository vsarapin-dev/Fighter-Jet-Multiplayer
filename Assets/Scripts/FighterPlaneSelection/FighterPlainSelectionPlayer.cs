using System.Collections.Generic;
using Mirror;
using Steamworks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FighterPlainSelectionPlayer : NetworkBehaviour
{
    [SyncVar(hook = nameof(PlayerNameUpdate))] private string _playerName;
    [SyncVar] private bool _hasPlayerSelectHelicopter;
    [SyncVar] private int _connectionId;
    
    public string PlayerName => _playerName;
    public int ConnectionID
    {
        get => _connectionId;
        set => _connectionId = value;
    }
    public bool HasPlayerSelectHelicopter
    {
        get => _hasPlayerSelectHelicopter;
        set => _hasPlayerSelectHelicopter = value;
    }
    
    private GameNetworkManager _manager;

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
    
    public override void OnStartAuthority()
    {
        if (SteamManager.Initialized == false) return;

        base.OnStartAuthority();
        gameObject.name = "LocalGamePlayer";
        CmdSetPlayerName(SteamFriends.GetPersonaName());
    }
    
    public override void OnStartClient()
    {
        Manager.HelicopterSelectionPlayers.Add(this);
    }

    private void PlayerNameUpdate(string oldName, string newName)
    {
        _playerName = newName;
    }

    private void ClearPlayerList()
    {
        foreach (FighterPlainSelectionPlayer player in new List<FighterPlainSelectionPlayer>(Manager.HelicopterSelectionPlayers))
        {
            Manager.HelicopterSelectionPlayers.Remove(player);
        }
    }

    #region Server

    [Command]
    public void CmdRemoveFromPlayerList(bool hostIsLeaving)
    {
        RpcRemoveFromPlayerList(_playerName, hostIsLeaving);
    }

    [Command]
    public void CmdSetPlayerHasSelectFighterPlain(bool playerHasSelectHelicopter)
    {
        _hasPlayerSelectHelicopter = playerHasSelectHelicopter;
        
        if (_hasPlayerSelectHelicopter == true) RpcShowPlayerHasSelectHelicopter();
    }

    [Command]
    private void CmdSetPlayerName(string playerName)
    {
        PlayerNameUpdate(_playerName, playerName);
    }

    [Command]
    private void CmdCheckAllPlayersAreReady()
    {
        bool allPlayersAreReady = false;
        foreach (FighterPlainSelectionPlayer player in Manager.HelicopterSelectionPlayers)
        {
            allPlayersAreReady = player.HasPlayerSelectHelicopter;
            if (allPlayersAreReady == false) break;
        }

        if (allPlayersAreReady == true) RpcCountDownTimerToStart();
    }
    
    [Command]
    public void CmdLoadRaceTrack()
    {
        if (isServer == true)
        {
            Manager.ServerChangeScene("RaceTrack");
        }
    }
    
    #endregion

    #region Client
    
    [ClientRpc]
    private void RpcRemoveFromPlayerList(string playerName, bool hostIsLeaving)
    {
        if (hostIsLeaving == true)
        {
            ClearPlayerList();
            SceneManager.LoadScene("Lobby");
            return;
        }
        
        foreach (FighterPlainSelectionPlayer player in new List<FighterPlainSelectionPlayer>(Manager.HelicopterSelectionPlayers))
        {
            if (player.PlayerName == playerName)
            {
                Manager.HelicopterSelectionPlayers.Remove(player);
            }
        }
    }
    
    [ClientRpc]
    private void RpcShowPlayerHasSelectHelicopter()
    {
        Actions.OnFighterPlainSelected?.Invoke(_playerName);

        if (hasAuthority == false) return;
        CmdCheckAllPlayersAreReady();
    }
    
    [ClientRpc]
    private void RpcCountDownTimerToStart()
    {
        Actions.OnAllPlayersSelectFighterPlain?.Invoke();
    }
    
    #endregion


    
}
