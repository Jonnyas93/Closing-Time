using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSelector : MonoBehaviour
{
    public bool overItem;
    Item itemSelected;
    // Start is called before the first frame update
    void Start()
    {
        overItem = false;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Item")
        {
            overItem = true;
            itemSelected = other.GetComponentInParent<Item>();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Item")
        {
            overItem = false;
        }
    }

    public Item ReturnItem()
    {
        if (overItem == true)
        {
            return itemSelected;
        }
        else
        {
            return null;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
