using StarterAssets;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    private string output;
    private List<string> itemTypes;
    private List<int> itemValues;
    private List<int> pickedNumbers;
    private List<Item> inventory;
    private List<string> shoppingList;
    private StarterAssetsInputs _input;

    private AnxietyManager anxMan;
    private int divTime;
    

    [SerializeField] GameObject cameraRoot;
    [SerializeField] GameObject player;
    [SerializeField] ItemSelector itemSelector;
    [SerializeField] GameObject CheckoutWall;
    [SerializeField] GameSFX gSFX;

    [Tooltip("Number of items in the list to be collected")] public int ShoppingListLength = 3;
    public int inventoryScore;
    public bool inProgress = true;
    [Tooltip("Amount of time that the round runs for")] public int timeRemaining = 500;
    [Tooltip("Amount the score given by time is multiplied by")] public int timeMult = 1;
    public int blackoutScoreReduction = 0;

    [Header("UI Elements")]
    [SerializeField] Canvas ui;
    [SerializeField] TextMeshProUGUI timerText;
    [SerializeField] GameObject receipt;
    [SerializeField] TextMeshProUGUI inventoryScoreText;
    [SerializeField] TextMeshProUGUI timeRemaningText;
    [SerializeField] TextMeshProUGUI blackoutReductionText;
    [SerializeField] TextMeshProUGUI totalScoreText;
    [SerializeField] GameObject pauseMenu;
    [SerializeField] TextMeshProUGUI[] ShoppingListText;

     

    // Start is called before the first frame update
    void Start()
    {
        divTime = timeRemaining/4;
        pauseMenu.SetActive(false);
        receipt.SetActive(false);
        itemTypes = new List<string>();
        itemValues = new List<int>();
        pickedNumbers = new List<int>();
        shoppingList = new List<string>();
        inventory = new List<Item>();
        _input = GetComponent<StarterAssetsInputs>();
        anxMan = GetComponent<AnxietyManager>();
        string[] lines = File.ReadAllLines("itemList.csv");
        foreach (string s in lines)
        {
            string[] splitItem = s.Split(',');
            itemTypes.Add(splitItem[0]);
            itemValues.Add(Int32.Parse(splitItem[1]));
        }
        
        GenerateShoppingList();
        StartCoroutine(Timer());
    }

    // Update is called once per frame
    void Update()
    {
        Item i;
        if (_input.escape == true)
        {
            Pause();
        }
        if (_input.interact == true)
        {
            if (itemSelector.overItem)
            {
                gSFX.PlaySound(0);
                i = itemSelector.ReturnItem();
                inventory.Add(i);
                i.PickUp();
            }
        }
        _input.interact = false;
        _input.escape = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Checkout")
        {
            inProgress = false;
        }
        if (other.tag == "Exit")
        {
            StartCoroutine(ExitGame());
        }
    }

    public void GenerateShoppingList()
    {
        int selector;
        for (int i = 0; i < ShoppingListLength; i++)
        {
            selector = UnityEngine.Random.Range(0, itemTypes.Count);
            if (pickedNumbers.Count == 0)
            {
                pickedNumbers.Add(selector);
            }
            else
            {
                for (int number = 0; number < pickedNumbers.Count; number++)
                {
                    
                    if (selector == pickedNumbers[number])
                    {
                        
                        while (selector == pickedNumbers[number])
                        {
                            selector = UnityEngine.Random.Range(0, itemTypes.Count);
                            number = 0;
                        }
                    }
                }
                pickedNumbers.Add(selector);
            }
            ShoppingListText[i].text = itemTypes[selector].ToString();
            shoppingList.Add(itemTypes[selector]);
        }
        foreach (TextMeshProUGUI t in ShoppingListText)
        {
            if (t.text == "-")
            {
                t.gameObject.SetActive(false);
            }
        }
    }

    public void CalcInvScore()
    {

        for (int i = 0; i < itemTypes.Count; i++)
        {
            foreach (string s in shoppingList)
            {
                if (s == itemTypes[i])
                {
                    int itemTypeCount = 0;
                    for (int n = 0; n < inventory.Count; n++)
                    {
                        if (inventory[n].itemName == itemTypes[i])
                        {
                            itemTypeCount += 1;
                        }
                    }
                    inventoryScore += (itemTypeCount * itemValues[i]);
                    output += itemTypes[i] + " X " + itemTypeCount.ToString() + " = " + (itemTypeCount * itemValues[i]).ToString() + "\n";
                    break;
                }
            }
        }
    }

    public void WinGame()
    {
        CalcInvScore();
        gSFX.PlaySound(3, 0.5f);
        StartCoroutine(gSFX.PlaySoundWait(2,0.25f));
        CheckoutWall.SetActive(true);
        Cursor.lockState = CursorLockMode.Confined;
        _input.cursorInputForLook = false;
        inventoryScoreText.text = "Item Score: \n-------------------\n" + output;
        timeRemaningText.text = "Time Score: " + (timeRemaining*timeMult).ToString();
        blackoutReductionText.text = "Blackout Score reduction: " + blackoutScoreReduction.ToString();
        totalScoreText.text = "Total Score: " + ((timeRemaining*timeMult) + inventoryScore + blackoutScoreReduction).ToString();
        receipt.SetActive(true);
    }
    public void LoseGame()
    {
        SceneManager.LoadScene(2);
    }

    public void CloseReceipt()
    {
        _input.cursorInputForLook = true;
        receipt.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void Pause()
    {
        Time.timeScale = 0f;
        pauseMenu.SetActive(true);
        Cursor.lockState = CursorLockMode.Confined;
        _input.cursorInputForLook = false;
        
    }
    public void Resume()
    {
        Time.timeScale = 1f;
        if (!receipt.activeSelf)
        {
            Cursor.lockState = CursorLockMode.Locked;
            _input.cursorInputForLook = true;
        }
        pauseMenu.SetActive(false);
    }
    public void MainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }

    public void PickUpItem(Item item)
    {
        item.PickUp();
    }
    public void DropItem(Item item)
    {
        item.Drop();
    }

    public IEnumerator Timer()
    {
        int minutes = Mathf.FloorToInt(timeRemaining / 60);
        int Seconds = Mathf.FloorToInt(timeRemaining % 60);
        timerText.text = "Time remaining: " + minutes.ToString("00") + ":" + Seconds.ToString("00");
        if (timeRemaining < divTime)
        {
            float anxLev = (anxMan.AnxietyLevel / 2 + 50) / 100;
            gSFX.PlaySound(1, anxLev);
        }
        if (timeRemaining <= 0)
        {
            inProgress = false;
            LoseGame();
        }
        if (!inProgress)
        {
            if (timeRemaining > 0)
            {
                WinGame();
            }
        }
        if (inProgress)
        {
            timeRemaining--;
            yield return new WaitForSeconds(1f);
            StartCoroutine(Timer());
        }
    }

    public IEnumerator ExitGame()
    {
        Cursor.lockState = CursorLockMode.Confined;
        _input.cursorInputForLook = false;
        AnxietyManager ax = GetComponent<AnxietyManager>();
        ax.AccessBlackout();
        yield return new WaitForSeconds(ax.blackoutDuration);
        MainMenu();
    }
}
