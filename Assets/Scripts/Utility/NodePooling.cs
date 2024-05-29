using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodePooling : MonoBehaviour
{
    public GameObject symbolPrefabs;

    private List<GameObject> _pool = new List<GameObject>();

    public void Clear()
    {
        foreach (var item in _pool)
        {
            this.ReturntoPool(item);            
        }      
    }

    public GameObject GetNode(GameObject parent)
    {        
        GameObject gameObj = this._pool.Find((obj) => obj.activeSelf == false);     
        if (gameObj == null)
        {
            gameObj = Instantiate(symbolPrefabs);
            this._pool.Add(gameObj);
        }
        gameObj.transform.SetParent(parent.transform);
        gameObj.transform.localScale = Vector3.one;
        gameObj.SetActive(true);       
        return gameObj;
    }   

    public void ReturntoPool(GameObject gameObj)
    {
        gameObj.SetActive(false);
        gameObj.transform.localScale = Vector3.zero;        
    }
}
