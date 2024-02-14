using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class BuffManager
{
    public List<Buff> Buffs => buffs;

    [SerializeField] List<Buff> buffs = new List<Buff>();

    public void GetBuff(BuffType type, int id, float value, int stack, int maxStack, int round)
    {
        Buff buff = buffs.FirstOrDefault(b => b.Id == id);
        if (buff == default(Buff))
        {
            buff = new Buff(type, id, value, stack, maxStack, round);
            buffs.Add(buff);
        }
        else
        {
            buff.GetBuff(value, stack, round);
        }
    }

    public void RemoveBuff(BuffType type)
    {
        Buff buff = buffs.FirstOrDefault(b => b.Type == type);
        if (buff == default(Buff)) return;
        buffs.Remove(buff);
    }

    public void RemoveBuff(BuffType type, int stack)
    {
        Buff buff = buffs.FirstOrDefault(b => b.Type == type);
        if (buff == default(Buff)) return;
        if (buff.RemoveBuff(stack)) buffs.Remove(buff);
    }

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
    public BuffType Type => type;
    public int Id => id;
    public int Round => round;
    public float Value => value;
    public int Stack => stack;
    public int MaxStack => maxStack;

    [SerializeField] BuffType type;
    [SerializeField] int id;
    [SerializeField] float value;
    [SerializeField] int round;
    [SerializeField] int stack;
    [SerializeField] int maxStack;

    public void GetBuff(float value, int stack, int round)
    {
        if (value >= this.value) this.value = value;
        if (round >= this.round) this.round = round;
        this.stack += stack;
        this.stack = Math.Min(this.stack, maxStack);
    }

    public bool RemoveBuff(int stack)
    {
        this.stack -= stack;
        return stack <= 0 ? true : false;
    }

    public bool RoundOver()
    {
        round--;
        return round <= 0 ? true : false;
    }

    public Buff(BuffType type, int id, float value, int stack, int maxStack, int round)
    {
        this.type = type;
        this.id = id;
        this.value = value;
        if (stack >= maxStack) stack = maxStack;
        this.stack = stack;
        this.maxStack = maxStack;
        this.round = round;
    }
}

[Serializable]
public enum BuffType
{
    Ignite = 0,
    Forzen = 1,
    Guard = 2,
}
