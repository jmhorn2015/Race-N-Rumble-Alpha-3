﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControlSetup : MonoBehaviour {
    bool animOn;
    bool isRun;
    bool isCrouch;
    bool crouchCheck;
    bool isJump;
    bool isFall;
    public bool isPowerUp;
    public bool curse;
    bool isPower;
    bool fadeIn;
    bool fadeOut;
    public int PlayerNum;
    float prevy;
    Color powerColor;
    Vector2 offs;
    Vector2 sizer;
    AudioClip audioJump;
    public AudioClip powerUp;
    public AudioClip powerActive;
    public AudioClip powerDown;
    CapsuleCollider2D cc;
    public float speedCheck;
    public float orig_speed = 5f;
    public float base_speed;
    public Vector2 base_jumpHeight = new Vector2(0.0f, 8.0f);
    public Animator animator;
    public Quaternion rotator;

    public void AddPlayerNum(int x)
    {
        PlayerNum = x;
    }

    public void Start()
    {
        speedCheck = orig_speed;
        base_speed = speedCheck*Time.deltaTime;
        crouchCheck = false;
        Invoke("SetAnimator", .5f);
        prevy = gameObject.transform.position.y;
        isCrouch = false;
        isPowerUp = false;
        isPower = false;
        animOn = false;
        cc = gameObject.GetComponent<CapsuleCollider2D>();
        offs = cc.offset;
        sizer = cc.size;
        audioJump = Resources.Load<AudioClip>("Audio/SE/Jump") as AudioClip;
        powerUp = Resources.Load<AudioClip>("Audio/Powers/PowerUp") as AudioClip;
        powerDown = Resources.Load<AudioClip>("Audio/Powers/PowerDown") as AudioClip;
        powerColor = gameObject.GetComponentsInChildren<SpriteRenderer>()[0].color;
    }

    void SetAnimator()
    {
        animator = gameObject.GetComponentsInChildren<Animator>()[0];
        animOn = true;
    }

    void Update()
    {
        base_speed = speedCheck*Time.deltaTime;
        if (gameObject.transform.position.y - prevy > 0.05)
        {
            isJump = true;
        }
        else
        {
            isJump = false;
        }
        isRun = false;
        isPower = false;
        if (curse)
        {
            fadeIn = true;
            Invoke("startFadeOut", 3f);
            curse = false;
        }
        if (fadeIn == true)
        {
            if (powerColor.a < 1)
            {
                powerColor.a += .05f;
            }
            else
            {
                fadeIn = false;
            }
            gameObject.GetComponentsInChildren<SpriteRenderer>()[0].color = powerColor;
        }
        if (fadeOut == true)
        {
            if (powerColor.a > 0)
            {
                powerColor.a -= .05f;
            }
            else
            {
                fadeOut = false;
                isPowerUp = false;
            }
            gameObject.GetComponentsInChildren<SpriteRenderer>()[0].color = powerColor;
        }
        if (InputManager.MainHorizontal(PlayerNum) < - .25f)
        {
            moveToLeft();
            isRun = true;
        }
        if (InputManager.MainHorizontal(PlayerNum) > .25f)
        {
            moveToRight();
            isRun = true;
        }
        if(InputManager.Jump(PlayerNum))
        {
            AudioSource.PlayClipAtPoint(audioJump, new Vector2(0f,0f));
            playerJump();
        }
        if(InputManager.Power(PlayerNum))
        {
            if (!isPowerUp)
            {
                isPower = true;
                usePowerUp();
            }
        }
        if(InputManager.Croutch(PlayerNum))
        {
            Debug.Log("croutch true");
            isCrouch = true;
            speedCheck = 2.5f;
            crouch();
        }
        if (!InputManager.Croutch(PlayerNum))
        {
            isCrouch = false;
            crouch();
        }
        if(InputManager.Croutch(PlayerNum) == false & crouchCheck == true)
        {
            speedCheck = orig_speed;
        }
        if (animOn == true)
        {
            animator.SetBool("isRunning", isRun);
            animator.SetBool("isCrouching", isCrouch);
            animator.SetBool("isJump", isJump);
            animator.SetBool("isPower", isPower);
        }
        prevy = gameObject.transform.position.y;
        crouchCheck = InputManager.Croutch(PlayerNum);
    }

	virtual public void moveToLeft()
    {
        Vector2 position = this.transform.position;
        position.x -= base_speed;
        this.transform.position = position;
        if (this.transform.rotation.y == 1f)
        {
            rotator = new Quaternion(0f, 0f, 0f, 0f);
            this.transform.rotation = rotator;
        }
    }

    virtual public void moveToRight()
    {
        Vector2 position = this.transform.position;
        position.x += base_speed;
        this.transform.position = position;
        if (this.transform.rotation.y == 0f)
        {
            rotator = new Quaternion(0f, 1f, 0f, 0f);
            this.transform.rotation = rotator;
        }
    }

    virtual public void playerJump()
    {
        GetComponent<Rigidbody2D>().AddForce(base_jumpHeight, ForceMode2D.Impulse);
    }

    virtual public void usePowerUp()
    {
        if (SaveState.PowerUpLeft[PlayerNum - 1] > 0)
        {
            AudioSource.PlayClipAtPoint(powerUp, new Vector2(0f, 0f));
            gameObject.GetComponent<ParticleSystem>().Play();
            isPowerUp = true;
            fadeIn = true;
            Invoke("startFadeOut", 5f);
            SaveState.PowerUpLeft[PlayerNum - 1]--;
        }
        else
        {
            Debug.Log("No more Powerups");
        }

    }

    void startFadeOut()
    {
        fadeOut = true;
        AudioSource.PlayClipAtPoint(powerDown, new Vector2(0f, 0f));
    }

    virtual public void crouch()
    {
        if (isCrouch)
        {
            offs.y = -1.068107f;
            sizer.y = 0.833f;
        }
        else
        {
            offs.y = -.4269078f;
            sizer.y = 2.116148f;
        }
        cc.offset = offs;
        cc.size = sizer;
    }
}
