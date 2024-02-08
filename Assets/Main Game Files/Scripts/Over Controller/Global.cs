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

    /* Game Object */
    public const string GENERAL_SETTINGS = "General Settings";
    public const string BUTTON_SKILL_NAME = "Button Skill Slot #";
    public const string BACKGROUND = "Background";
    public const string HANDLE = "Handle";
    public const string TIMER_IMAGE = "Timer Image";
    public const string COUNTER = "Counter";

    /* File Names */
    public const string QUICK_SLOT_FILE = "QuickSlot_";
}