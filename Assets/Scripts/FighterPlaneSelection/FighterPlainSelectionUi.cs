using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FighterPlainSelectionUi : MonoBehaviour
{
    [SerializeField] private Transform fighterPlainSelectionTextParent;
    [SerializeField] private GameObject fighterPlainSelectedTextPrefab;
    [SerializeField] private TMP_Text countDownTimerToStartText;
    [SerializeField] private AudioSource countDownTimerAudioSource;
    [SerializeField] private AudioClip rightBeforeStartSoundClip;
    [SerializeField] private GameObject loader;
    
    private List<GameObject> _playerTextPrefabs = new List<GameObject>();

    private void OnEnable()
    {
        Actions.OnFighterPlainSelected += OnAddFighterPlainSelectedText;
        Actions.OnAllPlayersSelectFighterPlain += OnStartCountDownTimer;
    }

    private void OnDisable()
    {
        Actions.OnFighterPlainSelected -= OnAddFighterPlainSelectedText;
        Actions.OnAllPlayersSelectFighterPlain -= OnStartCountDownTimer;
    }

    private void OnAddFighterPlainSelectedText(string playerName)
    {
        GameObject playerTextPrefab = Instantiate(fighterPlainSelectedTextPrefab, fighterPlainSelectionTextParent);
        playerTextPrefab.GetComponent<TMP_Text>().text = $"{playerName} is <color=green>Ready</color>";
        _playerTextPrefabs.Add(playerTextPrefab);
    }
    

    private void OnStartCountDownTimer()
    {
        StartCoroutine(TimerCoroutine());
    }

    private IEnumerator TimerCoroutine()
    {
        int timeToStart = 7;
        
        while (timeToStart > 0)
        {
            if (timeToStart == 2)
            {
                countDownTimerAudioSource.clip = rightBeforeStartSoundClip;
                countDownTimerAudioSource.Play();
                ClearAllBeforeLoadNextScene();
                loader.SetActive(true);
            }
            if (timeToStart > 2)
            {
                countDownTimerAudioSource.Play();
                countDownTimerToStartText.text = (timeToStart - 2).ToString();
            }
            timeToStart -= 1;
            yield return  new WaitForSeconds(1);
        }

        ClearAllBeforeLoadNextScene();
        LoadRaceTrackScene();
    }

    private void ClearAllBeforeLoadNextScene()
    {
        foreach (GameObject readyTextGo in _playerTextPrefabs)
        {
            Destroy(readyTextGo);
        }
        _playerTextPrefabs.Clear();
        countDownTimerToStartText.text = String.Empty;
    }

    private void LoadRaceTrackScene()
    {
        FighterPlainSelectionPlayer player = FindLocalPlayer();
        player.CmdLoadRaceTrack();
    }
    
    private FighterPlainSelectionPlayer FindLocalPlayer()
    {
        return GameObject.Find("LocalGamePlayer").GetComponent<FighterPlainSelectionPlayer>();
    }
}
