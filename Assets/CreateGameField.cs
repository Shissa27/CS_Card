using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateGameField : MonoBehaviour
{
    public GameObject cellPrefab;

    public GameObject gameManager;
    // Start is called before the first frame update
    void Start()
    {
        GameObject field = GameObject.Find("GameField");
        GameObject gameManager = GameObject.Find("GameManager");
        
        for (float y = 0; y < 4.7f; y = y + 0.62f)
        {
            
            for (float x = 0; x < 4f; x = x + 0.62f)
            {
                
                GameObject newCell = Instantiate(cellPrefab, new Vector3(x, y, 0), Quaternion.identity);
                newCell.transform.parent = field.transform;
                newCell.GetComponent<GameField>().SetPosX(x);
                newCell.GetComponent<GameField>().SetPosY(y);

                gameManager.GetComponent<GameManager>().AddCellToGameFieldArray(newCell);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
