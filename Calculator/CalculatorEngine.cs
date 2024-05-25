namespace Calculator
{
    public class CalculatorEngine : CalculatorBaseVisitor<double>
    {

        public override double VisitComputation( CalculatorParser.ComputationContext context )
        {
            if ( context.assignment() != null )
                return Visit( context.assignment() );
            else if ( context.expression() != null )
                return Visit( context.expression() );
            else
                return base.VisitComputation( context );
        }

        public override double VisitAssignment( CalculatorParser.AssignmentContext context )
        {
            // 1. DETERMINE THE VARIABLE NAME
            // 2. DETERMINE THE VALUE OF THE EXPRESSION
            // 3. STORE THE VALUE IN THE VARIABLE 
            return base.VisitAssignment( context );
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
                    // RETURN THE CURRENT VALUE OF THE VARIABLE
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
