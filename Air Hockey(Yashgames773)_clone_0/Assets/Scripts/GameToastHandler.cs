using UnityEngine;

public class GameToastHandler : MonoBehaviour{
    public static GameToastHandler Instance {get; set;}
    private void Awake() {
        DontDestroyOnLoad(this.gameObject);
        Instance = this;
    }

    [SerializeField] GameObject gameToastPanel;
    GameObject instance;
    // Queue<string> msgsQueue;
    // float timerMax = 2f;
    // float timer;

    // private void Start() {
    //     msgsQueue = new Queue<string>();
    //     timer = timerMax;
    // }

    // private void Update() {
    //     timer -= Time.deltaTime;
    //     if(timer < 0){
    //         timer = timerMax;
    //         if(msgsQueue.Count > 0 && instance == null){
    //             sendToast(msgsQueue.Dequeue());
    //         }
    //     }
    // }
    public void sendToast(string msg){
        if(instance == null){
            instance = Instantiate(gameToastPanel,GameObject.Find("Canvas").transform);
            instance.GetComponent<GameToast>().invokeToast(msg);
            Destroy(instance,2.1f);
        }
    }
    
}