using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillSetup : MonoBehaviour {
    [Header("Test Files")]
    [Header("TODO : This test files must be remove when the \nAPI and Skill Configurtation has been created")]
    [SerializeField] private TextAsset testSkillAPIJSON;
    [SerializeField] private TextAsset testSkillButtonConfiguration;

    [Space(2)]

    [Header("Game Object and Others")]
    [SerializeField] private GameObject skillButtonParent;
    [SerializeField] private GameObject generalSettings;

    private PlayerStatsManager playerStatsManager;
    private SkillCommand skillCommand;
    private SkillReference skillReference;
    private string skillConfigFileName;

    private List<SkillConfigurationInfo> skillConfigurationInfo = new List<SkillConfigurationInfo>();

    private void Awake() {
        playerStatsManager = generalSettings.GetComponent<PlayerStatsManager>();
        skillReference = GetComponent<SkillReference>();
        skillConfigFileName = $"{Global.QUICK_SLOT_FILE}{playerStatsManager.GetSetCharacterType}_{playerStatsManager.GetSetCharacterType}_{playerStatsManager.GetSetPlayerID}";
    }

    private void Start() {
        SetSkillBaseReference();
        SetupTestSkillConfiguration();
    }

    private void SetSkillBaseReference() {
        //TODO : This section gets the skill list from an API that will be basis of the skill settings used by the player
        skillReference.GetSetPlayerSkillList = FileHandler.ReadFromJSONString<BaseResponse<BaseResponseData<SkillPattern>>>(testSkillAPIJSON.text);

        if (skillReference.GetSetPlayerSkillList.success) {
            BaseResponseData<SkillPattern> skillPatternData = skillReference.GetSetPlayerSkillList.data;

            if (skillPatternData.rows_returned != 0) {
                foreach (SkillPattern current in skillPatternData.records) {
                    if (current.character == playerStatsManager.GetSetCharacterType.ToString()) {
                        skillReference.GetSetSkillTypeList = current.skillDetails;
                    }
                }
            } else {
                Debug.LogError("Empty Skill Set");
            }
        } else {
            Debug.LogError("No Skill Pattern Retreived");
        }
    }

    private void SetupTestSkillConfiguration() {
        //TODO: Remove this and replace with SetupSkillConfiguration once it is configured you can use the code here beacuse it will be the same. We will use a test skill config
        skillConfigurationInfo = FileHandler.ReadListFromJSONTextAsset<SkillConfigurationInfo>(testSkillButtonConfiguration.text);

        foreach (SkillConfigurationInfo _currentConfig in skillConfigurationInfo) {
            skillCommand = skillButtonParent.transform.
                Find(Global.BUTTON_SKILL_NAME + _currentConfig.buttonID).
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
        }
    }

    private void SetupSkillConfiguration() {
        //TODO: This will be the area where the program will check if skillConfigFileName then it will assign it in each Skill Button
        //TODO: use ReadListFromJSON because the file will be find internally
    }
}
