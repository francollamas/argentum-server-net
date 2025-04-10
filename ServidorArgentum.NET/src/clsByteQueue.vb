Option Strict On
Option Explicit On

Friend Class clsByteQueue
    ' Exception codes
    Private Const NOT_ENOUGH_DATA As Integer = vbObjectError + 9
    Private Const NOT_ENOUGH_SPACE As Integer = vbObjectError + 10
    Private Const DATA_BUFFER As Integer = 10240

    ' Modern classes using BitConverter instead of fixed arrays
    Private Class ByteConverter
        Public ByteArr As Byte()
        Public LongValue As Integer
        Public SingleValue As Single

        Public Sub New()
            ByteArr = New Byte(3) {}
        End Sub
    End Class

    Private Class DoubleConverter
        Public ByteArr As Byte()
        Public DoubleValue As Double

        Public Sub New()
            ByteArr = New Byte(7) {}
        End Sub
    End Class

    Private data() As Byte
    Private queueCapacity As Integer
    Private queueLength As Integer

    Public Sub New()
        data = New Byte(DATA_BUFFER - 1) {}
        queueCapacity = DATA_BUFFER
    End Sub

    Protected Overrides Sub Finalize()
        data = Nothing
        MyBase.Finalize()
    End Sub

    Private Function min(ByVal val1 As Integer, ByVal val2 As Integer) As Integer
        If val1 < val2 Then
            Return val1
        Else
            Return val2
        End If
    End Function

    Private Sub ByteArrayToType(ByRef destVariable As Object, ByRef sourceArray() As Byte, ByVal startPos As Integer, ByVal length As Integer)
        Select Case length
            Case 1
                destVariable = sourceArray(startPos)
            Case 2
                destVariable = BitConverter.ToInt16(sourceArray, startPos)
            Case 4
                If TypeOf destVariable Is Single Then
                    destVariable = BitConverter.ToSingle(sourceArray, startPos)
                Else
                    destVariable = BitConverter.ToInt32(sourceArray, startPos)
                End If
            Case 8
                destVariable = BitConverter.ToDouble(sourceArray, startPos)
        End Select
    End Sub

    Private Sub TypeToByteArray(ByRef destArray() As Byte, ByVal startPos As Integer, ByVal sourceVariable As Object)
        Dim valueType As Type = sourceVariable.GetType()

        If valueType Is GetType(Byte) Then
            destArray(startPos) = CByte(sourceVariable)
        ElseIf valueType Is GetType(Int16) Then
            Dim bytes As Byte() = BitConverter.GetBytes(CShort(sourceVariable))
            Array.Copy(bytes, 0, destArray, startPos, bytes.Length)
        ElseIf valueType Is GetType(Int32) Then
            Dim bytes As Byte() = BitConverter.GetBytes(CInt(sourceVariable))
            Array.Copy(bytes, 0, destArray, startPos, bytes.Length)
        ElseIf valueType Is GetType(Single) Then
            Dim bytes As Byte() = BitConverter.GetBytes(CSng(sourceVariable))
            Array.Copy(bytes, 0, destArray, startPos, bytes.Length)
        ElseIf valueType Is GetType(Double) Then
            Dim bytes As Byte() = BitConverter.GetBytes(CDbl(sourceVariable))
            Array.Copy(bytes, 0, destArray, startPos, bytes.Length)
        End If
    End Sub

    Private Sub CopyArrayData(ByRef destArray() As Byte, ByVal destStart As Integer, ByRef sourceArray() As Byte, ByVal sourceStart As Integer, ByVal length As Integer)
        Array.Copy(sourceArray, sourceStart, destArray, destStart, length)
    End Sub

    Private Function WriteData(ByRef buf() As Byte, ByVal dataLength As Integer) As Integer
        If queueCapacity - queueLength - dataLength < 0 Then
            Throw New InvalidOperationException("Not enough space in the queue")
        End If

        ' Ensure array has enough capacity
        If queueLength + dataLength > data.Length Then
            Array.Resize(Of Byte)(data, data.Length * 2)
            queueCapacity = data.Length
        End If

        CopyArrayData(data, queueLength, buf, 0, dataLength)

        queueLength = queueLength + dataLength
        Return dataLength
    End Function

    Private Function ReadData(ByRef buf() As Byte, ByVal dataLength As Integer) As Integer
        If dataLength > queueLength Then
            Throw New InvalidOperationException("Not enough data in the queue")
        End If

        CopyArrayData(buf, 0, data, 0, dataLength)
        Return dataLength
    End Function

    Private Function ReadDataWithOffset(ByRef buf() As Byte, ByVal dataLength As Integer, ByVal startPos As Integer) As Integer
        If dataLength > queueLength - startPos Then
            Throw New InvalidOperationException("Not enough data in the queue")
        End If

        CopyArrayData(buf, 0, data, startPos, dataLength)
        Return dataLength
    End Function

    Private Function RemoveData(ByVal dataLength As Integer) As Integer
        RemoveData = min(dataLength, queueLength)

        If RemoveData <> queueCapacity Then
            CopyArrayData(data, 0, data, RemoveData, queueLength - RemoveData)
        End If

        queueLength = queueLength - RemoveData
        Return RemoveData
    End Function

    ' Public methods - maintaining the same interface
    Public Function WriteByte(ByVal Value As Byte) As Integer
        Dim buf(0) As Byte

        buf(0) = Value

        Return WriteData(buf, 1)
    End Function

    Public Function WriteInteger(ByVal Value As Short) As Integer
        Dim buf(1) As Byte

        TypeToByteArray(buf, 0, Value)

        Return WriteData(buf, 2)
    End Function

    Public Function WriteLong(ByVal Value As Integer) As Integer
        Dim buf(3) As Byte

        TypeToByteArray(buf, 0, Value)

        Return WriteData(buf, 4)
    End Function

    Public Function WriteSingle(ByVal Value As Single) As Integer
        Dim buf(3) As Byte

        TypeToByteArray(buf, 0, Value)

        Return WriteData(buf, 4)
    End Function

    Public Function WriteDouble(ByVal Value As Double) As Integer
        Dim buf(7) As Byte

        TypeToByteArray(buf, 0, Value)

        Return WriteData(buf, 8)
    End Function

    Public Function WriteBoolean(ByVal Value As Boolean) As Integer
        Dim buf(0) As Byte

        If Value Then buf(0) = 1

        Return WriteData(buf, 1)
    End Function

    Public Function WriteASCIIStringFixed(ByVal Value As String) As Integer
        ' Handle null value
        If Value Is Nothing Then Value = String.Empty

        Dim bytes() As Byte = Text.Encoding.GetEncoding("Windows-1252").GetBytes(Value)
        Return WriteData(bytes, bytes.Length)
    End Function

    Public Function WriteUnicodeStringFixed(ByVal Value As String) As Integer
        ' Handle null value
        If Value Is Nothing Then Value = String.Empty

        Dim bytes() As Byte = Text.Encoding.Unicode.GetBytes(Value)
        Return WriteData(bytes, bytes.Length)
    End Function

    Public Function WriteASCIIString(ByVal Value As String) As Integer
        ' Handle null value
        If Value Is Nothing Then Value = String.Empty

        Dim length As Short = CShort(Value.Length)
        Dim lengthBytes As Byte() = BitConverter.GetBytes(length)
        Dim valueBytes As Byte() = Text.Encoding.GetEncoding("Windows-1252").GetBytes(Value)

        Dim combinedBytes(lengthBytes.Length + valueBytes.Length - 1) As Byte
        Array.Copy(lengthBytes, 0, combinedBytes, 0, lengthBytes.Length)
        Array.Copy(valueBytes, 0, combinedBytes, lengthBytes.Length, valueBytes.Length)

        Return WriteData(combinedBytes, combinedBytes.Length)
    End Function

    Public Function WriteUnicodeString(ByVal Value As String) As Integer
        ' Handle null value
        If Value Is Nothing Then Value = String.Empty

        Dim length As Short = CShort(Value.Length)
        Dim lengthBytes As Byte() = BitConverter.GetBytes(length)
        Dim valueBytes As Byte() = Text.Encoding.Unicode.GetBytes(Value)

        Dim combinedBytes(lengthBytes.Length + valueBytes.Length - 1) As Byte
        Array.Copy(lengthBytes, 0, combinedBytes, 0, lengthBytes.Length)
        Array.Copy(valueBytes, 0, combinedBytes, lengthBytes.Length, valueBytes.Length)

        Return WriteData(combinedBytes, combinedBytes.Length)
    End Function

    Public Function WriteBlock(ByRef Value() As Byte, Optional ByVal length As Integer = -1) As Integer
        ' Handle null array
        If Value Is Nothing Then
            Return 0
        End If

        If length > Value.Length Or length < 0 Then length = Value.Length
        Return WriteData(Value, length)
    End Function

    Public Function ReadByte() As Byte
        Dim buf(0) As Byte

        RemoveData(ReadData(buf, 1))

        Return buf(0)
    End Function

    Public Function ReadInteger() As Short
        Dim buf(1) As Byte
        Dim result As Short

        RemoveData(ReadData(buf, 2))

        result = BitConverter.ToInt16(buf, 0)
        Return result
    End Function

    Public Function ReadLong() As Integer
        Dim buf(3) As Byte
        Dim result As Integer

        RemoveData(ReadData(buf, 4))

        result = BitConverter.ToInt32(buf, 0)
        Return result
    End Function

    Public Function ReadSingle() As Single
        Dim buf(3) As Byte
        Dim result As Single

        RemoveData(ReadData(buf, 4))

        result = BitConverter.ToSingle(buf, 0)
        Return result
    End Function

    Public Function ReadDouble() As Double
        Dim buf(7) As Byte
        Dim result As Double

        RemoveData(ReadData(buf, 8))

        result = BitConverter.ToDouble(buf, 0)
        Return result
    End Function

    Public Function ReadBoolean() As Boolean
        Dim buf(0) As Byte

        RemoveData(ReadData(buf, 1))

        Return buf(0) = 1
    End Function

    Public Function ReadASCIIStringFixed(ByVal length As Integer) As String
        If length <= 0 Then Return String.Empty

        Dim buf(length - 1) As Byte

        If queueLength >= length Then
            RemoveData(ReadData(buf, length))
            Return Text.Encoding.GetEncoding("Windows-1252").GetString(buf)
        Else
            Throw New InvalidOperationException("Not enough data in the queue")
        End If
    End Function

    Public Function ReadUnicodeStringFixed(ByVal length As Integer) As String
        If length <= 0 Then Return String.Empty

        Dim byteLength As Integer = length * 2
        Dim buf(byteLength - 1) As Byte

        If queueLength >= byteLength Then
            RemoveData(ReadData(buf, byteLength))
            Return Text.Encoding.Unicode.GetString(buf)
        Else
            Throw New InvalidOperationException("Not enough data in the queue")
        End If
    End Function

    Public Function ReadASCIIString() As String
        If queueLength <= 1 Then
            Throw New InvalidOperationException("Not enough data in the queue")
        End If

        Dim lengthBuf(1) As Byte
        ReadData(lengthBuf, 2)
        Dim length As Short = BitConverter.ToInt16(lengthBuf, 0)

        If queueLength >= length + 2 Then
            RemoveData(2)

            If length > 0 Then
                Dim buf(length - 1) As Byte
                RemoveData(ReadData(buf, length))
                Return Text.Encoding.GetEncoding("Windows-1252").GetString(buf)
            Else
                Return String.Empty
            End If
        Else
            Throw New InvalidOperationException("Not enough data in the queue")
        End If
    End Function

    Public Function ReadUnicodeString() As String
        If queueLength <= 1 Then
            Throw New InvalidOperationException("Not enough data in the queue")
        End If

        Dim lengthBuf(1) As Byte
        ReadData(lengthBuf, 2)
        Dim length As Short = BitConverter.ToInt16(lengthBuf, 0)
        Dim byteLength As Integer = length * 2

        If queueLength >= byteLength + 2 Then
            RemoveData(2)

            If length > 0 Then
                Dim buf(byteLength - 1) As Byte
                RemoveData(ReadData(buf, byteLength))
                Return Text.Encoding.Unicode.GetString(buf)
            Else
                Return String.Empty
            End If
        Else
            Throw New InvalidOperationException("Not enough data in the queue")
        End If
    End Function

    Public Function ReadBlock(ByRef block() As Byte, ByVal dataLength As Integer) As Integer
        ' Check if block array is null or not initialized
        If block Is Nothing OrElse block.Length = 0 Then
            Return 0
        End If

        If dataLength > 0 Then
            Return RemoveData(ReadData(block, dataLength))
        Else
            Return 0
        End If
    End Function

    Public Function PeekByte() As Byte
        Dim buf(0) As Byte

        ReadData(buf, 1)

        Return buf(0)
    End Function

    Public Function PeekInteger() As Short
        Dim buf(1) As Byte

        ReadData(buf, 2)

        Return BitConverter.ToInt16(buf, 0)
    End Function

    Public Function PeekLong() As Integer
        Dim buf(3) As Byte

        ReadData(buf, 4)

        Return BitConverter.ToInt32(buf, 0)
    End Function

    Public Function PeekSingle() As Single
        Dim buf(3) As Byte

        ReadData(buf, 4)

        Return BitConverter.ToSingle(buf, 0)
    End Function

    Public Function PeekDouble() As Double
        Dim buf(7) As Byte

        ReadData(buf, 8)

        Return BitConverter.ToDouble(buf, 0)
    End Function

    Public Function PeekBoolean() As Boolean
        Dim buf(0) As Byte

        ReadData(buf, 1)

        Return buf(0) = 1
    End Function

    Public Function PeekASCIIStringFixed(ByVal length As Integer) As String
        If length <= 0 Then Return String.Empty

        Dim buf(length - 1) As Byte

        If queueLength >= length Then
            ReadData(buf, length)
            Return Text.Encoding.GetEncoding("Windows-1252").GetString(buf)
        Else
            Throw New InvalidOperationException("Not enough data in the queue")
        End If
    End Function

    Public Function PeekUnicodeStringFixed(ByVal length As Integer) As String
        If length <= 0 Then Return String.Empty

        Dim byteLength As Integer = length * 2
        Dim buf(byteLength - 1) As Byte

        If queueLength >= byteLength Then
            ReadData(buf, byteLength)
            Return Text.Encoding.Unicode.GetString(buf)
        Else
            Throw New InvalidOperationException("Not enough data in the queue")
        End If
    End Function

    Public Function PeekASCIIString() As String
        If queueLength <= 1 Then
            Throw New InvalidOperationException("Not enough data in the queue")
        End If

        Dim lengthBuf(1) As Byte
        ReadData(lengthBuf, 2)
        Dim length As Short = BitConverter.ToInt16(lengthBuf, 0)

        If queueLength >= length + 2 Then
            If length > 0 Then
                Dim buf(length - 1) As Byte
                ReadDataWithOffset(buf, length, 2)
                Return Text.Encoding.GetEncoding("Windows-1252").GetString(buf)
            Else
                Return String.Empty
            End If
        Else
            Throw New InvalidOperationException("Not enough data in the queue")
        End If
    End Function

    Public Function PeekUnicodeString() As String
        If queueLength <= 1 Then
            Throw New InvalidOperationException("Not enough data in the queue")
        End If

        Dim lengthBuf(1) As Byte
        ReadData(lengthBuf, 2)
        Dim length As Short = BitConverter.ToInt16(lengthBuf, 0)
        Dim byteLength As Integer = length * 2

        If queueLength >= byteLength + 2 Then
            If length > 0 Then
                Dim buf(byteLength - 1) As Byte
                ReadDataWithOffset(buf, byteLength, 2)
                Return Text.Encoding.Unicode.GetString(buf)
            Else
                Return String.Empty
            End If
        Else
            Throw New InvalidOperationException("Not enough data in the queue")
        End If
    End Function

    Public Function PeekBlock(ByRef block() As Byte, ByVal dataLength As Integer) As Integer
        ' Check if block array is null or not initialized
        If block Is Nothing OrElse block.Length = 0 Then
            Return 0
        End If

        If dataLength > 0 Then
            Return ReadData(block, dataLength)
        Else
            Return 0
        End If
    End Function

    Public Sub CopyBuffer(ByRef Source As clsByteQueue)
        ' Check if source is null
        If Source Is Nothing Then
            Exit Sub
        End If

        If Source.length = 0 Then
            RemoveData(length)
            Exit Sub
        End If

        queueCapacity = Source.Capacity

        ReDim data(queueCapacity - 1)

        Dim buf(Source.length - 1) As Byte

        Source.PeekBlock(buf, Source.length)

        queueLength = 0

        WriteBlock(buf, Source.length)
    End Sub

    Public ReadOnly Property length() As Integer
        Get
            Return queueLength
        End Get
    End Property

    Public ReadOnly Property Capacity() As Integer
        Get
            Return queueCapacity
        End Get
    End Property

    Public ReadOnly Property NotEnoughDataErrCode() As Integer
        Get
            Return NOT_ENOUGH_DATA
        End Get
    End Property

    Public ReadOnly Property NotEnoughSpaceErrCode() As Integer
        Get
            Return NOT_ENOUGH_SPACE
        End Get
    End Property
End Class
