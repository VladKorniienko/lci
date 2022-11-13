using System;
using System.Collections.Generic;
using static LCI_Korniienko.Evaluation.Utilities;

namespace LCI_Korniienko.Components
{
    class Variable : Expression
    {
        // single character variable name 
        private string var;

        public Variable(string v)
        {
            this.var = v;
        }

        public string GetVar()
        {
            return this.var;
        }

        // collects free variables by adding the only one in scope - itself
        public override List<string> GetFreeVars()
        {
            return new List<string>() { this.var };
        }

        // collects no bound variables because there can be none here
        public override List<string> GetBoundVars()
        {
            return new List<string>();
        }

        // tries to reduce this variable
        public override Expression Reduce(Evaluation _, bool annotate)
        {
            // a variable is already in normal form
            lastOperation = Operation.None;
            return this;
        }

        // replaces this single variable if it has a variable
        public override Expression ExpandVariable(bool annotate)
        {
            // if this variable was assigned a value, replace it
            if (assignedVariables.TryGetValue(this.var, out Expression value))
            {
                lastOperation = Operation.Expand;
                Log(value.ToString(), annotate);
                return value;
            }
            return this;
        }
        public bool CheckExpandableVariable()
        {
            // if this variable was assigned a value, return true
            if (assignedVariables.TryGetValue(this.var,out Expression value))
            {
                return true;
            }
            return false;
        }

        // this variable is only equal to another one if the name is equal
        public override bool Equals(Expression other)
        {
            if (other is Variable)
            {
                var otherVar = other as Variable;
                return this.var.Equals(otherVar.GetVar());
            }
            else
            {
                return false;
            }
        }

        // converts the character name to a string
        public override string ToString()
        {
            return this.var.ToString();
        }
    }
}
