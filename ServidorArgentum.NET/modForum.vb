Option Strict Off
Option Explicit On
Module modForum
	'Argentum Online 0.12.2
	'Copyright (C) 2002 Márquez Pablo Ignacio
	'
	'This program is free software; you can redistribute it and/or modify
	'it under the terms of the Affero General Public License;
	'either version 1 of the License, or any later version.
	'
	'This program is distributed in the hope that it will be useful,
	'but WITHOUT ANY WARRANTY; without even the implied warranty of
	'MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
	'Affero General Public License for more details.
	'
	'You should have received a copy of the Affero General Public License
	'along with this program; if not, you can find it at http://www.affero.org/oagpl.html
	'
	'Argentum Online is based on Baronsoft's VB6 Online RPG
	'You can contact the original creator of ORE at aaron@baronsoft.com
	'for more information about ORE please visit http://www.baronsoft.com/
	'
	'
	'You can contact me at:
	'morgolock@speedy.com.ar
	'www.geocities.com/gmorgolock
	'Calle 3 número 983 piso 7 dto A
	'La Plata - Pcia, Buenos Aires - Republica Argentina
	'Código Postal 1900
	'Pablo Ignacio Márquez
	
	
	
	Public Const MAX_MENSAJES_FORO As Byte = 30
	Public Const MAX_ANUNCIOS_FORO As Byte = 5
	
	Public Const FORO_REAL_ID As String = "REAL"
	Public Const FORO_CAOS_ID As String = "CAOS"
	
	Public Structure tPost
		Dim sTitulo As String
		Dim sPost As String
		Dim Autor As String
	End Structure
	
	Public Structure tForo
		<VBFixedArray(MAX_MENSAJES_FORO)> Dim vsPost() As tPost
		<VBFixedArray(MAX_ANUNCIOS_FORO)> Dim vsAnuncio() As tPost
		Dim CantPosts As Byte
		Dim CantAnuncios As Byte
		Dim ID As String
		
		'UPGRADE_TODO: Se debe llamar a "Initialize" para inicializar instancias de esta estructura. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="B4BFF9E0-8631-45CF-910E-62AB3970F27B"'
		Public Sub Initialize()
			'UPGRADE_WARNING: El límite inferior de la matriz vsPost ha cambiado de 1 a 0. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"'
			ReDim vsPost(MAX_MENSAJES_FORO)
			'UPGRADE_WARNING: El límite inferior de la matriz vsAnuncio ha cambiado de 1 a 0. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"'
			ReDim vsAnuncio(MAX_ANUNCIOS_FORO)
		End Sub
	End Structure
	
	Private NumForos As Short
	Private Foros() As tForo
	
	
	Public Sub AddForum(ByVal sForoID As String)
		'***************************************************
		'Author: ZaMa
		'Last Modification: 22/02/2010
		'Adds a forum to the list and fills it.
		'***************************************************
		Dim ForumPath As String
		Dim PostPath As String
		Dim PostIndex As Short
		Dim FileIndex As Short
		
		NumForos = NumForos + 1
		'UPGRADE_WARNING: Es posible que la matriz Foros necesite tener elementos individuales inicializados. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="B97B714D-9338-48AC-B03F-345B617E2B02"'
		'UPGRADE_WARNING: El límite inferior de la matriz Foros ha cambiado de 1 a 0. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"'
		ReDim Preserve Foros(NumForos)
		
		ForumPath = My.Application.Info.DirectoryPath & "\foros\" & sForoID & ".for"
		
		With Foros(NumForos)
			
			.ID = sForoID
			
			If FileExist(ForumPath) Then
				.CantPosts = Val(GetVar(ForumPath, "INFO", "CantMSG"))
				.CantAnuncios = Val(GetVar(ForumPath, "INFO", "CantAnuncios"))
				
				' Cargo posts
				For PostIndex = 1 To .CantPosts
					FileIndex = FreeFile
					PostPath = My.Application.Info.DirectoryPath & "\foros\" & sForoID & PostIndex & ".for"
					
					FileOpen(FileIndex, PostPath, OpenMode.Input, , OpenShare.Shared)
					
					' Titulo
					Input(FileIndex, .vsPost(PostIndex).sTitulo)
					' Autor
					Input(FileIndex, .vsPost(PostIndex).Autor)
					' Mensaje
					Input(FileIndex, .vsPost(PostIndex).sPost)
					
					FileClose(FileIndex)
				Next PostIndex
				
				' Cargo anuncios
				For PostIndex = 1 To .CantAnuncios
					FileIndex = FreeFile
					PostPath = My.Application.Info.DirectoryPath & "\foros\" & sForoID & PostIndex & "a.for"
					
					FileOpen(FileIndex, PostPath, OpenMode.Input, , OpenShare.Shared)
					
					' Titulo
					Input(FileIndex, .vsAnuncio(PostIndex).sTitulo)
					' Autor
					Input(FileIndex, .vsAnuncio(PostIndex).Autor)
					' Mensaje
					Input(FileIndex, .vsAnuncio(PostIndex).sPost)
					
					FileClose(FileIndex)
				Next PostIndex
			End If
			
		End With
		
	End Sub
	
	Public Function GetForumIndex(ByRef sForoID As String) As Short
		'***************************************************
		'Author: ZaMa
		'Last Modification: 22/02/2010
		'Returns the forum index.
		'***************************************************
		
		Dim ForumIndex As Short
		
		For ForumIndex = 1 To NumForos
			If Foros(ForumIndex).ID = sForoID Then
				GetForumIndex = ForumIndex
				Exit Function
			End If
		Next ForumIndex
		
	End Function
	
	Public Sub AddPost(ByVal ForumIndex As Short, ByRef Post As String, ByRef Autor As String, ByRef Titulo As String, ByVal bAnuncio As Boolean)
		'***************************************************
		'Author: ZaMa
		'Last Modification: 22/02/2010
		'Saves a new post into the forum.
		'***************************************************
		
		With Foros(ForumIndex)
			
			If bAnuncio Then
				If .CantAnuncios < MAX_ANUNCIOS_FORO Then .CantAnuncios = .CantAnuncios + 1
				
				Call MoveArray(ForumIndex, bAnuncio)
				
				' Agrego el anuncio
				With .vsAnuncio(1)
					.sTitulo = Titulo
					.Autor = Autor
					.sPost = Post
				End With
				
			Else
				If .CantPosts < MAX_MENSAJES_FORO Then .CantPosts = .CantPosts + 1
				
				Call MoveArray(ForumIndex, bAnuncio)
				
				' Agrego el post
				With .vsPost(1)
					.sTitulo = Titulo
					.Autor = Autor
					.sPost = Post
				End With
				
			End If
		End With
	End Sub
	
	Public Sub SaveForums()
		'***************************************************
		'Author: ZaMa
		'Last Modification: 22/02/2010
		'Saves all forums into disk.
		'***************************************************
		Dim ForumIndex As Short
		
		For ForumIndex = 1 To NumForos
			Call SaveForum(ForumIndex)
		Next ForumIndex
	End Sub
	
	
	Private Sub SaveForum(ByVal ForumIndex As Short)
		'***************************************************
		'Author: ZaMa
		'Last Modification: 22/02/2010
		'Saves a forum into disk.
		'***************************************************
		
		Dim PostIndex As Short
		Dim FileIndex As Short
		Dim PostPath As String
		
		Call CleanForum(ForumIndex)
		
		With Foros(ForumIndex)
			
			' Guardo info del foro
			Call WriteVar(My.Application.Info.DirectoryPath & "\Foros\" & .ID & ".for", "INFO", "CantMSG", CStr(.CantPosts))
			Call WriteVar(My.Application.Info.DirectoryPath & "\Foros\" & .ID & ".for", "INFO", "CantAnuncios", CStr(.CantAnuncios))
			
			' Guardo posts
			For PostIndex = 1 To .CantPosts
				
				PostPath = My.Application.Info.DirectoryPath & "\Foros\" & .ID & PostIndex & ".for"
				FileIndex = FreeFile
				FileOpen(FileIndex, PostPath, OpenMode.Output)
				
				With .vsPost(PostIndex)
					PrintLine(FileIndex, .sTitulo)
					PrintLine(FileIndex, .Autor)
					PrintLine(FileIndex, .sPost)
				End With
				
				FileClose(FileIndex)
				
			Next PostIndex
			
			' Guardo Anuncios
			For PostIndex = 1 To .CantAnuncios
				
				PostPath = My.Application.Info.DirectoryPath & "\Foros\" & .ID & PostIndex & "a.for"
				FileIndex = FreeFile
				FileOpen(FileIndex, PostPath, OpenMode.Output)
				
				With .vsAnuncio(PostIndex)
					PrintLine(FileIndex, .sTitulo)
					PrintLine(FileIndex, .Autor)
					PrintLine(FileIndex, .sPost)
				End With
				
				FileClose(FileIndex)
				
			Next PostIndex
			
		End With
		
	End Sub
	
	Public Sub CleanForum(ByVal ForumIndex As Short)
		'***************************************************
		'Author: ZaMa
		'Last Modification: 22/02/2010
		'Cleans a forum from disk.
		'***************************************************
		Dim PostIndex As Short
		Dim NumPost As Short
		Dim ForumPath As String
		
		With Foros(ForumIndex)
			
			' Elimino todo
			ForumPath = My.Application.Info.DirectoryPath & "\Foros\" & .ID & ".for"
			If FileExist(ForumPath) Then
				
				NumPost = Val(GetVar(ForumPath, "INFO", "CantMSG"))
				
				' Elimino los post viejos
				For PostIndex = 1 To NumPost
					Kill(My.Application.Info.DirectoryPath & "\Foros\" & .ID & PostIndex & ".for")
				Next PostIndex
				
				
				NumPost = Val(GetVar(ForumPath, "INFO", "CantAnuncios"))
				
				' Elimino los post viejos
				For PostIndex = 1 To NumPost
					Kill(My.Application.Info.DirectoryPath & "\Foros\" & .ID & PostIndex & "a.for")
				Next PostIndex
				
				
				' Elimino el foro
				Kill(My.Application.Info.DirectoryPath & "\Foros\" & .ID & ".for")
				
			End If
		End With
		
	End Sub
	
	Public Function SendPosts(ByVal UserIndex As Short, ByRef ForoID As String) As Boolean
		'***************************************************
		'Author: ZaMa
		'Last Modification: 22/02/2010
		'Sends all the posts of a required forum
		'***************************************************
		
		Dim ForumIndex As Short
		Dim PostIndex As Short
		Dim bEsGm As Boolean
		
		ForumIndex = GetForumIndex(ForoID)
		
		If ForumIndex > 0 Then
			
			With Foros(ForumIndex)
				
				' Send General posts
				For PostIndex = 1 To .CantPosts
					With .vsPost(PostIndex)
						Call WriteAddForumMsg(UserIndex, Declaraciones.eForumMsgType.ieGeneral, .sTitulo, .Autor, .sPost)
					End With
				Next PostIndex
				
				' Send Sticky posts
				For PostIndex = 1 To .CantAnuncios
					With .vsAnuncio(PostIndex)
						Call WriteAddForumMsg(UserIndex, Declaraciones.eForumMsgType.ieGENERAL_STICKY, .sTitulo, .Autor, .sPost)
					End With
				Next PostIndex
				
			End With
			
			bEsGm = EsGM(UserIndex)
			
			' Caos?
			If esCaos(UserIndex) Or bEsGm Then
				
				ForumIndex = GetForumIndex(FORO_CAOS_ID)
				
				With Foros(ForumIndex)
					
					' Send General Caos posts
					For PostIndex = 1 To .CantPosts
						
						With .vsPost(PostIndex)
							Call WriteAddForumMsg(UserIndex, Declaraciones.eForumMsgType.ieCAOS, .sTitulo, .Autor, .sPost)
						End With
						
					Next PostIndex
					
					' Send Sticky posts
					For PostIndex = 1 To .CantAnuncios
						With .vsAnuncio(PostIndex)
							Call WriteAddForumMsg(UserIndex, Declaraciones.eForumMsgType.ieCAOS_STICKY, .sTitulo, .Autor, .sPost)
						End With
					Next PostIndex
					
				End With
			End If
			
			' Caos?
			If esArmada(UserIndex) Or bEsGm Then
				
				ForumIndex = GetForumIndex(FORO_REAL_ID)
				
				With Foros(ForumIndex)
					
					' Send General Real posts
					For PostIndex = 1 To .CantPosts
						
						With .vsPost(PostIndex)
							Call WriteAddForumMsg(UserIndex, Declaraciones.eForumMsgType.ieREAL, .sTitulo, .Autor, .sPost)
						End With
						
					Next PostIndex
					
					' Send Sticky posts
					For PostIndex = 1 To .CantAnuncios
						With .vsAnuncio(PostIndex)
							Call WriteAddForumMsg(UserIndex, Declaraciones.eForumMsgType.ieREAL_STICKY, .sTitulo, .Autor, .sPost)
						End With
					Next PostIndex
					
				End With
			End If
			
			SendPosts = True
		End If
		
	End Function
	
	Public Function EsAnuncio(ByVal ForumType As Byte) As Boolean
		'***************************************************
		'Author: ZaMa
		'Last Modification: 22/02/2010
		'Returns true if the post is sticky.
		'***************************************************
		Select Case ForumType
			Case Declaraciones.eForumMsgType.ieCAOS_STICKY
				EsAnuncio = True
				
			Case Declaraciones.eForumMsgType.ieGENERAL_STICKY
				EsAnuncio = True
				
			Case Declaraciones.eForumMsgType.ieREAL_STICKY
				EsAnuncio = True
				
		End Select
		
	End Function
	
	Public Function ForumAlignment(ByVal yForumType As Byte) As Byte
		'***************************************************
		'Author: ZaMa
		'Last Modification: 01/03/2010
		'Returns the forum alignment.
		'***************************************************
		Select Case yForumType
			Case Declaraciones.eForumMsgType.ieCAOS, Declaraciones.eForumMsgType.ieCAOS_STICKY
				ForumAlignment = Declaraciones.eForumType.ieCAOS
				
			Case Declaraciones.eForumMsgType.ieGeneral, Declaraciones.eForumMsgType.ieGENERAL_STICKY
				ForumAlignment = Declaraciones.eForumType.ieGeneral
				
			Case Declaraciones.eForumMsgType.ieREAL, Declaraciones.eForumMsgType.ieREAL_STICKY
				ForumAlignment = Declaraciones.eForumType.ieREAL
				
		End Select
		
	End Function
	
	Public Sub ResetForums()
		'***************************************************
		'Author: ZaMa
		'Last Modification: 22/02/2010
		'Resets forum info
		'***************************************************
		'UPGRADE_WARNING: Es posible que la matriz Foros necesite tener elementos individuales inicializados. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="B97B714D-9338-48AC-B03F-345B617E2B02"'
		'UPGRADE_WARNING: El límite inferior de la matriz Foros ha cambiado de 1 a 0. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"'
		ReDim Foros(1)
		NumForos = 0
	End Sub
	
	Private Sub MoveArray(ByVal ForumIndex As Short, ByVal Sticky As Boolean)
		Dim i As Integer
		
		With Foros(ForumIndex)
			If Sticky Then
				For i = .CantAnuncios To 2 Step -1
					.vsAnuncio(i).sTitulo = .vsAnuncio(i - 1).sTitulo
					.vsAnuncio(i).sPost = .vsAnuncio(i - 1).sPost
					.vsAnuncio(i).Autor = .vsAnuncio(i - 1).Autor
				Next i
			Else
				For i = .CantPosts To 2 Step -1
					.vsPost(i).sTitulo = .vsPost(i - 1).sTitulo
					.vsPost(i).sPost = .vsPost(i - 1).sPost
					.vsPost(i).Autor = .vsPost(i - 1).Autor
				Next i
			End If
		End With
	End Sub
End Module