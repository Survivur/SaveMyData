using Photon.Pun;
using UnityEngine;

public class Player : MoveableCharacter
{
    protected override void Start()
    {
        base.Start();
        if (!photonView.IsMine)  // ??? ??????? ???? ??? ???? ????
        {
            enabled = false;  // ?????? ??????
            return;
        }
    }

    protected override void Update()
    {
        KeyChecker();
        base.Update();
    }

    protected override void FixedUpdate()
    {
        HorizontalInput = Input.GetAxisRaw("Horizontal");
        base.FixedUpdate();
    }

    void KeyChecker()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Shoot();
        }
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            photonView.RPC("RPC_Dash", RpcTarget.All);
        }
        if (Input.GetButtonDown("Jump"))
        {
            if (!jump.isJumping)
            {
                photonView.RPC("RPC_Jump", RpcTarget.All);
            }
        }
    }
    
}
