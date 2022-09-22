using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuFunctions : MonoBehaviour
{

    public GameObject child;
    public GameObject buttons;

    public void StartGame()
    {
        Shadow shadow = FindObjectOfType<Shadow>();
        shadow.CloseShadow();
        StartCoroutine(StartGameCoroutine(shadow));
    }

    public IEnumerator StartGameCoroutine(Shadow shadow)
    {
        yield return new WaitUntil(() => shadow.doneAnimating);
        SceneManager.LoadScene(1);
        DeactivateMainMenu();
    }

    public void ActivateMainMenu()
    {
        child.SetActive(true);
        buttons.SetActive(true);
    }

    public void DeactivateMainMenu()
    {
        child.SetActive(false);
    }

    public void DeactivateButtons()
    {
        buttons.SetActive(false);
    }

}
