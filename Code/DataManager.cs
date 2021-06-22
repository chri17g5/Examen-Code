using Firebase;
using Firebase.Database; 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DataManager : MonoBehaviour
{
    DatabaseReference reference;
    public TMP_Text Highscore;
    public TMP_Text CurrentScore;

    // Start is called before the first frame update
    void Start()
    {
        reference = FirebaseDatabase.DefaultInstance.RootReference;
        LoadData();
    }

    public void SaveData()
    {
        //Saves data from CurrentScore.text to given path
        reference.Child("Users").Child("User 1").Child("Highscore").SetValueAsync(CurrentScore.text.ToString());
    }

    public void LoadData()
    {
        FirebaseDatabase.DefaultInstance.GetReference("Users").ValueChanged += DataManager_ValueChanged;    
    }

    private void DataManager_ValueChanged(object sender, ValueChangedEventArgs e)
    {
        Highscore.text = "HighScore: " + e.Snapshot.Child("User 1").Child("Highscore").GetValue(true).ToString();
    }
}

