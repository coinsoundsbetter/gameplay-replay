namespace KillCam.Client {
    public static class Extensions {
        
        public static void Send<T>(this BattleWorld world, T msg) where T : INetworkMsg {
            var client = world.GetFeature<NetworkClient>();
            client?.Send(msg);
        }
    }
}