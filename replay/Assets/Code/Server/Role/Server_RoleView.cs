using KillCam.Client;
using UnityEngine;

namespace KillCam.Server
{
    public class Server_RoleView : RoleView
    {
        private Mono_Role role;
        private Server_RoleMovement movement;

        public Server_RoleView(Server_RoleMovement movement)
        {
            this.movement = movement;
        }
        
        public void Load(Vector3 pos, Quaternion rot)
        {
            var asset = Resources.Load<Mono_Role>("Server_Mono_Role");
            var instance = Object.Instantiate(asset);
            role = instance.GetComponent<Mono_Role>();
            role.transform.position = pos;
            role.transform.rotation = rot;
        }

        public void Unload()
        {
            if (role.gameObject)
            {
                Object.Destroy(role.gameObject);
            }
        }

        public void Update(float delta)
        {
            role.transform.position = movement.Pos;
        }
    }
}