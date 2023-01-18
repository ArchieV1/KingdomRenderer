using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;
using Assets;
using UnityEngine;
using Harmony;
using UnityStandardAssets.ImageEffects;
using Zat.Shared;
using Zat.Shared.ModMenu.API;
using Zat.Shared.ModMenu.Interactive;
using Colour = UnityEngine.Color;

namespace KingdomRenderer
{
    public class KingdomRenderer : MonoBehaviour
    {
        // USER SETTINGS
        /* 
         Change these values to change where images will be saved.
         Change `SavePathAppData` to change which subfolder files saved with the settings "AppData" will be saved
         Change `SavePathSteamApps` to change which subfolder files save with the settings "SteamApps" will be saved
         
         Unfortunately because of how KC works it is not possible to set other file saving destination. 
         Only directories (Folders) below this these two
         */
        
        // C:\Users\USERNAME\AppData\LocalLow\LionShield\Kingdoms and Castles\
        private const string SavePathAppData = "/";

        // ~\steamapps\common\Kingdoms and Castles\KingdomsAndCastles_Data\mods\KingdomRenderer\Renders
        private const string SavePathSteamApps = "/Renders/";
        
        // Instances
        public KCModHelper helper;
        public static KingdomRenderer inst;
        
        private string _renderWidth = "";
        private string _renderHeight = "";
        private static bool _shouldRender;
        private static bool _cloudSetting;
        public bool finishedRendering;

        // Settings to be init inside Start()
        // Ignore modSettingsProxy warning. IT IS USED
        private KRSettings _settings;
        public ModSettingsProxy modSettingsProxy;
        
        /// <summary>
        /// Path to save files at. Contains "/Renders/"
        /// </summary>
        private string _savePath = "";

        public GameObject cameraGameObject;
        public Camera camera;

        #region Initial Start
        public void Start()
        {
            helper.Log("START");
            inst = this;
            var config = new InteractiveConfiguration<KRSettings>();
            _settings = config.Settings;
            
            cameraGameObject = new GameObject("KRCameraGameObject");
            camera = cameraGameObject.AddComponent<Camera>();
            camera.enabled = false;
            camera.CopyFrom(Camera.main);

            LogModSettings();
            
            helper.Log("Creating Render files. SteamApps");
            helper.Log(CreateRendersDirectory(helper.modPath).ToString());
            
            ModSettingsBootstrapper.Register(config.ModConfig, (proxy, saved) =>
            {
                config.Install(proxy, saved);
                OnModRegistered(proxy, saved);
            }, (ex) =>
            {
                Debugging.Log("KingdomRenderer", $"Failed to register mod: {ex.Message}");
                Debugging.Log("KingdomRenderer", ex.StackTrace);
            });
        }

        private void OnModRegistered(ModSettingsProxy proxy, SettingsEntry[] saved)
        {
            try
            {
                modSettingsProxy = proxy;
                if (!proxy) 
                {
                    Debugging.Log("KingdomRenderer", "Failed to register modSettingsProxy");
                    return;
                }
                
                // Change the text in the settings menu when the options are changed
                // Change the update interval label to be the new value
                _settings.AutoRend.RendPer10Year.OnUpdate.AddListener(
                    (kingdomRendererSettings) =>
                    {
                        _settings.AutoRend.RendPer10Year.Label = 
                            _settings.AutoRend.RendPer10Year.Value.ToString(CultureInfo.InstalledUICulture);
                    });
                // Change the render height interval to be the new value
                _settings.RendSettings.RendHeight.OnUpdate.AddListener(
                    (kingdomRendererSettings) =>
                    {
                        _settings.RendSettings.RendHeight.Value =
                            (float) Math.Round(_settings.RendSettings.RendHeight.Value, 1);
                        _settings.RendSettings.RendHeight.Label =
                            _settings.RendSettings.RendHeight.Value.ToString(CultureInfo.InstalledUICulture);
                    });
                // Change the render resolution scaling label to be the new value
                _settings.RendSettings.ResScaling.OnUpdate.AddListener(
                    (kingdomRendererSettings) =>
                    {
                        _settings.RendSettings.ResScaling.Value =
                            (float) Math.Round(_settings.RendSettings.ResScaling.Value, 1);
                        _settings.RendSettings.ResScaling.Label =
                            _settings.RendSettings.ResScaling.Value.ToString(CultureInfo.InstalledUICulture);
                    });

                // Not int so that it just maps to the same location
                // No calculations are done here just a mapping of names
                _renderWidth =
                    _settings.RendSettings.ResolutionX.Options[
                        _settings.RendSettings.ResolutionX.Value];
                _renderHeight = _settings.RendSettings.ResolutionY.Options[
                    _settings.RendSettings.ResolutionY.Value];
                
                
                Debugging.Log("KingdomRenderer", "Finished registering KingdomRenderer");
                helper.Log("OnModRegistered");
                LogModSettings();
            }
            catch (Exception ex)
            {
                Debugging.Log("KingdomRenderer", $"OnRegisterMod failed: {ex.Message}");
                Debugging.Log("KingdomRenderer", ex.StackTrace);
            }
            helper.Log("Mod registered with ModMenu");
        }

