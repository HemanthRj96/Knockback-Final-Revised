using Knockback.Core;

namespace Knockback.Derived
{
    //todo: Finish implementatiion - KB_MainLobbyManager

    /// <summary>
    /// Derived class for main lobby
    /// </summary>
    public class KB_MainLobbyManager : KB_LevelManagerCore
    {
        protected override void CallOnAwake()
        {
        }

        protected override void CallOnStart()
        {
        }

        protected override void CallOnUpdate()
        {
        }

        public override KB_LevelManagerCore InstantiateManager()
        {
            return base.InstantiateManager();
        }

        public override void LoadManager(float delayInSeconds = -1, bool shouldLoadLevel = true)
        {
            base.LoadManager(delayInSeconds, shouldLoadLevel);
        }

        public override void UnloadManager(float delayInSeconds = -1, bool shouldUnload = true, bool shouldDestroy = true)
        {
            base.UnloadManager(delayInSeconds, shouldUnload, shouldDestroy);
        }
    }
}