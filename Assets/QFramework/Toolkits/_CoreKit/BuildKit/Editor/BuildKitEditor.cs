#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace QFramework
{
    [PackageKitGroup("QFramework")]
    [DisplayNameCN("BuildKit 自定义打包")]
    [DisplayNameEN("BuildKit CustomBuild")]
    [PackageKitRenderOrder(10)]
    public class BuildKitEditor : IPackageKitView
    {
        public EditorWindow EditorWindow { get; set; }
        public void Init()
        {
        }

        public class BuildActionView
        {
            public string DisplayName;
            public IBuildAction Action;
        }

        public List<BuildActionView> mActionViews = new List<BuildActionView>();
        
        public void OnShow()
        {
            var interfaceType = typeof(IBuildAction);
            mActionViews.Clear();
            foreach (var type in ReflectionExtension.GetAssemblyCSharpEditor().GetTypes()
                         .Where(t=>!t.IsAbstract && interfaceType.IsAssignableFrom(t)))
            {
                var displayName = "";
                if (type.GetAttribute<DisplayNameAttribute>() != null)
                {
                    displayName = type.GetAttribute<DisplayNameAttribute>().DisplayName;
                }
                else
                {
                    displayName = type.Name;
                }

                var action = type.CreateInstance<IBuildAction>();
                action.Init();
                mActionViews.Add(new BuildActionView()
                {
                    Action = action,
                    DisplayName = displayName
                });
            }
        }

        public void OnUpdate()
        {
        }

        private FluentGUIStyle mActionNameStyle = FluentGUIStyle.Label()
            .FontBold();
        public void OnGUI()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label($"Version:{PlayerSettings.bundleVersion}");
            GUILayout.Space(5);
            GUILayout.EndHorizontal();
            
            foreach (var buildActionView in mActionViews)
            {
                GUILayout.BeginVertical("box");
                GUILayout.Label(buildActionView.DisplayName,mActionNameStyle);
                buildActionView.Action.OnGUI();
                if (GUILayout.Button("构建"))
                {
                    buildActionView.Action.Build();
                    EditorWindow.Close();
                }
                GUILayout.EndVertical();
            }
        }

        public void OnHide()
        {
            mActionViews.Clear();
        }

        public void OnWindowGUIEnd()
        {
        }

        public void OnDispose()
        {
        }
    }
}
#endif