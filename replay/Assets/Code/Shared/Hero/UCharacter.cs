using UnityEngine;

namespace KillCam {
    
    public class UnityHero : MonoBehaviour, IUnityHero {
        public Transform cameraTarget;
        public CharacterController cc;
        public Animator animator;
        public void SetPosition(Vector3 pos) => transform.position = pos;
        public void SetRotation(Quaternion rot) => transform.rotation = rot;
        public Vector3 GetPosition() => transform.position;
        public Quaternion GetRotation() => transform.rotation;
        public void Move(Vector3 motion) => cc.Move(motion);
        public void SetAnimParam(string key, float value) => animator.SetFloat(key, value);
        public void SetAnimParam(string key, int value) => animator.SetInteger(key, value);
        public void SetAnimParam(string key, bool value) => animator.SetBool(key, value);
        public Transform GetCameraTarget() => cameraTarget;
    }

    public interface IUnityHero {
        void SetPosition(Vector3 pos);
        Vector3 GetPosition();
        void SetRotation(Quaternion rot);
        Quaternion GetRotation();
        void Move(Vector3 motion);
        void SetAnimParam(string key, float value);
        void SetAnimParam(string key, int value);
        void SetAnimParam(string key, bool value);
        Transform GetCameraTarget();
    }
}