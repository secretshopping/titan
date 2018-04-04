using Resources;
using System.Web;

public static class InputChecker
{
    public static string HtmlEncode(string input, int inputLimit, string inputName)
    {
        string encoded = HtmlEncode(input);

        if (encoded.Length > inputLimit)
            throw new MsgException(string.Format(U6012.INPUTTOOLONG, inputName, inputLimit));

        return encoded;
    }

    public static string HtmlEncode(string input)
    {
        string encoded = HttpUtility.HtmlEncode(input);
        return encoded;
    }

    public static string HtmlPartialDecode(string input)
    {
        return HttpUtility.HtmlAttributeEncode(HttpUtility.HtmlDecode(input));
    }
}