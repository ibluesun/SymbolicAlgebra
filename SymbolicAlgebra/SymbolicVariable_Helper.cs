using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;


namespace SymbolicAlgebra
{
    public partial class SymbolicVariable : ICloneable
    {
        private static SymbolicVariable _NegativeOne = new SymbolicVariable("-1");
        private static SymbolicVariable _Zero = new SymbolicVariable("0");
        private static SymbolicVariable _One = new SymbolicVariable("1");
        private static SymbolicVariable _Two = new SymbolicVariable("2");
        private static SymbolicVariable _Three = new SymbolicVariable("3");
        private static SymbolicVariable _Four = new SymbolicVariable("4");
        private static SymbolicVariable _Five = new SymbolicVariable("5");
        private static SymbolicVariable _Six = new SymbolicVariable("6");
        private static SymbolicVariable _Seven = new SymbolicVariable("7");
        private static SymbolicVariable _Eight = new SymbolicVariable("8");
        private static SymbolicVariable _Nine = new SymbolicVariable("9");
        private static SymbolicVariable _Ten = new SymbolicVariable("10");
        private static SymbolicVariable _Eleven = new SymbolicVariable("11");
        private static SymbolicVariable _Twelve = new SymbolicVariable("12");

        public static SymbolicVariable NegativeOne
        {
            get { return SymbolicVariable._NegativeOne; }
        }

        public static SymbolicVariable Zero
        {
            get { return SymbolicVariable._Zero; }
        }

        public static SymbolicVariable One
        {
            get { return SymbolicVariable._One; }
        }

        public static SymbolicVariable Two
        {
            get { return SymbolicVariable._Two; }
        }

        public static SymbolicVariable Three
        {
            get { return SymbolicVariable._Three; }
        }

        public static SymbolicVariable Four
        {
            get { return SymbolicVariable._Four; }
        }

        public static SymbolicVariable Five
        {
            get { return SymbolicVariable._Five; }
        }

        public static SymbolicVariable Six
        {
            get { return SymbolicVariable._Six; }
        }

        public static SymbolicVariable Seven
        {
            get { return SymbolicVariable._Seven; }
        }

        public static SymbolicVariable Eight
        {
            get { return SymbolicVariable._Eight; }
        }

        public static SymbolicVariable Nine
        {
            get { return SymbolicVariable._Nine; }
        }

        public static SymbolicVariable Ten
        {
            get { return SymbolicVariable._Ten; }
        }


        public static SymbolicVariable Eleven
        {
            get { return SymbolicVariable._Eleven; }
        }

        public static SymbolicVariable Twelve
        {
            get { return SymbolicVariable._Twelve; }
        }
    }
}