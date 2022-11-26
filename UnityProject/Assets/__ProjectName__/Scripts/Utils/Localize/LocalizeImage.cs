﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GarageKit.Localize
{
    [ExecuteInEditMode]
    public class LocalizeImage : MonoBehaviour, ILocalize
    {
        private static List<LocalizeImage> localizeList = new List<LocalizeImage>();
        public static void LocalizeAll(LANGUAGE lang)
        {
            foreach(LocalizeImage img in localizeList)
                img.Localize(lang);
        }

        public List<Sprite> localizeSprites;
        public LANGUAGE lang;

        private Image uiImg;


        void Awake()
        {
            localizeList.Add(this);

            uiImg = this.gameObject.GetComponent<Image>();
        }

        void Start()
        {

        }

        void Update()
        {
            if(Application.isEditor)
                Localize(this.lang);
        }

        void OnDestroy()
        {
            localizeList.Remove(this);
        }


        public void Localize(LANGUAGE lang)
        {
            this.lang = lang;

            if(localizeSprites.Count > (int)lang)
                uiImg.sprite = localizeSprites[(int)lang];
        }
    }
}
