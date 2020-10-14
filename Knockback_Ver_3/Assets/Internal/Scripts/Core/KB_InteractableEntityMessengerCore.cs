using UnityEngine;
using System.Collections;
using Knockback.Controllers;
using Knockback.Handlers;

namespace Knockback.Core
{
    public class KB_InteractableEntityMessengerCore : MonoBehaviour
    {
        [Header("Messenger backend settings")]
        [Space]

        [SerializeField] private int messengerId;


        public void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.GetComponent<KB_PlayerController>())
                KB_EventHandler.instance.Invoke("InteractiveItem", messengerId);
        }

        public void OnTriggerExit2D(Collider2D collision)
        {
            
        }
    }
}