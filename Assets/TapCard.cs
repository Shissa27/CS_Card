using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Photon.Pun;
using UnityEngine;

public class TapCard : MonoBehaviour
{
    public bool isChoosen = false;
    private float increasingSize = 1.15f;
    private bool teamCT;
    
    public GameObject prefabSoldier;
    private GameObject handObj;
    private GameObject choosenObj;

    private GameObject gameManager;

    void Start()
    {
        handObj = GameObject.Find("Hand");
        choosenObj = GameObject.Find("ChoosenObj");
        transform.parent = handObj.transform;
        gameObject.name = "Card";
        
        gameManager = GameObject.Find("/GameManager");
        
        teamCT = gameManager.GetComponent<GameManager>().GetTeam();
    }
    
    // Снятие выделения с текущей карты
    public void ClearChoosenCard(){
        Transform children = choosenObj.transform.GetChild(0);

        // если выбранный объект - карта
        if(children.GetComponent<TapCard>()){
            children.parent = handObj.transform;
            children.localScale /= increasingSize;
            children.position = new Vector3(children.position.x, children.position.y, children.position.z + 1);
            children.GetComponent<TapCard>().isChoosen = false;
        }

        // если выбранный объект - фигура
        else if(children.GetComponent<TapCharacter>()){
            children.GetComponent<TapCharacter>().DeleteChoose();
        }
        HighlightSpawnLine(false);
    }

    // Выбор карты
    void OnMouseDown()
    {
        if(choosenObj.transform.childCount > 0 && transform.parent != choosenObj.transform){
            ClearChoosenCard();
        }
        if(isChoosen){
            transform.parent = handObj.transform;
            transform.localScale /= increasingSize;
            transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + 1);
            isChoosen = false;
        }
        else{
            transform.parent = choosenObj.transform;
            transform.localScale *= increasingSize;
            transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z - 1);
            
            isChoosen = true;
        }
        HighlightSpawnLine(isChoosen);
    }

    // highlight line of spawn figure by 'greening' cells
    public void HighlightSpawnLine(bool toHighlight)
    {
        float x;
        if (teamCT)
        {
            // CT-line: from (0; 0) to (0; 4.34) - 7 cells, left border
            x = 0f;
        }
        else
        {
            // T-line: from (3.72; 0) to (3.72; 4.34) - 7 cells, right border
            x = 3.72f;
        }
        
        // Run 7 rays to each cell on left/right border and paint him with green/white color
        for (float y = 0; y < 4.7f; y = y + 0.62f)
        {
            Vector3 aimCell = new Vector3(x, y, -1f);

            Ray ray = new Ray(aimCell, transform.forward);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                GameObject touchedCell = hit.collider.gameObject;
                
                if (touchedCell.GetComponent<GameField>()) // if ray touched cell 
                {
                    bool visibleOfCell = touchedCell.GetComponent<GameField>().visible;
                    
                    if (toHighlight)
                    {
                        touchedCell.GetComponent<SpriteRenderer>().color =
                            new Color(0f, 255f, 0f); // paint with green color
                    }
                    else
                    {
                        touchedCell.GetComponent<GameField>().SetVisible(visibleOfCell);
                    }
                }
            }
        }

    }
    
    // Создание фигуры на поле и удаление карты из руки
    public void CreateFigure(Vector3 posField)
    {
        int currentMoney = gameManager.GetComponent<GameManager>().GetMoney();
        int costFigure = prefabSoldier.GetComponent<TapCharacter>().cost;
        
        // if it's not our turn
        if (!gameManager.GetComponent<GameManager>().IsMyTurn())
        {
            return;
        }
        
        if (costFigure > currentMoney)
        {
            return;
        }
        
        // buying a figure
        gameManager.GetComponent<GameManager>().AddMoney(-costFigure);
        
        posField.z -= 0.1f;
        
        GameObject newplr = PhotonNetwork.Instantiate(prefabSoldier.name, posField, Quaternion.identity);
        gameManager.GetComponent<GameManager>().AddFigureToList(newplr);
        transform.parent = handObj.transform;
        transform.parent.GetComponent<HandScript>().CardsOnHand -= 1;
        
        gameManager.GetComponent<GameManager>().UpdateFogOfWar(posField.x, posField.y, newplr.GetComponent<TapCharacter>().radiusView * 0.62f);
        gameManager.GetComponent<GameManager>().RemoveCardFromHand(gameObject);
        Destroy(gameObject);
    }
}
   
