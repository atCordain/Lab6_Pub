namespace Lab6_Pub
{
    public abstract class Agent
    {
        public abstract void Initialize();
        public abstract void Run();
        public abstract void Pause();
        public abstract void End();

        public abstract float SimulationSpeed { get; set; }
        public abstract bool IsActive { get; set; }

    }
}