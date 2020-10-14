﻿using Knockback.Controllers;
using Knockback.Core;
using Knockback.Utility;
using Mirror.Examples.Additive;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KB_HandGrenade : KB_ThrowableCore, IUsableEntity
{
    //todo: Commenting :: HandGrenade
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
        Throw(transform.rotation * Vector2.right, throwVelocity, 3, true);
    }

    protected override void OnFinishSplashDamage()
    {
        //todo: Change the sprite inside the sprite renderer
        Destroy(gameObject, 0.5f);
    }

    protected override void OnTimerEnd()
    {
        //todo: Add the explosion effect here
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