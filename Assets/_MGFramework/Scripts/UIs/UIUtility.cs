using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MGFramework
{
    public static class UIUtility 
    {
        public static IEnumerator FadeRoutine(this CanvasGroup group, float fromAlpha, float toAlpha, float duration)
        {
            float timer = 0f;
            while (timer < duration)
            {
                float normal = timer / duration;
                group.alpha = Mathf.Lerp(fromAlpha, toAlpha, normal);

                yield return null;
                timer += Time.deltaTime;
            }

            group.alpha = toAlpha;
        }
    }
}
