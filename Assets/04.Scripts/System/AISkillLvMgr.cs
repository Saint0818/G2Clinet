using System.Collections.Generic;
using JetBrains.Annotations;
using Newtonsoft.Json;
using UnityEngine;

public class AISkillLvMgr
{
    private static readonly AISkillLvMgr INSTANCE = new AISkillLvMgr();
    public static AISkillLvMgr Ins
    {
        get { return INSTANCE; }
    }

    private readonly Dictionary<int, AISkillData> mData = new Dictionary<int, AISkillData>();

    public void Load(string jsonText)
    {
        mData.Clear();

        AISkillData[] skillData = (AISkillData[])JsonConvert.DeserializeObject(jsonText, typeof(AISkillData[]));
        foreach(AISkillData data in skillData)
        {
            mData.Add(data.ID, data);
        }

        Debug.Log("[AISkillLv parsed finished.]");
    }

    [CanBeNull]
    public AISkillData GetByID(int id)
    {
        if(mData.ContainsKey(id))
            return mData[id];

        return null;
    }
}