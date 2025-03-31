Option Strict Off
Option Explicit On
Friend Class clsByteQueue
	' TODO MIGRA: debe ser muy poco performante. Cambiar por otra cosa apenas se pueda
	
	Private Const NOT_ENOUGH_DATA As Integer = vbObjectError + 9
	Private Const NOT_ENOUGH_SPACE As Integer = vbObjectError + 10
	Private Const DATA_BUFFER As Integer = 10240
	
	Private Structure ByteConverter
		<VBFixedArray(3)> Dim ByteArr() As Byte
		Dim LongValue As Integer
		Dim SingleValue As Single
		
		'UPGRADE_TODO: Se debe llamar a "Initialize" para inicializar instancias de esta estructura. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="B4BFF9E0-8631-45CF-910E-62AB3970F27B"'
		Public Sub Initialize()
			ReDim ByteArr(3)
		End Sub
	End Structure
	
	Private Structure DoubleConverter
		<VBFixedArray(7)> Dim ByteArr() As Byte
		Dim DoubleValue As Double
		
		'UPGRADE_TODO: Se debe llamar a "Initialize" para inicializar instancias de esta estructura. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="B4BFF9E0-8631-45CF-910E-62AB3970F27B"'
		Public Sub Initialize()
			ReDim ByteArr(7)
		End Sub
	End Structure
	
	Dim data() As Byte
	Dim queueCapacity As Integer
	Dim queueLength As Integer
	
	'UPGRADE_NOTE: Class_Initialize se actualizó a Class_Initialize_Renamed. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
	Private Sub Class_Initialize_Renamed()
		ReDim data(DATA_BUFFER - 1)
		queueCapacity = DATA_BUFFER
	End Sub
	Public Sub New()
		MyBase.New()
		Class_Initialize_Renamed()
	End Sub
	
	'UPGRADE_NOTE: Class_Terminate se actualizó a Class_Terminate_Renamed. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
	Private Sub Class_Terminate_Renamed()
		Erase data
	End Sub
	Protected Overrides Sub Finalize()
		Class_Terminate_Renamed()
		MyBase.Finalize()
	End Sub
	
	Private Function min(ByVal val1 As Integer, ByVal val2 As Integer) As Integer
		If val1 < val2 Then
			min = val1
		Else
			min = val2
		End If
	End Function
	
	Private Sub ByteArrayToType(ByRef destVariable As Object, ByRef sourceArray() As Byte, ByVal startPos As Integer, ByVal length As Integer)
		Dim i As Integer
		Dim tempArray() As Byte
		'UPGRADE_WARNING: Puede que necesite inicializar las matrices de la estructura tempBC, antes de poder utilizarlas. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="814DF224-76BD-4BB4-BFFB-EA359CB9FC48"'
		Dim tempBC As ByteConverter
		'UPGRADE_WARNING: Puede que necesite inicializar las matrices de la estructura tempDC, antes de poder utilizarlas. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="814DF224-76BD-4BB4-BFFB-EA359CB9FC48"'
		Dim tempDC As DoubleConverter
		
		ReDim tempArray(length - 1)
		
		For i = 0 To length - 1
			tempArray(i) = sourceArray(startPos + i)
		Next i
		
		Select Case length
			Case 1
				'UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto destVariable. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
				destVariable = tempArray(0)
			Case 2
				'UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto destVariable. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
				destVariable = CShort(tempArray(0)) + CShort(tempArray(1)) * 256
			Case 4
				'UPGRADE_WARNING: VarType tiene un nuevo comportamiento. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"'
				If VarType(destVariable) = VariantType.Single Then
					For i = 0 To 3
						tempBC.ByteArr(i) = tempArray(i)
					Next i
					'UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto destVariable. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
					destVariable = tempBC.SingleValue
				Else
					'UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto destVariable. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
					destVariable = CInt(tempArray(0)) + CInt(tempArray(1)) * 256 + CInt(tempArray(2)) * 65536 + CInt(tempArray(3)) * 16777216
				End If
			Case 8
				For i = 0 To 7
					tempDC.ByteArr(i) = tempArray(i)
				Next i
				'UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto destVariable. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
				destVariable = tempDC.DoubleValue
		End Select
	End Sub
	
	Private Sub TypeToByteArray(ByRef destArray() As Byte, ByVal startPos As Integer, ByVal sourceVariable As Object)
		Dim i As Integer
		Dim valueType As VariantType
		'UPGRADE_WARNING: Puede que necesite inicializar las matrices de la estructura tempBC, antes de poder utilizarlas. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="814DF224-76BD-4BB4-BFFB-EA359CB9FC48"'
		Dim tempBC As ByteConverter
		'UPGRADE_WARNING: Puede que necesite inicializar las matrices de la estructura tempDC, antes de poder utilizarlas. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="814DF224-76BD-4BB4-BFFB-EA359CB9FC48"'
		Dim tempDC As DoubleConverter
		
		'UPGRADE_WARNING: VarType tiene un nuevo comportamiento. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"'
		valueType = VarType(sourceVariable)
		
		Dim TempInt As Short
		Dim tempLong As Integer
		Select Case valueType
			Case VariantType.Byte
				'UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto sourceVariable. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
				destArray(startPos) = CByte(sourceVariable)
			Case VariantType.Short
				'UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto sourceVariable. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
				TempInt = CShort(sourceVariable)
				destArray(startPos) = CByte(TempInt And &HFF)
				destArray(startPos + 1) = CByte((CInt(TempInt) And &HFF00) \ 256)
			Case VariantType.Integer
				'UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto sourceVariable. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
				tempLong = CInt(sourceVariable)
				destArray(startPos) = CByte(tempLong And &HFF)
				destArray(startPos + 1) = CByte((tempLong And &HFF00) \ 256)
				destArray(startPos + 2) = CByte((tempLong And &HFF0000) \ 65536)
				destArray(startPos + 3) = CByte((tempLong And &HFF000000) \ 16777216)
			Case VariantType.Single
				'UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto sourceVariable. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
				tempBC.SingleValue = CSng(sourceVariable)
				For i = 0 To 3
					destArray(startPos + i) = tempBC.ByteArr(i)
				Next i
			Case VariantType.Double
				'UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto sourceVariable. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
				tempDC.DoubleValue = CDbl(sourceVariable)
				For i = 0 To 7
					destArray(startPos + i) = tempDC.ByteArr(i)
				Next i
		End Select
	End Sub
	
	Private Sub CopyArrayData(ByRef destArray() As Byte, ByVal destStart As Integer, ByRef sourceArray() As Byte, ByVal sourceStart As Integer, ByVal length As Integer)
		Dim i As Integer
		
		For i = 0 To length - 1
			destArray(destStart + i) = sourceArray(sourceStart + i)
		Next i
	End Sub
	
	Private Function WriteData(ByRef buf() As Byte, ByVal dataLength As Integer) As Integer
		If queueCapacity - queueLength - dataLength < 0 Then
			Call Err.Raise(NOT_ENOUGH_SPACE)
			Exit Function
		End If
		
		Call CopyArrayData(data, queueLength, buf, 0, dataLength)
		
		queueLength = queueLength + dataLength
		WriteData = dataLength
	End Function
	
	Private Function ReadData(ByRef buf() As Byte, ByVal dataLength As Integer) As Integer
		If dataLength > queueLength Then
			Call Err.Raise(NOT_ENOUGH_DATA)
			Exit Function
		End If
		
		Call CopyArrayData(buf, 0, data, 0, dataLength)
		ReadData = dataLength
	End Function
	
	Private Function ReadDataWithOffset(ByRef buf() As Byte, ByVal dataLength As Integer, ByVal startPos As Integer) As Integer
		If dataLength > queueLength - startPos Then
			Call Err.Raise(NOT_ENOUGH_DATA)
			Exit Function
		End If
		
		Call CopyArrayData(buf, 0, data, startPos, dataLength)
		ReadDataWithOffset = dataLength
	End Function
	
	Private Function RemoveData(ByVal dataLength As Integer) As Integer
		RemoveData = min(dataLength, queueLength)
		
		If RemoveData <> queueCapacity Then
			Call CopyArrayData(data, 0, data, RemoveData, queueLength - RemoveData)
		End If
		
		queueLength = queueLength - RemoveData
	End Function
	
	Public Function WriteByte(ByVal Value As Byte) As Integer
		Dim buf(0) As Byte
		
		buf(0) = Value
		
		WriteByte = WriteData(buf, 1)
	End Function
	
	Public Function WriteInteger(ByVal Value As Short) As Integer
		Dim buf(1) As Byte
		
		Call TypeToByteArray(buf, 0, Value)
		
		WriteInteger = WriteData(buf, 2)
	End Function
	
	Public Function WriteLong(ByVal Value As Integer) As Integer
		Dim buf(3) As Byte
		
		Call TypeToByteArray(buf, 0, Value)
		
		WriteLong = WriteData(buf, 4)
	End Function
	
	Public Function WriteSingle(ByVal Value As Single) As Integer
		Dim buf(3) As Byte
		
		Call TypeToByteArray(buf, 0, Value)
		
		WriteSingle = WriteData(buf, 4)
	End Function
	
	Public Function WriteDouble(ByVal Value As Double) As Integer
		Dim buf(7) As Byte
		
		Call TypeToByteArray(buf, 0, Value)
		
		WriteDouble = WriteData(buf, 8)
	End Function
	
	Public Function WriteBoolean(ByVal Value As Boolean) As Integer
		Dim buf(0) As Byte
		
		If Value Then buf(0) = 1
		
		WriteBoolean = WriteData(buf, 1)
	End Function
	
	Public Function WriteASCIIStringFixed(ByVal Value As String) As Integer
		Dim buf() As Byte
		Dim i, j As Integer
		
		ReDim buf(Len(Value) - 1)
		
		For i = 1 To Len(Value)
			buf(i - 1) = Asc(Mid(Value, i, 1))
		Next i
		
		WriteASCIIStringFixed = WriteData(buf, Len(Value))
	End Function
	
	Public Function WriteUnicodeStringFixed(ByVal Value As String) As Integer
		Dim buf() As Byte
		Dim i, j As Integer
		Dim tempStr As String
		
		ReDim buf(migr_LenB(Value) - 1)
		
		j = 0
		For i = 1 To Len(Value)
			tempStr = Mid(Value, i, 1)
			buf(j) = Asc(tempStr) And &HFF
			buf(j + 1) = (Asc(tempStr) And &HFF00) \ 256
			j = j + 2
		Next i
		
		WriteUnicodeStringFixed = WriteData(buf, migr_LenB(Value))
	End Function
	
	Public Function WriteASCIIString(ByVal Value As String) As Integer
		Dim buf() As Byte
		Dim i As Integer
		Dim length As Short
		
		length = Len(Value)
		ReDim buf(length + 1)
		
		Call TypeToByteArray(buf, 0, length)
		
		If length > 0 Then
			For i = 1 To length
				buf(i + 1) = Asc(Mid(Value, i, 1))
			Next i
		End If
		
		WriteASCIIString = WriteData(buf, length + 2)
	End Function
	
	Public Function WriteUnicodeString(ByVal Value As String) As Integer
		Dim buf() As Byte
		Dim i, j As Integer
		Dim tempStr As String
		Dim length As Short
		
		length = Len(Value)
		ReDim buf(length * 2 + 1)
		
		Call TypeToByteArray(buf, 0, length)
		
		If length > 0 Then
			j = 2
			For i = 1 To length
				tempStr = Mid(Value, i, 1)
				buf(j) = Asc(tempStr) And &HFF
				buf(j + 1) = (Asc(tempStr) And &HFF00) \ 256
				j = j + 2
			Next i
		End If
		
		WriteUnicodeString = WriteData(buf, length * 2 + 2)
	End Function
	
	Public Function WriteBlock(ByRef Value() As Byte, Optional ByVal length As Integer = -1) As Integer
		If length > UBound(Value) + 1 Or length < 0 Then length = UBound(Value) + 1
		
		WriteBlock = WriteData(Value, length)
	End Function
	
	Public Function ReadByte() As Byte
		Dim buf(0) As Byte
		
		Call RemoveData(ReadData(buf, 1))
		
		ReadByte = buf(0)
	End Function
	
	Public Function ReadInteger() As Short
		Dim buf(1) As Byte
		Dim result As Short
		
		Call RemoveData(ReadData(buf, 2))
		
		Call ByteArrayToType(result, buf, 0, 2)
		ReadInteger = result
	End Function
	
	Public Function ReadLong() As Integer
		Dim buf(3) As Byte
		Dim result As Integer
		
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
	
	Public Function ReadASCIIStringFixed(ByVal length As Integer) As String
		If length <= 0 Then Exit Function
		
		Dim buf() As Byte
		Dim i As Integer
		Dim result As String
		If queueLength >= length Then
			
			ReDim buf(length - 1)
			
			Call RemoveData(ReadData(buf, length))
			
			result = ""
			For i = 0 To length - 1
				result = result & Chr(buf(i))
			Next i
			
			ReadASCIIStringFixed = result
		Else
			Call Err.Raise(NOT_ENOUGH_DATA)
		End If
	End Function
	
	Public Function ReadUnicodeStringFixed(ByVal length As Integer) As String
		If length <= 0 Then Exit Function
		
		Dim buf() As Byte
		Dim i As Integer
		Dim result As String
		If queueLength >= length * 2 Then
			
			ReDim buf(length * 2 - 1)
			
			Call RemoveData(ReadData(buf, length * 2))
			
			result = ""
			For i = 0 To length - 1
				result = result & Chr(CInt(buf(i * 2)) + CInt(buf(i * 2 + 1)) * 256)
			Next i
			
			ReadUnicodeStringFixed = result
		Else
			Call Err.Raise(NOT_ENOUGH_DATA)
		End If
	End Function
	
	Public Function ReadASCIIString() As String
		Dim buf(1) As Byte
		Dim length As Short
		
		Dim buf2() As Byte
		Dim i As Integer
		Dim result As String
		If queueLength > 1 Then
			Call ReadData(buf, 2)
			Call ByteArrayToType(length, buf, 0, 2)
			
			If queueLength >= length + 2 Then
				Call RemoveData(2)
				
				If length > 0 Then
					
					ReDim buf2(length - 1)
					
					Call RemoveData(ReadData(buf2, length))
					
					result = ""
					For i = 0 To length - 1
						result = result & Chr(buf2(i))
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
		Dim length As Short
		
		Dim buf2() As Byte
		Dim i As Integer
		Dim result As String
		If queueLength > 1 Then
			Call ReadData(buf, 2)
			Call ByteArrayToType(length, buf, 0, 2)
			
			If queueLength >= length * 2 + 2 Then
				Call RemoveData(2)
				
				
				ReDim buf2(length * 2 - 1)
				
				Call RemoveData(ReadData(buf2, length * 2))
				
				result = ""
				For i = 0 To length - 1
					result = result & Chr(CInt(buf2(i * 2)) + CInt(buf2(i * 2 + 1)) * 256)
				Next i
				
				ReadUnicodeString = result
			Else
				Call Err.Raise(NOT_ENOUGH_DATA)
			End If
		Else
			Call Err.Raise(NOT_ENOUGH_DATA)
		End If
	End Function
	
	Public Function ReadBlock(ByRef block() As Byte, ByVal dataLength As Integer) As Integer
		If dataLength > 0 Then ReadBlock = RemoveData(ReadData(block, dataLength))
	End Function
	
	Public Function PeekByte() As Byte
		Dim buf(0) As Byte
		
		Call ReadData(buf, 1)
		
		PeekByte = buf(0)
	End Function
	
	Public Function PeekInteger() As Short
		Dim buf(1) As Byte
		Dim result As Short
		
		Call ReadData(buf, 2)
		
		Call ByteArrayToType(result, buf, 0, 2)
		PeekInteger = result
	End Function
	
	Public Function PeekLong() As Integer
		Dim buf(3) As Byte
		Dim result As Integer
		
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
	
	Public Function PeekASCIIStringFixed(ByVal length As Integer) As String
		If length <= 0 Then Exit Function
		
		Dim buf() As Byte
		Dim i As Integer
		Dim result As String
		If queueLength >= length Then
			
			ReDim buf(length - 1)
			
			Call ReadData(buf, length)
			
			result = ""
			For i = 0 To length - 1
				result = result & Chr(buf(i))
			Next i
			
			PeekASCIIStringFixed = result
		Else
			Call Err.Raise(NOT_ENOUGH_DATA)
		End If
	End Function
	
	Public Function PeekUnicodeStringFixed(ByVal length As Integer) As String
		If length <= 0 Then Exit Function
		
		Dim buf() As Byte
		Dim i As Integer
		Dim result As String
		If queueLength >= length * 2 Then
			
			ReDim buf(length * 2 - 1)
			
			Call ReadData(buf, length * 2)
			
			result = ""
			For i = 0 To length - 1
				result = result & Chr(CInt(buf(i * 2)) + CInt(buf(i * 2 + 1)) * 256)
			Next i
			
			PeekUnicodeStringFixed = result
		Else
			Call Err.Raise(NOT_ENOUGH_DATA)
		End If
	End Function
	
	Public Function PeekASCIIString() As String
		Dim buf(1) As Byte
		Dim length As Short
		
		Dim buf2() As Byte
		Dim i As Integer
		Dim result As String
		If queueLength > 1 Then
			Call ReadData(buf, 2)
			Call ByteArrayToType(length, buf, 0, 2)
			
			If queueLength >= length + 2 Then
				
				ReDim buf2(length - 1)
				
				Call ReadDataWithOffset(buf2, length, 2)
				
				result = ""
				For i = 0 To length - 1
					result = result & Chr(buf2(i))
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
		Dim length As Short
		
		Dim buf2() As Byte
		Dim i As Integer
		Dim result As String
		If queueLength > 1 Then
			Call ReadData(buf, 2)
			Call ByteArrayToType(length, buf, 0, 2)
			
			If queueLength >= length * 2 + 2 Then
				
				ReDim buf2(length * 2 - 1)
				
				Call ReadDataWithOffset(buf2, length * 2, 2)
				
				result = ""
				For i = 0 To length - 1
					result = result & Chr(CInt(buf2(i * 2)) + CInt(buf2(i * 2 + 1)) * 256)
				Next i
				
				PeekUnicodeString = result
			Else
				Call Err.Raise(NOT_ENOUGH_DATA)
			End If
		Else
			Call Err.Raise(NOT_ENOUGH_DATA)
		End If
	End Function
	
	Public Function PeekBlock(ByRef block() As Byte, ByVal dataLength As Integer) As Integer
		If dataLength > 0 Then
			PeekBlock = ReadData(block, dataLength)
		End If
	End Function
	
	Public Sub CopyBuffer(ByRef Source As clsByteQueue)
		If Source.length = 0 Then
			Call RemoveData(length)
			Exit Sub
		End If
		
		queueCapacity = Source.Capacity
		
		ReDim data(queueCapacity - 1)
		
		Dim buf() As Byte
		ReDim buf(Source.length - 1)
		
		Call Source.PeekBlock(buf, Source.length)
		
		queueLength = 0
		
		Call WriteBlock(buf, (Source.length))
	End Sub
	
	Public ReadOnly Property length() As Integer
		Get
			length = queueLength
		End Get
	End Property
	
	Public ReadOnly Property Capacity() As Integer
		Get
			Capacity = queueCapacity
		End Get
	End Property
	
	Public ReadOnly Property NotEnoughDataErrCode() As Integer
		Get
			NotEnoughDataErrCode = NOT_ENOUGH_DATA
		End Get
	End Property
	
	Public ReadOnly Property NotEnoughSpaceErrCode() As Integer
		Get
			NotEnoughSpaceErrCode = NOT_ENOUGH_SPACE
		End Get
	End Property
End Class