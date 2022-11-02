using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomerMover : MonoBehaviour
{
    Rigidbody cT;
    public float movementSpeed = 1f;
    public bool xMoving;
    // Start is called before the first frame update
    void Start()
    {
        cT = GetComponent<Rigidbody>();
        if (xMoving)
        {
            cT.AddForce(new Vector3(movementSpeed/2, 0, 0));
        }
        else
        {
            cT.AddForce(new Vector3(0, 0, movementSpeed/2));
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Barrier"))
        {
            MoveCustomer();
        }
        if(other.CompareTag("Player"))
        {
            movementSpeed *= -1;
            if (xMoving)
            {
                cT.AddForce(new Vector3(movementSpeed/2, 0, 0));
            }
            else
            {
                cT.AddForce(new Vector3(0, 0, movementSpeed/2));
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            movementSpeed *= -1;
            if (xMoving)
            {
                cT.AddForce(new Vector3(movementSpeed / 2, 0, 0));
            }
            else
            {
                cT.AddForce(new Vector3(0, 0, movementSpeed / 2));
            }
        }
    }

    public void MoveCustomer()
    {
        movementSpeed *= -1;
        if (xMoving)
        {
            cT.AddForce(new Vector3(movementSpeed, 0, 0));
        }
        else
        {
            cT.AddForce(new Vector3(0, 0, movementSpeed));
        }
    }


}
