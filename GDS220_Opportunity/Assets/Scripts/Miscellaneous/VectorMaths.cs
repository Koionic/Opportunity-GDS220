using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VectorMaths
{
    public static class Maths
    {
        public static float GetDistance(Vector3 start, Vector3 end)
        {
            Vector3 distanceVector = end - start;

            return distanceVector.magnitude;
        }

        public static float GetAngle(Transform start, Vector3 end)
        {
            Vector3 normalisedHeading = start.forward;
            Vector3 normalisedTargetVector = (end - start.position).normalized;
            float dotProduct = Vector3.Dot(normalisedHeading, normalisedTargetVector);
            float angle = Mathf.Acos(dotProduct) * Mathf.Rad2Deg;

            return angle;
        }

        public static float GetSignedHorizontalAngle(Transform start, Vector3 end)
        {
            Vector3 normalisedHeading = start.forward;
            Vector2 normalisedHorizontalHeading = new Vector2(normalisedHeading.x, normalisedHeading.z);

            Vector3 normalisedTargetVector = (end - start.position).normalized;
            Vector2 normalisedHorizontalTargetVector = new Vector2(normalisedTargetVector.x, normalisedTargetVector.z);

            float questAngle = Vector2.SignedAngle(normalisedHorizontalHeading, normalisedHorizontalTargetVector);

            return questAngle;
        }
    }
}
