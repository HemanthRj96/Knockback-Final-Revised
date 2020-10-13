using Mirror;
using System;
using UnityEngine;

public class LocalPlayerAnnouncer : NetworkBehaviour
{

    /// <summary>
    /// Dispatched when the local player changes, providing the new localPlayer.
    /// </summary>
    public static event Action<NetworkIdentity> OnLocalPlayerUpdated;

    public override void OnStartLocalPlayer()
    {
        OnLocalPlayerUpdated?.Invoke(base.netIdentity);
    }

    //private void OnDestroy()
    //{
    //    if (base.isLocalPlayer)
    //        OnLocalPlayerUpdated?.Invoke(null);
    //}

    private void OnEnable()
    {
        if (base.isLocalPlayer)
            OnLocalPlayerUpdated?.Invoke(base.netIdentity);
    }
    private void OnDisable()
    {
        if (base.isLocalPlayer)
            OnLocalPlayerUpdated?.Invoke(null);
    }

}
