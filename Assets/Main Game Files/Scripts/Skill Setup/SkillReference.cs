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
    private bool cancelSkill;
    

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

    public bool GetSetCancelSkill {
        get { return cancelSkill; }
        set { cancelSkill = value; }
    }
    #endregion

    private void Awake() {
        playerStatsManager = generalSettings.GetComponent<PlayerStatsManager>();
    }

    public T GetSkillProperty<T>(int skillID, Func<SkillDetail, T> propertySelector) {
        SkillType skillType;
        SkillDetail skillDetail;

        for (int skillType_i = 0; skillType_i < skillTypeList.Count; skillType_i++) {
            skillType = skillTypeList[skillType_i];

            for (int skillDetail_i = 0; skillDetail_i < skillType.details.Count; skillDetail_i++) {
                skillDetail = skillType.details[skillDetail_i];

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

    public float GetSkillDamage(int skillID) {
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

    public float GetDeductedSpeed(int skillID) {
        return GetSkillProperty(skillID, skillDetail => skillDetail.deductedSpeed);
    }

    public bool GetSkillForAlly(int skillID) {
        return GetSkillProperty(skillID, skillDetail => skillDetail.forAlly);
    }

    public float GetSetDeactivationTime(int skillID) {
        return GetSkillProperty(skillID, skillDetail => skillDetail.deactivationTime);
    }

    public float GetAddedDamage(int skillID) {
        return GetSkillProperty(skillID, skillDetail => skillDetail.addedDamage);
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
        Global.SwordsmanSkillIcon currentIconName;
        Sprite iconSprite;

        if (characterType == Global.Characters.Swordsman.ToString()) {
            for (int icon_i = 0; icon_i < swordsmanSkillIcon.Count; icon_i++) {
                currentIconName = swordsmanSkillIcon[icon_i].IconName;
                iconSprite = swordsmanSkillIcon[icon_i].IconSprite;

                if (currentIconName.ToString() == iconName) {
                    return iconSprite;
                }
            }
        }

        return null;
    }
}