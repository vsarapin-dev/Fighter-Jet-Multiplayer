using TMPro;
using UnityEngine;

public class PlayerInfoDebug : MonoBehaviour
{
    [SerializeField] private TMP_Text playerInfoText;
    
    public void SetPlayerInfoValues(string playerName, string fighterPlaneName, float health , bool isAlive)
    {
        fighterPlaneName = fighterPlaneName.Replace("(Clone)", "").Trim();

        string deadOrNot = isAlive ? $"<color=green>alive</color> and has {health} health." : "<color=red>dead</color>.";
        
        string startOfString = $"<color=green>{playerName}</color> flying on {fighterPlaneName}. Player is";
        string endOfString = deadOrNot;
        string finalString = $"{startOfString} {endOfString}";
        playerInfoText.text = finalString;
    }
    
    public void SetUserRamValues(string ramUsed)
    {
        playerInfoText.text = $"RAM usage: {ramUsed}";
    }
}
