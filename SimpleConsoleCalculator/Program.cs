using System;

namespace SimpleConsoleCalculator
{
  class Program
  {
    static void Main(string[] args)
    {
      if (args.Length > 0)
      {
        var sp = new InputStringParser();
        var ae = new ArithmeticExpression();

        sp.InputString = args[0];
        sp.PrepareArithmeticExpression(ae);

        Console.WriteLine("{0} = {1}", ae.Expression, ae.Result);
      }
      else
        Console.WriteLine("Enter an arithmetic expression in form \"EXPRESSION[;VAR1;VAR2;VAR3;...]\" as an argument,\ne.g.: (6.4 + 3.654) - a; a = 5.5\nSupported operations: +, -, *, /, %, ^, ~, ()");
    }
  }
}
