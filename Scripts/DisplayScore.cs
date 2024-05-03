using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayScore : MonoBehaviour
{
    public Text textComponent;
    public GameObject gameController;


    private void Awake()
    {
        textComponent = GetComponent<Text>();
        gameController = GameObject.Find("GameController");

    }

    private void Update()
    {
        textComponent.text = "hello"; //gameController.GetComponent<GameLogic>().displayScore();
    }
}
    
