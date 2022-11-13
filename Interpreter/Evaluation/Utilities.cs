﻿using System;
using System.Collections.Generic;
using LCI_Korniienko.Components;

namespace LCI_Korniienko.Evaluation
{
    static class Utilities
    {
        public static string OutputLog;
        // -- GETTER & CHECKER --

        // retrieves all name captured variables when substituting 
        public static List<string> GetNameCaptures(Expression source, Expression substitute)
        {
            var captured = new List<string>();
            foreach (string v in source.GetBoundVars())
            {
                if (substitute.GetFreeVars().Contains(v))
                {
                    captured.Add(v);
                }
            }
            return captured;
        }

        // for alpha conversion, a new variable name is necessary
        // because there must not be variables named equal,
        // this method retrieves the first unused variable name
        public static string GetNewVar(Expression source, Expression substitute)
        {
            var usedVars = substitute.GetFreeVars();
            usedVars.AddRange(source.GetFreeVars());
            usedVars.AddRange(source.GetBoundVars());

            var newVar = "a";
            while (usedVars.Contains(newVar))
            {
                int a = (int)newVar[0] + 1;
                newVar = ((char)(a)).ToString();
            }
            return newVar;
        }

        // checker whether the current lambda expression can still be reduced
        // under that evaluation, i.e. is a Reducible Expression
        public static bool IsRedex(Expression expr, Components.Evaluation eval)
        {
            return !expr.Equals(expr.Reduce(eval, false));
        }

        // -- EXPRESSION REPLACING --

        // essential meta operation on lambda expression, used for reduction of any kind
        public static Expression Substitute(Expression source, string target, Expression substitute)
        {
            if (source is Constant)
            {
                // nothing to substitute
                return source;
            }
            else if (source is Variable)
            {
                var v = source as Variable;
                // full replacement
                if (v.GetVar().Equals(target))
                {
                    return substitute;
                }
                // nothing to substitute
                else
                {
                    return source;
                }
            }
            else if (source is Application)
            {
                var a = source as Application;
                // substitution in both subexpressions
                return new Application(Substitute(a.GetExpr1(), target, substitute), Substitute(a.GetExpr2(), target, substitute));
            }
            else if (source is Lambda)
            {
                var l = source as Lambda;
                if (l.GetVar().Equals(target))
                {
                    // nothing to substitute
                    return source;
                }
                // substitution for beta reduction
                else if (!substitute.GetFreeVars().Contains(l.GetVar()))
                {
                    return new Lambda(l.GetVar(), Substitute(l.GetExpr(), target, substitute));
                }
                // alpha conversion + beta reduction
                else
                {
                    var z = GetNewVar(l.GetExpr() , substitute);
                    return Substitute(new Lambda(z, Substitute(l.GetExpr(), l.GetVar(), new Variable(z))), target, substitute);
                }
            }
            return null;
        }

        // more powerful replacement method than 'Substitute', can replace any variable
        // used mainly for alpha conversion
        public static Expression VarReplace(Expression source, string target, string substitute)
        {
            if (source is Constant)
            {
                // nothing to replace
                return source;
            }
            else if (source is Variable)
            {
                var v = source as Variable;
                // variable change
                if (v.GetVar().Equals(target))
                {
                    return new Variable(substitute);
                }
                // nothing to replace
                else
                {
                    return source;
                }
            }
            else if (source is Application)
            {
                var a = source as Application;
                // replacement in both subexpressions
                return new Application(VarReplace(a.GetExpr1(), target, substitute), VarReplace(a.GetExpr2(), target, substitute));
            }
            else if (source is Lambda)
            {
                var l = source as Lambda;
                if (l.GetVar().Equals(target))
                {
                    // changing the lambdas single bound variable
                    return new Lambda(substitute, VarReplace(l.GetExpr(), target, substitute));
                }
                else
                {
                    // changing free variables inside body
                    return new Lambda(l.GetVar(), VarReplace(l.GetExpr(), target, substitute));
                }
            }
            return null;
        }

        // -- EXPRESSION CALCULATIOn --

        // does the primitive calculuations under the operators: { '+', '-', '*', '/', '=' }
        // { '-', '*', '/' } are only implemented for numeric constants whereas
        // { '+' } can also be applied as string concatenation and
        // { '=' } compares for equality between any constants
        public static Expression DeltaReduce(Constant op, Constant e1, Constant e2)
        {
            switch (op.GetContent())
            {
                case "+":
                    if (e1.GetConstantType() == Constant.Type.Numeric && e2.GetConstantType() == Constant.Type.Numeric)
                    {
                        // addition
                        return new Constant((Int32.Parse(e1.GetContent()) + Int32.Parse(e2.GetContent())).ToString());
                    }
                    else if (e1.GetConstantType() == Constant.Type.Characters && e2.GetConstantType() == Constant.Type.Characters)
                    {
                        // string concatenation
                        return new Constant(e1.GetContent().Substring(0, e1.GetContent().Length - 1) + e2.GetContent().Substring(1));
                    }
                    break;
                case "-":
                    if (e1.GetConstantType() == Constant.Type.Numeric && e2.GetConstantType() == Constant.Type.Numeric)
                    {
                        // substraction
                        return new Constant((Int32.Parse(e1.GetContent()) - Int32.Parse(e2.GetContent())).ToString());
                    }
                    break;
                case "*":
                    if (e1.GetConstantType() == Constant.Type.Numeric && e2.GetConstantType() == Constant.Type.Numeric)
                    {
                        // multiplication
                        return new Constant((Int32.Parse(e1.GetContent()) * Int32.Parse(e2.GetContent())).ToString());
                    }
                    break;
                case "/":
                    if (e1.GetConstantType() == Constant.Type.Numeric && e2.GetConstantType() == Constant.Type.Numeric)
                    {
                        // division
                        return new Constant((Int32.Parse(e1.GetContent()) / Int32.Parse(e2.GetContent())).ToString());
                    }
                    break;
                case "=":
                    // equality checking
                    return new Constant(e1.GetContent().Equals(e2.GetContent()) ? "True" : "False");
                default:
                    break;
            }
            return null;
        }

        // -- OTHER --

        // logging helper that takes an extra visibility argument
        public static void Log(string message, bool visible)
        {
            if (visible)
            {
                var prompt = "";
                switch (Expression.GetLastOperation())
                {
                    case Operation.Alpha:
                        prompt = " | α | : ";
                        break;
                    case Operation.Beta:
                        prompt = " | β | : ";
                        break;
                    case Operation.Delta:
                        prompt = " | δ | : ";
                        break;
                    case Operation.Expand:
                        prompt = " | ==> | : ";
                        break;
                    default:
                        prompt = "";
                        break;
                }
                OutputLog = OutputLog + "\n" + prompt + message;
                Console.WriteLine(OutputLog);
            }
        }
    }
}
