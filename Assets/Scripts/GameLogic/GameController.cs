using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    private GameLogic gameLogic;
    private CellBackgroundController background;

    private void Start()
    {
        gameLogic = GetComponent<GameLogic>();
        background = GetComponent<CellBackgroundController>();
    }


    private void Update()
    {
        getInput();
    }


    private void getInput()
    {
        if (Input.GetMouseButtonDown (0) && !PauseController.isPause) {    
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 100f)) {
                // whatever tag you are looking for on your game object
                if(hit.collider.tag == "Icon") 
                {
                    gameLogic.selectIcon(hit.collider.GetComponent<IconController>());                     
                }
            }    
        }
    }
}
