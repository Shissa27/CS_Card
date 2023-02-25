using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombCard : MonoBehaviour
{
    public bool isChoosen = false;
    private float increasingSize = 1.15f;
    
    private GameObject _handObj;

    private GameObject _gameManager;

    void Start()
    {
        _handObj = GameObject.Find("Hand");

        transform.parent = _handObj.transform;
        gameObject.name = "Card";
        
        _gameManager = GameObject.Find("/GameManager");
        
    }

    public void GiveBomb()
    {
        transform.parent = _handObj.transform;
        transform.parent.GetComponent<HandScript>().CardsOnHand -= 1;
        _gameManager.GetComponent<GameManager>().RemoveCardFromHand(gameObject);
        Destroy(gameObject);
    }
    
}
