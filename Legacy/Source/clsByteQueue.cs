using System;
using System.Text;

namespace Legacy
{

    internal class clsByteQueue
    {
        // Exception codes
        private const int NOT_ENOUGH_DATA = -2147221495;  // vbObjectError + 9
        private const int NOT_ENOUGH_SPACE = -2147221494;  // vbObjectError + 10
        private const int DATA_BUFFER = 10240;

        // Modern classes using BitConverter instead of fixed arrays
        private class ByteConverter
        {
            public byte[] ByteArr;
            public int LongValue;
            public float SingleValue;

            public ByteConverter()
            {
                ByteArr = new byte[4];
            }
        }

        private class DoubleConverter
        {
            public byte[] ByteArr;
            public double DoubleValue;

            public DoubleConverter()
            {
                ByteArr = new byte[8];
            }
        }

        private byte[] data;
        private int queueCapacity;
        private int queueLength;

        public clsByteQueue()
        {
            data = new byte[10240];
            queueCapacity = DATA_BUFFER;
        }

        ~clsByteQueue()
        {
            data = null;
        }

        private int min(int val1, int val2)
        {
            if (val1 < val2)
            {
                return val1;
            }
            else
            {
                return val2;
            }
        }

        private void ByteArrayToType(ref object destVariable, ref byte[] sourceArray, int startPos, int length)
        {
            switch (length)
            {
                case 1:
                    {
                        destVariable = sourceArray[startPos];
                        break;
                    }
                case 2:
                    {
                        destVariable = BitConverter.ToInt16(sourceArray, startPos);
                        break;
                    }
                case 4:
                    {
                        if (destVariable is float)
                        {
                            destVariable = BitConverter.ToSingle(sourceArray, startPos);
                        }
                        else
                        {
                            destVariable = BitConverter.ToInt32(sourceArray, startPos);
                        }

                        break;
                    }
                case 8:
                    {
                        destVariable = BitConverter.ToDouble(sourceArray, startPos);
                        break;
                    }
            }
        }

        private void TypeToByteArray(ref byte[] destArray, int startPos, object sourceVariable)
        {
            var valueType = sourceVariable.GetType();

            if (ReferenceEquals(valueType, typeof(byte)))
            {
                destArray[startPos] = Convert.ToByte(sourceVariable);
            }
            else if (ReferenceEquals(valueType, typeof(short)))
            {
                byte[] bytes = BitConverter.GetBytes(Convert.ToInt16(sourceVariable));
                Array.Copy(bytes, 0, destArray, startPos, bytes.Length);
            }
            else if (ReferenceEquals(valueType, typeof(int)))
            {
                byte[] bytes = BitConverter.GetBytes(Convert.ToInt32(sourceVariable));
                Array.Copy(bytes, 0, destArray, startPos, bytes.Length);
            }
            else if (ReferenceEquals(valueType, typeof(float)))
            {
                byte[] bytes = BitConverter.GetBytes(Convert.ToSingle(sourceVariable));
                Array.Copy(bytes, 0, destArray, startPos, bytes.Length);
            }
            else if (ReferenceEquals(valueType, typeof(double)))
            {
                byte[] bytes = BitConverter.GetBytes(Convert.ToDouble(sourceVariable));
                Array.Copy(bytes, 0, destArray, startPos, bytes.Length);
            }
        }

        private void CopyArrayData(ref byte[] destArray, int destStart, ref byte[] sourceArray, int sourceStart, int length)
        {
            Array.Copy(sourceArray, sourceStart, destArray, destStart, length);
        }

        private int WriteData(ref byte[] buf, int dataLength)
        {
            if (queueCapacity - queueLength - dataLength < 0)
            {
                throw new InvalidOperationException("Not enough space in the queue");
            }

            // Ensure array has enough capacity
            if (queueLength + dataLength > data.Length)
            {
                Array.Resize(ref data, data.Length * 2);
                queueCapacity = data.Length;
            }

            CopyArrayData(ref data, queueLength, ref buf, 0, dataLength);

            queueLength = queueLength + dataLength;
            return dataLength;
        }

