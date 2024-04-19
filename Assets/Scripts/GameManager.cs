using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] private List<Button> buttonsContainer;
    [SerializeField] private UIManager uiManager;
    [SerializeField] private int currPlayer;            // 当前玩家

    [Header("Atrributes")]
    [SerializeField] private const int player1 = 1;
    [SerializeField] private const int player2 = -1;    // 若vsAI为true，则为电脑

    // 棋盘
    private Vector2[,] buttonPos = new Vector2[3, 3];   // 棋子位置
    private int[] chessRecord = new int[9];             // 记录，空位为0，1和-1对应不同玩家, i = x * 3 + y
    private int cnt;                                    // 棋盘棋子数量统计

    // 玩家设置
    private int firstHand;                              // 先手
    private bool vsAI = false;                          // Flag 标记 当前模式
    private bool start = false;                         // 游戏开始flag
    private bool end = false;                           // 游戏结束flag


    private readonly float cooldown = 1.0f;
    private float curr_cd;


    // Start is called before the first frame update
    void Start()
    {
        firstHand = player1;                                    // 玩家1先手                 
        curr_cd = cooldown;                                     // 初始化AI冷却时间
        for (int i = 0; i < buttonsContainer.Count; ++i)        // 初始化按钮
        {                                       
            int x = i / 3, y = i % 3;
            buttonsContainer[i].onClick.AddListener(() => OnPlay(x, y));
            RectTransform rectTransform = buttonsContainer[i].GetComponent<RectTransform>();
            buttonPos[x, y] = new Vector2(rectTransform.anchoredPosition.x, rectTransform.anchoredPosition.y);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (end) return;
        if (!start) return;
        if (vsAI && currPlayer == player2) 
        {
            if (curr_cd <= 0.0f)
            {
                AIMove();
                curr_cd = cooldown;
            }
            else
            {
                curr_cd -= Time.deltaTime;
            }
        }
    }

    private void OnPlay(int x, int y)
    {
        if ((vsAI && currPlayer != player1) || chessRecord[x * 3 + y] != 0 || end || !start) return;
        Debug.Log("now, player " + currPlayer +  " is playing\n");
        chessRecord[x * 3 + y] = currPlayer;
        uiManager.DisplayChess(currPlayer, buttonPos[x, y]);
        ++cnt;
        if (cnt == 9) { EndGame(0); }
        else if (CheckWin(x, y) == currPlayer) { EndGame(currPlayer); }
        TurnChange(-currPlayer);
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
        int currMark = chessRecord[x * 3 + y];
        bool isWin = true;

        // 判断横向
        for (int i = 1; i < 3; ++i)
        {
            if (chessRecord[x * 3 + ((y + i) % 3)] != currMark)
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
        for (int i = 1; i < 3; ++i)
        {
            if (chessRecord[((x + i) % 3) * 3 + y] != currMark)
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
                for (int i = 1; i < 3; i++)
                {
                    if (chessRecord[((x + i) % 3) * 3 + (y + i) % 3] != currMark)
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
                        if (chessRecord[4] != currMark || chessRecord[6] != currMark) isWin = false;
                        break;
                    case 1:
                        if (chessRecord[2] != currMark || chessRecord[6] != currMark) isWin = false;
                        break;
                    case 2:
                        if (chessRecord[2] != currMark || chessRecord[4] != currMark) isWin = false;
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
            uiManager.DisplayRoundText(0);
            Debug.Log("Draw!");
        }
        else if (player == 1)
        {
            uiManager.DisplayRoundText(player + 1);
            Debug.Log("Player 1 wins");
        }
        else if (!vsAI && player == -1)
        {
            uiManager.DisplayRoundText(player - 1);
            Debug.Log("Player 2 wins");
        }
        else
        {
            uiManager.DisplayRoundText(3);
            Debug.Log("Computer wins");
        }
        uiManager.EnableReset();
        return;
    }

    #region AI logics

    private void TryPlay(int x, int y)
    {
        chessRecord[x * 3 + y] = currPlayer;
        currPlayer = currPlayer == player1 ? player2 : player1;
    }
    private void UndoTryPlay(int x, int y)
    {
        chessRecord[x * 3 + y] = 0;
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
                if (chessRecord[i * 3 + j] != 0) continue;

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
                chessRecord[i * 3 + j] = 0; // 复原棋盘
            }
        }
        return bestValue;
    }

    private void AIMove()
    {
        Debug.Log("AI move");
        _ = MinMaxSearch(cnt);
        chessRecord[bestX * 3 + bestY] = player2;    // well it's guaranteed to be the AI playing this step, unlike previous method
        uiManager.DisplayChess(player2, buttonPos[bestX, bestY]);
        ++cnt;
        if (cnt == 9) { EndGame(0); }
        else if (CheckWin(bestX, bestY) == player2) { EndGame(player2); }
        TurnChange(player1);
    }

    public void OnAIStateChange()
    {
        vsAI = !vsAI;
        uiManager.DisplayAIStatus(vsAI);
    }

    #endregion

    #region Uitl

    void TurnChange(int curr)
    {
        if (end) return;

        currPlayer = curr;
        // Debug purpose
        if (currPlayer == player1) Debug.Log("Player1's Turn");
        else if (currPlayer == player2 && !vsAI) Debug.Log("Player2's Turn");
        else Debug.Log("Computer's Turn");

        uiManager.DisplayRoundText(currPlayer);
    }

    public void OnReset()
    {
        firstHand = -firstHand; // 反转先手，先手在一开始设置的时候是不变的
        currPlayer = firstHand; // 当前玩家为先手
        end = false;
        Debug.Log("now, player " + currPlayer + " is playing.");
        for (int x = 0; x < 3; ++x)
        {
            for (int y = 0; y < 3; ++y)
            {
                chessRecord[x * 3 + y] = 0;
            }
        }
        cnt = 0;
        uiManager.ResetGame();
        uiManager.DisplayRoundText(currPlayer);
    }

    public void OnGameStart()
    {
        TurnChange(firstHand);
        start = true;
        uiManager.StartGame();
    }

    #endregion
}
