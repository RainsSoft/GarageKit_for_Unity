﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using DG.Tweening;

namespace GarageKit
{
    public class VRGazeGuideArrow : MonoBehaviour
    {
        public Camera viewCamera;
        public Transform viewGuideTarget;
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
            directionRoot.transform.localPosition = new Vector3(0.0f, 0.0f, 10.0f);
            directionRoot.transform.localRotation = Quaternion.Euler(new Vector3(-90.0f, 0.0f, 0.0f));

            billbording = new GameObject("GuideArrowTarget [Billbording]");
            billbording.transform.parent = directionRoot.transform;
            billbording.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);

            guideArrowRotRoot = this.gameObject.GetComponent<RectTransform>();
            canvsGroup = this.gameObject.GetComponent<CanvasGroup>();

            foreach(GameObject arrow in arrows)
            {
                arrow.GetComponent<Image>().color = modeGreen;

                arrow.transform.DOLocalMove(new Vector3(0.0f, 50.0f, 0.0f), 0.5f)
                    .SetRelative()
                    .SetEase(Ease.Linear)
                    .SetLoops(-1)
                    .Play();
            }
        }

        void Update()
        {
            if(viewCamera == null || directionRoot == null || billbording == null
                || guideArrowRotRoot == null || canvsGroup == null)
                return;

            if(useArrow && viewGuideTarget != null)
            {
                Vector3 viewPt = viewCamera.WorldToViewportPoint(viewGuideTarget.position);

                if(viewPt.x > 1.0f - screenRatio && viewPt.x < screenRatio
                    && viewPt.y > 1.0f - screenRatio && viewPt.y < screenRatio
                    && viewPt.z >= 0)
                {
                    canvsGroup.alpha = 0.0f;
                }
                else
                {
                    canvsGroup.alpha = 1.0f;

                    billbording.transform.LookAt(viewGuideTarget.position);
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
