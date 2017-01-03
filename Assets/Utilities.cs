using UnityEngine;
using System.Collections;

namespace Utilities
{
    public class Randomness
    {
        public static T GetRandomValue<T>(params ProbabilityElement<T>[] selections)
        {
            float rand = Random.value;
            float currentProb = 0;
            foreach (var selection in selections)
            {
                currentProb += selection.probability;
                if (rand <= currentProb)
                    return selection.element;
            }

            //will happen if the input's probabilities sums to less than 1
            //throw error here if that's appropriate
            return default(T);
        }
    }

    public class ProbabilityElement<T>
    {
        public T element;
        public float probability;

        public ProbabilityElement(T _element, float _probability)
        {
            element = _element;
            probability = _probability;
        }
    }

}


