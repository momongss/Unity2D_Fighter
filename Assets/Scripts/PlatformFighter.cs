using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;

public class PlayerInfo
{
    public int[] weaponLevels;
    public int Money;
    public int Score;
    public int StageOpen;
    public int maxHP;
    public int maxMP;

    List<string[]> weaponLevelData;

    public PlayerInfo()
    {
        weaponLevels = new int[4];
        Money = 1000000;    // 디버깅용
        Score = 0;
        StageOpen = 0;
        maxHP = 100;
        maxMP = 100;

        FileManager weaponLevelFile = new FileManager("weaponLevel.csv");
        weaponLevelData = weaponLevelFile.ReadData();
        LoadPlayerInfo();
    }

    public void UpdateWeaponLevel(int weapon, int lv)
    {
        weaponLevels[weapon] = lv;
        weaponLevelData[0][weapon] = lv.ToString();

        UpdatePlayerInfo();
    }

    private void UpdatePlayerInfo()
    {
        FileManager weaponLevelFile = new FileManager("weaponLevel.csv");
        weaponLevelFile.WriteData(weaponLevelData);
    }

    private void LoadPlayerInfo()
    {
        foreach (string[] line in weaponLevelData)
        {
            string InfoName = line[0];
            switch (InfoName)
            {
                case "weaponLevel":
                    for (int i = 0; i < line.Length-1; i++)
                    {
                        weaponLevels[i] = Int32.Parse(line[i + 1]);
                    }
                    break;
                case "Money":
                    Money = Int32.Parse(line[1]);
                    break;
                case "Score":
                    Score = Int32.Parse(line[1]);
                    break;
                case "StageOpen":
                    StageOpen = Int32.Parse(line[1]);
                    break;
                case "maxHP":
                    maxHP = Int32.Parse(line[1]);
                    break;
                case "maxMP":
                    maxMP = Int32.Parse(line[1]);
                    break;
            }
        }
    }
}

public class GameInfo
{
    public int stageNumber;
    public int totalPoint;
    public int money;
    public bool clear;
    public long time;
}

public class StageInfo
{
    public float MoneyPerPoint;
    public int ClearBonusMoney;
    public StageInfo(float MoneyPerPoint, int ClearBonusMoney)
    {
        this.MoneyPerPoint = MoneyPerPoint;
        this.ClearBonusMoney = ClearBonusMoney;
    }
}

public class SkillInfo
{
    public class SkillOption
    {
        public float InitialValue;
        public float Upgrade;
        public SkillOption(float initialvalue, float upgrade)
        {
            InitialValue = initialvalue;
            Upgrade = upgrade;
        }
    }

    public const int ATTACK_BUFF = 0;
    public const int ATTACK = 1;
    public const int DEFENSIVE_BUFF = 2;
    public const int MOVE = 3;

    public string Name;
    public int ID;
    public int SkillType;

    public float ManaConsumption;
    public float UpManaConsumption;
    public float CoolTime;
    public float UpCoolTime;
    public float Power;
    public float UpPower;
    public Dictionary<string, SkillOption> Option;

    public SkillInfo(string Name, int ID, int SkillType, float Mana, float UpMana, float Cool, float UpCool, float Power, float UpPower)
    {
        this.Name = Name;
        this.ID = ID;
        this.SkillType = SkillType;
        ManaConsumption = Mana;
        UpManaConsumption = UpMana;
        CoolTime = Cool;
        UpCoolTime = UpCool;
        this.Power = Power;
        this.UpPower = UpPower;
        Option = new Dictionary<string, SkillOption>();
    }

    public void addOption(string optionName, float Initialvalue, float Upgrade)
    {
        Option.Add(optionName, new SkillOption(Initialvalue, Upgrade));
    }
}

public class WeaponInfo
{
    public const int FullWeaponSize = 4;

    public List<GameObject> WeaponObj;
    public List<string> Name;
    public int MaxUpgrade;
    public List<List<int>> UpgradeCost;    
    public List<float[]> weaponStats;
    public int[] weaponLevels;

