using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StoreMenuManager : MonoBehaviour
{
    public Button[] Button;

    PlatformFighter platformfighter;
    PlayerInfo playerInfo;
    WeaponInfo weaponInfo;

    private void Awake()
    {
        platformfighter = PlatformFighter.getPlatformFighter();
        playerInfo = platformfighter.getPlayerInfo();
        weaponInfo = platformfighter.getWeaponInfo();

        for (int i = 0; i < WeaponInfo.FullWeaponSize; i++)
        {
            if (weaponInfo.weaponLevels[i] != 0 && weaponInfo.weaponLevels[i] != weaponInfo.MaxUpgrade)
            {
                Button[i].GetComponentInChildren<Text>().text = "강화";
            }
            else if (weaponInfo.weaponLevels[i] == weaponInfo.MaxUpgrade)
            {
                Button[i].GetComponentInChildren<Text>().text = "MaxUpgrade";
                Button[i].interactable = false;
            }
        }
    }

    public void BuyButton(int buttonID)
    {
        // 돈 확인
        int cost = weaponInfo.UpgradeCost[buttonID][weaponInfo.weaponLevels[buttonID]];

        if (playerInfo.Money >= cost)
        {
            // 돈 사용
            playerInfo.Money -= cost;

            Debug.Log(weaponInfo.Name[buttonID] + " " + weaponInfo.weaponLevels[buttonID] + " " + weaponInfo.MaxUpgrade);

            // 레벨, 스탯 업데이트
            weaponInfo.weaponLevels[buttonID] += 1;
            platformfighter.UpdateWeaponLevel(buttonID, weaponInfo.weaponLevels[buttonID]);

            // 최대업글 확인해서 버튼 비활성화
            if (weaponInfo.weaponLevels[buttonID] == weaponInfo.MaxUpgrade)
            {
                Button[buttonID].GetComponentInChildren<Text>().text = "MaxUpgrade";
                Button[buttonID].interactable = false;
            }
            else
            {
                Button[buttonID].GetComponentInChildren<Text>().text = "강화";
            }
        }
    }

    public void BackToStageMenu()
    {
        SceneManager.LoadScene("StageMenuScene");
    }
}
