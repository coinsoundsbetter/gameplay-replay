using UnityEngine;

namespace KillCam {
    public class UCharacter : MonoBehaviour, IUCharacter {
        public Transform cameraTarget;
        public CharacterController cc;
        public Animator animator;

        public void SetPosition(Vector3 pos) => transform.position = pos;
        public void SetRotation(Quaternion rot) => transform.rotation = rot;
        public void Move(Vector3 motion) => cc.Move(motion);
        public void PlayAnimation(string animName) => animator.Play(animName);
    }

    public interface IUCharacter {
        public void SetPosition(Vector3 pos);
        public void SetRotation(Quaternion rot);
        public void Move(Vector3 motion);
        public void PlayAnimation(string animName);
    }
}