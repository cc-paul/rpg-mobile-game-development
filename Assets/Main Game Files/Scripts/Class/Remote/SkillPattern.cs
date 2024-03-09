using System;
using System.Collections.Generic;

[Serializable]
public class SkillDetail {
    public int id;
    public string displayName;
    public int defaultCooldown;
    public float baseDamage;
    public string indicatorType;
    public int maxTarget;
    public float areaRange;
    public bool includeInSkillList;
    public float addedHP;
    public bool forAlly;
    public float deactivationTime;
    public float mpConsumption;
    public float addedSpeed;
    public float deductedSpeed;
    public float distanceToCast;
    public string icon;
    public bool requiresWeapon;
    public float addedDamage;
    public bool isBuff;
}

[Serializable]
public class SkillType {
    public string type;
    public List<SkillDetail> details;
}

[Serializable]
public class SkillPattern {
    public string character;
    public List<SkillType> skillDetails;
}