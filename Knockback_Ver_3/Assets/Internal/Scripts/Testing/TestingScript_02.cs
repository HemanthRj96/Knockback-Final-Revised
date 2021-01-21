using Knockback.Utility;
using System.Collections.Generic;
using UnityEngine;


public class TestingScript_02 : MonoBehaviour, IUsableEntity
{
    [SerializeField] private GameObject bulletPrefab = null;
    [SerializeField] private Transform bulletTransform = null;
    [SerializeField] private float velocity;
    [SerializeField] private float impactDamage;

    public bool canUse { get; set; } = true;


    private GameObject bulletInstance;

    public void UseItem(GameObject source)
    {
        bulletInstance = Instantiate(bulletPrefab, bulletTransform.position, bulletTransform.rotation);
        Destroy(bulletInstance, 20);        
        bulletInstance.GetComponent<KB_BulletCore>().SetBulletParameters(impactDamage, bulletTransform.rotation * Vector2.right, velocity);
        bulletInstance.GetComponent<IUsableEntity>().UseItem(gameObject);
    }    
}