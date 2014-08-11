' ===============================================================================
' This file is part of Ecopath with Ecosim (EwE)
'
' EwE is free software: you can redistribute it and/or modify it under the terms
' of the GNU General Public License version 2 as published by the Free Software 
' Foundation.
'
' EwE is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; 
' without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR 
' PURPOSE. See the GNU General Public License for more details.
'
' You should have received a copy of the GNU General Public License along with EwE.
' If not, see <http://www.gnu.org/licenses/gpl-2.0.html>. 
'
' Copyright 1991- UBC Fisheries Centre, Vancouver BC, Canada.
' ===============================================================================
'

#Region " Imports "

Option Strict On
Imports System.Windows.Forms
Imports System.Reflection

#End Region ' Imports

Namespace Controls

    ''' =======================================================================
    ''' <summary>
    ''' Helper class for configuring file dialogs.
    ''' </summary>
    ''' =======================================================================
    Public Class cEwEFileDialogHelper

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Create an standardized File Open dialog for use in EwE.
        ''' </summary>
        ''' <param name="strTitle">Title to show.</param>
        ''' <param name="strFileName">The initial file name to open the dialog for.</param>
        ''' <param name="strFilters">Filters to display. Filters should be 
        ''' formatted '[Name|*.extension|]*'</param>
        ''' <param name="iDefaultFilter">Index of the default filter to set in the dialg.</param>
        ''' <param name="strInitialDirectory">Default directory to set in the dialog.</param>
        ''' <param name="bMultiSelect">Flag stating whether a user is allowed to 
        ''' select multiple files.</param>
        ''' <returns>A file dialog.</returns>
        ''' -------------------------------------------------------------------
        Public Shared Function OpenFileDialog(ByVal strTitle As String, _
                                    ByVal strFileName As String, _
                                    ByVal strFilters As String, _
                                    Optional ByVal iDefaultFilter As Integer = 0, _
                                    Optional ByVal strInitialDirectory As String = "", _
                                    Optional ByVal bMultiSelect As Boolean = False) As OpenFileDialog

            Dim dlg As New OpenFileDialog()

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
                .Multiselect = bMultiSelect
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

            Return dlg
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Create an standardized File Save dialog for use in EwE.
        ''' </summary>
        ''' <param name="strTitle">Title to show.</param>
        ''' <param name="strFileName">The initial file name to open the dialog for.</param>
        ''' <param name="strFilters">Filters to display. Filters should be 
        ''' formatted '[Name|*.extension|]*'</param>
        ''' <param name="iDefaultFilter">Index of the default filter to set in the dialg.</param>
        ''' <param name="strInitialDirectory">Default directory to set in the dialog.</param>
        ''' <returns>A file dialog.</returns>
        ''' -------------------------------------------------------------------
        Public Shared Function SaveFileDialog(ByVal strTitle As String, _
                                    ByVal strFileName As String, _
                                    ByVal strFilters As String, _
                                    Optional ByVal iDefaultFilter As Integer = 0, _
                                    Optional ByVal strInitialDirectory As String = "", _
                                    Optional ByVal bOverwritePrompt As Boolean = True) As SaveFileDialog

            Dim dlg As New SaveFileDialog()
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

            Return dlg

        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Create an standardized Folder browse dialog for use in EwE.
        ''' </summary>
        ''' <param name="strDescription">Description to display in the dialog.</param>
        ''' <param name="strInitialDirectory">The initial directory to open the
        ''' dialog for.</param>
        ''' <returns>A file dialog.</returns>
        ''' -------------------------------------------------------------------
        Public Shared Function FolderBrowserDialog(ByVal strDescription As String, _
                                                   ByVal strInitialDirectory As String) As FolderBrowserDialog

            Dim dlg As New FolderBrowserDialog()

            With dlg
                .SelectedPath = strInitialDirectory
                .ShowNewFolderButton = True
                .Description = strDescription
            End With

            ' Hack when SP1 installation is not detected
            Dim pi As PropertyInfo = GetType(FolderBrowserDialog).GetProperty("AutoUpgradeEnabled")
            If (pi IsNot Nothing) Then pi.SetValue(dlg, True, Nothing)

            Return dlg

        End Function
    End Class

End Namespace ' Controls
