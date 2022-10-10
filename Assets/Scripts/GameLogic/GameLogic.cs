using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;


public class GameLogic : MonoBehaviour
{
    
    [SerializeField] Transform parent;
    [SerializeField] GameObject cellBackground;
    [SerializeField] GameObject[] iconList;
    [SerializeField] GameObject line;
    [SerializeField] int numberOfIcon;


    [Space]
    [SerializeField] float shakeDuration;
    [SerializeField] float shakeAmount;

    [Space]
    public Vector2 fallDirection;
    public int[,] mapMatrix;
    public IconController[,] iconMatrix;
    public int row;
    public int col;
    public int winCount;


    private int[,] visited;
    private bool isFound;
    private List<Vector2> listFound;
    private List<List<IconController>> idMatrix;
    
    private IconController previousSelect;
    private AudioManager audioManager;
    private CameraShake cameraShake ;

    private bool isMute;
    



    private void Awake()
    {

        previousSelect = null;
        initMatrix();
        initIcons();
        winCount = col*row;
    }

    private void Start()
    {
        cameraShake = GetComponent<CameraShake>();
        audioManager = FindObjectOfType<AudioManager>();
        isMute = false;
        audioManager.playSound("MainMusic");
        Time.timeScale = 1f;
    }


    private void Update()
    {
        if(winCount <= 0)
        {
            PauseController pauseController = FindObjectOfType<PauseController>();
            pauseController.loadWinMenu();
        }

        for(int i = 1; i <= row; i++)
        {
            for(int j = 1; j <= col; j++)
            {
                if(iconMatrix[i, j] == null)
                {
                    mapMatrix[i, j] = -1;
                }
            }
        }
    }

    private void initMatrix()
    {
        mapMatrix = new int[row+2, col+2];

        for(int i = 0; i <= row+1; i++)
        {
            for(int j = 0; j <= col+1; j++)
            {
                mapMatrix[i, j] = -1;
            }
        }
    }


    private void initIcons()
    {
        int maxDuplicate = row*col/numberOfIcon+1;
        int[] countDuplicate = new int[numberOfIcon+1];

        List<Vector2> remainPosition = new List<Vector2>();
        idMatrix = new List<List<IconController>>();

        for(int i = 0; i <= numberOfIcon; i++)
        {
            List<IconController> listIndex = new List<IconController>();
            idMatrix.Add(listIndex);
        }
        
        for(int i = 1; i <= row; i++)
        {
            for(int j = 1; j <= col; j++)
            {
                remainPosition.Add(new Vector2(i, j));
            }
        }

        int index = 0;
        iconMatrix = new IconController[row+1, col+1];
        do
        {
            int randIconIndex = Random.Range(0, numberOfIcon);
            if(countDuplicate[randIconIndex] < maxDuplicate)
            {

                // create two icons at the same time to make sure that there is always has pair of icon
                countDuplicate[randIconIndex] += 2;
                for(int j = 0; j < 2; j++)
                {
                    int randIndex = Random.Range(0, remainPosition.Count);
                    int x = (int)remainPosition[randIndex].x;
                    int y = (int)remainPosition[randIndex].y;

                    Vector3 axes = new Vector3(y*3+3, (row-x)*3+3, 0);
                    IconController icon = Instantiate(iconList[randIconIndex], axes, Quaternion.identity, parent).GetComponent<IconController>();
                    Instantiate(cellBackground, axes, Quaternion.identity, icon.GetComponent<Transform>());
                    icon.setVectorIndex(new Vector2(x, y));
                    icon.setId(randIconIndex);

                    iconMatrix[x, y] = icon;


                    mapMatrix[x, y] = randIconIndex;
                    idMatrix[randIconIndex].Add(icon);

                    remainPosition.RemoveAt(randIndex);
                }
                index++;
            }
        }
        while(index < row*col/2);
    }



    // using Branch and Bound algorithm
    private void findPath(Vector2 start, Vector2 target)
    {
        visited = new int[row+2, col+2];
        for(int i = 0; i <= row+1; i++)
        {
            for(int j = 0; j <= col+1; j++)
            {
                visited[i, j] = 0;
            }
        }

        isFound = false;

        listFound = new List<Vector2>();
        
        Vector2 vectorFound = recusive(start, start, target, 0);
        if(vectorFound != Vector2.zero)
        {
            listFound.Add(vectorFound);
        }
    }


    private Vector2 recusive(Vector2 previous, Vector2 current, Vector2 target, int rotateCount)
    {
        if(isFound)
        {
            return Vector2.zero;
        }

        int[] dx = {1, -1, 0, 0};
        int[] dy = {0, 0, 1, -1};

        visited[(int)current.x, (int)current.y] = -1;

        for(int i = 0; i < 4; i++)
        {
            Vector2 next = current + new Vector2(dx[i], dy[i]);

            if(isLegal(next, target, rotateCount+collinear(previous, current, next)))
            {
                if(next.x == target.x && next.y == target.y)
                {
                    isFound = true;
                    listFound.Add(target);
                    return current;
                }
                else
                {
                    Vector2 vectorFound = recusive(current, next, target, rotateCount+collinear(previous, current, next));
                    if(vectorFound != Vector2.zero)
                    {
                        listFound.Add(vectorFound);
                        return current;
                    }
                }
            }
        }

        
        visited[(int)current.x, (int)current.y] = 0;
        return Vector2.zero;
    }

    private int collinear(Vector2 previous, Vector2 current, Vector2 next)
    {
        if(Vector2.Distance(previous, next) == Vector2.Distance(previous, current)+Vector2.Distance(current, next))
        {
            return 0;
        }
        return 1;
    }
    

