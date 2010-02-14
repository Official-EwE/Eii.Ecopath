#Region " Imports "

Option Strict On
Imports EwEUtils.Core
Imports JRO
Imports Microsoft.Win32
Imports System.IO
Imports System.Text

#End Region ' Imports

Namespace Database

    ''' =======================================================================
    ''' <summary>
    ''' Database compact class for JRO databases.
    ''' </summary>
    ''' =======================================================================
    Public Class cCompactJRO
        Implements IDatabaseCompact

#Region " Private vars "

        ''' <summary>Global, one time flags.</summary>
        Private Shared s_bEngineSearched As Boolean = False
        ''' <summary>Global, one time flags.</summary>
        Private Shared s_bEngineFound As Boolean = False

#End Region ' Private vars 

#Region " IDatabaseCompact implementation "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Returns whether the OS supports compacting databases via JRO.
        ''' </summary>
        ''' <returns>True if the OS supports compacting databases via JRO.</returns>
        ''' -------------------------------------------------------------------
        Public Function CanCompact() As Boolean _
            Implements IDatabaseCompact.CanCompact
            Return Me.DetectJRO()
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Compact a database via JRO.
        ''' </summary>
        ''' <param name="strFileFrom">Source database location.</param>
        ''' <param name="strConnectionFrom">Source database connection string.</param>
        ''' <param name="strFileTo">Target database location.</param>
        ''' <param name="strConnectionTo">Target database connection string.</param>
        ''' <returns>A <see cref="eDatasourceAccessType">database access
        ''' result code</see>.</returns>
        ''' -------------------------------------------------------------------
        Public Function Compact(ByVal strFileFrom As String, _
                                ByVal strConnectionFrom As String, _
                                ByVal strFileTo As String, _
                                ByVal strConnectionTo As String) As EwEUtils.Core.eDatasourceAccessType _
            Implements IDatabaseCompact.Compact

            Dim engine As JRO.JetEngine = Nothing

            ' Safety check: can compact at all?
            If Not Me.CanCompact() Then
                ' #No: abort
                Return eDatasourceAccessType.Failed_OSUnsupported
            End If

            Try
                ' Create engine
                engine = New JRO.JetEngine()
            Catch ex As Exception
                ' Woops
                Return eDatasourceAccessType.Failed_OSUnsupported
            End Try

            ' Able to get JET engine?
            If (engine Is Nothing) Then Return eDatasourceAccessType.Failed_OSUnsupported

            Try
                ' Compact DB
                engine.CompactDatabase(strConnectionFrom, strConnectionTo)
                ' Return result
                Return eDatasourceAccessType.Success
            Catch ex As Exception
                ' HMMM
            End Try

            Return eDatasourceAccessType.Failed_Unknown

        End Function

#End Region ' IDatabaseCompact implementation

#Region " Internals "

        Private Function DetectJRO() As Boolean

            If cCompactJRO.s_bEngineSearched Then Return cCompactJRO.s_bEngineFound

            ' "Universal" JRO key (same root on XP, Vista and Windows 7)
            cCompactJRO.s_bEngineFound = DetectJRORecursive(Registry.ClassesRoot.OpenSubKey("TypeLib\{AC3B8B4C-B6CA-11D1-9F31-00C04FC29D52}", False))
            cCompactJRO.s_bEngineSearched = True

            Return cCompactJRO.s_bEngineFound

        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Recursively find JRO registry entry to a valid registered copy of
        ''' msjro.dll, ye good ole' Jet engine database headaches that we
        ''' unfortunately need for compacting an MS Aaargcess Database.
        ''' </summary>
        ''' <param name="key">Registry to start searching.</param>
        ''' <returns>
        ''' True if a <see cref="IsCorrectJRO">correct</see> JRO version is 
        ''' reffered to by one <paramref name="key">key</paramref> or one of
        ''' its subkeys.
        ''' </returns>
        ''' -------------------------------------------------------------------
        Private Function DetectJRORecursive(ByVal key As RegistryKey) As Boolean

            Dim aKeys As String() = Nothing
            Dim keyValue As Object = Nothing
            Dim keyValueKind As RegistryValueKind = Nothing
            Dim strFile As String = ""

            If (key IsNot Nothing) Then
                keyValue = key.GetValue("")

                If keyValue IsNot Nothing Then

                    strFile = ""
                    keyValueKind = key.GetValueKind("")

                    ' Get default key
                    Select Case keyValueKind

                        Case RegistryValueKind.String, _
                             RegistryValueKind.ExpandString, _
                             RegistryValueKind.MultiString

                            strFile = CStr(keyValue).ToLower

                        Case RegistryValueKind.Binary
                            Dim abData As Byte() = DirectCast(keyValue, Byte())
                            Dim sb As New StringBuilder()

                            For i As Integer = 0 To abData.Length - 1
                                sb.Append(Chr(i))
                            Next
                            strFile = sb.ToString().ToLower

                    End Select

                    If Not String.IsNullOrEmpty(strFile) Then
                        If strFile.EndsWith("msjro.dll") Then
                            If (Me.IsCorrectJRO(strFile)) Then Return True
                        End If
                    End If
                End If

                For Each strSubkeyName As String In key.GetSubKeyNames
                    If Me.DetectJRORecursive(key.OpenSubKey(strSubkeyName, False)) Then Return True
                Next
            End If
            Return False

        End Function

        Private Function IsCorrectJRO(ByVal strFile As String) As Boolean

            If String.IsNullOrEmpty(strFile) Then Return False
            If Not File.Exists(strFile) Then Return False

            Dim fvi As FileVersionInfo = FileVersionInfo.GetVersionInfo(strFile)
            ' JRO 2.6 or newer
            Return ((fvi.FileMajorPart > 2) Or _
                    ((fvi.FileMajorPart = 2) And (fvi.FileMinorPart >= 60)))

        End Function

#End Region ' Internals

    End Class

End Namespace ' Database
