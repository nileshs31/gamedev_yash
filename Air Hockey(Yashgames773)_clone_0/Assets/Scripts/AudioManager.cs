using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] AudioClip puckCollide;
    [SerializeField] AudioClip goal;
    AudioSource audioSource;
   [SerializeField] AudioSource bgAudioSource;
    [SerializeField] GameObject OnS;
    [SerializeField] GameObject OffS;
    private bool muted = false;


    void Start()
    {
        audioSource = GetComponent<AudioSource>();
       

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


    public void PuckCollision()
    {
        audioSource.PlayOneShot(puckCollide);
    }

    public void PuckGoal()
    {
        audioSource.PlayOneShot(goal);
    }

    //public void OffSound()
    //{
    //    OnS.SetActive(true);
    //    OffS.SetActive(false);
       
    //    AudioListener.pause = false;


    //}
    //public void OnSound()
    //{
    //    OffS.SetActive(true);
    //    OnS.SetActive(false);
    //    AudioListener.pause = true;



    //}
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
