using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehaviour : MonoBehaviour {

    [HideInInspector]
    public int maxBombCount = 8;
    [HideInInspector]
    public int maxFirePower = 6;
    [HideInInspector]
    public float maxSpeed = 6f;

    [HideInInspector]
    public int minBombCount = 1;
    [HideInInspector]
    public int minFirePower = 1;
    [HideInInspector]
    public float minSpeed = 2f;


    [HideInInspector]
    public bool dead = false;
    [HideInInspector]
    public int playerNum = 1;
    

    PlayerPlaceBomb placeBomb;
    PlayerMovement movement;

	// Use this for initialization
	void Start () {
        placeBomb = GetComponent<PlayerPlaceBomb>();
        movement = GetComponent<PlayerMovement>();
	}
	

    void OnEnable()
    {
        //Debug.Log("Enabling...");
        dead = false;
        //gameObject.SetActive(true);
        ResetStatus();
    }

	// Update is called once per frame
	void Update () {
		
	}

    void ResetStatus()
    {
        if (placeBomb)
        {
            placeBomb.bombCount = minBombCount;
            placeBomb.firePower = minFirePower;
            placeBomb.bombPierce = false;
        }
        if (movement)
        {
            movement.Speed = minSpeed;
            movement.canKickBomb = false;
        }
    }

    public void changeBombCount(int val)
    {
        if (placeBomb && placeBomb.bombCount < maxBombCount)
        {
            placeBomb.bombCount= Mathf.Max(minBombCount, Mathf.Min(placeBomb.bombCount + val, maxBombCount));
        }
    }

    public void changeFirePower(int val)
    {
        if (placeBomb && placeBomb.firePower < maxFirePower)
        {
            placeBomb.firePower = Mathf.Max(minFirePower, Mathf.Min(placeBomb.firePower + val, maxFirePower));
        }
    }

    public void changeSpeed(float val)
    {
        if (movement && movement.Speed < maxSpeed)
        {
            movement.Speed += val;
            //Debug.Log("Speed = " + movement.Speed);
            movement.Speed = Mathf.Min(movement.Speed, maxSpeed);
            //Debug.Log("Speed After Min = " + movement.Speed);
            //Debug.Log("Speed = " + movement.Speed + ", MinSpeed = " + minSpeed);
            movement.Speed = Mathf.Max(minSpeed, movement.Speed);
            //Debug.Log("Speed After Max= " + movement.Speed);
        }
    }

    public void setCanKick(bool flag)
    {
        if (movement)
        {
            movement.canKickBomb = flag;
        }
    }

    public void setBombPierce(bool flag)
    {
        if (placeBomb)
        {
            placeBomb.bombPierce = true;
        }
    }

    public void KillSelf()
    {
        if (!dead)
        {
            //Debug.Log("Player " + playerNum + " got hit");
            dead = true;
            gameObject.SetActive(false);
            movement.enabled = false;
            placeBomb.enabled = false;
        }
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.tag == "Explosion")
        {
            //Debug.Log("KYS");
            KillSelf();
        }
    }
}
