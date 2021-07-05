﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;

namespace GarageKit
{
    public class VRGazeGuideArrow : MonoBehaviour
    {
        public Camera viewCamera;
        public float smoothTime = 0.3f;
        public float screenRatio = 0.9f;
        public GameObject[] arrows;
        public Color32 modeGreen = new Color32(0, 143, 61, 255);
        public Color32 modeRed = new Color32(255, 0, 0, 255);
        public bool useArrow = true;

        private GameObject directionRoot;
        private GameObject billbording;
        private RectTransform guideArrowRotRoot;
        private CanvasGroup canvsGroup;
        private float targetAngle = 0.0f;
        private float velocity;

        public enum COLOR_MODE
        {
            GREEN = 0,
            RED
        }


        void Awake()
        {

        }

        IEnumerator Start()
        {
            yield return new WaitUntil(() => Camera.main != null);
            
            if(viewCamera == null)
                viewCamera = Camera.main;

            directionRoot = new GameObject("GuideArrowTarget [Direction Root]");
            directionRoot.transform.parent = viewCamera.transform;
            directionRoot.transform.localRotation = Quaternion.Euler(new Vector3(-90.0f, 0.0f, 0.0f));

            billbording = new GameObject("GuideArrowTarget [Billbording]");
            billbording.transform.parent = directionRoot.transform;

            guideArrowRotRoot = this.gameObject.GetComponent<RectTransform>();
            canvsGroup = this.gameObject.GetComponent<CanvasGroup>();

            foreach(GameObject arrow in arrows)
            {
                arrow.GetComponent<Image>().color = modeGreen;

                iTween.MoveTo(arrow, iTween.Hash(
                    "time", 0.5f,
                    "position", arrow.transform.localPosition + new Vector3(0.0f, 50.0f, 0.0f),
                    "islocal", true,
                    "easetype", iTween.EaseType.linear,
                    "looptype", iTween.LoopType.loop));
            }
        }

        void Update()
        {
            if(viewCamera == null || directionRoot == null || billbording == null
                || guideArrowRotRoot == null || canvsGroup == null)
                return;

            VRSceneStateBase state = AppMain.Instance.sceneStateManager.CurrentState.StateObj as VRSceneStateBase;
            if(IsDevicePresent()
                && useArrow
                && !AppMain.Instance.sceneStateManager.StateChanging
                && state != null && state.viewGuideTarget != null)
            {
                Vector3 viewPt = Camera.main.WorldToViewportPoint(state.viewGuideTarget.transform.position);
                
                if(viewPt.x > 1.0f - screenRatio && viewPt.x < screenRatio
                    && viewPt.y > 1.0f - screenRatio && viewPt.y < screenRatio
                    && viewPt.z >= 0)
                {
                    canvsGroup.alpha = 0.0f;
                }
                else
                {
                    canvsGroup.alpha = 1.0f;

                    billbording.transform.LookAt(state.viewGuideTarget.transform.position);
                    billbording.transform.localRotation = Quaternion.Euler(
                        new Vector3(0.0f, billbording.transform.localRotation.eulerAngles.y, 0.0f));

                    targetAngle = Mathf.SmoothDampAngle(
                        targetAngle, billbording.transform.localRotation.eulerAngles.y, ref velocity, smoothTime);
                }
            }
            else
                canvsGroup.alpha = 0.0f;

            guideArrowRotRoot.localRotation = Quaternion.AngleAxis(targetAngle, Vector3.back);
        }


        public bool IsDevicePresent()
        {
            bool present = false;

            List<XRDisplaySubsystem> subsystems = new List<XRDisplaySubsystem>();
            SubsystemManager.GetInstances<XRDisplaySubsystem>(subsystems);
            foreach(XRDisplaySubsystem subsystem in subsystems)
            {
                if(subsystem.running)
                {
                    present = true;
                    break;
                }
            }

            return present;
        }

        // 色変更
        public void ChangeColor(COLOR_MODE mode)
        {
            foreach(GameObject arrow in arrows)
            {
                if(mode == COLOR_MODE.GREEN)
                    arrow.GetComponent<Image>().color = modeGreen;
                else if(mode == COLOR_MODE.RED)
                    arrow.GetComponent<Image>().color = modeRed;
            }
        }
    }
}
