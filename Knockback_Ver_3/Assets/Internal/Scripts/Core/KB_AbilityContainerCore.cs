using System.Collections.Generic;
using UnityEngine;

namespace Knockback.Core
{
    [CreateAssetMenu(fileName = "KB_AbilityContainerCore", menuName = "Ability Container/Container", order = 0)]
    public class KB_AbilityContainerCore : ScriptableObject
    {
        [Header("Ability container backend settings")]
        [Space]

        [SerializeField]
        private List<AbilityContainer> persistentContainer = new List<AbilityContainer>();
        private Dictionary<int, AbilityContainer> _persistentContainerCollection_1 = new Dictionary<int, AbilityContainer>();
        private Dictionary<string, AbilityContainer> _persistentContainerCollection_2 = new Dictionary<string, AbilityContainer>();
        private List<AbilityContainer> activeContainerAbilities = new List<AbilityContainer>();

        private List<AbilityContainer> temporaryContainer = new List<AbilityContainer>();

        [ContextMenu("Initialize collection")]
        private void InitializeColllection()
        {
            _persistentContainerCollection_1.Clear();
            _persistentContainerCollection_2.Clear();

            foreach(AbilityContainer container in  persistentContainer)
            {
                if(_persistentContainerCollection_1.ContainsKey(container.containerId))
                {
                    Debug.Log($"The Id {container.containerId} exists already, please change the Id");
                    continue;
                }
                if(_persistentContainerCollection_2.ContainsKey(container.containerTag))
                {
                    Debug.Log($"The tag {container.containerTag} exists already, please change the tag");
                    continue;
                }

                _persistentContainerCollection_1.Add(container.containerId, container);
                _persistentContainerCollection_2.Add(container.containerTag, container);
            }
        }


        public void ActivateContainerAbility(int containerId)
        {
            if (!_persistentContainerCollection_1.ContainsKey(containerId))
                return;
            foreach(KeyValuePair<int, AbilityContainer> container in _persistentContainerCollection_1)
            {
                if (container.Key == containerId)
                {
                    AbilityActivater(container.Value);
                    activeContainerAbilities.Add(container.Value);
                    return;
                }
            }
        }

        public void ActivateContainerAbility(string containerTag)
        {
            if(_persistentContainerCollection_2.ContainsKey(containerTag))
                return;
            foreach (KeyValuePair<string, AbilityContainer> container in _persistentContainerCollection_2)
            {
                if (container.Key == containerTag)
                {
                    AbilityActivater(container.Value);
                    activeContainerAbilities.Add(container.Value);
                    return;
                }
            }
        }

        public void DeactivateContainerAbility(int containerId)
        {
            foreach(AbilityContainer activeAbility in activeContainerAbilities)
            {
                if(activeAbility.containerId == containerId)
                {
                    activeContainerAbilities.Remove(activeAbility);
                    AbilityDeactivator(activeAbility);
                    break;
                }
            }
        }

        public void DeactivateContainerAbility(string containerTag)
        {
            foreach (AbilityContainer activeAbility in activeContainerAbilities)
            {
                if (activeAbility.containerTag == containerTag)
                {
                    activeContainerAbilities.Remove(activeAbility);
                    AbilityDeactivator(activeAbility);
                    break;
                }
            }
        }


        private void AbilityActivater(AbilityContainer container)
        {
            foreach(KB_AbilityCore ability in container.unitAbilities)
            {
                ability.ApplyEffect();
            }
        }

        private void AbilityDeactivator(AbilityContainer container)
        {
            foreach(KB_AbilityCore ability in container.unitAbilities)
            {
                ability.RemoveEffect();
            }
        }


    }

    [System.Serializable]
    public struct AbilityContainer
    {
        public int containerId;
        public string containerTag;
        public List<KB_AbilityCore> unitAbilities;
        [HideInInspector]
        public bool hasUsed;
        public bool isConsumable;
        public float abilityDuration;
        public float abilityCooldown;

        public AbilityContainer(int containerId, string containerTag, List<KB_AbilityCore> unitAbilities, bool isConsumable = false, float abilityDuration = 5, float abilityCooldown = 25)
        {
            this.containerId = containerId;
            this.containerTag = containerTag;
            this.unitAbilities = unitAbilities;
            this.isConsumable = isConsumable;
            this.abilityDuration = abilityDuration;
            this.abilityCooldown = abilityCooldown;
            hasUsed = false;
        }
    }

}