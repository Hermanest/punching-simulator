using System;
using System.Collections.Generic;
using UnityEngine;

internal class PunchProcessor : MonoBehaviour {
    #region Singleton

    public static PunchProcessor Instance {
        get {
            if (_instance == null) {
                throw new InvalidOperationException();
            }

            return _instance;
        }
    }

    private static PunchProcessor _instance;

    private void Awake() {
        _instance = this;
    }

    private void OnDestroy() {
        _instance = null;
    }

    #endregion

    #region Logic

    [SerializeField] private float _doublePunchThreshold = 0.05f;
    [SerializeField] private float _powerfulPunchThreshold = 3.5f;
    [SerializeField] private float _weakPunchThreshold = 2.5f;
    [SerializeField] private float _punchGap = 0.7f;

    // Scaling based on velocity
    [SerializeField] private float _punchMaxMultiplier = 1.2f;
    [SerializeField] private float _punchMinMultiplier = 0.7f;

    private readonly List<EarlyPunchResult> _pending = new();
    private float _lastHitTime;

    public void QueuePunch(EarlyPunchResult result) {
        var now = Time.time;

        if (now < _lastHitTime + _punchGap) {
            return;
        }

        for (var i = 0; i < _pending.Count; i++) {
            var res = _pending[i];

            if (res.puncher != result.puncher && res.bag == result.bag) {
                var punch1 = CalculatePunch(result);
                var punch2 = CalculatePunch(res);

                if (punch1 is { intensity: PunchIntensity.Powerful } && punch2 is { intensity: PunchIntensity.Powerful }) {
                    result.puncher.NotifyPunchHandled(punch1.Value);
                    result.puncher.NotifyPunchHandled(punch2.Value);

                    result.bag.RegisterDoublePunch(punch1.Value, punch2.Value);
                    _lastHitTime = now;
                }

                _pending.RemoveAt(i);
                return;
            }
        }

        _pending.Add(result);
    }

    private void Update() {
        var now = Time.time;

        for (var i = 0; i < _pending.Count; i++) {
            var result = _pending[i];

            // Threshold for double punch exceeded
            if (now >= result.time + _doublePunchThreshold) {
                var punch = CalculatePunch(result);

                if (punch != null) {
                    result.puncher.NotifyPunchHandled(punch.Value);
                    result.bag.RegisterPunch(punch.Value);

                    _lastHitTime = now;
                }

                _pending.RemoveAt(i);
                i--;
            }
        }
    }

    private PunchResult? CalculatePunch(in EarlyPunchResult result) {
        var vel = result.velocity.magnitude;
        var intensity = DetermineIntensity(vel);

        if (intensity == null) {
            return null;
        }

        var mul = Mathf.Clamp(vel / 5f, _punchMinMultiplier, _punchMaxMultiplier);
        var radius = result.puncher.Radius * mul;

        return new PunchResult(
            radius,
            result.velocity,
            result.contact,
            result.localContact,
            intensity.Value
        );
    }

    private PunchIntensity? DetermineIntensity(float velocity) {
        // Avg: slight punch ~2, powerful punch ~5.5
        if (velocity <= _weakPunchThreshold) {
            return null;
        }

        if (velocity <= _powerfulPunchThreshold) {
            return PunchIntensity.Weak;
        }

        return PunchIntensity.Powerful;
    }

    #endregion
}