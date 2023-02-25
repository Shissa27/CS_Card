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

    private GameObject gameManager;
    
    private GameObject choosenObj;
    private GameObject handObj;
    private GameObject gameField;
    private GameObject figures;
    
    private GameObject squareWithMP;
    private Text hpBar;
    private Text textMP;
    
    public int hp;
    public int damage;
    public float radiusView;
    public int cost;
    
    private bool isChoosen;
    private int movementPoints;
    float increasingSize;
    float lenToField;
    private Color greenClr;
    private List<GameObject> cellsGameFild;
    
    void Start()
    {
        InitVariables();
    }

    private void InitVariables()
    {
        _photonView = GetComponent<PhotonView>();
        choosenObj = GameObject.Find("/ChoosenObj");
        gameField = GameObject.Find("/GameField");
        handObj = GameObject.Find("/Hand");
        figures = GameObject.Find("/Figures");
        
        hpBar = transform.Find("Canvas/TextHPBar").GetComponent<Text>();
        squareWithMP = transform.Find("Canvas/Square").gameObject;
        textMP = transform.Find("Canvas/Square/TextMP").GetComponent<Text>();
        
        gameObject.name = "Figure";
        transform.parent = figures.transform;
        gameManager = GameObject.Find("/GameManager");

        cellsGameFild = gameManager.GetComponent<GameManager>().GetCells();

        isChoosen = false;
        increasingSize = 1.2f;
        lenToField = 0.62f;
        greenClr = new Color(0f, 255f, 0f);
        radiusView *= 0.62f;
        movementPoints = 0;

        CheckSquareWithMP();
        CheckVisibility();
    }

    private void CheckSquareWithMP()
    {
        if (!_photonView.IsMine)
        {
            squareWithMP.SetActive(false);
        }
    }
    
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
    
    [PunRPC]
    void SetVisibleFigure(float xCell, float yCell)
    {
        // Step #1: receive pos of CT and check if it cell is visible
        
        gameManager = GameObject.Find("/GameManager");
        GameObject currentCell = gameManager.GetComponent<GameManager>().GetCellByVector2(xCell, yCell);
        //Debug.Log("Current cell: " + currentCell.transform.position.x + " " + currentCell.transform.position.y);

        if (!currentCell)
            return;
        
        var visibleOfCell = currentCell.GetComponent<GameField>().visible;
        if (visibleOfCell)
        {
            //Debug.Log("this cell is visible!");
        }
        else
        {
            //Debug.Log("this cell isn't visible!");
        }


        TurnVisibility(gameObject, visibleOfCell);
        

        // Step #2: send 'x' and 'y' of all visible figures T to CT
        
        UpdateVisibleForAllFigures();

    }

    private void TurnVisibility(GameObject figureToTurn, bool visibility)
    {
        figureToTurn.GetComponent<SpriteRenderer>().enabled = visibility;
        figureToTurn.GetComponentInChildren<Canvas>().enabled = visibility;
        //figureToTurn.GetComponent<BoxCollider>().enabled = visibility;
        
    }
    
    public void UpdateVisibleForAllFigures()
    {
        /*
         * There we have '(x,y)' and 'r' of CT figure and we would like to set visibilities of all T figures for CT player
         */
        
        List<GameObject> ourFigures = gameManager.GetComponent<GameManager>().GetOurFigures(); // T figures
        
        foreach (var f in ourFigures)
        {
            if(!_photonView)
                _photonView = GetComponent<PhotonView>();
            _photonView.RPC("SetVisibleEnemy", RpcTarget.Others, f.transform.position.x, f.transform.position.y);
        }
    }
    
    [PunRPC]
    void SetVisibleEnemy(float xT, float yT)
    {
        // Step #3: CT receive ==> send ray to (x,y) and turn the visibility of collided figure
        
        List<GameObject> ourFigures = gameManager.GetComponent<GameManager>().GetOurFigures(); // CT figures
        bool finalVisible = false;
        foreach (var f in ourFigures)
        {
            bool mainCondition = Math.Abs(f.transform.position.x - xT) + Math.Abs(f.transform.position.y - yT) <= f.GetComponent<TapCharacter>().radiusView;
            if (mainCondition)
                finalVisible = true;
        }
        
        Vector3 rayPos = new Vector3(xT, yT, -5f);
        
        Ray ray = new Ray(rayPos, transform.forward);
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit)){
            if(hit.collider.gameObject.GetComponent<TapCharacter>())
            {
                //Debug.Log("Ray: " + xT + ";" + yT + " visibility: " + finalVisible);
                TurnVisibility(hit.collider.gameObject, finalVisible);
            }
        }
        
        
    }
    
    // закрашивание 4-ех клеток вокруг выбранной фигуры
    void PaintCloseCells(bool isPainted){

        float fX = transform.position.x;
        float fY = transform.position.y;
        float fZ = transform.position.z;

        Vector3 rayPosUP = new Vector3(fX, fY + lenToField, fZ - 2f);
        Vector3 rayPosRight = new Vector3(fX + lenToField, fY, fZ - 2f);
        Vector3 rayPosDown = new Vector3(fX, fY - lenToField, fZ - 2f);
        Vector3 rayPosLeft = new Vector3(fX - lenToField, fY, fZ - 2f);

        Ray rayUp = new Ray(rayPosUP, transform.forward);
        RaycastHit hitUp;
        if(Physics.Raycast(rayUp, out hitUp)){
            if(hitUp.collider.gameObject.GetComponent<GameField>()){
                if(isPainted){
                    hitUp.collider.gameObject.GetComponent<SpriteRenderer>().color = new Color(255f, 255f, 255f);
                }
                else{
                    hitUp.collider.gameObject.GetComponent<SpriteRenderer>().color = greenClr;
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
                    hitRight.collider.gameObject.GetComponent<SpriteRenderer>().color = greenClr;
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
                    hitDown.collider.gameObject.GetComponent<SpriteRenderer>().color = greenClr;
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
                    hitLeft.collider.gameObject.GetComponent<SpriteRenderer>().color = greenClr;
                }
            }
        }
    }

    void PaintRadiusView(bool isPainted)
    {
        foreach (var c in cellsGameFild)
        {
            if (Math.Abs(c.transform.position.x - transform.position.x) +
                Math.Abs(c.transform.position.y - transform.position.y) <=
                radiusView)
            {
                if (isPainted)
                    c.transform.GetComponent<SpriteRenderer>().color = new Color(255f, 255f, 255f);
                else
                    c.transform.GetComponent<SpriteRenderer>().color = Color.cyan;
            }
        }
    }
    
    // перемещение фигуры из текущего выбранного объекта
    public void DeleteChoose(){
        PaintRadiusView(true);
        PaintCloseCells(true);
        transform.parent = figures.transform;
        transform.localScale /= increasingSize;
        GetComponent<SpriteRenderer>().sprite = defaultSprite;
        isChoosen = false;
    }

    // перемещение стороннего текущего объекта
    public void ClearChoosenCard(){
        Transform children = choosenObj.transform.GetChild(0);

        // if we have chosen card right now
        if(children.GetComponent<TapCard>()){
            children.parent = handObj.transform;
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
        hpBar.text = hp.ToString();
    }

    [PunRPC]
    void RunRemoveVisibilityOfAllEnemies()
    {
        List<GameObject> l1 = gameManager.GetComponent<GameManager>().GetOurFigures();
        foreach (var figure in l1)
        {
            _photonView.RPC("GetGameObjectFigureByCoords", RpcTarget.Others ,figure.transform.position.x, figure.transform.position.y);
        }
    }

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

    // remove object
    void RemoveFigure()
    {
        _photonView.RPC("SendDeleteObj", RpcTarget.Others);
    }

    [PunRPC]
    void SendDeleteObj()
    {
        gameManager.GetComponent<GameManager>().RemoveFigureFromList(gameObject);
        List<GameObject> l1 = gameManager.GetComponent<GameManager>().GetOurFigures();
        gameObject.transform.position = new Vector3(transform.position.x, transform.position.y, 5f);
        
        _photonView.RPC("RunRemoveVisibilityOfAllEnemies", RpcTarget.Others);
        
        // if (l1.Count == 0)
        // {
        //     Debug.Log("ourFigures are empty!");
        //     gameManager.GetComponent<GameManager>().ResetFogOfWar();
        //     
        // }
        //
        gameManager.GetComponent<GameManager>().UpdateFogOfWarForAll();
        UpdateVisibleForAllFigures();
        
        
        PhotonNetwork.Destroy(gameObject);
    }

    
    // клик на фигуру
    void OnMouseDown(){
        
        // if it's not our turn
        if (!gameManager.GetComponent<GameManager>().IsMyTurn())
        {
            return;
        }

        // if we clicked on enemy
        if(!_photonView.IsMine)
        {
            GameObject ourFigure = choosenObj.GetComponent<ChoosenCardScript>().GetChoosenObj();
            // if we have in hand chosen figure
            if(ourFigure)
            {
                if (ourFigure.GetComponent<TapCharacter>().GetMovementPoints() > 0)
                {
                    if (Math.Abs(transform.position.x - ourFigure.transform.position.x) +
                        Math.Abs(transform.position.y - ourFigure.transform.position.y) <=
                        ourFigure.GetComponent<TapCharacter>().radiusView)
                    {
                        int localDmg = choosenObj.GetComponent<ChoosenCardScript>().GetChoosenObj().GetComponent<TapCharacter>().damage;
                        _photonView.RPC("TakeDamage", RpcTarget.All, localDmg);
                        //Debug.Log("SHOT! Remaining hp: " + hp);
                        ourFigure.GetComponent<TapCharacter>().SetMovementPoints(0);
                    }
                }
                
                if (hp <= 0)
                {
                    RemoveFigure();
                }
            }
            else{
                return;
            }
            choosenObj.GetComponent<ChoosenCardScript>().GetChoosenObj().GetComponent<TapCharacter>().ClearChoosenCard();
            return;
        }

        // если есть выбранный объект и он не равен текущему
        if(choosenObj.transform.childCount > 0 && transform.parent != choosenObj.transform)
        {
            ClearChoosenCard();
        }
        // если фигура выбрана
        if(isChoosen){
            DeleteChoose();
            PaintRadiusView(true);
            PaintCloseCells(true);
        }
        // если фигура ещё не выбрана
        else{
            transform.parent = choosenObj.transform;
            transform.localScale *= increasingSize;
            GetComponent<SpriteRenderer>().sprite = chosenSprite;
            isChoosen = true;
            PaintRadiusView(false);
            PaintCloseCells(false);
        }
    }
    
    // перемещение фигуры на клетку
    public void MoveFigure(Vector3 newPos, Color cell_color){
        newPos.z -= 0.3f;
        PaintRadiusView(true);
        PaintCloseCells(true);
        if (movementPoints > 0)
        {
            if (cell_color == new Color(0f, 255f, 0f))
            {
                GetComponent<BoxCollider>().enabled = false;
                transform.position = newPos;
                GetComponent<BoxCollider>().enabled = true;
                gameManager.GetComponent<GameManager>().UpdateFogOfWarForAll();
                
                SetMovementPoints(0);
                CheckVisibility();
            }
        }

        DeleteChoose();
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
        return movementPoints;
    }

    public void SetMovementPoints(int _movementPoints)
    {
        movementPoints = _movementPoints;
        textMP.text = _movementPoints.ToString();
        Debug.Log("* | Changed to " + _movementPoints);
    }
}
