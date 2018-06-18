using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TheStack : MonoBehaviour
{

    public Text textScore;
    public GameObject endPanel;

    private const float BOUNDS_SIZE = 3.5f;   //wielkosc kwadratu 
    private const float STACK_MOVING_SPEED = 5.0f;
    private const float ERROR_MARGIN = 0.1f;


    private GameObject[] theStack;
    private Vector2 stackBounds = new Vector2(BOUNDS_SIZE, BOUNDS_SIZE);

    private int stackIndex;       //indeks elementu
    private int scoreCount = 0;   //wynik
    private int combo = 0;

    private float tileTransition = 0.0f;        //pozcyja
    private float tileSpeed = 2.5f;    //szybkosc poruszania kwadratu
    private float secondaryPosition;   //update pozycji kwadratu

    private bool isMovingOnX = true;    //zmiana poruszania kwadratu
    private bool gameOver = false;

    private Vector3 desiredPosition;
    private Vector3 lastTilePosition;

    // Use this for initialization

    private void Start()
    {
        theStack = new GameObject[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            theStack[i] = transform.GetChild(i).gameObject;
        }

        stackIndex = transform.childCount - 1;
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (PlaceTile())
            {
                SpawnTile();
                scoreCount++;
                textScore.text = scoreCount.ToString();
            }
            else
            {
                EndGame();
            }
        }

        MoveTile();
        //move the stack
        transform.position = Vector3.Lerp(transform.position, desiredPosition, STACK_MOVING_SPEED * Time.deltaTime);
    }

    private void MoveTile()
    {
        if (gameOver)
        {
            return;
        }

        tileTransition += Time.deltaTime * tileSpeed;
        if (isMovingOnX)
        {
            theStack[stackIndex].transform.localPosition = new Vector3(Mathf.Sin(tileTransition) * BOUNDS_SIZE, scoreCount, secondaryPosition);
        }
        else
        {
            theStack[stackIndex].transform.localPosition = new Vector3(secondaryPosition, scoreCount, Mathf.Sin(tileTransition) * BOUNDS_SIZE);
        }
    }


    private void SpawnTile()
    {
        lastTilePosition = theStack[stackIndex].transform.localPosition;
        stackIndex--;
        if (stackIndex < 0)
        {
            stackIndex = transform.childCount - 1;
        }
        desiredPosition = (Vector3.down) * scoreCount;
        theStack[stackIndex].transform.localPosition = new Vector3(0, scoreCount, 0);
        theStack[stackIndex].transform.localScale = new Vector3(stackBounds.x, 1, stackBounds.y);    //zmniejszenie kwadratu
    }

    private bool PlaceTile()
    {
        Transform t = theStack[stackIndex].transform;

        if (isMovingOnX)
        {
            float deltaX = lastTilePosition.x - t.position.x;
            if (Mathf.Abs(deltaX) > ERROR_MARGIN)
            {

                //cięcie kwadratu
                combo = 0;
                stackBounds.x -= Mathf.Abs(deltaX);
                if (stackBounds.x <= 0)
                    return false;

                float middle = lastTilePosition.x + t.localPosition.x / 2;
                t.localScale = new Vector3(stackBounds.x, 1, stackBounds.y);
                t.localPosition = new Vector3(middle - (lastTilePosition.x / 2), scoreCount, lastTilePosition.z);

            }
            else
            {
                combo++;
                t.localPosition = new Vector3(lastTilePosition.x, scoreCount, lastTilePosition.z);
            }
        }
        else
        {
            float deltaZ = lastTilePosition.z - t.position.z;
            if (Mathf.Abs(deltaZ) > ERROR_MARGIN)
            {

                //cięcie kwadratu
                combo = 0;
                stackBounds.y -= Mathf.Abs(deltaZ);
                if (stackBounds.y <= 0)
                    return false;

                float middle = lastTilePosition.z + t.localPosition.z / 2;
                t.localScale = new Vector3(stackBounds.x, 1, stackBounds.y);
                t.localPosition = new Vector3(lastTilePosition.x, scoreCount, middle - (lastTilePosition.z / 2));
            }
            else
            {
                combo++;
                t.localPosition = new Vector3(lastTilePosition.x, scoreCount, lastTilePosition.z);
            }
        }


        if (isMovingOnX)
        {
            secondaryPosition = t.localPosition.x;
        }
        else
        {
            secondaryPosition = t.localPosition.z;
        }

        isMovingOnX = !isMovingOnX;
        return true;
    }

    private void EndGame()
    {
        Debug.Log("LOSE");
        if(PlayerPrefs.GetInt("score") < scoreCount){
            PlayerPrefs.SetInt("score", scoreCount);
        }
        gameOver = true;
        endPanel.SetActive(true);
        theStack[stackIndex].AddComponent<Rigidbody>();
    }

    public void OnButtonClick(string sceneName){
        SceneManager.LoadScene(sceneName);
    }
}