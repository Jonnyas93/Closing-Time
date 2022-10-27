using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class AnxietyManager : MonoBehaviour
{
    [Header("Anxiety Factors")]
    [Tooltip("The multiplier for how much anxiety the player gains whilst in a staff anxiety zone")] public float staffAnxietyMult = 1f;
    [Tooltip("The multiplier for how much anxiety the player gains whilst in a customer anxiety zone")] public float customerAnxietyMult = 1f;
    [Tooltip("The multiplier for how much anxiety the player gains whilst in a deli anxiety zone")] public float deliAnxietyMult = 1f;
    [Tooltip("How many tenths seconds between ticks up of anxiety")] public float anxIncreaseDelay = 10;
    [Tooltip("How many tenths seconds between ticks down of anxiety")] public float anxDecreaseDelay = 10;
    [Tooltip("The level of anxiety the player has")] public float AnxietyLevel { get; set; }
    [Tooltip("the thresholds at which the anxiety effects start")] public float[] anxietyTiers = { 10, 20, 30, 40, 50 };

    [Header("Effect Factors")]
    [Tooltip("The minimum percentage of default speed that the player will reach when affected by anxiety")] public float moveSpeedCutoff = 10f;
    [Tooltip("Time between beats, Lower is faster")] public float heartbeatRate = 1f;
    [Tooltip("The point at which the heartbeat wont get any faster")] public float heartbeatCutoff = 20f;
    [Tooltip("how heavily the fov adjustment factor gets divided")] public float fovDiv = 1f;
    [Tooltip("Length the blackout goes for")] public float blackoutDuration = 2f;

    bool anxietyMaxxed = false;
    bool[] conditionApplied = { false, false, false, false, false };
    int counterHeartSFX;
    float startFOV;
    float startSpeed;
    float startSprint;
    float anxietyMultiplier; //internal variable that holds the current zone's anxiety multiplier
    bool inAnxietyZone; //true/false value that determines if the player is in an anxiety zone or not
    float counter;

    Camera playerCamera;
    GameSFX gSFX;
    Vignette anxVignette;
    FirstPersonController playerController;
    Animator blackoutAnimator;

    public PostProcessProfile PPP;
    public Canvas screenBlackout;
    public Transform blackoutTransform;

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
        blackoutAnimator = screenBlackout.GetComponentInChildren<Animator>();
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
        if(conditionApplied[1]) //speed debuff
        {
            float speedMult = (90 - AnxietyLevel + moveSpeedCutoff) / 100;
            playerController.MoveSpeed = startSpeed * speedMult;
            playerController.SprintSpeed = startSprint * speedMult;
        }
        else
        {
            playerController.MoveSpeed = startSpeed;
            playerController.SprintSpeed = startSprint;
        }
        if (conditionApplied[0]) //heartbeat effect
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
        if(conditionApplied[2]) //fov effect
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
        if(anxietyMaxxed)
        {
            anxietyMaxxed = false;
            StartCoroutine(Blackout());
        }
    }

    IEnumerator Blackout()
    {
        blackoutAnimator.Play("Blackout");
        playerController.MoveSpeed = 0;
        playerController.SprintSpeed = 0;
        yield return new WaitForSeconds(blackoutDuration/2);
        transform.position = blackoutTransform.position;
        yield return new WaitForSeconds(blackoutDuration / 2);
        AnxietyLevel = 0;
        playerController.MoveSpeed = startSpeed;
        playerController.SprintSpeed = startSprint;
        conditionApplied[0] = false;
        conditionApplied[1] = false;
        conditionApplied[2] = false;
        conditionApplied[3] = false;
        conditionApplied[4] = false;
        anxietyMaxxed = false;
        blackoutAnimator.Play("BlackoutEnd");
    }

    public void AccessBlackout()
    {
        blackoutAnimator.Play("Blackout");
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
