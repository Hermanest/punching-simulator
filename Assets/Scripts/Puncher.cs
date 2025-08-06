using UnityEngine;

internal class Puncher : MonoBehaviour {
    public float Radius => _radius;

    [SerializeField] private float _radius;
    [SerializeField] private BloodEffect _effect;

    private Vector3 _lastPosition;
    private Vector3 _velocity;

    public void NotifyPunchHandled(in PunchResult punch) {
        if (punch.intensity is not PunchIntensity.Powerful) {
            return;
        }
        
        _effect.Play(punch.contact, punch.velocity);
    }

    private void Update() {
        _velocity = (transform.position - _lastPosition) / Time.deltaTime;
        _lastPosition = transform.position;
    }

    private void OnCollisionEnter(Collision other) {
        if (other.gameObject.GetComponent<PunchingBag>() is { } bag) {
            var time = Time.time;
            var contact = other.contacts[0].point;
            var localContact = other.transform.InverseTransformPoint(contact);

            var punch = new EarlyPunchResult(time, this, bag, _velocity, contact, localContact);
            PunchProcessor.Instance.QueuePunch(punch);

            Debug.Log($"VELOCITY:{_velocity.magnitude}");
        }
    }
}