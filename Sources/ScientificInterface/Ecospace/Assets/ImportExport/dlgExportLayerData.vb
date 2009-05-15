'==============================================================================
'
' $Log: dlgExportLayerData.vb,v $
' Revision 1.4  2009/05/15 14:12:12  jeroens
' Obtained layers properly disposed
'
' Revision 1.3  2009/05/11 01:50:52  jeroens
' Renamed command classes
'
' Revision 1.2  2008/11/12 00:40:21  jeroens
' Built initial mappings
'
' Revision 1.1  2008/11/10 23:12:52  jeroens
' Initial version
'
'==============================================================================

#Region " Imports "

Option Strict On

Imports System.IO
Imports System.Text
Imports EwECore
Imports EwEUtils.Commands
Imports EwEUtils.Utilities
Imports SAUPUtil.SAUPData
Imports SAUPUtil.SAUPFile
Imports ScientificInterface.Ecospace.Basemap.Layers
Imports SourceGrid2

#End Region ' Imports

Namespace Ecospace.Basemap

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' 
    ''' </summary>
    ''' ---------------------------------------------------------------------------
    Public Class dlgExportLayerData
        Inherits Form

#Region " Private classes "

        <CLSCompliant(False)> _
Public Class gridExportMappings
            Inherits EwEGrid

#Region " Private vars "

            ''' <summary>Custom <see cref="BehaviorModels.IBehaviorModel">behaviour model</see>
            ''' to trap cell edit events locally in this grid.</summary>
            Private m_bm As BehaviorModels.IBehaviorModel = New EndEditHandler(Me)
            ''' <summary>The layers to map upon.</summary>
            Private m_aLayers As cLayer()
            ''' <summary>Mappings. MAPPINGS!</summary>
            Private m_dtLayerMapping As New Dictionary(Of cLayer, String)

            Private Enum eColumnTypes As Integer
                ColumnLayer = 0
                ColumnAttribute
                ' Show datatype columns?
            End Enum

#End Region ' Private vars

#Region " Construction "

            Public Sub New()

            End Sub

#End Region ' Construction

#Region " Public interfaces "

            Public Property Layers() As cLayer()
                Get
                    Return Nothing
                End Get
                Set(ByVal value As cLayer())

                    Me.m_aLayers = value
                    Me.m_dtLayerMapping.Clear()

                    If value IsNot Nothing Then
                        For Each l As cLayer In value
                            Me.m_dtLayerMapping(l) = l.Name.Trim().Replace(" ", "")
                        Next
                    End If

                    Me.RefreshContent()
                End Set
            End Property

            Public Function Mappings() As Dictionary(Of cLayer, String)
                Return Me.m_dtLayerMapping
            End Function

#End Region ' Public interfaces

