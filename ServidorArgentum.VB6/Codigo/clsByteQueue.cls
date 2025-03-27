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
Option Explicit
Option Base 0

Private Const NOT_ENOUGH_DATA As Long = vbObjectError + 9
Private Const NOT_ENOUGH_SPACE As Long = vbObjectError + 10
Private Const DATA_BUFFER As Long = 10240

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

Private Function min(ByVal val1 As Long, ByVal val2 As Long) As Long
    If val1 < val2 Then
        min = val1
    Else
        min = val2
    End If
End Function

Private Function WriteData(ByRef buf() As Byte, ByVal dataLength As Long) As Long
    If queueCapacity - queueLength - dataLength < 0 Then
        Call Err.Raise(NOT_ENOUGH_SPACE)
        Exit Function
    End If
    
    Dim i As Long
    For i = 0 To dataLength - 1
        data(queueLength + i) = buf(i)
    Next i
    
    queueLength = queueLength + dataLength
    WriteData = dataLength
End Function

Private Function ReadData(ByRef buf() As Byte, ByVal dataLength As Long, Optional ByVal startPos As Long = 0) As Long
    If dataLength > queueLength - startPos Then
        Call Err.Raise(NOT_ENOUGH_DATA)
        Exit Function
    End If
    
    Dim i As Long
    For i = 0 To dataLength - 1
        buf(i) = data(i + startPos)
    Next i
    
    ReadData = dataLength
End Function

Private Function RemoveData(ByVal dataLength As Long) As Long
    RemoveData = min(dataLength, queueLength)
    
    If RemoveData <> queueCapacity Then
        Dim i As Long
        For i = 0 To queueLength - RemoveData - 1
            data(i) = data(i + RemoveData)
        Next i
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
    buf(0) = value And &HFF
    buf(1) = (value \ &H100) And &HFF
    WriteInteger = WriteData(buf, 2)
End Function

Public Function WriteLong(ByVal value As Long) As Long
    Dim buf(3) As Byte
    buf(0) = value And &HFF
    buf(1) = (value \ &H100) And &HFF
    buf(2) = (value \ &H10000) And &HFF
    buf(3) = (value \ &H1000000) And &HFF
    WriteLong = WriteData(buf, 4)
End Function

Public Function WriteSingle(ByVal value As Single) As Long
    Dim buf(3) As Byte
    Dim tempVar As Long
    tempVar = CLng(value * 10000)
    buf(0) = tempVar And &HFF
    buf(1) = (tempVar \ &H100) And &HFF
    buf(2) = (tempVar \ &H10000) And &HFF
    buf(3) = (tempVar \ &H1000000) And &HFF
    
    WriteSingle = WriteData(buf, 4)
End Function

Public Function WriteDouble(ByVal value As Double) As Long
    Dim buf(7) As Byte
    
    Dim tempVarHigh As Long
    Dim tempVarLow As Long
    
    Dim tempVal As Double
    tempVal = value * 10000#
    
    tempVarLow = tempVal And &HFFFFFFFF
    tempVarHigh = (tempVal - tempVarLow) / 4294967296#
    
    buf(0) = tempVarLow And &HFF
    buf(1) = (tempVarLow \ &H100) And &HFF
    buf(2) = (tempVarLow \ &H10000) And &HFF
    buf(3) = (tempVarLow \ &H1000000) And &HFF
    
    buf(4) = tempVarHigh And &HFF
    buf(5) = (tempVarHigh \ &H100) And &HFF
    buf(6) = (tempVarHigh \ &H10000) And &HFF
    buf(7) = (tempVarHigh \ &H1000000) And &HFF
    
    WriteDouble = WriteData(buf, 8)
End Function

Public Function WriteBoolean(ByVal value As Boolean) As Long
    Dim buf(0) As Byte
    If value Then buf(0) = 1
    WriteBoolean = WriteData(buf, 1)
End Function

Public Function WriteASCIIStringFixed(ByVal value As String) As Long
    Dim buf() As Byte
    ReDim buf(Len(value) - 1) As Byte
    
    Dim i As Long
    For i = 1 To Len(value)
        buf(i - 1) = Asc(mid$(value, i, 1))
    Next i
    
    WriteASCIIStringFixed = WriteData(buf, Len(value))
End Function

