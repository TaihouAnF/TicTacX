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
    private bool vsAI = true;                           // Flag ��� ��ǰģʽ
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
