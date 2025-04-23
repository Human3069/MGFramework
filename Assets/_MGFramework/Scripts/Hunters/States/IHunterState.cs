namespace MGFramework
{
    public interface IHunterState
    {
        void Enter(HunterContext context, HunterData data);
        void Exit();
        void FixedTick(); // FixedUpdate에서 호출
        void SlowTick(); // 0.5초마다 호출
    }
}