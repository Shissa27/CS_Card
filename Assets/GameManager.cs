using System;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviourPunCallbacks
{
    public GameObject canvasEndTurnButton;
    public GameObject textMoney;
    public GameObject textRound;
    
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
    
    private GameObject _chosenObj;
        
    public List<GameObject> cellsGameField;
    public List<GameObject> ourFigures;

    public PhotonView _photonView;

    private bool _teamCt;
    private bool _myTurn;
    private int _money;
    private int _round;
    
    private int _cardsOnHand;
    private List<GameObject> _handCards;
    private void Start()
    {
        InitVariables();
        if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
        {
            _teamCt = false;
            _myTurn = true;
            Debug.Log("Second player is joined the room!");
        }
        else
        {
            _teamCt = true;
            _myTurn = false;
        }
        canvasEndTurnButton.SetActive(_myTurn);
        AddMoney(5);

        // taking 3 cards
        for (var i = 0; i < 3; i++)
        {
            TakeCard();
        }
    }

    private void InitVariables()
    {
        _photonView = GetComponent<PhotonView>();
        _handCards = new List<GameObject>();
        _chosenObj = GameObject.Find("/ChoosenObj");

        _round = 1;
        
        T_prefabs_cards = new GameObject[] { prefab_T_AK47, prefab_T_AWP, prefab_T_GLOCK, prefab_T_SAWEDOFF, prefab_T_MAC10, prefab_T_NEGEV};
        CT_prefabs_cards = new GameObject[] { prefab_CT_M4A1S, prefab_CT_AWP, prefab_CT_USPS, prefab_CT_NOVA, prefab_CT_MP5, prefab_CT_NEGEV};
    }
     
    public bool IsMyTurn(){ return _myTurn; }

    public int GetMoney()
    {
        return _money;
        
    }

    public void AddMoney(int money)
    {
        this._money += money;
        textMoney.GetComponent<Text>().text = this._money + "$";
    }

    public int GetCardsOnHand()
    {
        return _cardsOnHand;
    }

    public void AddCardsOnHand(int difference)
    {
        _cardsOnHand += difference;
    }

    public void AddCardOnHand(GameObject toAdd)
    {
        _handCards.Add(toAdd);
        _cardsOnHand++;
        RedrawHand();
    }
    
    public void RemoveCardFromHand(GameObject toRemove)
    {
        _handCards.Remove(toRemove);
        _cardsOnHand--;
        RedrawHand();
    }
    
    private void RedrawHand()
    {
        switch (_cardsOnHand)
        {
            case 1:
                _handCards[0].transform.position = new Vector3(1.9f, -3.2f, -3.7f);
                _handCards[0].transform.rotation = Quaternion.Euler(0f, 0f, 0f);
                break;
            case 2:
                _handCards[0].transform.position = new Vector3(1.638f, -3.35f, -3.5f);
                _handCards[0].transform.rotation = Quaternion.Euler(0f, 0f, 5.3f);
                
                _handCards[1].transform.position = new Vector3(2.324f, -3.35f, -3.7f);
                _handCards[1].transform.rotation = Quaternion.Euler(0f, 0f, -5.3f);
                break;
            case 3:
                _handCards[0].transform.position = new Vector3(1.2f, -3.3f, -3.5f);
                _handCards[0].transform.rotation = Quaternion.Euler(0f, 0f, 9.77f);
                
                _handCards[1].transform.position = new Vector3(1.9f, -3.2f, -3.7f);
                _handCards[1].transform.rotation = Quaternion.Euler(0f, 0f, 0f);
                
                _handCards[2].transform.position = new Vector3(2.45f, -3.3f, -3.9f);
                _handCards[2].transform.rotation = Quaternion.Euler(0f, 0f, -9.4f);
                break;
            case 4:
                _handCards[0].transform.position = new Vector3(1f, -3.5f, -3.3f);
                _handCards[0].transform.rotation = Quaternion.Euler(0f, 0f, 16.5f);
                
                _handCards[1].transform.position = new Vector3(1.638f, -3.35f, -3.5f);
                _handCards[1].transform.rotation = Quaternion.Euler(0f, 0f, 5.3f);
                
                _handCards[2].transform.position = new Vector3(2.324f, -3.35f, -3.7f);
                _handCards[2].transform.rotation = Quaternion.Euler(0f, 0f, -5.3f);
                
                _handCards[3].transform.position = new Vector3(2.956f, -3.5f, -3.9f);
                _handCards[3].transform.rotation = Quaternion.Euler(0f, 0f, -16.5f);
                break;
            case 5:
                _handCards[0].transform.position = new Vector3(0.65f, -3.5f, -3.3f);
                _handCards[0].transform.rotation = Quaternion.Euler(0f, 0f, 18f);
                
                _handCards[1].transform.position = new Vector3(1.2f, -3.3f, -3.5f);
                _handCards[1].transform.rotation = Quaternion.Euler(0f, 0f, 9.77f);
                
                _handCards[2].transform.position = new Vector3(1.9f, -3.2f, -3.7f);
                _handCards[2].transform.rotation = Quaternion.Euler(0f, 0f, 0f);
                
                _handCards[3].transform.position = new Vector3(2.45f, -3.3f, -3.9f);
                _handCards[3].transform.rotation = Quaternion.Euler(0f, 0f, -9.4f);
                
                _handCards[4].transform.position = new Vector3(3f, -3.5f, -4.1f);
                _handCards[4].transform.rotation = Quaternion.Euler(0f, 0f, -23.68f);
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
        if (!_myTurn)
        {
            AddMoney(2);                // receive +2$
            RestoreMovementPoints();    // set 1 MP to all our figures on board
            TakeCard();                 // take one card on the hand
        }
        else if(_teamCt)
            photonView.RPC("IncreaseRound", RpcTarget.All);
        
        _myTurn = !_myTurn;
        canvasEndTurnButton.SetActive(_myTurn);
    }

    [PunRPC]
    void IncreaseRound()
    {
        _round++;
        textRound.GetComponent<Text>().text = "Round: " + _round;
    }

    private void RestoreMovementPoints()
    {
        foreach (var f in ourFigures)
        {
            f.GetComponent<TapCharacter>().SetMovementPoints(1);
        }
    }
    
    public bool GetTeam() { return _teamCt; }

    public void LeaveRoom() { PhotonNetwork.LeaveRoom(); }

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
            var position = f.transform.position;
            UpdateFogOfWar(position.x, position.y, f.GetComponent<TapCharacter>().radiusView);
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

    private void ResetFogOfWar()
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
        if(_chosenObj.transform.childCount != 0){
            Transform children = _chosenObj.transform.GetChild(0);
            if(children.GetComponent<TapCard>()){
                children.GetComponent<TapCard>().ClearChoosenCard();
            }
            return;
        }

        int cardsOnHand = GetCardsOnHand();
        bool teamCt = GetTeam();
        int cardID = UnityEngine.Random.Range(0, 6); // fix this to (0, 6)
        
        GameObject cardToCreate = teamCt ? CT_prefabs_cards[cardID] : T_prefabs_cards[cardID];
        
        // overflow of cards on the hand (=5 cards)
        if (cardsOnHand >= 5)
        {
            return;
        }
        
        GameObject newCard = Instantiate(cardToCreate);
        AddCardOnHand(newCard);
    }
    
}
