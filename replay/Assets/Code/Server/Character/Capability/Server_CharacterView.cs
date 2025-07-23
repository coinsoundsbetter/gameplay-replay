using UnityEngine;

namespace KillCam.Server
{
    public class Server_CharacterView : RoleView
    {
        private Mono_Role role;

        protected override void OnActivate()
        {
            var state = Owner.GetDataReadOnly<CharacterStateData>();
            Load(state.Pos, state.Rot);
        }

        protected override void OnDeactivate()
        {
            Unload();
        }

        protected override void OnTickActive()
        {
            var state = Owner.GetDataReadOnly<CharacterStateData>();
            role.transform.position = state.Pos;
        }

        private void Load(Vector3 pos, Quaternion rot)
        {
            var asset = Resources.Load<Mono_Role>("Server_Mono_Role");
            var instance = Object.Instantiate(asset);
            role = instance.GetComponent<Mono_Role>();
            role.transform.position = pos;
            role.transform.rotation = rot;
        }

        private void Unload()
        {
            if (role.gameObject)
            {
                Object.Destroy(role.gameObject);
            }
        }
    }
}