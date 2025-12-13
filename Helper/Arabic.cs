namespace ESSPMemberService.Helper
{
    public class Arabic
    {

        public static string NormalizeArabic(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return text;

            return text
                .Trim()
                .Replace("أ", "ا")
                .Replace("إ", "ا")
                .Replace("آ", "ا")
                .Replace("ة", "ه")
                .Replace("ى", "ي")
                .Replace("ؤ", "و")
                .Replace("ئ", "ي");
        }
    }
}
