using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace DrumSmasher
{
    public static class Extensions
    {
        public static IEnumerator MoveOverSecondsLocal(this Transform obj, Vector3 start, Vector3 end, float seconds)
        {
            float elapsedTime = 0;

            while (elapsedTime < seconds)
            {
                obj.transform.localPosition = Vector3.Lerp(start, end, (elapsedTime / seconds));
                elapsedTime += Time.deltaTime;

                yield return new WaitForEndOfFrame();
            }

            obj.transform.localPosition = end;
        }

        public static IEnumerator MoveOverSeconds(this Transform obj, Vector3 start, Vector3 end, float seconds)
        {
            float elapsedTime = 0;

            while (elapsedTime < seconds)
            {
                obj.transform.localPosition = Vector3.Lerp(start, end, (elapsedTime / seconds));
                elapsedTime += Time.deltaTime;

                yield return new WaitForEndOfFrame();
            }

            obj.transform.localPosition = end;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="r">Used to distinguish <see cref="MoveOverSeconds(Transform, Vector3, Vector3, float)"/> and <see cref="MoveOverSeconds(RectTransform, Vector3, Vector3, float, bool)"/></param>
        /// <returns></returns>
        public static IEnumerator MoveOverSeconds(this RectTransform rect, Vector3 start, Vector3 end, float seconds, bool r)
        {
            float elapsedTime = 0;

            while(elapsedTime < seconds)
            {
                rect.anchoredPosition3D = Vector3.Lerp(start, end, (elapsedTime / seconds));
                elapsedTime += Time.deltaTime;

                yield return new WaitForEndOfFrame();
            }

            rect.anchoredPosition3D = end;
        }
    }
}
