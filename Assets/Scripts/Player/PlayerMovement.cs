using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

    [HideInInspector]
    public int playerNumber = 1;
    [HideInInspector]
    public float Speed = 2;
    private string walkingTowards = "";
    [HideInInspector] public float kickSpeed = 500;
    public bool canKickBomb = false;

    private Rigidbody rigidBody;

    private string HorizontalName;
    private float HorizontalValue;

    private string VerticalName;
    private float VerticalValue;


	// Use this for initialization
	void Start () {
        rigidBody = GetComponent<Rigidbody>();

        HorizontalName = "Horizontal" + playerNumber;
        VerticalName = "Vertical" + playerNumber;
	}
	
    void Update()
    {
        walkingTowards = "";
        if (Mathf.Abs(VerticalValue) < 0.1f)
        {
            HorizontalValue = Input.GetAxis(HorizontalName);
        }
        //Priortize Horizontal Movement before Vertical => Only allow One Movement at a time
        if (Mathf.Abs(HorizontalValue) < 0.1f)
        {
            VerticalValue = Input.GetAxis(VerticalName);
        }

        
        if (Mathf.Abs(HorizontalValue) > Mathf.Abs(VerticalValue))
        {
            if (HorizontalValue < 0)
            {
                walkingTowards = "left";
            }
            else if (HorizontalValue > 0)
            {
                walkingTowards = "right";
            }
        }
        else if (Mathf.Abs(HorizontalValue) < Mathf.Abs(VerticalValue))
        {
            if (VerticalValue < 0)
            {
                walkingTowards = "down";
            }
            else if (VerticalValue > 0)
            {
                walkingTowards = "up";
            }
        }

        //Debug.Log(walkingTowards);
    }


    void FixedUpdate()
    {
        Move();
    }

    void Move()
    {
        Vector3 movement = (HorizontalValue * transform.right + VerticalValue * transform.forward) * Speed * Time.deltaTime;
        rigidBody.MovePosition(transform.position + movement);
    }


    void OnCollisionEnter(Collision col)
    {
        //Debug.Log("Collide " + col.gameObject.tag);
        if (col.gameObject.tag == "Bomb")
        {
            //Debug.Log("Collided with Bomb");
            Rigidbody bombRigidBody = col.gameObject.GetComponent<Rigidbody>();
            BombBehaviour behaviour = col.gameObject.GetComponent<BombBehaviour>();
            if (canKickBomb && !behaviour.pushed)
            {
                switch (walkingTowards)
                {
                    case "left":
                        if (transform.position.x - bombRigidBody.transform.position.x <= 1.2)
                        {
                            behaviour.pushed = true;
                            bombRigidBody.isKinematic = false;
                            bombRigidBody.AddForce(-transform.right * kickSpeed);
                        }
                        break;
                    case "right":
                        if (bombRigidBody.transform.position.x - transform.position.x<= 1.2)
                        {
                            behaviour.pushed = true;
                            bombRigidBody.isKinematic = false;
                            bombRigidBody.AddForce(transform.right * kickSpeed);
                        }
                        break;
                    case "up":
                        if (bombRigidBody.transform.position.y - transform.position.y <= 1.2)
                        {
                            behaviour.pushed = true;
                            bombRigidBody.isKinematic = false;
                            bombRigidBody.AddForce(transform.forward * kickSpeed);
                        }
                        break;
                    case "down":
                        if (transform.position.y - bombRigidBody.transform.position.y <= 1.2)
                        {
                            behaviour.pushed = true;
                            bombRigidBody.isKinematic = false;
                            bombRigidBody.AddForce(-transform.forward * kickSpeed);
                        }
                        break;
                }
            }
        }
    }
}
