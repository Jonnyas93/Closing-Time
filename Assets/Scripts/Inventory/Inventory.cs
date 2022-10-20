using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Inventory : MonoBehaviour
{
    List<Item> inventory;
    private StarterAssetsInputs _input;
    [SerializeField] GameObject cameraRoot;
    [SerializeField] GameObject player;
    [SerializeField] ItemSelector itemSelector;

    // Start is called before the first frame update
    void Start()
    {
        inventory = new List<Item>();
        _input = GetComponent<StarterAssetsInputs>();
    }

    // Update is called once per frame
    void Update()
    {
        Item i;
        if (_input.interact == true)
        {
            i = itemSelector.ReturnItem();
            inventory.Add(i);
            i.PickUp();
        }
        _input.interact = false;
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
