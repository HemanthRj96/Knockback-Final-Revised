using UnityEngine;

namespace Knockback.Utility
{
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
        bool canUse { get; set; }
        void StartUse();
        void StopUse();
    }

    /// <summary>
    /// Derive this to any objects that can take damage
    /// </summary>
    public interface IDamage
    {
        void ApplyDamage(float damage);
        void RemoveDamage(float damage);
    }

    /// <summary>
    /// 
    /// </summary>
    public interface IMessage
    {
        object data { get; set; }
        GameObject source { get; set; }
        float timeUntilActivation { get; set; }
    }
}