        private int ReadData(ref byte[] buf, int dataLength)
        {
            if (dataLength > queueLength)
            {
                throw new InvalidOperationException("Not enough data in the queue");
            }

            CopyArrayData(ref buf, 0, ref data, 0, dataLength);
            return dataLength;
        }

        private int ReadDataWithOffset(ref byte[] buf, int dataLength, int startPos)
        {
            if (dataLength > queueLength - startPos)
            {
                throw new InvalidOperationException("Not enough data in the queue");
            }

            CopyArrayData(ref buf, 0, ref data, startPos, dataLength);
            return dataLength;
        }

        private int RemoveData(int dataLength)
        {
            int RemoveDataRet = default;
            RemoveDataRet = min(dataLength, queueLength);

            if (RemoveDataRet != queueCapacity)
            {
                CopyArrayData(ref data, 0, ref data, RemoveDataRet, queueLength - RemoveDataRet);
            }

            queueLength = queueLength - RemoveDataRet;
            return RemoveDataRet;
        }

        // Public methods - maintaining the same interface
        public int WriteByte(byte Value)
        {
            var buf = new byte[1];

            buf[0] = Value;

            return WriteData(ref buf, 1);
        }

        public int WriteInteger(short Value)
        {
            var buf = new byte[2];

            TypeToByteArray(ref buf, 0, Value);

            return WriteData(ref buf, 2);
        }

        public int WriteLong(int Value)
        {
            var buf = new byte[4];

            TypeToByteArray(ref buf, 0, Value);

            return WriteData(ref buf, 4);
        }

        public int WriteSingle(float Value)
        {
            var buf = new byte[4];

            TypeToByteArray(ref buf, 0, Value);

            return WriteData(ref buf, 4);
        }

        public int WriteDouble(double Value)
        {
            var buf = new byte[8];

            TypeToByteArray(ref buf, 0, Value);

            return WriteData(ref buf, 8);
        }

        public int WriteBoolean(bool Value)
        {
            var buf = new byte[1];

            if (Value)
                buf[0] = 1;

            return WriteData(ref buf, 1);
        }

        public int WriteASCIIStringFixed(string Value)
        {
            // Handle null value
            if (Value is null)
                Value = string.Empty;

            byte[] bytes = Encoding.GetEncoding("Windows-1252").GetBytes(Value);
            return WriteData(ref bytes, bytes.Length);
        }

        public int WriteUnicodeStringFixed(string Value)
        {
            // Handle null value
            if (Value is null)
                Value = string.Empty;

            byte[] bytes = Encoding.Unicode.GetBytes(Value);
            return WriteData(ref bytes, bytes.Length);
        }

        public int WriteASCIIString(string Value)
        {
            // Handle null value
            if (Value is null)
                Value = string.Empty;

            short length = Convert.ToInt16(Value.Length);
            byte[] lengthBytes = BitConverter.GetBytes(length);
            byte[] valueBytes = Encoding.GetEncoding("Windows-1252").GetBytes(Value);

            var combinedBytes = new byte[(lengthBytes.Length + valueBytes.Length)];
            Array.Copy(lengthBytes, 0, combinedBytes, 0, lengthBytes.Length);
            Array.Copy(valueBytes, 0, combinedBytes, lengthBytes.Length, valueBytes.Length);

            return WriteData(ref combinedBytes, combinedBytes.Length);
        }

        public int WriteUnicodeString(string Value)
        {
            // Handle null value
            if (Value is null)
                Value = string.Empty;

            short length = Convert.ToInt16(Value.Length);
            byte[] lengthBytes = BitConverter.GetBytes(length);
            byte[] valueBytes = Encoding.Unicode.GetBytes(Value);

            var combinedBytes = new byte[(lengthBytes.Length + valueBytes.Length)];
            Array.Copy(lengthBytes, 0, combinedBytes, 0, lengthBytes.Length);
            Array.Copy(valueBytes, 0, combinedBytes, lengthBytes.Length, valueBytes.Length);

            return WriteData(ref combinedBytes, combinedBytes.Length);
        }

