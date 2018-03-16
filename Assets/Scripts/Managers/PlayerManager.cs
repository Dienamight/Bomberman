using System;
using UnityEngine;

[Serializable]
public class PlayerManager {

    public Transform spawnPoint;
    public Color playerColor;

    [HideInInspector] public int playerNum;
    [HideInInspector] public GameObject playerInstance;
    [HideInInspector] public int winCount;
    [HideInInspector] public string playerString;

    PlayerBehaviour behaviour;
    PlayerMovement movement;
    PlayerPlaceBomb placeBomb;

    public void Setup()
    {
        movement = playerInstance.GetComponent<PlayerMovement>();
        placeBomb = playerInstance.GetComponent<PlayerPlaceBomb>();
        behaviour = playerInstance.GetComponent<PlayerBehaviour>();

        movement.playerNumber = playerNum;
        placeBomb.playerNum = playerNum;
        behaviour.playerNum = playerNum;

        playerString = "<color=#" + ColorUtility.ToHtmlStringRGB(playerColor) + ">PLAYER " + playerNum + "</color>";

        MeshRenderer[] renderers = playerInstance.GetComponentsInChildren<MeshRenderer>();
        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].material.color = playerColor;
        }
    }

    public void EnableControl()
    {
        movement.enabled = true;
        placeBomb.enabled = true;
        behaviour.enabled = true;
    }

    public void DisableControl()
    {
        movement.enabled = false;
        placeBomb.enabled = false;
        behaviour.enabled = false;
    }

    public void Reset()
    {
        playerInstance.transform.position = spawnPoint.position;
        playerInstance.transform.rotation = spawnPoint.rotation;

        playerInstance.SetActive(false);
        playerInstance.SetActive(true);
    }
}
