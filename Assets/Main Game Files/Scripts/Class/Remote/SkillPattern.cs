using System;
using System.Collections.Generic;

[Serializable]
public class SkillDetail {
    public int id;
    public string displayName;
    public int defaultCooldown;
    public int baseDamage;
    public string indicatorType;
    public int maxTarget;
    public float areaRange;
    public bool includeInSkillList;
    public float addedHP;
    public bool forAlly;
    public float deactivationTime;
    public float mpConsumption;
    public float addedSpeed;
    public float distanceToCast;
    public string icon;
    public bool requiresWeapon;
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