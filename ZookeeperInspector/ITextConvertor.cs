namespace ZookeeperInspector
{
    public interface ITextConvertor
    {
        byte[] GetBytesFromTextAscii(string str);

        string GetSTextFromBytesAscii(byte[] bytes);

        string GetStringFromBytesUtf8(byte[] bytes);
    }
}