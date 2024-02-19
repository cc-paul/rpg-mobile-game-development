using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillReference : MonoBehaviour {
    [Header("Game Object and Others")]
    [SerializeField] private GameObject generalSettings;

    [Space(2)]

    [Header("Swordsman Skill Icons")]
    [SerializeField] private List<SkillIconInfo<Global.SwordsmanSkillIcon>> swordsmanSkillIcon = new List<SkillIconInfo<Global.SwordsmanSkillIcon>>();

    private PlayerStatsManager playerStatsManager;
    private BaseResponse<BaseResponseData<SkillPattern>> playerSkillList = new BaseResponse<BaseResponseData<SkillPattern>>();
    private List<SkillType> skillTypeList = new List<SkillType>();
    private int finalSkillID;
    

    #region GetSet Properties
    public BaseResponse<BaseResponseData<SkillPattern>> GetSetPlayerSkillList {
        get { return playerSkillList; }
        set { playerSkillList = value; }
    }

    public List<SkillType> GetSetSkillTypeList {
        get { return skillTypeList; }
        set { skillTypeList = value; }
    }

    public int GetSetFinalSkillID {
        get { return finalSkillID; }
        set { finalSkillID = value; }
    }
    #endregion

    private void Awake() {
        playerStatsManager = generalSettings.GetComponent<PlayerStatsManager>();
    }

    public T GetSkillProperty<T>(int skillID, Func<SkillDetail, T> propertySelector) {
        foreach (SkillType skillType in skillTypeList) {
            foreach (SkillDetail skillDetail in skillType.details) {
                if (skillDetail.id == skillID) {
                    return propertySelector(skillDetail);
                }
            }
        }

        return default(T);
    }

    public List<SkillType> GetSkillTypeList() {
        return skillTypeList;
    }

    public string GetTargetIndicator(int skillID) {
        finalSkillID = skillID;
        return GetSkillProperty(skillID, skillDetail => skillDetail.indicatorType);
    }

    public float GetSkillCoolDown(int skillID,string currentDateTime,string skillDateTime) {
        DateTime dtCurrentDateTime = DateTime.ParseExact(currentDateTime, "yyyy-MM-dd HH:mm:ss", null);
        DateTime dtSkillDateTime = DateTime.ParseExact(skillDateTime, "yyyy-MM-dd HH:mm:ss", null);

        TimeSpan duration = dtCurrentDateTime - dtSkillDateTime;
        float durationInSeconds = GetSkillDefaultCoolDown(skillID: skillID) - (float)duration.TotalSeconds;

        if (durationInSeconds < 0f) {
            return 0f;
        }

        return durationInSeconds;
    }

    public float GetSkillDefaultCoolDown(int skillID) {
        return GetSkillProperty(skillID, skillDetail => skillDetail.defaultCooldown);
    }

    public int GetSkillDamage(int skillID) {
        return GetSkillProperty(skillID, skillDetail => skillDetail.baseDamage);
    }

    public int GetSkillMaxTarget(int skillID) {
        return GetSkillProperty(skillID, skillDetail => skillDetail.maxTarget);
    }

    public float GetSkillTargetRanger(int skillID) {
        return GetSkillProperty(skillID, skillDetail => skillDetail.areaRange);
    }

    public float GetSkillAddedHP(int skillID) {
        return GetSkillProperty(skillID, skillDetail => skillDetail.addedHP);
    }

    public float GetSkillAddedSpeed(int skillID) {
        return GetSkillProperty(skillID, skillDetail => skillDetail.addedSpeed);
    }

    public bool GetSkillForAlly(int skillID) {
        return GetSkillProperty(skillID, skillDetail => skillDetail.forAlly);
    }

    public float GetSetDeactivationTime(int skillID) {
        return GetSkillProperty(skillID, skillDetail => skillDetail.deactivationTime);
    }

    public float GetSetMPConsumption(int skillID) {
        return GetSkillProperty(skillID, skillDetail => skillDetail.mpConsumption);
    }

    public float GetSetDistanceToCast(int skillID) {
        return GetSkillProperty(skillID, skillDetail => skillDetail.distanceToCast);
    }

    public string GetIconName(int skillID) {
        return GetSkillProperty(skillID, skillDetail => skillDetail.icon);
    }

    public bool GetRequiresWeapon(int skillID) {
        return GetSkillProperty(skillID, skillDetail => skillDetail.requiresWeapon);
    }

    public Sprite GetSkillSprite(string iconName) {
        string characterType = playerStatsManager.GetSetCharacterType.ToString();

        if (characterType == Global.Characters.Swordsman.ToString()) {
            foreach (SkillIconInfo<Global.SwordsmanSkillIcon> skillIconInfo in swordsmanSkillIcon) {
                Global.SwordsmanSkillIcon currentIconName = skillIconInfo.IconName;
                Sprite iconSprite = skillIconInfo.IconSprite;

                if (currentIconName.ToString() == iconName) {
                    return iconSprite;
                }
            }
        }

        return null;
    }
}