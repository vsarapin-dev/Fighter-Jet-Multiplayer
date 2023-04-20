using System;
using System.Collections;
using Michsky.UI.Shift;
using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FighterPlainSelectionExit : NetworkBehaviour
{
    [SerializeField] private ModalWindowManager exitWindow;
    [SerializeField] private GameObject loader;

    private bool _windowIsVisible = false;
    private bool _canExit;
    private float _secondsToWait = 2f;
    private AsyncOperation _asyncLoad;
    private GameNetworkManager _manager;

    private GameNetworkManager Manager
    {
        get
        {
            if (_manager != null)
            {
                return _manager;
            }

            return _manager = GameNetworkManager.singleton as GameNetworkManager;
        }
    }

    public bool CanExit
    {
        get => _canExit;
        set => _canExit = value;
    }

    private void Update()
    {
        if (_canExit == false) return;
        
        OpenExitWindow();
        ShowCursor();
    }

    public void OnExitPress()
    {
        StartCoroutine(ShowLoaderAndReturnToLobby());
    }
    
    public void CloseExitWindow()
    {
        _windowIsVisible = false;
        exitWindow.ModalWindowOut();
    }

    private void OpenExitWindow()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            _windowIsVisible = !_windowIsVisible;
            if (_windowIsVisible == true)
            {
                exitWindow.ModalWindowIn();
            }
            else
            {
                CloseExitWindow();
            }
        }
    }

    private void ShowCursor()
    {
        if (_windowIsVisible == true)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            Cursor.visible = false;
        }
    }

    private IEnumerator ShowLoaderAndReturnToLobby()
    {
        CloseExitWindow();
        loader.SetActive(true);
        _asyncLoad = SceneManager.LoadSceneAsync("Lobby", LoadSceneMode.Single);
        _asyncLoad.allowSceneActivation = false;
        bool sceneReadyToSwitch = false;
        while (!_asyncLoad.isDone)
        {
            if (_asyncLoad.progress >= 0.9f)
            {
                if (sceneReadyToSwitch == false)
                {
                    sceneReadyToSwitch = true;
                    FighterPlainSelectionPlayer currentPlayer = NetworkClient.localPlayer.gameObject.GetComponent<FighterPlainSelectionPlayer>();
                    currentPlayer.CmdRemoveFromPlayerList(isServer);
                    loader.SetActive(false);
                }
                _asyncLoad.allowSceneActivation = true;
            }
            yield return null;
        }
    }
}
