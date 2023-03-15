using System;
using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

public class TapCharacter : MonoBehaviour, IPunObservable
{
    private PhotonView _photonView;
    
    public Sprite defaultSprite;
    public Sprite chosenSprite;

    private GameObject _gameManager;
    
    private GameObject _chosenObj;
    private GameObject _handObj;
    private GameObject _gameField;
    private GameObject _figures;
    public GameObject _bomb;
    
    private GameObject _squareWithMp;
    private Text _hpBar;
    private Text _textMp;
    
    public int hp;
    public int damage;
    public float radiusView;
    public int cost;

    private bool _teamCt;
    private bool _isChosen;
    private int _movementPoints;
    private float _increasingSize;
    private float _lenToField;
    private Color _greenClr;
    private List<GameObject> _cellsGameField;

    private bool _bomber;

    void Start()
    {
        InitVariables();
    }

    private void InitVariables()
    {
        _photonView = GetComponent<PhotonView>();
        _chosenObj = GameObject.Find("/ChoosenObj");
        _gameField = GameObject.Find("/GameField");
        _handObj = GameObject.Find("/Hand");
        _figures = GameObject.Find("/Figures");
        
        _hpBar = transform.Find("Canvas/TextHPBar").GetComponent<Text>();
        _squareWithMp = transform.Find("Canvas/Square").gameObject;
        _textMp = transform.Find("Canvas/Square/TextMP").GetComponent<Text>();
        
        gameObject.name = "Figure";
        transform.parent = _figures.transform;
        _gameManager = GameObject.Find("/GameManager");

        _cellsGameField = _gameManager.GetComponent<GameManager>().GetCells();

        _teamCt = _gameManager.GetComponent<GameManager>().GetTeam();
        _isChosen = false;
        _increasingSize = 1.2f;
        _lenToField = 0.62f;
        _greenClr = new Color(0f, 255f, 0f);
        radiusView *= 0.62f;
        _movementPoints = 0;

        CheckSquareWithMp();
        CheckVisibility();
    }

    public bool IsBomber()
    {
        return _bomber;
    }

    public void SetIsBomber(bool isBomber)
    {
        _bomber = isBomber;
    }

    private void CheckSquareWithMp()
    {
        if (!_photonView.IsMine)
        {
            _squareWithMp.SetActive(false);
        }
    }
    
    // Run RPC method of checking visibility of enemies
    public void CheckVisibility()
    {
        if (_photonView.IsMine)
        {
            _photonView.RPC("SetVisibleFigure", RpcTarget.Others, transform.position.x, transform.position.y);
        }
    }

    [PunRPC]
    void OnCreateFigureVisibility()
    {
        CheckVisibility();
    }
    
    // Setting visibility of figure
    [PunRPC]
    void SetVisibleFigure(float xCell, float yCell)
    {
        // Step #1: receive pos of CT and check if it cell is visible
        _gameManager = GameObject.Find("/GameManager");
        GameObject currentCell = _gameManager.GetComponent<GameManager>().GetCellByVector2(xCell, yCell);

        if (!currentCell)
            return;
        
        var visibleOfCell = currentCell.GetComponent<GameField>().visible;
        
        TurnVisibility(gameObject, visibleOfCell);
        
        // Step #2: send 'x' and 'y' of all visible _figures T to CT
        UpdateVisibleForAllFigures();

    }

    // Changing visibility of %figureToTurn% in %visibility%:
    //      SpriteRenderer + Canvas (in children)
    private void TurnVisibility(GameObject figureToTurn, bool visibility)
    {
        figureToTurn.GetComponent<SpriteRenderer>().enabled = visibility;
        figureToTurn.GetComponentInChildren<Canvas>().enabled = visibility;
    }
    
    // Update visibility of all enemies for each ourFigure
    public void UpdateVisibleForAllFigures()
    {
        /*
         * There we have '(x,y)' and 'r' of CT figure and we would like to set visibilities of all T _figures for CT player
         */
        
        List<GameObject> ourFigures = _gameManager.GetComponent<GameManager>().GetOurFigures(); // T _figures
        
        foreach (var f in ourFigures) // for each our figure
        {
            if(!_photonView)
                _photonView = GetComponent<PhotonView>();
            
            // check distance from our figure to each enemy
            _photonView.RPC("SetVisibleEnemy", RpcTarget.Others, f.transform.position.x, f.transform.position.y);
        }
    }
    
