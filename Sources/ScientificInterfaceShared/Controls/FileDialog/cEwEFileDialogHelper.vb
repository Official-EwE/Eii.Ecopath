'==============================================================================
'
' $Log: cEwEFileDialogHelper.vb,v $
' Revision 1.3  2009/05/11 01:50:49  jeroens
' Renamed command classes
'
' Revision 1.2  2008/11/10 22:11:36  jeroens
' Updated file dlg config
'
' Revision 1.1  2008/09/09 14:57:08  jeroens
' Initial version
'
'==============================================================================

#Region " Imports "

Option Strict On
Imports System.Windows.Forms
Imports System.Reflection

#End Region ' Imports

Namespace Controls

    Public Class cEwEFileDialogHelper

        Public Shared Sub Configure(ByVal dlg As OpenFileDialog, _
                                    ByVal strTitle As String, _
                                    ByVal strFileName As String, _
                                    ByVal strFilters As String, _
                                    Optional ByVal iDefaultFilter As Integer = 0, _
                                    Optional ByVal strInitialDirectory As String = "")

            With dlg
                .FileName = strFileName
                .Filter = strFilters
                .FilterIndex = iDefaultFilter
                .CheckPathExists = True
                .CheckFileExists = True
                .Multiselect = False
                .RestoreDirectory = True
                .SupportMultiDottedExtensions = True
                .AddExtension = True
            End With

            ' Hack when SP1 installation is not detected
            Dim pi As PropertyInfo = GetType(OpenFileDialog).GetProperty("AutoUpgradeEnabled")
            If (pi IsNot Nothing) Then pi.SetValue(dlg, True, Nothing)

            If Not String.IsNullOrEmpty(strTitle) Then
                dlg.Title = strTitle
            End If

            If Not String.IsNullOrEmpty(strInitialDirectory) Then
                dlg.InitialDirectory = strInitialDirectory
            End If

        End Sub

        Public Shared Sub Configure(ByVal dlg As SaveFileDialog, _
                                    ByVal strTitle As String, _
                                    ByVal strFileName As String, _
                                    ByVal strFilters As String, _
                                    Optional ByVal iDefaultFilter As Integer = 0, _
                                    Optional ByVal strInitialDirectory As String = "", _
                                    Optional ByVal bOverwritePrompt As Boolean = True)

            With dlg
                .FileName = strFileName
                .Filter = strFilters
                .FilterIndex = iDefaultFilter
                .CheckPathExists = True
                .CheckFileExists = False
                .OverwritePrompt = bOverwritePrompt
                .RestoreDirectory = True
                .SupportMultiDottedExtensions = True
                .AddExtension = True
            End With

            ' Hack when SP1 installation is not detected
            Dim pi As PropertyInfo = GetType(SaveFileDialog).GetProperty("AutoUpgradeEnabled")
            If (pi IsNot Nothing) Then pi.SetValue(dlg, True, Nothing)

            If Not String.IsNullOrEmpty(strTitle) Then
                dlg.Title = strTitle
            End If

            If Not String.IsNullOrEmpty(strInitialDirectory) Then
                dlg.InitialDirectory = strInitialDirectory
            End If

        End Sub

        Public Shared Sub Configure(ByVal dlg As FolderBrowserDialog, _
                                    ByVal strDescription As String, _
                                    ByVal strInitialDirectory As String)

            With dlg
                .SelectedPath = strInitialDirectory
                .ShowNewFolderButton = True
                .Description = strDescription
            End With

            ' Hack when SP1 installation is not detected
            Dim pi As PropertyInfo = GetType(FolderBrowserDialog).GetProperty("AutoUpgradeEnabled")
            If (pi IsNot Nothing) Then pi.SetValue(dlg, True, Nothing)

        End Sub

    End Class

End Namespace ' Controls
