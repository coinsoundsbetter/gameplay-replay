namespace KillCam {
    public enum NetworkMsg : ushort {
        C2S_SendInput,


        S2C_WorldStateSnapshot,
        S2C_StartReplay,
    }
}