using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SaveName : MonoBehaviour
{
    // Start is called before the first frame update
    public int  hasNameBeenSaved = 1;
    [SerializeField] InputField nameText;
  
    public static string loadName;
    void Start()
    {
        // Check if the flag exists in PlayerPrefs
        if (PlayerPrefs.HasKey("HasNameBeenSaved"))
        {
            // If the flag exists, retrieve its value
            hasNameBeenSaved = PlayerPrefs.GetInt("HasNameBeenSaved", hasNameBeenSaved);
            loadName = PlayerPrefs.GetString("PlayerName", nameText.text);
        }

        // Hide the UI element if the name has been saved
        if (hasNameBeenSaved == 2)
        {

            gameObject.SetActive(false);

        }
        Debug.Log(nameText.text);
       
    }

    public void SavePlayerName()
    {
        if (hasNameBeenSaved != 2)
        {
            // Save the player name in PlayerPrefs or wherever you want to store it
            PlayerPrefs.SetString("PlayerName",nameText.text);
            PlayerPrefs.Save();
            // Hide the GameObject
            gameObject.SetActive(false);
            // Set the flag to true to indicate it has been saved
             PlayerPrefs.SetInt("HasNameBeenSaved", 2);
            LoadFromPP();
            // Hide or disable the UI element here
        }
    }

    public void LoadFromPP()
    {


        loadName = PlayerPrefs.GetString("PlayerName", nameText.text);
       

    }

}
