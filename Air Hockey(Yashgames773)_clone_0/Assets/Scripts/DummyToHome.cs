using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DummyToHome : MonoBehaviour
{
    public AsyncOperation op;
    float prog;

    void Start()
    {
        StartCoroutine(LoadAs());
    }

    IEnumerator LoadAs()
    {
        op = SceneManager.LoadSceneAsync("MainMenu");
        op.allowSceneActivation = false;


        yield return new WaitForSeconds(3.2f);
        op.allowSceneActivation = true;

    }
}
