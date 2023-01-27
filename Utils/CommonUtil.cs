using Office_Management_.NET_MVC_Angular_JWT.ResponseModels;

namespace Office_Management_.NET_MVC_Angular_JWT.Utils;
class CommonUtil : ICommonUtil
{
    public int PAGE_SIZE = 10;

    /// <summary>
    /// Converts a Unix timestamp into a System.DateTime
    /// </summary>
    /// <param name="timestamp">The Unix timestamp in milliseconds to convert, as a double</param>
    /// <returns>DateTime obtained through conversion</returns>
    public static DateTime ConvertFromUnixTimestamp(double timestamp)
    {
        DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
        return origin.AddSeconds(timestamp / 1000); // convert from milliseconds to seconds
    }

    int ICommonUtil.GetPageSize()
    {
        return PAGE_SIZE; // convert from milliseconds to seconds
    }
}