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
    private List<string> itemTypes;
    private List<int> pickedNumbers;
    private List<Item> inventory;
    private List<string> shoppingList;
    private StarterAssetsInputs _input;

    [SerializeField] GameObject cameraRoot;
    [SerializeField] GameObject player;
    [SerializeField] ItemSelector itemSelector;
    [SerializeField] GameObject CheckoutWall;

    [Tooltip("Number of items in the list to be collected")] public int ShoppingListLength = 3;
    public int inventoryScore;
    public bool inProgress = true;
    [Tooltip("Amount of time that the round runs for")] public int timeRemaining = 500;
    [Tooltip("Amount the score given by time is multiplied by")] public int timeMult = 1;

    [Header("UI Elements")]
    [SerializeField] Canvas ui;
    [SerializeField] TextMeshProUGUI timerText;
    [SerializeField] GameObject receipt;
    [SerializeField] TextMeshProUGUI inventoryScoreText;
    [SerializeField] TextMeshProUGUI timeRemaningText;
    [SerializeField] TextMeshProUGUI totalScoreText;
    [SerializeField] GameObject pauseMenu;
    [SerializeField] TextMeshProUGUI[] ShoppingListText;

    // Start is called before the first frame update
    void Start()
    {
        pauseMenu.SetActive(false);
        receipt.SetActive(false);
        itemTypes = new List<string>();
        pickedNumbers = new List<int>();
        shoppingList = new List<string>();
        inventory = new List<Item>();
        _input = GetComponent<StarterAssetsInputs>();
        string[] lines = File.ReadAllLines("Assets/Scripts/itemList.csv");
        foreach (string s in lines)
        {
            string[] splitItem = s.Split(',');
            itemTypes.Add(splitItem[0]);
        }
        GenerateShoppingList();
        StartCoroutine(Timer());
    }

    // Update is called once per frame
    void Update()
    {
        Item i;
        if(_input.escape == true)
        {
            Pause();
        }
        if (_input.interact == true)
        {
            if (itemSelector.overItem)
            {
                i = itemSelector.ReturnItem();
                inventory.Add(i);
                foreach (string n in shoppingList)
                {
                    if (n == i.itemName)
                    {
                        inventoryScore += i.scoreAmount;
                    }
                }
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
            foreach (int number in pickedNumbers)
            {
                if (selector == number)
                {
                    while (selector == number)
                    {
                        selector = UnityEngine.Random.Range(0, itemTypes.Count);
                    }
                    pickedNumbers.Add(selector);
                    break;
                }
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

    public void WinGame()
    {
        CheckoutWall.SetActive(true);
        Cursor.lockState = CursorLockMode.Confined;
        _input.cursorInputForLook = false;
        inventoryScoreText.text = "Item Score: " + inventoryScore.ToString();
        timeRemaningText.text = "Time Score: " + (timeRemaining*timeMult).ToString();
        totalScoreText.text = "Total Score: " + (timeRemaining*timeMult + inventoryScore).ToString();
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