    [PunRPC]
    void SetVisibleEnemy(float xT, float yT)
    {
        // Step #3: CT receive ==> send ray to (x,y) and turn the visibility of collided figure
        
        List<GameObject> ourFigures = _gameManager.GetComponent<GameManager>().GetOurFigures(); // CT _figures
        bool finalVisible = false;
        foreach (var f in ourFigures) // for each CT figure
        {
            var position = f.transform.position;
            bool mainCondition = Math.Abs(position.x - xT) + Math.Abs(position.y - yT) 
                                 <= f.GetComponent<TapCharacter>().radiusView;
            if (mainCondition)
                finalVisible = true;
        }
        
        Vector3 rayPos = new Vector3(xT, yT, -5f);
        
        // if we hit in figure by Ray ---> turning visible
        Ray ray = new Ray(rayPos, transform.forward);
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit)){
            if(hit.collider.gameObject.GetComponent<TapCharacter>()) 
            {
                TurnVisibility(hit.collider.gameObject, finalVisible);
            }
        }
        
        
    }
    
    // закрашивание 4-ех клеток вокруг выбранной фигуры
    void PaintCloseCells(bool isPainted){
        var position = transform.position;
        float fX = position.x;
        float fY = position.y;
        float fZ = position.z;

        Vector3 rayPosUp = new Vector3(fX, fY + _lenToField, fZ - 2f);
        Vector3 rayPosRight = new Vector3(fX + _lenToField, fY, fZ - 2f);
        Vector3 rayPosDown = new Vector3(fX, fY - _lenToField, fZ - 2f);
        Vector3 rayPosLeft = new Vector3(fX - _lenToField, fY, fZ - 2f);

        Ray rayUp = new Ray(rayPosUp, transform.forward);
        RaycastHit hitUp;
        if(Physics.Raycast(rayUp, out hitUp)){
            if(hitUp.collider.gameObject.GetComponent<GameField>()){
                if(isPainted){
                    hitUp.collider.gameObject.GetComponent<SpriteRenderer>().color = new Color(255f, 255f, 255f);
                }
                else{
                    hitUp.collider.gameObject.GetComponent<SpriteRenderer>().color = _greenClr;
                }
            }
        }

        Ray rayRight = new Ray(rayPosRight, transform.forward);
        RaycastHit hitRight;
        if(Physics.Raycast(rayRight, out hitRight)){
            if(hitRight.collider.gameObject.GetComponent<GameField>()){
                if(isPainted){
                    hitRight.collider.gameObject.GetComponent<SpriteRenderer>().color = new Color(255f, 255f, 255f);
                }
                else{
                    hitRight.collider.gameObject.GetComponent<SpriteRenderer>().color = _greenClr;
                }
            }
        }
        
        Ray rayDown = new Ray(rayPosDown, transform.forward);
        RaycastHit hitDown;
        if(Physics.Raycast(rayDown, out hitDown)){
            if(hitDown.collider.gameObject.GetComponent<GameField>()){
                if(isPainted){
                    hitDown.collider.gameObject.GetComponent<SpriteRenderer>().color = new Color(255f, 255f, 255f);
                }
                else{
                    hitDown.collider.gameObject.GetComponent<SpriteRenderer>().color = _greenClr;
                }
            }
        }

        Ray rayLeft = new Ray(rayPosLeft, transform.forward);
        RaycastHit hitLeft;
        if(Physics.Raycast(rayLeft, out hitLeft)){
            if(hitLeft.collider.gameObject.GetComponent<GameField>()){
                if(isPainted){
                    hitLeft.collider.gameObject.GetComponent<SpriteRenderer>().color = new Color(255f, 255f, 255f);
                }
                else{
                    hitLeft.collider.gameObject.GetComponent<SpriteRenderer>().color = _greenClr;
                }
            }
        }
    }

    
    // visualizing radius view of figure
    void PaintRadiusView(bool isPainted)
    {
        foreach (var c in _cellsGameField) // checking every game cell
        {
            if (Math.Abs(c.transform.position.x - transform.position.x) +
                Math.Abs(c.transform.position.y - transform.position.y) <=
                radiusView) // main condition of visibility
            {
                if (isPainted)
                    c.transform.GetComponent<SpriteRenderer>().color = new Color(255f, 255f, 255f); // paint cell in white
                else
                    c.transform.GetComponent<SpriteRenderer>().color = Color.cyan;                  // paint cell in cyan
            }
        }
    }
    
    // перемещение фигуры из текущего выбранного объекта
    public void DeleteChoose(){
        PaintRadiusView(true);
        PaintCloseCells(true);
        transform.parent = _figures.transform;
        transform.localScale /= _increasingSize;
        GetComponent<SpriteRenderer>().sprite = defaultSprite;
        _isChosen = false;
    }

    // перемещение стороннего текущего объекта
    public void ClearChoosenCard(){
        Transform children = _chosenObj.transform.GetChild(0);

        // if we have chosen card right now
        if(children.GetComponent<TapCard>()){
            children.parent = _handObj.transform;
            children.localScale /= 1.15f;
            children.position = new Vector3(children.position.x, children.position.y, children.position.z + 1);
            children.GetComponent<TapCard>().isChoosen = false;
            children.GetComponent<TapCard>().HighlightSpawnLine(false);
        }

        // если выбранный объект - фигура
        else if(children.GetComponent<TapCharacter>()){
            children.GetComponent<TapCharacter>().DeleteChoose();
        }
    }

    [PunRPC]
    void TakeDamage(int _damage)
    {
        hp -= _damage;
        _hpBar.text = hp.ToString();
    }

    // Remove all visibility
    [PunRPC]
    void RunRemoveVisibilityOfAllEnemies()
    {
        List<GameObject> l1 = _gameManager.GetComponent<GameManager>().GetOurFigures();
        foreach (var figure in l1)
        {
            _photonView.RPC("GetGameObjectFigureByCoords", RpcTarget.Others ,figure.transform.position.x, figure.transform.position.y);
        }
    }

    // Check if we have figure in (xT,yT)
    // if we found figure ---> turning off visibility
    [PunRPC]
    void GetGameObjectFigureByCoords(float xT, float yT)
    {
        Vector3 rayPos = new Vector3(xT, yT, -5f);
        
        Ray ray = new Ray(rayPos, transform.forward);
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit)){
            if(hit.collider.gameObject.GetComponent<TapCharacter>())
            {
                TurnVisibility(hit.collider.gameObject, false);
            }
        }
    }

    // Remove object
    void RemoveFigure()
    {
        _photonView.RPC("SendDeleteObj", RpcTarget.Others);
    }

    // Removing figure
    [PunRPC]
    void SendDeleteObj()
    {
        if (!_teamCt && _bomber)
        {
            _photonView.RPC("DropBomb", RpcTarget.All);
        }
        
        _gameManager.GetComponent<GameManager>().RemoveFigureFromList(gameObject);
        
        var position = transform.position;
        gameObject.transform.position = new Vector3(position.x, position.y, 5f);
        
        _photonView.RPC("RunRemoveVisibilityOfAllEnemies", RpcTarget.Others);
        
        _gameManager.GetComponent<GameManager>().UpdateFogOfWarForAll();
        UpdateVisibleForAllFigures();
        
        
        PhotonNetwork.Destroy(gameObject);
    }

    // Dropping the bomb on the game field
    [PunRPC]
    void DropBomb()
    {
        var droppedBomb = Instantiate(_gameManager.GetComponent<GameManager>().GetBombPrefab());
        droppedBomb.transform.position = gameObject.transform.position;
        droppedBomb.transform.rotation = Quaternion.Euler(0f, 0f, -45f);
        droppedBomb.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        _gameManager.GetComponent<GameManager>().SetDroppedBomb(droppedBomb);
        
        if (!_teamCt)
        {
            _gameManager.GetComponent<GameManager>().SetIsBombDropped(true);
            _gameManager.GetComponent<GameManager>().SetPosDroppedBomb(droppedBomb.transform.position);
            _gameManager.GetComponent<GameManager>().ShowBombButton(false);// disable button "Plant Bomb"
            _gameManager.GetComponent<GameManager>().ResetPlantingBomb();
        }
    }
    
    // Giving bomb from card to figure
    private void TakeBomb(BombCard bombCard)
    {
        _bomber = true;
        bombCard.GiveBomb(gameObject);
        _gameManager.GetComponent<GameManager>().SetBomber(gameObject);
        _bomb = GameObject.Find("Bomb");
    }
    
    // клик на фигуру
    void OnMouseDown(){
        
        // if it's not our turn
        if (!_gameManager.GetComponent<GameManager>().IsMyTurn())
        {
            return;
        }

        // if we clicked on enemy
        if(!_photonView.IsMine)
        {
            GameObject ourFigure = _chosenObj.GetComponent<ChoosenCardScript>().GetChoosenObj();
            // if we have in hand chosen figure
            if(ourFigure)
            {
                var currentPoints = ourFigure.GetComponent<TapCharacter>().GetMovementPoints();
                if (currentPoints > 1)
                {
                    if (Math.Abs(transform.position.x - ourFigure.transform.position.x) +
                        Math.Abs(transform.position.y - ourFigure.transform.position.y) <=
                        ourFigure.GetComponent<TapCharacter>().radiusView) // if enemy in our radiusView
                    {
                        int localDmg = _chosenObj.GetComponent<ChoosenCardScript>().GetChoosenObj().GetComponent<TapCharacter>().damage;
                        _photonView.RPC("TakeDamage", RpcTarget.All, localDmg);
                        ourFigure.GetComponent<TapCharacter>().SetMovementPoints(0);
                    }
                }
                
                if (hp <= 0) // Enemy is dead
                {
                    RemoveFigure();
                }
            }
            else{
                return;
            }
            _chosenObj.GetComponent<ChoosenCardScript>().GetChoosenObj().GetComponent<TapCharacter>().ClearChoosenCard();
            return;
        }

        
        
        // если есть выбранный объект и он не равен текущему
        if(_chosenObj.transform.childCount > 0 && transform.parent != _chosenObj.transform)
        {
            var bombCard = _chosenObj.GetComponentInChildren<BombCard>();
            ClearChoosenCard();
            if (bombCard) // if we chose bomb card and clicked on figure
            {
                var pos = transform.position;
                if (!_teamCt && _gameManager.GetComponent<GameManager>().IsSpawnCell(pos.x, pos.y)) // if we playing on T side && this cell is spawn cell
                {
                    TakeBomb(bombCard);
                    return;
                }
            }
            
        }
        // если фигура выбрана
        if(_isChosen){
            DeleteChoose();
            PaintRadiusView(true);
            PaintCloseCells(true);
        }
        // если фигура ещё не выбрана
        else{
            transform.parent = _chosenObj.transform;
            transform.localScale *= _increasingSize;
            GetComponent<SpriteRenderer>().sprite = chosenSprite;
            _isChosen = true;
            PaintRadiusView(false);
            PaintCloseCells(false);
        }
    }
    
    // move figure
    public void MoveFigure(Vector3 newPos, Color cellColor){
        newPos.z -= 0.3f;
        PaintRadiusView(true);
        PaintCloseCells(true);
        if (_movementPoints > 0) // if we have at least 1 MP
        {
            if (cellColor == new Color(0f, 255f, 0f)) // if color of cell is 'green'
            {
                GetComponent<BoxCollider>().enabled = false;
                transform.position = newPos;                    // moving figure
                GetComponent<BoxCollider>().enabled = true;
                _gameManager.GetComponent<GameManager>().UpdateFogOfWarForAll();
                
                SetMovementPoints(_movementPoints - 1);
                CheckVisibility();
                CheckPickUpBomb();
                CheckForEnterBombsite();
            }
        }

        DeleteChoose();
    }

    // check if we entered the bombsite on T side
    private void CheckForEnterBombsite()
    {
        if (_bomber) // if it figure has a bomb
        {
            var bombPos = _gameManager.GetComponent<GameManager>().GetBombSite();
                    
            if (Math.Abs(transform.position.x - bombPos.x) < 0.1f &&
                Math.Abs(transform.position.y - bombPos.y) < 0.1f) // if bomber staying on bombsite
            {
                _gameManager.GetComponent<GameManager>().ShowBombButton(true); // show button "Plant Bomb"
            }
            else
            {
                _gameManager.GetComponent<GameManager>().ShowBombButton(false);// disable button "Plant Bomb"
            }
        }
        else if (_teamCt && _gameManager.GetComponent<GameManager>().GetBombPlanted()) // if we CT and bomb planted
        {
            var bombPos = _gameManager.GetComponent<GameManager>().GetBombSite();
                    
            if (Math.Abs(transform.position.x - bombPos.x) < 0.1f &&
                Math.Abs(transform.position.y - bombPos.y) < 0.1f) // if CT stay on bombsite
            {
                _gameManager.GetComponent<GameManager>().ShowDefuseButton(true); // show button "Defuse Bomb"
            }
            else
            {
                _gameManager.GetComponent<GameManager>().ShowDefuseButton(false);// disable button "Defuse Bomb"
            }
        }
    }

    // check if we pickup the bomb on T side
    private void CheckPickUpBomb()
    {
        if (_gameManager.GetComponent<GameManager>().GetIsBombDropped())
        {
            Vector3 posDroppedBomb = _gameManager.GetComponent<GameManager>().GetPosDroppedBomb();
            if (Math.Abs(transform.position.x - posDroppedBomb.x) < 0.1f &&
                Math.Abs(transform.position.y - posDroppedBomb.y) < 0.1f)
            {
                _bomber = true;
                _gameManager.GetComponent<GameManager>().PickUpBomb(gameObject);
                _bomb = GameObject.Find("Bomb");
                _photonView.RPC("DestroyBombOnField", RpcTarget.All);
            }
        }
    }

    [PunRPC]
    void DestroyBombOnField()
    {
        _gameManager.GetComponent<GameManager>().DestroyDroppedBomb();
    }
    
    private void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }

    private void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(hp);
        }
        else
        {
            hp = (int)stream.ReceiveNext();
        }
    }

    public int GetMovementPoints()
    {
        return _movementPoints;
    }

    public void SetMovementPoints(int _movementPoints)
    {
        this._movementPoints = _movementPoints;
        _textMp.text = _movementPoints.ToString();
    }
}
