using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;


public class MenuController : MonoBehaviour
{   
    [SerializeField] int numberOfLevel;
    [SerializeField] TextMeshProUGUI levelText;

    private AudioManager audioManager;
    private int level;

    private void Start()
    {
        level = 1;
        levelText.text = "Level 1";

        DontDestroyOnLoad(this.gameObject);
        audioManager = FindObjectOfType<AudioManager>();
        audioManager.playSound("MenuMusic");
    }


    public void play()
    {
        audioManager.stopSound("MenuMusic");
        audioManager.playSound("Start");
        SceneManager.LoadScene(level);
    }

    public void nextLevel()
    {
        audioManager.playSound("Select");
        level = Mathf.Min(level+1, numberOfLevel);
        levelText.text = "Level " + level;
    }

    public void previousLevel()
    {
        audioManager.playSound("Select");
        level = Mathf.Max(level-1, 1);
        levelText.text = "Level " + level;
    }

    public void quit()
    {
        Application.Quit();
    }
}
