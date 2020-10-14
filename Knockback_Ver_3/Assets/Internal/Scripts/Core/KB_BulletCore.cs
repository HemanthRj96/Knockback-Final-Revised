using Knockback.Utility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KB_BulletCore : MonoBehaviour, IUsableEntity
{
    [Header("Bullet backend settings")]
    [Space]

    //*** Back end variable ***//

    public LayerMask layer;
    private Vector2 pointA;
    private Vector2 pointB;
    private float impactDamage = 0;
    private float velocity;
    private Vector2 direction;
    private List<GameObject> collidedObjects = new List<GameObject>();

    //*** Local variables ***//

    private bool canDetect = true;
    
    //*** Reference variable ***//

    public bool canUse { get; set; } = true;

    //*** Public functions ***//

    public void UseItem(GameObject source)
    {
        StartCoroutine(Fire(source));
    }

    public void SetBulletParameters(float impactDamage, Vector2 direction, float velocity)
    {
        this.impactDamage = impactDamage;
        this.direction = direction;
        this.velocity = velocity;
    }

    //*** Private functions ***//

    private void OnDestroy()
    {
        StopAllCoroutines();
    }

    private IEnumerator Fire(GameObject source)
    {
        while (canDetect)
        {
            pointA = transform.position;
            yield return null;
            pointB = transform.position;
            RaycastHit2D hit = Physics2D.Raycast(pointA, transform.rotation * Vector3.right, Vector3.Distance(pointA, pointB), layer);
            if (hit.collider != null)
            {
                if (hit.collider.gameObject != source)
                {
                    collidedObjects.Add(hit.collider.gameObject);
                    OnHit(hit.collider.gameObject);
                }
            }
        }
    }

    private void OnHit(GameObject collidedObject)
    {
        // Do some checking here to enable more control to bullet impact
        //if (collidedObject ==)
        //{

        //    canDetect = false;
        //}

        Destroy(gameObject);
    }
}
