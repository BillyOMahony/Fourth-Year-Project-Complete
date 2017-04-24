﻿using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using System.Linq;

public class MatchManager : Photon.PunBehaviour {

    #region public variables

    static public MatchManager Instance;
    
    public GameObject redPrefab;
    public GameObject bluePrefab;

    public GameObject[] redSpawners;
    public GameObject[] blueSpawners;

    public GameObject spawnPoint;

    public GameObject GameManager;

    public int RedScore = 0;
    public int BlueScore = 0;

    public int scoreToWin = 5;

    public string team;
    public PlayerManager playerManager;

    #endregion

    #region private variables

    public int _spawnCounter = 0;
    GameObject _redTeamText;
    GameObject _blueTeamText;
    PanelManager _pm;

    #endregion

    #region MonoBehaviour Methods

    void Start()
    {

        GameManager = GameObject.Find("GameManager");
        _pm = GameObject.Find("PanelManager").GetComponent<PanelManager>();

        GameObject.Find("CursorStates").GetComponent<CursorStates>().LockCursor();

        team = GameManager.GetComponent<GameManager>().GetMyTeam();

        if (redPrefab == null)
        {
            Debug.LogError("<Color=Red><a>Missing</a></Color> redPrefab reference. Set it Up!");
        }
        else if (bluePrefab == null)
        {
            Debug.LogError("<Color=Red><a>Missing</a></Color> bluePrefab reference. Set it Up!");
        }
        else if (redSpawners.Any(n => n == null) || blueSpawners.Any(n => n == null))
        {
            Debug.LogError("<Color=Red><a>Missing</a></Color> spawners reference contains one or more nulls. Fix This!");
        }

        Spawn();

    }

    #endregion

    // Update is called once per frame
    void Update() {

    }

    void Spawn()
    {
        GameObject spawnPoint = SelectSpawner();
        Debug.LogWarning("MatchManager: Spawn() called");

        if (team == "red")
        {
            PhotonNetwork.Instantiate(redPrefab.name, spawnPoint.transform.position, spawnPoint.transform.rotation, 0);
        }
        else if(team == "blue")
        {
            PhotonNetwork.Instantiate(bluePrefab.name, spawnPoint.transform.position, spawnPoint.transform.rotation, 0);
        }else
        {
            Debug.LogError("MatchManager: Spawn() could not spawn player");
        }
        _redTeamText = GameObject.Find("RedTeamScore Text");
        _blueTeamText = GameObject.Find("BlueTeamScore Text");

        _redTeamText.GetComponent<Text>().text = "Red Team " + RedScore;
        _blueTeamText.GetComponent<Text>().text = "BlueTeam " + BlueScore;
    }

    /// <summary>
    /// Selects the Spawn the player will appear at. 
    /// </summary>
    /// <returns>GameObject spawnPoint: The spawner the player appears at</returns>
    public GameObject SelectSpawner()
    {
        GameObject[] spawners = new GameObject[0];
        string team = GameManager.GetComponent<GameManager>().team;
        if (team == "red")
        {
            spawners = redSpawners;
        } else if (team == "blue")
        {
            spawners = blueSpawners;
        }
        else
        {
            Debug.LogError("MatchManager SelectSpawner() Cannot retrieve spawners. Team: " + team);
        }

        GameObject spawnPoint;

        if (_spawnCounter > 0)
        {
            int random = Random.Range(0, spawners.Length);
            Debug.LogWarning("RandomSpawnPoint" + random);
            spawnPoint = spawners[random];
        }
        else
        {
            Debug.LogWarning("Fixed Spawn Point");
            spawnPoint = spawners[GameManager.GetComponent<GameManager>().initialTeamSpawnNum - 1];
        }
        
        if (!spawnPoint.GetComponent<SpawnCollisionDetection>().CanSpawn)
        {
            return SelectSpawner();
        }

        _spawnCounter++;

        return spawnPoint;
    }

    #region RPCs
    
    [PunRPC]
    public void redTeamScored(int score)
    {
        RedScore += score;
        UpdateScoreText();
    }

    [PunRPC]
    public void blueTeamScored(int score)
    {
        BlueScore += score;
        UpdateScoreText();
    }

    [PunRPC]
    public void GameOver()
    {
        //Stuff that happens when the game ends

        _pm.EndGame();
        playerManager.EndGame();

    //    PhotonNetwork.Destroy(GameManager);
    //    SceneManager.LoadScene(0);
        
    }
    
    #endregion

    public void UpdateScore(string team, int addScore)
    {
        if(team == "red")
        {
            GetComponent<PhotonView>().RPC("redTeamScored", PhotonTargets.All, addScore);
        }
        else if(team == "blue")
        {
            GetComponent<PhotonView>().RPC("blueTeamScored", PhotonTargets.All, addScore);
        }

        if(BlueScore >= scoreToWin || RedScore >= scoreToWin)
        {
            GetComponent<PhotonView>().RPC("GameOver", PhotonTargets.All);
        }

        //Stuff here to update GUI and such
    }

    void UpdateScoreText()
    {
        _redTeamText.GetComponent<Text>().text = "Red Team " + RedScore;
        _blueTeamText.GetComponent<Text>().text = "BlueTeam " + BlueScore;
    }

    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) { }
}