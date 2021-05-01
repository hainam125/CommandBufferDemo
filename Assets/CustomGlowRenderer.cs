using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class CustomGlowRenderer : MonoBehaviour {
    private CommandBuffer glowBuffer;
    private Dictionary<Camera, CommandBuffer> cameras = new Dictionary<Camera, CommandBuffer>();

    private void Cleanup() {
        foreach (var cam in cameras) {
            if (cam.Key)
                cam.Key.RemoveCommandBuffer(CameraEvent.BeforeLighting, cam.Value);
        }
        cameras.Clear();
    }

    public void OnDisable() {
        Cleanup();
    }

    public void OnEnable() {
        Cleanup();
    }

    public void OnWillRenderObject() {
        var render = gameObject.activeInHierarchy && enabled;
        if (!render) {
            Cleanup();
            return;
        }

        var cam = Camera.current;
        if (!cam) return;

        if (cameras.ContainsKey(cam)) return;

        // create new command buffer
        glowBuffer = new CommandBuffer();
        glowBuffer.name = "Glow map buffer";
        cameras[cam] = glowBuffer;

        var glowSystem = CustomGlowSystem.Instance;

        // create render texture for glow map
        int tempID = Shader.PropertyToID("_Temp1");
        glowBuffer.GetTemporaryRT(tempID, -1, -1, 24, FilterMode.Bilinear);
        glowBuffer.SetRenderTarget(tempID);
        glowBuffer.ClearRenderTarget(true, true, Color.black); // clear before drawing to it each frame!!

        // draw all glow objects to it
        foreach (CustomGlowObj o in glowSystem.glowObjects) {
            Renderer r = o.GetComponent<Renderer>();
            Material glowMat = o.glowMaterial;
            if (r && glowMat) glowBuffer.DrawRenderer(r, glowMat);
        }

        // set render texture as globally accessable 'glow map' texture
        glowBuffer.SetGlobalTexture("_GlowMap", tempID);

        // add this command buffer to the pipeline
        // NOTE: if the event doesn't occur, this command buffer will be excecuted
        cam.AddCommandBuffer(CameraEvent.AfterForwardOpaque, glowBuffer);
    }
}


public class CustomGlowSystem {
    private static CustomGlowSystem instance;
    public static CustomGlowSystem Instance {
        get {
            if (instance == null) instance = new CustomGlowSystem();
            return instance;
        }
    }

    public HashSet<CustomGlowObj> glowObjects = new HashSet<CustomGlowObj>();

    public void Add(CustomGlowObj o) {
        Remove(o);
        glowObjects.Add(o);
    }

    public void Remove(CustomGlowObj o) {
        glowObjects.Remove(o);
    }
}