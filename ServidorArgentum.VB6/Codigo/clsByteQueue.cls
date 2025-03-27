VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
  Persistable = 0  'NotPersistable
  DataBindingBehavior = 0  'vbNone
  DataSourceBehavior  = 0  'vbNone
  MTSTransactionMode  = 0  'NotAnMTSObject
END
Attribute VB_Name = "clsByteQueue"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = True
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = False
' TODO MIGRA: debe ser muy poco performante. Cambiar por otra cosa apenas se pueda
Option Explicit
Option Base 0

Private Const NOT_ENOUGH_DATA As Long = vbObjectError + 9
Private Const NOT_ENOUGH_SPACE As Long = vbObjectError + 10
Private Const DATA_BUFFER As Long = 10240

Private Type ByteConverter
    ByteArr(3) As Byte
    LongValue As Long
    SingleValue As Single
End Type

Private Type DoubleConverter
    ByteArr(7) As Byte
    DoubleValue As Double
End Type

Dim data() As Byte
Dim queueCapacity As Long
Dim queueLength As Long

Private Sub Class_Initialize()
    ReDim data(DATA_BUFFER - 1) As Byte
    queueCapacity = DATA_BUFFER
End Sub

Private Sub Class_Terminate()
    Erase data
End Sub

Private Function min(ByVal val1 As Long, ByVal val2 As Long) As Long
    If val1 < val2 Then
        min = val1
    Else
        min = val2
    End If
End Function

Private Sub ByteArrayToType(ByRef destVariable As Variant, ByRef sourceArray() As Byte, ByVal startPos As Long, ByVal length As Long)
    Dim i As Long
    Dim tempArray() As Byte
    Dim tempBC As ByteConverter
    Dim tempDC As DoubleConverter
    
    ReDim tempArray(length - 1) As Byte
    
    For i = 0 To length - 1
        tempArray(i) = sourceArray(startPos + i)
    Next i
    
    Select Case length
        Case 1: destVariable = tempArray(0)
        Case 2: destVariable = CInt(tempArray(0)) + CInt(tempArray(1)) * 256
        Case 4:
            If VarType(destVariable) = vbSingle Then
                For i = 0 To 3
                    tempBC.ByteArr(i) = tempArray(i)
                Next i
                destVariable = tempBC.SingleValue
            Else
                destVariable = CLng(tempArray(0)) + CLng(tempArray(1)) * 256& + CLng(tempArray(2)) * 65536 + CLng(tempArray(3)) * 16777216
            End If
        Case 8:
            For i = 0 To 7
                tempDC.ByteArr(i) = tempArray(i)
            Next i
            destVariable = tempDC.DoubleValue
    End Select
End Sub

Private Sub TypeToByteArray(ByRef destArray() As Byte, ByVal startPos As Long, ByVal sourceVariable As Variant)
    Dim i As Long
    Dim valueType As VbVarType
    Dim tempBC As ByteConverter
    Dim tempDC As DoubleConverter
    
    valueType = VarType(sourceVariable)
    
    Select Case valueType
        Case vbByte:
            destArray(startPos) = CByte(sourceVariable)
        Case vbInteger:
            Dim tempInt As Integer
            tempInt = CInt(sourceVariable)
            destArray(startPos) = CByte(tempInt And &HFF&)
            destArray(startPos + 1) = CByte((CLng(tempInt) And &HFF00&) \ 256&)
        Case vbLong:
            Dim tempLong As Long
            tempLong = CLng(sourceVariable)
            destArray(startPos) = CByte(tempLong And &HFF&)
            destArray(startPos + 1) = CByte((tempLong And &HFF00&) \ 256&)
            destArray(startPos + 2) = CByte((tempLong And &HFF0000) \ 65536)
            destArray(startPos + 3) = CByte((tempLong And &HFF000000) \ 16777216)
        Case vbSingle:
            tempBC.SingleValue = CSng(sourceVariable)
            For i = 0 To 3
                destArray(startPos + i) = tempBC.ByteArr(i)
            Next i
        Case vbDouble:
            tempDC.DoubleValue = CDbl(sourceVariable)
            For i = 0 To 7
                destArray(startPos + i) = tempDC.ByteArr(i)
            Next i
    End Select
