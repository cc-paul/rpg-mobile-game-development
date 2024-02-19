using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Global {
    /* Axis Options */
    public enum AxisOptions {
        Both,
        Horizontal,
        Vertical
    }

    /* Stat Mod Type */
    public enum StatModType {
        Flat = 100,
        PercentAdd = 200,
        PercentMult = 300
    }

    /* Characters */
    public enum Characters {
        Brawler,
        Swordsman,
        Archer,
        Shaman,
        Default
    }

    /* Gender */
    public enum Gender {
        Male,
        Female,
        Default
    }

    /* Command */
    public enum Command {
        Attack,
        Stop_Attack,
        Heal,
        Stop_Heal
    }

    /* Swordsman Normal Animation */
    public enum SwordsmanNormalAnimation {
        Sword_Base_Idle,
        Idle_Sword,
        Idle_No_Sword,
        Run_No_Sword,
        Run_Sword,
        Walk_Sword,
        Walk_No_Sword,
        Death_Sword
    }

    public enum GameTags {
        Player
    }

    /* Animation Category */
    public enum AnimationCategory {
        Idle,
        Walk,
        Run
    }


    /* Tags */
    public const string PLAYER = "Player";
    public const string PLAYER_OTHERS = "Player Others";
    public const string MOB = "Mob";

    /* Swordsman Skill Icon */
    public enum SwordsmanSkillIcon {
        icon_ice_crack,
        icon_ground_impact,
        icon_lightning_ball,
        icon_lightning_strike,
        icon_health_up,
        icon_speed_up,
        icon_ice_down_wave
    }

    /* Target Indicator */
    public enum TargetIndicator {
        Area,
        Line
    }

    /* Offset */
    public const float RANGE_OFFSET = 3.6f;
    public const float RANGE_OFFSET_TARGET = 3f;

    /* Game Object */
    public const string GENERAL_SETTINGS = "General Settings";
    public const string BUTTON_SKILL_NAME = "Button Skill Slot #";
    public const string BACKGROUND = "Background";
    public const string HANDLE = "Handle";
    public const string TIMER_IMAGE = "Timer Image";
    public const string COUNTER = "Counter";
    public const string ICON_DRAG = "Icon Drag";
    public const string DRAGGABLE_ICON_HOLDER = "Draggable Icon Holder";
    public const string ICON_FIX = "Icon Fix";
    public const string ICON = "Icon";
    public const string CONTENT = "Content";
    public const string SKILL_NAME_VALUE = "Skill Value";
    public const string LEVEL_NAME_VALUE = "Level Value";
    public const string QUICK_SLOT = "Quick Slot ";
    public const string BORDER = "Border";
    public const string BASIC_INFO_UI = "Basic Info UI";
    public const string TARGET_INDICATOR = "Target Indicator";

    /* File Names and Naming Declaration */
    public const string QUICK_SLOT_FILE = "QuickSlot_";
    public const string PREFS_SKILLNO_ = "Prefs_Skill_No_";
}