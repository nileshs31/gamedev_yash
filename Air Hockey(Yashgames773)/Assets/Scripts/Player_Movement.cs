using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Movement : MonoBehaviour
{
    
    bool wasJustClicked = true; //Player Clicked the mouseButton
    bool canMove; //
    Vector2 playerSize; //for Size of the sprite 
    public Transform boundaryHolder; //It will get the Boundary Position of all the direction
    Rigidbody2D rb2d;

    Boundary playerBoundary;

    struct Boundary //Remembering the Position of boundaries
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
        playerSize = GetComponentInChildren<SpriteRenderer>().bounds.extents; 

        playerBoundary = new Boundary(boundaryHolder.GetChild(0).position.y, boundaryHolder.GetChild(1).position.y, //take the all the Boundaries position in their coordinates
                                      boundaryHolder.GetChild(2).position.x, boundaryHolder.GetChild(3).position.x);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition); //Transforms a mouse screen position  into world position

            if (wasJustClicked)
            {
                wasJustClicked = false; 

                if (mousePos.x > transform.position.x - playerSize.x && mousePos.x < transform.position.x + playerSize.x && //Only it will  move if the player is inside the puck 
               mousePos.y > transform.position.y - playerSize.y && mousePos.y < transform.position.y + playerSize.y)
                {
                    canMove = true;
                }
                else
                {
                    canMove = false;
                }
            }

            if (canMove)
            {
                Vector2 mouseClampedPos = new Vector2(Mathf.Clamp(mousePos.x, playerBoundary.Left, playerBoundary.Right), 
                                                      Mathf.Clamp(mousePos.y, playerBoundary.Down, playerBoundary.Up));
                rb2d.MovePosition(mouseClampedPos); 
            }
        }
        else
        {
            wasJustClicked = true;
        }
    }

}

