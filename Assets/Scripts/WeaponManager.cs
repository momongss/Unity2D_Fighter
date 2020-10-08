using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponManager : MonoBehaviour
{
    /*
     * 무기 데이터 관리
     * 총알 수 관리
     */

    private int usingWeapon;
    private List<GameObject> weapon;
    private List<bool> weaponUsable;
    private int NumberOfWeapon;

    private PlatformFighter platformfighter;
    private PlayerInfo playerInfo;
    private WeaponInfo weaponInfo;

    /*
     * Weapon type
     * 
     * type 0
     *  사용할 때 총알을 소모하지 않는 기본 총
     * 
     * type 1
     *  기본 총과 발사 방법이 동일하지만 총알을 소모하는 총
     * 
     * 
     */

    private void Awake()
    {
        platformfighter = PlatformFighter.getPlatformFighter();
        playerInfo = platformfighter.getPlayerInfo();
        weaponInfo = platformfighter.getWeaponInfo();

        NumberOfWeapon = WeaponInfo.FullWeaponSize; // 총 무기 개수

        weapon = new List<GameObject>(weaponInfo.WeaponObj);

        weaponUsable = new List<bool>();
        for (int i = 0; i < WeaponInfo.FullWeaponSize; i++)
        {
            if (weaponInfo.weaponLevels[i] == 0)
                weaponUsable.Add(false);
            else
                weaponUsable.Add(true);
        }

        setWeapon(0); // 무기들의 값 초기화 후 마지막에 둘 것
    }

    private void Update()
    {

    }

    public int getUsingWeapon()
    {
        return usingWeapon;
    }

    public GameObject getWeapon()
    {
        return weapon[usingWeapon];
    }

    public void nextWeapon()
    {
        int cnt = 0;
        int now = usingWeapon;
        int next = (now + 1) % NumberOfWeapon;

        while (!weaponUsable[next])
        {
            next = (next + 1) % NumberOfWeapon;
            cnt++;
            if (cnt == 100)
            {
                Debug.Log("무기변경 오류: 무한루프");
                break;
            }
        }

        setWeapon(next);
    }

    // usingWeapon을 변경하고 UI에 적용
    private void setWeapon(int uw)
    {
        usingWeapon = uw;
    }
}
