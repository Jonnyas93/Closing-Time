using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnxietyManager : MonoBehaviour
{
    public float staffAnxietyMult = 1f; //The multiplier for how much anxiety the player gains whilst in a staff anxiety zone
    public float customerAnxietyMult = 1f; //The multiplier for how much anxiety the player gains whilst in a customer anxiety zone
    public float deliAnxietyMult = 1f; //The multiplier for how much anxiety the player gains whilst in a deli anxiety zone
    public float anxietyLevel; //The level of anxiety the player has
    public int anxIncreaseRate = 1; //How many seconds between ticks up of anxiety
    public int anxDecreaseRate = 1; //how many seconds between ticks down of anxiety

    float anxietyMultiplier; //internal variable that holds the current zone's anxiety multiplier
    bool inAnxietyZone; //true/false value that determines if the player is in an anxiety zone or not
    
    int counter; //an integer value used as a timer

    
    // Start is called before the first frame update
    void Start()
    {
        //setting base values
        anxietyLevel = 0;
        inAnxietyZone = false;
        anxietyMultiplier = 1;
        counter = 0;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (inAnxietyZone)//checks for if the user is in the zone
        {
            if(counter >= (50 * anxIncreaseRate))
            {
                if(anxietyLevel >= 100)
                {
                    anxietyLevel = 100;
                }
                else
                {
                    anxietyLevel += anxietyMultiplier;
                }
                Debug.Log(anxietyLevel.ToString());
                counter = 0;
            }
            else
            {
                counter++;
            }
        }
        else if (!inAnxietyZone)
        {
            if (counter >= (50*anxDecreaseRate))
            {
                if (anxietyLevel <= 0)
                {
                    anxietyLevel = 0;
                }
                else
                {
                    anxietyLevel--;
                }
                Debug.Log(anxietyLevel.ToString()); 
                counter = 0;
            }
            else
            {
                counter++;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        counter = 0;
        if(other.tag == "StaffAZ")
        {
            anxietyMultiplier = staffAnxietyMult;
        }
        if (other.tag ==  "CustomerAZ")
        {
            anxietyMultiplier = customerAnxietyMult;
        }
        if (other.tag == "DeliAZ")
        {
            anxietyMultiplier = deliAnxietyMult;
        }
        inAnxietyZone = true;
    }

    private void OnTriggerExit(Collider other)
    {
        counter = 0;
        inAnxietyZone = false;    
    }

}