End Sub

Private Sub CopyArrayData(ByRef destArray() As Byte, ByVal destStart As Long, ByRef sourceArray() As Byte, ByVal sourceStart As Long, ByVal length As Long)
    Dim i As Long
    
    For i = 0 To length - 1
        destArray(destStart + i) = sourceArray(sourceStart + i)
    Next i
End Sub

Private Function WriteData(ByRef buf() As Byte, ByVal dataLength As Long) As Long
    If queueCapacity - queueLength - dataLength < 0 Then
        Call Err.Raise(NOT_ENOUGH_SPACE)
        Exit Function
    End If
    
    Call CopyArrayData(data, queueLength, buf, 0, dataLength)
    
    queueLength = queueLength + dataLength
    WriteData = dataLength
End Function

Private Function ReadData(ByRef buf() As Byte, ByVal dataLength As Long) As Long
    If dataLength > queueLength Then
        Call Err.Raise(NOT_ENOUGH_DATA)
        Exit Function
    End If
    
    Call CopyArrayData(buf, 0, data, 0, dataLength)
    ReadData = dataLength
End Function

Private Function ReadDataWithOffset(ByRef buf() As Byte, ByVal dataLength As Long, ByVal startPos As Long) As Long
    If dataLength > queueLength - startPos Then
        Call Err.Raise(NOT_ENOUGH_DATA)
        Exit Function
    End If
    
    Call CopyArrayData(buf, 0, data, startPos, dataLength)
    ReadDataWithOffset = dataLength
End Function

Private Function RemoveData(ByVal dataLength As Long) As Long
    RemoveData = min(dataLength, queueLength)
    
    If RemoveData <> queueCapacity Then
        Call CopyArrayData(data, 0, data, RemoveData, queueLength - RemoveData)
    End If
    
    queueLength = queueLength - RemoveData
End Function

Public Function WriteByte(ByVal value As Byte) As Long
    Dim buf(0) As Byte
    
    buf(0) = value
    
    WriteByte = WriteData(buf, 1)
End Function

Public Function WriteInteger(ByVal value As Integer) As Long
    Dim buf(1) As Byte
    
    Call TypeToByteArray(buf, 0, value)
    
    WriteInteger = WriteData(buf, 2)
End Function

Public Function WriteLong(ByVal value As Long) As Long
    Dim buf(3) As Byte
    
    Call TypeToByteArray(buf, 0, value)
    
    WriteLong = WriteData(buf, 4)
End Function

Public Function WriteSingle(ByVal value As Single) As Long
    Dim buf(3) As Byte
    
    Call TypeToByteArray(buf, 0, value)
    
    WriteSingle = WriteData(buf, 4)
End Function

Public Function WriteDouble(ByVal value As Double) As Long
    Dim buf(7) As Byte
    
    Call TypeToByteArray(buf, 0, value)
    
    WriteDouble = WriteData(buf, 8)
End Function

Public Function WriteBoolean(ByVal value As Boolean) As Long
    Dim buf(0) As Byte
    
    If value Then buf(0) = 1
    
    WriteBoolean = WriteData(buf, 1)
End Function

Public Function WriteASCIIStringFixed(ByVal value As String) As Long
    Dim buf() As Byte
    Dim i As Long, j As Long
    
    ReDim buf(Len(value) - 1) As Byte
    
    For i = 1 To Len(value)
        buf(i - 1) = Asc(mid$(value, i, 1))
    Next i
    
    WriteASCIIStringFixed = WriteData(buf, Len(value))
End Function

