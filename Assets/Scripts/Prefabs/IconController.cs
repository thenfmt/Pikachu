using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IconController : MonoBehaviour
{ 
    GameLogic gameLogic;
    private Vector2 vectorIndex;
    private int id;
    private bool isSelect;
    private bool isDissolving;


    private Material material;
    private float fade = 1f;

    private Material backgroundMaterial;


    private void Start()
    {
        isSelect = false;
        isDissolving = false;
        material = GetComponent<SpriteRenderer>().material;
        gameLogic = FindObjectOfType<GameLogic>();
    }

    private void Update()
    {
        dissolvingProcess();
        movingUpdate();
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

                gameLogic.mapMatrix[(int)vectorIndex.x, (int)vectorIndex.y] = -1;
                gameLogic.iconMatrix[(int)vectorIndex.x, (int)vectorIndex.y] = null;
                gameLogic.winCount--;
                Destroy(gameObject);
            }

            // Set the property
            material.SetFloat("_Fade", fade);
            backgroundMaterial.SetFloat("_Fade", fade);
        }
    }

    public void removeIcon()
    {
        backgroundMaterial = gameObject.transform.GetChild(0).GetComponent<SpriteRenderer>().material;
        
        isDissolving = true;
    }



    private void movingUpdate()
    {
        if(gameLogic.fallDirection == Vector2.zero)
        {
            return;
        }

        Vector2 newPosition = vectorIndex + gameLogic.fallDirection;

        if(isFallLegal(newPosition) && !isDissolving)
        {
            transform.position = new Vector3(newPosition.y*3+3, (gameLogic.row-newPosition.x)*3+3, 0);
            gameLogic.mapMatrix[(int)newPosition.x, (int)newPosition.y] = id;
            gameLogic.mapMatrix[(int)vectorIndex.x, (int)vectorIndex.y] = -1;

            gameLogic.iconMatrix[(int)vectorIndex.x, (int)vectorIndex.y] = null;
            gameLogic.iconMatrix[(int)newPosition.x, (int)newPosition.y] = this;
            
            vectorIndex = newPosition;
        }
    }

    private bool isFallLegal(Vector2 newPosition)
    {
        if(newPosition.x < 1 || newPosition.y < 1 || newPosition.x > gameLogic.row || newPosition.y > gameLogic.col)
        {
            return false;
        }

        if(gameLogic.mapMatrix[(int)newPosition.x, (int)newPosition.y] != -1)
        {
            return false;
        }

        return true;
    }

    public void swapPosition(Vector2 newPosition)
    {
        transform.position = new Vector3(newPosition.y*3+3, (gameLogic.row-newPosition.x)*3+3, 0);
        gameLogic.mapMatrix[(int)newPosition.x, (int)newPosition.y] = id;

        gameLogic.iconMatrix[(int)newPosition.x, (int)newPosition.y] = this;
        
        vectorIndex = newPosition;
    }


    // setter getter region
    public void setVectorIndex(Vector2 vector)
    {
        vectorIndex = vector;
    }

    public Vector2 getVectorIndex()
    {
        return vectorIndex;
    }

    public bool getSelect()
    {
        return isSelect;
    }

    public void setSelect(bool isSelect)
    {
        this.isSelect = isSelect;
    }

    public void setId(int id)
    {
        this.id = id;
    }

    public int getId()
    {
        return id;
    }
}
