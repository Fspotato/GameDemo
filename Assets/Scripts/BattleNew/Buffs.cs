using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BattleNew
{
    [Serializable]
    public class BuffManager
    {
        public List<Buff> Buffs => buffs;

        [SerializeField] List<Buff> buffs = new List<Buff>();

        // 開場清除Buff
        public void Reset()
        {
            buffs.Clear();
        }

        // 獲得Buff(id參照BuffConfig表, value, round 必填, stack, maxStack不填則自動視為1層及不變)
        public void GetBuff(uint id, float value, int round, int stack, int maxStack, Vector3 position)
        {
            Buff buff = buffs.FirstOrDefault(b => b.Id == id);
            if (buff == default)
            {
                buff = new Buff(id, value, round, stack, maxStack);
                buffs.Add(buff);
            }
            else buff.GetBuff(value, round, stack, maxStack);
            BattleUI.Instance.FadeOut(buff.Name, Color.white, position, true, false);
        }

        // 移除指定種類Buff 可以一次指定多種
        public void RemoveBuffs(params BuffType[] type)
        {
            List<Buff> temp = new List<Buff>();
            foreach (var buff in buffs)
                foreach (var t in buff.Type)
                {
                    if (!type.Contains(t)) continue;
                    temp.Add(buff);
                    break;
                }
            foreach (var buff in temp) buffs.Remove(buff);
        }

        // 回合結束
        public void RoundOver()
        {
            if (buffs.Count == 0) return;
            for (int i = buffs.Count - 1; i >= 0; i--)
            {
                if (buffs[i].RoundOver()) buffs.Remove(buffs[i]);
            }
        }
    }

    [Serializable]
    public class Buff
    {
        public uint Id => id;
        public string Name => name;
        public List<BuffType> Type => type;
        public int Round => round;
        public float Value => value;
        public int Stack => stack;
        public int MaxStack => maxStack;

        [SerializeField] uint id;
        [SerializeField] string name;
        [SerializeField] List<BuffType> type;
        [SerializeField] float value;
        [SerializeField] int round;
        [SerializeField] int stack;
        [SerializeField] int maxStack;

        public void GetBuff(float value, int round, int stack, int maxStack)
        {
            this.value = Mathf.Max(this.value, value);
            this.round = Math.Max(this.round, round);
            this.stack += stack == -1 ? 1 : stack;
            this.maxStack = Math.Max(this.maxStack, maxStack);
            this.stack = Math.Min(this.stack, maxStack);
        }

        public bool RoundOver()
        {
            round--;
            return round <= 0 ? true : false;
        }

        public Buff(uint id, float value, int round, int stack, int maxStack)
        {
            Buff buff = DataManager.Instance.GetBuffById(id);
            this.id = id;
            this.name = buff.name;
            this.type = buff.type;
            this.value = value;
            this.round = round;
            this.stack = stack == -1 ? 1 : stack;
            this.maxStack = maxStack == -1 ? buff.maxStack : maxStack;
        }

        // 建表用的 不要亂調用
        public Buff(uint id, List<BuffType> type, string name, int maxStack)
        {
            this.id = id;
            this.type = type;
            this.name = name;
            this.maxStack = maxStack;
        }

        public Buff DeepCopy()
        {
            return JsonUtility.FromJson<Buff>(JsonUtility.ToJson(this));
        }
    }

    [Serializable]
    public enum BuffType
    {
        Ignite = 0,
        Forzen = 1,
        Guard = 2,
        DeBuff = 3,
        Buff = 4,
        Bleeding = 5,
    }
}