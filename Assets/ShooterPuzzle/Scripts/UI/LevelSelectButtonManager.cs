using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelectButtonManager : MonoBehaviour
{
    public GameObject buttonPrefab;
    public MenuButtonCommands buttonCommands;
    PuzzleShooterGameManager saveData;
    public int buttonsPerRow;
    [Range(0,1)]public float percentRightBuffer;
    [Range(0, 1)] public float percentBottomBuffer;
    float spacingX;
     float spacingY;

    List<Button> buttonList;

    // Start is called before the first frame update
    void Start()
    {
        saveData = PuzzleShooterGameManager.instance;
        buttonList = new List<Button>();
        InstantiateLevelButtons();


    }

    void InstantiateLevelButtons()
    {
        GameObject newButton;
        Vector3 origin = gameObject.GetComponent<RectTransform>().position;
        spacingX = (Camera.main.scaledPixelWidth - (Camera.main.scaledPixelWidth * percentRightBuffer)) / buttonsPerRow;
        spacingY = (Camera.main.scaledPixelHeight - (Camera.main.scaledPixelHeight * percentBottomBuffer)) / (saveData.totalLevels / buttonsPerRow);
        int buttonsX = 0;
        int buttonsY = 0;
        int numberCompleted = saveData.levelsCompleted;
        int length = saveData.totalLevels;
        for (int i = 0; i < length; i++)
        {
            if (buttonsX == buttonsPerRow)
            {
                buttonsX = 0;
                ++buttonsY;
            }
            Vector3 pos = new Vector3(spacingX * buttonsX, -spacingY * buttonsY, 0) + origin;
            newButton = Instantiate(buttonPrefab, gameObject.transform);
            

            newButton.GetComponent<RectTransform>().position = pos;//sets pos
            newButton.GetComponentInChildren<Text>().text = (i + 1).ToString(); //sets text
            Button buttonComp = newButton.GetComponent<Button>();
            int levelContainer = new int();
            levelContainer = i+1;
            buttonComp.onClick.AddListener(delegate { buttonCommands.ChangeScene("Level "+ levelContainer); }); //sets onClick function

            if (numberCompleted < i)
            { buttonComp.interactable = false; }

            buttonList.Add(buttonComp);
            ++buttonsX;
        }
    }

    public void RefreshButtons()
    {
        int length = buttonList.Count;
        for (int i = 0; i < length; i++)
        {
            if(i>saveData.levelsCompleted)
            {
                buttonList[i].interactable = false;
            }
            else
            {
                buttonList[i].interactable = true;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(saveData.completedLevelValueChanged)
        {
            RefreshButtons();
        }
    }
}
