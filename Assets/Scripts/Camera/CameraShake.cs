using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour {

    private Vector3 originalPos;
    private float timeAtCurrentFrame;
    private float timeAtLastFrame;
    private float fakeDelta;

    void Update() {
        // Calculate a fake delta time, so we can Shake while game is paused.
        timeAtCurrentFrame = Time.realtimeSinceStartup;
        fakeDelta = timeAtCurrentFrame - timeAtLastFrame;
        timeAtLastFrame = timeAtCurrentFrame; 
    }

    public void Shake (float duration, float amount) {
        originalPos = gameObject.transform.localPosition;
        StopAllCoroutines();
        StartCoroutine(cShake(duration, amount));
    }

    private IEnumerator cShake (float duration, float amount) {
        float endTime = Time.time + duration;

        while (duration > 0) {
            transform.localPosition = originalPos + Random.insideUnitSphere * amount;

            duration -= fakeDelta;

            yield return null;
        }

        transform.localPosition = originalPos;
    }
}