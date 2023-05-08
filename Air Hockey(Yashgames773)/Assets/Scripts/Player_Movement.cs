using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Player_Movement : MonoBehaviour
{
   
    public Transform boundaryHolder; //It will get the Boundary Position of all the direction
    Rigidbody2D rb2d;
    Vector2 startingPosition;

    Boundary playerBoundary;
   public Collider2D playerCollider { get; private set; }
    public int? LockedFingerID { get; set; }
    public PlayerController Controller;

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
        Controller.Players.Add(this);
    }
    private void OnDisable()
    {
        Controller.Players.Remove(this);

    }



    public void MoveToPosition(Vector2 position)
    {
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

