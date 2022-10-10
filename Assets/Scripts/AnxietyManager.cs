using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnxietyManager : MonoBehaviour
{
    public float staffAnxietyMult = 1f; //The multiplier for how much anxiety the player gains whilst in a staff anxiety zone
    public float customerAnxietyMult = 1f; //The multiplier for how much anxiety the player gains whilst in a customer anxiety zone
    public float deliAnxietyMult = 1f; //The multiplier for how much anxiety the player gains whilst in a deli anxiety zone
    
    public int anxIncreaseRate = 1; //How many seconds between ticks up of anxiety
    public int anxDecreaseRate = 1; //how many seconds between ticks down of anxiety

    float anxietyMultiplier; //internal variable that holds the current zone's anxiety multiplier
    bool inAnxietyZone; //true/false value that determines if the player is in an anxiety zone or not
    
    int counter; //an integer value used as a timer
    public float AnxietyLevel { get; set;} //The level of anxiety the player has

    // Start is called before the first frame update
    void Start()
    {
        //setting base values
        AnxietyLevel = 0;
        inAnxietyZone = false;
        anxietyMultiplier = 1;
        counter = 0;
    }

    // FixedUpdate is called once per frame at 50fps
    void FixedUpdate()
    {
        if (inAnxietyZone) //checks for if the user is in an anxiety zone
        {
            if(counter >= (50 * anxIncreaseRate)) //waits for the number of seconds determined by AnxIncreaseRate
            {
                if(AnxietyLevel >= 100) //checks if the number is 100 or more, if not increments
                {
                    AnxietyLevel = 100; //rounds the number nicely at 100
                }
                else
                {
                    AnxietyLevel += anxietyMultiplier; //add anxiety based on anxiety multiplier
                }
                if (AnxietyLevel < 100)
                {
                    Debug.Log(AnxietyLevel.ToString());
                }
                counter = 0;// resets timer
            }
            else
            {
                counter++;//increment timer up
            }
        }
        else if (!inAnxietyZone) //checks for if the user is not in an anxiety zone
        {
            if (counter >= (50*anxDecreaseRate)) //waits for the number of seconds determined by AnxDecreaseRate
            {
                if (AnxietyLevel <= 0) //checks if the number is 0 or less, if not decrements
                {
                    AnxietyLevel = 0; //rounds the number nicely at 0
                }
                else
                {
                    AnxietyLevel--; //decrements anxiety by 1
                }
                if (AnxietyLevel > 0)
                {
                    Debug.Log(AnxietyLevel.ToString());
                }
                counter = 0;//resets timer
            }
            else
            {
                counter++;//increment timer up
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        counter = 0;//resets timer
        if(other.tag == "StaffAZ")
        {
            anxietyMultiplier = staffAnxietyMult;//sets multiplier to the staff anxiety zone value
            inAnxietyZone = true;//sets the player to be in the anxiety zone
        }
        if (other.tag ==  "CustomerAZ")
        {
            anxietyMultiplier = customerAnxietyMult;//sets multiplier to the customer anxiety zone value
            inAnxietyZone = true;
        }
        if (other.tag == "DeliAZ")
        {
            anxietyMultiplier = deliAnxietyMult;//sets multiplier to the deli anxiety zone value
            inAnxietyZone = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        counter = 0;
        if (other.tag == "StaffAZ"|| other.tag == "CustomerAZ"|| other.tag == "DeliAZ")
        {
            inAnxietyZone = false;//sets the player to not be in the anxiety zone
        }
    }
}
