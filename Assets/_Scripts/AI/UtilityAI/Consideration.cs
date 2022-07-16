using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OutcastMayor.UtilityAI
{
    public abstract class Consideration : ScriptableObject
    {
        [HorizontalGroup("General")]

        [BoxGroup("General/Left"), LabelWidth(130)]
        [DisplayAsString(false), Multiline(2)]
        public string RequiredDataTypes;

        public static Dictionary<ConsiderationData, float> considerationMemory;

        public virtual float GetScore(UtilityAICharacter controller, ConsiderationData considerationData)
        {
            if (considerationMemory.ContainsKey(considerationData))
            {
                return considerationMemory[considerationData];
            }
            else
            {
                float newScore = CalculateScore(controller, considerationData);
                considerationMemory.Add(considerationData, newScore);
                return newScore;
            }
        }

        protected abstract float CalculateScore(UtilityAICharacter controller, ConsiderationData considerationData);

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

        private void OnValidate()
        {
            System.Type[] t = GetRequiredDataTypes();
            RequiredDataTypes = "";
            for (int i = 0; i < t.Length; i++)
            {
                RequiredDataTypes += t[i].Name + ",";
            }
        }

        public abstract string[] GetSourceValueNames();

        public abstract float[] GetSourceValues(UtilityAICharacter controller, ConsiderationData considerationData);

        [System.Serializable]
        public struct ConsiderationLog
        {
            public Consideration consideration;
            public string[] considerationDataNames;
            public string[] sourceValueNames;
            public float[] sourceValues;

            public float score;

            public ConsiderationLog(Consideration consideration)
            {
                this.consideration = consideration;
                considerationDataNames = new string[0];
                sourceValueNames = consideration.GetSourceValueNames();
                sourceValues = new float[0];
                score = 0;
            }

            public void SetScore(float score)
            {
                this.score = score;
            }

            public void SetSourceValues(float[] sourceValues)
            {
                this.sourceValues = sourceValues;
            }

            public void SetConsiderationDataNames(ConsiderationData considerationData)
            {
                considerationDataNames = new string[considerationData.data.Length];
                for(int i = 0; i < considerationDataNames.Length; i++)
                {
                    considerationDataNames[i] = considerationData.data[i].GetType().ToString() + " at " + considerationData.data[i].name; 
                }
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
                    Debug.LogError(consideration.name + " data null");
                    break;
                }
                if (data[i] == null)
                {
                    Debug.LogError(consideration.name + " data i null");
                    break;
                }
                hashCode = hashCode * -1521134295 + data[i].GetHashCode();
            }
            return hashCode;
        }
    }
    [System.Serializable]
    public class ConsiderationInstance
    {
        [HorizontalGroup("General"), HideLabel]
        public Consideration consideration;

        [HideLabel]
        [HorizontalGroup("General")]
        [SerializeField] private AnimationCurve curve;
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

        public virtual float ScoreConsideration(UtilityAICharacter controller, ConsiderationData considerationData)
        {
            float score = consideration.GetScore(controller, considerationData);

            if (zeroStepCurve)
            {
                if (score == 0.0f)
                {
                    return 0.0f;
                }
                else
                {
                    return 1.0f;
                }
            }
            else if (oneStepCurve)
            {
                if (score == 1.0f)
                {
                    return 1.0f;
                }
                else
                {
                    return 0.0f;
                }
            }
            else
                return Mathf.Clamp01(curve.Evaluate(score));

        }
    }
}
