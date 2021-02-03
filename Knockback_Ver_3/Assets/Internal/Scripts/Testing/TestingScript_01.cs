using Knockback.Helpers;
using Knockback.Utility;
using System;
using UnityEngine;

namespace Knockback.Testing
{
    public class TestingScript_01 : MonoBehaviour
    {
        [SerializeField] private GameObject slot = null;

        private GameObject item => slot.transform.GetChild(0).gameObject;

        float rotationInDegrees;


        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
                UseGun();
            UpdateSlotRotation();
        }

        private void UpdateSlotRotation() => slot.transform.rotation = GetCalculatedRotation();

        private void UseGun()
        {
            item.GetComponent<IUsableEntity>().UseItem(gameObject);
        }

        public Quaternion GetCalculatedRotation()
        {
            Vector2 difference = Vector2.zero;
            Quaternion targetRotation = Quaternion.identity;

            difference = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
            difference.Normalize();

            if (difference != Vector2.zero)
                rotationInDegrees = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;

            targetRotation = Quaternion.Euler(0, 0, rotationInDegrees);
            return targetRotation;
        }
    }
}