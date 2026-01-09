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
' This plug-in was developed under the Safenet project, and has been contributed
' to the EwE approach by the Safenet project.
' 
' Copyright 1991- 
'    UBC Institute for the Oceans and Fisheries, Vancouver BC, Canada, and
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'



Imports System.IO
Imports System.Windows.Forms
Imports EwECore
Imports EwEUtils.Utilities
Imports ScientificInterfaceShared.Controls
Imports System.Drawing
Imports SharedResources = ScientificInterfaceShared.My.Resources
Imports EwECore.Common



''' <summary>
''' Utility to create biomass emitter time series files from before + after region
''' average CSV files.
''' </summary>
Public Class dlgBiomassEmitterTimeSeriesBuilder

#Region " Private classes "

    ' Sort entries by region
    Private Class cEntrySort
        Implements IComparer(Of cEntry)

        Public Function Compare(x As cEntry, y As cEntry) As Integer Implements IComparer(Of cEntry).Compare
            If (x.Region < y.Region) Then Return -1
            If (x.Region > y.Region) Then Return 1
            Return 0
        End Function

    End Class

    ''' <summary>
    ''' An Entry represents a time series for one specific region.
    ''' </summary>
    Private Class cEntry

        Private m_core As cCore = Nothing

        ''' <summary>
        ''' Values of (type, forcing data per group)
        ''' </summary>
        Private m_values As New Dictionary(Of Integer, Single())

        ''' <summary>
        ''' Internal admin, stating if there are values for a given group across all time steps
        ''' </summary>
        Private m_bHasValues() As Boolean

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Create a relative entry for two region average files: a before and an after file.
        ''' </summary>
        ''' <param name="core">The core to resolve group names for.</param>
        ''' <param name="before"></param>
        ''' <param name="after"></param>
        ''' -------------------------------------------------------------------
        Public Sub New(core As cCore, before As String, after As String)
            Me.m_core = core
            Me.IsRelative = True
            ReDim Me.m_bHasValues(Me.m_core.nGroups)
            Dim fn As String = Path.GetFileNameWithoutExtension(before).ToLower()
            Dim i As Integer = fn.IndexOf("region_")
            If (i > -1) Then
                Region = CInt(Val(fn.Substring(i + 7)))
            End If
            Me.DigestBeforeAfter(before, after)
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Create an absolute entry for a single region average file.
        ''' </summary>
        ''' <param name="core">The core to resolve group names for.</param>
        ''' <param name="file"></param>
        ''' -------------------------------------------------------------------
        Public Sub New(core As cCore, file As String)
            Me.m_core = core
            Me.IsRelative = False
            ReDim Me.m_bHasValues(Me.m_core.nGroups)
            Dim fn As String = Path.GetFileNameWithoutExtension(file).ToLower()
            Dim i As Integer = fn.IndexOf("region_")
            If (i > -1) Then
                Region = CInt(Val(fn.Substring(i + 7)))
            End If
            Me.DigestAbsolute(file)
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get the region number of this entry.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public ReadOnly Property Region As Integer

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get whether this is a relative (True) or absolute (False) time series.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public ReadOnly Property IsRelative As Boolean = True

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get the array of time stamps for data points.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public ReadOnly Property Times() As Integer()
            Get
                Return Me.m_values.Keys.ToArray()
            End Get
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get whether there is at least one entry for a given group.
        ''' </summary>
        ''' <param name="igroup">The one-based group index to check.</param>
        ''' <returns></returns>
        ''' -------------------------------------------------------------------
        Public ReadOnly Property HasValues(igroup As Integer) As Boolean
            Get
                Return Me.m_bHasValues(igroup)
            End Get
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get whether there is an entry for a given group at a given time step.
        ''' </summary>
        ''' <param name="t">The zero-based time index to check.</param>
        ''' <param name="iGroup">The one-based group index to check.</param>
        ''' <returns></returns>
        ''' -------------------------------------------------------------------
        Public ReadOnly Property HasValue(t As Integer, iGroup As Integer) As Boolean
            Get
                If (iGroup < 1) Then Return False
                If (Not Me.m_values.ContainsKey(t)) Then Return False
                If (Me.m_values.Count < iGroup) Then Return False
                Return (Me.m_values(t)(iGroup) > 0) And (Me.m_values(t)(iGroup) <> 1)
            End Get
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get the entry for a given group at a given time step. This will return
        ''' <see cref="cCore.NULL_VALUE"/> if <see cref="HasValue(Integer, Integer)">
        ''' no value could be found</see>.
        ''' </summary>
        ''' <param name="t"></param>
        ''' <param name="iGroup"></param>
        ''' <returns></returns>
        ''' -------------------------------------------------------------------
        Public ReadOnly Property Value(t As Integer, iGroup As Integer) As Single
            Get
                If Not Me.HasValue(t, iGroup) Then Return cCore.NULL_VALUE
                Return Me.m_values(t)(iGroup)
            End Get
        End Property

