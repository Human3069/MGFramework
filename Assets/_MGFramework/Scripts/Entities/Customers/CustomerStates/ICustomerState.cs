
namespace MGFramework
{
    public interface ICustomerState
    {
        void Enter();
        void Exit();
        void SlowTick(); // 0.5초마다 호출
        void FixedTick(); // FixedUpdate 호출
    }
}