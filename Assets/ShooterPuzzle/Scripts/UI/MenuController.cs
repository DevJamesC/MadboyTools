using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using System;
using UnityEngine.EventSystems;

public class MenuController : MonoBehaviour
{
    [SerializeField]
    private GameObject noMenuUIObj;
    private RectTransform noMenuUIPanel;
    [SerializeField]
    private GameObject mainMenuPanelObj;//the panel OBJECT on which all menus are.
    private RectTransform mainMenuPanel;//the piece of the MainMenuPanel which we modify. We use the above to set it active/inactive. This is used to change its size
    [SerializeField]
    private GameObject inLobbyNoMenuPanelObj;
    private RectTransform inLobbyNoMenuPanel;
    [SerializeField]
    private GameObject mainMenuObj;//the child object called MainMenu
    [SerializeField]
    private GameObject playMenuObj;//the child object called PlayMenu
    [SerializeField]
    private GameObject joinMenuObj;//the object called JoinMenu (not a child object. A sibling object)
    [SerializeField]
    private GameObject createMenuObj;//the object called createMenu (not a child object. A sibling object)
    [SerializeField]
    private GameObject inLobbyMenuObj;//the object called inLobbyMenu (not a child object. A sibling object)
    [SerializeField]
    private GameObject inLobbyNoMenuObj;//the object called inLobbyNoMenu (not a child object. A sibling object)
    [SerializeField]
    private GameObject closeButtonObj;//the object which closes the whole menu
    private Button closeButton;
    private Text closeButtonText;

    //Every dynamically positioned UI elements need the tag UIElement, otherwise it will not be added listItems, and it will not reposition upon screen resizing
    private List<MenuItemPosData> listItems;
    [SerializeField]
    private List<MenuItemFadeData> mainListItems;
    private List<MenuItemPosData> mainListItemsPos;
    [SerializeField]
    private List<MenuItemFadeData> playListItems;
    [SerializeField]
    private List<MenuItemFadeData> joinListItems;
    [SerializeField]
    private List<MenuItemFadeData> createListItems;
    [SerializeField]
    private List<MenuItemFadeData> inLobbyListItems;
    [SerializeField]
    private List<MenuItemFadeData> inLobbyNoMenuListItems;


    [SerializeField]
    private float dropDownDuration;
    private float currentDropDownTime;
    [SerializeField]
    private float playTransitionDuration;//this is used for ALL TRANSISIONS. 
    private float currentPlayTransitionTime;//and this
    private int playMenuPhaseTwoCurrentElementIteration;
    private Rect canvasRect;
    private Rect mainMenuStartingRect;
    private Rect inLobbyNoMenuStartingRect;

    private bool dropDownStateChanging;//StateChaning is used to know if it needs to change. //Activating tells weither it is opening or closing
    private bool droppingDown;//this is the "activating" for menuDropdown

    private bool playMenuStateChanging;
    private bool playMenuActivating;

    private bool joinMenuStateChanging;
    private bool joinMenuActivating;

    private bool createMenuStateChanging;
    private bool createMenuActivating;

    private bool inLobbyMenuStateChanging;
    private bool inLobbyMenuActivating;
    private int inLobbyReturnIndex;//used to know if inLobbyDeactivating will return to JoinMenu, CreateMenu, or mainMenu
    private bool returnToJoin;

    private bool inLobbyNoMenuStateChanging;
    private bool inLobbyNoMenuActivating;
    private int inLobbyLeaveIndex;//used to know which menu to close when activating inLobbyNoMenu 
    //0->noMainMenu 1->mainMenu 2->playMenu 3->joinMenu 4->CreateMenu 5->inLobbyMenu

    private int longestListCount;
    private bool inLobby;
    

    // Start is called before the first frame update
    void Start()
    {
        var ls = GameObject.FindGameObjectsWithTag("UIElement");
        listItems = new List<MenuItemPosData>();
        foreach (GameObject obj in ls)
        {
            MenuItemPosData data = new MenuItemPosData();
            data.rectTrans = obj.GetComponent<RectTransform>();
            listItems.Add(data);
        }


        mainMenuPanel = mainMenuPanelObj.GetComponent<RectTransform>();
        inLobbyNoMenuPanel= inLobbyNoMenuPanelObj.GetComponent<RectTransform>();
        noMenuUIPanel = noMenuUIObj.GetComponent<RectTransform>();
        currentDropDownTime = dropDownDuration;

        canvasRect = mainMenuPanelObj.transform.root.gameObject.GetComponent<RectTransform>().rect;
        mainMenuStartingRect = mainMenuPanelObj.GetComponent<RectTransform>().rect;
        inLobbyNoMenuStartingRect=inLobbyNoMenuPanelObj.GetComponent<RectTransform>().rect;
        closeButton = closeButtonObj.GetComponent<Button>();
        closeButtonText = closeButtonObj.GetComponentInChildren<Text>();


        dropDownStateChanging = false;
        droppingDown = true;
        playMenuStateChanging = false;
        playMenuActivating = true;
        joinMenuStateChanging = false;
        joinMenuActivating = true;
        createMenuStateChanging = false;
        createMenuActivating = true;
        inLobbyMenuStateChanging = false;
        inLobbyMenuActivating = true;
        inLobbyNoMenuStateChanging = false;
        inLobbyNoMenuActivating = false;
        


        playMenuPhaseTwoCurrentElementIteration = -1;
        UpdateElementStartingPos();

        longestListCount = mainListItems.Count;
        longestListCount = (longestListCount >= playListItems.Count) ? longestListCount : playListItems.Count;
        inLobby = false;

        mainListItemsPos = new List<MenuItemPosData>();
        for (int i = 0; i < mainListItems.Count; i++)
        {
            if (mainListItems[i].button != null)
            {
                MenuItemPosData data = new MenuItemPosData();
                data.rectTrans = mainListItems[i].button.gameObject.GetComponent<RectTransform>();
                data.startingPos = data.rectTrans.position;

                mainListItemsPos.Add(data);
            }
        }


    }
    // Update is called once per frame
    void Update()
    {

        ChangeState();

    }
    public void DropDownMainMenu()
    {
        if (!dropDownStateChanging)
        {
            currentDropDownTime = dropDownDuration;
            dropDownStateChanging = true;
            droppingDown = true;
            UpdateElementStartingPos();
            mainMenuPanelObj.SetActive(true);
           
        }
    }

    public void CloseMainMenu()
    {
        if (!dropDownStateChanging)
        {
            currentDropDownTime = dropDownDuration;
            dropDownStateChanging = true;
            droppingDown = false;
           // UpdateElementStartingPos(); due to the mainMenu being a child of the mainMenuPanel (which gets moved around a lot), taking a snapshot of the current 
           //element locations means that they get taken while the main menu elements are in a weird spot. this needs to be called by a "resize screen" button.
           //even then, the elements will need to be placed in thier original spot, then have the snapshot be taken, then moved back.
            noMenuUIObj.SetActive(true);
           
        }
    }

    public void ActivatePlayMenu()
    {
        if (!playMenuStateChanging)
        {
            if (!inLobby)
            {
                currentPlayTransitionTime = playTransitionDuration;

                playMenuPhaseTwoCurrentElementIteration = -1;
                playMenuStateChanging = true;
                playMenuActivating = true;
                FreezeButtons(true);
            }
            else
            {
                currentPlayTransitionTime = playTransitionDuration;

                inLobbyReturnIndex = 3;
                inLobbyMenuStateChanging = true;
                inLobbyMenuActivating = true;
            }



        }
    }
    public void DeactivatePlayMenu()
    {
        if (!playMenuStateChanging)
        {
            currentPlayTransitionTime = playTransitionDuration;

            playMenuPhaseTwoCurrentElementIteration = -1;
            playMenuStateChanging = true;
            playMenuActivating = false;
            FreezeButtons(true);



        }
    }

    public void ActivateJoinMenu()
    {
        if (!joinMenuStateChanging)
        {
            currentPlayTransitionTime = playTransitionDuration;
            joinMenuStateChanging = true;
            joinMenuActivating = true;
            FreezeButtons(true);
        }


    }
    public void DeactivateJoinMenu()
    {
        if (!joinMenuStateChanging)
        {
            currentPlayTransitionTime = playTransitionDuration;
            playMenuPhaseTwoCurrentElementIteration = -1;
            joinMenuStateChanging = true;
            joinMenuActivating = false;
            FreezeButtons(true);
        }

    }

    public void ActivateCreateMenu()
    {
        if (!createMenuStateChanging)
        {
            currentPlayTransitionTime = playTransitionDuration;
            createMenuStateChanging = true;
            createMenuActivating = true;
            FreezeButtons(true);
        }
    }
    public void DeactivateCreateMenu()
    {
        if (!createMenuStateChanging)
        {
            currentPlayTransitionTime = playTransitionDuration;
            playMenuPhaseTwoCurrentElementIteration = -1;
            createMenuStateChanging = true;
            createMenuActivating = false;
            FreezeButtons(true);
        }
    }

