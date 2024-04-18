using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public GameObject resetButton;
    // Start is called before the first frame update
    void Start()
    {
        ResetButtonDisplay(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void ResetButtonDisplay(bool on)
    {
        resetButton.SetActive(on);
    }
}
