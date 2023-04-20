using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using QFSW.QC;
using UnityEngine;
using Steamworks;
using UnityEngine.SceneManagement;

public class LobbiesListManager : MonoBehaviour
{
    public static LobbiesListManager Instance;
    
    public GameObject lobbyDataItemPrefab;
    public GameObject lobbyListContent;
    
    public List<GameObject> listOfLobbies = new List<GameObject>();

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

    public void DisplayLobbies(List<CSteamID> lobbyIds, LobbyDataUpdate_t result)
    {
        for (int i = 0; i < lobbyIds.Count; i++)
        {
            if (lobbyIds[i].m_SteamID == result.m_ulSteamIDLobby)
            {
                SteamNetworkPingLocation_t pingResult;
                float ping = SteamNetworkingUtils.GetLocalPingLocation(out pingResult);
                GameObject createdItem = Instantiate(lobbyDataItemPrefab);

                createdItem.GetComponent<LobbyEntryItem>().LobbyId = (CSteamID)lobbyIds[i].m_SteamID;
                createdItem.GetComponent<LobbyEntryItem>().LobbyName =
                    SteamMatchmaking.GetLobbyData((CSteamID)lobbyIds[i].m_SteamID, "name");
                createdItem.GetComponent<LobbyEntryItem>().LobbyPing = ((int)ping).ToString();
                createdItem.GetComponent<LobbyEntryItem>().LobbyMaxPlayers = SteamMatchmaking.GetLobbyData(lobbyIds[i], "lobbyMaxPlayers");
                createdItem.GetComponent<LobbyEntryItem>().LobbyCurrentPlayers = SteamMatchmaking.GetNumLobbyMembers(lobbyIds[i]).ToString();
                createdItem.GetComponent<LobbyEntryItem>().SetLobbyData();
                createdItem.transform.SetParent(lobbyListContent.transform);
                createdItem.transform.localScale = Vector3.one;
                listOfLobbies.Add(createdItem);
            }
        }
    }

    public void GetListOfLobbies()
    {
        SteamLobby.Instance.GetLobbiesList();
    }

    public void DestroyLobbies()
    {
        foreach (GameObject lobbyItem in listOfLobbies)
        {
            Destroy(lobbyItem);
        }
        listOfLobbies.Clear();
    }
}
