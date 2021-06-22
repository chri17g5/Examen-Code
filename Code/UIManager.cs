using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    //---------------------------------------------------
    //  Script that keep track of UI elements
    //
    //  SetActive(bool) enabels or disabels an element
    //  
    //  Display and tabs:
    //      Display = Parent to tabs
    //      Tabs = children to Display
    //
    //  Also Holds Scene Change
    //  Also Holds some Regex
    //---------------------------------------------------

    [Header("Login/Register Display & Tabs")]
    public GameObject AuthDisplay;
    public GameObject LoginScreen;
    public GameObject RegisterScreen;

    [Header("MainCotent Display & Tabs")]
    public GameObject MainContentDisplay;

    public GameObject DefaultScreen;

    public GameObject SmallNavigation;
    public GameObject BigNavigation;
    public GameObject UserSettings;
    public GameObject InventoryScreen;
    public GameObject NotePadScreen;

    [Header("Elements with Regex")]
    public TMP_InputField MobileNum;

    //creation of UIManager instance
    public static UIManager instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != null)
        {
            Debug.Log("Instance already exists, destroy object!");
            Destroy(this);
        }
    }

    #region AuthUI & its tabs
    public void ToRegFromLogin()
    {
        LoginScreen.SetActive(false);
        RegisterScreen.SetActive(true);
    }

    public void ToLoginFromReg()
    {
        RegisterScreen.SetActive(false);
        LoginScreen.SetActive(true);
    }

    public void ToMainFromLogin()
    {
        AuthDisplay.SetActive(false);
        MainContentDisplay.SetActive(true);
        BackToDefaultScreen();
    }
    #endregion

    #region MainDisplay & its tabs
    public void BackToLogin()
    {
        MainContentDisplay.SetActive(false);
        AuthDisplay.SetActive(true);

        //Just in case anything should happend
        LoginScreen.SetActive(true);
        RegisterScreen.SetActive(false);
    }

    public void BackToDefaultScreen() //Resets the app to main and enabels smallnav
    {
        CloseAllTabs();
        SmallNavigation.SetActive(true);
        DefaultScreen.SetActive(true);
    }

    public void BigNav() //Controlls BigNavs Visability
    {
        //If its open close it
        if (BigNavigation.activeInHierarchy == true)
        {
            BigNavigation.SetActive(false);
        }
        //else open it
        else
        {
            BigNavigation.SetActive(true);
        }
    }

    public void OpenUserSettings()
    {
        CloseAllTabs();
        SmallNavigation.SetActive(false);
        UserSettings.SetActive(true);
    }

    public void CloseUserSettings()
    {
        UserSettings.SetActive(false);
        BackToDefaultScreen();
    }

    public void OpenInventory()
    {
        CloseAllTabs();
        SmallNavigation.SetActive(false);
        InventoryScreen.SetActive(true);
    }

    public void OpenNotePad()
    {
        CloseAllTabs();
        SmallNavigation.SetActive(false);
        NotePadScreen.SetActive(true);
    }
    public void CloseNotePad()
    {
        BackToDefaultScreen();
    }

    public void LoadNewScene()
    {
        SceneManager.LoadScene(1);
    }

    //---------------------------------------------------------
    //  Below are removals so we don't have sevral
    //  tabs overwrite each other
    //---------------------------------------------------------
    public void RemoveNav()
    {
        BigNavigation.SetActive(false);
        SmallNavigation.SetActive(false);
    }

    public void CloseAllTabs() //Caveman solution
    {
        DefaultScreen.SetActive(false);
        BigNavigation.SetActive(false);
        NotePadScreen.SetActive(false);
        InventoryScreen.SetActive(false);
    }
    #endregion

    #region Regex
    /*
     * Was supposed to be used but ended up not using it as TMPro has it build in.
    private void NumOnly()
    {
        Regex rgx = new Regex("^[0-9]");
    }

    private void LettersOnly()
    {
        Regex rgx = new Regex("^[A-Za-z]+$");
    }
    */
    #endregion
}
