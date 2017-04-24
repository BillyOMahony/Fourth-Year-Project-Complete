using UnityEngine;
using System.Collections;

public class PlayerControllerRB : Photon.PunBehaviour
{

    public bool disabled = false;
    public bool uiActive = false;
    public string owner;

    #region public variables movement

    //Network variables
    public float turn;
    public float pitch;
    public float roll;

    //public 
    public float maxSpeed = 10.0f;
    public float maxHorizontalSpeed = 3.0f;
    public float maxVerticalSpeed = 3.0f;

    public float accelerationMultiplier = 10f;

    public float turnSpeed = 10f;
    public float maxTurn = 20f;

    public float engine = 0f;
    public float engineAcceleration = 0.2f;

    public float xVel;
    public float yVel;
    public float zVel;

    public Vector3 pointVelocity;

    public float maxAllowedSpeed;
    public float boostTimer = 10.0f;
    public float boostCooldown = 20.0f;
    public bool flightBoost = false;
    #endregion

    #region public variables shoot

    public float RateOfFire = 0.2f;

    public GameObject projectile;
    public GameObject projectileSpawner;

    #endregion

    #region private variables

    float newVel;
    float _acceleration = 0f;
    float _horizontalAcceleration = 0f;
    float _verticalAcceleration = 0f;

    float tmpMaxSpeed;
    float tmpAccelerationMultiplier;

    float tmpCooldown;

    GameObject _spaceship;
    Rigidbody _body;

    GameObject NewProjectile;
    Vector3 SpawnPosition;
    bool CanShoot = true;
    float timer;
    AudioSource audio;
    bool flightAssist = true;
    ScoreManager _SM;
    PanelManager _PM;
    bool audioEnd = false;
    bool lerpEngine;

    public float _massMultiplier;

    public Vector3 AngularVelocity;

    public GameObject boostParticleEffect;

    #endregion

    #region MonoBehaviour Methods

    // Use this for initialization
    void Start()
    {
        PhotonNetwork.sendRate = 20;
        PhotonNetwork.sendRateOnSerialize = 10;

        _spaceship = gameObject;
        _body = _spaceship.GetComponent<Rigidbody>();

        audio = projectileSpawner.GetComponent<AudioSource>();
        owner = GetComponent<PhotonView>().owner.NickName;

        _massMultiplier = _body.mass;

        _PM = GameObject.Find("PanelManager").GetComponent<PanelManager>();

        tmpMaxSpeed = maxSpeed;
        tmpAccelerationMultiplier = accelerationMultiplier;
        tmpCooldown = boostCooldown;

        GetComponent<PhotonView>().RPC("StopEffect", PhotonTargets.All);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        AngularVelocity = _body.angularVelocity;
        uiActive = _PM.uiActive;

        //Debug.LogWarning(disabled + ", " + uiActive);

        if (disabled || uiActive)
        {
            return;
        }

        if (photonView.isMine)
        {
            ToggleFligtAssist();

            if (flightAssist)
            {
                _body.AddTorque(-_body.angularVelocity * _massMultiplier);
            }
            
            xVel = transform.InverseTransformDirection(_body.velocity).x;
            yVel = transform.InverseTransformDirection(_body.velocity).y;
            zVel = transform.InverseTransformDirection(_body.velocity).z;

            SetThrottle();
            HorizontalMovement();
            VerticalMovement();

            SetTurn();
            Pitch();
            Roll();

            FireProjectile();

            ApplyThrottle();
            ApplyTurn();

            if (flightAssist)
            {
                Stabilization();
            }

            DetectFlightBoost();
            Boost();
            Cooldown();
        }

        ShootTimer();

        if (audioEnd)
        {
            EndAudioEffect();
        }

        if (lerpEngine)
        {
            engine += 0.05f;
            if(engine >= 1)
            {
                engine = 1;
                lerpEngine = false;
            }
        }
    }

    #endregion

    #region public methods

