﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class BuildingMain : MonoBehaviour 
{
    public string buildingName;
    public Vector2 size;
    public int[] levelsNeededNewBuilding;
    public int[] levelsNeededUpgrade;
    public int[] moneyNeededUpgrade;
    public int[] rpNeededUpgrade;
    [SerializeField]
    int[] timesForTasks, timesForBuilding;
    [SerializeField]
    string[] taskNames = new string[4];
    [SerializeField]
    int[] taskRewards = new int[4] // money - RP
        {50,20,30,40 };
    [SerializeField]
    public bool resourceBuilding = true, decoration = false;
    [SerializeField]
    string minigame = "", buildingInformation;
    public Sprite[] buildingSprites;
    public int[] exp;

    [HideInInspector]
    public bool busy, buildingBusy, clickedUpgrade;
    [HideInInspector]
    public Vector2 gridPosition;

    Color disabledColour, disabledRed;

    [Header("Don't Change")]
    [SerializeField]
    BuildingPlacer buildingPlacer;
    public int level, ID, taskDoing = -1;
    Account account;
    public float timeToFinishTask, timeToFinishTaskTotal, timeToFinishBuildTotal, timeLeftToFinishBuild;
    public TimeSpan endTime, startTime;
    public double maxtimeForTask, timer;
    public bool building = false, doneWithTask = false, onceToCreate = false, canClick = true;
    public TimeSpan timeSpan;

    float currentTime = 0;
    string[] minigameDiff = new string[4] {"Easy","Medium","Hard","Endless" };

    int[,] priceForUpgrading = new int[4, 3]
        { { 1, 100, 0}, // level 1
        { 4 , 300 , 0}, // level 2
        { 6, 500, 10}, // level 3
        { 8, 1000, 50}}; // level 4; // level, money, RP

    GameObject[] canvas;
    GameObject tempBar;
    Button[] allButtons;
    Image[] allImages;
    Text[] allText;
    Text[] alltext2;
    AudioClip upgradeSound, rewardSound, timelockSound;

    public bool ableToSave = true;
    
    public virtual void Start()
    {
        upgradeSound = GameObject.Find("HUD").GetComponent<HUD>().upgradeSound;
        rewardSound = GameObject.Find("HUD").GetComponent<HUD>().rewardSound;
        timelockSound = GameObject.Find("HUD").GetComponent<HUD>().timelockSound;
        disabledColour = GameObject.Find("HUD").GetComponent<HUD>().disabledColour;
        disabledRed = GameObject.Find("HUD").GetComponent<HUD>().disabledRed;
        if (!decoration)
        {
            for (int i = 0; i < 4; i++)
            {
                priceForUpgrading[i, 0] = levelsNeededUpgrade[i];
                priceForUpgrading[i, 1] = moneyNeededUpgrade[i];
                priceForUpgrading[i, 2] = rpNeededUpgrade[i];
            }
        }
        if(!resourceBuilding)
        {
            if(level == 3)
            {
                for(int i = 0; i < timesForTasks.Length; i++)
                {
                    timesForTasks[i] = 0;
                }
            }
        }
        if(size.x != size.y)
        {
            float newSize;
            if (size.x < size.y)
            {
                newSize = size.x;
            }
            else
            {
                newSize = size.y;
            }
            transform.localScale = new Vector3(newSize, newSize, 1);
        }
        else if(size.x == 2 && size.y == 2)
        {
            transform.localScale = new Vector2(3 * 0.98f, 3 * 0.95f);
        }
        account = GameObject.Find("Account").GetComponent<Account>();
        Input.simulateMouseWithTouches = true;
        if (busy)
        {
            Busy();
        }
        if (building)
        {
            Building();
        }
        canvas = GameObject.Find("HUD").GetComponent<HUD>().buildingCanvas;
        if(!resourceBuilding)
        {
            taskRewards = new int[4];
        }
        if (buildingSprites[level] != null)
        {
            GetComponent<SpriteRenderer>().sprite = buildingSprites[level];
        }
    }

    public virtual void Update()
    {
        SortingLayers();
        if (busy)
        {
            timer -= Time.deltaTime;
            timeSpan = TimeSpan.FromSeconds(timer);
            DrawBar(maxtimeForTask, timeSpan.TotalSeconds);
            if (timeSpan.TotalSeconds <= 0)
            {
                busy = false;
                doneWithTask = true;
                onceToCreate = true;
                GetComponent<CircleCollider2D>().enabled = false;
                Destroy(tempBar);
                if (!resourceBuilding)
                {
                    GameObject.Find("SFXController").GetComponent<AudioSource>().PlayOneShot(timelockSound);
                    GetReward();
                }
                else
                {
                    tempBar = (GameObject)Instantiate(canvas[3], transform.position + new Vector3(0, 3, 0), transform.rotation);
                    tempBar.GetComponentInChildren<Button>().onClick.AddListener(delegate { GetReward(); });
                    tempBar.GetComponent<Canvas>().sortingOrder = -1;
                }
            }
        }

        if (building)
        {
            timer -= Time.deltaTime;
            timeSpan = TimeSpan.FromSeconds(timer);
            DrawBar(maxtimeForTask, timeSpan.TotalSeconds);
            if (timeSpan.TotalSeconds <= 0)
            {
                building = false;
                level++;
                GameObject.Find("SFXController").GetComponent<AudioSource>().PlayOneShot(upgradeSound);
                Destroy(tempBar);
                account.exp += exp[level];
                Instantiate(GameObject.Find("HUD").GetComponent<HUD>().particleUpgrade, transform.position, transform.rotation);
                if (buildingSprites[level] != null)
                {
                    GetComponent<SpriteRenderer>().sprite = buildingSprites[level];
                }
                if (ableToSave)
                {
                    account.PushSave();
                }
            }
        }

        if (Time.time >= currentTime + 1f)
        {
            currentTime = Time.time;
            
        }
    }

    public virtual void OnMouseUp()
    {
        if (canClick)
        {
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                if (!busy && !building && !doneWithTask && GameObject.Find("BadgeScreen") == null)
                {
                    canvas[0].SetActive(true);
                    setupFirstButtons(canvas[0]);
                    GameObject.Find("HUD").GetComponent<HUD>().EnableButton(false);
                }
            }
        }
    }

    public void SetMaxTime()
    {
        timeToFinishBuildTotal = timesForBuilding[level];
    }

    #region Canvas
    void Busy()
    {
        maxtimeForTask = timesForTasks[taskDoing];
        if (canvas == null)
        {
            canvas = GameObject.Find("HUD").GetComponent<HUD>().buildingCanvas;
        }
        if (!resourceBuilding)
        {
            tempBar = (GameObject)Instantiate(canvas[2], transform.position + new Vector3(0, 3, 0), transform.rotation);
        }
        else
        {
            tempBar = (GameObject)Instantiate(canvas[4], transform.position + new Vector3(0, 3, 0), transform.rotation);
        }
    }

    void Building()
    {
        maxtimeForTask = timesForBuilding[level];
        if (canvas == null)
        {
            canvas = GameObject.Find("HUD").GetComponent<HUD>().buildingCanvas;
        }
        tempBar = (GameObject)Instantiate(canvas[5], transform.position + new Vector3(0, 3, 0), transform.rotation);
    }

    void DrawBar(double max, double min)
    {
        if (tempBar != null && tempBar != canvas[3])
        {
            Image[] all = tempBar.GetComponentsInChildren<Image>();
            foreach (Image temp in all)
            {
                if (temp.gameObject.tag == "Bar")
                {
                    temp.fillAmount = (float)(min / max);
                }
            }
            string timeToDisplay = "";
            if(timeSpan.Minutes < 1)
            {
                timeToDisplay = timeSpan.Seconds + " s";
            }
            else if(timeSpan.Hours < 1)
            {
                timeToDisplay = timeSpan.Minutes + 1 + " m";
            }
            else
            {
                timeToDisplay = timeSpan.Hours + 1 + " h";
            }
            tempBar.GetComponentInChildren<Text>().text = timeToDisplay;
        }
    }

    #region ButtonInnit

    public void setupFirstButtons(GameObject canvasTemp)
    {
        canvasTemp.GetComponent<RectTransform>().SetAsLastSibling();
        allButtons = canvasTemp.GetComponentsInChildren<Button>();
        allImages = canvasTemp.GetComponentsInChildren<Image>();
        for (int i = 0; i < allButtons.Length; i++)
        {
            allButtons[i].onClick.RemoveAllListeners();
        }
        allText = canvasTemp.GetComponentsInChildren<Text>();
        allText[0].text = buildingName + " – LVL " + (level + 1).ToString();
        allImages[16].sprite = buildingSprites[level];
        allButtons[0].onClick.AddListener(delegate { BackClicked(); });
        GameObject.Find("BackButton2").GetComponent<Button>().onClick.AddListener(delegate { BackClicked(); });
        allButtons[2].onClick.AddListener(delegate { ReposClicked(); });
        GameObject.Find("FullBarLevelBuilding").GetComponent<Image>().fillAmount = (float)(level + 1) / 4;
        ChangeResourceIconsTasks(true);

        if (!decoration)
        {
            allText[2].text = "Upgrade";
            allText[1].text = "";
            if (level != 3)
            {
                allText[3].text = priceForUpgrading[level + 1, 0].ToString();
                allText[22].text = priceForUpgrading[level + 1, 1].ToString();
                allText[23].text = priceForUpgrading[level + 1, 2].ToString();
                allButtons[1].interactable = true;
                allButtons[1].onClick.AddListener(delegate { UpgradeClickedFinal(); });
                if (!CheckIfEnoughResources(false))
                {
                    allButtons[1].interactable = false;
                    alltext2 = allButtons[1].GetComponentsInChildren<Text>();
                    foreach (Text temp in alltext2)
                    {
                        temp.color = disabledColour;
                        if (priceForUpgrading[level + 1, 0] > account.level)
                        {
                            allText[3].color = disabledRed;
                        }
                        else
                        {
                            allText[3].color = Color.white;
                        }
                        if (priceForUpgrading[level + 1, 1] > account.money)
                        {
                            allText[22].color = disabledRed;
                        }
                        else
                        {
                            allText[22].color = Color.white;
                        }
                        if (priceForUpgrading[level + 1, 2] > account.researchPoints)
                        {
                            allText[23].color = disabledRed;
                        } 
                        else
                        {
                            allText[23].color = Color.white;
                        }
                    }
                }
                else
                {
                    alltext2 = allButtons[1].GetComponentsInChildren<Text>();
                    foreach (Text temp in alltext2)
                    {
                        temp.color = Color.white;
                        allText[3].color = Color.white;
                        allText[22].color = Color.white;
                        allText[23].color = Color.white;
                    }
                }
            }
            else
            {
                allText[3].text = "Max Level";
                allButtons[1].interactable = false;
            }
            
            if (resourceBuilding)
            {
                allText[4].text = "Tasks";
                for (int i = 0; i < 4; i++)
                {
                    int tempi = i;
                    allButtons[tempi + 3].onClick.AddListener(delegate { TaskClicked(tempi); });
                    Text[] allText2 = allButtons[tempi + 3].GetComponentsInChildren<Text>();
                    allText2[0].text = taskNames[tempi];
                    allText2[1].text = taskRewards[tempi].ToString();
                    allText2[2].text = timesForTasks[tempi].ToString();
                    allText2[3].text = "";
                }
            }
            else
            {
                allText[4].text = "Mini-game\nDifficulty";
                ChangeResourceIconsTasks(false);
                for (int i = 0; i < 4; i++)
                {
                    int tempi = i;
                    allButtons[tempi + 3].onClick.AddListener(delegate { ClickedMinigame(tempi, minigame); });
                    Text[] allText2 = allButtons[tempi + 3].GetComponentsInChildren<Text>();
                    allText2[0].text = "";
                    allText2[1].text = "";
                    allText2[2].text = "";
                    allText2[3].text = minigameDiff[tempi];
                }
            }
            
            for (int i = 3; i < 7; i++)
            {
                allButtons[i].interactable = true;
                alltext2 = allButtons[i].GetComponentsInChildren<Text>();
                foreach (Text temp in alltext2)
                {
                    temp.color = Color.white;
                }
            }
            
            if(level < 3)
            {
                allButtons[6].interactable = false;
                alltext2 = allButtons[6].GetComponentsInChildren<Text>();
                foreach (Text temp in alltext2)
                {
                    temp.color = disabledColour;
                }
            }
            if(level < 2)
            {
                allButtons[5].interactable = false;
                alltext2 = allButtons[5].GetComponentsInChildren<Text>();
                foreach (Text temp in alltext2)
                {
                    temp.color = disabledColour;
                }
            }
            if(level < 1)
            {
                allButtons[4].interactable = false;
                alltext2 = allButtons[4].GetComponentsInChildren<Text>();
                foreach (Text temp in alltext2)
                {
                    temp.color = disabledColour;
                }
            }

            if(!resourceBuilding && account.level < 2)
            {
                allButtons[3].interactable = false;
                alltext2 = allButtons[3].GetComponentsInChildren<Text>();
                foreach (Text temp in alltext2)
                {
                    temp.color = disabledColour;
                }
            }
        }
        else
        {
            allButtons[1].interactable = true;
            allButtons[1].onClick.AddListener(delegate { DeleteDeco(); });
            allText[3].text = "";
            for (int i = 3; i < 7; i++)
            {
                allButtons[i].gameObject.SetActive(false);
            }
            allButtons[7].gameObject.SetActive(true);
            allButtons[8].gameObject.SetActive(true);
            allText[1].gameObject.SetActive(false);
            allText[2].text = "Destroy";
            allText[4].text = "";
            for (int i = 5; i < allText.Length; i++)
            {
                allText[i].gameObject.SetActive(false);
            }
            for (int i = 9; i < allImages.Length; i++)
            {
                if (i != 16 && i != 19 && i != 20)
                {
                    allImages[i].gameObject.SetActive(false);
                }
            }

        }

        allButtons[allButtons.Length - 1].onClick.AddListener(delegate { GiveBuildingInformation(buildingInformation); });

    }
    #endregion

    #region Buttons

    void GiveBuildingInformation(string info)
    {
        GameObject.Find("TextInformationBuilding").GetComponent<Text>().text = info;
    }

    void ChangeResourceIconsTasks(bool toChange)
    {
        allImages[5].enabled = toChange;
        allImages[6].enabled = toChange;

        allImages[8].enabled = toChange;
        allImages[9].enabled = toChange;

        allImages[11].enabled = toChange;
        allImages[12].enabled = toChange;

        allImages[14].enabled = toChange;
        allImages[15].enabled = toChange;
    }

    void DeleteDeco()
    {
        Vector3 positionOfNewBuilding = new Vector3(transform.position.x, transform.position.y + 0.3f, transform.position.z);
        BuildingPlacer tempBuilding = (BuildingPlacer)Instantiate(buildingPlacer, positionOfNewBuilding, transform.rotation);
        tempBuilding.buildingToPlace = this.gameObject;
        tempBuilding.activePlaceOnGrid = gridPosition;
        tempBuilding.builderPlacerTemp = buildingPlacer;
        tempBuilding.fieldID = GameObject.Find("Grid").GetComponent<Grid>().grid[(int)gridPosition.x, (int)gridPosition.y].ID;
        tempBuilding.oldBuilding = this.gameObject;
        tempBuilding.rePos = true;
        tempBuilding.oldActivePlace = gridPosition;
        BackClicked();
        tempBuilding.Delete();
        GameObject.FindGameObjectWithTag("Builder").GetComponent<BuildingPlacer>().Delete();
        MainGameController.ChangeColliders(true);
        Destroy(GameObject.FindGameObjectWithTag("Builder"));
        Destroy(gameObject);
        account.PushSave();
    }

    void ClickedMinigame(int minigameDifficulty, string minigame)
    {
        TaskClicked(minigameDifficulty);
        GameObject.Find("MiniGameController").GetComponent<MiniGameController>().buildingID = ID;
        if (ableToSave)
        {
            account.PushSave();
        }
        GameObject.Find("MiniGameController").GetComponent<MiniGameController>().ActivateMiniGame(minigame, minigameDifficulty);
    }

    public void UpgradeClickedFinal()
    {
        if (CheckIfEnoughResources())
        {
            building = true;
            timer = timesForBuilding[level];
            Building();
            BackClicked();
        }
    }

    public void TaskClicked(int task)
    {
        busy = true;
        taskDoing = task;
        timer = timesForTasks[taskDoing];
        if (ableToSave)
        {
            account.PushSave();
        }
        BackClicked();
        Busy();
    }

    public void BackClicked()
    {
        canvas[0].SetActive(false);
        for (int i = 0; i < allButtons.Length; i++)
        {
            allButtons[i].gameObject.SetActive(true);
        }
        allText[1].gameObject.SetActive(true);
        for (int i = 5; i < allText.Length; i++)
        {
            allText[i].gameObject.SetActive(true);
        }
        for (int i = 9; i < allImages.Length; i++)
        {
            allImages[i].gameObject.SetActive(true);
        }
        MainGameController.ChangeColliders(true);
        GameObject.Find("HUD").GetComponent<HUD>().EnableButton();
    }

    public void ReposClicked()
    {
        Vector3 positionOfNewBuilding = new Vector3(transform.position.x, transform.position.y + 0.3f, transform.position.z);
        BuildingPlacer tempBuilding = (BuildingPlacer)Instantiate(buildingPlacer, positionOfNewBuilding, transform.rotation);
        tempBuilding.buildingToPlace = this.gameObject;
        tempBuilding.activePlaceOnGrid = gridPosition;
        tempBuilding.builderPlacerTemp = buildingPlacer;
        tempBuilding.fieldID = GameObject.Find("Grid").GetComponent<Grid>().grid[(int)gridPosition.x, (int)gridPosition.y].ID;
        tempBuilding.oldBuilding = this.gameObject;
        tempBuilding.rePos = true;
        tempBuilding.oldActivePlace = gridPosition;
        BackClicked();
        account.autoSave = false;
        tempBuilding.Delete();
    }
    #endregion 
    #endregion

    void SortingLayers()
    {
        int layerSort = Mathf.RoundToInt(((gridPosition.x + size.x) + (gridPosition.y + size.y)) - (size.x / 2 + size.y / 2));
        layerSort *= -layerSort;
        GetComponent<SpriteRenderer>().sortingOrder = layerSort;
    }
    
    public void GetReward()
    {
        doneWithTask = false;
        if (resourceBuilding)
        {
            GameObject.Find("SFXController").GetComponent<AudioSource>().PlayOneShot(rewardSound);
        }
        account.money += taskRewards[taskDoing];
        taskDoing = -1;
        Destroy(tempBar);
        onceToCreate = false;
        GetComponent<CircleCollider2D>().enabled = true;
        Instantiate(GameObject.Find("HUD").GetComponent<HUD>().particleReward, transform.position, transform.rotation);
        if (ableToSave)
        {
            account.PushSave();
        }
    }

    bool CheckIfEnoughResources(bool remove = true)
    {
        bool enough = true;
        if (priceForUpgrading[level + 1, 0] > account.level)
        {
            enough = false;
        }
        if (priceForUpgrading[level + 1, 1] > account.money)
        {
            enough = false;
        }
        if (priceForUpgrading[level + 1, 2] > account.researchPoints)
        {
            enough = false;
        }

        if (enough && remove)
        {
            account.money -= priceForUpgrading[level + 1, 1];
            account.researchPoints -= priceForUpgrading[level + 1, 2];
        }
        return enough;
    }
}
