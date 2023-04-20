using UnityEngine;
using Mirror;
using System;
using System.Collections;
using Steamworks;
using TMPro;
using UnityEngine.UI;

public class LobbyChat : NetworkBehaviour
{
    [SerializeField] private TMP_InputField chatInputField;
    [SerializeField] private Transform messagesTextListGo;
    [SerializeField] private GameObject messagePrefab;
    [SerializeField] private ScrollRect chatPanelScrollRect;
    [SerializeField] private VoiceChat voiceChat;
    
    private int _characterLimit = 255;
    private string _currentUserName;
    
    // Messages in chat with different nickname colors
    private string _resultTextMessage;
    
    // Nicknames colors
    private string _myMessagePersonaNameColor = "green";
    private string _otherMessagePersonaNameColor = "red";
    
    public void Start()
    {
        _currentUserName = SteamFriends.GetPersonaName();
        chatInputField.onValueChanged.AddListener(delegate { OnTextChange(chatInputField.text); });
    }
    
    public void OnEndEdit()
    {
        voiceChat.CanActivateVoiceChat = true;
    }
    
    public void OnSelect()
    {
        voiceChat.CanActivateVoiceChat = false;
    }

    public void RemoveMessages()
    {
        foreach (Transform child in messagesTextListGo.transform)
        {
            Destroy(child.gameObject);
        }
    }
    
    [Command(requiresAuthority = false)]
    private void CmdSendMessage(string message, string fromUser, ulong currentLobbyId)
    {
        RpcCreateMessagePrefab(message, fromUser, currentLobbyId);
    }
    
    [ClientRpc]
    private void RpcCreateMessagePrefab(string text, string senderName, ulong lobbyIdToCompareWith)
    {
        if ((ulong)NetworkLobbyIdentificator.Instance.CurrentLobbyId != lobbyIdToCompareWith) return;
        
        GameObject messageItemPrefab = Instantiate(messagePrefab, messagesTextListGo);
        messageItemPrefab.transform.localScale = Vector3.one;
        ComputeMessagePrefabText(messageItemPrefab, senderName, text);
    }
    
    private void OnTextChange(string messageText)
    {
        bool enterPressed = messageText.EndsWith("\n");
        messageText = TrimCharacters(messageText);
        
        if (messageText.Length > _characterLimit && enterPressed == false)
        {
            messageText = messageText.Substring(0, _characterLimit);
            chatInputField.text = messageText;
        }
        
        if (enterPressed == true)
        {
            messageText = messageText.Trim();
            chatInputField.text = string.Empty;
            if (messageText.Length > 0)
            {
                _resultTextMessage = String.Empty;
                CmdSendMessage(messageText, _currentUserName, (ulong)NetworkLobbyIdentificator.Instance.CurrentLobbyId);
            }
        }
    }

    private void ComputeMessagePrefabText(GameObject prefab, string senderName, string text)
    {
        bool isCurrentUserSender = senderName == _currentUserName;
        string resultColor = isCurrentUserSender ? _myMessagePersonaNameColor : _otherMessagePersonaNameColor;
        string currentTime = DateTime.UtcNow.ToString("HH:mm:ss");
        _resultTextMessage = $"{currentTime} <color={resultColor}>{senderName} :</color> {text}";
        SetMessageText(prefab, _resultTextMessage);
    }

    private void SetMessageText(GameObject prefab, string text)
    {
        prefab.GetComponent<TMP_Text>().text = text;
        StartCoroutine(ScrollToTop());
    }
    
    private IEnumerator ScrollToTop()
    {
        yield return new WaitForEndOfFrame();
        chatPanelScrollRect.normalizedPosition = new Vector2(0, 0);   
    }

    private string TrimCharacters(string text)
    {
        char[] charsToTrim = { ' ', '\t'};
        return text.Trim(charsToTrim);
    }
}
