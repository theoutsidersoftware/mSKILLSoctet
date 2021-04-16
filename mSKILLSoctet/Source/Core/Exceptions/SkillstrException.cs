using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mSKILLSoctet.Source.Core.Exceptions
{
    public class SkillstrException : Exception
    {
        public SkillstrException(Exception e) : base(e.Message)
        {
        }
    }
}
