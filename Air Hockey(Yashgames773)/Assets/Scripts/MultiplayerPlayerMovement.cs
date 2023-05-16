using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class MultiplayerPlayerMovement : NetworkBehaviour
{


   public Transform boundaryHolder; //It will get the Boundary Position of all the direction
    public Rigidbody2D rb2d;
    Vector2 startingPosition;

    Boundary playerBoundary;
    public Collider2D playerCollider { get; private set; }
    public int? LockedFingerID { get; set; }
    
    public List<MultiplayerPlayerMovement> Players = new List<MultiplayerPlayerMovement>();

    public struct Boundary //Remembering the Position of boundaries
    {
        public float Up, Down, Left, Right;

        public Boundary(float up, float down, float left, float right)
        {
            Up = up; Down = down; Left = left; Right = right; //Declaring the min and max pos in mathf.clamp for restricting the position
        }
    }
   

    // Use this for initialization
    void Start()
    {

        rb2d = GetComponent<Rigidbody2D>();
        playerCollider = GetComponent<Collider2D>();
        startingPosition = rb2d.position;

        playerBoundary = new Boundary(boundaryHolder.GetChild(0).position.y, boundaryHolder.GetChild(1).position.y, //take the all the Boundaries position in their coordinates
                                      boundaryHolder.GetChild(2).position.x, boundaryHolder.GetChild(3).position.x);
    }
    private void OnEnable()
    {
      Players.Add(this);
    }
    private void OnDisable()
    {
      Players.Remove(this);

    }

    void Update()
    {
      //  if(!IsOwner) return;
        for (int i = 0; i < Input.touchCount; i++)
        {
            Vector2 touchWorldPos = Camera.main.ScreenToWorldPoint(Input.GetTouch(i).position);
            foreach (var player in Players)
            {
                if (player.LockedFingerID == null)
                {
                    if (Input.GetTouch(i).phase == TouchPhase.Began &&
                        player.playerCollider.OverlapPoint(touchWorldPos))
                    {
                        player.LockedFingerID = Input.GetTouch(i).fingerId;
                    }
                }
                else if (player.LockedFingerID == Input.GetTouch(i).fingerId)
                {
                    player.MoveToPosition(touchWorldPos);
                    if (Input.GetTouch(i).phase == TouchPhase.Ended ||
                        Input.GetTouch(i).phase == TouchPhase.Canceled)
                        player.LockedFingerID = null;
                }
            }
        }
    }

    public void MoveToPosition(Vector2 position)
    {
        Debug.Log("Code has reached to moveToPosition");
        Vector2 clampedMousePos = new Vector2(Mathf.Clamp(position.x, playerBoundary.Left,
                                                  playerBoundary.Right),
                                      Mathf.Clamp(position.y, playerBoundary.Down,
                                                  playerBoundary.Up));
        rb2d.MovePosition(clampedMousePos);
    }
    public void ResetPosition()
    {
        rb2d.position = startingPosition;
    }
}
