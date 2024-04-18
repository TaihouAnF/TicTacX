using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] private GameObject circleChess;
    [SerializeField] private GameObject crossChess;
    private List<GameObject> chesses = new List<GameObject>();
    public GameObject resetButton;
    // Start is called before the first frame update
    void Start()
    {
        ResetButtonDisplay(false);
    }

    void ResetButtonDisplay(bool on)
    {
        resetButton.SetActive(on);
    }
}
