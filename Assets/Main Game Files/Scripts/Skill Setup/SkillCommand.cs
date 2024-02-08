using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillCommand : MonoBehaviour {
    private int skillID = 0;

    #region
    public int GetSetSkillID {
        get { return skillID; }
        set { skillID = value; }
    }
    #endregion

    private void Start() {
        Debug.Log(skillID);
    }
}