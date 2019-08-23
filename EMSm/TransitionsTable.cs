using System;
using System.Collections.Generic;
using System.Text;

namespace EM.EMSm
{
    public class TransitionEntry
    {
        public Enum Transition { get; set; }

        public Type StateType { get; set; }

        public string StateName { get; set; }
    }

    public class TransitionsTable : List<TransitionEntry>
    { }


    /*class TransitionsTable : Dictionary<Enum, State>
    {

    }*/
}
