using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DebugInfoCanvas : MonoBehaviour
{
    [SerializeField] private KeyCode openDebugKeycode = KeyCode.Comma;

    private bool _debugInfoIsOpened = false;
    private PlayerInfoDebug _playerInfoDebugRAM;
    private GameNetworkManager _manager;

    private void Start()
    {
        if (SceneManager.GetActiveScene().name != "RaceTrack") return;
    }

    void Update()
    {
        ShowDebugInfo();
    }

    private void ShowDebugInfo()
    {
        // In RaceTrack this canvas active by default and could not be closed
        if (SceneManager.GetActiveScene().name == "RaceTrack") return;
        
        if (Input.GetKeyDown(openDebugKeycode))
        {
            _debugInfoIsOpened = !_debugInfoIsOpened;
            foreach (Transform child in gameObject.transform)
            {
                child.gameObject.SetActive(_debugInfoIsOpened);
            }
        }
    }
}
