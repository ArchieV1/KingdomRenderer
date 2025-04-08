using System;
using System.Reflection;
using Assets;
using UnityEngine;
using Harmony;
using KingdomRenderer.Shared.ArchieV1.Extensions;
using KingdomRenderer.Shared.Zat;
using KingdomRenderer.Shared.ArchieV1.Extensions.ModMenuApiExtensions;
using KingdomRenderer.Shared.ArchieV1.System.IO;
using static KingdomRenderer.FileHelper;

namespace KingdomRenderer
{
    public class KingdomRenderer : MonoBehaviour
    {
        // Instances
        public KCModHelper Helper;
        public static KingdomRenderer Inst;
        
        private string _renderWidth = "";
        private string _renderHeight = "";
        private static bool _shouldRender;
        private static bool _cloudSetting;
        public bool finishedRendering;

        // Settings to be init inside Start()
        // Ignore modSettingsProxy warning. IT IS USED
        private KrSettings.KrSettings _settings;
        public ModSettingsProxy modSettingsProxy;
        
        public GameObject cameraGameObject;
        public Camera camera;

        #region Initial Start
        public void Start()
        {
            Helper.Log("START");
            Inst = this;
            var config = new InteractiveConfiguration<KrSettings.KrSettings>();
            _settings = config.Settings;
            
            cameraGameObject = new GameObject("KRCameraGameObject");
            camera = cameraGameObject.AddComponent<Camera>();
            camera.enabled = false;
            camera.CopyFrom(Camera.main);

            LogModSettings();
            
            // Create all of the /KingdomRenderer/ directories
            Helper.Log("Creating Render directory in all allowed FileLocations:");
            CreateInitialRendersDirectories();
            
            Helper.Log($"Renders folder now exists at:");
            Helper.LogMultiLine(ListAllRendersDirectories(Helper));
            
            
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
                        _settings.AutoRend.RendPer10Year.SetLabelValue();
                    });
                
                // Change the render height interval to be the new value
                _settings.RendSettings.RendHeight.OnUpdate.AddListener(
                    (kingdomRendererSettings) =>
                    {
                        _settings.RendSettings.RendHeight.RoundValue();
                        _settings.RendSettings.RendHeight.SetLabelValue();
                    });
                
                // Change the render resolution scaling label to be the new value
                _settings.RendSettings.ResScaling.OnUpdate.AddListener(
                    (kingdomRendererSettings) =>
                    {
                        _settings.RendSettings.ResScaling.RoundValue();
                        _settings.RendSettings.ResScaling.SetLabelValue();
                    });

                // Not int so that it just maps to the same location
                // No calculations are done here just a mapping of names
                _renderWidth = _settings.RendSettings.ResolutionX.GetValue();
                _renderHeight = _settings.RendSettings.ResolutionY.GetValue();
                
