using System.Collections;
using System.Collections.Generic;
using Steamworks;
using UnityEngine;

public class SteamPlayerAvatar
{
    private CSteamID _playerSteamId;
    
    public SteamPlayerAvatar(CSteamID playerSteamId)
    {
        _playerSteamId = playerSteamId;
    }
    
    public Texture2D GetPlayerIcon()
    {
        int imageId = SteamFriends.GetLargeFriendAvatar(_playerSteamId);
        if (imageId == -1) return null;
        return GetSteamImageAsTexture(imageId);
    }

    private Texture2D GetSteamImageAsTexture(int iImage)
    {
        Texture2D texture = null;

        bool isValid = SteamUtils.GetImageSize(iImage, out uint width, out uint height);
        if (isValid)
        {
            byte[] image = new byte[width * height * 4];
            
            isValid = SteamUtils.GetImageRGBA(iImage, image, (int)(width * height * 4));

            if (isValid)
            {
                texture = new Texture2D((int)width, (int)height, TextureFormat.RGBA32, false, true);
                texture.LoadRawTextureData(image);
                texture.Apply();
            }
        }
        return texture;
    }
}
