using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdLevel : MonoBehaviour
{
    public Encounter enterLevelEncounter;
    private MainCanvas immortal;
    private Player player;
    public static float abit = 2f;

    void Start()
    {
        GoNeutral();
        StartCoroutine(FindImmortal());
    }

    private IEnumerator FindImmortal()
    {
        yield return new WaitUntil(() => (immortal = FindObjectOfType<MainCanvas>()) != null);
        if (!immortal.FinishedEncounters.ContainsKey(enterLevelEncounter.encounterId))
        {
            StartCoroutine(StartEncounter());
        }
    }

    private IEnumerator StartEncounter()
    {
        yield return new WaitUntil(() => FindObjectOfType<Player>().State == PlayerState.Neutral);
        enterLevelEncounter.StartEncounter();
    }

    public void GoNeutral()
    {
        StartCoroutine(FindPlayer());
    }

    private IEnumerator FindPlayer()
    {
        yield return new WaitUntil(() => (player = FindObjectOfType<Player>()) != null);
        StartCoroutine(WaitABit());
    }

    private IEnumerator WaitABit()
    {
        yield return new WaitForSeconds(abit);
        player.State = PlayerState.Neutral;
    }
}
