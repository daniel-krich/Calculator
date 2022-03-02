using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace WinCalculator.Core
{
    public static class Calculator
    {
        public static double Evaluate(string expression)
        {
            string updated_expression = CalculateAndParseMultiplicationsAndDivisions(expression);
            return CalculatePlusMinus(updated_expression);
        }

        /// <summary>
        /// Sums up all the plus/minus operations after we delt with the multiplications/divisions.
        /// </summary>
        /// <param name="expression">string math operation with plus/minus only, Example: "5+4+8-4-10+20"</param>
        /// <returns>Final result as a double</returns>
        private static double CalculatePlusMinus(string expression)
        {
            double finalResult = 0;
            //
            Regex reg = new Regex(@"([\+\-]*)([0-9.]*)");
            MatchCollection mc = reg.Matches(expression.Trim());
            for (int i = 0; i < mc.Count(); i++)
            {

                GroupCollection gc = mc[i].Groups;
                if (gc.Count > 2 && gc[2].Value.Length > 0)
                {
                    switch (gc[1].Value)
                    {
                        case "":
                        case "+":
                            finalResult += Convert.ToDouble(gc[2].Value);
                            break;
                        case "-":
                            finalResult -= Convert.ToDouble(gc[2].Value);
                            break;
                        default:
                            throw new ApplicationException("Invalid operation");
                    }
                }
            }
            return finalResult;
        }

        /// <summary>
        /// Takes a string math expression and inserts the multiplications/division results back to the string.
        /// For example "5+6+2*4*5-10+25" becomes "5+6+40-10+25"
        /// </summary>
        /// <param name="expression">string math operation with plus/minus/multiplication/division</param>
        /// <returns>Calculated string math expression with only plus/minus operations</returns>
        private static string CalculateAndParseMultiplicationsAndDivisions(string expression)
        {
            double multiplicationDivideCombo = 1;
            int[] tempReplaceIndexes = { 0, 0 }; // start, end
            IList<double[]> ReplaceExpresions = new List<double[]>();
            Regex multiplicationAndDivideRegex = new Regex(@"([0-9.]*)([\*\/])([0-9.]*)");
            MatchCollection matchCollection = multiplicationAndDivideRegex.Matches(expression.Trim());
            for (int i = 0; i < matchCollection.Count(); i++)
            {
                if (matchCollection[i].Groups[1].Value.Length > 0) // number start
                {
                    if (tempReplaceIndexes[0] != tempReplaceIndexes[1])
                        ReplaceExpresions.Add(new[]
                        {
                            multiplicationDivideCombo,
                            tempReplaceIndexes[0],
                            tempReplaceIndexes[1]
                        });
                    //
                    switch (matchCollection[i].Groups[2].Value)
                    {
                        case "*":
                            multiplicationDivideCombo = Convert.ToDouble(matchCollection[i].Groups[1].Value) * Convert.ToDouble(matchCollection[i].Groups[3].Value);
                            break;
                        case "/":
                            multiplicationDivideCombo = Convert.ToDouble(matchCollection[i].Groups[1].Value) / Convert.ToDouble(matchCollection[i].Groups[3].Value);
                            break;
                        default:
                            throw new ApplicationException("Invalid operation");
                    }
                    tempReplaceIndexes[0] = matchCollection[i].Groups[1].Index;
                    tempReplaceIndexes[1] = matchCollection[i].Groups[3].Index + matchCollection[i].Groups[3].Length - 1;
                    if (i == matchCollection.Count() - 1)
                    {
                        if (tempReplaceIndexes[0] != tempReplaceIndexes[1])
                            ReplaceExpresions.Add(new[]
                            {
                                multiplicationDivideCombo,
                                tempReplaceIndexes[0],
                                tempReplaceIndexes[1]
                            });
                        multiplicationDivideCombo = 1;
                    }

                }
                else
                {
                    switch (matchCollection[i].Groups[2].Value)
                    {
                        case "*":
                            multiplicationDivideCombo *= Convert.ToDouble(matchCollection[i].Groups[3].Value);
                            break;
                        case "/":
                            multiplicationDivideCombo /= Convert.ToDouble(matchCollection[i].Groups[3].Value);
                            break;
                        default:
                            throw new ApplicationException("Invalid operation");
                    }
                    tempReplaceIndexes[1] = matchCollection[i].Groups[3].Index + matchCollection[i].Groups[3].Length - 1;
                    if (i == matchCollection.Count() - 1)
                    {
                        if (tempReplaceIndexes[0] != tempReplaceIndexes[1])
                            ReplaceExpresions.Add(new[]
                            {
                                multiplicationDivideCombo,
                                tempReplaceIndexes[0],
                                tempReplaceIndexes[1]
                            });
                        multiplicationDivideCombo = 1;
                    }
                }
            }
            foreach (double[] data in ReplaceExpresions.Reverse())
            {
                expression = expression.Remove((int)data[1], (int)data[2] - (int)data[1] + 1).Insert((int)data[1], data[0].ToString());
            }
            return expression;
        }
    }
}
