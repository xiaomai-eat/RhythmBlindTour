using Qf.Commands.AudioEdit;
using QFramework;
using System;
using System.Collections.Generic;
using UnityEngine;
namespace Qf.Managers {
    public class Managers : MonoSingleton<Managers>, IController
    {
        [SerializeField]
        AudioClip audioClip;//≤‚ ‘º”‘ÿ±‡º≠∆˜“Ù∆µ
        [SerializeField]
        List<ManagerBase> ManagerOrderOfExecution = new();//π‹¿Ì∆˜÷¥––À≥–Ú
        Action _UpDate;
        Action _FixedUpDate;
        Action _LateUpDate;

        protected override void OnAwake()
        {
            GetArchitecture();

            foreach (var manager in ManagerOrderOfExecution)
            {
                manager.Init();
            }
            
            
        }
        public void Test()
        {
            this.SendCommand(new SetAudioEditAudioCommand(audioClip));//≤‚ ‘º”‘ÿ±‡º≠∆˜“Ù∆µ
            //this.SendCommand(new SetAudioEditAudioCommand(FileLoader.LoadAudioClip(FileLoader.OpenFolderPanel())));
        }
        void Update() => _UpDate?.Invoke();

        void FixedUpdate() => _FixedUpDate?.Invoke();

        void LateUpdate() => _LateUpDate?.Invoke();

        public void AddUpdate(Action action) => _UpDate += action;
        public void AddFixedUpDate(Action action) => _FixedUpDate += action;
        public void AddLateUpDate(Action action) => _LateUpDate += action;

        public void RemoveManager(ManagerBase managerBase) => ManagerOrderOfExecution.Remove(managerBase);
        public void AddManager(ManagerBase managerBase) => ManagerOrderOfExecution.Add(managerBase);

        public IArchitecture GetArchitecture()
        {
            return GameBody.Interface;
        }
    }
}
