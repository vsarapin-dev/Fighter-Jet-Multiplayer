using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Mono.CSharp;
using Steamworks;
using UnityEngine.SceneManagement;

public class PlayerObjectController : NetworkBehaviour
{
   //Player data
   [SyncVar] public int ConnectionID;
   [SyncVar] public int PlayerIdNumber;
   [SyncVar] public ulong PlayerSteamID;
   [SyncVar(hook = nameof(PlayerNameUpdate))] public string PlayerName;

   private GameNetworkManager _manager;
   private bool _sceneChanges;

   public bool SceneChanges
   {
      get => _sceneChanges;
      set => _sceneChanges = value;
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

   public override void OnStartAuthority()
   {
      _sceneChanges = false;
      base.OnStartAuthority();
      CmdSetPlayerName(SteamFriends.GetPersonaName());
      gameObject.name = "LocalGamePlayer";
      Lobby.Instance.UpdateLobbyName();
   }

   public override void OnStartClient()
   {
      Manager.GamePlayers.Add(this);
      Lobby.Instance.UpdateLobbyName();
      Lobby.Instance.UpdatePlayerList();
   }

   public override void OnStartLocalPlayer()
   {
      CmdSetReadyStatusForServer();
   }

   public override void OnStopClient()
   {
      Manager.GamePlayers.Remove(this);
      
      if (_sceneChanges == true) return;
      
      Lobby.Instance.UpdatePlayerList();
      if (Manager.GamePlayers.Count == 0) Lobby.Instance.LeaveLobby(false);
   }

   [Command]
   private void CmdSetReadyStatusForServer()
   {
      string hostName = SteamMatchmaking.GetLobbyData(NetworkLobbyIdentificator.Instance.CurrentLobbyId, "hostName");
      RpcUpdateStatusOnClients(hostName, true);
   }

   [Command]
   public void CmdUpdatePlayerReadyStatus(string playerName)
   {
      RpcUpdateStatusOnClients(playerName, false);
   }

   [Command]
   private void CmdSetPlayerName(string playerName)
   {
      this.PlayerNameUpdate(this.PlayerName, playerName);
   }

   [Command]
   public void CmdStartGame()
   {
      if (isServer)
      {
         bool allPlayersAreReady = true;
         foreach (PlayerListItem playerListItem in Lobby.Instance.PlayerListItems)
         {
            allPlayersAreReady = playerListItem.PlayerLobbyReadyStatus;
            if (allPlayersAreReady == false) return;
         }
         
         Manager.ServerChangeScene("FighterPlaneSelection");
         //if (Lobby.Instance.PlayerListItems.Count > 1)
         //{
            //RpcLeaveLobbyBeforeSceneSwitch(new CSteamID(SteamLobby.Instance.CurrentLobbyId));
            //Manager.ServerChangeScene("FighterPlaneSelection");
         //}
      }
   }

   [ClientRpc]
   private void RpcLeaveLobbyBeforeSceneSwitch(CSteamID lobbyId)
   {
      if (LobbiesListManager.Instance.listOfLobbies.Count > 0) LobbiesListManager.Instance.DestroyLobbies();
      SteamMatchmaking.LeaveLobby(lobbyId);
      NetworkLobbyIdentificator.Instance.ClearLobbyId();
   }

   [ClientRpc]
   private void RpcUpdateStatusOnClients(string playerName, bool isHost)
   {
      foreach (PlayerListItem playerListItem in Lobby.Instance.PlayerListItems)
      {
         if (playerListItem.PlayerName == playerName)
         {
            if (isHost)
            {
               playerListItem.PlayerLobbyReadyStatus = true;
               playerListItem.SetPlayerStatusText(playerListItem.PlayerLobbyReadyStatus);
               return;
            }
            
            playerListItem.PlayerLobbyReadyStatus = !playerListItem.PlayerLobbyReadyStatus;

            if (SteamFriends.GetPersonaName() == playerName)
            {
               LobbyUiManager lobbyUiManager = FindObjectOfType<LobbyUiManager>();
               lobbyUiManager.ChangeReadyButtonText(playerListItem.PlayerLobbyReadyStatus);
            }
         }
         playerListItem.SetPlayerStatusText(playerListItem.PlayerLobbyReadyStatus);
      }
   }
   
   public void PlayerNameUpdate(string oldValue, string newValue)
   {
      if (isServer)
      {
         this.PlayerName = newValue;
      }
      
      if (isClient)
      {
         Lobby.Instance.UpdatePlayerList();
      }
   }
   
}
