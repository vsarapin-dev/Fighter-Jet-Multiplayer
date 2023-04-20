using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SteamBottomPanelLobbyMember : MonoBehaviour
{
    [SerializeField] private RawImage playerIcon;

    public void SetPlayerIcon(Texture2D iconTexture)
    {
        playerIcon.texture = iconTexture;
    }
}
