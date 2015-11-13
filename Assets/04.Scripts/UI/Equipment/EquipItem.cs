using System;
using System.Collections.Generic;
using GameStruct;
using JetBrains.Annotations;

namespace UI
{
    /// <summary>
    /// 裝備介面所使用的裝備資料.
    /// </summary>
    public class EquipItem
    {
        public class AttrKindData
        {
            public string Icon;
            public int Value;
        }

        public string Name
        {
            set { mName = string.IsNullOrEmpty(value) ? string.Empty : value; }
            get { return mName; }
        }
        private string mName;

        public string Icon
        {
            set { mIcon = string.IsNullOrEmpty(value) ? string.Empty : value; }
            get { return mIcon; }
        }
        private string mIcon;

        public string Desc
        {
            set { mDesc = string.IsNullOrEmpty(value) ? string.Empty : value; }
            get { return mDesc; }
        }
        private string mDesc;

        // 道具會影響哪些屬性的數值.
        public Dictionary<EAttributeKind, AttrKindData> Values = new Dictionary<EAttributeKind, AttrKindData>();

        // 鑲嵌物品.
        [CanBeNull]
        public EquipInlay[] Inlays;

        public int Num; // 堆疊數量.

        public override string ToString()
        {
            return String.Format("Name: {0}, Icon: {1}, Desc: {2}", Name, Icon, Desc);
        }

        public int GetValue(EAttributeKind kind)
        {
            int sum = 0;
            if (Values.ContainsKey(kind))
                sum += Values[kind].Value;

            if (Inlays != null)
            {
                foreach (EquipInlay inlay in Inlays)
                {
                    sum += inlay.GetValue(kind);
                }
            }

            return sum;
        }

        public bool IsValid()
        {
            return !String.IsNullOrEmpty(Name);
        }
    }

    public class EquipInlay
    {
        public string Icon;

        // 道具會影響哪些屬性的數值.
        public Dictionary<EAttributeKind, int> Values = new Dictionary<EAttributeKind, int>();

        public int GetValue(EAttributeKind kind)
        {
            if (Values.ContainsKey(kind))
                return Values[kind];
            return 0;
        }
    }
}