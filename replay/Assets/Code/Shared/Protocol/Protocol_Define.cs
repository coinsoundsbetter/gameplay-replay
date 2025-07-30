namespace KillCam {
    public enum NetworkMsg : ushort {
        C2S_SendInput,
        C2S_SendCameraData,

        S2C_WorldStateSnapshot,
        S2C_StartReplay,
    }
}