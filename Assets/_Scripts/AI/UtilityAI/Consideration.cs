using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UtilityAI
{
    public abstract class Consideration : ScriptableObject
    {
        protected const string LEFT_VERTICAL_GROUP = "Split/Left";
        protected const string STATS_BOX_GROUP = "Split/Left/Stats";
        protected const string GENERAL_SETTINGS_VERTICAL_GROUP = "Split/Left/General Settings/Split/Right";

        [BoxGroup(LEFT_VERTICAL_GROUP + "/General Settings")]
        public string Name;

        [HideLabel]
        [BoxGroup("A")]
        [SerializeField] private AnimationCurve curve;
        [BoxGroup("A")]
        [SerializeField] protected float maxValue = 1.0f;

        /// <summary>
        /// An exclusive consideration HAS to be greater than 0 so that the action in whole is valid
        /// </summary>
        [BoxGroup("B")]
        public bool isExclusiveConsideration = false;
        /// <summary>
        /// Overrides Curve with: 0.0f -> 0.0f; else 1.0f
        /// </summary>
        [BoxGroup("B")]
        public bool zeroStepCurve = false;
        /// <summary>
        /// Overrides Curve with: 1.0f -> 1.0f; else 0.0f
        /// </summary>
        [BoxGroup("B")]
        public bool oneStepCurve = false;

        public abstract float ScoreConsideration(UtilityAICharacter controller, ConsiderationData considerationData);

        public abstract bool TryGetConsiderationData(Object[] instanceData, out ConsiderationData considerationData);

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
    }
}