    public void ActivateInLobbyMenu()
    {
        if (!inLobbyMenuStateChanging)
        {
            currentPlayTransitionTime = playTransitionDuration;
            inLobbyMenuStateChanging = true;
            inLobbyMenuActivating = true;
            FreezeButtons(true);
        }
    }
    public void DeactivateInLobbyMenu(int returnIndex = 0)
    {
        if (!inLobbyMenuStateChanging)
        {
            if (returnIndex > 0)
            {
                inLobbyReturnIndex = returnIndex;
            }
            else
            {
                if (inLobbyLeaveIndex != 5)//if we didn't enter through quickmatch
                {
                    inLobbyReturnIndex = (returnToJoin) ? 1 : 2;
                }
            }

            currentPlayTransitionTime = playTransitionDuration;
            playMenuPhaseTwoCurrentElementIteration = -1;
            inLobbyMenuStateChanging = true;
            inLobbyMenuActivating = false;
            FreezeButtons(true);
        }
    }

    public void ActivateInLobbyNoMenu()
    {
        if (!inLobbyNoMenuStateChanging)
        {
            //leavingIndex needs to be set in the other states

            currentPlayTransitionTime = playTransitionDuration;
            inLobbyNoMenuStateChanging = true;
            inLobbyNoMenuActivating = true;
            FreezeButtons(true);
        }
    }
    public void DeactivateInLobbyNoMenu()
    {
        if (!inLobbyNoMenuStateChanging)
        {
            //leavingIndex needs to be set in the other states

            currentPlayTransitionTime = playTransitionDuration;
            inLobbyNoMenuStateChanging = true;
            inLobbyNoMenuActivating = false;
            FreezeButtons(true);
        }
    }

    void ChangeState()
    {
        ChangeMainMenuState();
        ChangePlayMenuState();
        ChangeJoinMenuState();
        ChangeCreateMenuState();
        ChangeInLobbyMenuState();
        ChangeInLobbyNoMenuState();
    }

