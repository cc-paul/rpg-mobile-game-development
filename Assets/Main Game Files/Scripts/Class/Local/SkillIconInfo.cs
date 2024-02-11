using UnityEngine;

[System.Serializable]
public class SkillIconInfo<TEnum> where TEnum : System.Enum {
    public TEnum IconName;
    public Sprite IconSprite;

    public SkillIconInfo(TEnum iconName, Sprite iconSprite) {
        IconName = iconName;
        IconSprite = iconSprite;
    }
}

