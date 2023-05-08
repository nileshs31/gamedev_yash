using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Puck : MonoBehaviour
{
    public ScoreUI scoreUIInstance;
    public static bool wasGoal { get; private set; }
    private Rigidbody2D rb2d;
    public float maxSpeed;
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
                StartCoroutine(ResetPuck());
            }
            else if(other.tag == "PlayerGoal")
            {
                scoreUIInstance.IncreaseScore(ScoreUI.Score.Player2Score);
                wasGoal = true;
                StartCoroutine(ResetPuck());
            }
        }
    }

    IEnumerator ResetPuck()
    {
        yield return new WaitForSecondsRealtime(0.5f);
        wasGoal = false;
        rb2d.velocity = rb2d.position = new Vector2(0,0);
        
    }

    private void FixedUpdate()
    {
        rb2d.velocity = Vector2.ClampMagnitude(rb2d.velocity, maxSpeed);
    }
    public void CenterPuck()
    {
        rb2d.position = new Vector2(0, 0);
    }
}
