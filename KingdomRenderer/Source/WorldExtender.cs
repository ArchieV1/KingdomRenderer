using System;
using UnityEngine;
using UnityStandardAssets.ImageEffects;

namespace KingdomRenderer
{
    public class WorldExtender : World
    {
        // Most code copied from World.CaptureWorldShot() then tweaked in order to
        // Remove cloud and change camera angle to be more above the map
        public static Texture2D KingdomRenderer_CaptureWorldShot(int width, int height, float renderHeight)
        {
            try
            {
                KingdomRenderer.Inst.Helper.Log("CaptureWorldShot Start");
                Camera camera = KingdomRenderer.Inst.camera;

                if (camera == null)
                {
                    return new Texture2D(0, 0);
                }
                
                float num = (inst.GridHeight >= inst.GridWidth ? inst.GridHeight : inst.GridWidth) / 2f;
                
                Transform cameraMainTransform = camera.transform;
                cameraMainTransform.position =  new Vector3(inst.GridWidth / 2f,
                    num * renderHeight,
                    inst.GridHeight / 2f);
                cameraMainTransform.LookAt(new Vector3(inst.GridHeight / 2f, 0f, inst.GridWidth / 2f));
                cameraMainTransform.Rotate(new Vector3(0,0,270), Space.Self);

                if (camera.GetComponent<GlobalFog>() != null)
                {
                    camera.GetComponent<GlobalFog>().enabled = false;
                }

                RenderTexture originalRenderTexture = RenderTexture.active;
                RenderTexture renderTexture = new RenderTexture(width, height, 32);
                
                RenderTexture.active = renderTexture;
                camera.Render();
                RenderTexture.active = originalRenderTexture;

                Texture2D result = new Texture2D(width, height);
                result.ReadPixels(new Rect(0, 0, width, height), 0, 0);
                result.Apply();
                
                if (camera.GetComponent<GlobalFog>() != null)
                {
                    camera.GetComponent<GlobalFog>().enabled = true;
                }
                
                KingdomRenderer.Inst.Helper.Log("CaptureWorldShot End");
                return result;
            }
            catch (Exception e)
            {
                KingdomRenderer.Inst.Helper.Log(e.ToString());
                return new Texture2D(width, height);
            }
        }
    }
}