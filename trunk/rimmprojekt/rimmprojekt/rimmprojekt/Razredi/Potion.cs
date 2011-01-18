using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace rimmprojekt.Razredi
{
    class Potion
    {
        public String type;
        public Int32 value;

        public Potion(String tip, Int32 vrednost)
        {
            this.type = tip;
            this.value = vrednost;
        }

        public Potion()
        {
            this.type = "";
            this.value = 0;
        }

        public void setParameters(String tip,Int32 vrednost)
        {
            this.type = tip;
            this.value = vrednost;
        }
    }
}
