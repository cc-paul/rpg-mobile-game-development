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
    private string skillConfigFileName;

    private List<SkillConfigurationInfo> skillConfigurationInfo = new List<SkillConfigurationInfo>();

    private void Awake() {
        playerStatsManager = generalSettings.GetComponent<PlayerStatsManager>();
        skillConfigFileName = $"{Global.QUICK_SLOT_FILE}{playerStatsManager.GetSetCharacterType}_{playerStatsManager.GetSetCharacterType}_{playerStatsManager.GetSetPlayerID}";

        SetupTestSkillConfiguration();
        //SetupSkillConfiguration();
    }

    private void SetupTestSkillConfiguration() {
        //TODO: Remove this and replace with SetupSkillConfiguration once it is configured you can use the code here beacuse it will be the same. We will use a test skill config
        skillConfigurationInfo = FileHandler.ReadListFromJSONTextAsset<SkillConfigurationInfo>(testSkillButtonConfiguration.text);

        foreach (SkillConfigurationInfo _currentConfig in skillConfigurationInfo) {
            skillCommand = skillButtonParent.transform.
                Find(Global.BUTTON_SKILL_NAME + _currentConfig.buttonID)
                .GetComponent<SkillCommand>();

            skillCommand.GetSetSkillID = _currentConfig.skillID;
        }
    }

    private void SetupSkillConfiguration() {
        //TODO: This will be the area where the program will check if skillConfigFileName then it will assign it in each Skill Button
        //TODO: use ReadListFromJSON because the file will be find internally
    }
}
