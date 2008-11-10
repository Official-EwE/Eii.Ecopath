'==============================================================================
'
' $Log: dlgImportLayerData.vb,v $
' Revision 1.3  2008/11/10 23:12:26  jeroens
' Uses status feedback
'
' Revision 1.2  2008/11/10 18:25:43  jeroens
' Integrated grid
'
' Revision 1.1  2008/11/10 02:25:52  jeroens
' Renamed
'
' Revision 1.2  2008/11/10 01:51:52  jeroens
' Added .asc support
'
' Revision 1.1  2008/11/08 23:44:48  jeroens
' Supports CSV and SHP
'
'==============================================================================

#Region " Imports "

Option Strict On

Imports System.IO
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
    Public Class dlgImportLayerData
        Inherits Form

#Region " Private classes "

        <CLSCompliant(False)> _
Public Class gridMapLayerToAttribute
            Inherits EwEGrid

            ' ToDo: Sort and display layers by group
            ' ToDo: Accept Attributes as delivered by SAUPUtil so datatype can be verified
            ' ToDo: Do not allow incompatible data types to be linked

#Region " Private vars "

            Private Const cVALUE_NONE As String = " "

            ''' <summary>Custom <see cref="BehaviorModels.IBehaviorModel">behaviour model</see>
            ''' to trap cell edit events locally in this grid.</summary>
            Private m_bm As BehaviorModels.IBehaviorModel = New EndEditHandler(Me)

            ''' <summary>The layers to map upon.</summary>
            Private m_aLayers As cLayer()
            ''' <summary>The attribute names to map upon.</summary>
            Private m_astrAttributes As String()
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
                    Me.RefreshContent()
                End Set
            End Property

            Public Property Attributes() As String()
                Get
                    Return Me.m_astrAttributes
                End Get
                Set(ByVal value As String())
                    Dim lstr As New List(Of String)
                    If (value IsNot Nothing) Then lstr.AddRange(value)
                    If lstr.IndexOf(cVALUE_NONE) = -1 Then lstr.Insert(0, cVALUE_NONE)
                    Me.m_astrAttributes = lstr.ToArray()
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

                    cmb = New Cells.Real.ComboBox("", GetType(String), Me.m_astrAttributes, True)
                    cmb.EditableMode = EditableMode.SingleClick
                    Me(iLayer + 1, eColumnTypes.ColumnAttribute) = cmb
                    Me(iLayer + 1, eColumnTypes.ColumnAttribute).Behaviors.Add(m_bm)

                    Me.Rows(iLayer + 1).Tag = layer

                Next iLayer

                Me.UpdateMappingsColumn()

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
                    ' ToDo: Clear existing mappings to this attribute?
                    Me.m_dtLayerMapping(layer) = strAttribute
                    Me.UpdateMappingsColumn()
                Catch ex As Exception
                End Try

                Return True

            End Function

            Private Sub UpdateMappingsColumn()

                Dim layer As cLayer = Nothing
                Dim strAttribute As String = ""
                Dim cmb As Cells.Real.ComboBox = Nothing
                Dim dm As DataModels.EditorComboBox = Nothing

                For iRow As Integer = 1 To Me.RowsCount - 1

                    layer = Me.LayerAtRow(iRow)

                    cmb = DirectCast(Me(iRow, eColumnTypes.ColumnAttribute), Cells.Real.ComboBox)
                    dm = DirectCast(cmb.DataModel, DataModels.EditorComboBox)
                    dm.DefaultValue = cVALUE_NONE

                    Try
                        cmb.Value = Me.m_dtLayerMapping(layer)
                    Catch ex As Exception
                        cmb.Value = cVALUE_NONE
                    End Try

                Next iRow

            End Sub

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
                If Me.m_astrAttributes Is Nothing Then Return False
                If Me.m_astrAttributes.Length <= 1 Then Return False
                Return True
            End Function

#End Region ' Overrides

        End Class

#End Region ' Private classes

