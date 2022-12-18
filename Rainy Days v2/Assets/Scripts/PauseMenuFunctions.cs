using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenuFunctions : MonoBehaviour
{
    public GameObject child;
    public MainMenuFunctions mainMenu;
    private Player player;

    public void ActivatePauseMenu()
    {
        FindObjectOfType<HudFunctions>().DeactivateHud();
        child.SetActive(true);
    }

    public void DeactivatePauseMenu()
    {
        FindObjectOfType<HudFunctions>().ActivateHud();
        child.SetActive(false);
    }

    public void DeactivatePauseMenuWhenQuitting()
    {
        child.SetActive(false);
    }

    public void PlayerGoNeutral()
    {
        StartCoroutine(PlayerNeutral());
    }

    public IEnumerator PlayerNeutral()
    {
        yield return new WaitUntil(() => (player = FindObjectOfType<Player>()) != null);
        player.State = PlayerState.Neutral;
    }

    public void GoToCharacter()
    {
        child.SetActive(false);
        FindObjectOfType<CharacterMenuFunctions>().ActivateCharacterMenu();
    }

    public void GoToControls()
    {
        child.SetActive(false);
        FindObjectOfType<ControlsMenuFunctions>().fromPause = true;
        FindObjectOfType<ControlsMenuFunctions>().ActivateControlsMenu();
    }

    public void GoToOptions()
    {
        child.SetActive(false);
        FindObjectOfType<OptionsMenuFunctions>().fromPause = true;
        FindObjectOfType<OptionsMenuFunctions>().ActivateOptionsMenu();
    }

    public void ReturnToMainMenu()
    {
        Shadow shadow = FindObjectOfType<Shadow>();
        shadow.CloseShadow();
        StartCoroutine(ReturnToMM(shadow));
    }

    private IEnumerator ReturnToMM(Shadow shadow)
    {
        yield return new WaitUntil(() => shadow.doneAnimating);
        StartCoroutine(DestroyPlayer());
        DeactivatePauseMenuWhenQuitting();
        SceneManager.LoadScene(0);
        mainMenu.ActivateMainMenu();
        StartCoroutine(WaitAndActivateMM(shadow));
    }

    private IEnumerator WaitAndActivateMM(Shadow shadow)
    {
        yield return new WaitForSeconds(shadow.shadowTime);
        mainMenu.ActivateButtons();
    }

    public IEnumerator DestroyPlayer()
    {
        yield return new WaitUntil(() => (player = FindObjectOfType<Player>()) != null);
        if (player != null)
            Destroy(player.gameObject);
    }


}
