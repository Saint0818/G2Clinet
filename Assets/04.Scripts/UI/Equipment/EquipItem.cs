using System;
using System.Collections.Generic;
using GameStruct;
using JetBrains.Annotations;

namespace UI
{
    public class EquipItem
    {
        [NotNull]
        public string Name;
        [NotNull]
        public string Icon;
        [NotNull]
        public string Desc;

        // 道具會影響哪些屬性的數值.
        public Dictionary<EAttributeKind, int> Values = new Dictionary<EAttributeKind, int>();

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
                sum += Values[kind];

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