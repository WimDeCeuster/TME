namespace TME.CarConfigurator.Query.Tests.TestHelpers
{
    public static class Context
    {
        public static bool AreEqual(Repository.Objects.Context actualContext, Repository.Objects.Context expectedContext)
        {
            var type = actualContext.GetType();

            foreach (System.Reflection.PropertyInfo pi in type.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance))
            {
                var actualValue = type.GetProperty(pi.Name).GetValue(actualContext, null);
                var expectedValue = type.GetProperty(pi.Name).GetValue(expectedContext, null);

                if (actualValue == expectedValue || (actualValue != null && actualValue.Equals(expectedValue))) continue;
                
                return false;
            }

            return true;
        }
    }
}