    public void Disabled()
    {
        disabled = true;
        if (disabled)
        {
            engine = 0f;
        }
    }

    public void Killed()
    {
        _body.velocity = Vector3.zero;
        _body.angularVelocity = Vector3.zero;
    }

    public void Enabled()
    {
        disabled = false;
    }

    #endregion

    #region private methods

    void SetThrottle()
    {
        if (Input.GetButton("Forward") || Input.GetAxis("Forward") > 0) ;
        {
            engine += engineAcceleration * Time.deltaTime * Input.GetAxis("Forward");

            if (engine > 1)
            {
                engine = 1;
            }
            else if (engine < 0)
            {
                engine = 0;
            }
        }

        maxAllowedSpeed = maxSpeed * engine;

        if (zVel < maxAllowedSpeed)
        {
            _acceleration = engineAcceleration * maxSpeed * Time.deltaTime * accelerationMultiplier * _massMultiplier;
        }
        else if (zVel > maxAllowedSpeed && flightAssist)
        {
            newVel = zVel - maxAllowedSpeed;
            _acceleration = newVel * -5;
        }else if(zVel > maxSpeed)
        {
            newVel = zVel - maxAllowedSpeed;
            _acceleration = newVel * -0.1f;
        }

    }

    void HorizontalMovement()
    {

        _horizontalAcceleration = Input.GetAxis("Horizontal") * Time.deltaTime * 200 * _massMultiplier;

        if(xVel > maxHorizontalSpeed)
        {
            newVel = xVel - maxHorizontalSpeed;
            _horizontalAcceleration = newVel * -1;
        }else if(xVel < maxHorizontalSpeed * -1)
        {
            newVel = xVel * -1 - maxHorizontalSpeed;
            _horizontalAcceleration = newVel;
        } 
    }

    void VerticalMovement() {

        _verticalAcceleration = Input.GetAxis("Vertical") * Time.deltaTime * 200 * _massMultiplier;

        if (yVel > maxVerticalSpeed)
        {
            newVel = yVel - maxVerticalSpeed;
            _verticalAcceleration = newVel * -1;
        }
        else if (yVel < maxVerticalSpeed * -1)
        {
            newVel = yVel * -1 - maxVerticalSpeed;
            _verticalAcceleration = newVel;
        }
    }

    void ApplyThrottle()
    {
        _body.AddForce(_spaceship.transform.forward * _acceleration);
        _body.AddForce(_spaceship.transform.up * _verticalAcceleration);
        _body.AddForce(_spaceship.transform.right * _horizontalAcceleration);
    }

    void SetTurn()
    {
        turn = turnSpeed * Time.deltaTime * Input.GetAxis("Yaw") * _massMultiplier * 2;
    }

    void Pitch()
    {
        pitch = turnSpeed * Time.deltaTime * Input.GetAxis("Pitch") * -1 * _massMultiplier * 2;
    }

    void Roll()
    {
        roll = turnSpeed * Time.deltaTime * Input.GetAxis("Roll") * -1 * _massMultiplier * 4;
    }

    void ApplyTurn()
    {
        _body.AddTorque(gameObject.transform.up * turn);
        _body.AddTorque(gameObject.transform.forward * roll);
        _body.AddTorque(gameObject.transform.right * pitch);
    }



    void FireProjectile()
    {
        if ((Input.GetButton("Fire1") || Input.GetAxis("Fire1") > 0)&& CanShoot)
        {
            GetComponent<PhotonView>().RPC("PlayAudioShoot", PhotonTargets.All);
            NewProjectile = PhotonNetwork.Instantiate(projectile.name, projectileSpawner.transform.position, projectileSpawner.transform.rotation, 0) as GameObject; //This line for photon
            NewProjectile.GetComponent<PhotonView>().RPC("SetOwner", PhotonTargets.All, owner);
            CanShoot = false;
            timer = RateOfFire;
        }
    }

