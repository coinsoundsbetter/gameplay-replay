namespace KillCam.Client.Replay {
    public interface IReplayPlayer {
        void SetData(byte[] data);
        void Play();
        void Pause();
        void Resume();
        void Stop();
    }
}