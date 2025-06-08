using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Player : Character
{
    public bool isAnimating => animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f;

    [Header("Animation", order = 0)]
    [SerializeField, ReadOnly] private bool needCheckingAnimate = true;

    [Header("Components", order = 0)]
    [SerializeField, ReadOnly(true)] private Animator animator = null;
    [SerializeField, ReadOnly(true)] private PlayerDash playerDash = null;
    [SerializeField, ReadOnly(true)] private PlayerJump playerJump = null;
    [SerializeField, ReadOnly(true)] private PlayerGun playerGun = null;

    [Header("Children", order = 0)]
    [SerializeField, ReadOnly(true)] private GameObject _upsideChild = null;
    [SerializeField, ReadOnly(true)] private GameObject _downsideChild = null;
    [SerializeField, ReadOnly(true)] private GameObject _armChild = null;
    [SerializeField, ReadOnly(true)] private GameObject _parrychild = null;


    public GameObject UpsideChild => CodeExtensions.SetIfUnityNull(
        ref _upsideChild,
        GameObjectRegistry.GetOrRegister(ObjectPath.Player_Upside, gameObject)
    );

    public GameObject DownsideChild => CodeExtensions.SetIfUnityNull(
        ref _downsideChild,
        GameObjectRegistry.GetOrRegister(ObjectPath.Player_Downside, gameObject)
    );

    public GameObject ArmChild => CodeExtensions.SetIfUnityNull(
        ref _armChild,
        GameObjectRegistry.GetOrRegister(ObjectPath.Player_Arm, gameObject)
    ); 

    public GameObject ParryChild => CodeExtensions.SetIfUnityNull(
        ref _parrychild,
        GameObjectRegistry.GetOrRegister(ObjectPath.Player_Arm, gameObject)
    ); 
    
    public override List<string> TargetTags { get; protected set; } = new List<string>();

    protected override void Start()
    {
        // if (!photonView.IsMine)  // ??? ??????? ???? ??? ???? ????
        // {
        //     enabled = false;  // ?????? ??????
        //     return;
        // }

        CodeExtensions.SetIfNullOrEmpty(ref nameUI.str, "John Wick");
        CodeExtensions.SetIfNullOrEmpty(ref nameUI.strTextName, "PlayerName");

        base.Start();

        CodeExtensions.SetIfUnityNull(ref animator, gameObject.GetComponentCached<Animator>());
        CodeExtensions.SetIfUnityNull(ref playerDash, gameObject.GetComponentCached<PlayerDash>());
        CodeExtensions.SetIfUnityNull(ref playerJump, gameObject.GetComponentCached<PlayerJump>());
        CodeExtensions.SetIfUnityNull(ref playerGun, gameObject.GetComponentCached<PlayerGun>());

        //CodeExtensions.SetIfUnityNull(ref dash, GetComponent<PlayerDash>());
        //CodeExtensions.SetIfUnityNull(ref DownsideChildSprite, DownsideChild.GetComponent<SpriteRenderer>());

        UpsideChild.SetActive(false);
        DownsideChild.SetActive(false);
        ArmChild.SetActive(false);
        ParryChild.SetActive(false);

        TargetTags.Add(Tags.Enemy);
    }

    protected override void Update()
    {
        KeyChecker();
        base.Update();
        AnimationCheck();
        photonView.RPC(nameof(RPC_SetAim), RpcTarget.All);
    }

    void KeyChecker()
    {
        if (Input.GetMouseButtonDown(0))
        {
            playerGun.Shoot(Damage, AimDirection);
        }
        if (Input.GetMouseButtonDown(1))
        {
            photonView.RPC(nameof(RPC_Jump), RpcTarget.All);
        }
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            photonView.RPC(nameof(RPC_Dash), RpcTarget.All);
        }
        if (Input.GetButtonDown("Jump"))
        {
            photonView.RPC(nameof(RPC_Jump), RpcTarget.All);
        }
    }

    private void AnimationCheck()
    {
        if (needCheckingAnimate && !isAnimating)
        {
            animator.enabled = false;
            spriteRenderer.enabled = false;

            UpsideChild.SetActive(true);
            DownsideChild.SetActive(true);
            ArmChild.SetActive(true);

            needCheckingAnimate = false;
        }
    }

    [PunRPC]
    public void RPC_Dash()
    {
        playerDash.Ready();
    }

    [PunRPC]
    public void RPC_Jump()
    {
        playerJump.Ready();
    }

    [PunRPC]
    public void RPC_SetAim()
    {
        AimDirection = ((Vector2)(Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position)).normalized;
    }

}