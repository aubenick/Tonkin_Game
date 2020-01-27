using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WinnerText : MonoBehaviour
{
    public GameObject board;
    public Text winnerText;

    private void Start()
    {

    }

    private void Update()
    {
        bool gameWon = board.GetComponent<TonkinBoard>().winner;
        string text = board.GetComponent<TonkinBoard>().winnerTeam;
        if (gameWon)
        {
            Debug.Log("Game won");
           winnerText.text = text + " is the Winner!";
        }
    }
}
