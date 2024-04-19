using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] private GameObject circleChess;
    [SerializeField] private GameObject crossChess;
    [SerializeField] private Transform chessContainer;
    [SerializeField] private Text roundText;
    private List<GameObject> chesses = new(9);
    public GameObject resetButton;
    // Start is called before the first frame update
    void Start()
    {
        ResetButtonDisplay(false);
    }

    public void DisplayChess(int player, Vector2 pos)
    {
        GameObject chess = Instantiate(player == 1 ? crossChess : circleChess, chessContainer);
        chess.GetComponent<RectTransform>().anchoredPosition = pos;
        chesses.Add(chess);
    }

    public void DisplayRoundText(int turn)
    {
        switch (turn)
        {
            case 1:     // P1's Turn
                roundText.text = "P1的回合";
                break;
            case -1:    // P2's Turn
                roundText.text = "P2的回合";
                break;
            case 0:     // Draw
                roundText.text = "平局！";
                break;
            case 2:     // P1 wins
                roundText.text = "P1 赢了！";
                break;
            case -2:    // P2 wins
                roundText.text = "P2 赢了！";
                break;
            case 3:
                roundText.text = "电脑赢了！";
                break;
            default:
                break;
        }
    }

    public void ResetGame()
    {
        foreach(GameObject chess in chesses)
        {
            Destroy(chess);
        }
        chesses.Clear();
        ResetButtonDisplay(false);
    }

    public void EnableReset()
    {
        ResetButtonDisplay(true);
    }

    void ResetButtonDisplay(bool on)
    {
        resetButton.SetActive(on);
    }
}
