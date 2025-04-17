
namespace MGFramework
{
    public interface ICustomerState
    {
        void Enter(CustomerContext context, CustomerData data);

        void Exit();

        void FixedTick();

        void SlowTick(); // 0.5�ʸ��� ȣ��
    }
}
