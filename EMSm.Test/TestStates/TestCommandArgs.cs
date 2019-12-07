using System;
using System.Collections.Generic;
using System.Text;

namespace EMSm.Test.TestStates
{
    internal class ComplexRefType
    {
        public int IntVar { get; set; }

        public string StrVar { get; set; }

    }

    internal struct ComplexValType
    {
        public int IntVar { get; set; }

        public string StrVar { get; set; }

    }
    internal class TestCommandArgs
    {
        public int IntVar { get; set; }

        public string StrVar { get; set; }

        public ComplexRefType RefType { get; set; }

        public ComplexValType ValType { get; set; }
    }

}
