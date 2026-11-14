#if UNITY_EDITOR
using System;
using System.IO;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace DuckAdventure.Agent.Editor
{
    /// <summary>Full HUD capture for the remote Metal Editor, using a stored depth surface.</summary>
    public static class MarshCapture
    {
        public static string Capture(string fileName)
        {
            if(!Application.isPlaying)throw new InvalidOperationException("Checkpoint captures require Play Mode.");
            if(fileName.IndexOfAny(Path.GetInvalidFileNameChars())>=0 || fileName.Contains("/") || fileName.Contains("\\"))
                throw new ArgumentException("Use a simple capture filename.");
            Camera camera=Camera.main;
            if(camera==null)throw new InvalidOperationException("Main camera is missing.");
            Canvas[] canvases=Object.FindObjectsByType<Canvas>(FindObjectsSortMode.None);
            var modes=canvases.Select(c=>c.renderMode).ToArray();
            var cameras=canvases.Select(c=>c.worldCamera).ToArray();
            var distances=canvases.Select(c=>c.planeDistance).ToArray();
            var orders=canvases.Select(c=>c.sortingOrder).ToArray();
            var previous=camera.targetTexture;
            var previousActive=RenderTexture.active;
            var descriptor=new RenderTextureDescriptor(1280,720,RenderTextureFormat.ARGB32,24)
            {msaaSamples=1,memoryless=RenderTextureMemoryless.None,sRGB=true};
            var target=new RenderTexture(descriptor);
            target.Create();
            Texture2D texture=new Texture2D(1280,720,TextureFormat.RGBA32,false);
            try
            {
                camera.targetTexture=target;
                for(int i=0;i<canvases.Length;i++)
                {
                    canvases[i].renderMode=RenderMode.ScreenSpaceCamera;
                    canvases[i].worldCamera=camera;
                    canvases[i].planeDistance=1;
                    canvases[i].sortingOrder=2000+orders[i];
                }
                Canvas.ForceUpdateCanvases();camera.Render();RenderTexture.active=target;
                texture.ReadPixels(new Rect(0,0,1280,720),0,0);texture.Apply();
                byte[] png=texture.EncodeToPNG();
                Directory.CreateDirectory("Temp/MarshlightVerification");
                File.WriteAllBytes("Temp/MarshlightVerification/"+fileName,png);
                return Convert.ToBase64String(png);
            }
            finally
            {
                camera.targetTexture=previous;RenderTexture.active=previousActive;
                for(int i=0;i<canvases.Length;i++)
                {
                    canvases[i].renderMode=modes[i];canvases[i].worldCamera=cameras[i];
                    canvases[i].planeDistance=distances[i];canvases[i].sortingOrder=orders[i];
                }
                target.Release();Object.DestroyImmediate(target);Object.DestroyImmediate(texture);
                Canvas.ForceUpdateCanvases();
            }
        }
    }
}
#endif
