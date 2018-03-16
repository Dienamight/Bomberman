using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrickBehaviour : MonoBehaviour {

    public GameObject[] Items;


    MeshRenderer meshRenderer;
    int totalItems = 0;
    int dropItem = 99;
	// Use this for initialization
	void Start () {
        meshRenderer = GetComponent<MeshRenderer>();

        for (int i= 0; i < Items.Length; i++)
        {
            if (Items[i])
            {
                totalItems++;
            }
        }
        //Debug.Log("Total Type of Items = " + totalItems);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    IEnumerator DestroySelf()
    {
        yield return new WaitForSeconds(0.5f);
        Destroy(gameObject);
    }

    public void Destroy()
    {
        gameObject.layer = LayerMask.NameToLayer("Default");
        meshRenderer.enabled = false;
        if (dropItem == 99)
        {
            dropItem = Random.Range(-2, totalItems);
            //Debug.Log("DropItem = " + dropItem);
            //Create a random power-up object

            switch (dropItem)
            {
                case -1:
                case -2:
                    break;
                default:
                    //Debug.Log("Create Items");
                    if (Items[dropItem])
                    {
                        Instantiate(Items[dropItem], transform.position, transform.rotation);
                    }
                    break;
            }
        }
        StartCoroutine(DestroySelf());
    }
}
