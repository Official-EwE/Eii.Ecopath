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
Imports System.Collections.Generic
Imports System.IO
Imports System.Windows.Forms
Imports EwECore.SpatialData
Imports EwEUtils.Commands
Imports EwEUtils.Core
Imports EwEUtils.SpatialData
Imports EwEUtils.Utilities
Imports ScientificInterfaceShared.Commands
Imports ScientificInterfaceShared.Controls
Imports ScientificInterfaceShared.Style
Imports System.Text

#End Region ' Imports

Namespace SpatialData

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Configuration interface for <see cref="cMultiFileDataSetPlugin"/>s.
    ''' </summary>
    ''' ---------------------------------------------------------------------------
    Friend Class ucMultiFileDatasetConfigPage
        Implements IUIElement
        Implements IOptionsPage

#Region " Private classes "

        Private Class cFileEntry
            Private m_strFileName As String
            Public Sub New(strFileName As String, dt As Date)
                Me.FileDate = dt
                Me.m_strFileName = strFileName
            End Sub
            Public ReadOnly Property FileName As String
                Get
                    Return Me.m_strFileName
                End Get
            End Property
            Public Property FileDate As DateTime
        End Class

#End Region ' Private classes

#Region " Private vars "

        Private Enum eIntervalType
            Month = 0
            ThreeMonths
            HalfYear
            Year
            Decade
        End Enum

        Private m_dataset As cMultiFileDataSetPlugin = Nothing
        Private m_lFiles As New List(Of cFileEntry)
        Private m_strSource As String = ""

#End Region ' Private vars

#Region " Overrides "

        Public Property UIContext As ScientificInterfaceShared.Controls.cUIContext _
            Implements ScientificInterfaceShared.Controls.IUIElement.UIContext

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' 
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)
            MyBase.OnLoad(e)

            If (Me.m_dataset Is Nothing) Then
                Me.m_dataset = New cMultiFileDataSetPlugin()
            End If

            Dim aTimes As DateTime() = Me.m_dataset.TimeSteps
            Dim dt As DateTime = Nothing
            Dim iRow As Integer = 0
            Dim strFile As String = ""

            Me.m_mtbIntervalStart.ValidatingType = GetType(Date)
            For i As Integer = 0 To aTimes.Length - 1
                dt = aTimes(i)
                If (i = 0) Then Me.m_mtbIntervalStart.Text = dt.ToString("yyyy") & dt.ToString("MM")
                Me.m_lFiles.Add(New cFileEntry(Me.m_dataset.File(dt), dt))
            Next

            Me.UpdateGrid()

            Me.m_tbxName.Text = Me.m_dataset.DisplayName
            Me.m_tbxDescription.Text = Me.m_dataset.DataDescription
            Me.m_strSource = Me.m_dataset.Source

            Me.m_cmbInterval.SelectedIndex = 0

            Me.m_cbSeasonal.Checked = Me.m_dataset.IsSeasonal
            Me.m_mtbSeasonalEnd.ValidatingType = GetType(Date)
            Me.m_mtbSeasonalEnd.Text = Me.m_dataset.TimeEnd.ToString("yyyy") & Me.m_dataset.TimeEnd.ToString("MM")

            ' Allow all supported varnames
            Me.m_cmbVarName.Items.Add(eVarNameFlags.NotSet)
            If (Me.UIContext IsNot Nothing) Then
                For Each adt As cSpatialDataAdapter In Me.UIContext.Core.SpatialDataConnectionManager.Adapters
                    Me.m_cmbVarName.Items.Add(adt.VarName)
                Next
            End If

            Me.m_cmbVarName.SelectedItem = Me.m_dataset.VarName

            ' Set dynamic properties
            Me.m_hdrDescription.CollapsedParentHeight = Me.m_tbxDescription.Location.Y + (Me.m_plDescription.Height - Me.m_cmbVarName.Location.Y)
            Me.m_hdrDescription.IsCollapsed = False

            ' Set dynamic properties
            Me.m_hdrTime.CollapsedParentHeight = Me.m_rbFromDate.Location.Y
            Me.m_hdrTime.IsCollapsed = True

        End Sub

