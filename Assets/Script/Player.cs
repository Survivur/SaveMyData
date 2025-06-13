using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Player : Character
{
    public bool isAnimating => animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f;

    [Header("Info", order = 0)]
    [SerializeField, ReadOnly] private bool needCheckingAnimate = true;

    [Header("Components", order = 0)]
    [SerializeField, ReadOnly(true)] private Animator animator = null;
    [SerializeField, ReadOnly(true)] private PlayerDash playerDash = null;
    [SerializeField, ReadOnly(true)] private PlayerJump playerJump = null;
    [SerializeField, ReadOnly(true)] private PlayerGun playerGun = null;
    [SerializeField, ReadOnly(true)] private PlayerGhost playerGhost = null;
    [SerializeField, ReadOnly(true)] private PlayerParry playerParry = null;

    [Header("Children", order = 0)]
    [SerializeField, ReadOnly(true)] private GameObject _upsideChild = null;
    [SerializeField, ReadOnly(true)] private GameObject _downsideChild = null;
    [SerializeField, ReadOnly(true)] private GameObject _armChild = null;

    public bool isSit = false;

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

    protected override void Start()
    {
        CodeExtensions.SetIfNullOrEmpty(ref nameUI.str, "John Wick");
        CodeExtensions.SetIfNullOrEmpty(ref nameUI.strTextName, "PlayerName");

        base.Start();

        CodeExtensions.SetIfUnityNull(ref animator, gameObject.GetComponentCached<Animator>());
        CodeExtensions.SetIfUnityNull(ref playerDash, gameObject.GetComponentCached<PlayerDash>());
        CodeExtensions.SetIfUnityNull(ref playerJump, gameObject.GetComponentCached<PlayerJump>());
        CodeExtensions.SetIfUnityNull(ref playerGun, gameObject.GetComponentCached<PlayerGun>());
        CodeExtensions.SetIfUnityNull(ref playerGhost, gameObject.GetComponentCached<PlayerGhost>());
        CodeExtensions.SetIfUnityNull(ref playerParry, gameObject.GetComponent<PlayerParry>());

        //CodeExtensions.SetIfUnityNull(ref dash, GetComponent<PlayerDash>());
        //CodeExtensions.SetIfUnityNull(ref DownsideChildSprite, DownsideChild.GetComponent<SpriteRenderer>());

        UpsideChild.SetActive(false);
        DownsideChild.SetActive(false);
        ArmChild.SetActive(false);

        if (photonView.IsMine)
        {            
            gameObject.tag = Tags.Player;
            playerParry.ParryChild.tag = Tags.Player;
            //photonView.RPC(nameof(RPC_UISync), RpcTarget.All);
            GameObjectRegistry.GetOrRegister("Canvas/Game Panel/GameObject/Player1/HP Group/HP Text").GetComponent<GetTextGUI>().getTextFunc = () => $"Health : {Health} / {MaxHealth}";
            GameObjectRegistry.GetOrRegister("Canvas/Game Panel/GameObject/Player1/Status Group/Player Text").GetComponent<GetTextGUI>().getTextFunc = () => $"{nameUI.str}";
            TargetTags.Add(Tags.Enemy);

        }
        else // ??? ??????? ???? ??? ???? ????
        {
            gameObject.tag = Tags.Enemy;
            playerParry.ParryChild.tag = Tags.Enemy;
            GameObjectRegistry.GetOrRegister("Canvas/Game Panel/GameObject/Player2/HP Group/HP Text").GetComponent<GetTextGUI>().getTextFunc = () => $"Health : {Health} / {MaxHealth}";
            GameObjectRegistry.GetOrRegister("Canvas/Game Panel/GameObject/Player2/Status Group/Player Text").GetComponent<GetTextGUI>().getTextFunc = () => $"{nameUI.str}";
            TargetTags.Add(Tags.Player);
        }
        playerParry.ParryChild.SetActive(false);

    }

    protected void Update()
    {
        AnimationCheck();
        if (photonView.IsMine)
        {
            KeyChecker();
            photonView.RPC(nameof(RPC_SetAim), RpcTarget.All, Camera.main.ScreenToWorldPoint(Input.mousePosition));
        }
    }

    void KeyChecker()
    {
        if (Input.GetMouseButtonDown(0))
        {
            playerGun.Shoot(Damage, AimDirection);
        }
        if (Input.GetMouseButtonDown(1))
        {
            photonView.RPC(nameof(RPC_Parry), RpcTarget.All);
        }
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            photonView.RPC(nameof(RPC_Dash), RpcTarget.All);
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            photonView.RPC(nameof(RPC_Reload), RpcTarget.All);
        }
        if (Input.GetButtonDown("Jump"))
        {
            photonView.RPC(nameof(RPC_Jump), RpcTarget.All);
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            photonView.RPC(nameof(RPC_Sit), RpcTarget.All);
        }
        if (Input.GetKeyUp(KeyCode.S))
        {
            photonView.RPC(nameof(RPC_Stand), RpcTarget.All);
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

        // animator.SetBool("needChange", prevCount != playerJump.Counter.Count);
        // animator.SetBool("isJump", playerJump.IsJumping);
        // animator.SetBool("isSit", isSit);
        // animator.SetBool("isWalk", playerMove.Velocity.x != 0);


        // prevCount = playerJump.Counter.Count;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        IHittable hitable = other.GetComponent<IHittable>();    
        if (hitable != null)
        {
            hitable?.TakeDamage(Damage, (Vector2)transform.right);
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
    public void RPC_Reload()
    {
        playerGun.Reload();
    }

    [PunRPC]
    public void RPC_SetAim(Vector3 aim)
    {
        AimDirection = ((Vector2)(aim - transform.position)).normalized;
    }

    [PunRPC]
    public void RPC_Parry()
    {
        playerParry.Parry();
    }

    [PunRPC]
    public void RPC_Sit()
    {
        //_downsideChild.GetComponentCached<PlayerDownsideAnimation>().isSit = true;
    }

    [PunRPC]
    public void RPC_Stand()
    {
        //_downsideChild.GetComponentCached<PlayerDownsideAnimation>().isSit = false;
    }

    [PunRPC]
    public void RPC_UISync()
    {
    }    
}