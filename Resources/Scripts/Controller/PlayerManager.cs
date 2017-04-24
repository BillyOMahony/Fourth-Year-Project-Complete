using UnityEngine;
using System.Collections;

using System.Collections.Generic;

public class PlayerManager : Photon.PunBehaviour {

    #region public variables
    [Tooltip("The current Health of our player")]
    public float OriginalHealth = 100f;
    public float Health;
    public GameObject DeathCamera;
    public string team;
    public float outOfBoundsTimer = 10.0f;

    public GameObject blueScorePanel;
    public GameObject redScorePanel;
    public GameObject scoreBoard;

    #endregion

    #region private variables

    string owner;

    MatchManager matchManager;
    GameObject _gameManager;
    Teams _teams;
    bool dead = false;

    PlayerControllerRB controller;

    PhotonView _pv;

    PlayerUIManager _puim;
    ScoreManager _sm;
    IndividualScore _is;
    PanelManager _panM;

    float timer = 5f;

    bool outOfBounds = false;

    ExplosionScript _es;

    AsteroidSpawner _as;
    #endregion

    // Use this for initialization
    void Start () {

        scoreBoard = GameObject.Find("Canvas").transform.GetChild(1).gameObject;

        PhotonNetwork.sendRate = 20;
        PhotonNetwork.sendRateOnSerialize = 10;

        _panM = GameObject.Find("PanelManager").GetComponent<PanelManager>();
        _pv = GetComponent<PhotonView>();
        _sm = GetComponent<ScoreManager>();
        _puim = GetComponent<PlayerUIManager>();
        _es = GetComponent<ExplosionScript>();
        _as = GameObject.Find("AsteroidSpawner").GetComponent<AsteroidSpawner>();

        Health = OriginalHealth;

        controller = gameObject.GetComponent<PlayerControllerRB>();
        owner = _pv.owner.NickName;
        matchManager = GameObject.Find("MatchManager").GetComponent<MatchManager>();
        _is = GameObject.Find("IndividualScore").GetComponent<IndividualScore>();

        _gameManager = GameObject.Find("GameManager");
        _teams = _gameManager.GetComponent<Teams>();

        Debug.LogWarning("Calling Teams.GetTeam(|" + owner + "|)");

        foreach (KeyValuePair<string, string> player in _teams.teams)
        {
            Debug.LogWarning("|" + player.Key + "|");
        }

        _teams.RedCount();
        _teams.BlueCount();

        if (_pv.isMine)
        {
            team = _gameManager.GetComponent<GameManager>().team;
            //Let the Match Manager know which PlayerManager is owned by the local player
            GameObject.Find("MatchManager").GetComponent<MatchManager>().playerManager = gameObject.GetComponent<PlayerManager>();
        }
        else
        {
            team = _gameManager.GetComponent<Teams>().GetTeam(owner);
        }
    }
	
	// Update is called once per frame
	void Update () {
        if (dead)
        {
            _panM.GameOverlay.SetActive(false);
            Respawn();
        }

        if (outOfBounds)
        {
            outOfBoundsTimer -= Time.deltaTime;
            if(outOfBoundsTimer < 0)
            {
                GetComponent<PhotonView>().RPC("DeActivate", PhotonTargets.All);
                DisableClient();
                KillClient();
                outOfBounds = false;
                outOfBoundsTimer = 10.0f;
                _puim.BackInBounds();
                if (_pv.isMine)
                {
                    _is.deaths++;
                }
            }
        }

        if (_as.spawnPanels)
        {
            Debug.LogWarning("isMine:" + _pv.isMine + ", SpawnPanels:" + _as.spawnPanels);
        }
        if(_pv.isMine && _as.spawnPanels)
        {
            SpawnScorePanel();
            _as.spawnPanels = false;
        }
	}