#Region " Internals "

        Private Sub DigestBeforeAfter(fileBefore As String, fileAfter As String)
            Me.Read(fileBefore, True)
            Me.Read(fileAfter, False)
            Me.Validate()
        End Sub

        Private Sub DigestAbsolute(file As String)
            Me.Read(file, True)
            Me.Validate()
        End Sub

        Private Function Read(f As String, bInit As Boolean) As Boolean

            Try
                Using sr As New StreamReader(f)
                    ' Skip header
                    Dim strLine As String = sr.ReadLine().ToLower()
                    While Not strLine.StartsWith("timestep") And Not sr.EndOfStream
                        strLine = sr.ReadLine().ToLower()
                    End While
                    If Not strLine.StartsWith("timestep") Then
                        Return False
                    End If

                    Dim names As String() = cStringUtils.SplitQualified(strLine, ",")
                    Dim nCols As Integer = names.Length - 1
                    Dim iGroups(nCols) As Integer
                    For i As Integer = 1 To names.Length - 1
                        iGroups(i) = Me.GetGroupNumber(Me.m_core, names(i))
                    Next

                    While Not sr.EndOfStream
                        strLine = sr.ReadLine().ToLower()
                        If Not String.IsNullOrWhiteSpace(strLine) Then
                            Dim bits As String() = cStringUtils.SplitQualified(strLine, ",")
                            Dim time As Integer = Integer.Parse(bits(0))
                            If bInit Then
                                Dim values(Me.m_core.nGroups) As Single
                                Me.m_values(time) = values
                            End If
                            For i As Integer = 1 To bits.Count - 1
                                Dim val As Single = Single.Parse(bits(i))
                                Dim iGroup As Integer = iGroups(i)
                                If (iGroup > 0) Then
                                    If (bInit) Then
                                        ' Store absolute value
                                        Me.m_values(time)(iGroup) = val
                                    Else
                                        ' Store relative value
                                        ' Beware: this value is NOT relative to the previous forcing value!
                                        Me.m_values(time)(iGroup) = val / Me.m_values(time)(iGroup)
                                    End If
                                End If
                            Next
                        End If
                    End While
                End Using

            Catch ex As Exception
                Return False
            End Try
            Return True

        End Function

        Private Sub Validate()
            For Each time As Integer In Me.m_values.Keys
                For iGroup As Integer = 1 To m_core.nGroups
                    If (Me.m_values(time)(iGroup) > 0) Then
                        Me.m_bHasValues(iGroup) = True
                    End If
                Next
            Next
        End Sub

        Private Function GetGroupNumber(core As cCore, strName As String) As Integer

            strName = strName.ToLower()
            For i As Integer = 1 To core.nGroups
                Dim grp As cEcoPathGroupInput = core.EcopathGroupInputs(i)
                If String.Compare(grp.Name, strName, True) = 0 Then
                    Return i
                End If
            Next
            Return -1

        End Function

#End Region ' Internals

    End Class

#End Region ' Private classes

#Region " Private vars "

    Private m_entries As New List(Of cEntry)
    Private m_uic As cUIContext = Nothing
    Private m_bIsRelative As Boolean = True
    Private m_bInUpdate As Boolean = False

