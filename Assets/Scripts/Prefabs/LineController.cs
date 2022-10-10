using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineController : MonoBehaviour
{
    [SerializeField] float lifeInterval;
    [SerializeField] float fps;
    [SerializeField] Texture[] texture;


    private LineRenderer lineRenderer;
    private float fpsCounter;
    private int animationStep;
    private bool isDissolving;
    private float fade;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    private void Start()
    {
        isDissolving = false;
        fpsCounter = 0f;
        animationStep = 0;
        fade = 1f;
        StartCoroutine(destroyCoroutine());
    }

    private void Update()
    {
        dissolvingProcess();
        animationControl();
    }



    private void animationControl()
    {
        fpsCounter += Time.deltaTime;
        if(fpsCounter >= 1f/fps)
        {
            animationStep++;
            if(animationStep==texture.Length)
            {
                animationStep = 0;
            }

            lineRenderer.material.SetTexture("_MainTex", texture[animationStep]);
            fpsCounter = 0f;
        }
    }

    private IEnumerator destroyCoroutine()
    {
        yield return new WaitForSeconds(lifeInterval);
        isDissolving = true;
    }

    private void dissolvingProcess()
    {
        if (isDissolving)
        {
            fade -= Time.deltaTime;
            if (fade <= 0f)
            {
                fade = 0f;
                isDissolving = false;
                Destroy(gameObject);
            }

            // Set the property
            lineRenderer.material.SetFloat("_Fade", fade);
        }
    }

    public void drawLine(Vector3[] positionList)
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = positionList.Length;
        lineRenderer.SetPositions(positionList);

        lineRenderer.GetComponent<Transform>().position = new Vector3(3, 3, 0);
    }
}
