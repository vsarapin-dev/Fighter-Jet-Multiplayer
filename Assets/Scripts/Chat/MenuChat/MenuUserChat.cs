using System;
using System.Collections;
using Steamworks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuUserChat : MonoBehaviour
{
    [SerializeField] private Button chatButton;
    [SerializeField] private TMP_InputField chatInputFieldField;
    [SerializeField] private Transform messagesTextListGo;
    [SerializeField] private GameObject messagePrefab;
    [SerializeField] private ScrollRect chatPanelScrollRect;
    [SerializeField] private ChatCommandsExecutor сhatCommandsExecutor;
    [SerializeField] private MenuChatUserList menuChatUserList;
    [SerializeField] private VoiceChat voiceChat;
    
    private Callback<P2PSessionRequest_t> _p2PSessionRequestCallback;
    private SteamChat _steamChat;
    private int _characterLimit = 255;
    private bool _chatOpened = false;
    
    // Messages in chat with different nickname colors
    private string _myLastMessageTextForMe;
    private string _myLastMessageTextToSend;
    
    // Nicknames colors
    private string _myMessagePersonaNameColor = "green";
    private string _otherMessagePersonaNameColor = "red";
    
    // Chat button blinking settings
    private float _chatButtonNormalTransparency = 10f / 255f;
    private float _chatButtonLowerLimitTransparency = 0f;
    private float _chatButtonUpperLimitTransparency = 140f / 255f;

    public void OnTextChange(string messageText)
    {
        bool enterPressed = messageText.EndsWith("\n");
        messageText = TrimCharacters(messageText);
        
        if (messageText.Length > _characterLimit && enterPressed == false)
        {
            messageText = messageText.Substring(0, _characterLimit);
            chatInputFieldField.text = messageText;
        }
        
        if (enterPressed == true)
        {
            messageText = messageText.Trim();
            chatInputFieldField.text = string.Empty;
            if (messageText.Length > 0)
            {
                if (сhatCommandsExecutor.IsCommandExist(messageText) == true)
                {
                    SendMessageToNetwork(messageText);
                    return;
                }
                CreateMessagePrefab(messageText, SteamFriends.GetPersonaName());
                SendMessageToNetwork(_myLastMessageTextToSend);
            }
        }
    }

    public void OnEndEdit()
    {
        voiceChat.CanActivateVoiceChat = true;
    }
    
    public void OnSelect()
    {
        voiceChat.CanActivateVoiceChat = false;
    }

    public void OnChatButtonClick()
    {
        _chatOpened = !_chatOpened;

        menuChatUserList.ShowPlayersInChat(_chatOpened);
    }

    private void OnEnable()
    {
        Actions.OnStopRemoteVoiceChat += SendMessageToNetwork;
    }
    
    private void OnDisable()
    {
        Actions.OnStopRemoteVoiceChat -= SendMessageToNetwork;
    }
    
    private void Start()
    {
        _steamChat = new SteamChat();
        _p2PSessionRequestCallback = Callback<P2PSessionRequest_t>.Create(OnP2PSessionRequest);
    }

    private void Update()
    {
        if (!SteamManager.Initialized) return;
        
        string message = _steamChat.GetFriendsMessages();
        _steamChat.GetFriendsVoiceMessage();
        ProcessMessages(message);
    }

    private void ProcessMessages(string message)
    {
        if (message == string.Empty) return;
        if (сhatCommandsExecutor.IsCommandExistAndRunning(message) == true) return;
        
        CreateMessagePrefab(message, string.Empty, true);
        if (_chatOpened == false)
        {
            InMenuSoundManager.Instance.PlayMessageClip();
            StartCoroutine(ChatBlinkingOnNewMessage());
        }
    }

    private IEnumerator ChatBlinkingOnNewMessage()
    {
        ColorBlock colors = chatButton.colors;
        Color normalAlpha = colors.normalColor;
        
        while (_chatOpened == false)
        {
            normalAlpha.a = _chatButtonLowerLimitTransparency;
            colors.normalColor = normalAlpha;
            chatButton.colors = colors;
            
            yield return new WaitForSeconds(0.4f);
            
            normalAlpha.a = _chatButtonUpperLimitTransparency;
            colors.normalColor = normalAlpha;
            chatButton.colors = colors;
            
            yield return new WaitForSeconds(0.4f);
        }
        
        normalAlpha.a = _chatButtonNormalTransparency;
        colors.normalColor = normalAlpha;
        chatButton.colors = colors;
    }

    private void CreateMessagePrefab(string text, string senderName = "", bool messageReceivedFromNetwork = false)
    {
        GameObject messageItemPrefab = Instantiate(messagePrefab, messagesTextListGo);
        messageItemPrefab.transform.localScale = Vector3.one;

        if (senderName.Length == 0 && messageReceivedFromNetwork == true)
        {
            SetMessageText(messageItemPrefab, text);
        }
        else
        {
            ComputeMessagePrefabText(messageItemPrefab, senderName, text);
        }
    }

    private void ComputeMessagePrefabText(GameObject prefab, string senderName, string text)
    {
        bool isCurrentUserSender = senderName == SteamFriends.GetPersonaName();
        string resultColorForMe = isCurrentUserSender ? _myMessagePersonaNameColor : _otherMessagePersonaNameColor;
        string resultColorToSend = isCurrentUserSender ? _otherMessagePersonaNameColor : _myMessagePersonaNameColor;
        string currentTime = DateTime.UtcNow.ToString("HH:mm:ss");
        
        _myLastMessageTextForMe = $"{currentTime} <color={resultColorForMe}>{senderName} :</color> {text}";
        _myLastMessageTextToSend = $"{currentTime} <color={resultColorToSend}>{senderName} :</color> {text}";

        SetMessageText(prefab, _myLastMessageTextForMe);
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

    private void OnP2PSessionRequest(P2PSessionRequest_t callback)
    {
        _steamChat.OnP2PSessionRequest(callback);   
    }

    private void SendMessageToNetwork(string messageToSend)
    {
        _steamChat.SendMessageToAllFriends(messageToSend);
    }
}
