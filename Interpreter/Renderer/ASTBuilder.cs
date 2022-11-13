using LCI_Korniienko.Components;

namespace LCI_Korniienko.Renderer
{
    static class ASTBuilder
    {
        public static BuilderUtilities BuildTree(Expression expr)
        {
            return BuildTree(expr, 1);
        }

        private static BuilderUtilities BuildTree(Expression expr, int padding)
        {
            var tree = ConstructTree(expr);
            var treeRep = new Tree(tree.ToString());
            return new BuilderUtilities(treeRep.GetWidth() + 2 * padding, treeRep.GetHeight() + 2 * padding, 
                                  treeRep, BuilderUtilities.PlaneAlignment.CENTER);
        }

        private static Tree ConstructTree(Expression expr)
        {
            if (expr is Constant)
            {
                var c = expr as Constant;
                return new Tree(c.GetContent());
            }
            else if (expr is Variable)
            {
                var v = expr as Variable;
                return new Tree(v.GetVar().ToString());
            }
            else if (expr is Application)
            {
                var a = expr as Application;
                return new Tree("@", ConstructTree(a.GetExpr1()), ConstructTree(a.GetExpr2()));
            }
            else if (expr is Lambda)
            {
                var l = expr as Lambda;
                return new Tree("\u03bb " + l.GetVar(), ConstructTree(l.GetExpr()));

            }
            return null;
        }
    }
}
