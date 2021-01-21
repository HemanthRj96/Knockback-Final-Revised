namespace Knockback.Utility
{

    public enum ItemSlotType
    {
        Pickup,
        Inventory
    }

    public enum SplashDamageType
    {
        DefaultNull,
        singleExplosion,
        degradingExplosion,
        mobilizingExplosion,
        lingeringExplosion
    }

    public enum LevelNames
    {
        DefaultNull
    }

    public enum ArmourTypes
    {
        DefaultNull,
        type_1,
        type_2,
        type_3
    }

    public enum AbilityType
    {
        consumable,
        nonConsumable
    }

    public enum UICanvasButtons
    {
        defaultNull,
        start,
        pause,
        exit,
        settings,
        sound,
        graphics,
        playerProfile,
        button_1,
        button_2,
        button_3,
        button_4,
        button_5
    }

    public enum UICanvasGroups
    {
        defaultNull,
        startMenu,
        pauseMenu,
        settingsMenu,
        exitMenu,
        miscMenu_1,
        miscMenu_2,
        miscMenu_3,
        miscMenu_4
    }

    public enum InputType
    {
        defaultNull,
        MouseAndKeyboard,
        Touch
    }

    public enum PlayerBackendSettingType
    {
        defaultNull,
        moveSpeed,
        jumpForce,
        airControl,
        groundCheckerLayerMask,
        dashingCooldown,
        dashingSpeed,
        dashingDistance
    }

    public enum Layers
    {
        defaulNull = 0,
        player = 8,
        ignorePlayer = 9,
        ground = 10,
        trigger = 11,
        transparent  = 12,
        destructables = 13,
    }
}
