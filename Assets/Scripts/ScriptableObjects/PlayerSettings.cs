using UnityEngine;

[CreateAssetMenu(fileName = "PlaneScriptableObject", menuName = "ScriptableObjects/PlayerSettings", order = 1)]
public class PlayerSettings: ScriptableObject
{
    public string playerModelName;
    public Sprite playerHealthUISprite;
    public GameObject projectilePrefab;
    public float damagePerShot;
    public float delayBetweenShots;
    public KeyCode attackKey;
}
