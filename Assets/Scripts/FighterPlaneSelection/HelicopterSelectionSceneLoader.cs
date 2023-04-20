using System.Collections;
using UnityEngine;

public class HelicopterSelectionSceneLoader : MonoBehaviour
{
    [Header("GameObjects")]
    [SerializeField] private GameObject loader;
    [SerializeField] private FighterPlainSelectionExit fighterPlainSelectionExit;

    [Header("Scripts")]
    [SerializeField] private FighterPlainSelectController fighterPlainSelectController;

    [Header("Variables")]
    [SerializeField] private float loaderDuration = 3f;
    
    void Start()
    {
        StartCoroutine(LoaderCoroutine());
    }

    private IEnumerator LoaderCoroutine()
    {
        fighterPlainSelectionExit.CanExit = false;
        fighterPlainSelectController.CanSelect = false;
        yield return new WaitForSeconds(loaderDuration);
        loader.SetActive(false);
        fighterPlainSelectController.CanSelect = true;
        fighterPlainSelectionExit.CanExit = true;
    }
}
