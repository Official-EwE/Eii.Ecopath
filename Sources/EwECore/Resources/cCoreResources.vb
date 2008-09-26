'==============================================================================
'
' $Log: cCoreResources.vb,v $
' Revision 1.1  2008/09/26 07:30:32  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.2  2006/05/03 04:36:40  cvsuser
' + Added SaveResourceToFile
'
'
'==============================================================================
Option Strict On

Imports System.Reflection
Imports System.Globalization
Imports System.Resources
Imports System.IO

''' ---------------------------------------------------------------------------
''' <summary>
''' Provides access to resources embedded in the EwECore
''' </summary>
''' ---------------------------------------------------------------------------
Public Class cCoreResources

    ''' <summary>ResourceManager for resources in CoreMessages.resx</summary>
    Private Shared ResManMessages As ResourceManager = New ResourceManager("EwECore.CoreMessages", Assembly.GetExecutingAssembly())
    ''' <summary>Name of the current namespace. Cached to provide quick access</summary>
    Private Shared CurrentNamespace As String = Assembly.GetExecutingAssembly().GetName().Name.ToString()

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Obtains and returns a named string resource. If the string does not exist, an optional default is returned
    ''' </summary>
    ''' <param name="strName">The name of the string to find</param>
    ''' <param name="strDefault">The default string</param>
    ''' -----------------------------------------------------------------------
    Public Shared Function GetMessage(ByVal strName As String, Optional ByVal strDefault As String = "") As String

        Dim str As String = ResManMessages.GetString(strName, CultureInfo.CurrentUICulture)
        If String.IsNullOrEmpty(str) Then str = strDefault
        Return str

    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Saves an embedded resource to a file
    ''' </summary>
    ''' <param name="strResourceName">The name of the resource in the current assembly, current namespace.</param>
    ''' <param name="strFileName">The name of the file to save the resource to</param>
    ''' <param name="bOverwrite">States whether an existing file is allowed to be overwritten</param>
    ''' <returns>True if succesful</returns>
    ''' -----------------------------------------------------------------------
    Public Shared Function SaveResourceToFile(ByVal strResourceName As String, ByVal strFileName As String, _
            Optional ByVal bOverwrite As Boolean = False) As Boolean

        Dim sResource As Stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(CurrentNamespace & "." & strResourceName)
        Dim sFile As FileStream = Nothing
        Dim nBufLen As Integer = 256
        Dim byBuffer(nBufLen) As Byte
        Dim nBytesRead As Integer = 0

        ' Pre
        Debug.Assert(Not String.IsNullOrEmpty(strFileName), "Required target file name missing")
        Debug.Assert(sResource IsNot Nothing, String.Format("Resource {0} not found in {1}", strResourceName, CurrentNamespace))

        ' Work with full path
        strFileName = Path.GetFullPath(strFileName)

        Try
            If (bOverwrite) Then
                ' Create the file, overwriting any existing file with the same path
                sFile = New FileStream(strFileName, FileMode.Create, FileAccess.Write)
            Else
                ' Create the file but do not overwrite
                sFile = New FileStream(strFileName, FileMode.CreateNew, FileAccess.Write)
            End If
        Catch ex As Exception
            ' Just so you know
            Debug.Print("Unable to create or overwrite file {0}", strFileName)
            ' Report failure
            Return False
        End Try

        ' Copy embedded resource to file
        nBytesRead = sResource.Read(byBuffer, 0, nBufLen)
        While (nBytesRead > 0)
            sFile.Write(byBuffer, 0, nBytesRead)
            nBytesRead = sResource.Read(byBuffer, 0, nBufLen)
        End While
        ' Done
        sFile.Close()
        Return True

    End Function

End Class