Public Function WriteUnicodeStringFixed(ByVal value As String) As Long
    Dim buf() As Byte
    ReDim buf(LenB(value) - 1) As Byte
    
    Dim i As Long
    For i = 1 To LenB(value) / 2
        buf((i - 1) * 2) = Asc(mid$(value, i, 1)) And &HFF
        buf((i - 1) * 2 + 1) = (Asc(mid$(value, i, 1)) \ &H100) And &HFF
    Next i
    
    WriteUnicodeStringFixed = WriteData(buf, LenB(value))
End Function

Public Function WriteASCIIString(ByVal value As String) As Long
    Dim buf() As Byte
    ReDim buf(Len(value) + 1) As Byte
    
    buf(0) = Len(value) And &HFF
    buf(1) = (Len(value) \ &H100) And &HFF
    
    Dim i As Long
    For i = 1 To Len(value)
        buf(i + 1) = Asc(mid$(value, i, 1))
    Next i
    
    WriteASCIIString = WriteData(buf, Len(value) + 2)
End Function

Public Function WriteUnicodeString(ByVal value As String) As Long
    Dim buf() As Byte
    ReDim buf(LenB(value) + 1) As Byte
    
    buf(0) = Len(value) And &HFF
    buf(1) = (Len(value) \ &H100) And &HFF
    
    Dim i As Long
    For i = 1 To Len(value)
        buf((i - 1) * 2 + 2) = Asc(mid$(value, i, 1)) And &HFF
        buf((i - 1) * 2 + 3) = (Asc(mid$(value, i, 1)) \ &H100) And &HFF
    Next i
    
    WriteUnicodeString = WriteData(buf, LenB(value) + 2)
End Function

Public Function WriteBlock(ByRef value() As Byte, Optional ByVal length As Long = -1) As Long
    If length > UBound(value()) + 1 Or length < 0 Then length = UBound(value()) + 1
    WriteBlock = WriteData(value, length)
End Function

Public Function ReadByte() As Byte
    Dim buf(0) As Byte
    Call ReadData(buf, 1)
    Call RemoveData(1)
    ReadByte = buf(0)
End Function

Public Function ReadInteger() As Integer
    Dim buf(1) As Byte
    Call ReadData(buf, 2)
    Call RemoveData(2)
    ReadInteger = CLng(buf(0)) + CLng(buf(1)) * 256
End Function

Public Function ReadLong() As Long
    Dim buf(3) As Byte
    Call ReadData(buf, 4)
    Call RemoveData(4)
    ReadLong = CLng(buf(0)) + CLng(buf(1)) * 256 + CLng(buf(2)) * 65536 + CLng(buf(3)) * 16777216
End Function

Public Function ReadSingle() As Single
    Dim buf(3) As Byte
    Call ReadData(buf, 4)
    Call RemoveData(4)
    
    Dim tempVal As Long
    tempVal = CLng(buf(0)) + CLng(buf(1)) * 256 + CLng(buf(2)) * 65536 + CLng(buf(3)) * 16777216
    ReadSingle = tempVal / 10000
End Function

