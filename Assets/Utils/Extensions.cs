using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

namespace Utils
{
    public static class Extensions
    {
        /// <summary>
        /// Get all interfaces in the given scene
        /// </summary>
        public static List<T> FindInterfaces<T>(this Scene scene)
        {
            var interfaces = new List<T>();
            var rootGameObjects = scene.GetRootGameObjects();
            foreach (var rootGameObject in rootGameObjects)
            {
                var childrenInterfaces = rootGameObject.GetComponentsInChildren<T>();
                foreach (var childInterface in childrenInterfaces)
                    interfaces.Add(childInterface);
            }

            return interfaces;
        }

        public static float EvaluateMinMaxCurve(this ParticleSystem.MinMaxCurve curve, int t = 0)
        {
            switch (curve.mode)
            {
                case ParticleSystemCurveMode.Constant:
                    return curve.constant;
                case ParticleSystemCurveMode.Curve:
                    return curve.Evaluate(t);
                case ParticleSystemCurveMode.TwoCurves:
                    return curve.EvaluateMinMaxCurve(t);
                case ParticleSystemCurveMode.TwoConstants:
                    return Random.Range(curve.constantMin, curve.constantMax);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}