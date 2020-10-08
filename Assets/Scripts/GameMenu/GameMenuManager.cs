using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMenuManager : MonoBehaviour
{
    PlatformFighter platformfighter;

    private void Awake()
    {
        platformfighter = PlatformFighter.getPlatformFighter();
    }

    public void StartClick()
    {
        SceneManager.LoadScene("SampleScene");
    }

    public void StageButton()
    {
        SceneManager.LoadScene("StageMenuScene");
    }

    public void ExitGame()
    {

        // 에디터 종료
        // UnityEditor.EditorApplication.isPlaying = false; 
        // ####### wasd가 안먹히게되는 버그가 있는 듯

        // 실제 게임 종료
        // Application.Quit();
    }
}