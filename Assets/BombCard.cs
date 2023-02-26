using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombCard : MonoBehaviour
{
    public GameObject bomb;
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

    public void GiveBomb(GameObject figure)
    {
        transform.parent = _handObj.transform;
        transform.parent.GetComponent<HandScript>().CardsOnHand -= 1;
        _gameManager.GetComponent<GameManager>().RemoveCardFromHand(gameObject);
        
        Instantiate(bomb, figure.transform).name = "Bomb";
        Destroy(gameObject);
    }
    
}
