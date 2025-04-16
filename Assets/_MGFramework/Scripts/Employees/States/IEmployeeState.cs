
namespace MGFramework
{
    public interface IEmployeeState 
    {
        void Enter(Employee employee);

        void Exit();

        void Tick();

        void FixedTick();

        void SlowTick(); // 0.5�ʸ��� ȣ��
    }
}
