using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace Penny
{
    public class KinectGameManager : MonoBehaviour
    {
        [SerializeField]
        private Camera m_WallCamera = null;
        [SerializeField]
        private Transform m_LeftHandOverlay = null;
        [SerializeField]
        private Transform m_RightHandOverlay = null;
        [SerializeField]
        private Transform m_LeftSaber = null;
        [SerializeField]
        private Transform m_RightSaber = null;
        [SerializeField]
        private int m_PlayerIndex = 0;
        [SerializeField]
        private float m_SmoothFactor = 10f;

        private KinectManager m_KinectManager = null;
        private PortraitBackground m_PortraitBack = null;
        private long m_UserId = 0;
        private Rect m_BackgroundRect;
        private void Start()
        {
            if (KinectManager.Instance)
            {
                m_KinectManager = KinectManager.Instance;
                m_KinectManager.enabled = true;

                m_PortraitBack = PortraitBackground.Instance;
            }
            else
            {
                m_KinectManager = gameObject.GetOrAddComponent<KinectManager>();
            }

        }

        private void OnDestroy()
        {
            if (m_KinectManager)
                m_KinectManager.enabled = false;
        }

        private void Update()
        {
            if (m_KinectManager && m_KinectManager.IsInitialized() && m_WallCamera)
            {
                m_BackgroundRect = m_WallCamera.pixelRect;
                if (m_PortraitBack && m_PortraitBack.enabled)
                {
                    m_BackgroundRect = m_PortraitBack.GetBackgroundRect();
                }

                if (m_KinectManager.IsUserDetected())
                {
                    m_UserId = m_KinectManager.GetUserIdByIndex(m_PlayerIndex);

                    OverlayJoint(m_UserId, (int)KinectInterop.JointType.HandLeft, m_LeftHandOverlay, m_BackgroundRect);
                    OverlayJoint(m_UserId, (int)KinectInterop.JointType.HandRight, m_RightHandOverlay, m_BackgroundRect);

                    CalcRotation();
                }
                else
                {
                    m_LeftHandOverlay.localPosition = Vector3.zero;
                    m_RightHandOverlay.localPosition = Vector3.zero;
                }
            }
        }

        private void OverlayJoint(long userId, int jointIndex, Transform overlayObj, Rect backgroundRect)
        {
            if (m_KinectManager.IsJointTracked(userId, jointIndex))
            {
                Vector3 posJoint = m_KinectManager.GetJointKinectPosition(userId, jointIndex);
                if (posJoint != Vector3.zero)
                {
                    Vector2 posDepth = m_KinectManager.MapSpacePointToDepthCoords(posJoint);
                    ushort depthValue = m_KinectManager.GetDepthForPixel((int)posDepth.x, (int)posDepth.y);

                    if (depthValue > 0)
                    {
                        Vector2 posColor = m_KinectManager.MapDepthPointToColorCoords(posDepth, depthValue);

                        float xNorm = posColor.x / m_KinectManager.GetColorImageWidth();
                        float yNorm = 1.0f - posColor.y / m_KinectManager.GetColorImageHeight();

                        if (overlayObj && m_WallCamera)
                        {
                            float distanceToCamera = overlayObj.position.z - m_WallCamera.transform.position.z;
                            posJoint = m_WallCamera.ViewportToWorldPoint(new Vector3(xNorm, yNorm, distanceToCamera));
                            //posJoint.x = (Mathf.Round(posJoint.x * 1000)) / 1000;
                            //posJoint.y = (Mathf.Round(posJoint.y * 1000)) / 1000;
                            posJoint.z = 0;
                            //overlayObj.position = posJoint;
                            //Vector3 pos = Vector3.Lerp(overlayObj.position, posJoint, Time.deltaTime * m_SmoothFactor);
                            //pos.x = ((int)Mathf.Round(pos.x * 10)) / 10f;
                            //pos.y = ((int)Mathf.Round(pos.y * 10)) / 10f;
                            //pos.z = 0;
                            //overlayObj.position = pos;
                            overlayObj.position = Vector3.Lerp(overlayObj.position, posJoint, Time.deltaTime * m_SmoothFactor);
                            overlayObj.rotation = Quaternion.identity;
                        }
                    }
                }
            }
        }

        Quaternion quaternion = Quaternion.identity;
        Vector3 to;
        private void CalcRotation()
        {
            Vector3 pos = m_LeftHandOverlay.position;
            pos.x = ((int)Mathf.Round(pos.x * 10)) / 10f;
            pos.y = ((int)Mathf.Round(pos.y * 10)) / 10f;
            pos.z = 0;

            to = pos - m_LeftSaber.position;
            quaternion = Quaternion.FromToRotation(Vector3.forward, to);
            m_LeftSaber.rotation = Quaternion.Lerp(m_LeftSaber.rotation, quaternion, Time.deltaTime * m_SmoothFactor);

            pos = m_RightHandOverlay.position;
            pos.x = ((int)Mathf.Round(pos.x * 10)) / 10f;
            pos.y = ((int)Mathf.Round(pos.y * 10)) / 10f;
            pos.z = 0;

            to = m_RightHandOverlay.position - m_RightSaber.position;
            quaternion = Quaternion.FromToRotation(Vector3.forward, to);
            m_RightSaber.rotation = Quaternion.Lerp(m_RightSaber.rotation, quaternion, Time.deltaTime * m_SmoothFactor);
        }

    }

}