                Debugging.Log("KingdomRenderer", "Finished registering KingdomRenderer");
                Helper.Log("OnModRegistered");
                LogModSettings();
            }
            catch (Exception ex)
            {
                Debugging.Log("KingdomRenderer", $"OnRegisterMod failed: {ex.Message}");
                Debugging.Log("KingdomRenderer", ex.StackTrace);
            }
            Helper.Log("Mod registered with ModMenu");
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
            this.Helper = helper;
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
            Helper.LogInLine("CURRENT SETTINGS");
            try
            {
                Helper.Log($"Enabled: {_settings.RendSettings.Enabled.Value}");
                Helper.Log($"Rendering height: {_settings.RendSettings.RendHeight.Value}");
                Helper.Log($"Rendering res X: {_renderWidth}");
                Helper.Log($"Rendering res Y: {_renderHeight}");
                Helper.Log();
                Helper.Log($"Auto enabled: {_settings.AutoRend.Enabled.Value}");
                Helper.Log($"Auto interval (Per 10 years): {_settings.AutoRend.RendPer10Year.Value}");
                Helper.Log();
                Helper.Log($"Manual enabled: {_settings.ManRend.Enabled.Value}");
                Helper.Log($"Manual key: {_settings.ManRend.RenderKey.Key}");
                Helper.Log();
                Helper.Log($"FileLocation: {_settings.RendSettings.FileLocation.GetValue()}");
            }
            catch (Exception)
            {
                Helper.Log("Settings not yet loaded");
            }

            Helper.LogLine();
        }
        
        /// <summary>
        /// Runs every frame
        /// Checks if it should automatically render
        /// Checks if the manual render button has been pressed
        /// </summary>
        public void Update()
        {
            // If enabled and not on the menu/world creation screen
            if (!_settings.RendSettings.Enabled.GetValue()) return;
            
            // Manual _rendering
            if (Input.GetKeyDown(_settings.ManRend.RenderKey.Key)
                && _settings.ManRend.Enabled.GetValue()
                && GameState.inst.CurrMode == GameState.inst.playingMode)
            {
                Helper.Log("Manual render");
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
                    Helper.Log(e.ToString());
                }
            }

            // If Auto Enabled and in game (not menu)
            if (!_settings.AutoRend.Enabled.Value || GameState.inst.CurrMode != GameState.inst.playingMode)
            {
                return;
            }
            
            // If all _rendering is done then re-enable clouds and stoneUI
            if (finishedRendering)
            {
                Helper.Log("Finished Rendering");
                _shouldRender = false;
                finishedRendering = false;
                
                World.inst.GenerateStoneUIs();
                Settings.inst.ShowClouds = _cloudSetting;
                Settings.inst.SaveSettings();
            }
                    
            if (_shouldRender)
            {
                _shouldRender = false;
                Helper.Log($"Rendering picture at time {Weather.inst.GetYearProgress()} in year: {Player.inst.CurrYear}");
                Helper.LogInLine("Rendering NOW");
                        
                SaveRender(
                    RenderWorld(
                        int.Parse(_settings.RendSettings.ResolutionX.GetValue()),
                        int.Parse(_settings.RendSettings.ResolutionY.GetValue()),
                    _settings.RendSettings.RendHeight.Value),
                    CreateSaveName());
                Helper.LogInLine("RENDERING DONE");
                Helper.Log($"Render should be saved as:\n" +
                           $"{CreateSaveName()}.png\n" +
                           $"at" +
                           $"\n{GetCurrentSavePath()}");
                
                // Re-enable the clouds
                Settings.inst.ShowClouds = _cloudSetting;
            }
            
            // Example would be:
            // 0.8 % (1/20) = 0.8 % (0.05) = 16 = true
            // With wiggle room for loss of precision because of floats
            if (_shouldRender || !Weather.inst.GetYearProgress()
                    .IsFactor(1 / (_settings.AutoRend.RendPer10Year.Value / 10)))
            {
                return;
            }
            
            Helper.Log("Should render NEXT frame");
            // Tell it to render the pic next "update" so that way the clouds disappear for one update cycle to take the render
            _cloudSetting = Settings.inst.ShowClouds;
            Settings.inst.ShowClouds = false;
            Settings.inst.SaveSettings();
            _shouldRender = true;
        }
        
        private int GetWhichRenderThisYear()
        {
            float rendersPerYear = _settings.AutoRend.RendPer10Year.GetValue() / 10;
            return (int) Math.Round(Weather.inst.GetYearProgress() * rendersPerYear);
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
            Helper.Log($"RenderWorld Start at {DateTime.UtcNow}");
            Texture2D result = WorldExtender.KingdomRenderer_CaptureWorldShot(width, height, renderHeightLocal);
            Helper.Log($"RenderWorld End at {DateTime.UtcNow}");
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
            Inst.Helper.Log("SaveRender Start");
            try
            {
                DirectoryExtension.TryCreate(Path.Join(GetCurrentSavePath(), TownNameUI.inst.townName));
                
                string path = CreateFilePath(Path.Join(TownNameUI.inst.townName, filename + ".png"));
                Helper.Log($"Saving to: {path}");
                
                World.inst.SaveTexture(path, texture2D);
            }
            catch (Exception e)
            {
                Helper.Log(e.ToString());
            }

            Inst.Helper.Log("SaveRender End");
        }

        /// <summary>
        /// Creates the filename without the file suffix
        /// </summary>
        /// <param name="manual"></param>
        /// <returns></returns>
        private string CreateSaveName(bool manual = false)
        {
            string renderType = manual ? "manual" : GetWhichRenderThisYear().ToString();
            return $"{TownNameUI.inst.townName}-{Player.inst.CurrYear}-{renderType}-{DateTime.Now.Second}-{DateTime.Now.Millisecond}";
        }

        /// <summary>
        /// Creates the final file path from the FileLocation and the given saveName
        /// </summary>
        /// <param name="saveName"></param>
        /// <returns></returns>
        private string CreateFilePath(string saveName)
        {
            return Path.Join(GetCurrentSavePath(), saveName);
        }
        
        /// <summary>
        /// Attempt to create a "Renders" directory at both possible locations
        /// </summary>
        /// <returns></returns>
        private void CreateInitialRendersDirectories()
        {
            foreach (FileLocation location in EnumExtension.GetEnumList<FileLocation>())
            {
                string savePath = GetSavePath(location);
                
                // This does not care if dir already exist so may as well run the entire command
                DirectoryExtension.TryCreate(savePath);
            }
        }
        
        private string GetCurrentSavePath(string kingdomName = "")
        {
            return FileHelper.GetSavePath(
                Helper,
                KrSettings.KrSettings.Inst.RendSettings.FileLocation.GetValueEnum<FileLocation>(),
                kingdomName);
        }

        private string GetSavePath(FileLocation location, string kingdomName = "")
        {
            return FileHelper.GetSavePath(
                Helper,
                location,
                kingdomName); 
        }
    }
}