    List<string[]> weaponStatData;

    public WeaponInfo(PlayerInfo playerInfo)
    {
        WeaponObj = new List<GameObject>();
        WeaponObj.Add(Resources.Load<GameObject>("Prefabs/PlayerBullets/weaponNormal"));
        WeaponObj.Add(Resources.Load<GameObject>("Prefabs/PlayerBullets/weaponFire"));
        WeaponObj.Add(Resources.Load<GameObject>("Prefabs/PlayerBullets/weaponIce"));
        WeaponObj.Add(Resources.Load<GameObject>("Prefabs/PlayerBullets/weaponLightning"));

        Name = new List<string>() { "weaponNormal", "weaponFire", "weaponIce", "weaponLightning" };

        MaxUpgrade = 5;

        UpgradeCost = new List<List<int>>();
        UpgradeCost.Add(new List<int>() { 0, 1000, 2000, 3000, 4000});
        UpgradeCost.Add(new List<int>() { 5000, 1000, 2000, 3000, 4000 });
        UpgradeCost.Add(new List<int>() { 5000, 1000, 2000, 3000, 4000 });
        UpgradeCost.Add(new List<int>() { 5000, 1000, 2000, 3000, 4000 });

        FileManager weaponStatsFile = new FileManager("weaponStats.csv");
        weaponStatData = weaponStatsFile.ReadData();

        weaponLevels = playerInfo.weaponLevels;

        ApplyWeaponStats();
    }

    public void UpdateWeaponLevel(int weapon, int lv)
    {
        // weaponLevelData[0][weapon] = lv.ToString();
        weaponLevels[weapon] = lv;
        ApplyWeaponStats();

        Debug.Log(Name[weapon] + " " + lv +"단계 damage: " + weaponStats[weapon][0] + " delay: " + weaponStats[weapon][1] + " speed: " + weaponStats[weapon][2]);
    }

    private void ApplyWeaponStats()
    {
        weaponStats = new List<float[]>();

        for (int w = 0; w < FullWeaponSize; w++)
        {
            string[] stats = weaponStatData[weaponLevels[w]][w].Split('/');
            float damage = float.Parse(stats[0]);
            float fireDelay = float.Parse(stats[1]);
            float speed = float.Parse(stats[2]);

            weaponStats.Add(new float[] { damage, fireDelay, speed });            

            Bullet bullet = WeaponObj[w].GetComponent<Bullet>();
            bullet.damage = damage;
            bullet.FireDelay = fireDelay;
            bullet.Speed = speed;
        }
    }
}

public class PlatformFighter
{
    PlayerInfo PlayerInfo;
    GameInfo LastGameInfo;
    WeaponInfo WeaponInfo;
    List<SkillInfo> SkillList;

    Dictionary<int, StageInfo> StageInfo;

    private static PlatformFighter platformfighter;

    public static PlatformFighter getPlatformFighter()
    {
        if (platformfighter == null)
            platformfighter = new PlatformFighter();

        // 저장된 데이터 불러와서 platformfighter에 적용하기

        return platformfighter;
    }

    public PlatformFighter()
    {
        Debug.Log("PlatformFighter");
        // 불러오기 시도

        // 실패한 경우 초기값으로 설정
        PlayerInfo = new PlayerInfo();
        LastGameInfo = new GameInfo();
        WeaponInfo = new WeaponInfo(PlayerInfo);
        SkillInfoInit();

        StageInfo = new Dictionary<int, StageInfo>();
        StageInfo[010101] = new StageInfo(1.0f, 5000);
        StageInfo[010102] = new StageInfo(1.0f, 5000);
        StageInfo[010103] = new StageInfo(1.0f, 5000);
        StageInfo[010104] = new StageInfo(1.0f, 5000);
        StageInfo[010105] = new StageInfo(1.0f, 5000);
        StageInfo[010106] = new StageInfo(1.0f, 5000);
        StageInfo[010107] = new StageInfo(1.0f, 5000);
    }

