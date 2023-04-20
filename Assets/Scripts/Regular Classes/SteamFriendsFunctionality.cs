using System;
using Steamworks;

public class SteamFriendsFunctionality
{
    public string GetFriendsStatus(CSteamID friendSteamId)
    {
        string friendState = GetFriendsCurrentState(SteamFriends.GetFriendPersonaState(friendSteamId));
        
        string gameName = GetFriendGameNameIsPlayingNow(friendSteamId);
        friendState = SetPlayerStatusColor(friendState, gameName);
        
        return friendState;
    }
    
    public string GetFriendGameNameIsPlayingNow(CSteamID friendSteamId)
    {
        if (SteamFriends.GetFriendGamePlayed(friendSteamId, out FriendGameInfo_t friendGameInfo))
        {
            string nameBuffer;
            int length = SteamAppList.GetAppName(friendGameInfo.m_gameID.AppID(), out nameBuffer, 512);
                
            if (length != -1)
            {
                return nameBuffer;
            }
        }
        return String.Empty;
    }

    public bool FriendIsInTheSameGame(CSteamID friendSteamId)
    {
        string gameNameImPlayingNow = GetFriendGameNameIsPlayingNow(SteamUser.GetSteamID());
        string gameNameFriendPlayingNow = GetFriendGameNameIsPlayingNow(friendSteamId);;
        
        return gameNameImPlayingNow == gameNameFriendPlayingNow;
    }

    private string SetPlayerStatusColor(string friendStatus, string gameName = "")
    {
        string statusColor = "red";

        if (gameName.Length > 0)
        {
            return $"Playing in <color=green>{gameName}</color>";
        }
        
        switch (friendStatus)
        {
            case "Online":
                statusColor = "green";
                break;
            case "Offline":
                statusColor = "red";
                break;
            case "Sleeping":
                statusColor = "grey";
                break;
            case "Away":
                statusColor = "grey";
                break;
        }

        return $"<color={statusColor}>{friendStatus}</color>";
    }

    private string GetFriendsCurrentState(EPersonaState currentState)
    {
        string state = "Offline";
        switch (currentState)
        {
            case EPersonaState.k_EPersonaStateOnline:
                state = "Online";
                break;
            case EPersonaState.k_EPersonaStateOffline:
                state = "Offline";
                break;
            case EPersonaState.k_EPersonaStateSnooze:
                state = "Sleeping";
                break;
            case EPersonaState.k_EPersonaStateAway:
                state = "Away";
                break;
        }

        return state;
    }  
}
