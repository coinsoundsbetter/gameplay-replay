using Arch.Buffer;
using Arch.Core;
using Core;

namespace KillCam
{
    public struct S_ConnectSystem : ISystem
    {
        public World ECS { get; set; }
        
        public void OnUpdate()
        {
            var cmd = new CommandBuffer();
            var desc = new QueryDescription().WithAny<RequestLoginGame>();
            ECS.Query(desc, (Entity e, ref RequestLoginGame game) =>
            {
                // 创建角色命令
                var entity = cmd.Create(new ComponentType[]
                {
                    typeof(PlayerSpawnTag),
                });
                cmd.Set(entity, new PlayerSpawnTag()
                {
                    PlayerId = 1,
                    PlayerName = "Player 1",
                });
            });
            cmd.Playback(ECS);
        }
    }
}