        public int WriteBlock(ref byte[] Value, int length = -1)
        {
            // Handle null array
            if (Value is null)
            {
                return 0;
            }

            if (length > Value.Length | length < 0)
                length = Value.Length;
            return WriteData(ref Value, length);
        }

        public byte ReadByte()
        {
            var buf = new byte[1];

            RemoveData(ReadData(ref buf, 1));

            return buf[0];
        }

        public short ReadInteger()
        {
            var buf = new byte[2];
            short result;

            RemoveData(ReadData(ref buf, 2));

            result = BitConverter.ToInt16(buf, 0);
            return result;
        }

        public int ReadLong()
        {
            var buf = new byte[4];
            int result;

            RemoveData(ReadData(ref buf, 4));

            result = BitConverter.ToInt32(buf, 0);
            return result;
        }

        public float ReadSingle()
        {
            var buf = new byte[4];
            float result;

            RemoveData(ReadData(ref buf, 4));

            result = BitConverter.ToSingle(buf, 0);
            return result;
        }

        public double ReadDouble()
        {
            var buf = new byte[8];
            double result;

            RemoveData(ReadData(ref buf, 8));

            result = BitConverter.ToDouble(buf, 0);
            return result;
        }

        public bool ReadBoolean()
        {
            var buf = new byte[1];

            RemoveData(ReadData(ref buf, 1));

            return buf[0] == 1;
        }

        public string ReadASCIIStringFixed(int length)
        {
            if (length <= 0)
                return string.Empty;

            var buf = new byte[length];

            if (queueLength >= length)
            {
                RemoveData(ReadData(ref buf, length));
                return Encoding.GetEncoding("Windows-1252").GetString(buf);
            }
            else
            {
                throw new InvalidOperationException("Not enough data in the queue");
            }
        }

        public string ReadUnicodeStringFixed(int length)
        {
            if (length <= 0)
                return string.Empty;

            int byteLength = length * 2;
            var buf = new byte[byteLength];

            if (queueLength >= byteLength)
            {
                RemoveData(ReadData(ref buf, byteLength));
                return Encoding.Unicode.GetString(buf);
            }
            else
            {
                throw new InvalidOperationException("Not enough data in the queue");
            }
        }

        public string ReadASCIIString()
        {
            if (queueLength <= 1)
            {
                throw new InvalidOperationException("Not enough data in the queue");
            }

            var lengthBuf = new byte[2];
            ReadData(ref lengthBuf, 2);
            short length = BitConverter.ToInt16(lengthBuf, 0);

            if (queueLength >= length + 2)
            {
                RemoveData(2);

                if (length > 0)
                {
                    var buf = new byte[length];
                    RemoveData(ReadData(ref buf, length));
                    return Encoding.GetEncoding("Windows-1252").GetString(buf);
                }
                else
                {
                    return string.Empty;
                }
            }
            else
            {
                throw new InvalidOperationException("Not enough data in the queue");
            }
        }

        public string ReadUnicodeString()
        {
            if (queueLength <= 1)
            {
                throw new InvalidOperationException("Not enough data in the queue");
            }

            var lengthBuf = new byte[2];
            ReadData(ref lengthBuf, 2);
            short length = BitConverter.ToInt16(lengthBuf, 0);
            int byteLength = length * 2;

            if (queueLength >= byteLength + 2)
            {
                RemoveData(2);

                if (length > 0)
                {
                    var buf = new byte[byteLength];
                    RemoveData(ReadData(ref buf, byteLength));
                    return Encoding.Unicode.GetString(buf);
                }
                else
                {
                    return string.Empty;
                }
            }
            else
            {
                throw new InvalidOperationException("Not enough data in the queue");
            }
        }

        public int ReadBlock(ref byte[] block, int dataLength)
        {
            // Check if block array is null or not initialized
            if (block is null || block.Length == 0)
            {
                return 0;
            }

            if (dataLength > 0)
            {
                return RemoveData(ReadData(ref block, dataLength));
            }
            else
            {
                return 0;
            }
        }

        public byte PeekByte()
        {
            var buf = new byte[1];

            ReadData(ref buf, 1);

            return buf[0];
        }

        public short PeekInteger()
        {
            var buf = new byte[2];

            ReadData(ref buf, 2);

            return BitConverter.ToInt16(buf, 0);
        }

