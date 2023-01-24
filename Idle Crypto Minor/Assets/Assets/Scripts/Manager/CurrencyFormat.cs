public static class CurrencyFormat
{
    public static string ToCurrency(this long amount)
    {
        int length = amount.ToString().Length;

        if (length > 7)
        {
            return amount.ToString("0,,,.##B");
        }
        else if (length > 5)
        {
            return amount.ToString("0,,.##M");
        }
        else if (length > 3)
        {
            return amount.ToString("0,.##K");
        }

        return amount.ToString("F0");
    }
}
