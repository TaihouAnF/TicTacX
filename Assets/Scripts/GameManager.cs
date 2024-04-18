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
    [SerializeField] private int currPlayer;            // ��ǰ���

    [Header("Atrributes")]
    [SerializeField] private const int player1 = 1;
    [SerializeField] private const int player2 = -1;    // ��vsAIΪtrue����Ϊ����

    // ����
    private Vector2[,] buttonPos = new Vector2[3, 3];   // ����λ��
    private int[,] records = new int[3, 3];             // ��¼����λΪ0��1��-1��Ӧ��ͬ���
    private int cnt;                                    // ������������ͳ��

    // �������
    private int firstHand;                              // ����
    public bool vsAI = false;                           // Flag ��� ��ǰģʽ
    private bool end = false;                           // ��Ϸ����flag


    // Start is called before the first frame update
    void Start()
    {
        firstHand = player1;                            // ���1����
        TurnChange(firstHand);                          
        var buttons = buttonsContainer.GetComponentsInChildren<Button>();
        for (int i = 0; i < buttons.Length; ++i)        // ��ʼ��ť
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
    /// �������к������Լ�б�Խǣ�������֮���ж��Ƿ���һ��ʤ��, �÷���������@linnndenz
    /// ��Ŀ��ַ: <see href="https://github.com/linnndenz/TicTacToe"/>
    /// </summary>
    /// <param name="x">ֻ�����Ե�x�����ã���ʾx����λ��</param>
    /// <param name="y">ֻ�����Ե�y�����ã���ʾy����λ��</param>
    /// <returns>��Ҵ���</returns>
    private int CheckWin(in int x, in int y)
    {
        int currMark = records[x, y];
        bool isWin = true;

        // �жϺ���
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

        // �ж�����
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

        // �м���Ľ�λ���ж�б��
        int abs = Mathf.Abs(x - y);
        if (abs != 1)
        {
            //��0��0������1��1������2��2��
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

            //��0��2������1��1������2��0��
            if (abs == 2 || x == 1)
            {
                isWin = true;
                // �ж�x��Ҳ�ɷ������Ϻ�������
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
    /// ����С�������÷���������@linnndenz
    /// ��Ŀ��ַ: <see href="https://github.com/linnndenz/TicTacToe"/>
    /// </summary>
    /// <param name="depth">Ŀǰ��ȣ����Ϊ9���������Ͽ����ӵ�����</param>
    /// <returns>����ֵ</returns>
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
                records[i, j] = 0; // ��ԭ����
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
