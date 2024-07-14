namespace wema_test_service.Common.Helpers;

public static class UtilityHelper
{
    public static T DeSerializer<T>(string jsonString)
    {
        JsonSerializerSettings options = new()
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };
        return JsonConvert.DeserializeObject<T>(jsonString, options);
    }

    public static string GenerateOtp(int length)
    {
        char[] chars = "0123456789".ToCharArray();
        Random random = new();
        StringBuilder stringBuilder = new(length);

        for (int i = 0; i < length; i++)
        {
            int randomIndex = random.Next(chars.Length);
            stringBuilder.Append(chars[randomIndex]);
        }

        return stringBuilder.ToString();
    }

    public static string HashText(string text)
    {
        string hash = new PasswordHasher<object>().HashPassword(null, text);
        return hash;
    }

    public static bool VerifyHashedText(string hashedText, string text)
    {
        PasswordVerificationResult result = new PasswordHasher<object>().VerifyHashedPassword(null, hashedText, text);
        return result == PasswordVerificationResult.Success;
    }
}
