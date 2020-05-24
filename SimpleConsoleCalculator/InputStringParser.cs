using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleConsoleCalculator
{
  class InputStringParser
  {
    private readonly Dictionary<string, double> mVariables = new Dictionary<string, double>();

    private void Parse()
    {
      mVariables.Clear();
      mExpression = "";
      if (!string.IsNullOrWhiteSpace(mInputString))
      {
        string[] a = mInputString.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
        foreach (string s in a)
          if (s.Contains("="))
          {
            string[] ve = s.Split(new char[] { '=' }, StringSplitOptions.RemoveEmptyEntries);
            if (ve.Length >= 2)
              mVariables.Add(ve[0].Trim(), ArithmeticExpression.GetValue(ve[1].Trim()));
          }
          else
            mExpression = s.Trim();
      }
    }

    public bool PrepareArithmeticExpression(ArithmeticExpression expr)
    {
      bool res = (expr != null) && !string.IsNullOrWhiteSpace(mExpression);
      if (res)
      {
        expr.Variables = mVariables;
        expr.Expression = mExpression;
      }
      return res;
    }

    #region InputString

    private string mInputString;

    public string InputString
    {
      get
      {
        return mInputString;
      }

      set
      {
        if (value != mInputString)
        {
          mInputString = value;
          Parse();
        }
      }
    }

    #endregion

    #region Expression

    private string mExpression;

    public string Expression
    {
      get
      {
        return mExpression;
      }
    }

    #endregion
  }
}
