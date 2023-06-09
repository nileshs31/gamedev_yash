using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGAUIDO : MonoBehaviour
{


    [SerializeField] GameObject OnS;
    [SerializeField] GameObject OffS;
    private bool muted = false;




    void Start()
    {
        if (!PlayerPrefs.HasKey("muted"))
        {
            PlayerPrefs.SetInt("muted", 0);
            Load();

        }
        else
        {
            Load();
        }
        UpdateButtonIcon();
        AudioListener.pause = muted;

    }

    public void OnButtonPress()
    {
        if (muted == false)
        {
            muted = true;
            AudioListener.pause = true;
        }
        else
        {
            muted = false;
            AudioListener.pause = false;
        }
        Save();
        UpdateButtonIcon();
    }

    private void UpdateButtonIcon()
    {
        if (muted == false)
        {
            OnS.SetActive(true);
            OffS.SetActive(false);
        }
        else
        {
            OnS.SetActive(false);
            OffS.SetActive(true);
        }

    }

    private void Load()
    {
        muted = PlayerPrefs.GetInt("muted") == 1;
    }

    private void Save()
    {
        PlayerPrefs.SetInt("muted", muted ? 1 : 0);

    }

}

