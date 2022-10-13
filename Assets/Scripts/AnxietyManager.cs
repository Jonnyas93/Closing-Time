using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

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
    public float AnxietyLevel { get; set; } //The level of anxiety the player has
    public float[] anxietyTiers = { 10, 20, 30, 80, 100 }; //the thresholds at which the anxiety effects start
    public bool[] conditionApplied = { false, false, false, false, false };//An array of booleans that keeps track of what conditions should be affecting the player
    [Tooltip("Lower is faster")] public float heartbeatRate = 1f;//Time between beats
    [Tooltip("The point at which the heartbeat wont get any faster")] public float heartbeatCutoff = 20f;
    public float fovDiv = 1f; //how heavily the fov adjustment factor gets divided

    bool anxietyMaxxed = false;
    int counterHeartSFX;
    float startFOV;
    float startIntensity;
    float vignetteGlide;
    float prevAnx;

    Camera playerCamera;
    GameSFX gSFX;
    public PostProcessProfile PPP;
    Vignette anxVignette;
    

    // Start is called before the first frame update
    void Start()
    {
        //setting base values
        AnxietyLevel = 0;
        inAnxietyZone = false;
        anxietyMultiplier = 1;
        counter = 0;
        playerCamera = FindObjectOfType<Camera>();
        gSFX = GetComponent<GameSFX>();
        counterHeartSFX = 0;
        startFOV = playerCamera.fieldOfView;
        anxVignette = PPP.GetSetting<Vignette>();
        startIntensity = anxVignette.intensity;
        vignetteGlide = 0;
        prevAnx = 0;
    }

    // FixedUpdate is called once per frame at 50fps
    void FixedUpdate()
    {
        if (inAnxietyZone) //checks for if the user is in an anxiety zone
        {
            if (counter >= (50 * anxIncreaseRate)) //waits for the number of seconds determined by AnxIncreaseRate
            {
                if (AnxietyLevel >= 100) //checks if the number is 100 or more, if not increments
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
            if (counter >= (50 * anxDecreaseRate)) //waits for the number of seconds determined by AnxDecreaseRate
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
        AnxietyCheck();
        AnxietyEffects();
    }

    private void OnTriggerEnter(Collider other)
    {
        counter = 0;//resets timer
        if (other.tag == "StaffAZ")
        {
            anxietyMultiplier = staffAnxietyMult;//sets multiplier to the staff anxiety zone value
            inAnxietyZone = true;//sets the player to be in the anxiety zone
        }
        if (other.tag == "CustomerAZ")
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

    void AnxietyCheck()
    {
        if (0 < AnxietyLevel && AnxietyLevel < anxietyTiers[0])
        {
            conditionApplied[0] = false;
            conditionApplied[1] = false;
            conditionApplied[2] = false;
            conditionApplied[3] = false;
            conditionApplied[4] = false;
        }
        if ((anxietyTiers[0]) < AnxietyLevel && AnxietyLevel <= anxietyTiers[1])
        {
            conditionApplied[0] = true;
            conditionApplied[1] = false;
            conditionApplied[2] = false;
            conditionApplied[3] = false;
            conditionApplied[4] = false;
        }
        if ((anxietyTiers[1]) < AnxietyLevel && AnxietyLevel <= anxietyTiers[2])
        {
            conditionApplied[0] = true;
            conditionApplied[1] = true;
            conditionApplied[2] = false;
            conditionApplied[3] = false;
            conditionApplied[4] = false;
        }
        if ((anxietyTiers[2]) < AnxietyLevel && AnxietyLevel <= anxietyTiers[3])
        {
            conditionApplied[0] = true;
            conditionApplied[1] = true;
            conditionApplied[2] = true;
            conditionApplied[3] = false;
            conditionApplied[4] = false;
        }
        if ((anxietyTiers[3]) < AnxietyLevel && AnxietyLevel <= anxietyTiers[4])
        {
            conditionApplied[0] = true;
            conditionApplied[1] = true;
            conditionApplied[2] = true;
            conditionApplied[3] = true;
            conditionApplied[4] = false;
        }
        if ((anxietyTiers[4]) < AnxietyLevel && AnxietyLevel <= 100f)
        {
            conditionApplied[0] = true;
            conditionApplied[1] = true;
            conditionApplied[2] = true;
            conditionApplied[3] = true;
            conditionApplied[4] = true;
        }
        if (AnxietyLevel >= 100f)
        {
            anxietyMaxxed = true;
        }
    }

    void AnxietyEffects()
    {
        if(conditionApplied[0])
        {

        }
        if (conditionApplied[1])
        {
            float i = (heartbeatRate * (101 - AnxietyLevel));
            if (i <= (heartbeatCutoff * heartbeatRate))
            {
                i = heartbeatCutoff * heartbeatRate;
            }
            if (counterHeartSFX >= i)
            {
                if (!gSFX.IsPlaying())
                {
                    gSFX.PlaySound(0);
                }
                counterHeartSFX = 0;
            }
            else
            {
                counterHeartSFX++;
            }
        }
        else
        {
            counterHeartSFX = 0;
        }
        if(conditionApplied[2])
        {
            float anxietyDecimal = (AnxietyLevel) / 100;
            playerCamera.fieldOfView = startFOV * (1+(anxietyDecimal-(anxietyTiers[2]/100)));//(1+((anxietyDecimal+(anxietyTiers[2]/100))/fovDiv));
            anxVignette.active = true;
            anxVignette.intensity.Override(anxietyDecimal);
        }
        else
        {
            playerCamera.fieldOfView = startFOV;
            anxVignette.active = false;
            anxVignette.intensity.Override(0);
        }
    }

    void VignetteApply(float curAnx)
    {
        if (vignetteGlide < AnxietyLevel)
        {
            anxVignette.intensity.Override(vignetteGlide);
            if (prevAnx < curAnx && curAnx !>= 99)
            {
                vignetteGlide += 0.1f;
            }
            else if (prevAnx > curAnx && curAnx !<= 0)
            {
                vignetteGlide--;
            }
            prevAnx = curAnx;

        }
        else
        {
            anxVignette.intensity.Override(curAnx);
            prevAnx = curAnx;
        }
    }

    void AnxietyCheck(float lowVal, float highVal, ref bool condition)
    {
        if (lowVal < AnxietyLevel && AnxietyLevel <= highVal)
        {
            condition = true;
        }
        else
        {
            condition = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        counter = 0;
        if (other.tag == "StaffAZ" || other.tag == "CustomerAZ" || other.tag == "DeliAZ")
        {
            inAnxietyZone = false;//sets the player to not be in the anxiety zone
        }
    }
}
