using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PuckMultiplayer : MonoBehaviour
{
   
    public ScoreUIM scoreUIInstance;
    public static bool wasGoal { get; private set; }
    public Rigidbody2D rb2d;
  //  public float maxSpeed;
    public AudioManager audioManager;

    public float impulse = 4f;
    public float frictionCoffecient3d = 0.1f;
    public float gravity3d = 9.8f;
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        wasGoal = false;
        rb2d.position = new Vector2(0, 0);
       
    }
    private void FixedUpdate()
    {
        FrictionSimulate3d();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!wasGoal)
        {
            if (other.tag == "Player2Goal")
            {
                scoreUIInstance.IncreaseScore(ScoreUIM.Score.PlayerScore);
                wasGoal = true;
                audioManager.PuckGoal();
                StartCoroutine(ResetPuck(false));
            }
            else if (other.tag == "PlayerGoal")
            {
                scoreUIInstance.IncreaseScore(ScoreUIM.Score.Player2Score);
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
        rb2d.velocity = rb2d.position = new Vector2(0, 0);

        if (didAIscore)
        {
            rb2d.position = new Vector2(0, 0);
        }
        else
            rb2d.position = new Vector2(0, 0);


    }

   // private void FixedUpdate()
  //  {
     //   rb2d.velocity = Vector2.ClampMagnitude(rb2d.velocity, maxSpeed);
   // }
    public void CenterPuck()
    {
        rb2d.position = new Vector2(0, 0);
    }

   
    

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Vector2 resultantVector = new Vector2(0f, 0f);
        foreach (ContactPoint2D contact in collision.contacts)
        {
            Debug.Log(contact.point);
            //GetComponent<RectTransform>().anchoredPosition
            resultantVector += ((Vector2)transform.position - contact.point).normalized;
            //GetComponent<Rigidbody2D>().AddForce((GetComponent<RectTransform>().anchoredPosition - contact.point).normalized * force);
        }
        resultantVector += GetComponent<Rigidbody2D>().velocity.normalized;
        GetComponent<Rigidbody2D>().velocity = resultantVector * impulse;
        audioManager.PuckCollision();
    }

    public void FrictionSimulate3d()
    {
        Vector2 deceleration = -1 * GetComponent<Rigidbody2D>().velocity.normalized * frictionCoffecient3d * gravity3d;
        GetComponent<Rigidbody2D>().velocity += deceleration * Time.deltaTime;
    }
}

