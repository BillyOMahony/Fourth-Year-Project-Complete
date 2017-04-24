﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LobbyManager : Photon.PunBehaviour
{

    #region public variables

    public string playerName;
    public GameObject playerPanel;
    public GameObject canvas;
    public RectTransform playerImage;
    public float countdown = 10.0f;
    public Text countdownText;
    public int playersForGameToBegin = 4;

    #endregion

    #region private variables

    GameObject panel;
    Teams teams;
    public string team; 
    public bool playersReady = false;

    Color red = new Color(0.447f, 0.255f, 0.18f, 1.0f);
    Color blue = new Color(0.18f, 0.255f, 0.447f, 1.0f);

    IconScript _icons;

    #endregion

    #region MonoBehaviour Methods

    void Start()
    {
        _icons = GameObject.Find("PlayerIcon").GetComponent<IconScript>();

        playerName = PhotonNetwork.player.NickName;

        DontDestroyOnLoad(GameObject.Find("GameManager"));

        PhotonNetwork.sendRate = 10;
        PhotonNetwork.sendRateOnSerialize = 5;

        countdownText = Instantiate(countdownText) as Text;
        countdownText.transform.SetParent(canvas.transform, false);

        teams = GameObject.Find("GameManager").GetComponent<Teams>();

        GameObject.Find("CursorStates").GetComponent<CursorStates>().UnlockCursor();
    }

    void Update()
    {
        float currentTime = Mathf.Floor(countdown);
        // if enoughPlayers = True, countdown begins.
        // when countdown <= 0, Game begins.
        if (playersReady)
        {
            if (PhotonNetwork.isMasterClient)
            {
                countdown -= Time.deltaTime;
            }

            if(currentTime < 0)
            {
                currentTime = 0;
            }
            countdownText.text = "Game will begin in " + currentTime.ToString();

            if (countdown <= 0.0f)
            {
                BeginGame();
            }
            
        }

        UpdateGUI();
        CheckPlayerReadyStatus();
    }

    #endregion


    #region Photon Messages

    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting == true)
        {
            stream.SendNext(countdown);
        }
        else
        {
            countdown = (float)stream.ReceiveNext();
        }
    }

    // Called when a player connects. 
    public override void OnPhotonPlayerConnected(PhotonPlayer other)
    {
        UpdateGUI();
    }

    public override void OnPhotonPlayerDisconnected(PhotonPlayer other)
    {
        UpdateGUI();
    }

    #endregion

    #region private methods

    void CheckPlayerReadyStatus()
    {
        bool AllReady = true;
        Transform[] Children = GameObject.Find("PlayerInstance Container").GetComponentsInChildren<Transform>();
 
        foreach (Transform child in Children)
        {
            PlayerLobbyManager plm = child.GetComponent<PlayerLobbyManager>();
            if (child.name != "PlayerInstance Container")
            {
                if (!plm.ready)
                {
                    AllReady = false;
                    break;
                }
            }
        }

        if(Children.Length < 3)
        {
            AllReady = false;
        }

        if (AllReady)
        {
            playersReady = AllReady;
        }
    }

    public void UpdateGUI()
    {
        Destroy(panel);
        panel = Instantiate(playerPanel) as GameObject;
        panel.transform.SetParent(canvas.transform, false);

        float offset = 0;

        Transform[] Children = GameObject.Find("PlayerInstance Container").GetComponentsInChildren<Transform>();
        foreach(Transform child in Children)
        {
            PlayerLobbyManager plm = child.GetComponent<PlayerLobbyManager>();
            if (child.name != "PlayerInstance Container")
            {
                RectTransform indvPlayerPanel = Instantiate(playerImage);
                indvPlayerPanel.transform.SetParent(panel.transform, false);
                offset += 60;
                indvPlayerPanel.GetChild(0).GetComponent<Image>().sprite = _icons.GetIcon(plm.icon);
                indvPlayerPanel.GetChild(1).GetComponent<Text>().text = plm.Name;
                indvPlayerPanel.localPosition += Vector3.up * offset * -1;
                if(plm.team == "red")
                {
                    indvPlayerPanel.GetComponent<Image>().color = red;
                }else if(plm.team == "blue")
                {
                    indvPlayerPanel.GetComponent<Image>().color = blue;
                }
                else
                {
                    indvPlayerPanel.GetComponent<Image>().color = Color.gray;
                }
            }
        }
    }

    void BeginGame()
    {
        if (!PhotonNetwork.isMasterClient)
        {
            Debug.LogError("PhotonNetwork: Trying to load level, but not master client.");
        }
        Debug.Log("PhotonNetwork: Loading Level: Level_AsteroidField");

        teams.RedCount();
        teams.BlueCount();

        //levels can be loaded by name(string), or build number (int)
        PhotonNetwork.room.IsOpen = false;
        PhotonNetwork.LoadLevel("Level_AsteroidField");
    }

    #endregion
}
