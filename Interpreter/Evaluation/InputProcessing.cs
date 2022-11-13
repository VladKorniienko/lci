using LCI_Korniienko.Components;
using static LCI_Korniienko.Components.Expression;
using static LCI_Korniienko.Parser.Parser;
using static LCI_Korniienko.Renderer.ASTBuilder;
using LCI_Korniienko.Data;

namespace LCI_Korniienko.Evaluation
{
    static class InputProcessing
    {
        private static Components.Evaluation currentEval = Components.Evaluation.Lazy;
        public static string Run(CommandDto input)
        {
            if (!input.CommandString.ToLower().Equals(""))
            {
                string result = RunCommand(input.CommandString);
                Utilities.OutputLog = "";
                return result;
            }
            else
                return "Error! Empty input!";

        }
        private static string RunCommand(string input)
        {

            if (input.ToLower().Equals("info"))
            {
                return "Information about the API that can evaluate lambda expressions. Type:\n" +
                                  "    info                   - to show information about this API\n" +
                                  "    set evaluation [value] - to set the evaluation strategy to lazy or eager, where [value] = lazy (eager)\n" +
                                  "    let [var] = [expr]     - to assign an expression [expr] to the variable [var]\n" +
                                  "    reduce [expr]          - to reduce the expression according to the currently set strategy\n" +
                                  "                             (alpha conversion, beta and delta reduction and variable expansion)\n" +
                                  "    build_ast [expr]       - to build and display an abstract syntax tree of the given expression [expr]";
            }
            else if (input.ToLower().StartsWith("set") && input.Split(' ').Length >= 3)
            {
                if(input.Split(' ')[1].ToLower()== "evaluation")
                {
                        try
                        {
                            var evalName = char.ToUpper(input.Split(' ')[2][0]) + input.Split(' ')[2].Substring(1).ToLower();
                            currentEval = (Components.Evaluation)Enum.Parse(Components.Evaluation.Lazy.GetType(), evalName);
                            return "DONE - Set " + currentEval.ToString().ToLower() + " evaluation strategy";
                        }
                        catch (ArgumentException)
                        {
                            return "Error! Invalid evaluation name";
                        }     
                }
                else
                {
                        return "Error! Invalid command";
                }
            }
            else if (input.ToLower().StartsWith("let") && input.Split(' ').Length >= 2 && input.Contains('='))
            {
                //var name = input.Split(' ')[1][0];
                var name = input.Split(' ')[1];
                var expr = Parse(input.Split('=', 2)[1]);
                if (!(expr is Variable && (expr as Variable).GetVar().Equals(name)))
                {
                    SetVariable(name, expr);
                }
                return "DONE - Expression assigned to " + name;
            }
            else if (input.ToLower().StartsWith("reduce") && input.Split(' ').Length >= 2)
            {
                var expr = Parse(input.Split(' ', 2)[1]);
                var reduced = expr;
                do
                {
                    do
                    {
                        expr = reduced;
                        reduced = expr.Reduce(currentEval);
                    } while (!expr.Equals(reduced));
                    expr = reduced;
                    reduced = expr.ExpandVariable(true);
                } while (!expr.Equals(reduced));
                return Utilities.OutputLog + "\nDONE - Expression reduced\n";
            }
            else if (input.ToLower().StartsWith("build_ast") && input.Split(' ').Length >= 2)
            {
                return (BuildTree(Parse(input.Split(' ', 2)[1])).ToString());
            }
            else
            {
                return "Error! Invalid action";
            }
        }
    }
}
