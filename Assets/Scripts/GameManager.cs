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
    public bool vsAI = false;                           // Flag 标记 当前模式
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
        if (end) return;
        if (vsAI && currPlayer == player2)
        {
            AIMove();
        }
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
            Debug.Log("Player2\n");
        }
    }

    private void OnPlay(int x, int y)
    {
        if ((vsAI && currPlayer != player1) || records[x, y] != 0 || end) return;
        Debug.Log("now, " + currPlayer +  " is playing\n");
        int tmp = currPlayer;
        TurnChange(-currPlayer);
        records[x, y] = tmp;
        // uiManager.show
        ++cnt;
        if (cnt == 9) { EndGame(0); }
        else if (CheckWin(x, y) == player1) { EndGame(player1); }
        else if (CheckWin(x, y) == player2) { EndGame(player2); }

    }

    /// <summary>
    /// 遍历横行和竖行以及斜对角，在走棋之后判定是否有一方胜利, 该方法借用自@linnndenz
    /// 项目地址: <see href="https://github.com/linnndenz/TicTacToe"/>
    /// </summary>
    /// <param name="x">只读属性的x的引用，表示x坐标位置</param>
    /// <param name="y">只读属性的y的引用，表示y坐标位置</param>
    /// <returns>玩家代号</returns>
    private int CheckWin(in int x, in int y)
    {
        int currMark = records[x, y];
        bool isWin = true;

        // 判断横向
        for (int i = 1; i <= 2; i++)
        {
            if (records[x, (y + i) % 3] != currMark)
            {
                isWin = false;
                break;
            }
        }
        if (isWin)
        {
            return currMark;
        }

        // 判断纵向
        isWin = true;
        for (int i = 1; i <= 2; i++)
        {
            if (records[(x + i) % 3, y] != currMark)
            {
                isWin = false;
                break;
            }
        }
        if (isWin)
        {
            return currMark;
        }

        // 中间和四角位置判定斜向
        int abs = Mathf.Abs(x - y);
        if (abs != 1)
        {
            //（0，0）、（1，1）、（2，2）
            if (abs == 0)
            {
                isWin = true;
                for (int i = 1; i <= 2; i++)
                {
                    if (records[(x + i) % 3, (y + i) % 3] != currMark)
                    {
                        isWin = false;
                        break;
                    }
                }
                if (isWin)
                {
                    return currMark;
                }
            }

            //（0，2）、（1，1）、（2，0）
            if (abs == 2 || x == 1)
            {
                isWin = true;
                // 判断x，也可分往右上和往左下
                switch (x)
                {
                    case 0:
                        if (records[1, 1] != currMark || records[2, 0] != currMark) isWin = false;
                        break;
                    case 1:
                        if (records[0, 2] != currMark || records[2, 0] != currMark) isWin = false;
                        break;
                    case 2:
                        if (records[0, 2] != currMark || records[1, 1] != currMark) isWin = false;
                        break;
                }
                if (isWin)
                {
                    return currMark;
                }
            }
        }
        return 0;
    }

    void EndGame(in int player)
    {
        end = true;
        if (player == 0)
        {
            Debug.Log("draw");
        }
        else if (player == 1)
        {
            Debug.Log("Player1 wins");
        }
        else if (vsAI && player == -1)
        {
            Debug.Log("computer wins");
        }
        else
        {
            Debug.Log("player2 wins");
        }
        // Uimanager.showreset()
        return;
    }

    #region AI logics
    private void TryPlay(int x, int y)
    {
        records[x, y] = currPlayer;
        currPlayer = currPlayer == player1 ? player2 : player1;
    }
    private void UndoTryPlay(int x, int y)
    {
        records[x, y] = 0;
        currPlayer = currPlayer == player1 ? player2 : player1;
    }
    int bestX, bestY;

    /// <summary>
    /// 极大极小搜索，该方法借用自@linnndenz
    /// 项目地址: <see href="https://github.com/linnndenz/TicTacToe"/>
    /// </summary>
    /// <param name="depth">目前深度，最大为9，即棋盘上可落子的数量</param>
    /// <returns>评估值</returns>
    private int MinMaxSearch(int depth)
    {
        if (depth == 9)
        {
            return 0;
        }

        int bestValue;
        int value;
        if (currPlayer == player2) bestValue = int.MinValue;
        else bestValue = int.MaxValue;

        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                if (records[i, j] != 0) continue;

                if (currPlayer == player2)
                {
                    TryPlay(i, j);
                    if (CheckWin(i, j) == player2)
                    {
                        value = int.MaxValue;
                    }
                    else
                    {
                        value = MinMaxSearch(depth + 1);
                    }
                    UndoTryPlay(i, j);

                    if (value >= bestValue)
                    {
                        bestValue = value;
                        if (depth == cnt)
                        {
                            bestX = i;
                            bestY = j;
                        }
                    }
                }
                else
                {
                    TryPlay(i, j);
                    if (CheckWin(i, j) == player1)
                    {
                        value = int.MinValue;
                    }
                    else
                    {
                        value = MinMaxSearch(depth + 1);
                    }
                    UndoTryPlay(i, j);

                    if (value <= bestValue)
                    {
                        bestValue = value;
                        if (depth == cnt)
                        {
                            bestX = i; bestY = j;
                        }
                    }
                }
                records[i, j] = 0; // 复原棋盘
            }
        }
        return bestValue;
    }

    private void AIMove()
    {
        Debug.Log("AI move");
        MinMaxSearch(cnt);
        records[bestX, bestY] = player2;    // well it's guaranteed to be the AI playing this step, unlike previous method
        TurnChange(player1);
        ++cnt;

        // uiManager
        if (cnt == 9) { EndGame(0); }
        else if (CheckWin(bestX, bestY) == player2) { EndGame(player2); }
    }
    #endregion
}