#End Region ' Overrides

#Region " Interface implementation "

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' 
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Public Property Dataset As EwEUtils.SpatialData.ISpatialDataSet
            Get
                Return Me.m_dataset
            End Get
            Set(ByVal value As EwEUtils.SpatialData.ISpatialDataSet)
                Debug.Assert(TypeOf value Is cMultiFileDataSetPlugin)
                Me.m_dataset = DirectCast(value, cMultiFileDataSetPlugin)
            End Set
        End Property

        Public Function CanApply() As Boolean _
            Implements IOptionsPage.CanApply
            Return (Me.m_lFiles.Count > 0)
        End Function

        Public Event OnMultiFileConfigPageChanged(sender As IOptionsPage, args As System.EventArgs) _
            Implements ScientificInterfaceShared.Controls.IOptionsPage.OnChanged

        Public Function Apply() As IOptionsPage.eApplyResultType _
            Implements IOptionsPage.Apply
            Try
                Me.DoApply()
            Catch ex As Exception
                Return IOptionsPage.eApplyResultType.Failed
            End Try
            Return IOptionsPage.eApplyResultType.Success
        End Function

        Public Sub SetDefaults() _
            Implements IOptionsPage.SetDefaults
            ' NOP
        End Sub

#End Region ' Interface implementation

