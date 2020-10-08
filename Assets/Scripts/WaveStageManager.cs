using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WaveStageManager : MonoBehaviour
{
    public int StageNumber;

    private int NumWave;
    private int curWave;
    private int[] WaveEnemy;
    private GameObject[] WaveObject;

    PlatformFighter platformfighter;
    GameInfo gameInfo;
    System.Diagnostics.Stopwatch Stopwatch;

    private void Awake()
    {
        platformfighter = PlatformFighter.getPlatformFighter();
        gameInfo = platformfighter.getLastGameInfo();
        gameInfo.stageNumber = StageNumber;
        gameInfo.totalPoint = 0;
        gameInfo.money = 0;
        gameInfo.clear = false;
        gameInfo.time = 0L;

        Stopwatch = new System.Diagnostics.Stopwatch();
        Stopwatch.Reset();
        Stopwatch.Start();

        // wave 관리
        CountNumWave();
        CountWaveEnemy();
        GetWaveObject();

        curWave = 0;
        WaveObject[curWave].SetActive(true);

        for (int i = 0; i < NumWave; i++)
            Debug.Log(i + " " + WaveEnemy[i]);
    }

    private void GetWaveObject()
    {
        Transform Enemy = GameObject.Find("Enemy").transform;

        WaveObject = new GameObject[NumWave];

        for (int i = 0; i < NumWave; i++)
        {
            WaveObject[i] = Enemy.GetChild(i).gameObject;
        }
    }

    private void CountNumWave()
    {
        Transform Enemy = GameObject.Find("Enemy").transform;

        NumWave = Enemy.childCount;
    }

    private void CountWaveEnemy()
    {
        Transform Enemy = GameObject.Find("Enemy").transform;
        WaveEnemy = new int[NumWave];

        for (int i = 0; i < NumWave; i++)
        {
            Transform wave = Enemy.GetChild(i);
            int NumTroops = wave.childCount;

            for (int t = 0; t < NumTroops; t++)
            {
                Transform Troop = wave.GetChild(t);
                WaveEnemy[i] += Troop.childCount;
            }
        }
    }

    public void EnemyKill(GameObject gameObject)
    {
        Enemy_Boss enemyBoss = gameObject.GetComponent<Enemy_Boss>();
        if (enemyBoss != null)
        {
            OnBossDie(gameObject);
        }
        else
        {
            OnEnemyDie(gameObject);
        }

        WaveEnemy[curWave]--;
        if (WaveEnemy[curWave] <= 0)
        {
            WaveObject[curWave].SetActive(false);
            curWave++;
            if (curWave < NumWave)
                WaveObject[curWave].SetActive(true);
            else
                SceneManager.LoadScene("GameResultScene");
        }
    }

    private void OnEnemyDie(GameObject gameObject)
    {
        // killPoint 획득
        Enemy enemy = gameObject.GetComponent<Enemy>();
        gameInfo.totalPoint += enemy.killPoint;
    }

    private void OnBossDie(GameObject gameObject)
    {
        // killPoint 획득
        Enemy enemy = gameObject.GetComponent<Enemy>();
        gameInfo.totalPoint += enemy.killPoint;

        // 클리어시간 기록
        gameInfo.time = Stopwatch.ElapsedMilliseconds;
        Debug.Log("클리어시간 : " + gameInfo.time + "[ms]");

        // 클리어 기록
        gameInfo.clear = true;

        // 결과화면으로 이동
        SceneManager.LoadScene("GameResultScene");
    }

}
