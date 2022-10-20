using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    GameObject itemGameObject;
    public string itemName;

    // Start is called before the first frame update
    void Start()
    {
        itemGameObject = gameObject;
        if (itemName == null)
        {
            itemName = "untitled Item";
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
