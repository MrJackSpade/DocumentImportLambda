using System.Data;
using System.Diagnostics.CodeAnalysis;

namespace DocumentImportLambda.Tests.Mocks.Dummy
{
    internal class DummyDataReader : IDataReader
    {
        public int Depth => 0;

        public int FieldCount => 0;

        public bool IsClosed => false;

        public int RecordsAffected => 0;

        public object this[int i] => new();

        public object this[string name] => new();

        public void Close()
        {
        }

        public void Dispose()
        {
        }

        public bool GetBoolean(int i)
        {
            return false;
        }

        public byte GetByte(int i)
        {
            return 0;
        }

        public long GetBytes(int i, long fieldOffset, byte[]? buffer, int bufferoffset, int length)
        {
            return 0;
        }

        public char GetChar(int i)
        {
            return '\0';
        }

        public long GetChars(int i, long fieldoffset, char[]? buffer, int bufferoffset, int length)
        {
            return 0;
        }

        public IDataReader GetData(int i)
        {
            return new DummyDataReader();
        }

        public string GetDataTypeName(int i)
        {
            return string.Empty;
        }

        public DateTime GetDateTime(int i)
        {
            return DateTime.MinValue;
        }

        public decimal GetDecimal(int i)
        {
            return decimal.Zero;
        }

        public double GetDouble(int i)
        {
            return 0;
        }

        [return: DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicFields | DynamicallyAccessedMemberTypes.PublicProperties)]
        public Type GetFieldType(int i)
        {
            return typeof(object);
        }

        public float GetFloat(int i)
        {
            return 0;
        }

        public Guid GetGuid(int i)
        {
            return Guid.Empty;
        }

        public short GetInt16(int i)
        {
            return 0;
        }

        public int GetInt32(int i)
        {
            return 0;
        }

        public long GetInt64(int i)
        {
            return 0;
        }

        public string GetName(int i)
        {
            return string.Empty;
        }

        public int GetOrdinal(string name)
        {
            return -1;
        }

        public DataTable? GetSchemaTable()
        {
            return null;
        }

        public string GetString(int i)
        {
            return string.Empty;
        }

        public object GetValue(int i)
        {
            return new object();
        }

        public int GetValues(object[] values)
        {
            return 0;
        }

        public bool IsDBNull(int i)
        {
            return true;
        }

        public bool NextResult()
        {
            return false;
        }

        public bool Read()
        {
            return false;
        }
    }
}