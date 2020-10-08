using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class StageMenuManager : MonoBehaviour
{
    public GameObject Text_Money;

    PlatformFighter platformfighter;

    private void Awake()
    {
        platformfighter = PlatformFighter.getPlatformFighter();
    }

    public void EnterStore()
    {
        SceneManager.LoadScene("StoreMenuScene");
    }

    public void StartStage()
    {
        string stage = EventSystem.current.currentSelectedGameObject.name;
        SceneManager.LoadScene(stage+"Scene");
    }

    public void BackToGameMenu()
    {
        SceneManager.LoadScene("GameMenuScene");
    }

    private void Update()
    {
        Text_Money.GetComponent<Text>().text = "" + platformfighter.getPlayerInfo().Money + "원";
    }
}