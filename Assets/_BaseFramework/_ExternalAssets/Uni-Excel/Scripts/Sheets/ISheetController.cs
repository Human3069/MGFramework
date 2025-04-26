using System.Collections;
using System.Collections.Generic;

namespace _KMH_Framework
{
    public interface ISheetController 
    {
        List<IExcelRow> Read(string directory, string fileName);
    }
}
