using System;
using System.Collections.Generic;
using UnityEngine;

public class FighterPlainSelectController : MonoBehaviour
{
    [SerializeField] private List<GameObject> fighterPlainsSceneObjects = new List<GameObject>();

    private int _selectedFighterPlainIndex;
    private bool _canSelect = false;

    public bool CanSelect
    {
        get => _canSelect;
        set => _canSelect = value;
    }

    private void Start()
    {
        ShowFighterPlain(_selectedFighterPlainIndex);
    }

    private void Update()
    {
        if (_canSelect == false) return;
        
        LeftClick();
        RightClick();
        SelectVehicle();
    }

    private void LeftClick()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
        {
            ShowFighterPlain(_selectedFighterPlainIndex - 1);
        }
    }
    
    private void RightClick()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
        {
            ShowFighterPlain(_selectedFighterPlainIndex + 1);
        }
    }
    
    private void SelectVehicle()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            _canSelect = false;
            FighterPlainSelectionPlayer player = FindLocalPlayer();
            PlayerPrefs.SetInt("SelectedFighterPlain", _selectedFighterPlainIndex);
            player.CmdSetPlayerHasSelectFighterPlain(true);
        }
    }

    private FighterPlainSelectionPlayer FindLocalPlayer()
    {
        return GameObject.Find("LocalGamePlayer").GetComponent<FighterPlainSelectionPlayer>();
    }

    private void ShowFighterPlain(int index)
    {
        _selectedFighterPlainIndex = ComputeIndex(index);
        
        for (int i = 0; i < fighterPlainsSceneObjects.Count; i++)
        {
            if (i == _selectedFighterPlainIndex)
            {
                fighterPlainsSceneObjects[i].SetActive(true);
            }
            else
            {
                fighterPlainsSceneObjects[i].SetActive(false);
            }
        }
    }

    private int ComputeIndex(int index)
    {
        if (index >= fighterPlainsSceneObjects.Count) return 0;
        if (index < 0)  return fighterPlainsSceneObjects.Count - 1;
        return index;
    }
}
