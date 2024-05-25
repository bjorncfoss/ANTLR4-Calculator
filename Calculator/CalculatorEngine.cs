namespace Calculator
{
    public class CalculatorEngine : CalculatorBaseVisitor<double>
    {
        // We start by creating a dictionary which will store our variables
        // For them to later be assigned.
        private static Dictionary<string, double> storeVariables = new Dictionary<string, double>();

        public override double VisitComputation( CalculatorParser.ComputationContext context )
        {
            if ( context.assignment() != null )
                return Visit( context.assignment() );
            else if ( context.expression() != null )
                return Visit( context.expression() );
            else
                return base.VisitComputation( context );
        }

        // This functions assigns the variables and expressions containing those variables.
        public override double VisitAssignment( CalculatorParser.AssignmentContext context )
        {
            // STEP 1: Determine the Variable Name to be assigned
            var storeVariableNames = context.IDENTIFIER().GetText();

            // STEP 2: Determine the Expression Value
            var storeValues = Visit(context.expression());

            // STEP 3: Create return statement which stores the value in the assigned variable 
            storeVariables[storeVariableNames] = storeValues;
            return storeValues;
            //return base.VisitAssignment( context );
        }

        public override double VisitExpression( CalculatorParser.ExpressionContext context )
        {
            if ( context.ChildCount == 1 )
            {
                return Visit( context.term() );
            }
            else
            { 
                var expression = Visit( context.expression() );
                var term = Visit( context.term() );

                switch ( context.GetChild(1).GetText() ) 
                {
                    case "+": return expression + term;
                    case "-": return expression - term;
                }
            }
            return base.VisitExpression( context );
        }

        public override double VisitTerm( CalculatorParser.TermContext context )
        {
            if ( context.ChildCount == 1 )
            {
                return Visit( context.factor() );
            }
            else
            {
                var term = Visit( context.term() );
                var factor = Visit( context.factor() ); 

                switch( context.GetChild(1).GetText() ) 
                {
                    case "*": return term * factor;
                    case "/": return term / factor;
                }
            }
            return base.VisitTerm( context );
        }

        public override double VisitFactor( CalculatorParser.FactorContext context )
        {
            var value = Visit( context.value() );
            return context.ChildCount == 1 ? value : - value;
        }

        public override double VisitValue( CalculatorParser.ValueContext context )
        {
            if ( context.ChildCount == 1 )
            {
                if ( context.NUMBER() != null )
                {
                    var lexeme = context.NUMBER().GetText();
                    if ( double.TryParse( lexeme, out var value ) ) return value;
                }
                else if ( context.IDENTIFIER() != null )
                {
                    var lexeme = context.IDENTIFIER().GetText();

                    // STEP 4: Returns the current value of the value
                    // when it's assigned on the prompt
                    if (storeVariables.ContainsKey(lexeme))
                    {
                        if (storeVariables.TryGetValue(lexeme, out var storeValues)) {
                            return storeValues;
                        }
                    }
                }
            }
            else
            {
                return Visit( context.expression() );
            }
            return 0.0;
        }
    }
}
