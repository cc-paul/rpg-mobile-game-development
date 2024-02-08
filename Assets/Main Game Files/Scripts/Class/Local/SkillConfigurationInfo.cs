using System;

[Serializable]
public class SkillConfigurationInfo {
    public int buttonID;
    public int skillID;

    public SkillConfigurationInfo(int buttonID, int skillID) {
        this.buttonID = buttonID;
        this.skillID = skillID;
    }
}
