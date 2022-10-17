using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    GameObject itemGameObject;

    // Start is called before the first frame update
    void Start()
    {
        itemGameObject = GetComponent<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        
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
