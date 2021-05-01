using UnityEngine;

public class CustomGlowObj : MonoBehaviour {
    public Material glowMaterial;

    private void OnEnable() {
        CustomGlowSystem.Instance.Add(this);
    }

    private void Start() {
        CustomGlowSystem.Instance.Add(this);
    }

    private void OnDisable() {
        CustomGlowSystem.Instance.Remove(this);
    }
}
