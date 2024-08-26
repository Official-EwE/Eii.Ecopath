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
' Copyright 1991- 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'

#Region " Imports "

Option Strict On
Imports System.Windows.Forms
Imports System.Reflection
Imports System.Text

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
        ''' <seealso cref="LastOpenedHistory"/>
        ''' -------------------------------------------------------------------
        Public Shared Function OpenFileDialog(strTitle As String,
                                    strFileName As String,
                                    strFilters As String,
                                    Optional iDefaultFilter As Integer = -1,
                                    Optional strInitialDirectory As String = "",
                                    Optional bMultiSelect As Boolean = False) As OpenFileDialog

            Dim dlg As New OpenFileDialog()

            If (iDefaultFilter = -1) Then
                iDefaultFilter = IndexOf(m_lOpened.ToArray, strFilters)
            End If

            With dlg
                .FileName = strFileName
                .Filter = AddAllFilesEntry(strFilters)
                .FilterIndex = Math.Max(0, iDefaultFilter)
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
        Public Shared Function SaveFileDialog(strTitle As String,
                                    strFileName As String,
                                    strFilters As String,
                                    Optional iDefaultFilter As Integer = 0,
                                    Optional strInitialDirectory As String = "",
                                    Optional bOverwritePrompt As Boolean = True) As SaveFileDialog

            Dim dlg As New SaveFileDialog()

            If (iDefaultFilter = -1) Then
                iDefaultFilter = IndexOf(m_lSaved.ToArray, strFilters)
            End If

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
        Public Shared Function FolderBrowserDialog(strDescription As String,
                                                   strInitialDirectory As String) As OpenFileDialog

            'Dim dlg As New FolderBrowserDialog()

            'With dlg
            '    .SelectedPath = strInitialDirectory
            '    .ShowNewFolderButton = True
            '    .Description = strDescription
            'End With

            Dim dlg As New OpenFileDialog()

            ' Set the dialog title
            dlg.Title = strDescription

            ' This is important, it allows selecting folders
            dlg.CheckFileExists = False
            dlg.CheckPathExists = True

            ' Set the initial directory to something reasonable, like the desktop
            dlg.InitialDirectory = strInitialDirectory

            ' Use a dummy file name so that the dialog allows selecting folders
            dlg.FileName = My.Resources.PROMPT_FOLDER_SELECTION

            '' Show the dialog and process the result
            'If openFileDialog.ShowDialog() = DialogResult.OK Then
            '    ' Get the selected folder path
            '    Dim folderPath As String = IO.Path.GetDirectoryName(openFileDialog.FileName)
            '    MessageBox.Show("Selected Folder: " & folderPath)
            'End If
            ' Hack when SP1 installation is not detected
            Dim pi As PropertyInfo = GetType(FolderBrowserDialog).GetProperty("AutoUpgradeEnabled")
            If (pi IsNot Nothing) Then pi.SetValue(dlg, True, Nothing)

            Return dlg

        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Modify a file dialog filter by appending an 'all supported files' entry, if needed.
        ''' </summary>
        ''' <param name="strFilter">The filter to append the 'all supported files' entry to</param>
        ''' <returns>The appended filter.</returns>
        ''' <remarks>
        ''' The 'all supported files' entry is needed only if:
        ''' - the filter list contains more than one item (otherwise there is obviously no need to aggregate filters);
        ''' - the filter list does not already contain a "*.*" item.
        ''' </remarks>
        ''' -------------------------------------------------------------------
        Public Shared Function AddAllFilesEntry(strFilter As String) As String

            Dim bits As String() = strFilter.ToLower.Split("|"c)
            Dim lFilters As New List(Of String)
            Dim bNeedAllFilesEntry As Boolean = (bits.Length > 2)

            For i As Integer = 1 To bits.Length - 1 Step 2
                Dim filters As String() = bits(i).Split(";"c)
                For j As Integer = 0 To filters.Length - 1
                    If Not lFilters.Contains(filters(j)) Then
                        lFilters.Add(filters(j))
                        If (filters(j) = "*.*") Then bNeedAllFilesEntry = False
                    End If
                Next
                lFilters.Sort()
            Next

            If bNeedAllFilesEntry Then
                ' Append 'all supported files' entry
                Dim sbFilter As New StringBuilder(strFilter)
                sbFilter.Append("|")
                sbFilter.Append(My.Resources.LABEL_ALL_SUPPORTED_FILES)
                sbFilter.Append("|")
                For i As Integer = 0 To lFilters.Count - 1
                    If (i > 0) Then sbFilter.Append(";")
                    sbFilter.Append(lFilters(i))
                Next
                strFilter = sbFilter.ToString()
            End If

            Return strFilter

        End Function

        Private Shared m_lSaved As New List(Of String)
        Private Shared m_lOpened As New List(Of String)

        ''' <summary>
        ''' Get/set persistent last opene history
        ''' </summary>
        ''' <returns></returns>
        Public Shared Property LastOpenedHistory As String
            Get
                Return CollToString(m_lOpened.ToArray)
            End Get
            Set(value As String)
                m_lSaved.Clear()
                Dim items As String() = StringToColl(value)
                If (items IsNot Nothing) Then m_lOpened.AddRange(items)
            End Set
        End Property

        ''' <summary>
        ''' Get/set persistent last saved history
        ''' </summary>
        ''' <returns></returns>
        Public Shared Property LastSavedHistory As String
            Get
                Return CollToString(m_lSaved.ToArray)
            End Get
            Set(value As String)
                m_lSaved.Clear()
                Dim items As String() = StringToColl(value)
                If (items IsNot Nothing) Then m_lSaved.AddRange(items)
            End Set
        End Property

        Public Shared WriteOnly Property RecordLastOpened As String
            Set(file As String)
                Dim ext As String = System.IO.Path.GetExtension(file).ToLower
                m_lOpened.Remove(ext)
                m_lOpened.Add(ext)
                If m_lOpened.Count > 25 Then m_lOpened.RemoveAt(0)
            End Set
        End Property

        Public Shared WriteOnly Property RecordSavedFile As String
            Set(file As String)
                Dim ext As String = System.IO.Path.GetExtension(file).ToLower
                m_lSaved.Remove(ext)
                m_lSaved.Add(ext)
                If m_lSaved.Count > 25 Then m_lSaved.RemoveAt(0)
            End Set
        End Property

        Private Shared Function CollToString(items As String()) As String
            Dim sb As New StringBuilder
            For i As Integer = 0 To items.Length - 1
                If i > 0 Then sb.Append(";")
                sb.Append(items(i))
            Next
            Return sb.ToString()
        End Function

        Private Shared Function StringToColl(items As String) As String()
            Return items.Split({";"c}, StringSplitOptions.RemoveEmptyEntries)
        End Function

        Private Shared Function IndexOf(history As String(), filters As String) As Integer
            If (String.IsNullOrWhiteSpace(filters)) Then Return -1
            Dim bits() As String = filters.Split("|"c)
            For i As Integer = history.Length - 1 To 0 Step -1
                For j As Integer = 1 To bits.Length - 1 Step 2
                    For Each ext As String In bits(j).Split(";"c)
                        If String.Compare(ext, history(i), True) = 0 Then Return i
                    Next
                Next j
            Next i
            Return -1
        End Function

    End Class

End Namespace ' Controls
