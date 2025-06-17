using Mix;
using Unity.Entities;
using UnityEngine;

public class PlayerView : MonoBehaviour, IPlayerView
{
    public Animator Anim => animator;
    public CharacterController Cc => cc;
    public Transform Trans => transform;
    
    [SerializeField] private CharacterController cc;
    [SerializeField] private Animator animator;
    
    public class Backer : Baker<PlayerView> 
    {
        public override void Bake(PlayerView authoring)
        {
            var ent = GetEntity(TransformUsageFlags.Dynamic);
            AddComponentObject(ent, this);
        }
    }
}
