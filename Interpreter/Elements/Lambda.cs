using System;
using System.Collections.Generic;
using static LCI_Korniienko.Evaluation.Utilities;

namespace LCI_Korniienko.Components
{
    class Lambda : Expression
    {
        // variable that is bound in the body expression
        private string var;
        // lambda function body
        private Expression expr;

        public Lambda(string v, Expression e)
        {
            this.var = v;
            this.expr = e;
        }

        public string GetVar()
        {
            return this.var;
        }

        public Expression GetExpr()
        {
            return this.expr;
        }

        // collects free variables by getting all free variables of the body
        // and then removing all variables that equal its bound one
        public override List<string> GetFreeVars()
        {
            var freeVars = this.expr.GetFreeVars();
            freeVars.RemoveAll(item => item.Equals(this.var));
            return freeVars;
        }

        // collects bound variables by getting all bound variables of the body
        // and adding the one that is bound herer
        public override List<string> GetBoundVars()
        {
            var boundVars = this.expr.GetBoundVars();
            boundVars.Add(this.var);
            return boundVars;
        }

        // reduces the lambda expression
        public override Expression Reduce(Evaluation _, bool annotate)
        {
            // for lazy and eager evaluation, a single lambda expression is already in WHNF
            lastOperation = Operation.None;
            return this;
        }

        // expands nothing because it will not be evaluated
        public override Expression ExpandVariable(bool annotate)
        {
            // for lazy and eager evaluation, a single lambda expression is already in WHNF
            lastOperation = Operation.None;
            return this;
        }

        // this lambda is only equal if variable name and the body expression are equal
        public override bool Equals(Expression other)
        {
            if (other is Lambda)
            {
                var otherLam = other as Lambda;
                return this.var.Equals(otherLam.GetVar()) && this.expr.Equals(otherLam.GetExpr());
            }
            else
            {
                return false;
            }
        }

        // converts to a string by converting the function body to string and
        // merging consecutive lambdas into the notation: \x_1 ... x_k. expr
        public override string ToString()
        {
            var str = "\\" + this.var;
            Expression body = this.GetExpr(); ;
           // while (body is Lambda)
         //   {
          //      str += " " + (body as Lambda).GetVar();
          //      body = (body as Lambda).GetExpr();
           // }
            return str + ". " + body.ToString();
        }
    }
}
