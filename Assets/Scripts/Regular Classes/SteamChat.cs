using System;
using Steamworks;
using Unity.VisualScripting;
using UnityEngine;

public class SteamChat
{
    private Callback<P2PSessionRequest_t> _p2PSessionRequestCallback;
    private SteamFriendsFunctionality _steamFriendsFunctionality;

    public SteamChat()
    {
        _steamFriendsFunctionality = new SteamFriendsFunctionality();
        _p2PSessionRequestCallback = Callback<P2PSessionRequest_t>.Create(OnP2PSessionRequest);
    }

    public void SendMessageToAllFriends(string message)
    {
        int friendCount = SteamFriends.GetFriendCount(EFriendFlags.k_EFriendFlagImmediate);
        for (int i = 0; i < friendCount; i++)
        {
            CSteamID friendSteamId = SteamFriends.GetFriendByIndex(i, EFriendFlags.k_EFriendFlagImmediate);
            
            if (_steamFriendsFunctionality.FriendIsInTheSameGame(friendSteamId) == false) continue;
            
            CSteamID receiver = friendSteamId;

            byte[] bytes = new byte[message.Length * sizeof(char)];
            Buffer.BlockCopy(message.ToCharArray(), 0, bytes, 0, bytes.Length);
            SteamNetworking.SendP2PPacket(receiver, bytes, (uint) bytes.Length, EP2PSend.k_EP2PSendReliable);
        }
    }
    
    public void SendMessageToAllFriends(byte[] data, uint size)
    {
        int friendCount = SteamFriends.GetFriendCount(EFriendFlags.k_EFriendFlagImmediate);
        for (int i = 0; i < friendCount; i++)
        {
            CSteamID friendSteamId = SteamFriends.GetFriendByIndex(i, EFriendFlags.k_EFriendFlagImmediate);
            
            if (_steamFriendsFunctionality.FriendIsInTheSameGame(friendSteamId) == false) continue;
            
            CSteamID receiver = friendSteamId;
            SteamNetworking.SendP2PPacket(receiver, data, size, EP2PSend.k_EP2PSendReliable, 1);
        }
    }

    public string GetFriendsMessages()
    {
        uint size;
        
        while (SteamNetworking.IsP2PPacketAvailable(out size))
        {
            var buffer = new byte[size];
            uint bytesRead;
            CSteamID remoteId;
            
            if (SteamNetworking.ReadP2PPacket(buffer, size, out bytesRead, out remoteId))
            {
                char[] chars = new char[bytesRead / sizeof(char)];
                Buffer.BlockCopy(buffer, 0, chars, 0, Buffer.ByteLength(chars));
                
                string message = new string(chars, 0, chars.Length);

                return message;

            }
        }
        return string.Empty;
    }
    
    public void GetFriendsVoiceMessage()
    {
        uint size;

        while (SteamNetworking.IsP2PPacketAvailable(out size, 1))
        {
            var buffer = new byte[size];
            uint bytesRead;
            CSteamID remoteId;
            
            if (SteamNetworking.ReadP2PPacket(buffer, size, out bytesRead, out remoteId, 1))
            {
                Actions.OnStartVoiceChat?.Invoke(SteamFriends.GetFriendPersonaName(remoteId));
                InMenuSoundManager.Instance.PlayVoiceChat(buffer);
            }
        }
    }
    
    public void OnP2PSessionRequest(P2PSessionRequest_t request)
    {
        CSteamID clientId = request.m_steamIDRemote;
        SteamNetworking.AcceptP2PSessionWithUser(clientId);
    }
}