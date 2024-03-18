using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillSetup : MonoBehaviour {
    [Header("Test Files")]
    [Header("TODO : This test files must be remove when the \nAPI and Skill Configurtation has been created")]
    [SerializeField] private TextAsset testSkillAPIJSON;

    [Space(2)]

    [Header("Game Object and Others")]
    [SerializeField] private GameObject skillButtonParent;
    [SerializeField] private GameObject generalSettings;
    [SerializeField] private GameObject tempSkillIconParent;
    [SerializeField] private GameObject skillQuickSlotParent;
    [SerializeField] private GameObject skillDurationWindow;
    [SerializeField] private GameObject skillCooldownIndicatorPrefab;

    [Space(2)]

    [Header("UI")]
    [SerializeField] private GameObject skillWindowParent;
    [SerializeField] private GameObject skillContainer;
    [SerializeField] private GameObject skillSlotPrerab;
    [SerializeField] private Button btnClose;
    [SerializeField] private Button btnRemoveAll;

    private PlayerStatsManager playerStatsManager;
    private SkillCommand skillCommand;
    private SkillReference skillReference;
    private string skillConfigFileName;
    private string buttonSkillName = "";

    private List<SkillConfigurationInfo> skillConfigurationInfo = new List<SkillConfigurationInfo>();

    #region GetSet Properties 
    public string GetSetButtonSkillName {
        get { return buttonSkillName; }
        set { buttonSkillName = value; }
    }
    #endregion

    private void Awake() {
        playerStatsManager = generalSettings.GetComponent<PlayerStatsManager>();
        skillReference = GetComponent<SkillReference>();
        skillConfigFileName = $"{Global.QUICK_SLOT_FILE}{playerStatsManager.GetSetCharacterType}_{playerStatsManager.GetSetCharacterType}_{playerStatsManager.GetSetPlayerID}";

        btnClose.onClick.AddListener(() => {
            if (tempSkillIconParent.transform.childCount != 0) return;
            skillWindowParent.gameObject.SetActive(false);
            RecalibrateQuickSlot();
        });

        btnRemoveAll.onClick.AddListener(() => {
            ClearQuickSlot();
        });
    }

    private void Start() {
        SetSkillBaseReference();
        PopulateSkillList();
        SetupSkillConfiguration();
    }

    private void SetSkillBaseReference() {
        List<SkillType> skillTypeListForIconBuff = new List<SkillType>();
        List<SkillDetail> skillDetailForIconBuff = new List<SkillDetail>();
        GameObject skillCooldownIndicator;
        SkillPattern skillPattern;
        SkillType currentSkillSet;
        SkillDetail currentSkillDetail;
        string skillObjectName;

        //TODO : This section gets the skill list from an API that will be basis of the skill settings used by the player
        skillReference.GetSetPlayerSkillList = FileHandler.ReadFromJSONString<BaseResponse<BaseResponseData<SkillPattern>>>(testSkillAPIJSON.text);

        if (skillReference.GetSetPlayerSkillList.success) {
            BaseResponseData<SkillPattern> skillPatternData = skillReference.GetSetPlayerSkillList.data;

            if (skillPatternData.rows_returned != 0) {
                for (int skillPattern_i = 0; skillPattern_i < skillPatternData.records.Count; skillPattern_i++) {
                    skillPattern = skillPatternData.records[skillPattern_i];
                    skillTypeListForIconBuff = skillPattern.skillDetails;

                    if (skillPattern.character == playerStatsManager.GetSetCharacterType.ToString()) {
                        skillReference.GetSetSkillTypeList = skillPattern.skillDetails;
                    }

                    for (int currentSkillSet_i = 0; currentSkillSet_i < skillTypeListForIconBuff.Count; currentSkillSet_i++) {
                        skillDetailForIconBuff = skillTypeListForIconBuff[currentSkillSet_i].details;

                        for (int currentSkillDetail_i = 0; currentSkillDetail_i < skillDetailForIconBuff.Count; currentSkillDetail_i++) {
                            currentSkillDetail = skillDetailForIconBuff[currentSkillDetail_i];

                            if (currentSkillDetail.isBuff) {
                                skillObjectName = $"{skillPattern.character}_{currentSkillDetail.id}_{Global.DURATION_ITEM}";
                                skillCooldownIndicator = Instantiate(skillCooldownIndicatorPrefab, skillDurationWindow.transform);
                                skillCooldownIndicator.name = skillObjectName;
                            }
                        }
                    }
                }
            } else {
                Debug.LogError("Empty Skill Set");
            }
        } else {
            Debug.LogError("No Skill Pattern Retreived");
        }
    }

    private void PopulateSkillList() {
        List<SkillType> currentSkillTypeList = skillReference.GetSkillTypeList();
        List<SkillDetail> skillDetail;
        GameObject skillRowDetails;
        Sprite skillSprite;
        GameObject content;
        bool includeInSkillList;
        int skillID;
        int levelrowIndex = 0;
        string skillName;
        

        for (int currentSkillTypeList_i = 0; currentSkillTypeList_i < currentSkillTypeList.Count; currentSkillTypeList_i++) {
            skillDetail = currentSkillTypeList[currentSkillTypeList_i].details;

            for (int skillDetail_i = 0; skillDetail_i < skillDetail.Count; skillDetail_i++) {
                includeInSkillList = skillDetail[skillDetail_i].includeInSkillList;
                skillID = skillDetail[skillDetail_i].id;
                skillName = skillDetail[skillDetail_i].displayName;
                skillSprite = skillReference.GetSkillSprite(
                    iconName: skillReference.GetIconName(
                        skillID: skillID
                    )
                );

                if (includeInSkillList) {
                    levelrowIndex++;

                    skillRowDetails = Instantiate(
                        skillSlotPrerab,
                        Vector3.zero,
                        Quaternion.identity,
                        skillContainer.transform
                    );

                    skillRowDetails.transform.Find(Global.ICON_FIX).GetComponent<Image>().sprite = skillSprite;
                    skillRowDetails.transform.Find($"{Global.DRAGGABLE_ICON_HOLDER}/{Global.ICON_DRAG}").GetComponent<Image>().sprite = skillSprite;
                    skillRowDetails.transform.Find($"{Global.DRAGGABLE_ICON_HOLDER}/{Global.ICON_DRAG}").GetComponent<SkillDragAssignment>().GetSetSkillID = skillID;
                    skillRowDetails.transform.Find($"{Global.DRAGGABLE_ICON_HOLDER}/{Global.ICON_DRAG}").GetComponent<SkillDragAssignment>().GetSetTempSkillIconParent = tempSkillIconParent;

                    content = skillRowDetails.transform.Find(Global.CONTENT).gameObject;
                    content.transform.Find(Global.SKILL_NAME_VALUE).GetComponent<TextMeshProUGUI>().text = skillName;
                    content.transform.Find(Global.LEVEL_NAME_VALUE).GetComponent<TextMeshProUGUI>().text = levelrowIndex.ToString();
                }
            }
        }
    }

    private void SetupSkillConfiguration() {
        skillConfigurationInfo = FileHandler.ReadListFromJSON<SkillConfigurationInfo>(skillConfigFileName);
        SkillConfigurationInfo _currentConfig;
        GameObject quickSlot;
        Image quickSlotIcon;
        SkillDropAssignment skillDropAssignment;

        for (int skillConfig_i = 0; skillConfig_i < skillConfigurationInfo.Count; skillConfig_i++) {
            _currentConfig = skillConfigurationInfo[skillConfig_i];

            skillCommand = skillButtonParent.transform.
               Find($"{Global.BUTTON_SKILL_NAME}{_currentConfig.buttonID}").
               GetComponent<SkillCommand>();

            skillCommand.GetSetSkillID = _currentConfig.skillID;
            skillCommand.GetSetPrefCooldownRef = $"{Global.PREFS_SKILLNO_}{_currentConfig.skillID}";
            skillCommand.SetSkillSprite(
                skillSprite: skillReference.GetSkillSprite(
                   iconName: skillReference.GetIconName(
                      skillID: _currentConfig.skillID
                   )
                )
            );
            skillCommand.RecalibrateCooldown();


            /* For the quickslot in Skill Window */
            quickSlot = skillQuickSlotParent.transform.Find($"{Global.QUICK_SLOT}{_currentConfig.buttonID}").gameObject;
            quickSlotIcon = quickSlot.transform.Find(Global.ICON).gameObject.GetComponent<Image>();
            skillDropAssignment = quickSlot.GetComponent<SkillDropAssignment>();

            skillDropAssignment.GetSetSkillID = _currentConfig.skillID;

            if (_currentConfig.skillID == -1) {
                skillDropAssignment.ResetQuickSlot();
            } else {
                quickSlotIcon.sprite = skillReference.GetSkillSprite(
                    iconName: skillReference.GetIconName(
                        skillID: _currentConfig.skillID
                    )
                );
            }
        }
    }

    private void RecalibrateQuickSlot() {
        int rowQuickSlot = 1;
        skillConfigurationInfo.Clear();
        GameObject quickSlot;
        SkillDropAssignment currentDroppableItem;

        for (int child_i = 0; child_i < skillQuickSlotParent.transform.childCount; child_i++) {
            quickSlot = skillQuickSlotParent.transform.GetChild(child_i).gameObject;
            currentDroppableItem = quickSlot.GetComponent<SkillDropAssignment>();

            skillConfigurationInfo.Add(new SkillConfigurationInfo(
                buttonID: rowQuickSlot,
                skillID: currentDroppableItem.GetSetSkillID
            ));
            rowQuickSlot++;
        }

        File.Delete(path: FileHandler.GetPath(filename: skillConfigFileName));
        FileHandler.SaveToJSON(toSave: skillConfigurationInfo,fileName: skillConfigFileName);
        SetupSkillConfiguration();
    }

    public void RemoveExistingSkillID(int droppedSkillID) {
        GameObject quickSlot;
        SkillDropAssignment currentDroppableItem;

        for (int child_i = 0; child_i < skillQuickSlotParent.transform.childCount; child_i++) {
            quickSlot = skillQuickSlotParent.transform.GetChild(child_i).gameObject;
            currentDroppableItem = quickSlot.GetComponent<SkillDropAssignment>();

            if (currentDroppableItem.GetSetSkillID == droppedSkillID) {
                currentDroppableItem.ResetQuickSlot();
            }
        }
    }

    private void ClearQuickSlot() {
        GameObject quickSlot;
        SkillDropAssignment currentDroppableItem;

        for (int child_i = 0; child_i < skillQuickSlotParent.transform.childCount; child_i++) {
            quickSlot = skillQuickSlotParent.transform.GetChild(child_i).gameObject;
            currentDroppableItem = quickSlot.GetComponent<SkillDropAssignment>();
            currentDroppableItem.ResetQuickSlot();
        }
    }

    public void SetPrimarySkillButton(string _buttonName) {
        if (buttonSkillName == "" || buttonSkillName == _buttonName) {
            buttonSkillName = _buttonName;
        }
    }
}
