using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TheStack : MonoBehaviour
{
    /// <summary>
    /// Zmienna odpowiedzialna za aktualny wynik gracza
    /// </summary>
    public Text textScore;

    /// <summary>
    /// Obiekt odpowiedzialny za panel końcowy
    /// </summary>
    public GameObject endPanel;

    /// <summary>
    /// Zmienna odpowiedzialna za wielkość klocka
    /// </summary>
    private const float BOUNDS_SIZE = 3.5f;
    /// <summary>
    /// Zmienna, która decyduje o szybkości poruszania się naszego "stacku"
    /// </summary>
    private const float STACK_MOVING_SPEED = 5.0f;
    /// <summary>
    /// Zmienna odpowiedzialna za margines błędu tak aby łatwiej było spasować elementy
    /// </summary>
    private const float ERROR_MARGIN = 0.1f;

    /// <summary>
    /// Utworzenie zmiennej theStack, która będzie odpowiedzialna za nasze klocki
    /// </summary>
    private GameObject[] theStack;
    /// <summary>
    /// Zmienna typu Vector2
    /// </summary>
    private Vector2 stackBounds = new Vector2(BOUNDS_SIZE, BOUNDS_SIZE);

    /// <summary>
    /// Numer klocka
    /// </summary>
    private int stackIndex;   
    /// <summary>
    /// Wynik
    /// </summary>
    private int scoreCount = 0;
    /// <summary>
    /// Combo
    /// </summary>
    private int combo = 0;

    /// <summary>
    /// Pozycja naszego "stacka"
    /// </summary>
    private float tileTransition = 0.0f;        //pozcyja
    /// <summary>
    /// Szybkośc poruszania kwadratu
    /// </summary>
    private float tileSpeed = 2.5f;             //szybkosc poruszania kwadratu
    /// <summary>
    /// Update pozycji kwadratu
    /// </summary>
    private float secondaryPosition;            //update pozycji kwadratu
    /// <summary>
    /// Zmienna odpowiedzilna za zmiane poruszania kwadratu
    /// </summary>
    private bool isMovingOnX = true;            //zmiana poruszania kwadratu
    /// <summary>
    /// Zmienna sprawdzająca czy jest koniec gry
    /// </summary>
    private bool gameOver = false;

    private Vector3 desiredPosition;
    /// <summary>
    /// Zmienna, dzięki której mamy zapamiętany ostatni klocek
    /// </summary>
    private Vector3 lastTilePosition;

    // Use this for initialization

    /// <summary>
    /// Metoda startowa, która przypisuje nam obiekty do zmiennych w tablicy.
    /// </summary>
    private void Start()
    {
        theStack = new GameObject[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            theStack[i] = transform.GetChild(i).gameObject;
        }

        stackIndex = transform.childCount - 1;
    }

    /// <summary>
    /// Metoda, która po naciśnieciu przycisku uruchamia inne metody odpowiedzialne za resztę gry.
    /// </summary>
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

    /// <summary>
    /// Metoda, dzięki której nasz klocek porusza się po osi x i z.
    /// </summary>
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

    /// <summary>
    /// Metoda, która tworzy nowego klocka u góry. Wielkość jest zależna od ostatniego uciętego klocka
    /// </summary>
    private void SpawnTile()
    {
        lastTilePosition = theStack[stackIndex].transform.localPosition;
        stackIndex--;
        if (stackIndex < 0)
        {
            stackIndex = transform.childCount - 1;
        }
        desiredPosition = (Vector3.down) * scoreCount;  //Shorthand for writing Vector3(0, -1, 0)
        theStack[stackIndex].transform.localPosition = new Vector3(0, scoreCount, 0);
        theStack[stackIndex].transform.localScale = new Vector3(stackBounds.x, 1, stackBounds.y);    //zmniejszenie kwadratu
    }

    /// <summary>
    /// Metoda, która sprawdza w którą strone porusza się klocek i w zależności od tego ucina go.
    /// </summary>
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

                //ciecie kwadratu
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

    /// <summary>
    /// Metoda która jest aktywowana po zakończeeniu rozgrywki. Aktualizuje wynik i włącza panel końcowy
    /// </summary>
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

    /// <summary>
    /// Metoda, która zmienia scenę na menu po naciśnięciu przycisku
    /// </summary>
    /// <param name="sceneName"></param>
    public void OnButtonClick(string sceneName){
        SceneManager.LoadScene(sceneName);
    }
}