using System;
using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviourPunCallbacks
{
    public GameObject plrPrefab;
    public GameObject canvasEndTurnButton;
    public GameObject textMoney;
    
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
        
    public List<GameObject> cellsGameField;
    public List<GameObject> ourFigures;

    public PhotonView _photonView;

    private bool teamCT;
    private bool myTurn;
    private int money;
    
    private int cardsOnHand;
    private List<GameObject> handCards;
    private void Start()
    {
        InitVariables();
        if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
        {
            teamCT = false;
            myTurn = false;
            Debug.Log("Second player is joined the room!");
        }
        else
        {
            teamCT = true;
            myTurn = true;
        }
        canvasEndTurnButton.SetActive(myTurn);
        AddMoney(5);

        // taking 3 cards
        for (int i = 0; i < 3; i++)
        {
            TakeCard();
        }
    }

    private void InitVariables()
    {
        _photonView = GetComponent<PhotonView>();
        handCards = new List<GameObject>();
        choosenObj = GameObject.Find("/ChoosenObj");
        
        T_prefabs_cards = new GameObject[] { prefab_T_AK47, prefab_T_AWP, prefab_T_GLOCK, prefab_T_SAWEDOFF, prefab_T_MAC10, prefab_T_NEGEV};
        CT_prefabs_cards = new GameObject[] { prefab_CT_M4A1S, prefab_CT_AWP, prefab_CT_USPS, prefab_CT_NOVA, prefab_CT_MP5, prefab_CT_NEGEV};
    }
     
    public bool IsMyTurn(){ return myTurn; }

    public int GetMoney()
    {
        return money;
        
    }

    public void AddMoney(int _money)
    {
        money += _money;
        textMoney.GetComponent<Text>().text = money + "$";
    }

    public int GetCardsOnHand()
    {
        return cardsOnHand;
    }

    public void AddCardsOnHand(int difference)
    {
        cardsOnHand += difference;
    }

    public void AddCardOnHand(GameObject toAdd)
    {
        handCards.Add(toAdd);
        cardsOnHand++;
        RedrawHand();
    }
    
    public void RemoveCardFromHand(GameObject toRemove)
    {
        handCards.Remove(toRemove);
        cardsOnHand--;
        RedrawHand();
    }
    
    public void RedrawHand()
    {
        switch (cardsOnHand)
        {
            case 1:
                handCards[0].transform.position = new Vector3(1.9f, -3.2f, -3.7f);
                handCards[0].transform.rotation = Quaternion.Euler(0f, 0f, 0f);
                break;
            case 2:
                handCards[0].transform.position = new Vector3(1.638f, -3.35f, -3.5f);
                handCards[0].transform.rotation = Quaternion.Euler(0f, 0f, 5.3f);
                
                handCards[1].transform.position = new Vector3(2.324f, -3.35f, -3.7f);
                handCards[1].transform.rotation = Quaternion.Euler(0f, 0f, -5.3f);
                break;
            case 3:
                handCards[0].transform.position = new Vector3(1.2f, -3.3f, -3.5f);
                handCards[0].transform.rotation = Quaternion.Euler(0f, 0f, 9.77f);
                
                handCards[1].transform.position = new Vector3(1.9f, -3.2f, -3.7f);
                handCards[1].transform.rotation = Quaternion.Euler(0f, 0f, 0f);
                
                handCards[2].transform.position = new Vector3(2.45f, -3.3f, -3.9f);
                handCards[2].transform.rotation = Quaternion.Euler(0f, 0f, -9.4f);
                break;
            case 4:
                handCards[0].transform.position = new Vector3(1f, -3.5f, -3.3f);
                handCards[0].transform.rotation = Quaternion.Euler(0f, 0f, 16.5f);
                
                handCards[1].transform.position = new Vector3(1.638f, -3.35f, -3.5f);
                handCards[1].transform.rotation = Quaternion.Euler(0f, 0f, 5.3f);
                
                handCards[2].transform.position = new Vector3(2.324f, -3.35f, -3.7f);
                handCards[2].transform.rotation = Quaternion.Euler(0f, 0f, -5.3f);
                
                handCards[3].transform.position = new Vector3(2.956f, -3.5f, -3.9f);
                handCards[3].transform.rotation = Quaternion.Euler(0f, 0f, -16.5f);
                break;
            case 5:
                handCards[0].transform.position = new Vector3(0.65f, -3.5f, -3.3f);
                handCards[0].transform.rotation = Quaternion.Euler(0f, 0f, 18f);
                
                handCards[1].transform.position = new Vector3(1.2f, -3.3f, -3.5f);
                handCards[1].transform.rotation = Quaternion.Euler(0f, 0f, 9.77f);
                
                handCards[2].transform.position = new Vector3(1.9f, -3.2f, -3.7f);
                handCards[2].transform.rotation = Quaternion.Euler(0f, 0f, 0f);
                
                handCards[3].transform.position = new Vector3(2.45f, -3.3f, -3.9f);
                handCards[3].transform.rotation = Quaternion.Euler(0f, 0f, -9.4f);
                
                handCards[4].transform.position = new Vector3(3f, -3.5f, -4.1f);
                handCards[4].transform.rotation = Quaternion.Euler(0f, 0f, -23.68f);
                break;
        }
    }

    public void OnClickEndButtonTurn()
    {
        _photonView.RPC("ChangeTurn", RpcTarget.All);
    }

    [PunRPC]
    public void ChangeTurn()
    {
        // if it now my turn
        if (!myTurn)
        {
            AddMoney(2);                // receive +2$
            RestoreMovementPoints();    // set 1 MP to all our figures on board
            TakeCard();                 // take one card on the hand
        }
        
        myTurn = !myTurn;
        canvasEndTurnButton.SetActive(myTurn);
    }

    private void RestoreMovementPoints()
    {
        foreach (var f in ourFigures)
        {
            f.GetComponent<TapCharacter>().SetMovementPoints(1);
        }
    }
    
    public bool GetTeam() { return teamCT; }

    public void LeaveRoom() { PhotonNetwork.LeaveRoom(); }

    
    
    [PunRPC]
    private void SetTeam(bool _teamCT) { teamCT = _teamCT; }

    public override void OnConnected() // when we connected to the room
    {
        
    }
    public override void OnLeftRoom() // when we disconnect from the room
    {
        SceneManager.LoadScene(0);
    }

    
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.LogFormat("Player {0} entered the room", newPlayer.NickName);

    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.LogFormat("Player {0} left the room", otherPlayer.NickName);
    }

    public void AddCellToGameFieldArray(GameObject cell)
    {
        cellsGameField.Add(cell);
    }

    public List<GameObject> GetCells()
    {
        return cellsGameField;
    }
    
    public void AddFigureToList(GameObject figure)
    {
        ourFigures.Add(figure);
    }
    
    public void RemoveFigureFromList(GameObject figure)
    {
        ourFigures.Remove(figure);
    }

    public List<GameObject> GetOurFigures()
    {
        return ourFigures;
    }

    public void UpdateFogOfWarForAll()
    {
        ResetFogOfWar();
        foreach (var f in ourFigures)
        {
            UpdateFogOfWar(f.transform.position.x, f.transform.position.y, f.GetComponent<TapCharacter>().radiusView);
        }
    }
    public void UpdateFogOfWar(float x, float y, float r)
    {
        /*
         * Updating the fog of war:
         *      for each game cell we check distance from input 'x' and 'y' pos of figure
         *      with distance of view less or equals 'r'
         */
        
        foreach (GameObject c in cellsGameField)
        {
            float xPos = c.GetComponent<GameField>().GetPosX();
            float yPos = c.GetComponent<GameField>().GetPosY();
            
            if (Math.Abs(xPos - x) + Math.Abs(yPos - y) <= r)
            {
                c.GetComponent<GameField>().SetVisible(true);
            }
        }
    }

    public void ResetFogOfWar()
    {
        foreach (GameObject c in cellsGameField)
        {
            c.GetComponent<GameField>().SetVisible(false);
        }
    }

    public GameObject GetCellByVector2(float xCell, float yCell)
    {
        foreach (var c in cellsGameField)
        {
            if (Math.Abs(c.transform.position.x - xCell) + Math.Abs(c.transform.position.y - yCell) <= 0.3f)
            {
                return c;
            }
        }

        return null;
    }

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

        int CardsOnHand = GetCardsOnHand();
        bool teamCT = GetTeam();
        int cardID = UnityEngine.Random.Range(0, 6); // fix this to (0, 6)
        
        GameObject cardToCreate = teamCT ? CT_prefabs_cards[cardID] : T_prefabs_cards[cardID];
        
        // overflow of cards on the hand (=5 cards)
        if (CardsOnHand >= 5)
        {
            return;
        }
        
        GameObject newCard = Instantiate(cardToCreate);
        AddCardOnHand(newCard);

    }
    
}
