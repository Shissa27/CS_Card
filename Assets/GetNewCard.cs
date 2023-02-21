using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public class GetNewCard : MonoBehaviour
{
    public GameObject[] T_prefabs_cards;
    public GameObject prefab_T_AK47;
    public GameObject prefab_T_AWP;
    public GameObject prefab_T_GLOCK;
    public GameObject prefab_T_SAWEDOFF;
    public GameObject prefab_T_MAC10;
    public GameObject prefab_T_NEGEV;
    
    public GameObject[] CT_prefabs_cards;
    public GameObject prefab_CT_M4A1S;
    public GameObject prefab_CT_AWP;
    public GameObject prefab_CT_USPS;
    public GameObject prefab_CT_NOVA;
    public GameObject prefab_CT_MP5;
    public GameObject prefab_CT_NEGEV;
    
    private GameObject choosenObj;

    private GameObject gameManager;
    // Start is called before the first frame update
    void Start()
    {   
        choosenObj = GameObject.Find("/ChoosenObj");
        gameManager = GameObject.Find("/GameManager");
        T_prefabs_cards = new GameObject[] { prefab_T_AK47, prefab_T_AWP, prefab_T_GLOCK, prefab_T_SAWEDOFF, prefab_T_MAC10, prefab_T_NEGEV};
        CT_prefabs_cards = new GameObject[] { prefab_CT_M4A1S, prefab_CT_AWP, prefab_CT_USPS, prefab_CT_NOVA, prefab_CT_MP5, prefab_CT_NEGEV};
    }
    
    void OnMouseDown(){
        //TakeCard();
    }

    // Take a random card
    private void TakeCard()
    {
        // if we have chosen object now
        if(choosenObj.transform.childCount != 0){
            Transform children = choosenObj.transform.GetChild(0);
            if(children.GetComponent<TapCard>()){
                children.GetComponent<TapCard>().ClearChoosenCard();
            }
            return;
        }

        int CardsOnHand = gameManager.GetComponent<GameManager>().GetCardsOnHand();
        bool teamCT = gameManager.GetComponent<GameManager>().GetTeam();
        int cardID = Random.Range(0, 6); // fix this to (0, 6)
        
        GameObject cardToCreate = teamCT ? CT_prefabs_cards[cardID] : T_prefabs_cards[cardID];
        
        /*
         * 5 cards:
         * - 0.882 -3.163 4
         * - 
         */
        
        // overflow of cards on the hand (=5 cards)
        if (CardsOnHand >= 5)
        {
            return;
        }
        
        GameObject newCard = Instantiate(cardToCreate);
        gameManager.GetComponent<GameManager>().AddCardOnHand(newCard);

    }
}