#End Region ' Private vars

    Public Sub New(uic As cUIContext, Optional bLaunchedFromEmitter As Boolean = False)
        MyBase.New()
        Me.InitializeComponent()
        Me.Text = My.Resources.CAPTION_BUILDER
        Me.m_uic = uic
        Me.RunIntegrated = bLaunchedFromEmitter
    End Sub

#Region " Overrides "

    Protected Overrides Sub OnLoad(e As EventArgs)

        MyBase.OnLoad(e)
        Me.CenterToParent()

        Me.m_cbApplyOnSave.Visible = Me.RunIntegrated
        Me.m_btnCancel.Visible = Me.RunIntegrated
        Me.ControlBox = Not Me.RunIntegrated

        Me.UpdateControls()

    End Sub

#End Region ' Overrides

#Region " Public bits "

    Public ReadOnly Property FileName As String
        Get
            Return Me.m_tbxFile.Text
        End Get
    End Property

    Public ReadOnly Property LoadOnSave As Boolean
        Get
            Return Me.m_cbApplyOnSave.Checked
        End Get
    End Property

    Public Property IsRelativeTimeseries As Boolean
        Get
            Return Me.m_bIsRelative
        End Get
        Private Set(value As Boolean)
            If (value <> Me.m_bIsRelative) Then
                Me.m_entries.Clear()
                Me.m_bIsRelative = value
                Me.UpdateGrids()
                Me.UpdateControls()
            End If
        End Set
    End Property

    Public Property IsAbsoluteTimeSeries As Boolean
        Get
            Return Not Me.IsRelativeTimeseries
        End Get
        Set(value As Boolean)
            Me.IsRelativeTimeseries = Not value
        End Set
    End Property

#End Region ' Public bits 

