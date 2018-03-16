using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombBehaviour : MonoBehaviour {


    private GameObject Corrs_Player;       //The player who place this bomb
    private PlayerPlaceBomb playerPlaceBomb; //Corresponding PlayerPlaceBomb Script

    [HideInInspector] public float firePower = 1f;
    [HideInInspector] public float explosionOffset = 3f;
    [HideInInspector]
    public bool pierceBomb = false;
    [HideInInspector]
    public bool pushed = true;

    public GameObject explosionPrefab;
    public GameObject pierceExplosionPrefab;

    private AudioSource boomAudio;

    private int hitLayer;
    private float timer;
    private float animationTimer = 0.2f;
    public bool exploded = false;
    private MeshRenderer meshRenderer;
    private Rigidbody rigidBody;

    private Ray Ray = new Ray();
    private RaycastHit RayHit;

    void Awake()
    {
        hitLayer = LayerMask.GetMask("Obstacle");
        //Debug.Log("Layer = " + hitLayer);
    }

	// Use this for initialization
	void Start () {
        meshRenderer = GetComponent<MeshRenderer>();
        boomAudio = GetComponent<AudioSource>();
        rigidBody = GetComponent<Rigidbody>();
    }
	
	// Update is called once per frame
	void Update () {
        timer += Time.deltaTime;

        if (timer >= explosionOffset && !exploded)
        {            
            Explode();
        }
        else if(timer >= (explosionOffset + animationTimer))
        {
            HideEffectsAndDestroySelf();
        }
	}

    public void Initialize(GameObject player, float FirePower = 1, bool pierceBomb = false,float ExplosionOffset = 3)
    {
        Corrs_Player = player;
        firePower = FirePower;
        explosionOffset = ExplosionOffset;
        this.pierceBomb = pierceBomb;
    }

    void HideEffectsAndDestroySelf()
    {
        Destroy(gameObject);
    }

    public void Explode()
    {
        boomAudio.Play();
        exploded = true;
            timer = explosionOffset;

            meshRenderer.enabled = false;
            Explode(transform.right);
            Explode(-transform.right);
            Explode(transform.forward);
            Explode(-transform.forward);

            if (Corrs_Player)
            {
                playerPlaceBomb = Corrs_Player.GetComponent<PlayerPlaceBomb>();
                if (playerPlaceBomb)
                {
                    //playerPlaceBomb.hasBombAtDropPosition = false;
                    playerPlaceBomb.bombCount++;
                }
            }
    }

    void Explode(Vector3 direction)
    {
        bool hitObstacle = false;
        Ray.origin = new Vector3(transform.position.x, 0.5f,transform.position.z);
        Ray.direction = direction;

        for (int i = 0; i <= firePower && !hitObstacle; i++)
        {
            if (Physics.Raycast(Ray,out RayHit,i, hitLayer))
            {
                Debug.Log("Explosion hit " + RayHit.collider.name);
                if (RayHit.collider.tag == "HardBrick")
                {
                    //No matter it pierce or not, it will never pass through
                    hitObstacle = true;
                }
                else if (RayHit.collider.tag == "Brick")
                {
                    //It pass through when it pierce
                    if (!pierceBomb)
                    {
                        hitObstacle = true;
                    }
                    //Debug.Log("Explosion hit " + RayHit.collider.name);
                    BrickBehaviour brick = RayHit.collider.gameObject.GetComponent<BrickBehaviour>();
                    if (brick)
                    {
                        brick.Destroy();
                    }
                }
                /*else if (RayHit.collider.tag == "Item")
                {
                    //It pass through no matter what
                    Debug.Log("Explosion hit " + RayHit.collider.name);
                    RayHit.collider.gameObject.layer = LayerMask.NameToLayer("Default");
                    ItemBehaviour item = RayHit.collider.gameObject.GetComponent<ItemBehaviour>();
                    if (item)
                    {
                        item.DestroySelf();
                    }
                }*/
                if (!pierceBomb)
                {
                    if (explosionPrefab && !RayHit.collider && !hitObstacle)
                    {
                        //Debug.Log("Did not hit = " + direction + " i = " + i);
                        Instantiate(explosionPrefab, transform.position + direction * i, transform.rotation);
                    }
                }
                else
                {
                    if (pierceExplosionPrefab && !hitObstacle)
                    {
                        Instantiate(pierceExplosionPrefab, transform.position + direction * i, transform.rotation);

                    }
                }

            }
            else
            {
                if (!pierceBomb)
                {
                    if (explosionPrefab)
                    {
                        //Debug.Log("direction = " + direction + " i = " + i);
                        Instantiate(explosionPrefab, transform.position + direction * i, transform.rotation);
                    }
                }
                else
                {
                    if (pierceExplosionPrefab)
                    {
                        Instantiate(pierceExplosionPrefab, transform.position + direction * i, transform.rotation);

                    }
                }
            }
        }
    }

    void OnCollisionEnter(Collision col)
    {
        //Debug.Log("Bomb Collide with = " + col.gameObject);
        if (col.gameObject.tag == "Brick" || col.gameObject.tag == "Bomb")
        {
            rigidBody.velocity = Vector3.zero;
            rigidBody.transform.position = new Vector3(Mathf.RoundToInt(rigidBody.transform.position.x), 1f, Mathf.RoundToInt(rigidBody.transform.position.z));
            pushed = false;
        }
        else if (col.gameObject.tag == "Player")
        {
            rigidBody.velocity = Vector3.zero;
            pushed = false;
        }
    }

    void OnTriggerEnter(Collider col)
    {
        //Debug.Log("Bomb hit = " + col.name);
        if (!exploded && col.tag == "Explosion")
        {
            exploded = true;
            CancelInvoke("Explode");
            Explode();
        }
            
    }
}
