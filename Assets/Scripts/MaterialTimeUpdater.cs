using UnityEngine;

internal class MaterialTimeUpdater : MonoBehaviour {
    private static readonly int timeProp = Shader.PropertyToID("_ActualTime");
    
    private void Update() {
        Shader.SetGlobalFloat(timeProp, Time.time);
    }
}