        public int PeekLong()
        {
            var buf = new byte[4];

            ReadData(ref buf, 4);

            return BitConverter.ToInt32(buf, 0);
        }

        public float PeekSingle()
        {
            var buf = new byte[4];

            ReadData(ref buf, 4);

            return BitConverter.ToSingle(buf, 0);
        }

        public double PeekDouble()
        {
            var buf = new byte[8];

            ReadData(ref buf, 8);

            return BitConverter.ToDouble(buf, 0);
        }

        public bool PeekBoolean()
        {
            var buf = new byte[1];

            ReadData(ref buf, 1);

            return buf[0] == 1;
        }

        public string PeekASCIIStringFixed(int length)
        {
            if (length <= 0)
                return string.Empty;

            var buf = new byte[length];

            if (queueLength >= length)
            {
                ReadData(ref buf, length);
                return Encoding.GetEncoding("Windows-1252").GetString(buf);
            }
            else
            {
                throw new InvalidOperationException("Not enough data in the queue");
            }
        }

        public string PeekUnicodeStringFixed(int length)
        {
            if (length <= 0)
                return string.Empty;

            int byteLength = length * 2;
            var buf = new byte[byteLength];

            if (queueLength >= byteLength)
            {
                ReadData(ref buf, byteLength);
                return Encoding.Unicode.GetString(buf);
            }
            else
            {
                throw new InvalidOperationException("Not enough data in the queue");
            }
        }

        public string PeekASCIIString()
        {
            if (queueLength <= 1)
            {
                throw new InvalidOperationException("Not enough data in the queue");
            }

            var lengthBuf = new byte[2];
            ReadData(ref lengthBuf, 2);
            short length = BitConverter.ToInt16(lengthBuf, 0);

            if (queueLength >= length + 2)
            {
                if (length > 0)
                {
                    var buf = new byte[length];
                    ReadDataWithOffset(ref buf, length, 2);
                    return Encoding.GetEncoding("Windows-1252").GetString(buf);
                }
                else
                {
                    return string.Empty;
                }
            }
            else
            {
                throw new InvalidOperationException("Not enough data in the queue");
            }
        }

        public string PeekUnicodeString()
        {
            if (queueLength <= 1)
            {
                throw new InvalidOperationException("Not enough data in the queue");
            }

            var lengthBuf = new byte[2];
            ReadData(ref lengthBuf, 2);
            short length = BitConverter.ToInt16(lengthBuf, 0);
            int byteLength = length * 2;

            if (queueLength >= byteLength + 2)
            {
                if (length > 0)
                {
                    var buf = new byte[byteLength];
                    ReadDataWithOffset(ref buf, byteLength, 2);
                    return Encoding.Unicode.GetString(buf);
                }
                else
                {
                    return string.Empty;
                }
            }
            else
            {
                throw new InvalidOperationException("Not enough data in the queue");
            }
        }

        public int PeekBlock(ref byte[] block, int dataLength)
        {
            // Check if block array is null or not initialized
            if (block is null || block.Length == 0)
            {
                return 0;
            }

            if (dataLength > 0)
            {
                return ReadData(ref block, dataLength);
            }
            else
            {
                return 0;
            }
        }

        public void CopyBuffer(ref clsByteQueue Source)
        {
            // Check if source is null
            if (Source is null)
            {
                return;
            }

            if (Source.length == 0)
            {
                RemoveData(length);
                return;
            }

            queueCapacity = Source.Capacity;

            data = new byte[queueCapacity];

            var buf = new byte[Source.length];

            Source.PeekBlock(ref buf, Source.length);

            queueLength = 0;

            WriteBlock(ref buf, Source.length);
        }

        public int length
        {
            get
            {
                return queueLength;
            }
        }

        public int Capacity
        {
            get
            {
                return queueCapacity;
            }
        }

        public int NotEnoughDataErrCode
        {
            get
            {
                return NOT_ENOUGH_DATA;
            }
        }

        public int NotEnoughSpaceErrCode
        {
            get
            {
                return NOT_ENOUGH_SPACE;
            }
        }
    }
}