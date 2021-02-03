using UnityEngine;

namespace Knockback.Utility
{
    /// <summary>
    /// <para> Add this interface to any class that can be interacted and used </para>
    /// Compatible scripts : any items that can be used
    /// </summary>
    public interface IUsableEntity
    {
        bool i_canUse { get; set; }
        void UseItem(GameObject source);
    }

    /// <summary>
    /// <para> Derive this interface to objects that can be interacted by the player </para>
    /// Compatible scripts : any item that has Interactability script attached
    /// </summary>
    public interface IInteractableEntity
    {
        bool canUse { get; set; }
        void StartUse();
        void StopUse();
    }

    /// <summary>
    /// <para> Derive this to any objects that can take damage </para>
    /// Compatible scripts : any item that can take damage
    /// </summary>
    public interface IDamage
    {
        void ApplyDamage(float damage, GameObject source);
        void RemoveDamage(float damage);
    }

    /// <summary>
    /// This interface is used as a function parameter for custom events
    /// </summary>
    public interface IMessage
    {
        object data { get; set; }
        GameObject source { get; set; }
        float timeUntilActivation { get; set; }
    }

    /// <summary>
    /// This interface is used for calling custom actions or routine of operations
    /// </summary>
    public interface IUIAction
    {
        void DoAction(IMessage _message = null);
    }
}