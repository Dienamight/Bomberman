using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBehaviour : MonoBehaviour {

    public int itemType = 0;

    private AudioSource pickupItem;
    private float audioTime = 0.5f;
    private MeshRenderer mesh;
    private bool picked = false;

    private float currentTime = 0f;
    private float protectedTime = 1f;

	// Use this for initialization
	void Awake () {
        pickupItem = GetComponent<AudioSource>();
        mesh = GetComponent<MeshRenderer>();


    }
	
	// Update is called once per frame
	void Update () {
        if (currentTime < protectedTime)
        {
            currentTime += Time.deltaTime;
        }
    }

    public void DestroySelf()
    {
        gameObject.layer = LayerMask.NameToLayer("Default");
        //Debug.Log("Destroying Item");
        StartCoroutine(DestroyWithSound());
    }

    IEnumerator DestroyWithSound(bool sound = false)
    {
        if (sound)
        {
            pickupItem.Play();
            yield return new WaitForSeconds(audioTime);
        }
        Destroy(gameObject);
    }

    void OnTriggerEnter(Collider col)
    {
        //Debug.Log("Hit + " + col.gameObject.tag);
        //gameObject.layer = LayerMask.NameToLayer("Default");
        if (col.tag == "Player")
        {
            mesh.enabled = false;
            PlayerBehaviour player = col.GetComponent<PlayerBehaviour>();
            if (player)
            {
                if (!picked)
                {
                    switch (itemType)
                    {
                        //Fire Up
                        case 1:
                            player.changeFirePower(1);
                            break;
                        //Bomb Up
                        case 2:
                            player.changeBombCount(1);
                            break;
                        //Speed Up
                        case 3:
                            player.changeSpeed(0.5f);
                            break;
                        //Bomb Kick
                        case 4:
                            player.setCanKick(true);
                            break;
                        //Spike Bomb (Pierce)
                        case 5:
                            player.setBombPierce(true);
                            break;

                    }
                    picked = true;
                    StartCoroutine(DestroyWithSound(true));
                }
            }
        }
        else if (col.tag == "Explosion")
        {
            if (currentTime >= protectedTime)
            {
                DestroySelf();
            }
        }
    }
}
