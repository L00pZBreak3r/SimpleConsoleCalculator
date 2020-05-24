using System;
using System.Globalization;
using System.Collections.Generic;
using System.Linq;

namespace SimpleCalculatorExpressionParserLibrary
{
  public class ArithmeticExpression
  {
    public Dictionary<string, double> Variables;

    public static double GetValue(string s)
    {
      double res = double.NaN;
      if (!double.TryParse(s, out res) && !double.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out res))
        res = double.NaN;
      return res;
    }

    private static bool IsDelim(char c)
    {
      return c == ' ' || c == '\t';
    }

    private static bool IsOperation(char c)
    {
      return c == '+' || c == '-' || c == '*' || c == '/' || c == '%' || c == '^' || c == '~';
    }

    private static bool IsOperationUnary(char c)
    {
      return c == '#' || c == '$' || c == '~';
    }

    private static bool IsUnary(char c, char p)
    {
      return (c == '~') || (c == '+' || c == '-') && (p == '\0' || p == '(');
    }

    private static char ConvertToUnary(char c)
    {
      char res = c;
      if (c == '+')
        res = '#';
      else if (c == '-')
        res = '$';
      return res;
    }

    private static int GetPriority(char op)
    {
      if (op == '#' || op == '$' || op == '~')
        return 4; // '#' == '+' || '$' == '-' || '~' == '~'
      return
        op == '^' ? 5 :
        op == '+' || op == '-' ? 1 :
        op == '*' || op == '/' || op == '%' ? 2 :
        -1;
    }

    private static void ProcessOperation(List<double> st, char op)
    {
      if (op == '#' || op == '$' || op == '~')
      {
        double l = st.Last(); st.RemoveAt(st.Count - 1);
        switch (op)
        {
          case '#': st.Add(l);  break; //+
          case '$': st.Add(-l); break; //-
          case '~': st.Add(~((int)l)); break; //~
        }
      }
      else
      {
        double r = st.Last(); st.RemoveAt(st.Count - 1);
        double l = st.Last(); st.RemoveAt(st.Count - 1);
        switch (op)
        {
          case '+': st.Add(l + r); break;
          case '-': st.Add(l - r); break;
          case '*': st.Add(l * r); break;
          case '/': st.Add(l / r); break;
          case '%': st.Add((int)l % (int)r); break;
          case '^': st.Add(Math.Pow(l, r)); break;
        }
      }
    }

    private double GetVariableValue(string s)
    {
      double res = double.NaN;
      if ((Variables != null) && (Variables.Count > 0) && !string.IsNullOrWhiteSpace(s))
      {
        s = s.Trim().ToLowerInvariant();
        if (Variables.ContainsKey(s))
          res = Variables[s];
      }
      return res;
    }

    private void Calculate()
    {
      bool may_unary = true;
      List<double> st = new List<double>();
      List<char> op = new List<char>();
      for (int i = 0; i < mExpression.Length; ++i)
        if (!IsDelim(mExpression[i]))
          if (mExpression[i] == '(')
          {
            op.Add('(');
            may_unary = true;
          }
          else if (mExpression[i] == ')')
          {
            while (op.Last() != '(')
            {
              ProcessOperation(st, op.Last());
              op.RemoveAt(op.Count - 1);
            }
            op.RemoveAt(op.Count - 1);
            may_unary = false;
          }
          else if (IsOperation(mExpression[i]))
          {
            char curop = mExpression[i];
            char p = (i > 0) ? mExpression[i - 1] : '\0';
            if (may_unary && IsUnary(curop, p))
              curop = ConvertToUnary(curop);
            while ((op.Count > 0) && (
              !IsOperationUnary(curop) && GetPriority(op.Last()) >= GetPriority(curop)
              || IsOperationUnary(curop) && GetPriority(op.Last()) > GetPriority(curop))
              )
            {
              ProcessOperation(st, op.Last());
              op.RemoveAt(op.Count - 1);
            }
            op.Add(curop);
            may_unary = true;
          }
          else
          {
            string operand = "";
            while (i < mExpression.Length && (char.IsLetterOrDigit(mExpression[i]) || (mExpression[i] == '.') || (mExpression[i] == ',')))
              operand += mExpression[i++];
            --i;
            if (char.IsDigit(operand[0]))
              st.Add(GetValue(operand));
            else
              st.Add(GetVariableValue(operand));
            may_unary = false;
          }
      while (op.Count > 0)
      {
        ProcessOperation(st, op.Last());
        op.RemoveAt(op.Count - 1);
      }
      mResult = st.Last();
    }

    #region Expression

    private string mExpression;

    public string Expression
    {
      get
      {
        return mExpression;
      }

      set
      {
        if (value != mExpression)
        {
          mExpression = value;
          Calculate();
        }
      }
    }

    #endregion

    #region Result

    private double mResult = double.NaN;

    public double Result
    {
      get
      {
        return mResult;
      }
    }

    #endregion
  }
}
