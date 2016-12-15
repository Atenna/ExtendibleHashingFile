namespace ExtendibleHashingFile.DataStructure
{
    public class Record<T>
    {
        public int Hash { get; set; }
        public T Data { get; set; }

        public Record(int hash, T data)
        {
            Hash = hash;
            Data = data;
        }

        public override string ToString()
        {
            return Hash + ", " + Data.ToString();
        }
    }
}
