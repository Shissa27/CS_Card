using System;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Class of the game manager
/// </summary>
public class GameManager : MonoBehaviourPunCallbacks
{
    public GameObject canvasEndTurnButton;
    public GameObject buttonBomb;
    public GameObject buttonDefuse;
    public GameObject prefabBomb;
    public GameObject prefabBombOnFigure;
    
    public GameObject textMoney;
    public GameObject textRound;
    
    public GameObject prefabTextBombRound;
    public GameObject textBombRound;
    
    public GameObject prefabTextDefuseRound;
    public GameObject textDefuseRound;

    public GameObject gameOverMenu;

    public GameObject[] T_prefabs_cards;
    public GameObject prefab_T_AK47;
    public GameObject prefab_T_AWP;
    public GameObject prefab_T_GLOCK;
    public GameObject prefab_T_SAWEDOFF;
    public GameObject prefab_T_MAC10;
    public GameObject prefab_T_NEGEV;

    private const int TimeForPlant = 1;
    private const int TimeForDefuse = 3;
    private const int TimeForExplode = 7;

    public GameObject prefab_T_card_bomb;
    private Vector3 _bombSite;
    private GameObject _plantBombButton;
    private GameObject _bomber;
    private bool _planting;
    private int _timeToPlant;
    
    private GameObject _droppedBomb;
    private bool _bombDropped;
    private Vector3 _posDroppedBomb;

    private bool _bombPlanted;
    private int _timeToExplode;

    private GameObject _toDefuseButton;
    private int _timeToDefuse;
    private bool _defusing;

    public GameObject[] CT_prefabs_cards;
    public GameObject prefab_CT_M4A1S;
    public GameObject prefab_CT_AWP;
    public GameObject prefab_CT_USPS;
    public GameObject prefab_CT_NOVA;
    public GameObject prefab_CT_MP5;
    public GameObject prefab_CT_NEGEV;

    private GameObject _chosenObj;

    public List<GameObject> cellsGameField;
    private List<GameObject> _cellsSpawn;
    public List<GameObject> ourFigures;

    public PhotonView _photonView;

    private bool _teamCt;
    private bool _myTurn;
    private int _money;
    private int _round;
    private bool _gameIsOver;

    private int _cardsOnHand;
    private List<GameObject> _handCards;

    private void Start()
    {
        InitVariables();

        SetUpTeam();

        canvasEndTurnButton.SetActive(_myTurn); // turning off button "End turn"

        AddMoney(5);

        // taking 3 cards
        for (var i = 0; i < 3; i++)
            TakeCard();

        // taking a bomb card to T hand
        if (!_teamCt)
            TakeCard(prefab_T_card_bomb);
    }

    private void SetUpTeam()
    {
        // setup our team
        if (PhotonNetwork.CurrentRoom.PlayerCount == 2) // if players in room == 2
        {
            _teamCt = false; // ==> we playing on T side
            _myTurn = false;
            Debug.Log("Second player is joined the room!");
        }
        else // if players in room == 1
        {
            _teamCt = true; // ==> we playing on CT side
            _myTurn = true;
        }
    }

    private void InitVariables()
    {
        _photonView = GetComponent<PhotonView>();
        _handCards = new List<GameObject>();
        _cellsSpawn = new List<GameObject>();
        _chosenObj = GameObject.Find("/ChoosenObj");

        _round = 1;

        T_prefabs_cards = new GameObject[]
            { prefab_T_AK47, prefab_T_AWP, prefab_T_GLOCK, prefab_T_SAWEDOFF, prefab_T_MAC10, prefab_T_NEGEV };
        CT_prefabs_cards = new GameObject[]
            { prefab_CT_M4A1S, prefab_CT_AWP, prefab_CT_USPS, prefab_CT_NOVA, prefab_CT_MP5, prefab_CT_NEGEV };

        _bombSite = new Vector3(1.858f, 1.235f, -0.2f);
        _bomber = null;
        _planting = false;
        _bombPlanted = false;
        _timeToPlant = TimeForPlant;
        _timeToDefuse = TimeForDefuse;
        _timeToExplode = TimeForExplode;
    }

