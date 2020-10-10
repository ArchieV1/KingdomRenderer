using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Assets;
using UnityEngine;
using Harmony;
using UnityEngine.Events;
using UnityStandardAssets.ImageEffects;
using Valve.VR;
using Zat.Shared;
using Zat.Shared.InterModComm;
using Zat.Shared.ModMenu.API;
using Zat.Shared.ModMenu.Interactive;
using Zat.Shared.Reflection;
using Colour = UnityEngine.Color;


namespace KingdomRenderer
{
    public class KingdomRenderer : MonoBehaviour
    {
        // Instances
        public KCModHelper helper;
        private static TimelapseGif timelapseGif = new TimelapseGif();
        public static KingdomRenderer inst;
        
        private string renderWidth = "";
        private string renderHeight = "";
        
        // Vars for rendering
        static bool shouldRender = false;
        static bool cloudSetting;
        static bool stoneUI;
        private bool rendering = false;
        public int rendersDoneThisYear;
        
        // Settings to be init inside Start()
        // Ignore proxy warning. IT IS USED
        public KingdomRendererSettings kingdomRendererSettings;
        public ModSettingsProxy proxy;

        // So that it puts the clouds+stone popups back after the render has finished
        public bool finishedRendering = false;

        #region Initial Start
        public void Start()
        {
            var config = new InteractiveConfiguration<KingdomRendererSettings>();
            kingdomRendererSettings = config.Settings;
            helper.Log("START");
            LogModSettings(helper);
            ModSettingsBootstrapper.Register(config.ModConfig, (proxy, saved) =>
            {
                config.Install(proxy, saved);
                OnModRegistered(proxy, saved);
                // Assign settings
                // modEnabled = kingdomRendererSettings.Enabled;
                //
                // manualRenderingEnabled = kingdomRendererSettings.ManualRendering.ManualEnabled;
                // renderKey = kingdomRendererSettings.ManualRendering.RenderKey.Key;
                //
                // automaticRendingEnabled = kingdomRendererSettings.AutomaticRendering.AutomaticEnabled;
                // renderInterval = kingdomRendererSettings.AutomaticRendering.RendersPerYear;
                
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
                this.proxy = proxy;
                if (!proxy)
                {
                    Debugging.Log("KingdomRenderer", "Failed to register proxy");
                    return;
                }
                
                // Change the text in the settings menu when the options are changed
                // Change the update interval label to be the new value
                kingdomRendererSettings.AutomaticRendering.RendersPerYear.OnUpdate.AddListener(
                    (kingdomRendererSettings) =>
                    {
                        this.kingdomRendererSettings.AutomaticRendering.RendersPerYear.Label = 
                            this.kingdomRendererSettings.AutomaticRendering.RendersPerYear.Value.ToString();
                    });
                // Change the render height interval to be the new value
                kingdomRendererSettings.RenderingSetting.RenderingHeight.OnUpdate.AddListener(
                    (kingdomRendererSettings) =>
                    {
                        this.kingdomRendererSettings.RenderingSetting.RenderingHeight.Label =
                            this.kingdomRendererSettings.RenderingSetting.RenderingHeight.Value.ToString();
                    });
                // Change the render resolution scaling label to be the new value
                kingdomRendererSettings.RenderingSetting.ResolutionScaling.OnUpdate.AddListener(
                    (kingdomRendererSettings) =>
                    {
                        this.kingdomRendererSettings.RenderingSetting.ResolutionScaling.Label =
                            this.kingdomRendererSettings.RenderingSetting.ResolutionScaling.Value.ToString();
                    });

                // Not int so that if just maps to the same location
                // No calulations are done here just a mapping of names
                renderWidth =
                    kingdomRendererSettings.RenderingSetting.ResolutionX.Options[
                        kingdomRendererSettings.RenderingSetting.ResolutionX.Value];
                renderHeight = kingdomRendererSettings.RenderingSetting.ResolutionY.Options[
                    kingdomRendererSettings.RenderingSetting.ResolutionY.Value];
                
                
                Debugging.Log("KingdomRenderer", "Finished registering KingdomRenderer");
                helper.Log("OnModRegistered");
                LogModSettings(helper);
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
        public void SceneLoaded(KCModHelper helper)
        {
            helper.Log("Scene Loaded");
            helper.Log("renderPerYear loaded: " + PlayerPrefs.HasKey("rendersDoneThisYear" + World.inst.name));
            
            // if (AlmostEqual(GetRendersPerYear(), kingdomRendererSettings.AutomaticRendering.RendersPerYear.Value)
            //     || GetRendersPerYear() > kingdomRendererSettings.AutomaticRendering.RendersPerYear.Value)
            // {
            //     ResetRendersPerYear();
            // }
            
            LogModSettings(helper);
            // rendersDoneThisYear = GetRendersPerYear();

        }
        
        public void LogModSettings(KCModHelper helper)
        {
            helper.Log("---CURRENT SETTINGS---");
            try
            {
                helper.Log("Enabled: " + kingdomRendererSettings.RenderingSetting.Enabled.Value);
                helper.Log("Rendering height: " + kingdomRendererSettings.RenderingSetting.RenderingHeight.Value);
                helper.Log("Rendering res X: " + renderWidth);
                helper.Log("Rendering res Y: " + renderHeight + "\n");
                
                helper.Log("Auto enabled: " + kingdomRendererSettings.AutomaticRendering.AutomaticEnabled.Value);
                helper.Log("Auto interval: " + kingdomRendererSettings.AutomaticRendering.RendersPerYear.Value + "\n");
                
                helper.Log("Manual enabled: " + kingdomRendererSettings.ManualRendering.ManualEnabled.Value);
                helper.Log("Manual key:" + kingdomRendererSettings.ManualRendering.RenderKey.Key);
            }
            catch (Exception e)
            {
                helper.Log("Settings not yet loaded");
            }

            helper.Log("----------------------");
        }
        
        #endregion
        
        /// <summary>
        /// Runs every frame
        /// Checks if it should automatically render
        /// Checks if the manual render button has been pressed
        /// </summary>
        public void Update()
        {
            //helper.Log((GameState.inst.CurrMode == GameState.inst.playingMode).ToString());
            // If enabled and not on the menu/world creation screen
            if (kingdomRendererSettings.RenderingSetting.Enabled.Value)
            {
                // if (GameState.inst.CurrMode == GameState.inst.playingMode)
                // {
                //     helper.Log("TimeInYear: " + Weather.inst.GetYearProgress());
                // }
                
                // If not currently waiting to render AND a save has been loaded (No screen of home screen) 
                // AND auto rendering is enabled
                
                // Rendering with seconds\
                // if (kingdomRendererSettings.AutomaticRendering.AutomaticEnabled.Value
                //     && !rendering
                //     && GameState.inst.CurrMode == GameState.inst.playingMode)
                // {
                //     StartCoroutine(RenderTimer());
                // }

                // Manual rendering
                if (Input.GetKeyDown(kingdomRendererSettings.ManualRendering.RenderKey.Key)
                    && kingdomRendererSettings.ManualRendering.ManualEnabled.Value
                    && GameState.inst.CurrMode == GameState.inst.playingMode)
                {
                    helper.Log("Manual render");
                    // var texture2D = new Texture2D(100, 100);
                    // SaveRenderAsGif(texture2D, "blackSquare");
                    // SaveRenderAsGif((RenderWorld(int.Parse(renderWidth),int.Parse(renderHeight), 
                    //     kingdomRendererSettings.RenderingSetting.RenderingHeight.Value)), CreateSaveName(true));
                    SaveRender(RenderWorld(
                        int.Parse(kingdomRendererSettings.RenderingSetting.ResolutionX.Options[kingdomRendererSettings.RenderingSetting.ResolutionX.Value]),
                        int.Parse(kingdomRendererSettings.RenderingSetting.ResolutionY.Options[kingdomRendererSettings.RenderingSetting.ResolutionY.Value]), 
                        kingdomRendererSettings.RenderingSetting.RenderingHeight.Value), CreateSaveName(true));
                }

                // If Auto Enabled and in game (not menu)
                if (kingdomRendererSettings.AutomaticRendering.AutomaticEnabled.Value
                    && GameState.inst.CurrMode == GameState.inst.playingMode)
                {
                    // if (!rendering && AlmostEqual(Weather.inst.GetYearProgress(),
                    //         rendersDoneThisYear / kingdomRendererSettings.AutomaticRendering.RendersPerYear.Value,
                    //         0.0001f))
                    // {
                    //     // Tell it to render the pic next "update" so that way the clouds disappear for one update cycle to take the render
                    //     cloudSetting = Settings.inst.ShowClouds;
                    //     Settings.inst.ShowClouds = false;
                    //     shouldRender = true;
                    // }
                    
                    // If all rendering is done then re enable clouds and stoneUI
                    if (finishedRendering)
                    {
                        helper.Log("Finished rendering");
                        shouldRender = false;
                        World.inst.GenerateStoneUIs();
                        Settings.inst.ShowClouds = cloudSetting;
                        Settings.inst.SaveSettings();
                    }
                    
                    if (shouldRender)
                    {
                        shouldRender = false;
                        helper.Log("Rendering picture at time " + Weather.inst.GetYearProgress() + " in year: " + Player.inst.CurrYear);
                        helper.Log("Rendering NOW");
                        // SaveRender(RenderWorld(
                        //     int.Parse(kingdomRendererSettings.RenderingSetting.ResolutionX.Options[kingdomRendererSettings.RenderingSetting.ResolutionX.Value]),
                        //     int.Parse(kingdomRendererSettings.RenderingSetting.ResolutionY.Options[kingdomRendererSettings.RenderingSetting.ResolutionY.Value]),
                        //     kingdomRendererSettings.RenderingSetting.RenderingHeight.Value), CreateSaveName());
                        SaveRender(RenderWorld(500, 500, 5f), CreateSaveName());
                        helper.Log("RENDING DONE");
                        helper.Log("Render should be saved as:\n" +
                                   CreateSaveName() + ".gif");
                        //IncreaseRenderPerYear();
                        
                        
                        // Re enable the clouds
                        Settings.inst.ShowClouds = cloudSetting;
                    }
                    // At end of year. Reset rendersPerYear number
                    // May be changed to a harmony postfix to Player.OnNewYear in future
                    // if (AlmostEqual(GetRendersPerYear(), kingdomRendererSettings.AutomaticRendering.RendersPerYear.Value)
                    //     || GetRendersPerYear() > kingdomRendererSettings.AutomaticRendering.RendersPerYear.Value)
                    // {
                    //     ResetRendersPerYear();
                    // }
                    
                    
                    // Example would be:
                    // 0.8 % (1/20) = 0.8 % (0.05) = 16 = true
                    // With wiggle room for loss of precision because of floats
                    if (!shouldRender &&
                        IsFactorFloat(Weather.inst.GetYearProgress(),
                        1 / kingdomRendererSettings.AutomaticRendering.RendersPerYear.Value))
                    {
                        helper.Log("Should render NEXT frame");
                        //Tell it to render the pic next "update" so that way the clouds disappear for one update cycle to take the render
                        cloudSetting = Settings.inst.ShowClouds;
                        Settings.inst.ShowClouds = false;
                        Settings.inst.SaveSettings();
                        shouldRender = true;
                    }
                }
            }
        }

        public int GetWhichRenderThisYear()
        {
            return (int) Math.Round(Weather.inst.GetYearProgress() /
                              (1 / kingdomRendererSettings.AutomaticRendering.RendersPerYear.Value));
        }

        public static bool AlmostWholeNumber(float num, float precision = 0.0001f)
        {
            return num - Math.Round(num) < precision;
        }

        public bool IsFactorFloat(float large, float small)
        {
            //return Math.IEEERemainder(large, small) <= 0.0001f;
            helper.Log("large%small=" + large % small + "ACTIVE");
            helper.Log("large/small=" + large / small);
            return large % small <= 0.001f;
        }
        
        /// <summary>
        /// Renders the world every x seconds
        /// </summary>
        /// <returns></returns>
        private IEnumerator RenderTimer()
        {
            rendering = true;
            yield return new WaitForSecondsRealtime(kingdomRendererSettings.AutomaticRendering.RendersPerYear);
            //int width = (int) Math.Round((double) kingdomRendererSettings.RenderingSetting.ResolutionScaling * renderWidth);
            //int height = (int) Math.Round((double) kingdomRendererSettings.RenderingSetting.ResolutionScaling * renderHeight);
            //int width = kingdomRendererSettings.RenderingSetting.ResolutionX.Value;
            //int height = kingdomRendererSettings.RenderingSetting.ResolutionY.Value;
            //SaveRenderAsGif(RenderWorld(int.Parse(renderWidth), int.Parse(renderHeight), kingdomRendererSettings.RenderingSetting.RenderingHeight.Value));
            SaveRenderAsGif(RenderWorld(int.Parse(kingdomRendererSettings.RenderingSetting.ResolutionX.Options[kingdomRendererSettings.RenderingSetting.ResolutionX.Value]),
                int.Parse(kingdomRendererSettings.RenderingSetting.ResolutionY.Options[kingdomRendererSettings.RenderingSetting.ResolutionY.Value]), 
                kingdomRendererSettings.RenderingSetting.RenderingHeight.Value), CreateSaveName());
            rendering = false;
        }

        /// <summary>
        /// Render the world
        /// </summary>
        /// <param name="width">Width of the Texture2D to be returned</param>
        /// <param name="height">Height of the Texture2D to be returned</param>
        /// <param name="renderHeight_local">How far above the map to take the render from</param>
        /// <returns>Render of the entire world</returns>
        public Texture2D RenderWorld(int width, int height, float renderHeight_local)
        {
            inst.helper.Log("RenderWorld Start");
            Texture2D result = WorldExtender.KingdomRenderer_CaptureWorldShot(width, height, renderHeight_local);
            inst.helper.Log("RenderWorld End");
            return result;
        }

        /// <summary>
        /// Saves a texture2D
        /// </summary>
        /// <param name="texture2D">The texture to be saved</param>
        /// <param name="filename">Filename not including extension</param>
        public void SaveRender(Texture2D texture2D, string filename)
        {
            // This exists so that other saving methods can be easily swapped out. eg Change gif to jpeg
            inst.helper.Log("SaveRender Start");
            SaveRenderAsGif(texture2D, filename);
            inst.helper.Log("SaveRender End");
        }
        
        /// <summary>
        /// Saves a gif of the given texture
        /// </summary>
        /// <param name="texture2D">The texture to be converted to a gif</param>
        /// <param name="filename">Filename not including extension</param>
        public void SaveRenderAsGif(Texture2D texture2D, string filename)
        {
            try
            {
                KingdomRenderer.inst.helper.Log("SaveRenderAsGif Start");
                // Full path will be: C:\Users\USER\AppData\LocalLow\LionShield\Kingdoms and Castles\Renders
                // USER MUST CREATE RENDERS FILE
                timelapseGif.gifPath = "/Renders/" + filename + ".gif";
                helper.Log("Saving image to: " + timelapseGif.gifPath);
                //timelapseGif.gifPath = "/Renders/" + "test.gif";
                timelapseGif.Create(0);
  
                // Converts texture2D to imagedata
                ImageData imageData = new ImageData();

                imageData.width = texture2D.width;
                imageData.height = texture2D.height;
                imageData.pixelData = new byte[3 * imageData.width * imageData.height];
                int num3 = 0;
                for (int i = imageData.height - 1; i >= 0; i--)
                {
                    for (int j = 0; j < imageData.width; j++)
                    {
                        Colour pixel = texture2D.GetPixel(j, i);
                        imageData.pixelData[num3] = (byte) (pixel.r * 255f);
                        num3++;
                        imageData.pixelData[num3] = (byte) (pixel.g * 255f);
                        num3++;
                        imageData.pixelData[num3] = (byte) (pixel.b * 255f);
                        num3++;
                    }
                }

                //Add frame to gif and save gid
                timelapseGif.AddFrame(imageData);
                timelapseGif.CloseGif();
                KingdomRenderer.inst.helper.Log("SaveRenderAsGif End");
            }
            catch (Exception e)
            {
                helper.Log(e.ToString());
            }
        }

        /// <summary>
        /// Returns if two floats are equal within a given tolerance
        /// </summary>
        /// <param name="num1">Number 1</param>
        /// <param name="num2">Number 2</param>
        /// <param name="tolerance">The tolerance in which the two will be counted as "equal"</param>
        /// <returns></returns>
        public static bool AlmostEqual(float num1, float num2, float tolerance)
        {
            return Math.Abs(num1 - num2) <= tolerance;
        }
        
        /// <summary>
        /// Returns if a floating point and and int are equal
        /// </summary>
        /// <param name="num1"></param>
        /// <param name="num2"></param>
        /// <returns></returns>
        public static bool AlmostEqual(int num1, float num2)
        {
            return Math.Abs(num1 - num2) <= 0.001f;
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
    
        public static void SetCloudSettings(bool val)
        {
            cloudSetting = val;
        }

        public static void SetStoneUI(bool val)
        {
            stoneUI = val;
        }
    }

    public class WorldExtender : World
    {
        // Most code copied from World.CaptureWorldShot() then tweaked in order to
        // Remove cloud and change camera angle to be more above the map
        // Original code is commented out with new code below
        public static Texture2D KingdomRenderer_CaptureWorldShot(int width, int height, float renderHeight)
        {
            try
            {
                // if (Camera.main == null)
                // {
                //     return null;
                // }
                KingdomRenderer.inst.helper.Log("CaptureWorldShot Start");
                // KingdomRenderer.inst.finishedRendering = false; //
                // KingdomRenderer.SetCloudSettings(Settings.inst.ShowClouds); //
                // KingdomRenderer.SetStoneUI(World.inst
                //     .hasStoneUI); // This is a val stored in world for if stone UI or not
                // // Remove clouds and Stone
                // inst.DestroyStoneUIs(); //
                // Settings.inst.ShowClouds = false; //
                // Settings.inst.SaveSettings(); //

                bool fog = RenderSettings.fog;
                var cameraMainTransform = Camera.main.transform;
                Vector3 position = cameraMainTransform.position;
                Quaternion rotation = cameraMainTransform.rotation;
                RenderSettings.fog = false;
                float num = (float) ((World.inst.GridHeight >= inst.GridWidth) ? inst.GridHeight : inst.GridWidth) / 2f;
                //Camera.main.transform.position = new Vector3(-num / 2f, num, -num / 2f);
                cameraMainTransform.position =
                    new Vector3(inst.GridWidth / 2f, num * renderHeight, inst.GridHeight / 2f);
                //Camera.main.transform.LookAt(new Vector3((float)inst.GridWidth * 0.33f, 0f, (float)inst.GridHeight * 0.33f));
                cameraMainTransform.LookAt(new Vector3(inst.GridWidth / 2f, 0f, inst.GridHeight / 2f));
                if (Camera.main.GetComponent<GlobalFog>() != null)
                {
                    Camera.main.GetComponent<GlobalFog>().enabled = false;
                }

                Texture2D result = inst.CaptureCameraShot(width, height);
                if (Camera.main.GetComponent<GlobalFog>() != null)
                {
                    Camera.main.GetComponent<GlobalFog>().enabled = true;
                }

                RenderSettings.fog = fog;
                cameraMainTransform.position = position;
                cameraMainTransform.rotation = rotation;

                // KingdomRenderer.inst.finishedRendering = true; //
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

    // [HarmonyPatch(typeof(Player), "OnNewYear")]
    // public class newYearPatch_KingdomRenderer
    // {
    //     static void Postfix(ref string __result)
    //     {
    //         KingdomRenderer.inst.helper.Log("Patched");
    //         KingdomRenderer.inst.rendersDoneThisYear = 0;
    //     }
    // }
    
    
    
    #region ModSettings
    [Mod("KingdomRenderer", "v1.0", "ArchieV / greenking2000")]
    public class KingdomRendererSettings
    {
        [Category("Render Settings")]
        public RenderingSetting RenderingSetting { get; private set; }

        [Category("Automatic Rendering")]
        public AutomaticRendering AutomaticRendering { get; private set; }
        
        [Category("Manual Rendering")]
        public ManualRendering ManualRendering { get; private set; }
    }

    public class RenderingSetting
    {
        [Setting("Enabled", "Whether or not to enable rendering")]
        [Toggle(true, "Enabled")]
        public InteractiveToggleSetting Enabled { get; private set; }
        
        [Setting("Rending Height", "How far above the map the render is taken from")]
        [Slider(2f, 5f, 3.5f, "3.5", false)]
        public InteractiveSliderSetting RenderingHeight { get; private set; }
        
        [Setting("Render Resolution Scaling Factor", "The scale factor of the render where 1.0 is 1920x1080\n" +
                                                     "Larger number will impact performance while rendering but look better")]
        [Slider(0.1f, 4f, 1f, "1", false)]
        public InteractiveSliderSetting ResolutionScaling { get; private set; }

        [Setting("Render Resolution X", "Larger resolutions will cause the rendering process to take longer")]
        [Select(720,  new []{"160", "320", "480", "512", "720", "1280", "1920", "3840", "4096", "7680"})]
        public InteractiveSelectSetting ResolutionX { get; private set; }
        
        [Setting("Render Resolution Y", "Larger resolutions will cause the rendering process to take longer")]
        [Select(720,  new []{"120", "240", "272", "342", "480", "960", "1080", "2160", "3072", "4320 "})]
        public InteractiveSelectSetting ResolutionY { get; private set; }
    }
    public class ManualRendering
    {
        [Setting("Manual Rending Enabled", "Whether or not to enable manual rendering (Requires rendering to be enabled)")]
        [Toggle(true, "Enabled")]
        public InteractiveToggleSetting ManualEnabled { get; private set; }
        
        [Setting("Manual Render Hotkey", "Which button to press to manually render the map")]
        [Hotkey(KeyCode.N)]
        public InteractiveHotkeySetting RenderKey { get; private set; }
    }

    public class AutomaticRendering
    {
        [Setting("Automatic Rending Enabled", "Whether or not to enable automatic rendering (Requires rendering to be enabled)")]
        [Toggle(true, "Enabled")]
        public InteractiveToggleSetting AutomaticEnabled { get; private set; }
        
        [Setting("Number of renders per 10 years", "Number of renderings per 10 in-game year")]
        [Slider(1, 100, 30, "30", true)]
        public InteractiveSliderSetting RendersPerYear { get; private set; }
    }
    #endregion
}
