using System.Collections.Generic;
using FighterPlane;
using Michsky.UI.Shift;
using Mirror;
using RaceTrackScene;
using UnityEngine;
using UnityEngine.SceneManagement;
using Steamworks;

public class GameNetworkManager : NetworkManager
{
    [Header("Player prefabs")]
    [SerializeField] private PlayerObjectController gamePlayerPrefab;
    [SerializeField] private FighterPlainSelectionPlayer fighterPlainSelectionPlayerPrefab;
    [SerializeField] private RaceTrackPlayer raceTrackPlayerPrefab;
    
    public List<PlayerObjectController> GamePlayers { get; } = new List<PlayerObjectController>();
    public List<FighterPlainSelectionPlayer> HelicopterSelectionPlayers { get; } = new List<FighterPlainSelectionPlayer>();

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        AddPlayerForLobbyScene(conn);
        AddPlayerForHelicopterSelectionScene(conn);
        AddPlayerForRaceTrackScene(conn);
        //Debug.Log($"im server and on a server " + $"there are {numPlayers} players");
    }

    public override void OnServerChangeScene(string newSceneName)
    {
        HelicopterSelectionPlayerSendSignal(newSceneName);
    }

    public override void OnClientChangeScene(string newSceneName, SceneOperation sceneOperation, bool customHandling)
    {
        HelicopterSelectionPlayerSendSignal(newSceneName);
    }

    private void AddPlayerForLobbyScene(NetworkConnectionToClient conn)
    {
        if (SceneManager.GetActiveScene().name == "Lobby")
        {
            PlayerObjectController gamePlayerInstance = Instantiate(gamePlayerPrefab);
            gamePlayerInstance.ConnectionID = conn.connectionId;
            gamePlayerInstance.PlayerIdNumber = GamePlayers.Count + 1;
            gamePlayerInstance.PlayerSteamID =
                (ulong) SteamMatchmaking.GetLobbyMemberByIndex(NetworkLobbyIdentificator.Instance.CurrentLobbyId, GamePlayers.Count);
            
            NetworkServer.AddPlayerForConnection(conn, gamePlayerInstance.gameObject);
        }
    }
    
    private void AddPlayerForHelicopterSelectionScene(NetworkConnectionToClient conn)
    {
        if (SceneManager.GetActiveScene().name == "FighterPlaneSelection")
        {
            FighterPlainSelectionPlayer player = Instantiate(fighterPlainSelectionPlayerPrefab);
            player.ConnectionID = conn.connectionId;
            NetworkServer.AddPlayerForConnection(conn, player.gameObject);
        }
    }
    
    private void AddPlayerForRaceTrackScene(NetworkConnectionToClient conn)
    {
        if (SceneManager.GetActiveScene().name == "RaceTrack")
        {
            HelicopterSelectionPlayers.Clear();
            
            RaceTrackPlayer player = Instantiate(raceTrackPlayerPrefab);
            NetworkServer.AddPlayerForConnection(conn, player.gameObject);
        }
    }

    private void HelicopterSelectionPlayerSendSignal(string newSceneName)
    {
        if (newSceneName == "FighterPlaneSelection")
        {
            SteamMatchmaking.LeaveLobby(NetworkLobbyIdentificator.Instance.CurrentLobbyId);
            NetworkLobbyIdentificator.Instance.ClearLobbyId();

            PlayerObjectController[] players = FindObjectsOfType<PlayerObjectController>();
            foreach (PlayerObjectController player in players)
            {
                player.SceneChanges = true; 
            }
            
            GamePlayers.Clear();
        }
    }
}
