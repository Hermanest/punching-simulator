using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

[RequireComponent(typeof(Rigidbody))]
internal class Grabber : MonoBehaviour {
    [SerializeField]
    private InputDeviceCharacteristics _characteristic;

    private Rigidbody _rigidbody;
    private readonly InputFeatureUsage<bool> _triggerFeature = CommonUsages.triggerButton;
    private InputDevice _device;

    private readonly HashSet<Grabbable> _colliding = new();
    private bool _lastPressState;
    private Grabbable _grabbed;

    private void Awake() {
        _rigidbody = GetComponent<Rigidbody>();
        InputDevices.deviceConnected += HandleDeviceAdded;
    }

    private void OnCollisionEnter(Collision other) {
        if (other.gameObject.GetComponent<Grabbable>() is { } grabbable) {
            _colliding.Add(grabbable);
        }
    }

    private void OnCollisionExit(Collision other) {
        if (other.gameObject.GetComponent<Grabbable>() is { } grabbable) {
            _colliding.Remove(grabbable);
        }
    }

    private void Update() {
        if (!_device.TryGetFeatureValue(_triggerFeature, out var pressed)) {
            return;
        }

        if (pressed != _lastPressState) {
            if (pressed) {
                Grab();
            } else {
                Release();
            }

            _lastPressState = pressed;
        }
    }

    private void Grab() {
        if (_colliding.Count == 0) return;

        // Find closest from colliding objects
        Grabbable closest = null;
        var pos = transform.position;
        var minDist = float.MaxValue;

        foreach (var g in _colliding) {
            var dist = Vector3.Distance(pos, g.transform.position);
            
            if (dist < minDist) {
                minDist = dist;
                closest = g;
            }
        }

        closest!.BindTo(_rigidbody);
        _grabbed = closest;
    }

    private void Release() {
        if (_grabbed == null) {
            return;
        }

        _grabbed.Unbind();
        _grabbed = null;
    }

    private void HandleDeviceAdded(InputDevice device) {
        if (device.characteristics.HasFlag(_characteristic)) {
            _device = device;
            InputDevices.deviceConnected -= HandleDeviceAdded;
        }
    }
}