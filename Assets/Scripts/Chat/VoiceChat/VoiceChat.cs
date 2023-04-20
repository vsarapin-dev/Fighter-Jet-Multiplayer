using Steamworks;
using UnityEngine;

public class VoiceChat : MonoBehaviour
{
    private SteamChat _steamChat;
    private bool _canActivateVoiceChat = true;

    public bool CanActivateVoiceChat
    {
        get => _canActivateVoiceChat;
        set => _canActivateVoiceChat = value;
    }

    private void Start()
    {
        _steamChat = new SteamChat();
    }

    private void Update()
    {
        if (!SteamManager.Initialized) return;
        
        if (_canActivateVoiceChat == false) return;
        
        if (Input.GetKeyDown(KeyCode.V))
        {
            Actions.OnStartVoiceChat?.Invoke(SteamFriends.GetPersonaName());
            SteamUser.StartVoiceRecording();
        }
        else if (Input.GetKeyUp(KeyCode.V))
        {
            Actions.OnStopVoiceChat?.Invoke(SteamFriends.GetPersonaName());
            Actions.OnStopRemoteVoiceChat?.Invoke($"StopVoice {SteamFriends.GetPersonaName()}");
            SteamUser.StopVoiceRecording();
        }
 
        uint Compressed;
        EVoiceResult ret = SteamUser.GetAvailableVoice(out Compressed);
        if(ret == EVoiceResult.k_EVoiceResultOK && Compressed > 1024) 
        {
            byte[] DestBuffer = new byte[1024];
            uint BytesWritten;
            ret = SteamUser.GetVoice(true, DestBuffer, 1024, out BytesWritten);
            if(ret == EVoiceResult.k_EVoiceResultOK && BytesWritten > 0)
            {
                _steamChat.SendMessageToAllFriends(DestBuffer, BytesWritten);
            }
        }
    }
}
 
 