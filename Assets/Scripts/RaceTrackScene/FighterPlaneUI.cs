using System.Collections;
using DG.Tweening;
using Enums;
using StateBehaviours.FighterPlane;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RaceTrackScene
{
    public class FighterPlaneUI : MonoBehaviour
    {
        [Header("Sprites")]
        [SerializeField] private Image currentPlaneSprite;
        [SerializeField] private Image enemyPlaneSprite;
    
        [Header("Health sprites filled")]
        [SerializeField] private Image currentPlaneSpriteForHealthFilled;
        [SerializeField] private Image enemyPlaneSpriteForHealthFilled;

        [Header("GameObjects")]
        [SerializeField] private GameObject currentPlayerPlaneImage;
        [SerializeField] private GameObject enemyPlayerPlaneImage;
    
        [Header("Player Name Text")]
        [SerializeField] private TMP_Text currentPlayerNameText;
        [SerializeField] private TMP_Text enemyPlayerNameText;
    
        [Header("Player Plane Name Text")]
        [SerializeField] private TMP_Text currentPlayerPlaneNameText;
        [SerializeField] private TMP_Text enemyPlayerPlaneNameText;
    
        [Header("Low HP warning")]
        [SerializeField] private GameObject lowHpWarningGo;
    
        [Header("Catapult Message")]
        [SerializeField] private GameObject catapultMessageGo;
    
        [Header("Radar")]
        [SerializeField] private GameObject fighterPlaneRadarUI;

        private void OnEnable()
        {
            Actions.OnChangeCurrentPlayerHealthOnUi += ChangeCurrentPlayerHealthOnUi;
            Actions.OnChangeEnemyPlayerHealthOnUi += ChangeEnemyPlayerHealthOnUi;
            Actions.OnFighterPlaneRadarActivate += FighterPlaneRadarUI;
        }

        private void OnDisable()
        {
            Actions.OnChangeCurrentPlayerHealthOnUi -= ChangeCurrentPlayerHealthOnUi;
            Actions.OnChangeEnemyPlayerHealthOnUi -= ChangeEnemyPlayerHealthOnUi;
            Actions.OnFighterPlaneRadarActivate -= FighterPlaneRadarUI;
        }

        public void SetCurrentPlayerHealthUiSprite(Sprite sprite)
        {
            currentPlayerPlaneImage.SetActive(true);
        
            currentPlaneSprite.sprite = sprite;
            currentPlaneSpriteForHealthFilled.sprite = sprite;
        }
    
        public void SetEnemyPlayerHealthUiSprite(Sprite sprite)
        {
            enemyPlayerPlaneImage.SetActive(true);
        
            enemyPlaneSprite.sprite = sprite;
            enemyPlaneSpriteForHealthFilled.sprite = sprite;
        }
    
        public void SetCurrentPlayerHealthUiPlayerName(string playerName)
        {
            currentPlayerPlaneImage.SetActive(true);
            currentPlayerNameText.text = playerName;
        }
    
        public void SetEnemyPlayerHealthUiPlayerName(string playerName)
        {
            enemyPlayerPlaneImage.SetActive(true);
            enemyPlayerNameText.text = playerName;
        }
    
        public void SetCurrentPlayerHealthUiPlaneName(string planeName)
        {
            currentPlayerPlaneImage.SetActive(true);
            currentPlayerPlaneNameText.text = "Plane: " + planeName;
        }
    
        public void SetEnemyPlayerHealthUiPlaneName(string planeName)
        {
            enemyPlayerPlaneImage.SetActive(true);
            enemyPlayerPlaneNameText.text = "Plane: " + planeName;
        }
        
        public void OnCriticalHealthPointsWarningText(bool showText)
        {
            lowHpWarningGo.SetActive(showText);
        }
        
        public void DisableAllPlaneUi()
        {
            lowHpWarningGo.SetActive(false);
            catapultMessageGo.SetActive(false);
            currentPlayerPlaneImage.SetActive(false);
            enemyPlayerPlaneImage.SetActive(false);
            fighterPlaneRadarUI.SetActive(false);
        }
        
        public void ShowCatapultMessage(FighterPlaneType fighterPlaneType)
        {
            StartCoroutine(ShowCatapultMessageWithDelay());
        }
    
        private void ChangeCurrentPlayerHealthOnUi(float healthValue, float duration)
        {
            currentPlaneSpriteForHealthFilled.DOFillAmount(healthValue, duration);
        }
    
        private void ChangeEnemyPlayerHealthOnUi(float healthValue, float duration)
        {
            enemyPlaneSpriteForHealthFilled.DOFillAmount(healthValue, duration);
        }

        private IEnumerator ShowCatapultMessageWithDelay()
        {
            yield return new WaitForSeconds(3.5f);
            catapultMessageGo.SetActive(true);
        }

        private void FighterPlaneRadarUI(bool isActive)
        {
            fighterPlaneRadarUI.SetActive(isActive);
        }
    }
}
