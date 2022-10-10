using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;



public class PauseController : MonoBehaviour
{
    [SerializeField] GameObject gamePlayUI;
    [SerializeField] GameObject pauseMenu;
    [SerializeField] GameObject loseMenu;
    [SerializeField] GameObject winMenu;
    [SerializeField] GameObject muteButton;
    [SerializeField] GameObject focus;
    [SerializeField] Sprite muteIcon;
    [SerializeField] Sprite unMuteIcon;

    public static bool isPause;
    private bool isMute;
    private AudioSource audioSource;

    private void Awake()
    {
        isPause = false;
        isMute = false;
        audioSource = GetComponent<AudioSource>();
    }


    public void pause()
    {
        isPause = true;
        gamePlayUI.SetActive(false);
        focus.SetActive(false);
        pauseMenu.SetActive(true);
        Time.timeScale = 0f;
    }

    public void menu()
    {
        AudioManager audioManager = FindObjectOfType<AudioManager>();
        audioManager.stopSound("MainMusic");

        Time.timeScale = 1f;
        isPause = false;
        SceneManager.LoadScene(0);
    }

    public void unPause()
    {
        isPause = false;
        Time.timeScale = 1f;
        pauseMenu.SetActive(false);
        gamePlayUI.SetActive(true);
        winMenu.SetActive(false);
        loseMenu.SetActive(false);
    }

    public void mute()
    {
        isMute = !isMute;

        if(isMute)
        {
            muteButton.GetComponent<Image>().sprite = muteIcon;
            audioSource.Stop();
        }
        else
        {
            muteButton.GetComponent<Image>().sprite = unMuteIcon;
        }
    }

    public void quit()
    {
        Application.Quit();
    }

    public void loadLoseMenu()
    {
        Time.timeScale = 0f;
        isPause = true;
        gamePlayUI.SetActive(false);
        loseMenu.SetActive(true);
    }

    public void loadWinMenu()
    {
        Time.timeScale = 0f;
        isPause = true;
        gamePlayUI.SetActive(false);
        winMenu.SetActive(true);
    }

    public void nextLevel()
    {
        Time.timeScale = 1f;
        isPause = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
