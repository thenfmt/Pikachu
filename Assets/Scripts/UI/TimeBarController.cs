using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeBarController : MonoBehaviour
{

    [SerializeField] float timeInterval;
    [SerializeField] float shakeAmount;
    [SerializeField] float timeOutPercent;
    [SerializeField] Image barFrontgourndImage;
    [SerializeField] GameObject focus;

    private float timeRemaining;
    private bool isSoundPlayed;
    private bool unLoad;
    private AudioSource audioSource;
    private CameraShake cameraShake;


    private void Start()
    {
        timeRemaining = timeInterval;
        audioSource = GetComponent<AudioSource>();
        cameraShake = FindObjectOfType<CameraShake>();
        isSoundPlayed = false;
    }


    private void Update()
    {
        if(timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;

            barFrontgourndImage.fillAmount = timeRemaining / timeInterval;


            if(timeRemaining < timeOutPercent*timeInterval)
            {
                barFrontgourndImage.color = Color.red;

                if(!isSoundPlayed)
                {
                    focus.SetActive(true);
                    isSoundPlayed = true;
                    cameraShake.Shake(timeOutPercent*timeInterval, shakeAmount);
                    audioSource.Play();
                }
            }
        }
        else 
        {
            timeRemaining = timeInterval;
            focus.SetActive(false);
            audioSource.Stop();
            PauseController pauseController = FindObjectOfType<PauseController>();
            pauseController.loadLoseMenu();
        }
    }
}
