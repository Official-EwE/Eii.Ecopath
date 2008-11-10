'==============================================================================
'
' $Log: cEwEFileDialogHelper.vb,v $
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

        Public Shared Sub Configure(ByVal dlg As OpenFileDialog, ByVal strFileName As String, _
                ByVal strFilters As String, Optional ByVal iDefaultFilter As Integer = 0, _
                Optional ByVal strInitialDirectory As String = "")

            dlg.FileName = strFileName
            dlg.Filter = strFilters
            dlg.FilterIndex = iDefaultFilter
            dlg.CheckPathExists = True
            dlg.CheckFileExists = True
            dlg.Multiselect = False
            dlg.RestoreDirectory = True
            dlg.SupportMultiDottedExtensions = True
            dlg.AddExtension = True

            ' Hack when SP1 installation is not detected
            Dim pi As PropertyInfo = GetType(OpenFileDialog).GetProperty("AutoUpgradeEnabled")
            If (pi IsNot Nothing) Then pi.SetValue(dlg, True, Nothing)

            If Not String.IsNullOrEmpty(strInitialDirectory) Then
                dlg.InitialDirectory = strInitialDirectory
            End If

        End Sub

        Public Shared Sub Configure(ByVal dlg As SaveFileDialog, ByVal strFileName As String, _
                ByVal strFilters As String, Optional ByVal iDefaultFilter As Integer = 0, _
                Optional ByVal strInitialDirectory As String = "", _
                Optional ByVal bOverwritePrompt As Boolean = True)

            dlg.FileName = strFileName
            dlg.Filter = strFilters
            dlg.FilterIndex = iDefaultFilter
            dlg.CheckPathExists = True
            dlg.CheckFileExists = False
            dlg.OverwritePrompt = bOverwritePrompt
            dlg.RestoreDirectory = True
            dlg.SupportMultiDottedExtensions = true
            dlg.AddExtension = True

            ' Hack when SP1 installation is not detected
            Dim pi As PropertyInfo = GetType(SaveFileDialog).GetProperty("AutoUpgradeEnabled")
            If (pi IsNot Nothing) Then pi.SetValue(dlg, True, Nothing)

            If Not String.IsNullOrEmpty(strInitialDirectory) Then
                dlg.InitialDirectory = strInitialDirectory
            End If

        End Sub

    End Class

End Namespace ' Controls
