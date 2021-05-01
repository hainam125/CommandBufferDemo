using UnityEngine;

public class PostProcess : MonoBehaviour {
    [SerializeField] private Material material;
    private Camera cam;

    private void Start() {
        cam = GetComponent<Camera>();
        cam.depthTextureMode = DepthTextureMode.Depth;
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination) {
        Graphics.Blit(source, destination, material);
    }
}