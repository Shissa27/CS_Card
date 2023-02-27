using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChoosenCardScript : MonoBehaviour
{
    // return chosen figure 
    public GameObject GetChoosenObj(){
        if(transform.childCount > 0){
            Transform children = transform.GetChild(0);
            return children.gameObject;
        }
        else{
            return null;
        }
        
    }
}
