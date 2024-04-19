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
    [SerializeField] private Text aiStatusText;
    [SerializeField] private Text player1ScoreText;
    [SerializeField] private Text player2ScoreText;
    [SerializeField] private Text drawScoreText;
    public GameObject resetButton;
    public GameObject startButton;
    private List<GameObject> chesses = new(9);

    // Start is called before the first frame update
    void Start()
    {
        StartButtonDisplay(true);
        ResetButtonDisplay(false);
        DisplayAIStatus(false);
    }

    public void DisplayPlayerScore(int player, int score)
    {
        switch (player)
        {
            case 0:
                drawScoreText.text = "平局: " + score; break;
            case 1:
                player1ScoreText.text = "P1分数: " + score; break;
            case -1:
                player2ScoreText.text = "P2分数: " + score; break;
            default:
                break;
        }
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

    public void DisplayAIStatus(bool on)
    {
        string tmp = on ? "ON" : "OFF";
        aiStatusText.text = "AI状态: " + tmp;
    }

    public void StartGame()
    {
        StartButtonDisplay(false);
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

    void StartButtonDisplay(bool on)
    {
        startButton.SetActive(on);
    }

    void ResetButtonDisplay(bool on)
    {
        resetButton.SetActive(on);
    }
}
