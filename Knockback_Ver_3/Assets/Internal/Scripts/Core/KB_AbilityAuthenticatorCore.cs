using System.Collections.Generic;

namespace Knockback.Core
{
    public static class KB_AbilityAuthenticatorCore
    {
        private static Dictionary<string, KB_AbilityInjectorCore> registeredInjectorCollection = new Dictionary<string, KB_AbilityInjectorCore>();

        public static void RegisterInjector(KB_AbilityInjectorCore injector)
        {
            if(!IsRegistered(injector.GetUniqueId()))
                registeredInjectorCollection.Add(injector.GetUniqueId(), injector);
        }

        public static void DeregisterInjector(KB_AbilityInjectorCore injector)
        {
            if (!IsRegistered(injector.injectorId))
                registeredInjectorCollection.Remove(injector.injectorId);
        }

        public static bool IsInjectorRegistered(KB_AbilityInjectorCore injector) => IsRegistered(injector.injectorId);

        private static bool IsRegistered(string id) => registeredInjectorCollection.ContainsKey(id);
    }
}