using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NotePad : MonoBehaviour
{
    public TMP_InputField NoteText;

    // Start is called before the first frame update
    void Start()
    {
        //Checks if there are any notes, if there is print them to NoteText
        if (PlayerPrefs.GetString("Notes") != null)
        {
            NoteText.text = PlayerPrefs.GetString("Notes");
        }
    }

    public void SafeAndReturn()
    {
        PlayerPrefs.SetString("Notes", NoteText.text);
        UIManager.instance.CloseNotePad();
    }
}
