using UnityEngine;

internal class Grabbable : MonoBehaviour {
    private ConfigurableJoint _joint;

    private void Awake() {
        _joint = gameObject.AddComponent<ConfigurableJoint>();
        _joint.enablePreprocessing = false;
    }

    public void BindTo(Rigidbody rigid) {
        _joint.xMotion = ConfigurableJointMotion.Limited;
        _joint.yMotion = ConfigurableJointMotion.Limited;
        _joint.zMotion = ConfigurableJointMotion.Limited;

        var drive = new JointDrive {
            positionSpring = 500f,
            positionDamper = 50f,
            maximumForce = 1000f
        };

        _joint.xDrive = drive;
        _joint.yDrive = drive;
        _joint.zDrive = drive;
        _joint.angularXDrive = drive;
        _joint.angularYZDrive = drive;

        _joint.connectedBody = rigid;
    }

    public void Unbind() {
        _joint.xMotion = ConfigurableJointMotion.Free;
        _joint.yMotion = ConfigurableJointMotion.Free;
        _joint.zMotion = ConfigurableJointMotion.Free;

        var drive = new JointDrive();
        _joint.xDrive = drive;
        _joint.yDrive = drive;
        _joint.zDrive = drive;
        _joint.angularXDrive = drive;
        _joint.angularYZDrive = drive;

        _joint.connectedBody = null;
    }
}