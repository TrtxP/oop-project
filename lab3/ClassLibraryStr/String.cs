namespace ClassLibraryStr
{
    public class String
    {
        public static string StringInvert(string str)
        {
            char[] charArr = str.ToCharArray();
            Array.Reverse(charArr);
            return new string(charArr);
        }

        public static int CountChars(string str, char ch)
        {
            int count = 0;
            foreach (char c in str)
            {
                if (c == ch)
                {
                    count++;
                }
            }
            return count;
        }
    }
}
