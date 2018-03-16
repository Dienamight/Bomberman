using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    public int brickCount = 90;
    public int gameModes = 3;

    public GameObject brickPrefab;
    public GameObject hardBrickPrefab;

    public float startDelay = 3f;
    public float endDeloay = 2f;
    public int winNum = 5;
    public int startingTime = 180;
    public GameObject playerPrefab;
    public PlayerManager[] playerManager;

    public Text messageText;
    public Text timerText;

    private bool gameStarting = false;
    private int currentTime;
    private int roundNum = 1;
    private WaitForSeconds WaitStartTime;
    private WaitForSeconds WaitEndTime;

    private PlayerManager roundWinner;
    private PlayerManager gameWinner;


    private int[,] grid = new int[13, 13];

    // Use this for initialization
    void Start() {
        Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Player"));

        WaitStartTime = new WaitForSeconds(startDelay);
        WaitEndTime = new WaitForSeconds(endDeloay);
        SpawnPlayers();
        StartCoroutine(GameLoop());

    }

    void Update()
    {
        string text = (currentTime / 60) + ":";
        if (currentTime % 60 < 10)
        {
            text = text + "0";
        }
        text = text + (currentTime % 60);
        timerText.text = text;
    }


    IEnumerator GameLoop()
    {
        yield return StartCoroutine(GameStart());
        yield return StartCoroutine(GameRunning());
        yield return StartCoroutine(GameEnd());

        if (gameWinner != null)
        {
            //End Game
            SceneManager.LoadScene(0);
        }
        else
        {
            StartCoroutine(GameLoop());
        }
    }

    IEnumerator GameStart()
    {
        currentTime = startingTime;
        ResetGridNum();
        RemoveAllBricks();
        RemoveAllItems();
        RemoveAllBombs();
        RemoveAllHardBricks();
        if (brickPrefab && hardBrickPrefab)
        {
            int currentGameMode = Random.Range(0, gameModes);
            //Debug.Log("Gamemode = " + currentGameMode);
            switch (currentGameMode)
            {
                case 0:
                    SetupFieldNormally();
                    break;
                case 1:
                    SetupOpendField();
                    break;
            }

        }
        ResetAllPlayer();
        DisableControls();
        messageText.text = "Round " + roundNum;
        yield return WaitStartTime;
    }

    IEnumerator GameRunning()
    {
        EnableControls();
        gameStarting = true;
        StartCoroutine(CountDownTime());
        messageText.text = "Start!";
        yield return new WaitForSeconds(1f);
        messageText.text = "";
        while (!GameSet() && currentTime > 0)
        {
            yield return null;
        }
    }

    IEnumerator GameEnd()
    {
        DisableControls();
        gameStarting = false;
        roundWinner = null;
        if (currentTime > 0)
        {
            roundWinner = GetRoundWinner();
        }
        if (roundWinner != null)
        {
            roundWinner.winCount++;
        }

        gameWinner = null;
        gameWinner = GetGameWinner();

        string message = ShowEndMessage();
        messageText.text = message;

        roundNum++;
        yield return WaitEndTime;
    }

    IEnumerator CountDownTime()
    {
        while (currentTime > 0 && gameStarting)
        {
            yield return new WaitForSeconds(1f);
            --currentTime;
        }
    }

    void SpawnPlayers()
    {
        for (int i = 0; i < playerManager.Length; i++)
        {
            playerManager[i].playerInstance = Instantiate(playerPrefab, playerManager[i].spawnPoint.position, playerManager[i].spawnPoint.rotation);
            playerManager[i].playerNum = i + 1;
            playerManager[i].Setup();
        }
    }

    void ResetGridNum()
    {
        for (int i = 0; i < 13; i++)
        {
            for (int j = 0; j < 13; j++)
            {
                grid[i, j] = 0;
            }
        }
    }

    void RemoveAllHardBricks()
    {
        GameObject[] hardbricks = GameObject.FindGameObjectsWithTag("HardBrick");
        for (int i = 0; i < hardbricks.Length; i++)
        {
            Destroy(hardbricks[i]);
        }
    }

    void RemoveAllBombs()
    {
        GameObject[] bombs = GameObject.FindGameObjectsWithTag("Bomb");
        for (int i = 0; i < bombs.Length; i++)
        {
            Destroy(bombs[i]);
        }
    }

    void RemoveAllBricks()
    {
        GameObject[] blocks = GameObject.FindGameObjectsWithTag("Brick");
        for (int i = 0; i < blocks.Length; i++)
        {
            Destroy(blocks[i]);
        }
    }

    void RemoveAllItems()
    {
        GameObject[] items = GameObject.FindGameObjectsWithTag("Item");
        for (int i = 0; i < items.Length; i++)
        {
            Destroy(items[i]);
        }
    }

    void SetupOpendField()
    {
        for (int i = 0; i < 13; i++)
        {
            for (int j = 0; j < 13; j++)
            {
                grid[i, j] = 0;
            }
        }

        EmptyPlayerPosition();
        //CheckGrid();
        SetupBricks(30);
    }

    void SetupFieldNormally()
    {
        for (int i = 0; i < 13; i++)
        {
            for (int j = 0; j < 13; j++)
            {
                if (i%2 == 1 && j%2 == 1)
                {
                    grid[i, j] = -2;
                }
                else
                {
                    grid[i, j] = 0;
                }
                
            }
        }

        EmptyPlayerPosition();
        //CheckGrid();
        SetupBricks();
    }

    void EmptyPlayerPosition()
    {
        //Player 1
        grid[0, 0] = grid[0, 1] = grid[1, 0] = -1;
        //Player 2
        grid[12, 0] = grid[12, 1] = grid[11, 0] = -1;
        //Player 3
        grid[0, 12] = grid[0, 11] = grid[1, 12] = -1;
        //Player 4
        grid[12, 12] = grid[12, 11] = grid[11, 12] = -1;
    }

    void SetupBricks(int additionBrickNum = 0)
    {
        int bricksPlaced = 0;
        int x = 0;
        int y = 0;
        while (bricksPlaced < (brickCount + additionBrickNum > 169? 169: brickCount + additionBrickNum))
        {
            x = Random.Range(0, 13);
            y = Random.Range(0, 13);

            if (grid[x, y] == 0)
            {
                grid[x, y] = 1;
                //Instantiate(brickPrefab, new Vector3(-6 + x, 0.5f, -6 + y), transform.rotation);
                bricksPlaced++;
            }
        }
        BuildBricks();
    }

    void BuildBricks()
    {
        for (int i = 0; i < 13; i++)
        {
            for (int j = 0; j < 13; j++)
            {
                switch (grid[i, j])
                {
                    case -2:
                        Instantiate(hardBrickPrefab, new Vector3(-6 + i, 0.5f, -6 + j), transform.rotation);
                        break;
                    case 1:
                        Instantiate(brickPrefab, new Vector3(-6 + i, 0.5f, -6 + j), transform.rotation);
                        break;
                }
            }
        }
    }

    void EnableControls()
    {
        for (int i = 0; i < playerManager.Length; i++)
        {
            playerManager[i].EnableControl();
        }
    }

    void DisableControls()
    {
        for (int i = 0; i < playerManager.Length; i++)
        {
            playerManager[i].DisableControl();
        }
    }

    void ResetAllPlayer()
    {
        for (int i = 0; i < playerManager.Length; i++)
        {
            playerManager[i].Reset();
        }
    }

    bool GameSet()
    {
        int playerCount = 0;
        for (int i = 0; i < playerManager.Length; i++)
        {
            if (playerManager[i].playerInstance.activeSelf)
            {
                playerCount++;
            }
        }
        return playerCount <= 1;
    }

    PlayerManager GetRoundWinner()
    {
        for (int i = 0; i < playerManager.Length; i++)
        {
            if (playerManager[i].playerInstance.activeSelf)
            {
                return playerManager[i];
            }
        }
        return null;
    }

    PlayerManager GetGameWinner()
    {
        for (int i = 0; i < playerManager.Length; i++)
        {
            if (playerManager[i].winCount == winNum)
            {
                return playerManager[i];
            }
        }
        return null;
    }

    string ShowEndMessage()
    {
        string message = "Draw...";

        if(roundWinner != null)
        {
            message = roundWinner.playerString + " WIN THIS ROUND!!";
        }

        message += "\n\n\n\n";

        for (int i = 0; i < playerManager.Length; i++)
        {
            message += playerManager[i].playerString + ": " + playerManager[i].winCount + " WINS\n";
        }

        if (gameWinner != null)
            message = gameWinner.playerString + " WINS THE GAME!";

        return message;
    }
}