    void ChangeMainMenuState()
    {
        if (dropDownStateChanging)//changes the state of the main menu screen
        {
            inLobbyLeaveIndex = 1;
            if (currentDropDownTime > 0)
            {
                if (!playMenuObj.activeSelf)
                {
                    mainMenuObj.SetActive(true);
                }

                currentDropDownTime -= Time.deltaTime;

                float percent = Mathf.Clamp01(currentDropDownTime / dropDownDuration);
                percent = (droppingDown) ? percent : 1 - percent;
                //adjusts the height of the main panel 
                float height = Mathf.Lerp(canvasRect.height, 0, percent);
                
                Vector3 pos = mainMenuPanel.anchoredPosition;
                pos.y = Mathf.Lerp(canvasRect.y, 0, percent);
                pos.x = -mainMenuStartingRect.x;

                mainMenuPanel.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
                mainMenuPanel.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, mainMenuStartingRect.width);
                mainMenuPanel.anchoredPosition = pos;
                //adjusts the height of the "noMenuUI" panel
                height = Mathf.Lerp(canvasRect.height, 0, 1 - percent);

                pos = noMenuUIPanel.anchoredPosition;
                pos.y = Mathf.Lerp(canvasRect.y, 0, percent);
                pos.y -= canvasRect.y;
                noMenuUIPanel.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
                noMenuUIPanel.anchoredPosition = pos;



                //adjusts the y pos of each button element in the panel
                foreach (MenuItemPosData item in listItems)
                {
                    item.rectTrans.position = item.startingPos;
                }



            }
            else
            {
                if (!droppingDown)
                {
                    inLobbyLeaveIndex = 0;
                    mainMenuObj.SetActive(true);
                    playMenuObj.SetActive(false);
                    if (playMenuActivating)
                    {
                        for (int i = 0; i < mainListItems.Count; i++)//this sets us back to the home menu if we are on "play" and we close the menu
                        {
                            if (mainListItems[i].image != null)
                            {
                                mainListItems[i].image.canvasRenderer.SetAlpha(1);
                            }

                            if (mainListItems[i].textObj != null)
                            {
                                mainListItems[i].textObj.canvasRenderer.SetAlpha(1);
                            }

                            if (mainListItems[i].button != null)
                            {

                                mainListItems[i].button.image.canvasRenderer.SetAlpha(1);
                            }
                        }
                    }
                    mainMenuPanelObj.SetActive(false);
                }
                dropDownStateChanging = false;
                
            }

        }
    }
    void ChangePlayMenuState()
    {
        if (playMenuStateChanging)
        {
            if (currentPlayTransitionTime > 0)
            {
                currentPlayTransitionTime -= Time.deltaTime;
                float phaseOneDuration = playTransitionDuration / 2;
                float percent = Mathf.Clamp01(currentPlayTransitionTime / playTransitionDuration);
                float phaseOnePercent = Mathf.Clamp01(Mathf.Lerp(-1, 1, percent));
                float phaseTwoPercent = (phaseOnePercent > 0) ? 1 : Mathf.Clamp01(Mathf.Lerp(0, 2, percent));

                //Phase One: dissapear all current menu options
                if (playMenuActivating)
                {
                    if (phaseOnePercent > 0)
                    {
                        inLobbyLeaveIndex = 2;
                        inLobbyReturnIndex = 4;

                        mainMenuObj.SetActive(true);
                        playMenuObj.SetActive(true);


                        for (int i = 0; i < playListItems.Count; i++)//this needs to be set every frame for... no reason. It just doesn't work otherwise
                        {

                            if (playListItems[i].image != null)
                            {
                                playListItems[i].image.canvasRenderer.SetAlpha(0);

                            }

                            if (playListItems[i].textObj != null)
                            {
                                playListItems[i].textObj.canvasRenderer.SetAlpha(0);

                            }

                            if (playListItems[i].button != null)
                            {
                                playListItems[i].button.image.canvasRenderer.SetAlpha(0);

                            }
                        }


                        for (int i = 0; i < mainListItems.Count; i++)
                        {
                            if (mainListItems[i].image != null)
                            {
                                mainListItems[i].image.CrossFadeAlpha(0, phaseOneDuration / longestListCount, true);
                            }

                            if (mainListItems[i].textObj != null)
                            {
                                mainListItems[i].textObj.CrossFadeAlpha(0, phaseOneDuration / longestListCount, true);
                            }

                            if (mainListItems[i].button != null)
                            {

                                mainListItems[i].button.image.CrossFadeAlpha(0, phaseOneDuration / longestListCount, true);
                            }
                        }
                    }
                    else//Phase Two: appear all new menu options
                    {
                        int currentElement = Mathf.CeilToInt((1 - phaseTwoPercent) * mainListItems.Count) - 1;
                        mainMenuObj.SetActive(false);
                        playMenuObj.SetActive(true);

                        if (playMenuPhaseTwoCurrentElementIteration < currentElement)
                        {
                            if (currentElement < playListItems.Count)
                            {
                                if (playListItems[currentElement].image != null)
                                {

                                    playListItems[currentElement].image.CrossFadeAlpha(1, phaseOneDuration / longestListCount, true);
                                }
                                if (playListItems[currentElement].textObj != null)
                                {

                                    playListItems[currentElement].textObj.CrossFadeAlpha(1, phaseOneDuration / longestListCount, true);
                                }
                                if (playListItems[currentElement].button != null)
                                {

                                    playListItems[currentElement].button.image.CrossFadeAlpha(1, phaseOneDuration / longestListCount, true);
                                }
                            }


                            playMenuPhaseTwoCurrentElementIteration = currentElement;
                        }

                    }


                }
                else
                {
                    if (phaseOnePercent > 0)//Phase one
                    {
                        inLobbyLeaveIndex = 1;
                     
                        mainMenuObj.SetActive(true);
                        playMenuObj.SetActive(true);


                        for (int i = 0; i < mainListItems.Count; i++)//this needs to be set every frame for... no reason. It just doesn't work otherwise
                        {

                            if (mainListItems[i].image != null)
                            {
                                mainListItems[i].image.canvasRenderer.SetAlpha(0);

                            }

                            if (mainListItems[i].textObj != null)
                            {
                                mainListItems[i].textObj.canvasRenderer.SetAlpha(0);

                            }

                            if (mainListItems[i].button != null)
                            {
                                mainListItems[i].button.image.canvasRenderer.SetAlpha(0);

                            }
                        }
                        for (int i = 0; i < playListItems.Count; i++)
                        {
                            if (playListItems[i].image != null)
                            {
                                playListItems[i].image.CrossFadeAlpha(0, phaseOneDuration / longestListCount, true);
                            }

                            if (playListItems[i].textObj != null)
                            {
                                playListItems[i].textObj.CrossFadeAlpha(0, phaseOneDuration / longestListCount, true);
                            }

                            if (playListItems[i].button != null)
                            {

                                playListItems[i].button.image.CrossFadeAlpha(0, phaseOneDuration / longestListCount, true);
                            }
                        }


                    }
                    else//Phase Two
                    {
                        int currentElement = Mathf.CeilToInt((1 - phaseTwoPercent) * mainListItems.Count) - 1;
                        mainMenuObj.SetActive(true);
                        playMenuObj.SetActive(false);

                        if (playMenuPhaseTwoCurrentElementIteration < currentElement)
                        {
                            if (mainListItems[currentElement].image != null)
                            {


                                mainListItems[currentElement].image.CrossFadeAlpha(1, phaseOneDuration / longestListCount, true);
                            }
                            if (mainListItems[currentElement].textObj != null)
                            {

                                mainListItems[currentElement].textObj.CrossFadeAlpha(1, phaseOneDuration / longestListCount, true);
                            }
                            if (mainListItems[currentElement].button != null)
                            {

                                mainListItems[currentElement].button.image.CrossFadeAlpha(1, phaseOneDuration / longestListCount, true);
                            }


                            playMenuPhaseTwoCurrentElementIteration = currentElement;
                        }
                    }
                }

            }
            else
            {
                playMenuStateChanging = false;
                if (playMenuActivating)
                {
                    mainMenuObj.SetActive(false);
                }
                else
                {
                    playMenuObj.SetActive(false);
                }
                FreezeButtons(false);
            }

        }
    }
    void ChangeJoinMenuState()
    {
        if (joinMenuStateChanging)
        {
            if (currentPlayTransitionTime > 0)
            {
                currentPlayTransitionTime -= Time.deltaTime;
                float percent = Mathf.Clamp01(currentPlayTransitionTime / playTransitionDuration);
                float phaseOneDuration = playTransitionDuration / 2;
                float phaseOnePercent = Mathf.Clamp01(Mathf.Lerp(-2, 1, percent));//change the -2 either higher or lower to adjust the speed of phase one
                float phaseTwoPercent = (phaseOnePercent > 0) ? 1 : Mathf.Clamp01(Mathf.Lerp(0, 2, percent));

                if (joinMenuActivating)
                {
                    //phase one: dissapear play menu buttons and expand the panel
                    inLobbyReturnIndex = 1;
                    returnToJoin = true;
                    
                    inLobbyLeaveIndex = 3;
                    if (percent > .6f)
                    {
                        joinMenuObj.SetActive(true);
                        float width = Mathf.Lerp(canvasRect.width, mainMenuStartingRect.width, phaseOnePercent);

                        Vector3 pos = mainMenuPanel.anchoredPosition;
                        pos.x = Mathf.Lerp(-canvasRect.x, -mainMenuStartingRect.x, phaseOnePercent);

                        mainMenuPanel.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
                        mainMenuPanel.anchoredPosition = pos;

                        for (int i = 0; i < joinListItems.Count; i++)//setting the phase two elements alpha to 0 so they do appear visible in phase one
                        {
                            if (joinListItems[i].image != null)
                            {
                                joinListItems[i].image.canvasRenderer.SetAlpha(0);
                            }

                            if (joinListItems[i].textObj != null)
                            {
                                joinListItems[i].textObj.canvasRenderer.SetAlpha(0);
                            }

                            if (joinListItems[i].button != null)
                            {

                                joinListItems[i].button.image.canvasRenderer.SetAlpha(0);
                            }
                        }

                        for (int i = 0; i < playListItems.Count; i++)
                        {
                            if (playListItems[i].image != null)
                            {
                                playListItems[i].image.CrossFadeAlpha(0, phaseOneDuration / longestListCount, true);
                            }

                            if (playListItems[i].textObj != null)
                            {
                                playListItems[i].textObj.CrossFadeAlpha(0, phaseOneDuration / longestListCount, true);
                            }

                            if (playListItems[i].button != null)
                            {

                                playListItems[i].button.image.CrossFadeAlpha(0, phaseOneDuration / longestListCount, true);
                            }
                        }

                        closeButton.image.CrossFadeAlpha(0, phaseOneDuration / longestListCount, true);
                        closeButtonText.CrossFadeAlpha(0, phaseOneDuration / longestListCount, true);

                    }
                    else//phase two
                    {
                        playMenuObj.SetActive(false);
                        for (int i = 0; i < joinListItems.Count; i++)
                        {
                            if (joinListItems[i].image != null)
                            {
                                joinListItems[i].image.CrossFadeAlpha(1, phaseOneDuration / longestListCount, true);
                            }

                            if (joinListItems[i].textObj != null)
                            {
                                joinListItems[i].textObj.CrossFadeAlpha(1, phaseOneDuration / longestListCount, true);
                            }

                            if (joinListItems[i].button != null)
                            {

                                joinListItems[i].button.image.CrossFadeAlpha(1, phaseOneDuration / longestListCount, true);
                            }
                        }
                    }

                }
                else//If we are exiting the join menu
                {
                    if (percent > .6f)
                    {
                        mainMenuObj.SetActive(true);
                        inLobbyLeaveIndex = 1;
                        
                        float width = Mathf.Lerp(mainMenuStartingRect.width, canvasRect.width, phaseOnePercent);

                        Vector3 pos = mainMenuPanel.anchoredPosition;
                        pos.x = Mathf.Lerp(-mainMenuStartingRect.x, -canvasRect.x, phaseOnePercent);

                        mainMenuPanel.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
                        mainMenuPanel.anchoredPosition = pos;

                        for (int i = 0; i < mainListItems.Count; i++)//setting the phase two elements alpha to 0 so they do appear visible in phase one
                        {
                            if (mainListItems[i].image != null)
                            {
                                mainListItems[i].image.canvasRenderer.SetAlpha(0);
                            }

                            if (mainListItems[i].textObj != null)
                            {
                                mainListItems[i].textObj.canvasRenderer.SetAlpha(0);
                            }

                            if (mainListItems[i].button != null)
                            {

                                mainListItems[i].button.image.canvasRenderer.SetAlpha(0);
                            }
                        }

                        for (int i = 0; i < joinListItems.Count; i++)
                        {
                            if (joinListItems[i].image != null)
                            {
                                joinListItems[i].image.CrossFadeAlpha(0, phaseOneDuration / longestListCount, true);
                            }

                            if (joinListItems[i].textObj != null)
                            {
                                joinListItems[i].textObj.CrossFadeAlpha(0, phaseOneDuration / longestListCount, true);
                            }

                            if (joinListItems[i].button != null)
                            {

                                joinListItems[i].button.image.CrossFadeAlpha(0, phaseOneDuration / longestListCount, true);
                            }
                        }
                        closeButtonObj.SetActive(true);
                        closeButton.image.canvasRenderer.SetAlpha(0);
                        closeButtonText.canvasRenderer.SetAlpha(0);


                    }
                    else//phase two
                    {
                        joinMenuObj.SetActive(false);
                        closeButton.image.CrossFadeAlpha(1, phaseOneDuration / longestListCount, true);
                        closeButtonText.CrossFadeAlpha(1, phaseOneDuration / longestListCount, true);
                        int currentElement = Mathf.CeilToInt((1 - phaseTwoPercent) * mainListItems.Count) - 1;

                        if (playMenuPhaseTwoCurrentElementIteration < currentElement)
                        {
                            if (mainListItems[currentElement].image != null)
                            {


                                mainListItems[currentElement].image.CrossFadeAlpha(1, phaseOneDuration / longestListCount, true);
                            }
                            if (mainListItems[currentElement].textObj != null)
                            {

                                mainListItems[currentElement].textObj.CrossFadeAlpha(1, phaseOneDuration / longestListCount, true);
                            }
                            if (mainListItems[currentElement].button != null)
                            {

                                mainListItems[currentElement].button.image.CrossFadeAlpha(1, phaseOneDuration / longestListCount, true);
                            }


                            playMenuPhaseTwoCurrentElementIteration = currentElement;
                        }
                    }
                }
            }
            else
            {
                joinMenuStateChanging = false;
                FreezeButtons(false);
                if (joinMenuActivating)
                {
                    closeButtonObj.SetActive(false);
                }
            }
        }
    }

    void ChangeCreateMenuState()
    {
        if (createMenuStateChanging)
        {
            if (currentPlayTransitionTime > 0)
            {
                currentPlayTransitionTime -= Time.deltaTime;
                float percent = Mathf.Clamp01(currentPlayTransitionTime / playTransitionDuration);
                float phaseOneDuration = playTransitionDuration / 2;
                float phaseOnePercent = Mathf.Clamp01(Mathf.Lerp(-2, 1, percent));//change the -2 either higher or lower to adjust the speed of phase one
                float phaseTwoPercent = (phaseOnePercent > 0) ? 1 : Mathf.Clamp01(Mathf.Lerp(0, 2, percent));

                if (createMenuActivating)
                {
                    //phase one: dissapear play menu buttons and expand the panel
                    inLobbyReturnIndex = 2;
                    returnToJoin = false;
                    
                    inLobbyLeaveIndex = 4;
                    if (percent > .6f)
                    {
                        createMenuObj.SetActive(true);
                        float width = Mathf.Lerp(canvasRect.width, mainMenuStartingRect.width, phaseOnePercent);

                        Vector3 pos = mainMenuPanel.anchoredPosition;
                        pos.x = Mathf.Lerp(-canvasRect.x, -mainMenuStartingRect.x, phaseOnePercent);

                        mainMenuPanel.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
                        mainMenuPanel.anchoredPosition = pos;

                        for (int i = 0; i < createListItems.Count; i++)//setting the phase two elements alpha to 0 so they do appear visible in phase one
                        {
                            if (createListItems[i].image != null)
                            {
                                createListItems[i].image.canvasRenderer.SetAlpha(0);
                            }

                            if (createListItems[i].textObj != null)
                            {
                                createListItems[i].textObj.canvasRenderer.SetAlpha(0);
                            }

                            if (createListItems[i].button != null)
                            {

                                createListItems[i].button.image.canvasRenderer.SetAlpha(0);
                            }
                        }

                        for (int i = 0; i < playListItems.Count; i++)
                        {
                            if (playListItems[i].image != null)
                            {
                                playListItems[i].image.CrossFadeAlpha(0, phaseOneDuration / longestListCount, true);
                            }

                            if (playListItems[i].textObj != null)
                            {
                                playListItems[i].textObj.CrossFadeAlpha(0, phaseOneDuration / longestListCount, true);
                            }

                            if (playListItems[i].button != null)
                            {

                                playListItems[i].button.image.CrossFadeAlpha(0, phaseOneDuration / longestListCount, true);
                            }
                        }

                        closeButton.image.CrossFadeAlpha(0, phaseOneDuration / longestListCount, true);
                        closeButtonText.CrossFadeAlpha(0, phaseOneDuration / longestListCount, true);

                    }
                    else//phase two
                    {
                        playMenuObj.SetActive(false);
                        for (int i = 0; i < createListItems.Count; i++)
                        {
                            if (createListItems[i].image != null)
                            {
                                createListItems[i].image.CrossFadeAlpha(1, phaseOneDuration / longestListCount, true);
                            }

                            if (createListItems[i].textObj != null)
                            {
                                createListItems[i].textObj.CrossFadeAlpha(1, phaseOneDuration / longestListCount, true);
                            }

                            if (createListItems[i].button != null)
                            {

                                createListItems[i].button.image.CrossFadeAlpha(1, phaseOneDuration / longestListCount, true);
                            }
                        }
                    }
                }
                else
                {
                    if (percent > .6f)
                    {
                        inLobbyLeaveIndex = 1;
                        
                        mainMenuObj.SetActive(true);
                        float width = Mathf.Lerp(mainMenuStartingRect.width, canvasRect.width, phaseOnePercent);

                        Vector3 pos = mainMenuPanel.anchoredPosition;
                        pos.x = Mathf.Lerp(-mainMenuStartingRect.x, -canvasRect.x, phaseOnePercent);

                        mainMenuPanel.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
                        mainMenuPanel.anchoredPosition = pos;

                        for (int i = 0; i < mainListItems.Count; i++)//setting the phase two elements alpha to 0 so they do appear visible in phase one
                        {
                            if (mainListItems[i].image != null)
                            {
                                mainListItems[i].image.canvasRenderer.SetAlpha(0);
                            }

                            if (mainListItems[i].textObj != null)
                            {
                                mainListItems[i].textObj.canvasRenderer.SetAlpha(0);
                            }

                            if (mainListItems[i].button != null)
                            {

                                mainListItems[i].button.image.canvasRenderer.SetAlpha(0);
                            }
                        }

                        for (int i = 0; i < createListItems.Count; i++)
                        {
                            if (createListItems[i].image != null)
                            {
                                createListItems[i].image.CrossFadeAlpha(0, phaseOneDuration / longestListCount, true);
                            }

                            if (createListItems[i].textObj != null)
                            {
                                createListItems[i].textObj.CrossFadeAlpha(0, phaseOneDuration / longestListCount, true);
                            }

                            if (createListItems[i].button != null)
                            {

                                createListItems[i].button.image.CrossFadeAlpha(0, phaseOneDuration / longestListCount, true);
                            }
                        }
                        closeButtonObj.SetActive(true);
                        closeButton.image.canvasRenderer.SetAlpha(0);
                        closeButtonText.canvasRenderer.SetAlpha(0);


                    }
                    else//phase two
                    {
                        createMenuObj.SetActive(false);
                        closeButton.image.CrossFadeAlpha(1, phaseOneDuration / longestListCount, true);
                        closeButtonText.CrossFadeAlpha(1, phaseOneDuration / longestListCount, true);
                        int currentElement = Mathf.CeilToInt((1 - phaseTwoPercent) * mainListItems.Count) - 1;

                        if (playMenuPhaseTwoCurrentElementIteration < currentElement)
                        {
                            if (mainListItems[currentElement].image != null)
                            {


                                mainListItems[currentElement].image.CrossFadeAlpha(1, phaseOneDuration / longestListCount, true);
                            }
                            if (mainListItems[currentElement].textObj != null)
                            {

                                mainListItems[currentElement].textObj.CrossFadeAlpha(1, phaseOneDuration / longestListCount, true);
                            }
                            if (mainListItems[currentElement].button != null)
                            {

                                mainListItems[currentElement].button.image.CrossFadeAlpha(1, phaseOneDuration / longestListCount, true);
                            }


                            playMenuPhaseTwoCurrentElementIteration = currentElement;
                        }
                    }
                }
            }
            else
            {
                createMenuStateChanging = false;
                FreezeButtons(false);
                if (createMenuActivating)
                {
                    closeButtonObj.SetActive(false);
                }
            }
        }
    }

    void ChangeInLobbyMenuState()
    {
        if (inLobbyMenuStateChanging)
        {
            if (currentPlayTransitionTime > 0)
            {
                currentPlayTransitionTime -= Time.deltaTime;
                float percent = Mathf.Clamp01(currentPlayTransitionTime / playTransitionDuration);
                float phaseOneDuration = playTransitionDuration / 2;
                float phaseOnePercent = Mathf.Clamp01(Mathf.Lerp(-2, 1, percent));//change the -2 either higher or lower to adjust the speed of phase one
                float phaseTwoPercent = (phaseOnePercent > 0) ? 1 : Mathf.Clamp01(Mathf.Lerp(0, 2, percent));
                
                if (inLobbyMenuActivating)//going to inLobby
                {
                    inLobby = true;
                    inLobbyLeaveIndex = 5;
                    switch (inLobbyReturnIndex)
                    {
                        //for join->inLobby
                        case 1:
                            {
                                if (percent > .6f)
                                {
                                    inLobbyMenuObj.SetActive(true);

                                    for (int i = 0; i < inLobbyListItems.Count; i++)
                                    {
                                        if (inLobbyListItems[i].image != null)
                                        {
                                            inLobbyListItems[i].image.canvasRenderer.SetAlpha(0);

                                        }

                                        if (inLobbyListItems[i].textObj != null)
                                        {
                                            inLobbyListItems[i].textObj.canvasRenderer.SetAlpha(0);
                                        }

                                        if (inLobbyListItems[i].button != null)
                                        {

                                            inLobbyListItems[i].button.image.canvasRenderer.SetAlpha(0);

                                        }
                                    }


                                    for (int i = 0; i < joinListItems.Count; i++)
                                    {
                                        if (joinListItems[i].image != null)
                                        {
                                            joinListItems[i].image.CrossFadeAlpha(0, phaseOneDuration / longestListCount, true);
                                        }

                                        if (joinListItems[i].textObj != null)
                                        {
                                            joinListItems[i].textObj.CrossFadeAlpha(0, phaseOneDuration / longestListCount, true);
                                        }

                                        if (joinListItems[i].button != null)
                                        {

                                            joinListItems[i].button.image.CrossFadeAlpha(0, phaseOneDuration / longestListCount, true);
                                        }
                                    }
                                }
                                else
                                {
                                    joinMenuObj.SetActive(false);
                                    for (int i = 0; i < inLobbyListItems.Count; i++)
                                    {
                                        if (inLobbyListItems[i].image != null)
                                        {
                                            inLobbyListItems[i].image.CrossFadeAlpha(1, phaseOneDuration / longestListCount, true);
                                        }

                                        if (inLobbyListItems[i].textObj != null)
                                        {
                                            inLobbyListItems[i].textObj.CrossFadeAlpha(1, phaseOneDuration / longestListCount, true);
                                        }

                                        if (inLobbyListItems[i].button != null)
                                        {

                                            inLobbyListItems[i].button.image.CrossFadeAlpha(1, phaseOneDuration / longestListCount, true);

                                        }
                                    }
                                }
                                break;
                            }
                        //for create->inLobby
                        case 2:
                            {
                                if (percent > .6f)
                                {
                                    inLobbyMenuObj.SetActive(true);

                                    for (int i = 0; i < inLobbyListItems.Count; i++)
                                    {
                                        if (inLobbyListItems[i].image != null)
                                        {
                                            inLobbyListItems[i].image.canvasRenderer.SetAlpha(0);
                                        }

                                        if (inLobbyListItems[i].textObj != null)
                                        {
                                            inLobbyListItems[i].textObj.canvasRenderer.SetAlpha(0);
                                        }

                                        if (inLobbyListItems[i].button != null)
                                        {

                                            inLobbyListItems[i].button.image.canvasRenderer.SetAlpha(0);

                                        }
                                    }


                                    for (int i = 0; i < createListItems.Count; i++)
                                    {
                                        if (createListItems[i].image != null)
                                        {
                                            createListItems[i].image.CrossFadeAlpha(0, phaseOneDuration / longestListCount, true);
                                        }

                                        if (createListItems[i].textObj != null)
                                        {
                                            createListItems[i].textObj.CrossFadeAlpha(0, phaseOneDuration / longestListCount, true);
                                        }

                                        if (createListItems[i].button != null)
                                        {

                                            createListItems[i].button.image.CrossFadeAlpha(0, phaseOneDuration / longestListCount, true);
                                        }
                                    }
                                }
                                else
                                {
                                    createMenuObj.SetActive(false);
                                    for (int i = 0; i < inLobbyListItems.Count; i++)
                                    {
                                        if (inLobbyListItems[i].image != null)
                                        {
                                            inLobbyListItems[i].image.CrossFadeAlpha(1, phaseOneDuration / longestListCount, true);
                                        }

                                        if (inLobbyListItems[i].textObj != null)
                                        {
                                            inLobbyListItems[i].textObj.CrossFadeAlpha(1, phaseOneDuration / longestListCount, true);
                                        }

                                        if (inLobbyListItems[i].button != null)
                                        {

                                            inLobbyListItems[i].button.image.CrossFadeAlpha(1, phaseOneDuration / longestListCount, true);

                                        }
                                    }
                                }
                                break;
                            }
                        //for mainMenu(clicked play while in lobby->inLobby)
                        case 3:
                            {
                                if (percent > .6f)
                                {
                                    inLobbyMenuObj.SetActive(true);
                                    float width = Mathf.Lerp(canvasRect.width, mainMenuStartingRect.width, phaseOnePercent);

                                    Vector3 pos = mainMenuPanel.anchoredPosition;
                                    pos.x = Mathf.Lerp(-canvasRect.x, -mainMenuStartingRect.x, phaseOnePercent);

                                    mainMenuPanel.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
                                    mainMenuPanel.anchoredPosition = pos;

                                    for (int i = 0; i < inLobbyListItems.Count; i++)//setting the phase two elements alpha to 0 so they do appear visible in phase one
                                    {
                                        if (inLobbyListItems[i].image != null)
                                        {
                                            inLobbyListItems[i].image.canvasRenderer.SetAlpha(0);
                                        }

                                        if (inLobbyListItems[i].textObj != null)
                                        {
                                            inLobbyListItems[i].textObj.canvasRenderer.SetAlpha(0);
                                        }

                                        if (inLobbyListItems[i].button != null)
                                        {

                                            inLobbyListItems[i].button.image.canvasRenderer.SetAlpha(0);
                                        }
                                    }

                                    for (int i = 0; i < mainListItems.Count; i++)
                                    {
                                        if (mainListItems[i].image != null)
                                        {
                                            mainListItems[i].image.CrossFadeAlpha(0, phaseOneDuration / longestListCount, true);
                                        }

                                        if (mainListItems[i].textObj != null)
                                        {
                                            mainListItems[i].textObj.CrossFadeAlpha(0, phaseOneDuration / longestListCount, true);
                                        }

                                        if (mainListItems[i].button != null)
                                        {

                                            mainListItems[i].button.image.CrossFadeAlpha(0, phaseOneDuration / longestListCount, true);
                                        }
                                    }

                                    closeButton.image.CrossFadeAlpha(0, phaseOneDuration / longestListCount, true);
                                    closeButtonText.CrossFadeAlpha(0, phaseOneDuration / longestListCount, true);
                                }
                                else
                                {
                                    mainMenuObj.SetActive(false);
                                    for (int i = 0; i < inLobbyListItems.Count; i++)
                                    {
                                        if (inLobbyListItems[i].image != null)
                                        {
                                            inLobbyListItems[i].image.CrossFadeAlpha(1, phaseOneDuration / longestListCount, true);
                                        }

                                        if (inLobbyListItems[i].textObj != null)
                                        {
                                            inLobbyListItems[i].textObj.CrossFadeAlpha(1, phaseOneDuration / longestListCount, true);
                                        }

                                        if (inLobbyListItems[i].button != null)
                                        {

                                            inLobbyListItems[i].button.image.CrossFadeAlpha(1, phaseOneDuration / longestListCount, true);

                                        }
                                    }
                                }
                                break;
                            }
                      //for play (quickmatch)->inLobby
                        case 4:
                            {
                                if (percent > .6f)
                                {
                                    inLobbyMenuObj.SetActive(true);
                                    float width = Mathf.Lerp(canvasRect.width, mainMenuStartingRect.width, phaseOnePercent);

                                    Vector3 pos = mainMenuPanel.anchoredPosition;
                                    pos.x = Mathf.Lerp(-canvasRect.x, -mainMenuStartingRect.x, phaseOnePercent);

                                    mainMenuPanel.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
                                    mainMenuPanel.anchoredPosition = pos;

                                    for (int i = 0; i < inLobbyListItems.Count; i++)//setting the phase two elements alpha to 0 so they do appear visible in phase one
                                    {
                                        if (inLobbyListItems[i].image != null)
                                        {
                                            inLobbyListItems[i].image.canvasRenderer.SetAlpha(0);
                                        }

                                        if (inLobbyListItems[i].textObj != null)
                                        {
                                            inLobbyListItems[i].textObj.canvasRenderer.SetAlpha(0);
                                        }

                                        if (inLobbyListItems[i].button != null)
                                        {

                                            inLobbyListItems[i].button.image.canvasRenderer.SetAlpha(0);
                                        }
                                    }

                                    for (int i = 0; i < playListItems.Count; i++)
                                    {
                                        if (playListItems[i].image != null)
                                        {
                                            playListItems[i].image.CrossFadeAlpha(0, phaseOneDuration / longestListCount, true);
                                        }

                                        if (playListItems[i].textObj != null)
                                        {
                                            playListItems[i].textObj.CrossFadeAlpha(0, phaseOneDuration / longestListCount, true);
                                        }

                                        if (playListItems[i].button != null)
                                        {

                                            playListItems[i].button.image.CrossFadeAlpha(0, phaseOneDuration / longestListCount, true);
                                        }
                                    }

                                    closeButton.image.CrossFadeAlpha(0, phaseOneDuration / longestListCount, true);
                                    closeButtonText.CrossFadeAlpha(0, phaseOneDuration / longestListCount, true);
                                }
                                else
                                {
                                    playMenuObj.SetActive(false);
                                    closeButtonObj.SetActive(false);
                                    for (int i = 0; i < inLobbyListItems.Count; i++)
                                    {
                                        if (inLobbyListItems[i].image != null)
                                        {
                                            inLobbyListItems[i].image.CrossFadeAlpha(1, phaseOneDuration / longestListCount, true);
                                        }

                                        if (inLobbyListItems[i].textObj != null)
                                        {
                                            inLobbyListItems[i].textObj.CrossFadeAlpha(1, phaseOneDuration / longestListCount, true);
                                        }

                                        if (inLobbyListItems[i].button != null)
                                        {

                                            inLobbyListItems[i].button.image.CrossFadeAlpha(1, phaseOneDuration / longestListCount, true);

                                        }
                                    }
                                }
                                break;
                            }
                    }
                }
                else
                {

                    //Need to add (inside all cases except "leave room" case) inLobbyNoMenu activating transition (any->inLobby)
                    switch (inLobbyReturnIndex)//going from inLobby
                    {
                        //for inLobby->join
                        case 1:
                            {
                                inLobbyLeaveIndex = 3;
                                if (percent > .6f)
                                {
                                    inLobby = false;
                                    joinMenuObj.SetActive(true);

                                    for (int i = 0; i < joinListItems.Count; i++)
                                    {
                                        if (joinListItems[i].image != null)
                                        {
                                            joinListItems[i].image.canvasRenderer.SetAlpha(0);
                                        }

                                        if (joinListItems[i].textObj != null)
                                        {
                                            joinListItems[i].textObj.canvasRenderer.SetAlpha(0);
                                        }

                                        if (joinListItems[i].button != null)
                                        {

                                            joinListItems[i].button.image.canvasRenderer.SetAlpha(0);

                                        }
                                    }


                                    for (int i = 0; i < inLobbyListItems.Count; i++)
                                    {
                                        if (inLobbyListItems[i].image != null)
                                        {
                                            inLobbyListItems[i].image.CrossFadeAlpha(0, phaseOneDuration / longestListCount, true);
                                        }

                                        if (inLobbyListItems[i].textObj != null)
                                        {
                                            inLobbyListItems[i].textObj.CrossFadeAlpha(0, phaseOneDuration / longestListCount, true);
                                        }

                                        if (inLobbyListItems[i].button != null)
                                        {

                                            inLobbyListItems[i].button.image.CrossFadeAlpha(0, phaseOneDuration / longestListCount, true);
                                        }
                                    }
                                }
                                else
                                {
                                    inLobbyMenuObj.SetActive(false);
                                    for (int i = 0; i < joinListItems.Count; i++)
                                    {
                                        if (joinListItems[i].image != null)
                                        {
                                            joinListItems[i].image.CrossFadeAlpha(1, phaseOneDuration / longestListCount, true);
                                        }

                                        if (joinListItems[i].textObj != null)
                                        {
                                            joinListItems[i].textObj.CrossFadeAlpha(1, phaseOneDuration / longestListCount, true);
                                        }

                                        if (joinListItems[i].button != null)
                                        {

                                            joinListItems[i].button.image.CrossFadeAlpha(1, phaseOneDuration / longestListCount, true);

                                        }
                                    }
                                }
                                break;
                            }
                        //for inLobby->create
                        case 2:
                            {
                                inLobbyLeaveIndex = 4;
                                if (percent > .6f)
                                {
                                    inLobby = false;
                                    createMenuObj.SetActive(true);

                                    for (int i = 0; i < createListItems.Count; i++)
                                    {
                                        if (createListItems[i].image != null)
                                        {
                                            createListItems[i].image.canvasRenderer.SetAlpha(0);
                                        }

                                        if (createListItems[i].textObj != null)
                                        {
                                            createListItems[i].textObj.canvasRenderer.SetAlpha(0);
                                        }

                                        if (createListItems[i].button != null)
                                        {

                                            createListItems[i].button.image.canvasRenderer.SetAlpha(0);
                                        }
                                    }


                                    for (int i = 0; i < inLobbyListItems.Count; i++)
                                    {
                                        if (inLobbyListItems[i].image != null)
                                        {
                                            inLobbyListItems[i].image.CrossFadeAlpha(0, phaseOneDuration / longestListCount, true);
                                        }

                                        if (inLobbyListItems[i].textObj != null)
                                        {
                                            inLobbyListItems[i].textObj.CrossFadeAlpha(0, phaseOneDuration / longestListCount, true);
                                        }

                                        if (inLobbyListItems[i].button != null)
                                        {

                                            inLobbyListItems[i].button.image.CrossFadeAlpha(0, phaseOneDuration / longestListCount, true);
                                        }
                                    }
                                }
                                else
                                {
                                    inLobbyMenuObj.SetActive(false);
                                    for (int i = 0; i < createListItems.Count; i++)
                                    {
                                        if (createListItems[i].image != null)
                                        {
                                            createListItems[i].image.CrossFadeAlpha(1, phaseOneDuration / longestListCount, true);
                                        }

                                        if (createListItems[i].textObj != null)
                                        {
                                            createListItems[i].textObj.CrossFadeAlpha(1, phaseOneDuration / longestListCount, true);
                                        }

                                        if (createListItems[i].button != null)
                                        {

                                            createListItems[i].button.image.CrossFadeAlpha(1, phaseOneDuration / longestListCount, true);
                                        }
                                    }
                                }
                                break;
                            }
                        //for inLobby->mainMenu
                        case 3:
                        case 4:
                            {
                                if(inLobbyReturnIndex==4)
                                {
                                    inLobby = false;
                                }
                                inLobbyLeaveIndex = 1;
                                if(inLobby)
                                {
                                    inLobbyNoMenuActivating = true;
                                }

                                if (percent > .6f)
                                {
                                    mainMenuObj.SetActive(true);

                                    float width = Mathf.Lerp(mainMenuStartingRect.width, canvasRect.width, phaseOnePercent);

                                    Vector3 pos = mainMenuPanel.anchoredPosition;
                                    pos.x = Mathf.Lerp(-mainMenuStartingRect.x, -canvasRect.x, phaseOnePercent);

                                    mainMenuPanel.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
                                    mainMenuPanel.anchoredPosition = pos;

                                    for (int i = 0; i < mainListItems.Count; i++)
                                    {
                                        if (mainListItems[i].image != null)
                                        {
                                            mainListItems[i].image.canvasRenderer.SetAlpha(0);
                                        }

                                        if (mainListItems[i].textObj != null)
                                        {
                                            mainListItems[i].textObj.canvasRenderer.SetAlpha(0);
                                        }

                                        if (mainListItems[i].button != null)
                                        {

                                            mainListItems[i].button.image.canvasRenderer.SetAlpha(0);
                                        }
                                    }


                                    for (int i = 0; i < inLobbyListItems.Count; i++)
                                    {
                                        if (inLobbyListItems[i].image != null)
                                        {
                                            inLobbyListItems[i].image.CrossFadeAlpha(0, phaseOneDuration / longestListCount, true);
                                        }

                                        if (inLobbyListItems[i].textObj != null)
                                        {
                                            inLobbyListItems[i].textObj.CrossFadeAlpha(0, phaseOneDuration / longestListCount, true);
                                        }

                                        if (inLobbyListItems[i].button != null)
                                        {

                                            inLobbyListItems[i].button.image.CrossFadeAlpha(0, phaseOneDuration / longestListCount, true);
                                        }
                                    }
                                }
                                else
                                {
                                    inLobbyMenuObj.SetActive(false);
                                    closeButtonObj.SetActive(true);

                                    int currentElement = Mathf.CeilToInt((1 - phaseTwoPercent) * mainListItems.Count) - 1;

                                    if (playMenuPhaseTwoCurrentElementIteration < currentElement)
                                    {
                                        if (mainListItems[currentElement].image != null)
                                        {


                                            mainListItems[currentElement].image.CrossFadeAlpha(1, phaseOneDuration / longestListCount, true);
                                        }
                                        if (mainListItems[currentElement].textObj != null)
                                        {

                                            mainListItems[currentElement].textObj.CrossFadeAlpha(1, phaseOneDuration / longestListCount, true);
                                        }
                                        if (mainListItems[currentElement].button != null)
                                        {

                                            mainListItems[currentElement].button.image.CrossFadeAlpha(1, phaseOneDuration / longestListCount, true);
                                        }


                                        playMenuPhaseTwoCurrentElementIteration = currentElement;
                                    }
                                }
                                break;
                            }
                    }
                }


            }
            else
            {
                inLobbyMenuStateChanging = false;
                FreezeButtons(false);

                if (inLobbyReturnIndex == 1 || inLobbyReturnIndex == 2)
                {
                    closeButtonObj.SetActive(false);
                }

            }
        }
    }

    void ChangeInLobbyNoMenuState()
    {
        //activating the banner regardless of transition state. only inLobby state
        if (inLobby)
        {
            inLobbyNoMenuObj.SetActive(true);
            inLobbyNoMenuPanelObj.SetActive(true);
            if (!inLobbyNoMenuActivating)
            {
                for (int i = 0; i < inLobbyNoMenuListItems.Count; i++)
                {
                    if (inLobbyNoMenuListItems[i].image != null)
                    {
                        inLobbyNoMenuListItems[i].image.gameObject.SetActive(false);
                    }

                    if (inLobbyNoMenuListItems[i].textObj != null)
                    {
                        if (inLobbyNoMenuListItems[i].textObj.gameObject.name != "Title")
                        {
                            inLobbyNoMenuListItems[i].textObj.gameObject.SetActive(false);
                        }
                    }

                    if (inLobbyNoMenuListItems[i].button != null)
                    {

                        inLobbyNoMenuListItems[i].button.image.gameObject.SetActive(false);
                    }
                }
            }
            else
            {
                for (int i = 0; i < inLobbyNoMenuListItems.Count; i++)
                {
                    if (inLobbyNoMenuListItems[i].image != null)
                    {
                        inLobbyNoMenuListItems[i].image.gameObject.SetActive(true);
                    }

                    if (inLobbyNoMenuListItems[i].textObj != null)
                    {
                        inLobbyNoMenuListItems[i].textObj.gameObject.SetActive(true);
                    }

                    if (inLobbyNoMenuListItems[i].button != null)
                    {

                        inLobbyNoMenuListItems[i].button.image.gameObject.SetActive(true);
                    }
                }
            }
        }
        else
        {
            inLobbyNoMenuObj.SetActive(false);
            inLobbyNoMenuPanelObj.SetActive(false);
        }

        if (inLobbyNoMenuStateChanging)
        {
            currentPlayTransitionTime -= Time.deltaTime;
            float percent = Mathf.Clamp01(currentPlayTransitionTime / playTransitionDuration);
            float phaseOneDuration = playTransitionDuration / 2;
            float phaseOnePercent = Mathf.Clamp01(Mathf.Lerp(-2, 1, percent));//change the -2 either higher or lower to adjust the speed of phase one
            float phaseTwoPercent = (phaseOnePercent > 0) ? 1 : Mathf.Clamp01(Mathf.Lerp(0, 2, percent));

            if (currentPlayTransitionTime > 0)
            {

                switch (inLobbyLeaveIndex)
                {//NoMenu-> InLobby
                    case 0:
                        {
                        inLobbyNoMenuActivating = false;
                            Debug.Log("test 0");
                            if (percent > .6f)
                            {
                                inLobbyMenuObj.SetActive(true);
                                float height = Mathf.Lerp(canvasRect.height, inLobbyNoMenuStartingRect.height, phaseOnePercent);
                                inLobbyNoMenuPanel.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);

                               Vector2 pos = inLobbyNoMenuPanel.anchoredPosition;
                                pos.y = Mathf.Lerp(canvasRect.y, 0, phaseOnePercent);
                                pos.y -= canvasRect.y;
                                inLobbyNoMenuPanel.anchoredPosition = pos;

                                //pulls up the NoMainMenu mask to reveal the "main menu" button
                                height = Mathf.Lerp(0,canvasRect.height, phaseOnePercent);

                                pos = noMenuUIPanel.anchoredPosition;
                                pos.y = Mathf.Lerp(canvasRect.y,0, phaseOnePercent);
                                pos.y -= canvasRect.y;
                                noMenuUIPanel.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
                                noMenuUIPanel.anchoredPosition = pos;




                                for (int i = 0; i < inLobbyListItems.Count; i++)//setting the phase two elements alpha to 0 so they do appear visible in phase one
                                {
                                    if (inLobbyListItems[i].image != null)
                                    {
                                        inLobbyListItems[i].image.canvasRenderer.SetAlpha(0);
                                    }

                                    if (inLobbyListItems[i].textObj != null)
                                    {
                                        inLobbyListItems[i].textObj.canvasRenderer.SetAlpha(0);
                                    }

                                    if (inLobbyListItems[i].button != null)
                                    {

                                        inLobbyListItems[i].button.image.canvasRenderer.SetAlpha(0);
                                    }
                                }

                                
                            }
                            else
                            {
                                noMenuUIObj.SetActive(false);
                                //resetting the "fake" panel
                                Vector2 pos = inLobbyNoMenuPanel.anchoredPosition;
                                pos.y = -canvasRect.y - 25;
                                inLobbyNoMenuPanel.anchoredPosition = pos;
                                inLobbyNoMenuPanel.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, inLobbyNoMenuStartingRect.height);

                                //resetting the "real" panel
                                pos = mainMenuPanel.anchoredPosition;
                                pos.y = mainMenuStartingRect.y;
                                pos.x = -canvasRect.x;
                                mainMenuPanel.anchoredPosition = pos;
                                float width = canvasRect.width;
                                mainMenuPanel.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);



                                for (int i = 0; i < inLobbyListItems.Count; i++)
                                {
                                    if (inLobbyListItems[i].image != null)
                                    {
                                        inLobbyListItems[i].image.CrossFadeAlpha(1, phaseOneDuration / longestListCount, true);
                                    }

                                    if (inLobbyListItems[i].textObj != null)
                                    {
                                        inLobbyListItems[i].textObj.CrossFadeAlpha(1, phaseOneDuration / longestListCount, true);
                                    }

                                    if (inLobbyListItems[i].button != null)
                                    {

                                        inLobbyListItems[i].button.image.CrossFadeAlpha(1, phaseOneDuration / longestListCount, true);
                                    }
                                }
                            }
                                break;
                        }
                    //mainMenu -> InLobby
                    case 1:
                        {
                            inLobbyNoMenuActivating = false;
                            Debug.Log("test 1");
                            if (percent > .6f)
                            {
                                inLobbyMenuObj.SetActive(true);
                                //drops down the panels
                                //drops down the mainMenuPanel
                                Vector3 pos = mainMenuPanel.anchoredPosition;
                                pos.y = Mathf.Lerp(mainMenuStartingRect.y*3, mainMenuStartingRect.y- (inLobbyNoMenuStartingRect.height/2), phaseOnePercent); 
                                mainMenuPanel.anchoredPosition = pos;

                                //drops down the "fake" inLobby panel
                                float height = Mathf.Lerp(canvasRect.height,inLobbyNoMenuStartingRect.height,phaseOnePercent);
                                inLobbyNoMenuPanel.SetSizeWithCurrentAnchors( RectTransform.Axis.Vertical,height);
                                
                                pos = inLobbyNoMenuPanel.anchoredPosition;
                                pos.y = Mathf.Lerp(canvasRect.y, 0, phaseOnePercent);
                                pos.y -= canvasRect.y;
                                inLobbyNoMenuPanel.anchoredPosition = pos;


                                for (int i = 0; i < mainListItemsPos.Count; i++)
                                {
                                    Vector2 itemPos = mainListItemsPos[i].startingPos;
                                   
                                    mainListItemsPos[i].rectTrans.position = itemPos;
                                }



                                for (int i = 0; i < inLobbyListItems.Count; i++)//setting the phase two elements alpha to 0 so they do appear visible in phase one
                                {
                                    if (inLobbyListItems[i].image != null)
                                    {
                                        inLobbyListItems[i].image.canvasRenderer.SetAlpha(0);
                                    }

                                    if (inLobbyListItems[i].textObj != null)
                                    {
                                        inLobbyListItems[i].textObj.canvasRenderer.SetAlpha(0);
                                    }

                                    if (inLobbyListItems[i].button != null)
                                    {

                                        inLobbyListItems[i].button.image.canvasRenderer.SetAlpha(0);
                                    }
                                }

                                for (int i = 0; i < mainListItems.Count; i++)
                                {
                                    if (mainListItems[i].image != null)
                                    {
                                        mainListItems[i].image.CrossFadeAlpha(0, phaseOneDuration / longestListCount, true);
                                    }

                                    if (mainListItems[i].textObj != null)
                                    {
                                        mainListItems[i].textObj.CrossFadeAlpha(0, phaseOneDuration / longestListCount, true);
                                    }

                                    if (mainListItems[i].button != null)
                                    {

                                        mainListItems[i].button.image.CrossFadeAlpha(0, phaseOneDuration / longestListCount, true);
                                    }
                                }

                                closeButton.image.CrossFadeAlpha(0, phaseOneDuration / longestListCount, true);
                                closeButtonText.CrossFadeAlpha(0, phaseOneDuration / longestListCount, true);

                            }
                            else//phase two
                            {
                                mainMenuObj.SetActive(false);
                                //resetting the "fake" panel
                                Vector2 pos = inLobbyNoMenuPanel.anchoredPosition;
                                pos.y = -canvasRect.y-25;
                                inLobbyNoMenuPanel.anchoredPosition = pos;
                                inLobbyNoMenuPanel.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, inLobbyNoMenuStartingRect.height);

                                //resetting the "real" panel
                                pos = mainMenuPanel.anchoredPosition;
                                pos.y = mainMenuStartingRect.y;
                                pos.x = -canvasRect.x;
                                mainMenuPanel.anchoredPosition = pos;
                                float width = canvasRect.width;
                                mainMenuPanel.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);



                                for (int i = 0; i < inLobbyListItems.Count; i++)
                                {
                                    if (inLobbyListItems[i].image != null)
                                    {
                                        inLobbyListItems[i].image.CrossFadeAlpha(1, phaseOneDuration / longestListCount, true);
                                    }

                                    if (inLobbyListItems[i].textObj != null)
                                    {
                                        inLobbyListItems[i].textObj.CrossFadeAlpha(1, phaseOneDuration / longestListCount, true);
                                    }

                                    if (inLobbyListItems[i].button != null)
                                    {

                                        inLobbyListItems[i].button.image.CrossFadeAlpha(1, phaseOneDuration / longestListCount, true);
                                    }
                                }
                            }
                            break;
                        }
                    //PlayMenu -> InLobby
                    case 2:
                        {
                            //should never happen




                            break;
                        }
                    //JoinMenu  -> InLobby
                    case 3:
                        {
                            //should never happen
                            inLobbyNoMenuActivating = false;
                            inLobbyNoMenuStateChanging = false;
                            Debug.Log("leaving JoinMenu->going to InLobby");
                            FreezeButtons(false);
                            break;
                        }
                    //CreateMenu  -> InLobby
                    case 4:
                        {
                            //should never happen
                            inLobbyNoMenuActivating = false;
                            inLobbyNoMenuStateChanging = false;
                            Debug.Log("leaving CreateMenu->going to InLobby");
                            FreezeButtons(false);
                            break;
                        }
                    //InLobby  -> NoInLobby
                    case 5:
                        {
                            inLobbyNoMenuActivating = true;
                            

                            if (percent > .6f)
                            {


                                inLobbyMenuObj.SetActive(true);
                                noMenuUIObj.SetActive(true);
                                //pulls up the panels
                                //dissapears the mainMenuPanel
                                Vector3 pos = mainMenuPanel.anchoredPosition;
                                pos.x = mainMenuStartingRect.x;
                                mainMenuPanel.anchoredPosition = pos;
                                mainMenuPanel.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, mainMenuStartingRect.width);

                                //pulls up the "fake" inLobby panel
                                float height = Mathf.Lerp(inLobbyNoMenuStartingRect.height,canvasRect.height, phaseOnePercent);
                                inLobbyNoMenuPanel.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);

                                pos = inLobbyNoMenuPanel.anchoredPosition;
                                pos.y = Mathf.Lerp(0,canvasRect.y, phaseOnePercent);
                                pos.y -= Mathf.Lerp(canvasRect.y+25, canvasRect.y, phaseOnePercent);
                                inLobbyNoMenuPanel.anchoredPosition = pos;

                                //pulls up the NoMainMenu mask to reveal the "main menu" button
                                height = Mathf.Lerp(canvasRect.height, 0,  phaseOnePercent);

                                pos = noMenuUIPanel.anchoredPosition;
                                pos.y = Mathf.Lerp(0,canvasRect.y, phaseOnePercent);
                                pos.y -= canvasRect.y;
                                noMenuUIPanel.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
                                noMenuUIPanel.anchoredPosition = pos;



                                for (int i = 0; i < inLobbyNoMenuListItems.Count; i++)//setting the phase two elements alpha to 0 so they do appear visible in phase one
                                {
                                    if (inLobbyNoMenuListItems[i].image != null)
                                    {
                                        inLobbyNoMenuListItems[i].image.canvasRenderer.SetAlpha(0);
                                    }

                                    if (inLobbyNoMenuListItems[i].textObj != null)
                                    {
                                        if (inLobbyNoMenuListItems[i].textObj.gameObject.name != "Title")
                                        {
                                            inLobbyNoMenuListItems[i].textObj.canvasRenderer.SetAlpha(0);
                                        }
                                    }

                                    if (inLobbyNoMenuListItems[i].button != null)
                                    {

                                        inLobbyNoMenuListItems[i].button.image.canvasRenderer.SetAlpha(0);
                                    }
                                }

                                for (int i = 0; i < inLobbyListItems.Count; i++)
                                {
                                    if (inLobbyListItems[i].image != null)
                                    {
                                        inLobbyListItems[i].image.CrossFadeAlpha(0, phaseOneDuration / longestListCount, true);
                                    }

                                    if (inLobbyListItems[i].textObj != null)
                                    {
                                        inLobbyListItems[i].textObj.CrossFadeAlpha(0, phaseOneDuration / longestListCount, true);
                                    }

                                    if (inLobbyListItems[i].button != null)
                                    {

                                        inLobbyListItems[i].button.image.CrossFadeAlpha(0, phaseOneDuration / longestListCount, true);
                                    }
                                }
                            }
                            else
                            {
                                inLobbyMenuObj.SetActive(false);
                                
                                for (int i = 0; i < inLobbyNoMenuListItems.Count; i++)
                                {
                                    if (inLobbyNoMenuListItems[i].image != null)
                                    {
                                        inLobbyNoMenuListItems[i].image.CrossFadeAlpha(1, phaseOneDuration / longestListCount, true);
                                    }

                                    if (inLobbyNoMenuListItems[i].textObj != null)
                                    {
                                        if (inLobbyNoMenuListItems[i].textObj.gameObject.name != "Title")
                                        {
                                            inLobbyNoMenuListItems[i].textObj.CrossFadeAlpha(1, phaseOneDuration / longestListCount, true);
                                            }
                                    }

                                    if (inLobbyNoMenuListItems[i].button != null)
                                    {

                                        inLobbyNoMenuListItems[i].button.image.CrossFadeAlpha(1, phaseOneDuration / longestListCount, true);
                                    }
                                }
                            }
                             
                            break;
                        }

                }


            }
            else
            {
                inLobbyNoMenuStateChanging = false;
                FreezeButtons(false);

                if(inLobbyLeaveIndex==1)
                {
                    closeButtonObj.SetActive(false);
                }
                if(inLobbyLeaveIndex==5||inLobbyLeaveIndex==0)
                {
                    inLobbyLeaveIndex = (inLobbyNoMenuActivating)?0:5;
                }
               

            }

        }
        
    }

    void UpdateElementStartingPos()//this updates the position of each element in case the screen resolution has changed since the last click
    {
        for (int i = 0; i < listItems.Count; i++)
        {
            MenuItemPosData data = listItems[i];
            data.startingPos = listItems[i].rectTrans.position;
            listItems[i] = data;
        }
    }
    void FreezeButtons(bool freeze)
    {
        if (freeze)
        {
            EventSystem.current.SetSelectedGameObject(noMenuUIObj);
            for (int i = 0; i < mainListItems.Count; i++)
            {
                if (mainListItems[i].button != null)
                {
                    mainListItems[i].button.interactable = false;
                }
            }
            for (int i = 0; i < playListItems.Count; i++)
            {
                if (playListItems[i].button != null)
                {
                    playListItems[i].button.interactable = false;
                }
            }
            for (int i = 0; i < joinListItems.Count; i++)
            {
                if (joinListItems[i].button != null)
                {
                    joinListItems[i].button.interactable = false;
                }
            }
            for (int i = 0; i < createListItems.Count; i++)
            {
                if (createListItems[i].button != null)
                {
                    createListItems[i].button.interactable = false;
                }
            }
            for (int i = 0; i < inLobbyListItems.Count; i++)
            {
                if (inLobbyListItems[i].button != null)
                {
                    inLobbyListItems[i].button.interactable = false;
                }
            }
            for (int i = 0; i < inLobbyNoMenuListItems.Count; i++)
            {
                if (inLobbyNoMenuListItems[i].button != null)
                {
                    inLobbyNoMenuListItems[i].button.interactable = false;
                }
            }


            closeButton.interactable = false;

        }
        else
        {
            for (int i = 0; i < mainListItems.Count; i++)
            {
                if (mainListItems[i].button != null)
                {
                    mainListItems[i].button.interactable = true;
                }
            }
            for (int i = 0; i < playListItems.Count; i++)
            {
                if (playListItems[i].button != null)
                {
                    playListItems[i].button.interactable = true;
                }
            }
            for (int i = 0; i < joinListItems.Count; i++)
            {
                if (joinListItems[i].button != null)
                {
                    joinListItems[i].button.interactable = true;
                }
            }
            for (int i = 0; i < createListItems.Count; i++)
            {
                if (createListItems[i].button != null)
                {
                    createListItems[i].button.interactable = true;
                }
            }
            for (int i = 0; i < inLobbyListItems.Count; i++)
            {
                if (inLobbyListItems[i].button != null)
                {
                    inLobbyListItems[i].button.interactable = true;
                }
            }
            for (int i = 0; i < inLobbyNoMenuListItems.Count; i++)
            {
                if (inLobbyNoMenuListItems[i].button != null)
                {
                    inLobbyNoMenuListItems[i].button.interactable = false;
                }
            }
            closeButton.interactable = true;
        }

    }




    [Serializable]
    [SerializeField]
    struct MenuItemPosData
    {
        public RectTransform rectTrans;
        [HideInInspector]
        public Vector3 startingPos;
    }

    [Serializable]
    [SerializeField]
    struct MenuItemFadeData
    {
        public Button button;
        public Image image;
        public Text textObj;
    }

}
