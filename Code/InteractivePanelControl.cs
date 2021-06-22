using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

//-------------------------------------------------------------
//  Method is made to generate the same button several times,
//  but with diffrent values.
//-------------------------------------------------------------

public static class ButtonExtention
{
    //Keeps track of which button is which
    public static void AddEventListener<T>(this Button button, T param, Action<T> OnClick)
    {
        button.onClick.AddListener(delegate ()
        {
            OnClick(param);
        });
    }
}

public class InteractivePanelControl : MonoBehaviour
{
    [Serializable] 
    public struct Item
    {
        public string Title;
        public string Description;
        public Sprite Icon;
    }

    [SerializeField] Item[] AllItems;

    // Start is called before the first frame update
    void Start()
    {
        // buttonTemplate is child to panel parrent
        GameObject buttonTemplate = transform.GetChild(0).gameObject;
        GameObject g;

        int items = AllItems.Length;
        for (int i = 0; i < items; i++)
        {
            g = Instantiate(buttonTemplate, transform);

            // Title text
            g.transform.GetChild (0).GetComponent<TMP_Text>().text = AllItems[i].Title;
            // Description text
            g.transform.GetChild (1).GetComponent<TMP_Text>().text = AllItems[i].Description;
            // Img Sprite
            g.transform.GetChild (2).GetComponent<Image>().sprite = AllItems[i].Icon;

            g.GetComponent<Button>().AddEventListener(i, ItemClicked);
        }
        Destroy(buttonTemplate);
    }

    void ItemClicked(int itemIndex)
    {
        // Debug message that confirms item number when clicked
        Debug.Log("item " + itemIndex + " clicked");
    }
}