    void ShootTimer()
    {
        timer -= Time.deltaTime;
        if (timer < 0)
        {
            CanShoot = true;
        }
    }

    void Stabilization()
    {
        if (!Input.GetButton("Vertical"))
        {
            _body.AddForce(_spaceship.transform.up * yVel * -1 * _massMultiplier);
        }
        if (!Input.GetButton("Horizontal"))
        {
            _body.AddForce(_spaceship.transform.right * xVel * -1 * _massMultiplier);
        }
    }

    [PunRPC]
    public void PlayAudioShoot()
    {
        audio.Play();
    }

    #endregion

    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting == true)
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
            stream.SendNext(engine);
            stream.SendNext(flightBoost);
        }
        else
        {
            transform.position = (Vector3)stream.ReceiveNext();
            transform.rotation = (Quaternion)stream.ReceiveNext();
            engine = (float)stream.ReceiveNext();
            flightBoost = (bool)stream.ReceiveNext();
        }
    }

    void ToggleFligtAssist()
    {
        if(Input.GetButtonDown("ToggleFlightAssist")) flightAssist = !flightAssist;
    }

    void DetectFlightBoost()
    {
        if ((Input.GetButton("Fire2") || Input.GetAxis("Fire2") > 0) && boostTimer > 0)
        {
            FlightBoost();
        }
        else if (Input.GetButtonUp("Fire2") || Input.GetAxis("Fire2") == 0)
        {
            EndBoost();
        }
    }

    void FlightBoost()
    {
        if (!flightBoost && boostTimer > 0)
        {
            maxSpeed *= 1.5f;
            accelerationMultiplier *= 3;
            flightBoost = true;
            //Lerp Engine
            lerpEngine = true;
            boostParticleEffect.transform.GetChild(2).GetComponent<ParticleSystem>().Play();
            GetComponent<PhotonView>().RPC("PlayEffect", PhotonTargets.All);
        }
    }

    void EndBoost()
    {
        maxSpeed = tmpMaxSpeed;
        accelerationMultiplier = tmpAccelerationMultiplier;
        if (flightBoost) {
            GetComponent<PhotonView>().RPC("StopEffect", PhotonTargets.All);
        }
        flightBoost = false;
        boostParticleEffect.transform.GetChild(2).GetComponent<ParticleSystem>().Stop();
    }

    void Cooldown()
    {
        if (!flightBoost && boostTimer < 10.0f)
        {
            boostCooldown -= Time.deltaTime;
            if (boostCooldown < 0)
            {
                boostCooldown = 20.0f;
                boostTimer = 10.0f;
            }
        }
        else if (flightBoost)
        {
            boostCooldown = 20.0f;
        }
    }
    
    void Boost()
    {
        if (flightBoost)
        {
            boostTimer -= Time.deltaTime;
        }
        if(boostTimer < 0)
        {
            EndBoost();
        }
    }

    [PunRPC]
    public void PlayEffect()
    {
        boostParticleEffect.transform.GetChild(0).GetComponent<TrailRenderer>().time = 1;
        boostParticleEffect.transform.GetChild(1).GetComponent<TrailRenderer>().time = 1;
        boostParticleEffect.GetComponent<AudioSource>().Play();
        boostParticleEffect.GetComponent<AudioSource>().volume = 1;
    }

    [PunRPC]
    public void StopEffect()
    {
        boostParticleEffect.transform.GetChild(0).GetComponent<TrailRenderer>().time = 0;
        boostParticleEffect.transform.GetChild(1).GetComponent<TrailRenderer>().time = 0;
        audioEnd = true;
    }

    void EndAudioEffect()
    {
        boostParticleEffect.GetComponent<AudioSource>().volume -= (Time.deltaTime * 0.4f);
        if(boostParticleEffect.GetComponent<AudioSource>().volume <= 0)
        {
            boostParticleEffect.GetComponent<AudioSource>().Stop();
            audioEnd = false;
        }
    }
}