    public void UpdateWeaponLevel(int weapon, int lv)
    {
        PlayerInfo.UpdateWeaponLevel(weapon, lv);
        WeaponInfo.UpdateWeaponLevel(weapon, lv);
    }

    public void setPlayerInfo(PlayerInfo Pinfo)
    {
        PlayerInfo = Pinfo;
    }

    public PlayerInfo getPlayerInfo()
    {
        return PlayerInfo;
    }

    public WeaponInfo getWeaponInfo()
    {
        return WeaponInfo;
    }

    public List<SkillInfo> getSkillList()
    {
        return SkillList;
    }

    public void setLastGameInfo(GameInfo Ginfo)
    {
        LastGameInfo = Ginfo;
    }

    public GameInfo getLastGameInfo()
    {
        return LastGameInfo;
    }

    public Dictionary<int, StageInfo> getStageInfo()
    {
        return StageInfo;
    }

    private void SkillInfoInit()
    {
        SkillList = new List<SkillInfo>();

        // 스킬정의, ID 순서대로 list에 추가해야 함
        SkillInfo skill = new SkillInfo("SpreadAttack", 0, SkillInfo.ATTACK_BUFF, 30f, -1f, 20f, -1f, 5f, 0.5f);
        skill.addOption("Duration", 5, 0.5f);
        SkillList.Add(skill);
        skill = new SkillInfo("1", 1, SkillInfo.ATTACK_BUFF, 30f, -1f, 20f, -1f, 5f, 0.5f);
        skill.addOption("", 5, 0.5f);
        SkillList.Add(skill);
        skill = new SkillInfo("2", 2, SkillInfo.ATTACK_BUFF, 30f, -1f, 20f, -1f, 5f, 0.5f);
        skill.addOption("", 5, 0.5f);
        SkillList.Add(skill);
        skill = new SkillInfo("FullAttack", 3, SkillInfo.ATTACK, 50f, -2f, 60f, -2f, 150f, 10f);
        skill.addOption("DamageType", Bullet.TYPE_FIRE, 0);
        skill.addOption("Range", 10f, 1f);
        SkillList.Add(skill);
        skill = new SkillInfo("4", 4, SkillInfo.ATTACK_BUFF, 30f, -1f, 20f, -1f, 5f, 0.5f);
        skill.addOption("", 5, 0.5f);
        SkillList.Add(skill);
        skill = new SkillInfo("5", 5, SkillInfo.ATTACK_BUFF, 30f, -1f, 20f, -1f, 5f, 0.5f);
        skill.addOption("", 5, 0.5f);
        SkillList.Add(skill);
        skill = new SkillInfo("IceShield", 6, SkillInfo.DEFENSIVE_BUFF, 30f, -1f, 60, 0f, 0.55f, 0.05f);
        skill.addOption("Duration", 1f, 0.1f);
        SkillList.Add(skill);
        skill = new SkillInfo("IceFullAttack", 7, SkillInfo.ATTACK, 30f, -1f, 20f, -1f, 10f, 5f);
        skill.addOption("DamageType", Bullet.TYPE_ICE2, 0.5f);
        skill.addOption("Range", 10f, 1f);
        SkillList.Add(skill);
        skill = new SkillInfo("8", 8, SkillInfo.ATTACK_BUFF, 30f, -1f, 20f, -1f, 5f, 0.5f);
        skill.addOption("", 5, 0.5f);
        SkillList.Add(skill);
        skill = new SkillInfo("Flash", 9, SkillInfo.MOVE, 30f, -1f, 20f, -1f, 0f, 0f);
        skill.addOption("Range", 3, 0.2f);
        SkillList.Add(skill);
        skill = new SkillInfo("10", 10, SkillInfo.ATTACK_BUFF, 30f, -1f, 20f, -1f, 5f, 0.5f);
        skill.addOption("", 5, 0.5f);
        SkillList.Add(skill);
        skill = new SkillInfo("11", 11, SkillInfo.ATTACK_BUFF, 30f, -1f, 20f, -1f, 5f, 0.5f);
        skill.addOption("", 5, 0.5f);
        SkillList.Add(skill);
    }
}
