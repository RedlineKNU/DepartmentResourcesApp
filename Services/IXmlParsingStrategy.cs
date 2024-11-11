using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DepartmentResourcesApp.Services
{
    public interface IXmlParsingStrategy
    {
        void Parse(string filePath);
    }

}