    public void DamageTaken(float damage, string hitBy)
    {
        if (PhotonNetwork.isMasterClient)
        {
            GetComponent<PhotonView>().RPC("ReceiveDamage", PhotonTargets.All, damage);

            Debug.Log("Damage Taken, calling _PUIM.UpdateHealthBar()");

            if (Health <= 0)
            {
                Debug.LogWarning("<color=red>Calling Teams.GetTeam("+ hitBy +")</color>");
                string hitByTeam = _teams.GetTeam(hitBy);
                Debug.LogWarning("Returned: " + hitByTeam);
                int scoreAdd = 1;
                if(hitByTeam == team)
                {
                    scoreAdd = -1;
                }
                Debug.Log("Player from " + hitByTeam + " killed player from " + team);


                GetComponent<PhotonView>().RPC("DeActivate", PhotonTargets.All);
                int i = 0;

                foreach (PhotonPlayer player in PhotonNetwork.playerList)
                {
                    if (player.NickName == owner)
                    {
                        break;
                    }
                    i++;
                }

                _is.CallUpdateDeaths(i);

                GetComponent<PhotonView>().RPC("DisableClient", PhotonNetwork.playerList[i]);
                GetComponent<PhotonView>().RPC("KillClient", PhotonNetwork.playerList[i]);

                i = 0;

                foreach (PhotonPlayer player in PhotonNetwork.playerList)
                {
                    if (player.NickName == hitBy)
                    {
                        break;
                    }
                    i++;
                }
                //Updates the kill count for the player who killed this player
                _is.CallUpdateKills(i, scoreAdd);

                Debug.LogWarning("Calling MatchManager.UpdateScore(|" + hitByTeam + "|,|" + scoreAdd + "|");
                matchManager.UpdateScore(hitByTeam, scoreAdd);
            }
        }
    }

    public void DamageAudio()
    {
        GetComponents<AudioSource>()[1].Play();
    }

    public void EndGame()
    {
        DisableClient();
    }

    [PunRPC]
    void DeActivate()
    {
        transform.GetChild(0).gameObject.SetActive(false);
        transform.GetChild(3).gameObject.SetActive(false);
        _es.PlayExplosion();
    }

    [PunRPC]
    void Activate()
    {
        transform.GetChild(0).gameObject.SetActive(true);
        transform.GetChild(3).gameObject.SetActive(true);
        // Health is set here because currently when Activate is called, it's used for respawning.
        Health = 100f;
    }

    [PunRPC]
    void DisableClient()
    {
        controller.Disabled();
    }

    [PunRPC]
    void KillClient()
    {
        dead = true;
        controller.Killed();
    }

    [PunRPC]
    void ReceiveDamage(float dmg)
    {
        Health -= dmg;
        DamageAudio();
    }

    void Respawn()
    {
        timer -= Time.deltaTime;
        if(timer <= 0)
        {
            SpawnPlayer();
            dead = false;
            timer = 5;
            _panM.GameOverlay.SetActive(true);
        }
    }

    void SpawnScorePanel()
    {
        // scoreBoard.SetActive(true);
        string tm = _gameManager.GetComponent<GameManager>().team;

        GameObject Panel = new GameObject();

        if (tm == "red")
        {
            Panel = PhotonNetwork.Instantiate(redScorePanel.name, redScorePanel.transform.position, redScorePanel.transform.rotation, 0) as GameObject;
            //Panel.transform.SetParent(GameObject.Find("Red Team Panel").transform);
        }
        else// if(tm == "blue")
        {
            Panel = PhotonNetwork.Instantiate(blueScorePanel.name, blueScorePanel.transform.position, blueScorePanel.transform.rotation, 0) as GameObject;
            //Panel.transform.SetParent(GameObject.Find("Blue Team Panel").transform);
        }

        Panel.GetComponent<PhotonView>().RPC("SetTeam", PhotonTargets.All, team);

        // scoreBoard.SetActive(false);
    }

    void SpawnPlayer()
    {
        GameObject spawnPoint = matchManager.SelectSpawner();
        transform.position = spawnPoint.transform.position;
        transform.rotation = spawnPoint.transform.rotation;
        GetComponent<PhotonView>().RPC("Activate", PhotonTargets.All);
        controller.Enabled();
    }

    public void OutOfBounds()
    {
        outOfBounds = true;
        _puim.OutOfBounds();
    }

    public void BackInBounds()
    {
        outOfBounds = false;
        outOfBoundsTimer = 10.0f;
        _puim.BackInBounds();
    }
}