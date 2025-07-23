using UnityEngine;

namespace KillCam.Client
{
    public class Client_CharacterView : RoleView
    {
        private Mono_Role role;

        protected override void OnActivate()
        {
            var cState = Owner.GetDataReadOnly<CharacterStateData>();
            Load(cState.Pos, cState.Rot);
        }

        protected override void OnDeactivate()
        {
            Unload();
        }

        protected override void OnTickActive()
        {
            var cState = Owner.GetDataReadOnly<CharacterStateData>();
            role.transform.position = cState.Pos;
            role.transform.rotation = cState.Rot;
        }

        private void Load(Vector3 pos, Quaternion rot)
        {
            var asset = Resources.Load<Mono_Role>("Client_Mono_Role");
            var instance = Object.Instantiate(asset);
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