using UnityEngine;

namespace KillCam.Server
{
    public class CharacterView : RoleView
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
            var asset = Resources.Load("Server_Mono_Role");
            var instance = (GameObject)Object.Instantiate(asset);
            role = instance.GetComponent<Mono_Role>();
            role.transform.position = pos;
            role.transform.rotation = rot;
        }

        private void Unload()
        {
            if (role)
            {
                Object.Destroy(role.gameObject);
            }
        }
    }
}