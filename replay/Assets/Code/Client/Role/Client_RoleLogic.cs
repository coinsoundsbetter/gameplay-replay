namespace KillCam.Client
{
    public class Client_RoleLogic
    {
        private BattleWorld myWorld;
        
        public Client_RoleInput Input { get; private set; }
        public Client_RoleMovement Movement { get; private set; }
        public Client_RoleView View { get; private set; }
        
        public void Init(BattleWorld world)
        {
            myWorld = world;
            Input = new Client_RoleInput();
            Movement = new Client_RoleMovement(Input);
            View = new Client_RoleView();
        }

        public void Clear()
        {
            Input.Update();
            Movement.Update();
        }

        public void Update()
        {
            
        }
    }
}