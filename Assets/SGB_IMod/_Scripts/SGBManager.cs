using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SGB_IMod
{
    public static class SGBManager
    {
        //
        //  Settings
        //

        // The name of the scene to load into when doing any SGB-related tasks.
        private static readonly string sceneNameWorkspace = "IModWorkspace";

        // The resource path of the dev overlay prefab
        private static readonly string resourcePathPrefabDevOverlay = "IMod_DevOverlay";


        //
        //  Variables
        //

        private static readonly GameObject prefabDevOverlay = null;
        private static GameObject instanceDevOverlay = null;


        static SGBManager()
        {
            // Load the dev overlay prefab
            prefabDevOverlay = Resources.Load(resourcePathPrefabDevOverlay) as GameObject;
        }




        // Load into a Smile Game Builder game, given the location of it's assets
        public static void LoadSmileGame(string smileGameName)
        {
            // Check to see if there's a smile game by this name
            if(!DoesSGBSubpathExist(smileGameName))
            {
                throw new System.Exception("No Smile Game Builder game with this subpath could be found!");
            }

            // Set the game subpath we want to load
            UnityEntry.gameSubpathName = smileGameName;

            // Start async loading the scene
            var asyncLoadScene = EnterWorkspaceScene();

            // When the scene is done loading...
            asyncLoadScene.completed += delegate {
                CreateSGBRequiredGameObjects();
                CreateCustomRequiredGameObjects();
            };
        }

        // Unload from current Smile Builder Game, and load into a specific scene
        public static void UnloadSmileGame(string sceneToLoadTo)
        {
            // Start Async re-loading into the empty workspace scene
            var asyncLoadScene = EnterWorkspaceScene();

            // When the scene is done loading...
            asyncLoadScene.completed += delegate {
                DestroyCustomRequiredGameObjects();

                // Load into the scene we want to return to.
                asyncLoadScene = SceneManager.LoadSceneAsync(sceneToLoadTo);
            };
        }



        // Asynchronously load into the workspace scene
        private static AsyncOperation EnterWorkspaceScene()
        {
            return SceneManager.LoadSceneAsync(sceneNameWorkspace);
        }

        // Construct the GameObjects that SGB needs to initialize
        private static void CreateSGBRequiredGameObjects()
        {
            CreateSGBRequiredGameObjectsHelper_MainCamera();
            CreateSGBRequiredGameObjectsHelper_ParentAndDummy("MapScene");
            CreateSGBRequiredGameObjectsHelper_ParentAndDummy("BattleScene");
            CreateSGBRequiredGameObjectsHelper_ParentAndDummy("Sound");
            CreateSGBRequiredGameObjectsHelper_ParentAndDummy("Template");
            CreateSGBRequiredGameObjectsHelper_UnityAds();
            CreateSGBRequiredGameObjectsHelper_UnityEntry();
        }

        // Helper class for CreateSGBRequiredGameObjects. Creates the Main Camera
        private static void CreateSGBRequiredGameObjectsHelper_MainCamera()
        {
            // Create the Main Camera
            GameObject mainCameraObject = new GameObject(
                "Main Camera",
                typeof(Camera),
                typeof(FlareLayer),
                typeof(AudioListener)
            );

            // Set the Main Camera's Tag and Layer
            mainCameraObject.tag = "MainCamera";
            mainCameraObject.layer = 5;     // (UI Layer)
        }

        // Helper class for CreateSGBRequiredGameObjects. Creates the Entry object
        private static void CreateSGBRequiredGameObjectsHelper_UnityEntry()
        {
            // Create the Entry Object
            GameObject unityEntryObject = new GameObject(
                "Entry",
                typeof(UnityEntry)
            );
        }

        // Helper class for CreateSGBRequiredGameObjects. Creates an empty GameObject with a dummy child object,
        // given the parent object's name.
        private static void CreateSGBRequiredGameObjectsHelper_ParentAndDummy(string parentObjectName)
        {
            // Create the parent object, and a dummy child object (why is this needed? idk)
            GameObject parentObject = new GameObject(parentObjectName);
            GameObject childDummyObject = new GameObject("Dummy");
            childDummyObject.transform.SetParent(parentObject.transform);
        }

        // Helper class for CreateSGBRequiredGameObjects. Creates the Unity Ads Manager
        private static void CreateSGBRequiredGameObjectsHelper_UnityAds()
        {
            // Create the Unity Ads object
            GameObject unityAdsObject = new GameObject(
                "UnityAds",
                typeof(UnityAdsManager)
            );
        }

        private static void CreateCustomRequiredGameObjects()
        {
            // Create the dev UI
            instanceDevOverlay = GameObject.Instantiate(prefabDevOverlay);
        }




        private static void DestroyCustomRequiredGameObjects()
        {
            // Destroy the dev UI
            GameObject.Destroy(instanceDevOverlay);
        }




        private static bool DoesSGBSubpathExist(string gameSubpath)
        {
            string potentialAssetPath = Path.Combine("SGB", gameSubpath, "assets");

            // Try and load the file
            var assetFile = Resources.Load(potentialAssetPath);

            // If it has been loaded correctly, it shouldn't be null.
            return assetFile != null;
        }
    }
}