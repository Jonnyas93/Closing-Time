using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class AnxietyManager : MonoBehaviour
{
    [Tooltip("The multiplier for how much anxiety the player gains whilst in a staff anxiety zone")] public float staffAnxietyMult = 1f;
    [Tooltip("The multiplier for how much anxiety the player gains whilst in a customer anxiety zone")] public float customerAnxietyMult = 1f;
    [Tooltip("The multiplier for how much anxiety the player gains whilst in a deli anxiety zone")] public float deliAnxietyMult = 1f;

    [Tooltip("How many tenths seconds between ticks up of anxiety")] public float anxIncreaseDelay = 10;
    [Tooltip("How many tenths seconds between ticks down of anxiety")] public float anxDecreaseDelay = 10;

    float anxietyMultiplier; //internal variable that holds the current zone's anxiety multiplier
    bool inAnxietyZone; //true/false value that determines if the player is in an anxiety zone or not

    float counter;
    [Tooltip("The level of anxiety the player has")] public float AnxietyLevel { get; set; }
    [Tooltip("the thresholds at which the anxiety effects start")] public float[] anxietyTiers = { 10, 20, 30, 40, 50 };
    [Tooltip("An array of booleans that keeps track of what conditions should be affecting the player")] public bool[] conditionApplied = { false, false, false, false, false };
    [Tooltip("The minimum percentage of default speed that the player will reach when affected by anxiety")] public float moveSpeedCutoff = 10f;
    [Tooltip("Time between beats, Lower is faster")] public float heartbeatRate = 1f;
    [Tooltip("The point at which the heartbeat wont get any faster")] public float heartbeatCutoff = 20f;
    [Tooltip("how heavily the fov adjustment factor gets divided")] public float fovDiv = 1f;

    bool anxietyMaxxed = false;
    int counterHeartSFX;
    float startFOV;
    float startSpeed;
    float startSprint;

    Camera playerCamera;
    GameSFX gSFX;
    
    Vignette anxVignette;
    FirstPersonController playerController;

    public PostProcessProfile PPP;
    public Canvas ScreenBlackout;


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
        playerController = GetComponent<FirstPersonController>();
        startSpeed = playerController.MoveSpeed;
        startSprint = playerController.SprintSpeed;
    }

    // FixedUpdate is called once per frame at 50fps
    void FixedUpdate()
    {
        if (inAnxietyZone) //checks for if the user is in an anxiety zone
        {
            if (counter >= (5 * anxIncreaseDelay)) //waits for the number of seconds determined by AnxIncreaseRate
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
            if (counter >= (5 * anxDecreaseDelay)) //waits for the number of seconds determined by AnxDecreaseRate
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
        if ((anxietyTiers[4]) < AnxietyLevel && AnxietyLevel < 100f)
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
            float speedMult = (100 - AnxietyLevel + moveSpeedCutoff) / 100;
            playerController.MoveSpeed = startSpeed * speedMult;
            playerController.SprintSpeed = startSprint * speedMult;
        }
        else
        {
            playerController.MoveSpeed = startSpeed;
            playerController.SprintSpeed = startSprint;
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

    void AnxietyBlackout()
    {

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
