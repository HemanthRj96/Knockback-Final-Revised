using Mirror;
using UnityEngine;


public class TestingScript_01 : NetworkBehaviour
{
    [Client]
    private void Update()
    {
        if (!hasAuthority)
            return;

        if(Input.GetKeyDown(KeyCode.Space))
        CmdMove();
    }

    [Command]
    private void CmdMove()
    {
        RpcMove();
    }

    [ClientRpc]
    private void RpcMove() => transform.Translate(1, 0, 0);
}