        /// <summary>
        /// Runs before scene loads
        /// Sets up modHelper and patches Harmony
        /// </summary>
        /// <param name="helper">The helper injected by KaC upon compilation</param>
        // ReSharper disable once ParameterHidesMember
        public void Preload(KCModHelper helper)
        {
            // Set up mod helper
            this.helper = helper;
            helper.Log("Loaded KingdomRenderer");
            
            // Load harmony
            var harmony = HarmonyInstance.Create("KingdomRenderer");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }
        
        /// <summary>
        /// Runs after the scene has loaded
        /// </summary>
        /// <param name="helper">The helper injected by KaC upon compilation</param>
        // ReSharper disable once ParameterHidesMember
        public void SceneLoaded(KCModHelper helper)
        {
            helper.Log("Scene Loaded");
            helper.Log($"renderPerYear loaded: {PlayerPrefs.HasKey($"rendersDoneThisYear {World.inst.name}")}");

            LogModSettings();
        }

        #endregion
        
        /// <summary>
        /// Logs all settings to settings menu
        /// </summary>
        public void LogModSettings()
        {
            helper.Log("---CURRENT SETTINGS---");
            try
            {
                helper.Log($"Enabled: {_settings.RendSettings.Enabled.Value}");
                helper.Log($"Rendering height: {_settings.RendSettings.RendHeight.Value}");
                helper.Log($"Rendering res X: {_renderWidth}");
                helper.Log($"Rendering res Y: {_renderHeight}");
                helper.Log("");
                helper.Log($"Auto enabled: {_settings.AutoRend.Enabled.Value}");
                helper.Log($"Auto interval (Per 10 years): {_settings.AutoRend.RendPer10Year.Value}");
                helper.Log("");
                helper.Log($"Manual enabled: {_settings.ManRend.Enabled.Value}");
                helper.Log($"Manual key: {_settings.ManRend.RenderKey.Key}");
                helper.Log("");
                helper.Log($"Filetype val: {_settings.RendSettings.FileType.Options[_settings.RendSettings.FileType.Value]}");
                helper.Log($"Filetype: {GetFileExtension()}");
            }
            catch (Exception)
            {
                helper.Log("Settings not yet loaded");
            }

            helper.Log("----------------------");
        }
        
