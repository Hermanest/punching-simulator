using UnityEngine;

internal class BloodEffect : MonoBehaviour {
    [SerializeField]
    private ParticleSystem _bloodPs;
    
    public void Play(Vector3 pos, Vector3 velocity) {
        var count = (int)velocity.magnitude * 5;

        transform.position = pos;
        transform.forward = -velocity.normalized;

        _bloodPs.Emit(count);
    }
}