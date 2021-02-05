using Mirror;
using UnityEngine;

namespace FirstGearGames.Mirrors.StrippingCodeFromBuilds
{

    public class StrippedCodeExample : NetworkBehaviour
    {
        [SerializeField]
        private AudioClip _teleportAudio;

        private void Update()
        {
            if (base.hasAuthority)
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    CmdTeleport();
                    PlayTeleportAudio();
                }
            }
        }


        [Client]
        private void PlayTeleportAudio()
        {
#if !UNITY_SERVER
            AudioSource.PlayClipAtPoint(_teleportAudio, transform.position);
#endif
        }

        [Command]
        private void CmdTeleport() //strip 
        {
#if UNITY_SERVER || UNITY_EDITOR
            transform.position += Random.insideUnitSphere * 3f;
#endif
        }
    }


}