Public Function WriteUnicodeStringFixed(ByVal value As String) As Long
    Dim buf() As Byte
    Dim i As Long, j As Long
    Dim tempStr As String
    
    ReDim buf(LenB(value) - 1) As Byte
    
    j = 0
    For i = 1 To Len(value)
        tempStr = mid$(value, i, 1)
        buf(j) = Asc(tempStr) And &HFF&
        buf(j + 1) = (Asc(tempStr) And &HFF00&) \ 256&
        j = j + 2
    Next i
    
    WriteUnicodeStringFixed = WriteData(buf, LenB(value))
End Function

Public Function WriteASCIIString(ByVal value As String) As Long
    Dim buf() As Byte
    Dim i As Long
    Dim length As Integer
    
    length = Len(value)
    ReDim buf(length + 1) As Byte
    
    Call TypeToByteArray(buf, 0, length)
    
    If length > 0 Then
        For i = 1 To length
            buf(i + 1) = Asc(mid$(value, i, 1))
        Next i
    End If
    
    WriteASCIIString = WriteData(buf, length + 2)
End Function

Public Function WriteUnicodeString(ByVal value As String) As Long
    Dim buf() As Byte
    Dim i As Long, j As Long
    Dim tempStr As String
    Dim length As Integer
    
    length = Len(value)
    ReDim buf(length * 2 + 1) As Byte
    
    Call TypeToByteArray(buf, 0, length)
    
    If length > 0 Then
        j = 2
        For i = 1 To length
            tempStr = mid$(value, i, 1)
            buf(j) = Asc(tempStr) And &HFF&
            buf(j + 1) = (Asc(tempStr) And &HFF00&) \ 256&
            j = j + 2
        Next i
    End If
    
    WriteUnicodeString = WriteData(buf, length * 2 + 2)
End Function

Public Function WriteBlock(ByRef value() As Byte, Optional ByVal length As Long = -1) As Long
    If length > UBound(value()) + 1 Or length < 0 Then length = UBound(value()) + 1
    
    WriteBlock = WriteData(value, length)
End Function

Public Function ReadByte() As Byte
    Dim buf(0) As Byte
    
    Call RemoveData(ReadData(buf, 1))
    
    ReadByte = buf(0)
End Function

Public Function ReadInteger() As Integer
    Dim buf(1) As Byte
    Dim result As Integer
    
    Call RemoveData(ReadData(buf, 2))
    
    Call ByteArrayToType(result, buf, 0, 2)
    ReadInteger = result
End Function

Public Function ReadLong() As Long
    Dim buf(3) As Byte
    Dim result As Long
    
    Call RemoveData(ReadData(buf, 4))
    
    Call ByteArrayToType(result, buf, 0, 4)
    ReadLong = result
End Function

Public Function ReadSingle() As Single
    Dim buf(3) As Byte
    Dim result As Single
    
    Call RemoveData(ReadData(buf, 4))
    
    Call ByteArrayToType(result, buf, 0, 4)
    ReadSingle = result
End Function

Public Function ReadDouble() As Double
    Dim buf(7) As Byte
    Dim result As Double
    
    Call RemoveData(ReadData(buf, 8))
    
    Call ByteArrayToType(result, buf, 0, 8)
    ReadDouble = result
End Function

Public Function ReadBoolean() As Boolean
    Dim buf(0) As Byte
    
    Call RemoveData(ReadData(buf, 1))
    
    If buf(0) = 1 Then ReadBoolean = True
End Function

Public Function ReadASCIIStringFixed(ByVal length As Long) As String
    If length <= 0 Then Exit Function
    
    If queueLength >= length Then
        Dim buf() As Byte
        Dim i As Long
        Dim result As String
        
        ReDim buf(length - 1) As Byte
        
        Call RemoveData(ReadData(buf, length))
        
        result = ""
        For i = 0 To length - 1
            result = result & Chr$(buf(i))
        Next i
        
        ReadASCIIStringFixed = result
    Else
        Call Err.Raise(NOT_ENOUGH_DATA)
    End If
End Function

