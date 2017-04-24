using UnityEngine;
using System.Collections;

public class CameraWorks : Photon.PunBehaviour {

    #region private variables

    public Transform _cameraTransform;
    public Canvas canvas;

    public PlayerControllerRB _PC;
    public Vector3 NearShip;
    public Vector3 FarFromShip;

    public bool MovingTowards = false;
    public bool MovingAway = false;

    float lerpTimeAway = 0.35f;
    float lerpTimeTowards = 1f;
    float currentLerpTimeTowards;
    float currentLerpTimeAway;
    #endregion

    void Start()
    {
        _PC = GetComponent<PlayerControllerRB>();

        canvas = GameObject.Find("Canvas").GetComponent<Canvas>();

        _cameraTransform = transform.GetChild(1);
        if (!photonView.isMine && PhotonNetwork.connected)
        {
            _cameraTransform.gameObject.SetActive(false);
        }
        if (photonView.isMine)
        {
            canvas.worldCamera = _cameraTransform.gameObject.GetComponent<Camera>();
            canvas.planeDistance = 1;
        }
    }

    void Update()
    {
        if (_PC.flightBoost)
        {
            MovingTowards = false;
            MovingAway = true;
        }
        else
        {
            MovingTowards = true;
            MovingAway = false;
        }

        if (MovingTowards)
        {
            currentLerpTimeAway = 0;
            currentLerpTimeTowards += Time.deltaTime;
            if(currentLerpTimeTowards >= lerpTimeTowards)
            {
                currentLerpTimeTowards = lerpTimeTowards;
            }
            float per = currentLerpTimeTowards / lerpTimeTowards;
            _cameraTransform.localPosition = Vector3.Lerp(FarFromShip, NearShip, per);
        }
        else if (MovingAway)
        {
            currentLerpTimeTowards = 0;
            currentLerpTimeAway += Time.deltaTime;
            if (currentLerpTimeAway >= lerpTimeAway)
            {
                currentLerpTimeAway = lerpTimeAway;
            }
            float per = currentLerpTimeAway / lerpTimeAway;
            _cameraTransform.localPosition = Vector3.Lerp(NearShip, FarFromShip, per);
        }

    }

}
