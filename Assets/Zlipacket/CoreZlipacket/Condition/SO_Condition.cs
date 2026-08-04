using UnityEngine;

namespace Zlipacket.CoreZlipacket.Condition
{
    public abstract class SO_Condition : ScriptableObject
    {
        public abstract bool IsMet();
    }
}