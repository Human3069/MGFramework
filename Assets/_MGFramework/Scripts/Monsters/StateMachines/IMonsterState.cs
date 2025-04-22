namespace MGFramework
{
    public interface IMonsterState
    {
        void Enter(MonsterContext context, MonsterData data);
        void Exit();
        void FixedTick(); // FixedUpdate에서 호출
        void SlowTick(); // 0.5초마다 호출
    }
}