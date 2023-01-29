using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
using FishNet.Connection;

public class Target : NetworkBehaviour
{
    private Animator animator;
    private Score score;

    private bool blockAnim;

    private void Start()
    {
        animator = GetComponent<Animator>();
        score = FindObjectOfType<Score>();
    }

    public void TargetHit()
    {
        animator.SetTrigger("hit");
        blockAnim = true;
        CmdHitAnimation();
    }
    [ServerRpc(RequireOwnership = false)]
    public void CmdTargetHit(NetworkConnection sender = null) => score.ChangePoint(sender, 1);

    [ServerRpc(RequireOwnership = false)]
    private void CmdHitAnimation() => RpcHitAnimation();
    [ObserversRpc]
    private void RpcHitAnimation()
    {
        if (!blockAnim)
            animator.SetTrigger("hit");

        blockAnim = false;
    }
}