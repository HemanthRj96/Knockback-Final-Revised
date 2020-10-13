using UnityEngine;


/// <summary>
/// <para> Add this interface to any class that can be interacted and used </para>
/// Compatible scripts : any weapon scripts
/// </summary>
public interface IUsableEntity
{
    bool canUse { get; set; }
    void UseItem(GameObject source);
}

/// <summary>
/// Derive this interface to objects that can be interacted by the player
/// </summary>
public interface IInteractableEntity
{
    void OnUse();
}

/// <summary>
/// Derive this to any objects that can take damage
/// </summary>
public interface IDamage
{
    void AddDamage(float damage);
}