#Region " Control events "

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' 
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Private Sub OnBrowse(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_btnBrowse.Click
            Me.DoBrowse()
        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' 
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Private Sub OnFriendlyInfoChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_tbxName.TextChanged, m_tbxDescription.TextChanged
            Me.UpdateControls()
        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' 
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Private Sub OnGridCellValueChanged(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) _
            Handles m_dgvFiles.CellValueChanged

            If (e.RowIndex < 0) Or (e.ColumnIndex <> 1) Then Return

            Dim row As DataGridViewRow = Me.m_dgvFiles.Rows(e.RowIndex)
            Dim dt As Date = CDate(row.Cells(1).Value)
            Dim entry As cFileEntry = DirectCast(row.Tag, cFileEntry)

            entry.FileDate = dt

        End Sub

        Private Sub OnGridSelectionChanged(ByVal sender As Object, ByVal e As System.EventArgs) _
            Handles m_dgvFiles.SelectionChanged

            Dim row As DataGridViewRow = Nothing
            If (Me.m_dgvFiles.SelectedRows.Count > 0) Then
                row = Me.m_dgvFiles.SelectedRows(0)
            End If

            If (row IsNot Nothing) Then
                Me.RefreshDataPartSample(CStr(row.Cells(0).Value))
            End If

        End Sub

        Private Sub OnSetTime(ByVal sender As Object, ByVal e As EventArgs) _
            Handles m_btnSetTime.Click

            For i As Integer = 0 To Me.m_lFiles.Count - 1
                Dim fe As cFileEntry = Me.m_lFiles(i)
                fe.FileDate = Me.ToFileDate(i, fe.FileName)
            Next
            Me.UpdateGrid()

        End Sub

        Private Sub OnFormatVarname(sender As Object, e As System.Windows.Forms.ListControlConvertEventArgs) _
            Handles m_cmbVarName.Format
            Dim fmt As New cVarnameTypeFormatter
            e.Value = fmt.GetDescriptor(e.ListItem)
        End Sub

        Private Sub OnSeasonalCheckChanged(sender As System.Object, e As System.EventArgs) _
            Handles m_cbSeasonal.CheckedChanged
            Try
                Me.m_dataset.IsSeasonal = Me.m_cbSeasonal.Checked
                Me.UpdateControls()
            Catch ex As Exception

            End Try
        End Sub

        Private Sub OnCollapsed(sender As Object, args As cEwEHeaderLabel.cCollapsedEventArgs) _
            Handles m_hdrDescription.OnCollapsed
            Try
                Me.UpdateControls()
            Catch ex As Exception

            End Try
        End Sub

#End Region ' Control events

#Region " Internals "

        Private Sub DoBrowse()

            Dim ofd As New OpenFileDialog
            Dim sbFileName As New StringBuilder()

            ofd.Title = String.Format(My.Resources.PROMPT_SELECTFILES, Me.m_tbxName.Text)
            ofd.Multiselect = True
            ofd.InitialDirectory = Me.AbsolutePath()
            ofd.Filter = Me.m_dataset.DialogReadFilter(True, False, True)

            ' Pre-selecting current files does not work somehow
            'For i As Integer = 0 To Me.m_lFiles.Count - 1
            '    If (i > 0) Then sbFileName.Append(" ")
            '    sbFileName.Append("""" & Path.GetFileName(Me.m_lFiles(i).FileName) & """")
            'Next
            'ofd.FileName = sbFileName.ToString()

            If (ofd.ShowDialog() = DialogResult.OK) Then

                Me.m_strSource = ofd.InitialDirectory

                ' Merge file list
                Dim lFilesTemp As cFileEntry() = Me.m_lFiles.ToArray()
                Dim strFile As String = ""
                Dim bFound As Boolean = False

                Me.m_lFiles.Clear()

                For i As Integer = 0 To ofd.FileNames.Length - 1

                    strFile = ofd.FileNames(i)
                    bFound = False

                    ' Maintain original file def, if already present
                    For Each fe As cFileEntry In lFilesTemp
                        If (cFileUtils.Equals(fe.FileName, strFile, True)) Then
                            Me.m_lFiles.Add(fe)
                            bFound = True
                        End If
                    Next

                    If (Not bFound) Then
                        Dim dt As DateTime = Me.ToFileDate(i, ofd.FileNames(i))
                        Dim fe As New cFileEntry(ofd.FileNames(i), dt)
                        Me.m_lFiles.Add(fe)
                    End If
                Next

                Me.UpdateGrid()
                Me.UpdateControls()
            End If

        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' 
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Private Sub UpdateControls()

            ' Prevent intiialization errors
            If (Me.m_dataset Is Nothing) Then Return

            Dim bHasPattern As Boolean = (Not String.IsNullOrEmpty(Me.m_tbxDatePart.SelectedText))
            Dim strPath As String = Me.AbsolutePath()
            Dim bHasFolder As Boolean = False

            Me.m_mtbSeasonalEnd.Enabled = Me.m_cbSeasonal.Checked
            Me.m_lblDescription.Visible = (Not Me.m_hdrDescription.IsCollapsed)

            Try
                If Not String.IsNullOrWhiteSpace(strPath) Then
                    bHasFolder = Directory.Exists(Path.GetFullPath(strPath))
                End If
            Catch ex As Exception

            End Try

        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Get the absolute path for the current dataset.
        ''' </summary>
        ''' <returns></returns>
        ''' -----------------------------------------------------------------------
        Private Function AbsolutePath() As String
            Dim strPath As String = Me.m_strSource
            If (Me.m_dataset.IsSourceRelative) Then
                Return Me.m_dataset.ToAbsolutePath(strPath)
            End If
            Return strPath
        End Function

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' 
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Private Sub RefreshDataPartSample(Optional ByVal strFile As String = "")

            Dim iSel As Integer = Me.m_tbxDatePart.SelectionStart
            Dim iLen As Integer = Me.m_tbxDatePart.SelectionLength

            If (String.IsNullOrWhiteSpace(strFile)) Then
                If (Me.m_lFiles.Count > 0) Then
                    strFile = Me.m_lFiles(0).FileName
                End If
            End If

            If Not String.IsNullOrWhiteSpace(strFile) Then
                Me.m_tbxDatePart.Text = Path.GetFileName(strFile)
                Me.m_tbxDatePart.SelectionStart = iSel
                Me.m_tbxDatePart.SelectionLength = iLen
            Else
                Me.m_tbxDatePart.Text = String.Empty
            End If

        End Sub

        Private Sub UpdateGrid()

            Me.Cursor = Cursors.WaitCursor
            Me.m_dgvFiles.SuspendLayout()
            Me.m_dgvFiles.Rows.Clear()

            For i As Integer = 0 To Me.m_lFiles.Count - 1
                Dim entry As cFileEntry = Me.m_lFiles(i)
                Dim iRow As Integer = Me.m_dgvFiles.Rows.Add(Path.GetFileName(entry.FileName), entry.FileDate)
                Me.m_dgvFiles.Rows(iRow).Tag = entry
            Next

            Me.m_dgvFiles.ResumeLayout()
            Me.Cursor = Cursors.Default

        End Sub

        Private Sub DoApply()

            Me.m_dataset.DisplayName = Me.m_tbxName.Text
            Me.m_dataset.DataDescription = Me.m_tbxDescription.Text
            Me.m_dataset.Source = Me.m_strSource
            Me.m_dataset.VarName = DirectCast(Me.m_cmbVarName.SelectedItem, eVarNameFlags)
            Me.m_dataset.IsSeasonal = Me.m_cbSeasonal.Checked
            Me.m_dataset.SeasonsEnd = CType(Me.m_mtbSeasonalEnd.ValidateText, Date)

            Me.m_dataset.Clear()
            For Each entry As cFileEntry In Me.m_lFiles
                Me.m_dataset.File(entry.FileDate) = entry.FileName
            Next entry

        End Sub

#Region " Date helpers "

        Private Function ToFileDate(iFile As Integer, strFile As String) As Date

            Dim dtStart As Date = CType(Me.m_mtbIntervalStart.ValidateText, Date)
            Dim interval As eIntervalType = DirectCast(Me.m_cmbInterval.SelectedIndex, eIntervalType)
            Dim dt As Date

            If Me.m_rbInterval.Checked Then
                dt = Me.GetDateFromInterval(dtStart.Year, dtStart.Month, iFile, interval)
            ElseIf Me.m_rbFromName.Checked Then
                dt = Me.GetDateFromFileName(Path.Combine(Me.m_strSource, strFile))
            ElseIf Me.m_rbFromDate.Checked Then
                dt = Me.GetDateFromFile(Path.Combine(Me.m_strSource, strFile))
            End If
            Return dt

        End Function

        Private Function GetDateFromInterval(ByVal iYear As Integer, ByVal iMonth As Integer, ByVal iFile As Integer, ByVal interval As eIntervalType) As Date

            Dim dt As New Date(iYear, iMonth, 1)
            Select Case interval
                Case eIntervalType.Month
                    Return dt.AddMonths(iFile)
                Case eIntervalType.ThreeMonths
                    Return dt.AddMonths(iFile * 3)
                Case eIntervalType.HalfYear
                    Return dt.AddMonths(iFile * 6)
                Case eIntervalType.Year
                    Return dt.AddYears(iFile)
                Case eIntervalType.Decade
                    Return dt.AddYears(iFile * 10)
            End Select
            Return Date.MinValue

        End Function

        Private Function GetDateFromFile(ByVal strFile As String) As Date
            Return File.GetCreationTime(strFile)
        End Function

        Private Function GetDateFromFileName(ByVal strFile As String) As Date
            Dim dt As Date = Date.MinValue
            DateTime.TryParse(strFile.Substring(Me.m_tbxDatePart.SelectionStart, Me.m_tbxDatePart.SelectionLength), dt)
            Return dt
        End Function

#End Region ' Date helpers

#End Region ' Internals

    End Class

End Namespace
