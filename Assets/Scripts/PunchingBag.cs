using UnityEngine;

[RequireComponent(typeof(Renderer))]
internal class PunchingBag : MonoBehaviour {
    private static readonly int hitPositionsProp = Shader.PropertyToID("_HitPositions");
    private static readonly int hitTimesProp = Shader.PropertyToID("_HitTimes");
    private static readonly int hitCountProp = Shader.PropertyToID("_HitCount");
    private static readonly int timeProp = Shader.PropertyToID("_Time");

    private const int MaxHitCount = 20;

    [SerializeField] private Material _materialPrefab;
    [SerializeField] private float _punchPowerfulThreshold = 3.5f;
    [SerializeField] private float _punchMaxMultiplier = 1.2f;
    [SerializeField] private float _punchMinMultiplier = 0.7f;

    private Material _material;
    private readonly Vector4[] _punches = new Vector4[MaxHitCount];
    private readonly float[] _times = new float[MaxHitCount];
    private int _punchCount;

    private void Start() {
        _material = Instantiate(_materialPrefab);
        GetComponent<Renderer>().material = _material;
    }

    private void OnDestroy() {
        if (_material != null) {
            Destroy(_material);
        }
    }

    public void RegisterDoublePunch(in PunchResult first, in PunchResult second) {
        PunchSoundManager.Instance.PlayDouble();

        RegisterPunchInternal(first, 2f);
        RegisterPunchInternal(second, 2f);
    }

    public void RegisterPunch(in PunchResult punch) {
        PunchSoundManager.Instance.Play(punch.intensity);

        RegisterPunchInternal(punch);
    }

    private void RegisterPunchInternal(in PunchResult punch, float radiusMultiplier = 1f) {
        if (punch.intensity is not PunchIntensity.Powerful) {
            return;
        }

        var radius = punch.radius * (1 / transform.localScale.x) * radiusMultiplier;
        var pos = punch.localContact;

        // Shift array if at max capacity
        if (_punchCount == MaxHitCount) {
            for (var i = 1; i < MaxHitCount; i++) {
                _punches[i - 1] = _punches[i];
                _times[i - 1] = _times[i];
            }
            _punchCount--;
        }

        // Add new punch
        _punches[_punchCount] = new Vector4(pos.x, pos.y, pos.z, radius);
        _times[_punchCount] = Time.time;
        _punchCount++;

        RefreshMaterial();
    }

    private void RefreshMaterial() {
        _material.SetVectorArray(hitPositionsProp, _punches);
        _material.SetFloatArray(hitTimesProp, _times);
        _material.SetInt(hitCountProp, _punchCount);
    }
}