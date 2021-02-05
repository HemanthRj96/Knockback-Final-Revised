namespace Knockback.Utility
{
    
    // Used in ItemSlot class
    public enum ItemSlotType
    {
        Pickup,
        Inventory
    }

    // Used in splash damage class
    public enum SplashDamageType
    {
        DefaultNull,
        singleExplosion,
        degradingExplosion,
        mobilizingExplosion,
        lingeringExplosion
    }

    //todo: Obselete enum - LevelNames
    public enum LevelNames
    {
        DefaultNull
    }

    // Used in PlayerStats class
    public enum ArmourTypes
    {
        DefaultNull,
        type_1,
        type_2,
        type_3
    }

    //todo: Obselete enum - AbilityType
    public enum AbilityType
    {
        consumable,
        nonConsumable
    }

    //todo: Obselete enum - UICanvasButtons
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

    //todo: Obselete enum - UICanvasGroups
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

    // Used in InputSettings class
    public enum InputType
    {
        defaultNull,
        MouseAndKeyboard,
        Touch
    }

    // Used in PlayerBackendSettings class
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

    // Enum for maintaining a reference for all layers used in the game
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
