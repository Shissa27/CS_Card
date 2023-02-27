using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombCard : MonoBehaviour
{
    public GameObject bomb;
    private GameObject _handObj;
    private GameObject _gameManager;

    void Start()
    {
        _handObj = GameObject.Find("Hand");

        transform.parent = _handObj.transform;
        gameObject.name = "Card";
        
        _gameManager = GameObject.Find("/GameManager");
    }

	// give bomb to figure
    public void GiveBomb(GameObject figure)
    {
        transform.parent = _handObj.transform;
        transform.parent.GetComponent<HandScript>().CardsOnHand -= 1;
        _gameManager.GetComponent<GameManager>().RemoveCardFromHand(gameObject);
        
        Instantiate(bomb, figure.transform).name = "Bomb";	// create bomb on figure
        Destroy(gameObject);								// destroy "Bomb Card"
    }
}
