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

    /* Swordsman Skills Animation */
    public enum SwordsmanSkillAnimation {
        Ground_Impact,
        Lightning_Ball,
        Ice_Crack,
        Lightning_Strike,
        Health_Up,
        Speed_Up,
        Ice_Down_Wave,
        Whirlwind,
        Lightning_Orb,
        Berserk_Aura,
        Swords_Blessing
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

    /* Swordsman Skill Icon */
    public enum SwordsmanSkillIcon {
        icon_ice_crack,
        icon_ground_impact,
        icon_lightning_ball,
        icon_lightning_strike,
        icon_health_up,
        icon_speed_up,
        icon_ice_down_wave,
        icon_whirlwind,
        icon_berserk,
        icon_swords_blessing
    }

    /* Target Indicator */
    public enum TargetIndicator {
        Area,
        Line
    }

    public enum EnemyAnimation {
        Enemy_Idle,
        Enemy_Attack,
        Enemy_Run,
        Enemy_Walk,
        Enemy_Death,
        Attack1_Animation,
        Attack2_Animation,
        Attack3_Animation,
        Enemy_Chase
    }

    /* Regen Category */
    public enum RegenCategory {
        HPRegen,
        MPRegen
    }

    /* Tags */
    public const string PLAYER = "Player";
    public const string PLAYER_OTHERS = "Player Others";
    public const string MOB = "Mob";

    /* Offset */
    public const float RANGE_OFFSET = 3.6f;
    public const float RANGE_OFFSET_TARGET = 3f;

    /* Game Object */
    public const string GENERAL_SETTINGS = "General Settings";
    public const string SKILL_SETTINGS = "Skill Settings";
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
    public const string BASIC_INFO_UI_V2 = "Basic Info UI V2";
    public const string MOBS_INFO_UI = "Mobs Info UI";
    public const string TARGET_INDICATOR = "Target Indicator";
    public const string TARGET_NOTIFY = "Target Notify";
    public const string CONTROLLER = "Controller";
    public const string DUMMY = "Dummy Only";
    public const string NUMBER_CONTAINER = "Number Container";
    public const string TARGET_ATTACK = "Target Attack";
    public const string TARGET_HELP = "Target Help";
    public const string BUFF_EFFECT_POOL = "Buff Effect Pool";

    /* File Names and Naming Declaration */
    public const string QUICK_SLOT_FILE = "QuickSlot_";
    public const string PREFS_SKILLNO_ = "Prefs_Skill_No_";
    public const string DURATION_ITEM = "Duration_Item";

    /* Message */
    public const string MESSAGE_NO_MANA = "Unable to cast skill, no mana";
    public const string MESSAGE_COOLDOWN = "Unable to cast skill, please wait for the cooldown";
    public const string MESSAGE_NO_TARGET = "Please select a target";
    public const string MESSAGE_NO_WEAPON = "Please equip a weapon to cast a skill";
}