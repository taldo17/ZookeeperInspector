using System.Text;

namespace ZookeeperInspector
{
    public class BasicTextConvertor : ITextConvertor
    {

        public byte[] GetBytesFromTextAscii(string str)
        {
            return Encoding.ASCII.GetBytes(str);
        }

        public string GetSTextFromBytesAscii(byte[] bytes)
        {
            return Encoding.ASCII.GetString(bytes);
        }

        public string GetStringFromBytesUtf8(byte[] bytes)
        {
            return Encoding.UTF8.GetString(bytes);
        }
    }
}