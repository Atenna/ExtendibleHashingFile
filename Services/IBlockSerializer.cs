namespace ExtendibleHashingFile.DataStructure
{
    public interface IBlockSerializer<T>
    {
        int BlockSize { get; }
        byte[] Serialize(T data);
        T Deserialize(byte[] array);
    }
}
