
namespace MGFramework
{
    public interface IEmployeeState 
    {
        void Enter(EmployeeContext context, EmployeeData data);

        void Exit();

        void SlowTick(); // 0.5초마다 호출
    }
}
