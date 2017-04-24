using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class PanelManager : MonoBehaviour {

    public GameObject GameOverlay;
    public GameObject Scoreboard;
    public GameObject GameMenu;

    public GameObject EndGameButton;

    public bool uiActive = false;

    private bool _menuState = false;
    CursorStates _cs;

    bool gameOver = false;

	// Use this for initialization
	void Start () {
        EndGameButton.SetActive(false);

	    if(GameOverlay == null || Scoreboard == null || GameMenu == null)
        {
            Debug.LogError("PanelManager: A panel is not assigned");
        }
        GameOverlay.SetActive(true);
        Scoreboard.transform.localPosition = new Vector3(10000, 0, 0);
        GameMenu.SetActive(false);

        _cs = GameObject.Find("CursorStates").GetComponent<CursorStates>();
	}
	
	// Update is called once per frame
	void Update () {
        if (!gameOver)
        {
            if (!Scoreboard.activeSelf)
            {
                Scoreboard.SetActive(true);
            }

            if (Input.GetButtonDown("Cancel"))
            {
                if (_menuState == false)
                {
                    GameOverlay.SetActive(false);
                    Scoreboard.transform.localPosition = new Vector3(10000, 0, 0);
                    GameMenu.SetActive(true);
                    uiActive = true;
                    _cs.UnlockCursor();
                    _menuState = true;
                    GameMenu.transform.GetChild(0).GetComponent<Button>().Select();
                }
                else
                {
                    Resume();
                }

            }

            if (Input.GetButtonDown("Scoreboard") && !GameMenu.GetActive())
            {
                GameOverlay.SetActive(false);
                Scoreboard.transform.localPosition = new Vector3(0, 0, 0);
                GameMenu.SetActive(false);
            }

            if (Input.GetButtonUp("Scoreboard") && !GameMenu.GetActive())
            {
                GameOverlay.SetActive(true);
                Scoreboard.transform.localPosition = new Vector3(10000, 0, 0);
                GameMenu.SetActive(false);
            }
        }
    }

    public void Resume()
    {
        GameOverlay.SetActive(true);
        Scoreboard.transform.localPosition = new Vector3(10000, 0, 0);
        GameMenu.SetActive(false);
        _cs.UnlockCursor();
        uiActive = false;
        _menuState = false;
    }

    public void EndGame()
    {
        gameOver = true;
        GameOverlay.SetActive(false);
        Scoreboard.transform.localPosition = new Vector3(0, 0, 0);
        GameMenu.SetActive(false);
        EndGameButton.SetActive(true);
        _cs.UnlockCursor();
        EndGameButton.GetComponent<Button>().Select();
    }
}
