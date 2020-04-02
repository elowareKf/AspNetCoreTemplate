using System.Text;

namespace Template.Models {
    public class User : ModelBase {
        public string UserName { get; set; }
        public string Name { get; set; }
        public string PasswordHash { get; set; }
        
        public static string CreateMD5(string input) {
            if (string.IsNullOrEmpty(input))
                return null;
            // Use input string to calculate MD5 hash
            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create()
            ) {
                byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                // Convert the byte array to hexadecimal string
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++) {
                    sb.Append(hashBytes[i].ToString("X2"));
                }

                return sb.ToString();
            }
        }

    }
}