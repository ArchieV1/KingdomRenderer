using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using UnityEngine;
using Harmony;
using UnityEngine.UI;


namespace KingdomRenderer
{
    public class KingdomRenderer : MonoBehaviour
    {
        public KCModHelper helper;

        private bool rendering = false;

        private int renderWidth = 400;

        private int renderHeight = 300;

        private static TimelapseGif timelapseGif = new TimelapseGif();
        
        // Runs before scene loads
        public void Preload(KCModHelper helper)
        {
            // Set up mod helper
            this.helper = helper;
            helper.Log("Loaded KingdomRenderer");
            
            // Load harmony
            var harmony = HarmonyInstance.Create("KingdomRenderer");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }
        
        // Runs after scene loads
        public void SceneLoaded(KCModHelper helper)
        {
            helper.Log("Scene Loaded");
        }
        
        // Runs every frame
        public void Update()
        {
            // If not currently waiting to render AND a save has been loaded (No screen of home screen)
            if (!rendering && !TownNameUI.inst.townName.Equals(""))
            {
                helper.Log("Not rendering");
                StartCoroutine( RenderTimer());
            }
        }

        private IEnumerator RenderTimer()
        {
            rendering = true;
            helper.Log("pre yield");
            yield return new WaitForSecondsRealtime(60f);
            SaveRenderAsGif(RenderWorld(renderWidth, renderHeight));
            helper.Log("post yield and render");
            rendering = false;
        }

        /// <summary>
        /// Render the world
        /// </summary>
        /// <param name="width">Width of the Texture2D to be returned</param>
        /// <param name="height">Height of the Texture2D to be returned</param>
        /// <returns>Render of the entire world</returns>
        public static Texture2D RenderWorld(int width, int height)
        {
            return World.inst.CaptureWorldShot(width,height);   
        }

        /// <summary>
        /// Saves a gif of the given texture
        /// </summary>
        /// <param name="texture2D">The texture to be converted to a gif</param>
        public void SaveRenderAsGif(Texture2D texture2D)
        {
            // Full path will be: C:\Users\USER\AppData\LocalLow\LionShield\Kingdoms and Castles\Renders
            // USER MUST CREATE RENDERS FILE
            timelapseGif.gifPath = "/Renders/" + TownNameUI.inst.townName + DateTime.Now.Ticks + ".gif";
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
                    Color pixel = texture2D.GetPixel(j, i);
                    imageData.pixelData[num3] = (byte)(pixel.r * 255f);
                    num3++;
                    imageData.pixelData[num3] = (byte)(pixel.g * 255f);
                    num3++;
                    imageData.pixelData[num3] = (byte)(pixel.b * 255f);
                    num3++;
                }
            }
            
            //Add frame to gif and save gid
            timelapseGif.AddFrame(imageData);
            timelapseGif.CloseGif();

        }
    }
}