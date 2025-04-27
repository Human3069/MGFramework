namespace _KMH_Framework
{
    public interface IExcelRow
    {
        /// <summary>
        /// 값들을 정규화하는 기능.
        /// </summary>
        void Validate();

        string GetName();
    }
}
