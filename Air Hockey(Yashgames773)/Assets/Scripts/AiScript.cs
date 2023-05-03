using UnityEngine;
using static Player_Movement;

public class AiScript : MonoBehaviour
{
    public float maxMoveSpeed;
    private Rigidbody2D rb2D;
    private Vector2 startPos;

    public Transform player2BboundaryHolder;
    private Player_Movement.Boundary player2Boundary;

    public Rigidbody2D puckRb2d;
    public Transform puckBboundaryHolder;
    private Vector2 targetPos;
    private Player_Movement.Boundary puckBoundary;

    private bool isFirstTimeInOpponentsHalf = true;
    private float offsetXFromTarget;

    private void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();
        startPos = rb2D.position;

        player2Boundary = new Boundary(player2BboundaryHolder.GetChild(0).position.y, player2BboundaryHolder.GetChild(1).position.y, //take the all the Boundaries position in their coordinates
                                    player2BboundaryHolder.GetChild(2).position.x, player2BboundaryHolder.GetChild(3).position.x);


        puckBoundary = new Boundary(puckBboundaryHolder.GetChild(0).position.y, puckBboundaryHolder.GetChild(1).position.y, //take the all the Boundaries position in their coordinates
                                   puckBboundaryHolder.GetChild(2).position.x, puckBboundaryHolder.GetChild(3).position.x);
    }

    private void FixedUpdate()
    {
        if (!Puck.wasGoal)
        {
            float puckMoveSpeed;


            if (puckRb2d.position.y < puckBoundary.Down)
            {

                if (isFirstTimeInOpponentsHalf)
                {
                    isFirstTimeInOpponentsHalf = false;
                    offsetXFromTarget = Random.Range(-1f, 1f);
                }
                puckMoveSpeed = maxMoveSpeed * Random.Range(0.1f, 0.4f);


                targetPos = new Vector2(Mathf.Clamp(puckRb2d.position.x + offsetXFromTarget, puckBoundary.Left, puckBoundary.Right),
                                                    startPos.y);
            }

            else
            {
                puckMoveSpeed = Random.Range(maxMoveSpeed * 0.5f, maxMoveSpeed);
                targetPos = new Vector2(Mathf.Clamp(puckRb2d.position.x, puckBoundary.Left, puckBoundary.Right),
                                        Mathf.Clamp(puckRb2d.position.y, puckBoundary.Down, puckBoundary.Up));


            }
            rb2D.MovePosition(Vector2.MoveTowards(rb2D.position, targetPos, puckMoveSpeed * Time.fixedDeltaTime));
        }
    }

}

