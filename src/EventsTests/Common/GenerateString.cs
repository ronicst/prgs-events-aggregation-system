using System.Text;

namespace EventsTests.Common
{
    internal class GenerateString
    {
        public string createString(int length, char character)
        {
            StringBuilder builder = new StringBuilder(length);
            for (int i = 0; i < length; i++)
            {
                builder.Append(character);
            }
            return builder.ToString();
        }
    }
}
