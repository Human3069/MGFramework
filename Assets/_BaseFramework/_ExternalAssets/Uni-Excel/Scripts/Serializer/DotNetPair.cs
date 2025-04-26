using System;
using UnityEngine;

namespace _KMH_Framework
{
    /// <summary>
    /// To serialize two string and can be used as Type
    /// </summary>
    [Serializable]
    public struct DotNetPair
    {
        [Tooltip("namespace of class or struct")]
        public string Namespace;
        [Tooltip("likewise name of class or struct")]
        public string CLR;

        public override int GetHashCode()
        {
            int combinedHash = HashCode.Combine(Namespace, CLR);

            return combinedHash;
        }

        public override bool Equals(object obj)
        {
            return obj is DotNetPair otherStringPair &&

                   this.Namespace == otherStringPair.Namespace &&
                   this.CLR == otherStringPair.CLR;
        }

        public Type GetValueType()
        {
            string typeName;
            if (string.IsNullOrEmpty(Namespace) == true)
            {
                typeName = CLR;
            }
            else
            {
                typeName = Namespace + "." + CLR;
            }

            Type resultType = Type.GetType(typeName);
            return resultType;
        }
    }
}