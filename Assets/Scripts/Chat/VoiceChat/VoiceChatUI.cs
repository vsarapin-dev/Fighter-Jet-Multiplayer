using System;
using System.Collections;
using System.Collections.Generic;
using Steamworks;
using TMPro;
using UnityEngine;

public class VoiceChatUI : MonoBehaviour
{
    [SerializeField] private Transform voiceIconsParent;
    [SerializeField] private GameObject voiceIconTalkPrefab;

    private List<GameObject> _voiceIconsPrefabs = new List<GameObject>();
    
    private void OnEnable()
    {
        Actions.OnStartVoiceChat += ShowVoiceIconTalk;
        Actions.OnStopVoiceChat += DeleteVoiceIcon;
        Actions.OnRemoveAllVoiceChatsIcons += DeleteAllVoiceIconsExceptMine;
    }

    private void OnDisable()
    {
        Actions.OnStartVoiceChat -= ShowVoiceIconTalk;
        Actions.OnStopVoiceChat -= DeleteVoiceIcon;
        Actions.OnRemoveAllVoiceChatsIcons -= DeleteAllVoiceIconsExceptMine;
    }
    
    public void ShowVoiceIconTalk(string userName)
    {
        CreateVoiceIcon(userName);
    }

    private void CreateVoiceIcon(string userName)
    {
        if(CheckGameObjectIfAlreadyExists(userName) == true) return;
        
        GameObject voiceIconTalk = Instantiate(voiceIconTalkPrefab, voiceIconsParent);
        voiceIconTalk.transform.localScale = Vector3.one;

        VoiceIconTalkItem voiceIconTalkScript = voiceIconTalk.GetComponent<VoiceIconTalkItem>();
        voiceIconTalkScript.PlayerName = userName;
        voiceIconTalkScript.SetPlayerName();
        _voiceIconsPrefabs.Add(voiceIconTalk);
    }
    
    private void DeleteVoiceIcon(string userName)
    {
        for (int i = 0; i < _voiceIconsPrefabs.Count; i++)
        {
            VoiceIconTalkItem voiceIconScript = _voiceIconsPrefabs[i].GetComponent<VoiceIconTalkItem>();
            if (voiceIconScript.PlayerName == userName)
            {
                Destroy(_voiceIconsPrefabs[i]);
                _voiceIconsPrefabs.RemoveAt(i);
            }
        }
    }
    
    private void DeleteAllVoiceIconsExceptMine()
    {
        string myUserName = SteamFriends.GetPersonaName();
        for (int i = 0; i < _voiceIconsPrefabs.Count; i++)
        {
            VoiceIconTalkItem voiceIconScript = _voiceIconsPrefabs[i].GetComponent<VoiceIconTalkItem>();
            if (voiceIconScript.PlayerName != myUserName)
            {
                Destroy(_voiceIconsPrefabs[i]);
                _voiceIconsPrefabs.RemoveAt(i);
            }
        }
    }

    private bool CheckGameObjectIfAlreadyExists(string userName)
    {
        if (_voiceIconsPrefabs.Count == 0) return false;

        foreach (GameObject voiceIconPrefab in _voiceIconsPrefabs)
        {
            VoiceIconTalkItem voiceIconPrefabScript = voiceIconPrefab.GetComponent<VoiceIconTalkItem>();
            if (voiceIconPrefabScript.PlayerName == userName)
            {
                return true;
            }
        }

        return false;
    }
}
