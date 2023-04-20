using System;
using UnityEngine;

public class ChatCommandsExecutor : MonoBehaviour
{
    public bool IsCommandExist(string messageToCheck)
    {
        if (messageToCheck.Contains("StopVoice") == true) return true;
        if (messageToCheck.Contains("playmusic") == true) return true;
        if (messageToCheck.Contains("stopmusic") == true) return true;
        
        return false;
    }
    
    public bool IsCommandExistAndRunning(string messageToCheck)
    {
        if (messageToCheck.Contains("StopVoice") == true)
        {
            ExecuteStopVoiceCommand(messageToCheck);
            return true;
        }
        if (messageToCheck.Contains("playmusic") == true)
        {
            ExecuteMusicPlayCommand(messageToCheck);
            return true;
        }
        if (messageToCheck.Contains("stopmusic") == true)
        {
            ExecuteMusicStopCommand();
            return true;
        }
        return false;
    }

    private void ExecuteStopVoiceCommand(string messageToSplit)
    {
        string[] splittedMessage = messageToSplit.Split("StopVoice ");
        Actions.OnStopVoiceChat?.Invoke(splittedMessage[1]);
    }
    
    private void ExecuteMusicPlayCommand(string messageToSplit)
    {
        string[] splittedMessage = messageToSplit.Split("playmusic ");
        int clipCount = splittedMessage.Length > 1 ? Int32.Parse(splittedMessage[1]) : 1;
        string clipName = InMenuSoundManager.Instance.PlayMusicClip(clipCount);
        MainMenuUi.Instance.ShowSongName(clipName);
    }
    
    private void ExecuteMusicStopCommand()
    {
        InMenuSoundManager.Instance.StopMusicClip();
    }
}
