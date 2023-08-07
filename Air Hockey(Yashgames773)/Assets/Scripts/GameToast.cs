using UnityEngine;

public class GameToast : MonoBehaviour{
    // msg txt box
    [SerializeField] TMPro.TMP_Text msgBox;
    private void Start() {
    }
    public void invokeToast(string msg){
        msgBox.text = msg;
    }
}