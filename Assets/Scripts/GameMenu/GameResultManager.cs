using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class GameResultManager : MonoBehaviour
{
    public GameObject Text_Clear;
    public GameObject Text_Time;
    public GameObject Text_TotalPoint;
    public GameObject Text_Money;
    public GameObject Text_TotalMoney;

    PlatformFighter platformfighter;
    GameInfo gameInfo;
    PlayerInfo playerInfo;
    StageInfo StageInfo;

    private void Awake()
    {
        platformfighter = PlatformFighter.getPlatformFighter();
        gameInfo = platformfighter.getLastGameInfo();
        playerInfo = platformfighter.getPlayerInfo();
        StageInfo = platformfighter.getStageInfo()[gameInfo.stageNumber];

        // 점수로 돈계산, 클리어 했다면 클리어보너스
        gameInfo.money = (int)(StageInfo.MoneyPerPoint * gameInfo.totalPoint);
        if (gameInfo.clear) gameInfo.money += StageInfo.ClearBonusMoney;

        playerInfo.Money += gameInfo.money;


        if (gameInfo.clear) Text_Clear.GetComponent<Text>().text = "STAGE CLEAR";
        else Text_Clear.GetComponent<Text>().text = "STAGE FAIL";

        Text_Time.GetComponent<Text>().text = "" + (gameInfo.time / 1000) + "s";

        Text_TotalPoint.GetComponent<Text>().text = "" + gameInfo.totalPoint;

        Text_Money.GetComponent<Text>().text = "+" + gameInfo.money;

        Text_TotalMoney.GetComponent<Text>().text = "" + playerInfo.Money;
    }

    public void ButtonEvent_OK()
    {

        SceneManager.LoadScene("StageMenuScene");
    }

    private void Update()
    {
        
    }
}
