namespace Hathora {
    public class JWT {

        // Source: https://stackoverflow.com/a/39280625/834459
        public static string Decode(string token) {

            var parts = token.Split('.');
            if (parts.Length > 2) {
                var decode = parts[1];
                var padLength = 4 - decode.Length % 4;
                if (padLength < 4) {
                    decode += new string('=', padLength);
                }
                var bytes = System.Convert.FromBase64String(decode);

                return System.Text.ASCIIEncoding.ASCII.GetString(bytes);
            }

            return "";
        }
    }
}
