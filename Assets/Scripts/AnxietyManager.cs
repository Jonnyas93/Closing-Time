using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnxietyManager : MonoBehaviour
{
    public float staffAnxietyMult = 1f;
    public float customerAnxietyMult = 1f;
    public float deliAnxietyMult = 1f;
    public float anxietyLevel;

    float anxietyMultiplier;
    bool inAnxietyZone;
    int anxIncreaseRate;
    int anxDecreaseRate;
    int counter;

    
    // Start is called before the first frame update
    void Start()
    {
        anxietyLevel = 0;
        inAnxietyZone = false;
        anxIncreaseRate = 1;
        anxDecreaseRate = 1;
        anxietyMultiplier = 1;
        counter = 0;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        
        if (inAnxietyZone)
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