        /// <summary>
        /// Runs every frame
        /// Checks if it should automatically render
        /// Checks if the manual render button has been pressed
        /// </summary>
        public void Update()
        {
            // If enabled and not on the menu/world creation screen
            if (!_settings.RendSettings.Enabled.Value) return;
            
            // Manual _rendering
            if (Input.GetKeyDown(_settings.ManRend.RenderKey.Key)
                && _settings.ManRend.Enabled.Value
                && GameState.inst.CurrMode == GameState.inst.playingMode)
            {
                helper.Log("Manual render");
                try
                {
                    SaveRender(RenderWorld(
                        int.Parse(_settings.RendSettings.ResolutionX.Options[
                            _settings.RendSettings.ResolutionX.Value]),
                        int.Parse(_settings.RendSettings.ResolutionY.Options[
                            _settings.RendSettings.ResolutionY.Value]),
                        _settings.RendSettings.RendHeight.Value), CreateSaveName(true));
                }
                catch (Exception e)
                {
                    helper.Log(e.ToString());
                }
            }

            // If Auto Enabled and in game (not menu)
            if (!_settings.AutoRend.Enabled.Value ||
                GameState.inst.CurrMode != GameState.inst.playingMode) return;
            
            // If all _rendering is done then re enable clouds and stoneUI
            if (finishedRendering)
            {
                helper.Log("Finished _rendering");
                _shouldRender = false;
                World.inst.GenerateStoneUIs();
                Settings.inst.ShowClouds = _cloudSetting;
                Settings.inst.SaveSettings();
            }
                    
            if (_shouldRender)
            {
                _shouldRender = false;
                helper.Log($"Rendering picture at time {Weather.inst.GetYearProgress()} in year: {Player.inst.CurrYear}");
                helper.Log("----Rendering NOW----");
                        
                SaveRender(RenderWorld(int.Parse(_settings.RendSettings.ResolutionX.Options[
                        _settings.RendSettings.ResolutionX.Value]),
                    int.Parse(_settings.RendSettings.ResolutionY.Options[
                        _settings.RendSettings.ResolutionY.Value]),
                    _settings.RendSettings.RendHeight.Value), CreateSaveName());
                helper.Log("----RENDERING DONE----");
                helper.Log($"Render should be saved as:\n" +
                           $"{CreateSaveName()}.{GetFileExtension()}\n" +
                           $"at" +
                           $"\n{_savePath}");
                        
                // Re enable the clouds
                Settings.inst.ShowClouds = _cloudSetting;
            }
            
            // Example would be:
            // 0.8 % (1/20) = 0.8 % (0.05) = 16 = true
            // With wiggle room for loss of precision because of floats
            if (_shouldRender || !IsFactorFloat(Weather.inst.GetYearProgress(),
                1 / (_settings.AutoRend.RendPer10Year.Value / 10))) return;
            
            helper.Log("Should render NEXT frame");
            //Tell it to render the pic next "update" so that way the clouds disappear for one update cycle to take the render
            _cloudSetting = Settings.inst.ShowClouds;
            Settings.inst.ShowClouds = false;
            Settings.inst.SaveSettings();
            _shouldRender = true;
        }
        
        private int GetWhichRenderThisYear()
        {
            return (int) Math.Round(Weather.inst.GetYearProgress() /
                              (1 / (_settings.AutoRend.RendPer10Year.Value / 10)));
        }

        private static bool IsFactorFloat(float large, float small)
        {
            // helper.Log("large%small=" + large % small + "ACTIVE");
            // helper.Log("large/small=" + large / small);
            return large % small <= 0.001f;
        }
        
        /// <summary>
        /// Render the world
        /// </summary>
        /// <param name="width">Width of the Texture2D to be returned</param>
        /// <param name="height">Height of the Texture2D to be returned</param>
        /// <param name="renderHeightLocal">How far above the map to take the render from</param>
        /// <returns>Render of the entire world</returns>
        private Texture2D RenderWorld(int width, int height, float renderHeightLocal)
        {
            helper.Log("RenderWorld Start");
            Texture2D result = WorldExtender.KingdomRenderer_CaptureWorldShot(width, height, renderHeightLocal);
            helper.Log("RenderWorld End");
            return result;
        }

