using StarterAssets;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class Inventory : MonoBehaviour
{
    private List<string> itemTypes;
    private List<int> pickedNumbers;
    List<Item> inventory;
    List<Item> shoppingList;
    private StarterAssetsInputs _input;
    [SerializeField] GameObject cameraRoot;
    [SerializeField] GameObject player;
    [SerializeField] ItemSelector itemSelector;
    public int ShoppingListLength = 3;
    public int inventoryScore;

    // Start is called before the first frame update
    void Start()
    {
        itemTypes = new List<string>();
        pickedNumbers = new List<int>();
        shoppingList = new List<Item>();
        string[] lines = File.ReadAllLines("Assets/Scripts/itemList.csv");
        foreach (string s in lines)
        {
            string[] splitItem = s.Split(',');
            itemTypes.Add(splitItem[0]);
        }
        inventory = new List<Item>();
        _input = GetComponent<StarterAssetsInputs>();
    }

    // Update is called once per frame
    void Update()
    {
        Item i;
        if (_input.interact == true)
        {
            if (itemSelector.overItem)
            {
                i = itemSelector.ReturnItem();
                inventory.Add(i);

                i.PickUp();
            }
        }
        _input.interact = false;
    }

    public string[] GenerateShoppingList()
    {
        int selector;
        for (int i = 0; i < ShoppingListLength; i++)
        {
            selector = UnityEngine.Random.Range(0, itemTypes.Count());
            foreach (int number in pickedNumbers)
            {
                if(selector == number)
                {
                    while(selector == number)
                    {
                        selector = UnityEngine.Random.Range(0, itemTypes.Count());
                    }
                    pickedNumbers.Add(selector);
                    break;
                }
            }
        }
        return null;
    }

    public void PickUpItem(Item item)
    {
        item.PickUp();
    }

    public void DropItem(Item item)
    {
        item.Drop();
    }


}
