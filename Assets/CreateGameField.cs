using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateGameField : MonoBehaviour
{
    public GameObject cellPrefab;
    public GameObject bombsite;
    public GameObject gameManager;

    private GameObject _field;
    
    // Start is called before the first frame update
    void Start()
    {
        InitVariables();
        CreateGameCells();
    }

    private void InitVariables()
    {
        _field = GameObject.Find("GameField");
        gameManager = GameObject.Find("GameManager");
    }

    // Creating a game field with cells
    private void CreateGameCells()
    {
        for (float y = 0; y < 4.7f; y = y + 0.62f)
        {
            for (float x = 0; x < 4f; x = x + 0.62f)
            {
                GameObject newCell = Instantiate(cellPrefab, new Vector3(x, y, 0), Quaternion.identity);
                newCell.transform.parent = _field.transform;
                newCell.GetComponent<GameField>().SetPosX(x);
                newCell.GetComponent<GameField>().SetPosY(y);

                gameManager.GetComponent<GameManager>().AddCellToGameFieldArray(newCell);
            }
        }

        Instantiate(bombsite);
    }
}
