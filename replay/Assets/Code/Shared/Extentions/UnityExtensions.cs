using UnityEngine;

namespace KillCam
{
    public static class UnityExtensions
    {
        public static void SetLayerRecursively(this GameObject go, int layer)
        {
            if (go == null)
            {
                return;
            }
    
            go.layer = layer;
            foreach (Transform child in go.transform)
            {
                SetLayerRecursively(child.gameObject, layer);
            }
        }
    }
}