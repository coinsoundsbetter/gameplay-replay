namespace KillCam.Server {
    public static class Extensions {

        public static void Rpc<T>(this BattleWorld world, T msg) where T : INetworkMsg {
            var server = world.GetFunction<NetworkServer>();
            server.Rpc(msg);
        }
    }
}