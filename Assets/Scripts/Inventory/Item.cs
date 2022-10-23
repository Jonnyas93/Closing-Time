using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using UnityEngine;

public class Item : MonoBehaviour
{
    public Dictionary<string,int> itemTypes = new();
    GameObject itemGameObject;
    public string itemName;
    public int scoreAmount = 0;

    // Start is called before the first frame update
    void Start()
    {
        CreateDictionary();
        itemGameObject = gameObject;
        if (itemName == null)
        {
            Debug.LogError("Untitled Item Discovered");
        }
        if (scoreAmount <= 0)
        {
            scoreAmount = 100;
        }
    }

    public void CreateDictionary()
    {
        itemTypes = File.ReadLines("Assets/Scripts/itemList.csv").Select(line => line.Split(',')).ToDictionary(line => line[0], line => int.Parse(line[1]));
        foreach(var i in itemTypes)
        {
            if (i.Key.ToString() == itemName)
            {
                scoreAmount = i.Value;
                return;
            }
            Debug.LogError("Unknown item discovered called:" + itemName);
        }
    }

    public void PickUp()
    {
        itemGameObject.SetActive(false);
    }

    public void Drop()
    {
        itemGameObject.SetActive(true);
    }
}
