using UnityEngine;

internal readonly struct EarlyPunchResult {
    public readonly float time;
    public readonly Puncher puncher;
    public readonly PunchingBag bag;
    public readonly Vector3 velocity;
    public readonly Vector3 contact;
    public readonly Vector3 localContact;

    public EarlyPunchResult(
        float time,
        Puncher puncher,
        PunchingBag bag,
        Vector3 velocity,
        Vector3 contact,
        Vector3 localContact
    ) {
        this.time = time;
        this.puncher = puncher;
        this.bag = bag;
        this.velocity = velocity;
        this.contact = contact;
        this.localContact = localContact;
    }
}