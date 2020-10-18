using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkSelfDestruct : NetworkBehaviour
{

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();

        base.Invoke("CmdDestroyMe", 5f);
    }


    [Command]
    private void CmdDestroyMe()
    {
        NetworkServer.Destroy(gameObject);
    }
}
