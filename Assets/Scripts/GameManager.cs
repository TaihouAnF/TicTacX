using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] private Transform buttonsContainer;
    [SerializeField] private UIManager uiManager;
    [SerializeField] private int currPlayer;            // 当前玩家

    [Header("Atrributes")]
    [SerializeField] private const int player1 = 1;
    [SerializeField] private const int player2 = -1;    // 若vsAI为true，则为电脑

    // 棋盘
    private Vector2[,] buttonPos = new Vector2[3, 3];   // 棋子位置
    private int[,] records = new int[3, 3];             // 记录，空位为0，1和-1对应不同玩家
    private int cnt;                                    // 棋盘棋子数量统计

    // 玩家设置
    private int firstHand;                              // 先手
    private bool vsAI = true;                           // Flag 标记 当前模式
    private bool end = false;                           // 游戏结束flag


    // Start is called before the first frame update
    void Start()
    {
        firstHand = player1;                            // 玩家1先手
        TurnChange(firstHand);                          
        var buttons = buttonsContainer.GetComponentsInChildren<Button>();
        for (int i = 0; i < buttons.Length; ++i)        // 初始按钮
        {                                       
            int x = i / 3, y = i % 3;
            buttons[i].onClick.AddListener(() => OnPlay(x, y));
            RectTransform rectTransform = buttons[i].GetComponent<RectTransform>();
            buttonPos[x, y] = new Vector2(rectTransform.anchoredPosition.x, rectTransform.anchoredPosition.y);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void TurnChange(int curr)
    {
        currPlayer = curr;
        if (currPlayer == player1)
        {
            Debug.Log("Player1\n");
        }
        else
        {
            Debug.Log("Player1\n");
        }
    }

    private void OnPlay(int x, int y)
    {
        if ((vsAI && currPlayer != player1) || records[x, y] != 0 || end) return;
        Debug.Log("now, " + currPlayer +  " is playing\n");
    }
}