#Region " Events "

    Private Sub OnRelativeSelected(sender As Object, e As EventArgs) Handles m_tsbnRelative.Click
        If (Me.m_bInUpdate) Then Return
        Me.IsRelativeTimeseries = True
    End Sub

    Private Sub OnAbsoluteSelected(sender As Object, e As EventArgs) Handles m_tsbnAbsolute.Click
        If (Me.m_bInUpdate) Then Return
        Me.IsRelativeTimeseries = False
    End Sub

    Private Sub OnAcceptFiles(sender As Object, e As cFileDropLabel.EwEFileDropArgs) Handles m_droppie.OnAcceptFiles

        If (Me.IsRelativeTimeseries) Then
            Dim before As String() = Nothing
            Dim after As String() = Nothing
            e.Accept = GetFilePairs(e.Files, False, before, after)
        Else
            Debug.Assert(False, "Not implemented yet")
        End If

    End Sub

    Private Sub OnFilesDropped(sender As Object, fileDropped() As String) Handles m_droppie.OnFilesDropped

        Dim core As cCore = Me.m_uic.Core

        Me.m_entries.Clear()

        Dim lFiles As New List(Of String)
        For Each f As String In fileDropped
            If Directory.Exists(f) Then
                lFiles.AddRange(Directory.GetFiles(f, "*.csv", SearchOption.TopDirectoryOnly))
            Else
                lFiles.Add(f)
            End If
        Next

        If (Me.IsRelativeTimeseries) Then
            Dim before As String() = Nothing
            Dim after As String() = Nothing
            If GetFilePairs(lFiles.ToArray, True, before, after) Then
                For i As Integer = 0 To before.Length - 1
                    Me.m_entries.Add(New cEntry(core, before(i), after(i)))
                Next
            End If
        Else
            Dim files As String() = Nothing
            If GetFiles(lFiles.ToArray, True, files) Then
                For i As Integer = 0 To files.Length - 1
                    Me.m_entries.Add(New cEntry(core, files(i)))
                Next
            End If
        End If

        Me.m_entries.Sort(New cEntrySort())
        Me.UpdateGrids()
        Me.UpdateControls()

    End Sub

    Private Sub OnSave(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles m_btnSave.Click

        If (Me.WriteFile(Me.m_tbxFile.Text)) And Me.RunIntegrated Then
            Me.DialogResult = DialogResult.OK
            Me.Close()
        End If

    End Sub

    Private Sub OnCancel(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles m_btnCancel.Click

        Me.DialogResult = DialogResult.Cancel
        Me.Close()

    End Sub

#End Region ' Events

#Region " Internals - input CSV handling "

    Private Class cPair
        Public Property Before As String = ""
        Public Property After As String = ""
        Public Sub Add(f As String, cl As eClassification)
            Select Case cl
                Case eClassification.Before
                    Before = f
                Case eClassification.After
                    After = f
                Case eClassification.Unknown
            End Select
        End Sub

        Public Function IsComplete() As Boolean
            Return Not String.IsNullOrWhiteSpace(Before) And Not String.IsNullOrWhiteSpace(After)
        End Function
    End Class

    Private Function GetFilePairs(files As String(), bQuiet As Boolean, ByRef before As String(), ByRef after As String()) As Boolean

        Debug.Assert(Me.IsRelativeTimeseries)

        If (files.Length Mod 2 = 1) Then
            Return False
        End If

        Dim dtFilePairs As New Dictionary(Of String, cPair)
        Dim lBefore As New List(Of String)
        Dim lAfter As New List(Of String)

        For i As Integer = 0 To files.Length - 1

            Dim n As String = ""
            Dim cl As eClassification = eClassification.Unknown

            If ClassifyPath(files(i), n, cl) Then
                If (Not dtFilePairs.ContainsKey(n)) Then dtFilePairs(n) = New cPair
                dtFilePairs(n).Add(files(i), cl)
            End If

        Next

        For Each p As cPair In dtFilePairs.Values()
            If p.IsComplete() Then
                lBefore.Add(p.Before)
                lAfter.Add(p.After)
            Else
                Return False
            End If
        Next

        before = lBefore.ToArray
        after = lAfter.ToArray
        Return True

    End Function

    Private Function GetFiles(files As String(), bQuiet As Boolean, ByRef filesOut As String()) As Boolean

        Debug.Assert(Me.IsAbsoluteTimeSeries)

        Dim lFiles As New List(Of String)

        For i As Integer = 0 To files.Length - 1

            Dim n As String = ""
            Dim cl As eClassification = eClassification.Unknown

            If ClassifyPath(files(i), n, cl) Then
                lFiles.Add(files(i))
            End If
        Next

        filesOut = lFiles.ToArray
        Return True

    End Function

    Private Enum eClassification As Integer
        Unknown
        Before
        After
    End Enum

    ''' <summary>
    ''' Try to decipher species name and <see cref="eClassification">before/after classification</see>
    ''' from a file path. Note that the classification is only completed when reading in <see cref="IsRelativeTimeseries">
    ''' relative time series</see>.
    ''' </summary>
    ''' <param name="file"></param>
    ''' <param name="name"></param>
    ''' <param name="classification"></param>
    ''' <returns></returns>
    Private Function ClassifyPath(file As String, ByRef name As String, ByRef classification As eClassification) As Boolean

        classification = eClassification.Unknown
        If (String.IsNullOrWhiteSpace(file)) Then Return False
        file = file.ToLower()
        If (Not Path.GetExtension(file) = ".csv") Then Return False

        name = Path.GetFileNameWithoutExtension(file)

        If (Not name.Contains("_region_")) Then Return False
        If (Not name.Contains("_biomass")) Then Return False
        If (name.Contains("_annual_")) Then Return False

        If (Me.IsRelativeTimeseries) Then
            If (name.StartsWith("before") Or name.EndsWith("before")) Then
                name = name.Replace("before", "").Trim()
                classification = eClassification.Before
            ElseIf (name.StartsWith("after") Or name.EndsWith("after")) Then
                name = name.Replace("after", "").Trim()
                classification = eClassification.Before
            End If

            If (classification = eClassification.Unknown) Then
                Dim pathbits() As String = Path.GetDirectoryName(file).Split(Path.DirectorySeparatorChar)
                Dim l As Integer = pathbits.Length
                If (l = 0) Then Return False
                If pathbits(l - 1).Contains("before") Then
                    classification = eClassification.Before
                ElseIf pathbits(l - 1).Contains("after") Then
                    classification = eClassification.After
                End If
            End If
        End If

        Return (classification <> eClassification.Unknown) Or (Me.IsAbsoluteTimeSeries)

    End Function

#End Region ' Internals - input CSV handling

#Region " Internals - UI "

    Private ReadOnly Property Core As cCore
        Get
            Return Me.m_uic.Core
        End Get
    End Property

    ''' <summary>
    ''' When integrated, the utility is invoked as a create-once dialog box directly 
    ''' from the biomass emitter UI. The dialog can be cancelled, and supports the 
    ''' option for the emitter to load a created file.
    ''' Then not integrated, the utility can be used to create a range of files. As 
    ''' such the utility stays open after saving, and needs to be dismissed through
    ''' the control box.
    ''' </summary>
    Private ReadOnly Property RunIntegrated As Boolean = False

    Private Sub UpdateGrids()

        Me.m_dgvSettings.SuspendLayout()

        ' Choppy
        Me.m_dgvSettings.Rows.Clear()
        While (Me.m_dgvSettings.Columns.GetColumnCount(DataGridViewElementStates.None) > 2)
            Me.m_dgvSettings.Columns.RemoveAt(2)
        End While

        ' Redefine new from data
        Dim cell As New DataGridViewCheckBoxCell()
        For iEntry As Integer = 0 To m_entries.Count - 1
            Dim col As New DataGridViewColumn(cell)
            Dim e As cEntry = Me.m_entries(iEntry)
            ' ToDo: globalize this
            col.Name = String.Format("Region {0}", e.Region)
            col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
            Me.m_dgvSettings.Columns.Add(col)
        Next

        Dim core As cCore = Me.m_uic.Core
        Dim nCols As Integer = Me.m_dgvSettings.Columns.GetColumnCount(DataGridViewElementStates.None)

        For iGroup As Integer = 1 To core.nGroups
            Dim data(nCols) As Object
            data(0) = iGroup
            data(1) = core.EcopathGroupInputs(iGroup).Name
            For iEntry As Integer = 0 To m_entries.Count - 1
                Dim entry As cEntry = m_entries(iEntry)
                data(iEntry + 2) = entry.HasValues(iGroup)
            Next
            Dim iRow As Integer = Me.m_dgvSettings.Rows.Add(data)
            ' Now set readonly states
            Dim row As DataGridViewRow = Me.m_dgvSettings.Rows(iRow)
            For i As Integer = 2 To nCols - 1
                row.Cells(i).ReadOnly = Not CBool(row.Cells(i).Value)
            Next
        Next

        Me.m_dgvSettings.ResumeLayout()

        Me.m_dgvMappings.SuspendLayout()
        Me.m_dgvMappings.Rows.Clear()
        For iEntry As Integer = 0 To m_entries.Count - 1
            Dim e As cEntry = Me.m_entries(iEntry)
            Dim data(3) As Object
            ' ToDo: globalize this
            data(0) = String.Format("Region {0}", e.Region)
            data(1) = e.Region
            Me.m_dgvMappings.Rows.Add(data)
        Next
        Me.m_dgvMappings.ResumeLayout()

    End Sub

    Private Sub UpdateControls()
        If (Me.m_bInUpdate) Then Return
        Me.m_bInUpdate = True
        Try
            Dim dir As String = Me.Core.DataSource.Directory
            Dim fn As String = cFileUtils.ToValidFileName(Path.GetFileNameWithoutExtension(Core.DataSource.FileName) & "_biomass_emitters_" & If(Me.IsRelativeTimeseries, "relative", "absolute") & ".csv", False)
            Me.m_tbxFile.Text = Path.Combine(dir, fn)

            Me.m_tsbnRelative.Checked = Me.IsRelativeTimeseries
            Me.m_tsbnAbsolute.Checked = Not Me.IsRelativeTimeseries

            Me.m_droppie.Text = If(Me.IsRelativeTimeseries, My.Resources.DROPLABEL_RELATIVE, My.Resources.DROPLABEL_ABSOLUTE)
        Catch ex As Exception

        End Try
        Me.m_bInUpdate = False
    End Sub

    Private Function WriteFile(f As String) As Boolean

        ' Gather which (entry, group) combos need to be saved to file
        Dim core As cCore = Me.m_uic.Core
        Dim data As New List(Of Point)
        Dim times As New List(Of Integer)
        Dim msg As cMessage = Nothing

        Dim iMappings(Me.m_entries.Count) As Integer
        For iEntry As Integer = 0 To Me.m_entries.Count - 1
            Dim e As cEntry = Me.m_entries(iEntry)
            If Not Integer.TryParse(CStr(Me.m_dgvMappings(2, iEntry).Value), iMappings(e.Region)) Then
                iMappings(e.Region) = e.Region
            End If
        Next

        For iGroup As Integer = 1 To core.nGroups
            For iEntry As Integer = 0 To Me.m_entries.Count - 1
                Dim cell As DataGridViewCell = Me.m_dgvSettings(2 + iEntry, iGroup - 1)
                Dim e As cEntry = Me.m_entries(iEntry)
                If CBool(cell.Value) Then
                    data.Add(New Point(iEntry, iGroup))
                    times.AddRange(e.Times)
                End If
            Next
        Next

        times.Sort()

        Try
            Using sw As New StreamWriter(f)
                sw.Write("Group")
                For i As Integer = 0 To data.Count - 1
                    Dim group As Integer = data(i).Y
                    sw.Write("," & group)
                Next
                sw.WriteLine()

                sw.Write("Target")
                For i As Integer = 0 To data.Count - 1
                    Dim e As cEntry = Me.m_entries(data(i).X)
                    sw.Write("," & iMappings(e.Region))
                Next
                sw.WriteLine()

                Dim tlast As Integer = -1
                Dim tNow As Integer = -1
                For t As Integer = 0 To times.Count - 1
                    tNow = times(t)
                    If (tlast <> tNow) Then
                        Dim dt As Date = core.EcospaceTimestepToAbsoluteTime(tNow)
                        sw.Write(dt.ToString("yyyy-MM"))
                        For i As Integer = 0 To data.Count - 1
                            Dim e As cEntry = Me.m_entries(data(i).X)
                            Dim group As Integer = data(i).Y
                            sw.Write(",")
                            If (e.HasValue(tNow, group)) Then
                                sw.Write(e.Value(tNow, group))
                            End If
                        Next
                        sw.WriteLine()
                        tlast = tNow
                    End If
                Next
                sw.Flush()
            End Using

        Catch ex As Exception
            msg = New cMessage(cStringUtils.Localize(My.Resources.STATUS_UTILITY_SAVE_ERROR, f, ex.Message), eMessageType.DataExport, eCoreComponentType.External, eMessageImportance.Critical)
            msg.Hyperlink = System.IO.Path.GetDirectoryName(f)
            core.Messages.SendMessage(msg)
            Return False
        End Try

        msg = New cMessage(cStringUtils.Localize(My.Resources.STATUS_UTILITY_SAVE_OK, f), eMessageType.DataExport, eCoreComponentType.External, eMessageImportance.Information)
        msg.Hyperlink = System.IO.Path.GetDirectoryName(f)
        Me.m_uic.Core.Messages.SendMessage(msg)

        Return True

    End Function

    Private Sub OnBrowseOutput(sender As Object, e As EventArgs) Handles m_btnBrowseOutput.Click

        Dim sfd As SaveFileDialog = cEwEFileDialogHelper.SaveFileDialog("Select output file", Me.m_tbxFile.Text, SharedResources.FILEFILTER_CSV)
        If (sfd.ShowDialog() = DialogResult.OK) Then
            Me.m_tbxFile.Text = sfd.FileName
        End If
        'Me.UpdateControls()

    End Sub


#End Region ' Internals

End Class
