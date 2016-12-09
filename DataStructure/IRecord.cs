using System.Collections;

namespace ExtendibleHashingFile.DataStructure
{
    public interface IRecord<in T>
    {
        bool Equal(T data);
        BitArray HashCode();
        byte[] ToByteArray();
        void FromByteArray(byte[] arr, int offset);
        int Size();
        string ToString();
    }
}
