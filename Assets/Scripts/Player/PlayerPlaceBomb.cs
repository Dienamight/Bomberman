using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPlaceBomb : MonoBehaviour {

    [HideInInspector] public float bombCount = 1;
    [HideInInspector] public float firePower = 1;
    [HideInInspector] public int playerNum = 1;
    [HideInInspector]
    public bool bombPierce = false;
    public GameObject Bomb;

    private float originalPitch;
    private float pitchOffset = 0.2f;
    private string fireKey;
    private AudioSource bombPlace;

    private bool FireKeyPressed = false;
    Vector3 DropBombPosition;
    GameObject sittingBomb;

	// Use this for initialization
	void Start () {
        fireKey = "Fire" + playerNum;
        bombPlace = GetComponent<AudioSource>();
        originalPitch = bombPlace.pitch;

    }
	
	// Update is called once per frame
	void Update () {
        //Debug.Log(sittingBomb);
        DropBombPosition = new Vector3(Mathf.RoundToInt(transform.position.x), transform.position.y, Mathf.RoundToInt(transform.position.z));
        FireKeyPressed = Input.GetButtonDown(fireKey);
        DropBomb();
    }


    void DropBomb()
    {
        if (Bomb && FireKeyPressed && bombCount > 0 && sittingBomb==null)
        {
            GameObject bomb = Instantiate(Bomb, DropBombPosition, transform.rotation);
            BombBehaviour bombBehaviour = bomb.GetComponent<BombBehaviour>();
            if (bombBehaviour)
            {
                bombBehaviour.Initialize(gameObject, firePower,bombPierce);
                bombPlace.pitch = originalPitch + Random.Range(-pitchOffset, pitchOffset);
                bombPlace.Play();
            }
            bombCount--;
        }
    }


    void OnTriggerStay(Collider col)
    {
        //Debug.Log(col.name);
        if (col.tag == "Bomb")
        {
            //Debug.Log("Hit Bomb");
            if (sittingBomb == null)
            {
                sittingBomb = col.gameObject;
            }
        }
    }

    void OnTriggerExit(Collider col)
    {
        if (col.tag == "Bomb")
        {
            //Debug.Log("Release");
            col.isTrigger = false;
            if (sittingBomb != null)
            {
                sittingBomb = null;
            }
        }
    }
}
