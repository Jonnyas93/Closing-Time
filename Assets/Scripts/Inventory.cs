using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Inventory : MonoBehaviour
{
    List<Item> inventory;
    float xRotate;
    float yRotate;
    Vector3 rayPos;
    Vector3 rayRotate;
    private CharacterController controller;
    private StarterAssetsInputs _input;
    private PlayerInput _playerInput;
    RaycastHit hitInfo;
    [SerializeField] GameObject cameraRoot;
    [SerializeField] GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        inventory = new List<Item>();
        rayPos = new Vector3();
        rayRotate = new Vector3();
        controller = GetComponent<CharacterController>();
        _input = GetComponent<StarterAssetsInputs>();
        _playerInput = GetComponent<PlayerInput>();
    }

    // Update is called once per frame
    void Update()
    {
        rayPos.Set(player.transform.position.x, rayPos.y + cameraRoot.transform.position.y, player.transform.position.z);
        rayRotate.Set(player.transform.rotation.x, cameraRoot.transform.rotation.y, player.transform.rotation.z);
        //if(_playerInput.)
        //{
        //    if(Physics.Raycast(rayPos,rayRotate,out hitInfo))
        //    {
        //        if(hitInfo.collider.tag == "Item")
        //        {
        //            Debug.Log("Pick up item");
        //        }
        //    }
        //}
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
