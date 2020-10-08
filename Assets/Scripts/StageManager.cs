using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StageManager : MonoBehaviour
{
    public int StageNumber;
    public GameObject WaveTitle;
    public float[] EnemyCycle;

    private CanvasRenderer renderer;
    private int TotalWave;
    private int curWave;
    private int WaveEnemy;
    private GameObject[] WaveTroops;

    int troopIndex;

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
        Transform Enemy = GameObject.Find("Enemy").transform;
        TotalWave = Enemy.childCount;   // wave 개수 확인
        for (int i = 0; i < TotalWave; i++)
        {
            Transform wave = Enemy.GetChild(i);
            for (int j = 0; j < wave.childCount; j++)
            {
                wave.GetChild(j).gameObject.SetActive(false);
            }
        }
        curWave = 0;
        StartWave();
    }

    private void StartWave()
    {
        // Wave 전환 시 글자 표시
        renderer = WaveTitle.GetComponent<CanvasRenderer>();
        WaveTitle.GetComponent<Text>().text = "WAVE" + (curWave + 1);

        // 글자가 서서히 밝아지는 효과
        StartCoroutine("FadeIn");

        CountWaveEnemy();
        GetWaveTroops();
        troopIndex = 0;
        Invoke("ActivateObject", 3);
    }

    private void ActivateObject()
    {
        Transform Enemy = GameObject.Find("Enemy").transform;
        Transform wave = Enemy.GetChild(curWave);
        wave.gameObject.SetActive(true);

        if (troopIndex < WaveTroops.Length)
        {
            WaveTroops[troopIndex].SetActive(true);
            troopIndex++;
            Invoke("ActivateObject", Random.Range(0, EnemyCycle[curWave]));
        }
    }

    private void GetWaveTroops()
    {
        Transform Enemy = GameObject.Find("Enemy").transform;
        Transform wave = Enemy.GetChild(curWave);

        WaveTroops = new GameObject[WaveEnemy];
        for (int i = 0; i < WaveEnemy; i++)
            WaveTroops[i] = wave.GetChild(i).gameObject;
    }

    private void CountWaveEnemy()
    {
        Transform Enemy = GameObject.Find("Enemy").transform;
        Transform wave = Enemy.GetChild(curWave);

        WaveEnemy = CountEnemy(wave);
    }

    private int CountEnemy(Transform E)
    {
        if (E.gameObject.tag == "Enemy")
            return 1;

        int count = 0;
        for (int i = 0; i < E.childCount; i++)
            count += CountEnemy(E.GetChild(i));

        return count;
    }

    private void GoGameResultScene()
    {
        SceneManager.LoadScene("GameResultScene");
    }

    public void EnemyKill(GameObject gameObject)
    {
        WaveEnemy--;
        if (WaveEnemy < 0)
        {
            Debug.Log("[버그 발생] EnemyKill " + WaveEnemy);
            return;
        }

        if (WaveEnemy == 0)
        {
            curWave++;
            if (curWave < TotalWave)
                StartWave();
            else
            {
                // UI 표시
                WaveTitle.GetComponent<Text>().text = "STAGE CLEAR!!";
                Color c = renderer.GetColor();
                c.a = 1;
                renderer.SetColor(c);

                // killPoint 획득
                Enemy enemy = gameObject.GetComponent<Enemy>();
                gameInfo.totalPoint += enemy.killPoint;

                // 클리어시간 기록
                gameInfo.time = Stopwatch.ElapsedMilliseconds;
                Debug.Log("클리어시간 : " + gameInfo.time + "[ms]");

                // 클리어 기록
                gameInfo.clear = true;

                // 결과화면으로 이동
                Invoke("GoGameResultScene", 3);
            }
        }
    }

    IEnumerator _FadeOut()
    {
        int i = 100;
        while (i > 0)
        {
            i -= 1;
            float f = i / 100.0f;
            Color c = renderer.GetColor();
            c.a = f;
            renderer.SetColor(c);
            yield return new WaitForSeconds(0.014f);
        }
    }

    private void FadeOut()
    {
        StartCoroutine("_FadeOut");
    }

    IEnumerator FadeIn()
    {
        int i = 0;
        while (i < 100)
        {
            i += 1;
            float f = i / 100.0f;
            Color c = renderer.GetColor();
            c.a = f;
            renderer.SetColor(c);
            yield return new WaitForSeconds(0.014f);
        }
        Invoke("FadeOut", 1);
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