    public void AddSpawnCell(GameObject spawnCell)
    {
        _cellsSpawn.Add(spawnCell);
    }

    public bool GetBombPlanted()
    {
        return _bombPlanted;
    }

    public void SetBombPlanted(bool bombPlanted)
    {
        _bombPlanted = bombPlanted;
    }

    public bool IsMyTurn()
    {
        return _myTurn;
    }

    public int GetMoney()
    {
        return _money;
    }

    public void AddMoney(int money)
    {
        _money += money;
        textMoney.GetComponent<Text>().text =
            _money + "$";
    }

    public int GetCardsOnHand()
    {
        return _cardsOnHand;
    }

    // increasing counter of cards on hand
    public void AddCardsOnHand(int difference)
    {
        _cardsOnHand += difference;
    }

    // take %toAdd% card to hand
    public void AddCardOnHand(GameObject toAdd)
    {
        _handCards.Add(toAdd);
        _cardsOnHand++;
        RedrawHand();
    }

    // remove %toRemove% card from hand
    public void RemoveCardFromHand(GameObject toRemove)
    {
        _handCards.Remove(toRemove);
        _cardsOnHand--;
        RedrawHand();
    }

    // redraw all card on hand:
    //  change positions of all card on hand
    private void RedrawHand()
    {
        switch (_cardsOnHand)
        {
            case 1: // if we have only 1 card on hand
                _handCards[0].transform.position = new Vector3(1.9f, -3.2f, -3.7f);
                _handCards[0].transform.rotation = Quaternion.Euler(0f, 0f, 0f);
                break;
            case 2: // if we have 2 cards on hand
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

    // listener onClick of button "End Turn"
    public void OnClickEndButtonTurn()
    {
        _photonView.RPC("ChangeTurn", RpcTarget.All);
    }

    // change turn 
    [PunRPC]
    public void ChangeTurn()
    {
        // if it's will be my turn now
        if (!_myTurn)
        {
            /*
             *  1) receive +2$
             *  2) set 2 MP to all our figures on board
             *  3) take 1 card on the hand
             */
            
            AddMoney(2);
            RestoreMovementPoints(2);
            TakeCard();
        }
        else // if my turn is over
        {
            if (_teamCt)
            {
                photonView.RPC("IncreaseRound", RpcTarget.All);
            }

            if (_planting)
            {
                if (_timeToPlant < 1) // bomb planted now
                {
                    SetBomb();
                }
                else if (_bomber.GetComponent<TapCharacter>().GetMovementPoints() > 1) // bomb not planted yet
                {
                    _timeToPlant--;
                    Debug.Log("Current rounds to plant: " + _timeToPlant);
                }
                else
                {
                    _timeToPlant = TimeForPlant; // figure moved/shot, planting is over
                }
            }
        }

        _myTurn = !_myTurn;
        canvasEndTurnButton.SetActive(_myTurn);
    }

    // plant the bomb
    private void SetBomb()
    {
        Debug.Log("BOMB HAS BEEN PLANTED!");
        _planting = false;

        var bomb = _bomber.GetComponent<TapCharacter>()._bomb; // bomb on figure
        _bomber.GetComponent<TapCharacter>().SetIsBomber(false); // bomber is not more bomber
        Destroy(bomb); // remove bomb from figure

        _photonView.RPC("CreateBomb", RpcTarget.All);
    }

    // create bomb on bombsite
    // #sending to each player
    [PunRPC]
    void CreateBomb()
    {
        // create bomb on bombsite
        var bomb = Instantiate(prefabBomb);
        var posBomb = _bombSite;
        bomb.transform.position = new Vector3(posBomb.x, posBomb.y, -0.21f);
        bomb.transform.localScale = new Vector3(0.13f, 0.13f, 0.13f);

        _bombPlanted = true;

        // create UI text "Bomb %roundsToExplode%"
        textBombRound = Instantiate(prefabTextBombRound, GameObject.Find("/Canvas").transform);
    }

    public GameObject GetBombPrefab()
    {
        return prefabBomb;
    }
    
    // increase round counter
    // #sending to each player
    [PunRPC]
    void IncreaseRound()
    {
        _round++;
        textRound.GetComponent<Text>().text = "Round: " + _round;

        if (_bombPlanted)
        {
            if (_defusing)
            {
                CheckForBombDefuse();
            }
            if (_gameIsOver)
                return;
            
            CheckForBombExplode();
        }
    }

    // check if bomb has been exploded already
    private void CheckForBombExplode()
    {
        _timeToExplode--;
        textBombRound.GetComponent<Text>().text = "Bomb: " + _timeToExplode;

        // if it is time to explode the bomb
        if (_timeToExplode < 1)
        {
            ShowGameOver(false);
            _gameIsOver = true;
        }
    }

    // check if bomb has been defused already
    private void CheckForBombDefuse()
    {
        _timeToDefuse--;
        
        if(_teamCt)
            textDefuseRound.GetComponent<Text>().text = "Defuse: " + _timeToDefuse;

        // if bomb should be defused now
        if (_timeToDefuse < 1)
        {
            ShowGameOver(true);
            _gameIsOver = true;
        }
    }

    private void ShowGameOver(bool ctIsWinner)
    {
        // game over (bomb exploded) ---> show menu
        GameObject menuGameOver = Instantiate(gameOverMenu);
        menuGameOver.name = "CanvasMenu";

        GameObject textWinnerObj = GameObject.Find("/CanvasMenu/TextWinner");
        var textWinner = textWinnerObj.GetComponent<Text>();

        // if we played on CT side
        if (_teamCt)
        {
            if (ctIsWinner) // if CT won
            {
                textWinner.text = "Your win!";
                textWinner.color = Color.green;
            }
            else // if T won
            {
                textWinner.text = "You lost!";
                textWinner.color = Color.red;
            }
            
        }
        else
        {
            if (!ctIsWinner) // if T won
            {
                textWinner.text = "Your win!";
                textWinner.color = Color.green;
            }
            else // if CT won
            {
                textWinner.text = "You lost!";
                textWinner.color = Color.red;
            }
        }
    }

    // restore all movements points on all figures at start of round
    private void RestoreMovementPoints(int toSetMp)
    {
        foreach (var f in ourFigures)
        {
            f.GetComponent<TapCharacter>().SetMovementPoints(toSetMp);
        }
    }

    public bool GetTeam()
    {
        return _teamCt;
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

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

            if (Math.Abs(xPos - x) + Math.Abs(yPos - y) <= r) // main condition of figure's vision 
            {
                c.GetComponent<GameField>().SetVisible(true);
            }
        }
    }

    // set all game field cells to not visible
    private void ResetFogOfWar()
    {
        foreach (GameObject c in cellsGameField)
        {
            c.GetComponent<GameField>().SetVisible(false);
        }
    }

    // @return cell by coordinates
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

    // give card to hand
    private void TakeCard()
    {
        // if we have chosen object now
        if (_chosenObj.transform.childCount != 0)
        {
            Transform children = _chosenObj.transform.GetChild(0);
            if (children.GetComponent<TapCard>())
            {
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

    // give %cardToTake% card to hand
    private void TakeCard(GameObject cardToTake)
    {
        // if we have chosen object now
        if (_chosenObj.transform.childCount != 0)
        {
            Transform children = _chosenObj.transform.GetChild(0);
            if (children.GetComponent<TapCard>())
            {
                children.GetComponent<TapCard>().ClearChoosenCard();
            }

            return;
        }

        int cardsOnHand = GetCardsOnHand();

        // overflow of cards on the hand (cardsOnHand = 5 cards)
        if (cardsOnHand >= 5)
        {
            return;
        }

        GameObject newCard = Instantiate(cardToTake);
        AddCardOnHand(newCard);
    }

    public void SetBomber(GameObject bomber)
    {
        _bomber = bomber;
    }

    // @return Vector3 of bombsite
    public Vector3 GetBombSite()
    {
        return _bombSite;
    }

    // switch visibility of "Plant Bomb" button
    public void ShowBombButton(bool toShow)
    {
        if (toShow) // if we moved on bombsite now
        {
            _plantBombButton = Instantiate(buttonBomb, canvasEndTurnButton.transform); // create a button "Plant Bomb"
            _plantBombButton.GetComponent<Button>().onClick.AddListener(PlantBomb);
        }
        else if (_plantBombButton) // if we moved out from a bombsite && button "Plant Bomb" exist
        {
            Destroy(_plantBombButton.gameObject);
            _plantBombButton = null;
        }
    }

    public void ShowDefuseButton(bool toShow)
    {
        if (toShow) // if we moved on bombsite now
        {
            _toDefuseButton = Instantiate(buttonDefuse, canvasEndTurnButton.transform); // create a button "Defuse bomb"
            _toDefuseButton.GetComponent<Button>().onClick.AddListener(DefuseBomb);
        }
        else if (_toDefuseButton) // if we moved out from a bombsite && button "Defuse bomb" exist
        {
            Destroy(_toDefuseButton.gameObject);
            _toDefuseButton = null;
        }
    }

    // onClick of button "Plant Bomb"
    private void PlantBomb()
    {
        _planting = true;
        Debug.Log("Planting!");
        ShowBombButton(false);
    }

    // onClick of button "Defuse Bomb"
    private void DefuseBomb()
    {
        _photonView.RPC("SetDefusingToAll", RpcTarget.All, true);
        Debug.Log("Defusing!");
        ShowDefuseButton(false);
        textDefuseRound = Instantiate(prefabTextDefuseRound, GameObject.Find("/Canvas").transform);
    }

    [PunRPC]
    private void SetDefusingToAll(bool defusing)
    {
        _defusing = defusing;
    }

    public void PickUpBomb(GameObject figureT)
    {
        SetBomber(figureT);
        ShowBombButton(false);
        Instantiate(prefabBombOnFigure, figureT.transform).name = "Bomb";
        SetIsBombDropped(false);
    }

    public void SetDroppedBomb(GameObject droppedBomb)
    {
        _droppedBomb = droppedBomb;
    }

    public GameObject GetDroppedBomb()
    {
        return _droppedBomb;
    }

    public void DestroyDroppedBomb()
    {
        Destroy(_droppedBomb);
    }

    public void SetIsBombDropped(bool dropped)
    {
        _bombDropped = dropped;
    }

    public bool GetIsBombDropped()
    {
        return _bombDropped;
    }

    public void SetPosDroppedBomb(Vector3 posDroppedBomb)
    {
        _posDroppedBomb = posDroppedBomb;
    }

    public Vector3 GetPosDroppedBomb()
    {
        return _posDroppedBomb;
    }

    public void ResetPlantingBomb()
    {
        _bomber = null;
        _planting = false;
        _bombPlanted = false;
        _timeToPlant = TimeForPlant;
        _timeToDefuse = TimeForDefuse;
        _timeToExplode = TimeForExplode;
    }

    public bool IsSpawnCell(float x, float y)
    {
        foreach (var c in _cellsSpawn)
        {
            var pos = c.transform.position;
            if (Math.Abs(x - pos.x) < 0.1f && Math.Abs(y - pos.y) < 0.1f)
            {
                return true;
            } 
        }

        return false;
    }
}