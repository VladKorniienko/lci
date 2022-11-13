namespace LCI_Korniienko.Components
{
    // 2 evaluation strategies are possible:
    //   lazy - evaluates until WHNF (no top-level redexes)
    //          by starting the reduction leftmost outermost
    //   eager - tries to evaluate until WHNF (no top-level redexes)
    //          by starting the reduction leftmost innermost
    public enum Evaluation
    {
        Lazy, Eager
    }

    // 3 operations are possible:
    //   alpha conversion (= bound variable renaming)
    //   beta reduction (= term evaluation by substitution)
    //   delta reduction (= primitive calculation)
    // 1 extra operation is the variable expansion, called when there are no redexes
    public enum Operation
    {
        None, Alpha, Beta, Delta, Expand
    }

    public abstract class Expression : IEquatable<Expression>
    {
        // -- STATIC METHODS --
        
        internal static Dictionary<string, Expression> assignedVariables = new();
        internal static Operation lastOperation = Operation.None;

        public static Expression GetVariable(string name)
        {
            return assignedVariables[name];
        }

        public static void SetVariable(string name, Expression expr)
        {
            assignedVariables[name] = expr;
        }

        // access to the last reduction operation
        public static Operation GetLastOperation()
        {
            return Expression.lastOperation;
        }

        // -- INSTANCE METHODS --

        // concept of free variables: are retrieved here
        public abstract List<string> GetFreeVars();

        // concept of bound variables: are retrieved here
        public abstract List<string> GetBoundVars();

        // default call to reduce
        public Expression Reduce(Evaluation eval)
        {
            return Reduce(eval, true);
        }

        // Lambda expression can be reduced in 3 ways:
        // lazy, eager or normal evaluation
        // 
        // For this, 3 types of operations are used:
        // - alpha conversion (= renaming)
        // - beta reduction (= reduction)
        // - delta reduction (= primitive calulation)
        public abstract Expression Reduce(Evaluation eval, bool annotate);

        // Expands one free variable if it has an assigned value
        public abstract Expression ExpandVariable(bool annotate);

        // has to implement the equality check
        public abstract bool Equals(Expression other);

        // has to override the string representation for correct notation
        public override abstract string ToString();
    }
}