#Region " Overrides "

            Protected Overrides Sub InitStyle()
                MyBase.InitStyle()

                If Not Me.HasData() Then Return

                Me.Redim(1, System.Enum.GetValues(GetType(eColumnTypes)).Length)

                ' ToDo_JS: Globalize this
                Me(0, eColumnTypes.ColumnLayer) = New EwEColumnHeaderCell("Layer")
                Me(0, eColumnTypes.ColumnAttribute) = New EwEColumnHeaderCell("Attribute")

                Me.Columns(eColumnTypes.ColumnLayer).AutoSizeMode = SourceGrid2.AutoSizeMode.EnableAutoSize
                Me.Columns(eColumnTypes.ColumnAttribute).AutoSizeMode = SourceGrid2.AutoSizeMode.EnableStretch
                Me.AutoStretchColumnsToFitWidth = True
                Me.FixedColumns = 1

            End Sub

            Protected Overrides Sub FillData()

                If Not Me.HasData Then Return

                Me.RowsCount = 1

                Dim layer As cLayer = Nothing
                Dim ewec As EwECell = Nothing
                Dim cmb As Cells.Real.ComboBox = Nothing

                For iLayer As Integer = 0 To Me.m_aLayers.Length - 1

                    Me.AddRow()
                    layer = Me.m_aLayers(iLayer)

                    ewec = New EwECell(layer.Name, GetType(String))
                    ewec.Style = (StyleGuide.eStyleFlags.Names Or StyleGuide.eStyleFlags.NotEditable)
                    Me(iLayer + 1, eColumnTypes.ColumnLayer) = ewec

                    ewec = New EwECell(layer.Name, GetType(String))
                    ewec.Behaviors.Add(Me.m_bm)
                    Me(iLayer + 1, eColumnTypes.ColumnAttribute) = ewec

                    Me.Rows(iLayer + 1).Tag = layer

                Next iLayer

            End Sub

            Protected Overrides Sub FinishStyle()
                MyBase.FinishStyle()
                Me.FixedColumnWidths = False
            End Sub

            Protected Overrides Function DefaultDockStyle() As System.Windows.Forms.DockStyle
                Return Windows.Forms.DockStyle.None
            End Function

            Protected Overrides Function OnCellEdited(ByVal p As SourceGrid2.Position, ByVal cell As SourceGrid2.Cells.ICellVirtual) As Boolean

                Dim strAttribute As String = Me.AttributeAtRow(p.Row)
                Dim layer As cLayer = Me.LayerAtRow(p.Row)

                Try
                    Me.m_dtLayerMapping(layer) = strAttribute
                Catch ex As Exception
                End Try

                Return True

            End Function

            Private Function LayerAtRow(ByVal iRow As Integer) As cLayer
                If iRow > 0 And iRow < Me.RowsCount Then
                    Return DirectCast(Me.Rows(iRow).Tag, cLayer)
                End If
                Return Nothing
            End Function

            Private Function AttributeAtRow(ByVal iRow As Integer) As String
                If iRow > 0 And iRow < Me.RowsCount Then
                    Return CStr(Me(iRow, eColumnTypes.ColumnAttribute).Value)
                End If
                Return ""
            End Function

            Private Function HasData() As Boolean
                If Me.m_aLayers Is Nothing Then Return False
                Return True
            End Function

#End Region ' Overrides

        End Class

#End Region ' Private classes

#Region " Private vars "

        Private m_core As cCore = Nothing
        Private m_lLayers As New List(Of cLayer)
        Private m_bDataValid As Boolean = False
        Private m_data As cImportExportData = Nothing

#End Region ' Private vars

#Region " Constructor "

        Public Sub New()
            Me.InitializeComponent()
        End Sub

#End Region ' Constructor

#Region " Public properties "

        Public Property Layers() As cLayer()
            Get
                Return Me.m_lLayers.ToArray()
            End Get
            Set(ByVal aLayers As cLayer())
                Me.m_lLayers.Clear()

                If aLayers Is Nothing Then Return
                If aLayers.Length = 0 Then Return

                Me.m_lLayers.AddRange(aLayers)
            End Set
        End Property

#End Region ' Public properties