Public Function ReadUnicodeStringFixed(ByVal length As Long) As String
    If length <= 0 Then Exit Function
    
    If queueLength >= length * 2 Then
        Dim buf() As Byte
        Dim i As Long
        Dim result As String
        
        ReDim buf(length * 2 - 1) As Byte
        
        Call RemoveData(ReadData(buf, length * 2))
        
        result = ""
        For i = 0 To length - 1
            result = result & Chr$(CLng(buf(i * 2)) + CLng(buf(i * 2 + 1)) * 256&)
        Next i
        
        ReadUnicodeStringFixed = result
    Else
        Call Err.Raise(NOT_ENOUGH_DATA)
    End If
End Function

Public Function ReadASCIIString() As String
    Dim buf(1) As Byte
    Dim length As Integer
    
    If queueLength > 1 Then
        Call ReadData(buf, 2)
        Call ByteArrayToType(length, buf, 0, 2)
        
        If queueLength >= length + 2 Then
            Call RemoveData(2)
            
            If length > 0 Then
                Dim buf2() As Byte
                Dim i As Long
                Dim result As String
                
                ReDim buf2(length - 1) As Byte
                
                Call RemoveData(ReadData(buf2, length))
                
                result = ""
                For i = 0 To length - 1
                    result = result & Chr$(buf2(i))
                Next i
                
                ReadASCIIString = result
            End If
        Else
            Call Err.Raise(NOT_ENOUGH_DATA)
        End If
    Else
        Call Err.Raise(NOT_ENOUGH_DATA)
    End If
End Function

Public Function ReadUnicodeString() As String
    Dim buf(1) As Byte
    Dim length As Integer
    
    If queueLength > 1 Then
        Call ReadData(buf, 2)
        Call ByteArrayToType(length, buf, 0, 2)
        
        If queueLength >= length * 2 + 2 Then
            Call RemoveData(2)
            
            Dim buf2() As Byte
            Dim i As Long
            Dim result As String
            
            ReDim buf2(length * 2 - 1) As Byte
            
            Call RemoveData(ReadData(buf2, length * 2))
            
            result = ""
            For i = 0 To length - 1
                result = result & Chr$(CLng(buf2(i * 2)) + CLng(buf2(i * 2 + 1)) * 256&)
            Next i
            
            ReadUnicodeString = result
        Else
            Call Err.Raise(NOT_ENOUGH_DATA)
        End If
    Else
        Call Err.Raise(NOT_ENOUGH_DATA)
    End If
End Function

Public Function ReadBlock(ByRef block() As Byte, ByVal dataLength As Long) As Long
    If dataLength > 0 Then _
        ReadBlock = RemoveData(ReadData(block(), dataLength))
End Function

Public Function PeekByte() As Byte
    Dim buf(0) As Byte
    
    Call ReadData(buf, 1)
    
    PeekByte = buf(0)
End Function

Public Function PeekInteger() As Integer
    Dim buf(1) As Byte
    Dim result As Integer
    
    Call ReadData(buf, 2)
    
    Call ByteArrayToType(result, buf, 0, 2)
    PeekInteger = result
End Function

Public Function PeekLong() As Long
    Dim buf(3) As Byte
    Dim result As Long
    
    Call ReadData(buf, 4)
    
    Call ByteArrayToType(result, buf, 0, 4)
    PeekLong = result
End Function

Public Function PeekSingle() As Single
    Dim buf(3) As Byte
    Dim result As Single
    
    Call ReadData(buf, 4)
    
    Call ByteArrayToType(result, buf, 0, 4)
    PeekSingle = result
End Function

Public Function PeekDouble() As Double
    Dim buf(7) As Byte
    Dim result As Double
    
    Call ReadData(buf, 8)
    
    Call ByteArrayToType(result, buf, 0, 8)
    PeekDouble = result
End Function

Public Function PeekBoolean() As Boolean
    Dim buf(0) As Byte
    
    Call ReadData(buf, 1)
    
    If buf(0) = 1 Then PeekBoolean = True
End Function

