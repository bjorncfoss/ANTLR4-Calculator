using System;
using System.Collections.Generic;
using System.Linq;

namespace Calculator
{
    public class CalculatorEngine : CalculatorBaseVisitor<double>
    {
        // 1. For variable implementation, we start by creating a list
        private static List<(string Key, double Value)> storeVariables;

        // Constructor
        static CalculatorEngine()
        {
            storeVariables = new List<(string Key, double Value)>();
        }

        private static bool TryGetVariable(string key, out double value)
        {
            var variable = storeVariables.FirstOrDefault(x => x.Key == key);
            if (variable.Key != null)
            {
                value = variable.Value;
                return true;
            }
            value = 0.0;
            return false;
        }

        private static void SetVariable(string key, double value)
        {
            var index = storeVariables.FindIndex(x => x.Key == key);
            if (index >= 0)
            {
                storeVariables[index] = (key, value);
            }
            else
            {
                storeVariables.Add((key, value));
            }
        }

        public override double VisitComputation(CalculatorParser.ComputationContext context)
        {
            if (context.assignment() != null)
                return Visit(context.assignment());
            else if (context.expression() != null)
                return Visit(context.expression());
            else
                return base.VisitComputation(context);
        }

        public override double VisitAssignment(CalculatorParser.AssignmentContext context)
        {
            // 1.
            var storeVariableNames = context.IDENTIFIER().GetText();
            // 2.
            var storeValues = Visit(context.expression());
            // 3.
            SetVariable(storeVariableNames, storeValues);
            return storeValues;
        }

        public override double VisitExpression(CalculatorParser.ExpressionContext context)
        {
            if (context.ChildCount == 1)
            {
                return Visit(context.term());
            }
            else
            {
                var expression = Visit(context.expression());
                var term = Visit(context.term());

                switch (context.GetChild(1).GetText())
                {
                    case "+":
                        return expression + term;
                    case "-":
                        return expression - term;
                }
            }
            return base.VisitExpression(context);
        }

        public override double VisitTerm(CalculatorParser.TermContext context)
        {
            if (context.ChildCount == 1)
            {
                return Visit(context.factor());
            }
            else
            {
                var term = Visit(context.term());
                var factor = Visit(context.factor());

                switch (context.GetChild(1).GetText())
                {
                    case "*":
                        return term * factor;
                    case "/":
                        return term / factor;
                }
            }
            return base.VisitTerm(context);
        }

        public override double VisitFactor(CalculatorParser.FactorContext context)
        {
            var value = Visit(context.value());
            return context.ChildCount == 1 ? value : -value;
        }

        public override double VisitValue(CalculatorParser.ValueContext context)
        {
            if (context.ChildCount == 1)
            {
                if (context.NUMBER() != null)
                {
                    var lexeme = context.NUMBER().GetText();
                    if (double.TryParse(lexeme, out var value)) return value;
                }
                else if (context.IDENTIFIER() != null)
                {
                    var lexeme = context.IDENTIFIER().GetText();
                    if (TryGetVariable(lexeme, out var storeValues))
                    {
                        return storeValues;
                    }
                }
            }
            else
            {
                return Visit(context.expression());
            }
            return 0.0;
        }
    }
}