        /// <summary>
        /// Saves a texture2D
        /// </summary>
        /// <param name="texture2D">The texture to be saved</param>
        /// <param name="filename">Filename not including extension or path</param>
        public void SaveRender(Texture2D texture2D, string filename)
        {
            // This exists so that other saving methods can be easily swapped out at a later date. eg Change gif to jpeg
            inst.helper.Log("SaveRender Start");
            try
            {
                if (_settings.RendSettings.FileType.Value == 0 ||
                    _settings.RendSettings.FileType.Value == 1) // png (SteamApps) || png (AppData)
                {
                    UpdateSavePath();
                    
                    string path = CreatePath(filename) + ".png";
                    helper.Log($"Saving as .png to: {path}");
                    
                    World.inst.SaveTexture(path, texture2D);
                }
            }
            catch (Exception e)
            {
                helper.Log(e.ToString());
            }

            inst.helper.Log("SaveRender End");
        }
        
        private string CreateSaveName(bool manual = false)
        {
            if (manual)
            {
                return
                    string.Join("-", new string[]
                    {TownNameUI.inst.townName,
                        Player.inst.CurrYear.ToString(),
                        "Manual",
                        DateTime.Now.Second.ToString()
                    }); 
            }
            return
                string.Join("-", new string[]
                {TownNameUI.inst.townName,
                    Player.inst.CurrYear.ToString(),
                    GetWhichRenderThisYear().ToString(),
                    DateTime.Now.Second.ToString() // This is just in case. Don't think it's needed
                });}

        private string CreatePath(string savename)
        {
            return _savePath + savename;
        }

        private string GetFileExtension()
        {
            return _settings.RendSettings.FileType.Options
                [_settings.RendSettings.FileType.Value].Split('(')[0].Trim();
        }
        
        /// <summary>
        /// Updates _savePath to make sure it is correct
        /// </summary>
        private void UpdateSavePath()
        {
            helper.Log("Updating save path...");
            string savePlace = _settings.RendSettings.FileType.Options
                    [_settings.RendSettings.FileType.Value]
                .Split('(')[1].Trim(')'); // Get the bit in brackets eg AppData
            if (savePlace.Equals("AppData"))
            {
                // C:\Users\USERNAME\AppData\LocalLow\LionShield\Kingdoms and Castles\
                _savePath = Application.persistentDataPath + SavePathAppData;
            }
            else
            {
                // ~\steamapps\common\Kingdoms and Castles\KingdomsAndCastles_Data\mods\KingdomRenderer\Renders
                _savePath = helper.modPath + SavePathSteamApps; 
            }
        }
        
        /// <summary>
        /// Attempt to create a "Renders" directory at the specified path
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private bool CreateRendersDirectory(string path)
        {
            try
            {
                string command = $"cd {path} & mkdir Renders";
                System.Diagnostics.Process.Start("CMD.exe", $"/C {command}"); // /C hide cmd, /K show
                _savePath = path + "/Renders/";
                helper.Log($"Directory (now if it didn't before) exists at: {_savePath}");
                helper.Log($"Command used: {command}");
                return true;
            }
            catch (Exception e)
            {
                helper.Log(e.ToString());
                return false;
            }
        }
    }

    public class WorldExtender : World
    {
        // Most code copied from World.CaptureWorldShot() then tweaked in order to
        // Remove cloud and change camera angle to be more above the map
        // Original code is commented out with new code below
        // Renamed to: "Func_CaptureWorldShot"
        public static Texture2D KingdomRenderer_CaptureWorldShot(int width, int height, float renderHeight)
        {
            try
            {
                KingdomRenderer.inst.helper.Log("CaptureWorldShot Start");
                Camera camera = KingdomRenderer.inst.camera;
                KingdomRenderer.inst.helper.Log($"Cameras: {Camera.allCamerasCount}");

                if (camera == null)
                {
                    return new Texture2D(0, 0);
                }
                
                float num = (inst.GridHeight >= inst.GridWidth ? inst.GridHeight : inst.GridWidth) / 2f;
                
                var cameraMainTransform = camera.transform;
                cameraMainTransform.position =  new Vector3(inst.GridWidth / 2f,
                                                            num * renderHeight,
                                                            inst.GridHeight / 2f);
                cameraMainTransform.LookAt(new Vector3(inst.GridHeight / 2f, 0f, inst.GridWidth / 2f));
                cameraMainTransform.Rotate(new Vector3(0,0,270), Space.Self);


                if (camera.GetComponent<GlobalFog>() != null)
                {
                    camera.GetComponent<GlobalFog>().enabled = false;
                }

                RenderTexture originalRenderTexure = RenderTexture.active;
                RenderTexture renderTexture = new RenderTexture(width, height, 32);
                
                RenderTexture.active = renderTexture;
                camera.Render();
                RenderTexture.active = originalRenderTexure;

                Texture2D result = new Texture2D(width, height);
                result.ReadPixels(new Rect(0, 0, width, height), 0, 0);
                result.Apply();
                
                if (camera.GetComponent<GlobalFog>() != null)
                {
                    camera.GetComponent<GlobalFog>().enabled = true;
                }

                KingdomRenderer.inst.finishedRendering = true; //TODO why is this commented out????
                KingdomRenderer.inst.helper.Log("CaptureWorldShot End");
                return result;
            }
            catch (Exception e)
            {
                KingdomRenderer.inst.helper.Log(e.ToString());
                return new Texture2D(width, height);
            }
        }
    }
    
    
    #region ModSettings
    [Mod("KingdomRenderer", "v1.2", "ArchieV / greenking2000")]
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local")]
    public class KRSettings
    {
        [Category("Render Settings")]
        public RenderingSetting RendSettings { get; private set; }