#Region " Events "

        Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)

            MyBase.OnLoad(e)

            Me.m_core = cCore.GetInstance()

            If (Me.m_lLayers.Count = 0) Then

                ' Add default layers
                Me.m_lLayers.AddRange(cLayerFactory.GetLayers(Me.m_core, EwEUtils.Core.eVarNameFlags.LayerImportance))
                Me.m_lLayers.AddRange(cLayerFactory.GetLayers(Me.m_core, EwEUtils.Core.eVarNameFlags.LayerDepth))
                Me.m_lLayers.AddRange(cLayerFactory.GetLayers(Me.m_core, EwEUtils.Core.eVarNameFlags.LayerHabitat))
                Me.m_lLayers.AddRange(cLayerFactory.GetLayers(Me.m_core, EwEUtils.Core.eVarNameFlags.LayerMPA))
                Me.m_lLayers.AddRange(cLayerFactory.GetLayers(Me.m_core, EwEUtils.Core.eVarNameFlags.LayerRelPP))
                Me.m_lLayers.AddRange(cLayerFactory.GetLayers(Me.m_core, EwEUtils.Core.eVarNameFlags.LayerRelCin))

            End If

            Me.m_grid.Layers = Me.m_lLayers.ToArray()

            Me.UpdateControls()

        End Sub

        Protected Overrides Sub OnFormClosing(ByVal e As System.Windows.Forms.FormClosingEventArgs)

            For Each layer As cLayer In Me.m_lLayers
                If layer IsNot Nothing Then
                    layer.Dispose()
                End If
            Next
            Me.m_lLayers = Nothing

            MyBase.OnFormClosing(e)
        End Sub

        Private Sub OnBrowseTarget(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_btnBrowseTarget.Click

            ' Browse via EwE6 open file dialog 
            Dim cmdh As cCommandHandler = cCommandHandler.GetInstance()
            Dim fsc As cFileSaveCommand = TryCast(cmdh.GetCommand(cFileSaveCommand.COMMAND_NAME), cFileSaveCommand)
            Dim strFileFilter As String = My.Resources.FILEFILTER_CSV

            ' Sanity check
            If fsc Is Nothing Then Return

            If String.IsNullOrEmpty(Me.m_tbTarget.Text) Then
                fsc.Invoke(strFileFilter)
            Else
                fsc.Invoke(Path.GetFileName(Me.m_tbTarget.Text), Path.GetDirectoryName(Me.m_tbTarget.Text), strFileFilter)
            End If

            If (fsc.Result = Windows.Forms.DialogResult.OK) Then
                Me.m_tbTarget.Text = fsc.FileName
            End If

        End Sub

        Private Sub OnOK(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_bntOK.Click

            If Not Me.SaveMappedLayers() Then Return

            Me.DialogResult = Windows.Forms.DialogResult.OK
            Me.Close()

        End Sub

#End Region ' Events

#Region " Internals "

        ''' <summary>
        ''' Write data to a shape file.
        ''' </summary>
        ''' <param name="strFile"></param>
        ''' <returns></returns>
        Private Function WriteCSVFile(ByVal strFile As String) As Boolean

            Dim tw As TextWriter = Nothing
            Dim sb As New StringBuilder()

            ' Write header line
            For iAttribute As Integer = 0 To Me.m_data.Attributes.Count - 1
                If iAttribute > 0 Then sb.Append(",")
                sb.Append(Me.m_data.Attributes(iAttribute))
            Next
            sb.AppendLine()

            For iCell As Integer = 0 To Me.m_data.NumCells - 1
                For iAttribute As Integer = 0 To Me.m_data.Attributes.Count - 1
                    If iAttribute > 0 Then sb.Append(",")
                    sb.Append(Me.m_data.Value(iCell, Me.m_data.Attributes(iAttribute)))
                Next iAttribute
                sb.AppendLine()
            Next iCell

            Try
                tw = New StreamWriter(strFile)
                tw.Write(sb.ToString())
                tw.Close()
            Catch ex As Exception
                Return False
            End Try

            Return True

        End Function

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Write data to a shape file.
        ''' </summary>
        ''' <returns></returns>
        ''' -----------------------------------------------------------------------
        Private Function WriteShapeFile(ByVal strFile As String) As Boolean

            Dim bm As cEcospaceBasemap = Me.m_core.EcospaceBasemap
            Dim sfio As New ShapeFileIO()
            Dim lsd As New List(Of SpatialData)
            Dim sd As SpatialData = Nothing
            Dim sValue As Single = 0.0!

            'Dim lstrAttributes As New List(Of String)

            'If Not sfio.Read(strFile, lsd) Then
            '    sfio.Close()
            '    Return eSpatialFileCompatibility.Unreadable
            'End If
            'sfio.Close()

            'If (lsd.Count = 0) Then Return eSpatialFileCompatibility.IncompatibleEmpty

            'For Each strAttribute As String In sfio.AttributeDefintions.Keys
            '    lstrAttributes.Add(strAttribute)
            'Next strAttribute
            'lstrAttributes.Sort()

            'Me.m_data = New cImportExportData(bm.InRow, bm.InCol, lstrAttributes.ToArray())

            'For iShape As Integer = 0 To lsd.Count - 1
            '    sd = lsd(iShape)
            '    For Each strAttribute As String In lstrAttributes
            '        sValue = CSng(Val(sd.GetAttribute(strAttribute)))
            '        Me.m_data.Value(iShape, strAttribute) = sValue
            '    Next strAttribute
            'Next iShape

            Return True

        End Function

        Private Function WriteAscFile(ByVal strFile As String) As Boolean

            Dim bm As cEcospaceBasemap = Me.m_core.EcospaceBasemap
            Dim sfio As New ASCIIFileIO()
            Dim rs As New Raster()
            Dim sd As SpatialData = Nothing
            Dim sValue As Single = 0.0!

            'If Not sfio.Read(strFile, rs) Then
            '    sfio.Close()
            '    Return eSpatialFileCompatibility.Unreadable
            'End If

            'sfio.Close()

            'If False Then
            '    ' Ask VC: ignore spatial extent?
            '    rs = rs.Project(New SpatialData.Extent(bm.Longitude, bm.Latitude, _
            '                                      bm.Longitude + bm.CellLength * bm.InCol, _
            '                                      bm.Latitude + bm.CellLength * bm.InRow))

            '    If (rs Is Nothing) Then Return eSpatialFileCompatibility.IncompatibleFormat
            '    If (rs.CellSize <> bm.CellLength) Then Return eSpatialFileCompatibility.IncompatibleDimensions
            'End If

            '' Create data without attributes, row and col pos are implicit
            'Me.m_data = New cImportExportData(bm.InRow, bm.InCol)

            'For iRow As Integer = 1 To bm.InRow
            '    For icol As Integer = 1 To bm.InCol
            '        Me.m_data.Value(iRow - 1, icol - 1, cImportExportData.cMAPPING_IMPLICIT) = rs.GetCell(icol - 1, iRow - 1)
            '    Next
            'Next

            'Me.m_cmbRow.Items.Add(My.Resources.VALUE_NOTAVAILABLE) : Me.m_cmbRow.SelectedIndex = 0
            'Me.m_cmbCol.Items.Add(My.Resources.VALUE_NOTAVAILABLE) : Me.m_cmbCol.SelectedIndex = 0
            'Me.m_grid.Attributes = New String() {cImportExportData.cMAPPING_IMPLICIT}

            Return True

        End Function

        Private Function SaveMappedLayers() As Boolean

            Dim dtMappings As Dictionary(Of cLayer, String) = Me.m_grid.Mappings()
            Dim bm As cEcospaceBasemap = Me.m_core.EcospaceBasemap
            Dim lstrAttributes As New List(Of String)
            Dim strAttribute As String = ""
            Dim strFile As String = Me.m_tbTarget.Text
            Dim layer As cLayer = Nothing
            Dim iRow As Integer = 0
            Dim iCol As Integer = 0
            Dim iCell As Integer = 0

            AppLauncher.GetInstance().SetStatusText(My.Resources.STATUS_APPLYVALUES, TriState.True)

            ' Populate local data
            For Each layer In dtMappings.Keys
                strAttribute = dtMappings(layer).Trim
                If Not String.IsNullOrEmpty(strAttribute) Then
                    If (lstrAttributes.IndexOf(strAttribute) = -1) Then
                        lstrAttributes.Add(strAttribute)
                    End If
                End If
            Next
            ' Yippee
            lstrAttributes.Sort()
            lstrAttributes.Insert(0, Me.RowAttribute)
            lstrAttributes.Insert(0, Me.ColAttribute)

            ' Create data
            Me.m_data = New cImportExportData(bm.InRow, bm.InCol, lstrAttributes.ToArray())

            ' Store layer
            For iRow = 1 To bm.InRow
                For iCol = 1 To bm.InCol
                    ' Populate row, col value (duh!)
                    Me.m_data.Value(iRow - 1, iCol - 1, Me.RowAttribute) = CSng(iRow)
                    Me.m_data.Value(iRow - 1, iCol - 1, Me.ColAttribute) = CSng(iCol)

                    ' Populate data
                    For Each layer In dtMappings.Keys
                        strAttribute = dtMappings(layer)
                        If Not String.IsNullOrEmpty(strAttribute.Trim) Then
                            Me.m_data.Value(iRow - 1, iCol - 1, strAttribute) = CSng(layer.Value(iRow, iCol))
                        End If
                    Next layer
                Next iCol
            Next iRow

            Select Case Path.GetExtension(strFile).ToLower
                Case ".asc"
                    Me.WriteAscFile(strFile)
                Case ".csv" ' csv
                    Me.WriteCSVFile(strFile)
                Case ".shp" ' shp
                    Me.WriteShapeFile(strFile)
            End Select

            AppLauncher.GetInstance().SetStatusText("", TriState.False)

            Return True

        End Function

        Private Property RowAttribute() As String
            Get
                Return Me.m_tbRow.Text
            End Get
            Set(ByVal value As String)
                Me.m_tbRow.Text = value
            End Set
        End Property

        Private Property ColAttribute() As String
            Get
                Return Me.m_tbCol.Text
            End Get
            Set(ByVal value As String)
                Me.m_tbCol.Text = value
            End Set
        End Property

        Private Sub UpdateControls()

            'Me.m_cmbRow.Enabled = (Me.m_cmbRow.Items.Count > 0)
            'Me.m_cmbCol.Enabled = (Me.m_cmbCol.Items.Count > 0)

            'Me.m_grid.Enabled = Me.m_bDataValid
            'Me.m_bntOK.Enabled = Me.m_bDataValid

        End Sub

#End Region ' Internals

#Region " DevStudio generated surprises "

        'Form overrides dispose to clean up the component list.
        <System.Diagnostics.DebuggerNonUserCode()> _
        Protected Overrides Sub Dispose(ByVal disposing As Boolean)
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
            MyBase.Dispose(disposing)
        End Sub

        'Required by the Windows Form Designer
        Private components As System.ComponentModel.IContainer

        'NOTE: The following procedure is required by the Windows Form Designer
        'It can be modified using the Windows Form Designer.  
        'Do not modify it using the code editor.
        <System.Diagnostics.DebuggerStepThrough()> _
        Private Sub InitializeComponent()
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(dlgExportLayerData))
            Me.m_lblTarget = New System.Windows.Forms.Label
            Me.m_tbTarget = New System.Windows.Forms.TextBox
            Me.m_btnBrowseTarget = New System.Windows.Forms.Button
            Me.m_lblMappings = New System.Windows.Forms.Label
            Me.m_tlpOkCancel = New System.Windows.Forms.TableLayoutPanel
            Me.m_bntOK = New System.Windows.Forms.Button
            Me.m_btnCancel = New System.Windows.Forms.Button
            Me.m_grid = New gridExportMappings
            Me.m_lblRow = New System.Windows.Forms.Label
            Me.m_lblCol = New System.Windows.Forms.Label
            Me.m_tbRow = New System.Windows.Forms.TextBox
            Me.m_tbCol = New System.Windows.Forms.TextBox
            Me.m_tlpOkCancel.SuspendLayout()
            Me.SuspendLayout()
            '
            'm_lblTarget
            '
            resources.ApplyResources(Me.m_lblTarget, "m_lblTarget")
            Me.m_lblTarget.Name = "m_lblTarget"
            '
            'm_tbTarget
            '
            resources.ApplyResources(Me.m_tbTarget, "m_tbTarget")
            Me.m_tbTarget.Name = "m_tbTarget"
            '
            'm_btnBrowseTarget
            '
            resources.ApplyResources(Me.m_btnBrowseTarget, "m_btnBrowseTarget")
            Me.m_btnBrowseTarget.Image = Global.ScientificInterface.My.Resources.Resources.openHS
            Me.m_btnBrowseTarget.Name = "m_btnBrowseTarget"
            Me.m_btnBrowseTarget.UseVisualStyleBackColor = True
            '
            'm_lblMappings
            '
            resources.ApplyResources(Me.m_lblMappings, "m_lblMappings")
            Me.m_lblMappings.Name = "m_lblMappings"
            '
            'm_tlpOkCancel
            '
            resources.ApplyResources(Me.m_tlpOkCancel, "m_tlpOkCancel")
            Me.m_tlpOkCancel.Controls.Add(Me.m_bntOK, 0, 0)
            Me.m_tlpOkCancel.Controls.Add(Me.m_btnCancel, 1, 0)
            Me.m_tlpOkCancel.Name = "m_tlpOkCancel"
            '
            'm_bntOK
            '
            resources.ApplyResources(Me.m_bntOK, "m_bntOK")
            Me.m_bntOK.Name = "m_bntOK"
            '
            'm_btnCancel
            '
            resources.ApplyResources(Me.m_btnCancel, "m_btnCancel")
            Me.m_btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
            Me.m_btnCancel.Name = "m_btnCancel"
            '
            'm_grid
            '
            resources.ApplyResources(Me.m_grid, "m_grid")
            Me.m_grid.AutoSizeMinHeight = 10
            Me.m_grid.AutoSizeMinWidth = 10
            Me.m_grid.AutoStretchColumnsToFitWidth = False
            Me.m_grid.AutoStretchRowsToFitHeight = False
            Me.m_grid.BackColor = System.Drawing.Color.White
            Me.m_grid.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
            Me.m_grid.ContextMenuStyle = CType((((SourceGrid2.ContextMenuStyle.ColumnResize Or SourceGrid2.ContextMenuStyle.AutoSize) _
                        Or SourceGrid2.ContextMenuStyle.CopyPasteSelection) _
                        Or SourceGrid2.ContextMenuStyle.CellContextMenu), SourceGrid2.ContextMenuStyle)
            Me.m_grid.CustomSort = False
            Me.m_grid.FixedColumnWidths = False
            Me.m_grid.FocusStyle = SourceGrid2.FocusStyle.None
            Me.m_grid.GridToolTipActive = True
            Me.m_grid.Layers = Nothing
            Me.m_grid.Name = "m_grid"
            Me.m_grid.SpecialKeys = CType((((((((((SourceGrid2.GridSpecialKeys.Ctrl_C Or SourceGrid2.GridSpecialKeys.Ctrl_V) _
                        Or SourceGrid2.GridSpecialKeys.Ctrl_X) _
                        Or SourceGrid2.GridSpecialKeys.Delete) _
                        Or SourceGrid2.GridSpecialKeys.Arrows) _
                        Or SourceGrid2.GridSpecialKeys.Tab) _
                        Or SourceGrid2.GridSpecialKeys.PageDownUp) _
                        Or SourceGrid2.GridSpecialKeys.Enter) _
                        Or SourceGrid2.GridSpecialKeys.Escape) _
                        Or SourceGrid2.GridSpecialKeys.Backspace), SourceGrid2.GridSpecialKeys)
            '
            'm_lblRow
            '
            resources.ApplyResources(Me.m_lblRow, "m_lblRow")
            Me.m_lblRow.Name = "m_lblRow"
            '
            'm_lblCol
            '
            resources.ApplyResources(Me.m_lblCol, "m_lblCol")
            Me.m_lblCol.Name = "m_lblCol"
            '
            'm_tbRow
            '
            resources.ApplyResources(Me.m_tbRow, "m_tbRow")
            Me.m_tbRow.Name = "m_tbRow"
            '
            'm_tbCol
            '
            resources.ApplyResources(Me.m_tbCol, "m_tbCol")
            Me.m_tbCol.Name = "m_tbCol"
            '
            'dlgExportLayerData
            '
            Me.AcceptButton = Me.m_bntOK
            Me.CancelButton = Me.m_btnCancel
            resources.ApplyResources(Me, "$this")
            Me.ControlBox = False
            Me.Controls.Add(Me.m_tbCol)
            Me.Controls.Add(Me.m_tbRow)
            Me.Controls.Add(Me.m_lblCol)
            Me.Controls.Add(Me.m_lblRow)
            Me.Controls.Add(Me.m_tlpOkCancel)
            Me.Controls.Add(Me.m_grid)
            Me.Controls.Add(Me.m_lblMappings)
            Me.Controls.Add(Me.m_tbTarget)
            Me.Controls.Add(Me.m_btnBrowseTarget)
            Me.Controls.Add(Me.m_lblTarget)
            Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
            Me.Name = "dlgExportLayerData"
            Me.ShowIcon = False
            Me.ShowInTaskbar = False
            Me.m_tlpOkCancel.ResumeLayout(False)
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub

        Private WithEvents m_lblTarget As System.Windows.Forms.Label
        Private WithEvents m_tbTarget As System.Windows.Forms.TextBox
        Private WithEvents m_btnBrowseTarget As System.Windows.Forms.Button
        Private WithEvents m_lblMappings As System.Windows.Forms.Label
        Private WithEvents m_grid As gridExportMappings
        Private WithEvents m_tlpOkCancel As System.Windows.Forms.TableLayoutPanel
        Private WithEvents m_bntOK As System.Windows.Forms.Button
        Private WithEvents m_btnCancel As System.Windows.Forms.Button
        Private WithEvents m_lblRow As System.Windows.Forms.Label
        Private WithEvents m_lblCol As System.Windows.Forms.Label
        Private WithEvents m_tbRow As System.Windows.Forms.TextBox
        Private WithEvents m_tbCol As System.Windows.Forms.TextBox

#End Region ' DevStudio generated surprises

    End Class

End Namespace ' Ecospace.Basemap
