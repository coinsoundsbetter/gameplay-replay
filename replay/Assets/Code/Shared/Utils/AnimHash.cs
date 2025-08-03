using UnityEngine;

namespace KillCam {
    public static class AnimHash {

        public static int GetHash(string animKey) {
            return Animator.StringToHash(animKey);
        }
    }
}