namespace ExtendibleHashingFile.DataStructure
{
    public interface IBlockStorage<T>
    {
        int Count { get; }
        Block<T> Read(int index);
        int ReadDepth(int index);
        void Write(Block<T> block, int index);
        int Add(Block<T> block);
        void RemoveAt(int index);
    }
}