        [Category("Automatic Rendering")]
        public AutomaticRendering AutoRend { get; private set; }
        
        [Category("Manual Rendering")]
        public ManualRendering ManRend { get; private set; }
    }

    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local")]
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
    public class RenderingSetting
    {
        [Setting("Enabled", "Whether or not to enable rendering")]
        [Toggle(true, "Enabled")]
        public InteractiveToggleSetting Enabled { get; private set; }
        
        [Setting("Rending Height", "How far above the map the render is taken from")]
        [Slider(2f, 15f, 4.5f, "4.5")]
        public InteractiveSliderSetting RendHeight { get; private set; }
        
        [Setting("Render Resolution Scaling Factor", "The scaling factor of the render where 1.0 is 1920x1080\n" +
                                                     "Larger number will impact performance while rendering but look better")]
        [Slider(0.1f, 16f, 1f, "1")]
        public InteractiveSliderSetting ResScaling { get; private set; }

        [Setting("Render Resolution X", "Larger resolutions will cause the rendering process to take longer")]
        [Select(6,  new []{"160", "320", "480", "512", "720", "1280", "1920", "3840", "4096", "7680", "15360"})]
        public InteractiveSelectSetting ResolutionX { get; private set; }
        
        [Setting("Render Resolution Y", "Larger resolutions will cause the rendering process to take longer")]
        [Select(6,  new []{"120", "240", "272", "342", "480", "960", "1080", "2160", "3072", "4320", "8640"})]
        public InteractiveSelectSetting ResolutionY { get; private set; }
        
        [Setting("Render Filetype", "Which image type do you want images to be saved as?")]
        [Select(0,  new []{"png (SteamApps)", "png (AppData)"})]
        public InteractiveSelectSetting FileType { get; private set; }
    }
    
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local")]
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
    public class ManualRendering
    {
        [Setting("Manual Rending Enabled", "Whether or not to enable manual rendering (Requires rendering to be enabled)")]
        [Toggle(true, "Enabled")]
        public InteractiveToggleSetting Enabled { get; private set; }
        
        [Setting("Manual Render Hotkey", "Which button to press to manually render the map")]
        [Hotkey(KeyCode.N)]
        public InteractiveHotkeySetting RenderKey { get; private set; }
    }

    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local")]
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
    public class AutomaticRendering
    {
        [Setting("Automatic Rending Enabled", "Whether or not to enable automatic rendering (Requires rendering to be enabled)")]
        [Toggle(true, "Enabled")]
        public InteractiveToggleSetting Enabled { get; private set; }
        
        [Setting("Number of renders per 10 years", "Number of renderings per 10 in-game year")]
        [Slider(1, 150, 30, "30", true)]
        public InteractiveSliderSetting RendPer10Year { get; private set; }
        
    }
    #endregion
}
