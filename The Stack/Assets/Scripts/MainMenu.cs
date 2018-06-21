using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {
    /// <summary>
    /// Zmienna odpowiedzialna za wynik
    /// </summary>
    public Text textScore;
    /// <summary>
    /// Metoda odpowiedzialna za aktualizowanie najlepszego wyniku
    /// </summary>
    private void Start(){
        textScore.text = PlayerPrefs.GetInt("score").ToString();
    }
    /// <summary>
    /// Metoda, dzięki której zostanie załadowana scena z grą
    /// </summary>
	public void ToGame(){
        SceneManager.LoadScene("Game");
    }
}
