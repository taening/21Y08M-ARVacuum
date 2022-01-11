using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

namespace ARVaccum.Development
{
    public class MyARRaycast : MonoBehaviour
    {
        private ARRaycastManager m_ARRaycastManager;
        public ARRaycastManager aRRaycastManager
        {
            get => m_ARRaycastManager;
            set => m_ARRaycastManager = value;
        }

        private static List<ARRaycastHit> m_FloorHits = new List<ARRaycastHit>();
        private static List<ARRaycastHit> m_WallHits = new List<ARRaycastHit>();

#nullable enable
        public Pose? RaycastToSetPoint(Vector2 screenPoint, PlaneClassification planeClassification)
        {
            if (m_ARRaycastManager == null)
                return null;

            if (planeClassification == PlaneClassification.Floor)
            {
                if (m_ARRaycastManager.Raycast(screenPoint, m_FloorHits, TrackableType.PlaneWithinBounds))
                    return m_FloorHits[0].pose;
                return null;
            }
            else if (planeClassification == PlaneClassification.Wall)
            {
                if (m_ARRaycastManager.Raycast(screenPoint, m_WallHits, TrackableType.PlaneWithinBounds))
                    return m_FloorHits[0].pose;
                return null;
            }
            else
                return null;

        }
#nullable disable
    }
}