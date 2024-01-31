using System.Net.Mail;
using System.Text.RegularExpressions;

namespace WebApiLibrary.Util
{
    public static class Helper
    {
        /*1) It must contain at least a number
         * 2) one upper case letter
         * 3) 8 characters long.*/
        public static bool CheckPassword(string password)
        {
            string pattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)[a-zA-Z\d]{8,}$";
            Regex regex = new Regex(pattern);
            Match match = regex.Match(password);
            return match.Success;
        }

        public static bool CheckEmail(string emailaddress)
        {
            try
            {
                MailAddress m = new MailAddress(emailaddress);

                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }
    }
}
