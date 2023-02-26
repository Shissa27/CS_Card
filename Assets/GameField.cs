using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameField : MonoBehaviour
{
    GameObject choosenObj;
    GameObject crnObj;
    public GameObject gameManager;
    
    public Sprite normal_field;
    public Sprite fog_field;
    
    public float xPos, yPos;
    public bool visible;

    void Start()
    {
        choosenObj = GameObject.Find("ChoosenObj");
        gameManager = GameObject.Find("/GameManager");
        
        visible = false;
    }

    
    void Update()
    {
        
    }

    // нажатие на клетку поля
    void OnMouseDown(){ // взаимодействие выбранного объекта с полем
        crnObj = choosenObj.GetComponent<ChoosenCardScript>().GetChoosenObj(); // текущий выбранный объект
        
        // текущий выбранный объект - не существует
        if(!crnObj){ 
            return;
        }
        if(crnObj.transform.position.x == transform.position.x && crnObj.transform.position.y == transform.position.y){
            return;
        }
        // текущий выбранный объект - карта
        if(crnObj.name == "Card"){
            if (crnObj.GetComponent<BombCard>())
            {
                crnObj.GetComponent<TapCard>().ClearChoosenCard();
                return;
            }

            if (gameObject.GetComponent<SpriteRenderer>().color == new Color(0f, 255f, 0f))
            {
                // создание фигуры на поле
                crnObj.GetComponent<TapCard>().ClearChoosenCard();
                crnObj.GetComponent<TapCard>().CreateFigure(transform.position);
            }
            return;
        }

        // текущий выбранный объект - фигура на поле
        if(crnObj.name == "Figure"){
            crnObj.GetComponent<TapCharacter>().MoveFigure(transform.position, transform.GetComponent<SpriteRenderer>().color);
            return;
        }

    }

    public float GetPosX() { return xPos; }

    public void SetPosX(float _x)
    {
        xPos = _x;
    }

    public float GetPosY() { return yPos; }

    public void SetPosY(float _y)
    {
        yPos = _y;
    }
    
    public bool IsVisible() { return visible; }

    public void SetVisible(bool _visible)
    {
        visible = _visible;
        if (visible)
        {
            GetComponent<SpriteRenderer>().sprite = normal_field;
            GetComponent<SpriteRenderer>().color = Color.white;
        }
        else
        {
            GetComponent<SpriteRenderer>().sprite = fog_field;
            GetComponent<SpriteRenderer>().color = Color.white;
        }
    }
}
