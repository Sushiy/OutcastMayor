using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OutcastMayor.UtilityAI
{
    public abstract class Consideration : ScriptableObject
    {
        [HorizontalGroup("General")]
        [BoxGroup("General/Left"), LabelWidth(50)]
        public string Name;

        [BoxGroup("General/Left"), LabelWidth(130)]
        [DisplayAsString(false), Multiline(2)]
        public string RequiredDataTypes;

        [HideLabel]
        [BoxGroup("General/Right")]
        [SerializeField] private AnimationCurve curve;
        [BoxGroup("General/Right")]
        [SerializeField] protected float maxValue = 1.0f;

        /// <summary>
        /// An exclusive consideration HAS to be greater than 0 so that the action in whole is valid
        /// </summary>
        [BoxGroup("Settings")]
        public bool isExclusiveConsideration = false;
        /// <summary>
        /// Overrides Curve with: 0.0f -> 0.0f; else 1.0f
        /// </summary>
        [BoxGroup("Settings")]
        public bool zeroStepCurve = false;
        /// <summary>
        /// Overrides Curve with: 1.0f -> 1.0f; else 0.0f
        /// </summary>
        [BoxGroup("Settings")]
        public bool oneStepCurve = false;

        public abstract float ScoreConsideration(UtilityAICharacter controller, ConsiderationData considerationData);

        public abstract bool TryGetConsiderationData(Object[] instanceData, out ConsiderationData considerationData);

        public abstract System.Type[] GetRequiredDataTypes();

        public virtual bool HasAllData(System.Type[] actionData)
        {
            System.Type[] types = GetRequiredDataTypes();
            bool hasData = true;
            for(int i = 0; i < types.Length; i++)
            {
                bool typeFound = false;
                for(int j = 0; j < actionData.Length; j++)
                {
                    if(types[i] == actionData[j] || actionData[j].IsSubclassOf(types[i]))
                    {
                        typeFound = true;
                        break;
                    }
                }
                if(!typeFound)
                {
                    hasData = false;
                    break;
                }
            }
            return hasData;
        }

        public virtual float Evaluate(float input)
        {
            if(zeroStepCurve)
            {
                if (input == 0.0f)
                {
                    return 0.0f;
                }
                else
                {
                    return 1.0f;
                }
            }
            else if(oneStepCurve)
            {
                if (input == 1.0f)
                {
                    return 1.0f;
                }
                else
                {
                    return 0.0f;
                }
            }
            else
                return Mathf.Clamp01(curve.Evaluate(input));

        }

        private void OnValidate()
        {
            System.Type[] t = GetRequiredDataTypes();
            RequiredDataTypes = "";
            for (int i = 0; i < t.Length; i++)
            {
                RequiredDataTypes += t[i].Name + ",";
            }
        }
    }


    //There should be a way to save consideration in order to reuse 
    public class ConsiderationData : System.IEquatable<ConsiderationData>
    {
        public Consideration consideration;
        public Object[] data;

        public ConsiderationData(Consideration consideration, Object[] considerationData)
        {
            this.consideration = consideration;
            this.data = considerationData;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as ConsiderationData);
        }

        public bool Equals(ConsiderationData other)
        {
            if (consideration == other.consideration)
            {
                if (data.Length == other.data.Length)
                {
                    for (int i = 0; i < data.Length; i++)
                    {
                        if (data[i] != other.data[i])
                        {
                            return false;
                        }
                    }

                    return true;
                }
            }
            return false;
        }

        public override int GetHashCode()
        {
            var hashCode = 1938039292; 
            hashCode = hashCode * -1521134295 + consideration.GetHashCode();
            for (int i = 0; i < data.Length; i++)
            {
                if (data == null)
                {
                    Debug.LogError(consideration.Name + " data null");
                    break;
                }
                if (data[i] == null)
                {
                    Debug.LogError(consideration.Name + " data i null");
                    break;
                }
                hashCode = hashCode * -1521134295 + data[i].GetHashCode();
            }
            return hashCode;
        }
    }
}