    private bool isLegal(Vector2 next, Vector2 target, int rotateCount)
    {

        if(next == target && rotateCount < 3)
        {
            return true;
        }


        // out of bound
        if(next.x < 0 || next.y < 0 || next.x > row+1 || next.y > col+1)
        {
            return false;
        }

        // blocked by another object
        if(mapMatrix[(int)next.x, (int)next.y] != -1)
        {
            return false;
        }

        // visited
        if(visited[(int)next.x, (int)next.y] == -1)
        {
            return false;
        }

        // more than 3 lines
        if(rotateCount > 2)
        {
            return false;
        }
        

        return true;
    }


    public void selectIcon(IconController icon)
    {
        Vector2 currentSelect = icon.getVectorIndex();
        if(previousSelect != null)
        {
            // select an icon twice
            if(iconMatrix[(int)currentSelect.x, (int)currentSelect.y].getSelect())
            {
                if(!isMute)
                {
                    audioManager.playSound("UnSelect");
                }
                
                previousSelect.GetComponent<Transform>().GetChild(0).GetComponent<CellBackgroundController>().setSelect(false);
                previousSelect = null;
                iconMatrix[(int)currentSelect.x, (int)currentSelect.y].setSelect(false);
            }
            else
            {
                findPath(currentSelect, previousSelect.getVectorIndex());

                if(isFound && icon.getId() == previousSelect.getId())
                {
                    if(!isMute)
                    {
                        audioManager.playSound("LineDestroy");
                    }

                    Vector3[] listPosition = new Vector3[listFound.Count];
                    for(int i = 0; i < listFound.Count; i++)
                    {
                        listPosition[i] = new Vector3(listFound[i].y*3, (row-listFound[i].x)*3,  0);
                    }

                    LineController newLine = Instantiate(line, listPosition[listFound.Count-1], Quaternion.identity).GetComponent<LineController>();
                    newLine.drawLine(listPosition);

                    icon.GetComponent<Transform>().GetChild(0).GetComponent<CellBackgroundController>().setSelect(true);

                    icon.removeIcon();
                    previousSelect.removeIcon();
                    previousSelect = null;
                }
                else
                {
                    if(!isMute)
                    {
                        audioManager.playSound("WrongSelect");
                    }
                    cameraShake.Shake(shakeDuration, shakeAmount);
                };
            }
        }
        else
        {
            if(!isMute)
            {
                audioManager.playSound("Select");
            }
            previousSelect = icon;
            previousSelect.GetComponent<Transform>().GetChild(0).GetComponent<CellBackgroundController>().setSelect(true);
            iconMatrix[(int)currentSelect.x, (int)currentSelect.y].setSelect(true);
        }
    }


    public void help()
    {
        if(previousSelect != null)
        {
            previousSelect.GetComponent<Transform>().GetChild(0).GetComponent<CellBackgroundController>().setSelect(false);
            iconMatrix[(int)previousSelect.getVectorIndex().x, (int)previousSelect.getVectorIndex().y].setSelect(false);
            previousSelect = null;
        }
        

        for(int i = 0; i <= numberOfIcon; i++)
        {
            for(int j = 0; j < idMatrix[i].Count; j++)
            {
                if(idMatrix[i][j] == null)
                {
                    continue;
                }
                for(int k = j+1; k < idMatrix[i].Count; k++)
                {
                    if(idMatrix[i][k] == null)
                    {
                        continue;
                    }
                    findPath(idMatrix[i][j].getVectorIndex(), idMatrix[i][k].getVectorIndex());
                    if(isFound)
                    {
                        selectIcon(idMatrix[i][j]);
                        selectIcon(idMatrix[i][k]);
                        return;
                    }
                }
            }
        }
    }

    public void shuffle()
    {
        if(previousSelect != null)
        {
            previousSelect.GetComponent<Transform>().GetChild(0).GetComponent<CellBackgroundController>().setSelect(false);
            iconMatrix[(int)previousSelect.getVectorIndex().x, (int)previousSelect.getVectorIndex().y].setSelect(false);
            previousSelect = null;
        }

        List<IconController> remainIcon = new List<IconController>();

        for(int i = 1; i <= row; i++)
        {
            for(int j = 1;  j <= col; j++)
            {
                if(iconMatrix[i, j] != null && mapMatrix[i, j] != -1)
                {
                    remainIcon.Add(iconMatrix[i, j]);
                }
            }
        }


        while(remainIcon.Count > 2)
        {
            int randSelect1 = Random.Range(0, remainIcon.Count-1);
            int randSelect2 = Random.Range(0, remainIcon.Count-1);

            while(randSelect2 == randSelect1)
            {
                randSelect2 = Random.Range(0, remainIcon.Count-1);

            }

            Vector2 position1 = remainIcon[randSelect1].getVectorIndex();
            Vector2 position2 = remainIcon[randSelect2].getVectorIndex();

            remainIcon[randSelect1].swapPosition(position2);
            remainIcon[randSelect2].swapPosition(position1);

            remainIcon.RemoveAt(randSelect1);
            remainIcon.RemoveAt(randSelect2);
        }
    }

    public void newGame()
    {
        for(int i = 1; i <= row; i++)
        {
            for(int j = 1;  j <= col; j++)
            {
                if(iconMatrix[i, j] != null)
                {
                    Destroy(iconMatrix[i, j].gameObject);
                }
            }
        }

        initMatrix();
        initIcons();
        winCount = col*row;
    }

    public void mute()
    {
        isMute = !isMute;

        if(isMute)
        {
            audioManager.stopSound("MainMusic");
        }
        else 
        {
            audioManager.playSound("MainMusic");
        }
    }
}
