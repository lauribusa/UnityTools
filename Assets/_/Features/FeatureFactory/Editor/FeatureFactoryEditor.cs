using System;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.KeyCode;
using static UnityEngine.Event;
using static UnityEngine.EventType;
using static System.Threading.Tasks.Task;

namespace FeatureFactory.Editor
{
    public class FeatureFactoryEditor: EditorWindow
    {
        private bool _isFocused;
        private Toggle _hasEditor;
        private Toggle _hasRuntime;
        private Toggle _hasData;
        private TextField _featureName;
        private Button _confirm;
        
        [MenuItem("Assets/Create/Feature", priority = 11)]
        public static void ShowWindow()
        {
            var wnd = GetWindow<FeatureFactoryEditor>();
            wnd.titleContent = new GUIContent("Feature Factory");
        }
        public void CreateGUI()
        {
            _isFocused = false;
            var root = rootVisualElement;
            _featureName = new TextField("Feature Name");
            _hasEditor = new Toggle("Editor");
            _hasRuntime = new Toggle("Runtime");
            _hasData = new Toggle("Data");
            var group = new GroupBox("Include Assemblies");
            group.Add(_hasEditor);
            group.Add(_hasRuntime);
            group.Add(_hasData);
           
            _confirm = new Button(() =>
            {
                var success = GenerateFeature(_featureName.text, _hasEditor.value, _hasData.value, _hasRuntime.value);
                OnSuccess(success);
            })
            {
                text = "Generate"
            };
            root.Add(group);
            root.Add(_featureName);
            root.Add(_confirm);
            Focus();
        }

        public void OnGUI()
        {
            if (!_isFocused)
            {
                _isFocused = true;
                _featureName.Focus();
            }
            var keyPressed = current.type is KeyUp 
                             && current.keyCode is Return or KeypadEnter;

            if (!keyPressed) return;
            
            var success = GenerateFeature(_featureName.text, _hasEditor.value, _hasData.value, _hasRuntime.value);
            OnSuccess(success);
        }

        private async void OnSuccess(bool success, int milliseconds = 2000)
        {
            if (!success) return;
            SceneView.lastActiveSceneView.ShowNotification(new GUIContent("Feature created."));
            await Delay(milliseconds);
            AssetDatabase.Refresh();
            Close();
        }

        private bool GenerateFeature(string featureName, bool hasEditor, bool hasData, bool hasRuntime)
        {
            if (!hasEditor && !hasData && !hasRuntime)
            {
                EditorUtility.DisplayDialog("Error", "Please select at least one assembly type.", "OK");
                return false;
            }
            
            if (string.IsNullOrWhiteSpace(featureName))
            {
                EditorUtility.DisplayDialog("Error", "Feature name is empty.", "OK");
                return false;
            }

            if (!IsValidFilename(featureName))
            {
                EditorUtility.DisplayDialog("Error", "Feature name contains invalid characters.", "OK");
                return false;
            }
            var createdAsset = false;
            if (hasData)
            {
                if(GenerateAssemblyDefinitionFile(featureName, DATA)) createdAsset = true;
            }

            if (hasEditor)
            {
                if(GenerateAssemblyDefinitionFile(featureName, EDITOR)) createdAsset = true;
            }

            if (hasRuntime)
            {
                if(GenerateAssemblyDefinitionFile(featureName, RUNTIME)) createdAsset = true;
            }

            return createdAsset;
        }

        private bool GenerateAssemblyDefinitionFile(string featureName, string assemblyType)
        {
            if (FileAlreadyExists(featureName, $"{featureName}.{assemblyType}.{ASMDEF}", assemblyType)) return false;
            var jsonContent = GetJsonFromAssemblyDefModel(featureName, assemblyType);
            GenerateFile(featureName, $"{featureName}.{assemblyType}.{ASMDEF}", assemblyType, jsonContent);
            return true;
        }

        private void GenerateFile(string featureName, string fileName, string assemblyType, string fileContent)
        {
            if (FileAlreadyExists(featureName, fileName, assemblyType)) return;
            var directoryInfo = Directory.CreateDirectory($@"{PATH}\{featureName}\{assemblyType}");
            using var writer = new StreamWriter($@"{directoryInfo.FullName}\{fileName}");
            writer.WriteLine(fileContent);
        }

        private static bool FileAlreadyExists(string featureName, string fileName, string assemblyType)
        {
            if (!File.Exists($@"{PATH}\{featureName}\{assemblyType}\{fileName}")) return false;
            EditorUtility.DisplayDialog("Error", $@"File at path {PATH}\{featureName}\{assemblyType}\{fileName} already exists.", "OK");
            return true;
        }

        private string GetJsonFromAssemblyDefModel(string featureName, string assemblyType)
        {
            var assembly = new AssemblyDefinitionModel($"{featureName}.{assemblyType}");
            if (assemblyType == EDITOR)
            {
                assembly.includePlatforms = new[] { EDITOR };
            }
            var json = JsonUtility.ToJson(assembly);
            return json;
        }
        
        private bool IsValidFilename(string testName)
        {
            var containsABadCharacter = new Regex("["+ Regex.Escape(new string(Path.GetInvalidFileNameChars())) +"]");
            return !containsABadCharacter.IsMatch(testName);
        }

        private const string ASMDEF = "asmdef";
        private const string PATH = @"Assets\_\Features";
        private const string EDITOR = "Editor";
        private const string DATA = "Data";
        private const string RUNTIME = "Runtime";
    }
    
    public class AssemblyDefinitionModel
    {
        public AssemblyDefinitionModel(string featureName)
        {
            name = $"{featureName}";
            autoReferenced = true;
            rootNamespace = $"{featureName}";
            references = Array.Empty<string>();
            includePlatforms = Array.Empty<string>();
            excludePlatforms = Array.Empty<string>();
            allowUnsafeCode = false;
            overrideReferences = false;
            precompiledReferences = Array.Empty<string>();
            defineConstraints = Array.Empty<string>();
            versionDefines = Array.Empty<string>();
            noEngineReferences = false;
        }

        public string name;
        public string rootNamespace;
        public string[] references;
        public string[] includePlatforms;
        public string[] excludePlatforms;
        public bool allowUnsafeCode;
        public bool overrideReferences;
        public string[] precompiledReferences;
        public bool autoReferenced;
        public string[] defineConstraints;
        public string[] versionDefines;
        public bool noEngineReferences;
    }
}