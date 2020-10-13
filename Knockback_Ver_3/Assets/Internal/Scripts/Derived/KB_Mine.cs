﻿using UnityEngine;
using System.Collections;
using Knockback.Core;

public class KB_Mine : KB_ThrowableCore
{
    //todo: Commenting :: Mine
    //todo: Network implementation
    [Header("Hand grenade backend settings")]
    [Space]

    [SerializeField] private float throwVelocity = 5;
    [SerializeField] private float maxDamageRadius = 5;
    [SerializeField] private float maxDamageAmount = 110;
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private LayerMask layerMask;

    public bool canUse { get; set; } = false;

    public void UseItem(GameObject source)
    {
        if (!canUse)
            return;
        RemoveFromInventory(source);
        Throw(transform.rotation * Vector2.right, throwVelocity);
    }

    protected override void OnFinishSplashDamage()
    {
        //todo: Change the sprite inside the sprite renderer
        Destroy(gameObject, 0.5f);
    }

    protected override void OnHit(Collision2D collider)
    {
        // Do some checking if necessary
        transform.position = collider.transform.position;
        transform.parent = collider.transform;
    }

    public void DoIfPlayerCloseBy()
    {
        //todo: Play some animation
        ApplySplashDamage(transform.position, maxDamageRadius, maxDamageAmount, layerMask);
    }

    /// <summary>
    /// Remove this item from the player inventory
    /// </summary>
    /// <param name="source"></param>
    private void RemoveFromInventory(GameObject source)
    {
        if (source == null)
            return;
        KB_PlayerInventoryHandler inventory;
        if (source.TryGetComponent(out inventory))
            inventory.RemoveItemFromInventory(gameObject);
    }
}
