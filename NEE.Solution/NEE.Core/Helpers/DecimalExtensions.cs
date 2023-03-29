namespace NEE.Core.Helpers
{
    public static class DecimalExtensions
    {
        public static string DisplayAsCurrencyAmountFormatted(this decimal? value)
        {
            if (!value.HasValue)
            {
                value = 0;
            }

            return string.Format("{0:C}", value.Value);
        }

        public static string DisplayAsCurrencyAmountFormatted(this decimal value)
        {
            return string.Format("{0:C}", value);
        }
    }
}
