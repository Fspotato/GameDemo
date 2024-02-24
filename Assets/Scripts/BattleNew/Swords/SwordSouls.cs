using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace BattleNew
{
    [System.Serializable]
    public class SwordSoul
    {
        [SerializeField] protected int round;
        [SerializeField] protected int maxRound;
        [SerializeField] protected SwordType type;

        public int Round => round;
        public int MaxRound => maxRound;
        public SwordType Type => type;

        public virtual void StartTurn(UnityAction<bool> callback)
        {
            round--;
            if (round <= 0)
            {
                callback(true);
                round = maxRound;
            }
        }

        public SwordSoul(SwordType type, int maxRound)
        {
            this.type = type;
            this.maxRound = maxRound;
            round = maxRound;
        }
    }

}