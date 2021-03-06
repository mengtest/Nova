﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using UnityEngine;

namespace Nova
{
    [RequireComponent(typeof(SpriteChangerWithFade))]
    public class BackgroundController : MonoBehaviour, IRestorable
    {
        public string imageFolder;

        private GameState gameState;

        private SpriteChangerWithFade _spriteChanger;

        private void Awake()
        {
            _spriteChanger = GetComponent<SpriteChangerWithFade>();
            LuaRuntime.Instance.BindObject("backgroundController", this);
            gameState = Utils.FindNovaGameController().GetComponent<GameState>();
            gameState.AddRestorable(this);
        }

        private void OnDestroy()
        {
            gameState.RemoveRestorable(this);
        }

        private string currentImageName;

        #region Methods called by external scripts

        /// <summary>
        /// Change the background image
        /// This method is designed to be called by external scripts
        /// </summary>
        /// <param name="imageName">The name of the image file</param>
        public void SetImage(string imageName)
        {
            _spriteChanger.sprite = AssetsLoader.GetSprite(System.IO.Path.Combine(imageFolder, imageName));
            currentImageName = imageName;
        }

        public void ClearImage()
        {
            _spriteChanger.sprite = null;
            currentImageName = null;
        }

        #endregion

        [Serializable]
        private class RestoreData : IRestoreData
        {
            public string currentImageName { get; private set; }

            public RestoreData(string currentImageName)
            {
                this.currentImageName = currentImageName;
            }
        }

        public string restorableObjectName
        {
            get { return "backgroundController"; }
        }

        public IRestoreData GetRestoreData()
        {
            return new RestoreData(currentImageName);
        }

        public void Restore(IRestoreData restoreData)
        {
            var data = restoreData as RestoreData;
            if (data.currentImageName != null)
            {
                SetImage(data.currentImageName);
            }
            else
            {
                ClearImage();
            }
        }
    }
}