#Region " Private vars "

        Private Enum eSpatialFileCompatibility As Integer
            ''' <summary>File fits Ecospace.</summary>
            Compatible = 0
            ''' <summary>File could not be read.</summary>
            Unreadable
            ''' <summary>Incompatible number of cols and/or rows found.</summary>
            IncompatibleDimensions
            ''' <summary>Incompatible format.</summary>
            IncompatibleFormat
            ''' <summary>Incompatible number of cols and/or rows found.</summary>
            IncompatibleEmpty
        End Enum

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

        Private Sub DoLoad(ByVal sender As Object, ByVal e As System.EventArgs) _
            Handles Me.Load

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

        Private Sub OnBrowseInput(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_btnBrowseInput.Click

            ' Browse via EwE6 open file dialog 
            Dim cmdh As CommandHandler = CommandHandler.GetInstance()
            Dim foc As cFileOpenCommand = TryCast(cmdh.GetCommand(cFileOpenCommand.COMMAND_NAME), cFileOpenCommand)
            Dim strFileFilter As String = My.Resources.FILEFILTER_LOAD_RASTER
            Dim sfc As eSpatialFileCompatibility = eSpatialFileCompatibility.Unreadable

            ' Sanity check
            If foc Is Nothing Then Return

            If String.IsNullOrEmpty(Me.m_tbInput.Text) Then
                foc.Invoke(strFileFilter)
            Else
                foc.Invoke(Path.GetFileName(Me.m_tbInput.Text), Path.GetDirectoryName(Me.m_tbInput.Text), strFileFilter)
            End If

            If (foc.Result = Windows.Forms.DialogResult.OK) Then

                Me.m_tbInput.Text = foc.FileName
                Me.m_cmbRow.Items.Clear()
                Me.m_cmbRow.Items.Clear()
                Me.m_grid.Attributes = Nothing

                Select Case Path.GetExtension(foc.FileName).ToLower
                    Case ".asc"
                        sfc = Me.ReadAscFile(Me.m_tbInput.Text)
                    Case ".csv" ' csv
                        sfc = Me.ReadCSVFile(Me.m_tbInput.Text)
                    Case ".shp" ' shp
                        sfc = Me.ReadShapeFile(Me.m_tbInput.Text)
                End Select

                Me.UpdateControls()

                Select Case sfc
                    Case eSpatialFileCompatibility.Compatible
                        ' NOP

                    Case eSpatialFileCompatibility.Unreadable
                        ' ToDo_JS: Globalize this
                        MsgBox("The selected file could not be read.", MsgBoxStyle.Exclamation Or MsgBoxStyle.OkOnly)

                    Case eSpatialFileCompatibility.IncompatibleEmpty
                        ' ToDo_JS: Globalize this
                        MsgBox("The selected file did not contain any data.", MsgBoxStyle.Exclamation Or MsgBoxStyle.OkOnly)

                    Case eSpatialFileCompatibility.IncompatibleDimensions
                        ' ToDo_JS: Globalize this
                        MsgBox("The content in the selected file is not compatible with the cell size of the current map.", MsgBoxStyle.Exclamation Or MsgBoxStyle.OkOnly)

                End Select

            End If

        End Sub

        Private Sub OnOK(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_bntOK.Click

            If Not Me.LoadMappedLayers() Then Return

            Me.DialogResult = Windows.Forms.DialogResult.OK
            Me.Close()

        End Sub

        Private Sub OnRowColAttributeChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_cmbRow.SelectedIndexChanged, m_cmbCol.SelectedIndexChanged

            Select Case Me.ValidateData()

                Case eSpatialFileCompatibility.Compatible
                    Me.m_bDataValid = True

                Case eSpatialFileCompatibility.IncompatibleDimensions
                    ' ToDo_JS: Globalize this
                    Me.m_bDataValid = (MsgBox("The selected shape file is not compatible with the current Ecospace basemap dimensions." & _
                              "Use shape file anyway?", MsgBoxStyle.Exclamation Or MsgBoxStyle.YesNo) = MsgBoxResult.Yes)
            End Select

            Me.UpdateControls()
        End Sub

#End Region ' Events

#Region " Internals "

        Private Function ReadCSVFile(ByVal strFile As String) As eSpatialFileCompatibility

            Dim bm As cEcospaceBasemap = Me.m_core.EcospaceBasemap
            Dim tr As TextReader = Nothing
            Dim strLine As String = ""
            Dim astrAttributes As String() = Nothing
            Dim astrValues As String() = Nothing
            Dim iCell, iAttribute As Integer
            Dim sValue As Single = 0.0!
            Dim result As eSpatialFileCompatibility = eSpatialFileCompatibility.Compatible

            Try
                tr = New StreamReader(strFile)
            Catch ex As Exception
                Return eSpatialFileCompatibility.Unreadable
            End Try

            ' Read attributes line
            strLine = tr.ReadLine()
            astrAttributes = strLine.Split(","c)
            ' Clean up
            For i As Integer = 0 To astrAttributes.Length - 1
                astrAttributes(i) = astrAttributes(i).Trim
            Next

            Me.m_data = New cImportExportData(bm.InRow, bm.InCol, astrAttributes)

            iCell = 0
            While (tr.Peek() <> -1) And (iCell < Me.m_data.NumCells)
                strLine = tr.ReadLine()
                astrValues = strLine.Split(","c)

                For iAttribute = 0 To astrAttributes.Length - 1
                    Me.m_data.Value(iCell, astrAttributes(iAttribute)) = CSng(Val(astrValues(iAttribute)))
                Next

                iCell += 1
            End While

            tr.Close()

            Array.Sort(astrAttributes)

            Me.m_cmbRow.Items.AddRange(astrAttributes) : Me.m_cmbRow.SelectedIndex = Me.m_cmbRow.FindString("Row")
            Me.m_cmbCol.Items.AddRange(astrAttributes) : Me.m_cmbCol.SelectedIndex = Me.m_cmbCol.FindString("Col")
            Me.m_grid.Attributes = astrAttributes

            Return result

        End Function

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Read the content of the shape file into cData.
        ''' </summary>
        ''' <returns></returns>
        ''' -----------------------------------------------------------------------
        Private Function ReadShapeFile(ByVal strFile As String) As eSpatialFileCompatibility

            Dim bm As cEcospaceBasemap = Me.m_core.EcospaceBasemap
            Dim sfio As New ShapeFileIO()
            Dim lsd As New List(Of SpatialData)
            Dim sd As SpatialData = Nothing
            Dim sValue As Single = 0.0!

            Dim lstrAttributes As New List(Of String)

            If Not sfio.Read(strFile, lsd) Then
                sfio.Close()
                Return eSpatialFileCompatibility.Unreadable
            End If
            sfio.Close()

            If (lsd.Count = 0) Then Return eSpatialFileCompatibility.IncompatibleEmpty

            For Each strAttribute As String In sfio.AttributeDefintions.Keys
                lstrAttributes.Add(strAttribute)
            Next strAttribute
            lstrAttributes.Sort()

            Me.m_data = New cImportExportData(bm.InRow, bm.InCol, lstrAttributes.ToArray())

            For iShape As Integer = 0 To lsd.Count - 1
                sd = lsd(iShape)
                For Each strAttribute As String In lstrAttributes
                    sValue = CSng(Val(sd.GetAttribute(strAttribute)))
                    Me.m_data.Value(iShape, strAttribute) = sValue
                Next strAttribute
            Next iShape

            Me.m_cmbRow.Items.AddRange(lstrAttributes.ToArray()) : Me.m_cmbRow.SelectedIndex = Me.m_cmbRow.FindString("Row")
            Me.m_cmbCol.Items.AddRange(lstrAttributes.ToArray()) : Me.m_cmbCol.SelectedIndex = Me.m_cmbCol.FindString("Col")
            Me.m_grid.Attributes = lstrAttributes.ToArray()

            Return eSpatialFileCompatibility.Compatible

        End Function

        Private Function ReadAscFile(ByVal strFile As String) As eSpatialFileCompatibility

            Dim bm As cEcospaceBasemap = Me.m_core.EcospaceBasemap
            Dim sfio As New ASCIIFileIO()
            Dim rs As New Raster()
            Dim sd As SpatialData = Nothing
            Dim sValue As Single = 0.0!

            If Not sfio.Read(strFile, rs) Then
                sfio.Close()
                Return eSpatialFileCompatibility.Unreadable
            End If

            sfio.Close()

            If False Then
                ' Ask VC: ignore spatial extent?
                rs = rs.Project(New SpatialData.Extent(bm.Longitude, bm.Latitude, _
                                                  bm.Longitude + bm.CellLength * bm.InCol, _
                                                  bm.Latitude + bm.CellLength * bm.InRow))

                If (rs Is Nothing) Then Return eSpatialFileCompatibility.IncompatibleFormat
                If (rs.CellSize <> bm.CellLength) Then Return eSpatialFileCompatibility.IncompatibleDimensions
            End If

            ' Create data without attributes, row and col pos are implicit
            Me.m_data = New cImportExportData(bm.InRow, bm.InCol)

            For iRow As Integer = 1 To bm.InRow
                For icol As Integer = 1 To bm.InCol
                    Me.m_data.Value(iRow - 1, icol - 1, cImportExportData.cMAPPING_IMPLICIT) = rs.GetCell(icol - 1, iRow - 1)
                Next
            Next

            Me.m_cmbRow.Items.Add(My.Resources.VALUE_NOTAVAILABLE) : Me.m_cmbRow.SelectedIndex = 0
            Me.m_cmbCol.Items.Add(My.Resources.VALUE_NOTAVAILABLE) : Me.m_cmbCol.SelectedIndex = 0
            Me.m_grid.Attributes = New String() {cImportExportData.cMAPPING_IMPLICIT}

            Return eSpatialFileCompatibility.Compatible

        End Function

        Private Function ValidateData() As eSpatialFileCompatibility

            Dim bm As cEcospaceBasemap = Me.m_core.EcospaceBasemap
            'Dim sd As SpatialData = Nothing
            Dim iInRow As Integer = 0
            Dim iInCol As Integer = 0

            If String.IsNullOrEmpty(Me.RowAttribute) Then Return eSpatialFileCompatibility.Unreadable
            If String.IsNullOrEmpty(Me.ColAttribute) Then Return eSpatialFileCompatibility.Unreadable

            Return eSpatialFileCompatibility.Compatible

        End Function

        Private Function LoadMappedLayers() As Boolean

            Dim dtMappings As Dictionary(Of cLayer, String) = Me.m_grid.Mappings()
            Dim bm As cEcospaceBasemap = Me.m_core.EcospaceBasemap
            Dim layer As cLayer = Nothing
            Dim strAttribute As String = ""
            Dim iRow As Integer = 0
            Dim iCol As Integer = 0
            Dim iCell As Integer = 0

            AppLauncher.GetInstance().SetStatusText(My.Resources.STATUS_APPLYVALUES, TriState.True)

            ' For each mapped attribute
            For Each layer In dtMappings.Keys
                strAttribute = dtMappings(layer)
                If Not String.IsNullOrEmpty(strAttribute.Trim) Then

                    ' Clear layer
                    For iRow = 1 To bm.InRow
                        For iCol = 1 To bm.InCol
                            layer.Value(iRow, iCol) = 0.0!
                        Next
                    Next

                    ' Load layer
                    For iCell = 0 To Me.m_data.NumCells
                        If Me.m_data.IsRowColImplicit Then
                            ' Calculate row, col from cell index
                            iRow = CInt(Math.Floor(iCell / bm.InCol)) + 1
                            iCol = CInt(iCell Mod bm.InCol) + 1
                        Else
                            ' Obtain row, col attribute values from data
                            iRow = CInt(Me.m_data.Value(iCell, Me.RowAttribute()))
                            iCol = CInt(Me.m_data.Value(iCell, Me.ColAttribute()))
                        End If
                        layer.Value(iRow, iCol) = Me.m_data.Value(iCell, strAttribute)
                    Next

                    layer.IsModified = True
                    layer.Update(cLayer.eChangeFlags.Map)

                End If
            Next layer

            AppLauncher.GetInstance().SetStatusText("", TriState.False)

            Return True

        End Function

        Private Property RowAttribute() As String
            Get
                Return Me.m_cmbRow.Text
            End Get
            Set(ByVal value As String)
                Me.m_cmbRow.Text = value
            End Set
        End Property

        Private Property ColAttribute() As String
            Get
                Return Me.m_cmbCol.Text
            End Get
            Set(ByVal value As String)
                Me.m_cmbCol.Text = value
            End Set
        End Property

        Private Sub UpdateControls()

            Me.m_cmbRow.Enabled = (Me.m_cmbRow.Items.Count > 0)
            Me.m_cmbCol.Enabled = (Me.m_cmbCol.Items.Count > 0)

            Me.m_grid.Enabled = Me.m_bDataValid
            Me.m_bntOK.Enabled = Me.m_bDataValid

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
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(dlgImportLayerData))
            Me.m_lblSource = New System.Windows.Forms.Label
            Me.m_tbInput = New System.Windows.Forms.TextBox
            Me.m_btnBrowseInput = New System.Windows.Forms.Button
            Me.m_lblMappings = New System.Windows.Forms.Label
            Me.m_tlpOkCancel = New System.Windows.Forms.TableLayoutPanel
            Me.m_bntOK = New System.Windows.Forms.Button
            Me.m_btnCancel = New System.Windows.Forms.Button
            Me.m_grid = New gridMapLayerToAttribute
            Me.m_lblRow = New System.Windows.Forms.Label
            Me.m_cmbRow = New System.Windows.Forms.ComboBox
            Me.m_cmbCol = New System.Windows.Forms.ComboBox
            Me.m_lblCol = New System.Windows.Forms.Label
            Me.m_tlpOkCancel.SuspendLayout()
            Me.SuspendLayout()
            '
            'm_lblSource
            '
            resources.ApplyResources(Me.m_lblSource, "m_lblSource")
            Me.m_lblSource.Name = "m_lblSource"
            '
            'm_tbInput
            '
            resources.ApplyResources(Me.m_tbInput, "m_tbInput")
            Me.m_tbInput.Name = "m_tbInput"
            Me.m_tbInput.ReadOnly = True
            '
            'm_btnBrowseInput
            '
            resources.ApplyResources(Me.m_btnBrowseInput, "m_btnBrowseInput")
            Me.m_btnBrowseInput.Image = Global.ScientificInterface.My.Resources.Resources.openHS
            Me.m_btnBrowseInput.Name = "m_btnBrowseInput"
            Me.m_btnBrowseInput.UseVisualStyleBackColor = True
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
            Me.m_grid.Attributes = New String() {" "}
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
            'm_cmbRow
            '
            Me.m_cmbRow.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
            Me.m_cmbRow.FormattingEnabled = True
            resources.ApplyResources(Me.m_cmbRow, "m_cmbRow")
            Me.m_cmbRow.Name = "m_cmbRow"
            '
            'm_cmbCol
            '
            Me.m_cmbCol.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
            Me.m_cmbCol.FormattingEnabled = True
            resources.ApplyResources(Me.m_cmbCol, "m_cmbCol")
            Me.m_cmbCol.Name = "m_cmbCol"
            '
            'm_lblCol
            '
            resources.ApplyResources(Me.m_lblCol, "m_lblCol")
            Me.m_lblCol.Name = "m_lblCol"
            '
            'dlgImportLayerData
            '
            Me.AcceptButton = Me.m_bntOK
            Me.CancelButton = Me.m_btnCancel
            resources.ApplyResources(Me, "$this")
            Me.ControlBox = False
            Me.Controls.Add(Me.m_cmbCol)
            Me.Controls.Add(Me.m_cmbRow)
            Me.Controls.Add(Me.m_lblCol)
            Me.Controls.Add(Me.m_lblRow)
            Me.Controls.Add(Me.m_tlpOkCancel)
            Me.Controls.Add(Me.m_grid)
            Me.Controls.Add(Me.m_lblMappings)
            Me.Controls.Add(Me.m_tbInput)
            Me.Controls.Add(Me.m_btnBrowseInput)
            Me.Controls.Add(Me.m_lblSource)
            Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
            Me.Name = "dlgImportLayerData"
            Me.ShowIcon = False
            Me.ShowInTaskbar = False
            Me.m_tlpOkCancel.ResumeLayout(False)
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub

        Private WithEvents m_lblSource As System.Windows.Forms.Label
        Private WithEvents m_tbInput As System.Windows.Forms.TextBox
        Private WithEvents m_btnBrowseInput As System.Windows.Forms.Button
        Private WithEvents m_lblMappings As System.Windows.Forms.Label
        Private WithEvents m_grid As gridMapLayerToAttribute
        Private WithEvents m_tlpOkCancel As System.Windows.Forms.TableLayoutPanel
        Private WithEvents m_bntOK As System.Windows.Forms.Button
        Private WithEvents m_btnCancel As System.Windows.Forms.Button
        Private WithEvents m_lblRow As System.Windows.Forms.Label
        Private WithEvents m_cmbRow As System.Windows.Forms.ComboBox
        Private WithEvents m_cmbCol As System.Windows.Forms.ComboBox
        Private WithEvents m_lblCol As System.Windows.Forms.Label

#End Region ' DevStudio generated surprises

    End Class

End Namespace ' Ecospace.Basemap
