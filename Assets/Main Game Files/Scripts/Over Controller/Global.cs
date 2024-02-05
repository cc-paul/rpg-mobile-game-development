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

    public enum AnimationCategory {
        Idle,
        Walk,
        Run
    }
}