Public Function ReadDouble() As Double
    Dim buf(7) As Byte
    Call ReadData(buf, 8)
    Call RemoveData(8)
    
    Dim tempVarLow As Long
    Dim tempVarHigh As Long
    
    tempVarLow = CLng(buf(0)) + CLng(buf(1)) * 256 + CLng(buf(2)) * 65536 + CLng(buf(3)) * 16777216
    tempVarHigh = CLng(buf(4)) + CLng(buf(5)) * 256 + CLng(buf(6)) * 65536 + CLng(buf(7)) * 16777216
    
    ReadDouble = (CDbl(tempVarHigh) * 4294967296# + tempVarLow) / 10000#
End Function

Public Function ReadBoolean() As Boolean
    Dim buf(0) As Byte
    Call ReadData(buf, 1)
    Call RemoveData(1)
    If buf(0) = 1 Then ReadBoolean = True
End Function

Public Function ReadASCIIStringFixed(ByVal length As Long) As String
    If length <= 0 Then Exit Function
    
    If queueLength >= length Then
        Dim buf() As Byte
        ReDim buf(length - 1) As Byte
        
        Call ReadData(buf, length)
        Call RemoveData(length)
        
        Dim result As String
        result = ""
        
        Dim i As Long
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
        ReDim buf(length * 2 - 1) As Byte
        
        Call ReadData(buf, length * 2)
        Call RemoveData(length * 2)
        
        Dim result As String
        result = ""
        
        Dim i As Long
        For i = 0 To length - 1
            result = result & Chr$(buf(i * 2) + buf(i * 2 + 1) * 256)
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
        length = buf(0) + buf(1) * 256
        
        If queueLength >= length + 2 Then
            Call RemoveData(2)
            
            If length > 0 Then
                Dim buf2() As Byte
                ReDim buf2(length - 1) As Byte
                
                Call ReadData(buf2, length)
                Call RemoveData(length)
                
                Dim result As String
                result = ""
                
                Dim i As Long
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
        length = buf(0) + buf(1) * 256
        
        If queueLength >= length * 2 + 2 Then
            Call RemoveData(2)
            
            Dim buf2() As Byte
            ReDim buf2(length * 2 - 1) As Byte
            
            Call ReadData(buf2, length * 2)
            Call RemoveData(length * 2)
            
            Dim result As String
            result = ""
            
            Dim i As Long
            For i = 0 To length - 1
                result = result & Chr$(buf2(i * 2) + buf2(i * 2 + 1) * 256)
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
    If dataLength > 0 Then
        ReadBlock = ReadData(block(), dataLength)
        Call RemoveData(dataLength)
    End If
End Function

Public Function PeekByte() As Byte
    Dim buf(0) As Byte
    Call ReadData(buf, 1)
    PeekByte = buf(0)
End Function

Public Function PeekInteger() As Integer
    Dim buf(1) As Byte
    Call ReadData(buf, 2)
    PeekInteger = CLng(buf(0)) + CLng(buf(1)) * 256
End Function

Public Function PeekLong() As Long
    Dim buf(3) As Byte
    Call ReadData(buf, 4)
    PeekLong = CLng(buf(0)) + CLng(buf(1)) * 256 + CLng(buf(2)) * 65536 + CLng(buf(3)) * 16777216
End Function

Public Function PeekSingle() As Single
    Dim buf(3) As Byte
    Call ReadData(buf, 4)
    
    Dim tempVal As Long
    tempVal = CLng(buf(0)) + CLng(buf(1)) * 256 + CLng(buf(2)) * 65536 + CLng(buf(3)) * 16777216
    PeekSingle = tempVal / 10000
End Function

Public Function PeekDouble() As Double
    Dim buf(7) As Byte
    Call ReadData(buf, 8)
    
    Dim tempVarLow As Long
    Dim tempVarHigh As Long
    
    tempVarLow = CLng(buf(0)) + CLng(buf(1)) * 256 + CLng(buf(2)) * 65536 + CLng(buf(3)) * 16777216
    tempVarHigh = CLng(buf(4)) + CLng(buf(5)) * 256 + CLng(buf(6)) * 65536 + CLng(buf(7)) * 16777216
    
    PeekDouble = (CDbl(tempVarHigh) * 4294967296# + tempVarLow) / 10000#
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
        ReDim buf(length - 1) As Byte
        
        Call ReadData(buf, length)
        
        Dim result As String
        result = ""
        
        Dim i As Long
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
        ReDim buf(length * 2 - 1) As Byte
        
        Call ReadData(buf, length * 2)
        
        Dim result As String
        result = ""
        
        Dim i As Long
        For i = 0 To length - 1
            result = result & Chr$(buf(i * 2) + buf(i * 2 + 1) * 256)
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
        length = buf(0) + buf(1) * 256
        
        If queueLength >= length + 2 Then
            Dim buf2() As Byte
            ReDim buf2(length - 1) As Byte
            
            Call ReadData(buf2, length, 2)
            
            Dim result As String
            result = ""
            
            Dim i As Long
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
        length = buf(0) + buf(1) * 256
        
        If queueLength >= length * 2 + 2 Then
            Dim buf2() As Byte
            ReDim buf2(length * 2 - 1) As Byte
            
            Call ReadData(buf2, length * 2, 2)
            
            Dim result As String
            result = ""
            
            Dim i As Long
            For i = 0 To length - 1
                result = result & Chr$(buf2(i * 2) + buf2(i * 2 + 1) * 256)
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
    If dataLength > 0 Then _
        PeekBlock = ReadData(block(), dataLength)
End Function

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