Public Function PeekASCIIStringFixed(ByVal length As Long) As String
    If length <= 0 Then Exit Function
    
    If queueLength >= length Then
        Dim buf() As Byte
        Dim i As Long
        Dim result As String
        
        ReDim buf(length - 1) As Byte
        
        Call ReadData(buf, length)
        
        result = ""
        For i = 0 To length - 1
            result = result & Chr$(buf(i))
        Next i
        
        PeekASCIIStringFixed = result
    Else
        Call Err.Raise(NOT_ENOUGH_DATA)
    End If
End Function

Public Function PeekUnicodeStringFixed(ByVal length As Long) As String
    If length <= 0 Then Exit Function
    
    If queueLength >= length * 2 Then
        Dim buf() As Byte
        Dim i As Long
        Dim result As String
        
        ReDim buf(length * 2 - 1) As Byte
        
        Call ReadData(buf, length * 2)
        
        result = ""
        For i = 0 To length - 1
            result = result & Chr$(CLng(buf(i * 2)) + CLng(buf(i * 2 + 1)) * 256&)
        Next i
        
        PeekUnicodeStringFixed = result
    Else
        Call Err.Raise(NOT_ENOUGH_DATA)
    End If
End Function

Public Function PeekASCIIString() As String
    Dim buf(1) As Byte
    Dim length As Integer
    
    If queueLength > 1 Then
        Call ReadData(buf, 2)
        Call ByteArrayToType(length, buf, 0, 2)
        
        If queueLength >= length + 2 Then
            Dim buf2() As Byte
            Dim i As Long
            Dim result As String
            
            ReDim buf2(length - 1) As Byte
            
            Call ReadDataWithOffset(buf2, length, 2)
            
            result = ""
            For i = 0 To length - 1
                result = result & Chr$(buf2(i))
            Next i
            
            PeekASCIIString = result
        Else
            Call Err.Raise(NOT_ENOUGH_DATA)
        End If
    Else
        Call Err.Raise(NOT_ENOUGH_DATA)
    End If
End Function

Public Function PeekUnicodeString() As String
    Dim buf(1) As Byte
    Dim length As Integer
    
    If queueLength > 1 Then
        Call ReadData(buf, 2)
        Call ByteArrayToType(length, buf, 0, 2)
        
        If queueLength >= length * 2 + 2 Then
            Dim buf2() As Byte
            Dim i As Long
            Dim result As String
            
            ReDim buf2(length * 2 - 1) As Byte
            
            Call ReadDataWithOffset(buf2, length * 2, 2)
            
            result = ""
            For i = 0 To length - 1
                result = result & Chr$(CLng(buf2(i * 2)) + CLng(buf2(i * 2 + 1)) * 256&)
            Next i
            
            PeekUnicodeString = result
        Else
            Call Err.Raise(NOT_ENOUGH_DATA)
        End If
    Else
        Call Err.Raise(NOT_ENOUGH_DATA)
    End If
End Function

Public Function PeekBlock(ByRef block() As Byte, ByVal dataLength As Long) As Long
    If dataLength > 0 Then
        PeekBlock = ReadData(block(), dataLength)
    End If
End Function

Public Sub CopyBuffer(ByRef source As clsByteQueue)
    If source.length = 0 Then
        Call RemoveData(length)
        Exit Sub
    End If
    
    queueCapacity = source.Capacity
    
    ReDim data(queueCapacity - 1) As Byte
    
    Dim buf() As Byte
    ReDim buf(source.length - 1) As Byte
    
    Call source.PeekBlock(buf, source.length)
    
    queueLength = 0
    
    Call WriteBlock(buf, source.length)
End Sub

Public Property Get length() As Long
    length = queueLength
End Property

Public Property Get Capacity() As Long
    Capacity = queueCapacity
End Property

Public Property Get NotEnoughDataErrCode() As Long
    NotEnoughDataErrCode = NOT_ENOUGH_DATA
End Property

Public Property Get NotEnoughSpaceErrCode() As Long
    NotEnoughSpaceErrCode = NOT_ENOUGH_SPACE
End Property

