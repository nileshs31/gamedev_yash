using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Puck : MonoBehaviour
{
    public ScoreUI scoreUIInstance;
    public static bool wasGoal { get; private set; }
    private Rigidbody2D rb2d;
    public float maxSpeed;
    public AudioManager audioManager;
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        wasGoal = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(!wasGoal)
        {
            if(other.tag == "Player2Goal")
            {
                scoreUIInstance.IncreaseScore(ScoreUI.Score.PlayerScore);
                wasGoal = true;
                audioManager.PuckGoal();
                StartCoroutine(ResetPuck(false));
            }
            else if(other.tag == "PlayerGoal")
            {
                scoreUIInstance.IncreaseScore(ScoreUI.Score.Player2Score);
                wasGoal = true;
                audioManager.PuckGoal();
                StartCoroutine(ResetPuck(true));
            }
        }
    }

    IEnumerator ResetPuck(bool didAIscore)
    {
        yield return new WaitForSecondsRealtime(0.5f);
        wasGoal = false;
        rb2d.velocity = rb2d.position = new Vector2(0,0);

        if(didAIscore )
        {
            rb2d.position = new Vector2(0, -1);
        }
        else
            rb2d.position = new Vector2(0, 1);

        
    }

    private void FixedUpdate()
    {
        rb2d.velocity = Vector2.ClampMagnitude(rb2d.velocity, maxSpeed);
    }
    public void CenterPuck()
    {
        rb2d.position = new Vector2(0, 0);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        audioManager.PuckCollision();
    }
}
