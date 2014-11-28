using System;

namespace TME.CarConfigurator.Publisher.Exceptions
{
    public class UnrecognisedRuleCategoryException : Exception
    {
         public UnrecognisedRuleCategoryException(Administration.Enums.RuleCategory category)
             : base(String.Format("Cannot map unknown category value {0}", category.ToString("g")))
         {}
    }
}