
namespace MGFramework
{
    public interface ICustomerState
    {
        void Enter();
        void Exit();
        void SlowTick(); // 0.5�ʸ��� ȣ��
        void FixedTick(); // FixedUpdate ȣ��
    }
}