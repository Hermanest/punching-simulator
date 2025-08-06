using UnityEngine;

internal readonly struct PunchResult {
    public readonly float radius;
    public readonly Vector3 velocity;
    public readonly Vector3 contact;
    public readonly Vector3 localContact;
    public readonly PunchIntensity intensity;

    public PunchResult(
        float radius,
        Vector3 velocity, 
        Vector3 contact,
        Vector3 localContact,
        PunchIntensity intensity
    ) {
        this.radius = radius;
        this.velocity = velocity;
        this.contact = contact;
        this.localContact = localContact;
        this.intensity = intensity;
    }
}