using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class VoiceIconTalkItem : MonoBehaviour
{
    [SerializeField] private TMP_Text usernameText;

    private string _playerName;

    public string PlayerName
    {
        get => _playerName;
        set
        {
            if (value.Trim().Length > 0)
            {
                _playerName = value;
            }
        }
    }

    public void SetPlayerName()
    {
        usernameText.text = _playerName;
    }
}
