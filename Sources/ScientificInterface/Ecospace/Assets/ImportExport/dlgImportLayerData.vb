#Region " Imports "

Option Strict On

Imports System.IO
Imports EwECore
Imports EwEUtils.Commands
Imports EwEUtils.Utilities
Imports ScientificInterfaceShared.Commands
Imports ScientificInterfaceShared.Controls.Map.Layers
Imports SharedResources = ScientificInterfaceShared.My.Resources
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
        Public Class gridMapLayerToField
            Inherits EwEGrid

#Region " Private vars "

            ''' <summary>The layers to map upon.</summary>
            Private m_aLayers As cLayer()
            ''' <summary>The field names to map upon.</summary>
            Private m_astrFields As String() = {}
            ''' <summary>Mappings. MAPPINGS!</summary>
            Private m_dtLayerMapping As New Dictionary(Of cLayer, String)

            Private Enum eColumnTypes As Integer
                ColumnLayer = 0
                ColumnField
            End Enum

#End Region ' Private vars

#Region " Construction "

            Public Sub New()
                MyBase.New()
            End Sub

#End Region ' Construction

#Region " Public interfaces "

            Public Event MappingChanged()

            Public Property Layers() As cLayer()
                Get
                    Return Nothing
                End Get
                Set(ByVal value As cLayer())
                    Me.m_aLayers = value
                End Set
            End Property

            Public Property Fields() As String()
                Get
                    Return Me.m_astrFields
                End Get
                Set(ByVal value As String())
                    Dim lstr As New List(Of String)
                    If (value IsNot Nothing) Then lstr.AddRange(value)
                    If lstr.IndexOf(SharedResources.GENERIC_VALUE_NONE) = -1 Then lstr.Insert(0, SharedResources.GENERIC_VALUE_NONE)
                    Me.m_astrFields = lstr.ToArray()
                    Me.RefreshContent()
                End Set
            End Property

            Public Function Mappings() As Dictionary(Of cLayer, String)
                Return Me.m_dtLayerMapping
            End Function

            Public Function HasMappings() As Boolean
                For Each l As cLayer In Me.m_dtLayerMapping.Keys
                    If Not String.IsNullOrWhiteSpace(Me.m_dtLayerMapping(l)) Then
                        Return True
                    End If
                Next
                Return False
            End Function

#End Region ' Public interfaces

#Region " Overrides "

            Protected Overrides Sub InitStyle()
                MyBase.InitStyle()

                If Not Me.HasData() Then Return

                Me.Redim(1, System.Enum.GetValues(GetType(eColumnTypes)).Length)

                Me(0, eColumnTypes.ColumnLayer) = New EwEColumnHeaderCell(SharedResources.HEADER_LAYER)
                Me(0, eColumnTypes.ColumnField) = New EwEColumnHeaderCell(SharedResources.HEADER_FIELD)

                Me.Columns(eColumnTypes.ColumnLayer).AutoSizeMode = SourceGrid2.AutoSizeMode.EnableAutoSize
                Me.Columns(eColumnTypes.ColumnField).AutoSizeMode = SourceGrid2.AutoSizeMode.EnableStretch

                Me.FixedColumns = 1
                Me.FixedColumnWidths = False

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
                    ewec.Style = (cStyleGuide.eStyleFlags.Names Or cStyleGuide.eStyleFlags.NotEditable)
                    Me(iLayer + 1, eColumnTypes.ColumnLayer) = ewec

                    cmb = New Cells.Real.ComboBox(SharedResources.GENERIC_VALUE_NONE, GetType(String), Me.m_astrFields, True)
                    cmb.EditableMode = EditableMode.SingleClick
                    Me(iLayer + 1, eColumnTypes.ColumnField) = cmb
                    Me(iLayer + 1, eColumnTypes.ColumnField).Behaviors.Add(Me.EwEEditHandler)

                    Me.Rows(iLayer + 1).Tag = layer

                Next iLayer

                Me.UpdateMappingsColumn()

            End Sub

            Protected Overrides Sub FinishStyle()
                MyBase.FinishStyle()
                Me.StretchColumnsToFitWidth()
            End Sub

            Protected Overrides Function OnCellEdited(ByVal p As SourceGrid2.Position, ByVal cell As SourceGrid2.Cells.ICellVirtual) As Boolean

                Dim strField As String = Me.FieldAtRow(p.Row)
                Dim layer As cLayer = Me.LayerAtRow(p.Row)

                Try
                    Me.m_dtLayerMapping(layer) = strField
                    Me.UpdateMappingsColumn()
                Catch ex As Exception
                End Try

                Return True

            End Function

            Private Sub UpdateMappingsColumn()

                Dim layer As cLayer = Nothing
                Dim strField As String = ""
                Dim cmb As Cells.Real.ComboBox = Nothing
                Dim dm As DataModels.EditorComboBox = Nothing
                Dim strValue As String = ""

                For iRow As Integer = 1 To Me.RowsCount - 1

                    layer = Me.LayerAtRow(iRow)

                    cmb = DirectCast(Me(iRow, eColumnTypes.ColumnField), Cells.Real.ComboBox)
                    dm = DirectCast(cmb.DataModel, DataModels.EditorComboBox)
                    dm.DefaultValue = SharedResources.GENERIC_VALUE_NONE

                    If Me.m_dtLayerMapping.ContainsKey(layer) Then
                        strValue = Me.m_dtLayerMapping(layer)
                    Else
                        strValue = SharedResources.GENERIC_VALUE_NONE
                    End If

                    Try
                        cmb.Value = strValue
                    Catch ex As Exception
                    End Try

                Next iRow

                Try
                    RaiseEvent MappingChanged()
                Catch ex As Exception

                End Try
            End Sub

            Private Function LayerAtRow(ByVal iRow As Integer) As cLayer
                If iRow > 0 And iRow < Me.RowsCount Then
                    Return DirectCast(Me.Rows(iRow).Tag, cLayer)
                End If
                Return Nothing
            End Function

            Private Function FieldAtRow(ByVal iRow As Integer) As String
                Dim strField As String = ""
                If iRow > 0 And iRow < Me.RowsCount Then
                    strField = CStr(Me(iRow, eColumnTypes.ColumnField).Value)
                    If (strField = SharedResources.GENERIC_VALUE_NONE) Then
                        strField = ""
                    End If
                End If
                Return strField
            End Function

            Private Function HasData() As Boolean
                Return (Me.m_aLayers IsNot Nothing)
            End Function

#End Region ' Overrides

        End Class

#End Region ' Private classes

#Region " Private vars "

        Private m_uic As cUIContext = Nothing
        Private m_lLayers As New List(Of cLayer)
        Private m_data As cImportExportData = Nothing

#End Region ' Private vars

#Region " Constructor "

        Public Sub New(ByVal uic As cUIContext)
            Me.InitializeComponent()
            Me.m_uic = uic
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

#Region " Form overrides "

        Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)
            MyBase.OnLoad(e)

            If (Me.DesignMode = True) Then Return

            Debug.Assert(Me.m_uic IsNot Nothing)

            ' Set default file
            Me.m_tbInput.Text = Path.Combine(Me.m_uic.Core.OutputPath, Me.m_uic.Core.EcospaceOutputFileLocation("layer"))

            ' Get default layers if needed
            If (Me.m_lLayers.Count = 0) Then
                Me.m_lLayers.AddRange(cImportExportData.DefaultLayers(Me.m_uic))
            End If
            Me.m_grid.Layers = Me.m_lLayers.ToArray()
            Me.m_grid.UIContext = Me.m_uic

            AddHandler Me.m_grid.MappingChanged, AddressOf UpdateControls

            Me.UpdateControls()

        End Sub

        Protected Overrides Sub OnFormClosed(e As System.Windows.Forms.FormClosedEventArgs)

            RemoveHandler Me.m_grid.MappingChanged, AddressOf UpdateControls

            Me.m_grid.Layers = Nothing
            Me.m_grid.UIContext = Nothing

            MyBase.OnFormClosed(e)

        End Sub

#End Region ' Form overrides

#Region " Events "

        Protected Overrides Sub OnDragEnter(e As System.Windows.Forms.DragEventArgs)
            If e.Data.GetDataPresent(DataFormats.FileDrop) Then
                e.Effect = DragDropEffects.All
            End If
            MyBase.OnDragEnter(e)
        End Sub

        Protected Overrides Sub OnDragDrop(e As System.Windows.Forms.DragEventArgs)
            If e.Data.GetDataPresent(DataFormats.FileDrop) Then
                Try
                    Dim astrFiles() As String = CType(e.Data.GetData(DataFormats.FileDrop), String())
                    If astrFiles.Length > 0 Then
                        Me.m_tbInput.Text = astrFiles(0)
                        Me.ReadCSVFields()
                    End If
                Catch ex As Exception

                End Try
            End If
            MyBase.OnDragDrop(e)
        End Sub

        Private Sub OnBrowseInput(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_btnBrowseInput.Click

            ' Browse via EwE6 open file dialog 
            Dim cmdh As cCommandHandler = Me.m_uic.CommandHandler
            Dim foc As cFileOpenCommand = TryCast(cmdh.GetCommand(cFileOpenCommand.COMMAND_NAME), cFileOpenCommand)

            ' Sanity check
            If (foc Is Nothing) Then Return

            foc.Invoke(Me.m_tbInput.Text, SharedResources.FILEFILTER_CSV, 0, Me.Text)

            If (foc.Result = Windows.Forms.DialogResult.OK) Then
                Me.m_tbInput.Text = foc.FileName
                Me.ReadCSVFields()
            End If

        End Sub

        Private Sub OnOK(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_bntOK.Click

            If Not Me.LoadMappedLayers() Then Return

            Me.DialogResult = Windows.Forms.DialogResult.OK
            Me.Close()

        End Sub

        Private Sub OnRowColFieldChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_cmbRow.SelectedIndexChanged, m_cmbCol.SelectedIndexChanged
            Me.UpdateControls()
        End Sub

#End Region ' Events

#Region " Internals "

        Private Function ReadCSVFields() As Boolean

            Dim bSuccess As Boolean = True

            Me.m_cmbRow.Items.Clear()
            Me.m_cmbRow.Items.Clear()
            Me.m_grid.Fields = Nothing

            Me.UpdateControls()

            If Not Me.ReadCSVFile() Then
                Dim msg As New cMessage(String.Format(SharedResources.FILE_LOAD_ERROR_READ, Me.m_tbInput.Text), eMessageType.Any, EwEUtils.Core.eCoreComponentType.External, eMessageImportance.Warning)
                Me.m_uic.Core.Messages.SendMessage(msg)
                bSuccess = False
            End If

            Me.UpdateControls()

            Return True

        End Function

        Private Function ReadCSVFile() As Boolean

            Dim bm As cEcospaceBasemap = Me.m_uic.Core.EcospaceBasemap
            Dim tr As TextReader = Nothing
            Dim strLine As String = ""
            Dim astrFields As String() = Nothing
            Dim astrValues As String() = Nothing
            Dim iCell, iField As Integer
            Dim sValue As Single = 0.0!

            Try
                tr = New StreamReader(Me.m_tbInput.Text)
            Catch ex As Exception
                Return False
            End Try

            ' Read fields line
            strLine = tr.ReadLine()
            astrFields = cStringUtils.SplitQualified(strLine, (","c))

            ' Clean up
            For i As Integer = 0 To astrFields.Length - 1
                astrFields(i) = astrFields(i).Trim
            Next

            Me.m_data = New cImportExportData(bm.InRow, bm.InCol, astrFields)

            iCell = 0
            While (tr.Peek() <> -1) And (iCell < Me.m_data.NumCells)
                strLine = tr.ReadLine()
                astrValues = strLine.Split(","c)

                For iField = 0 To astrFields.Length - 1
                    Me.m_data.Value(iCell, astrFields(iField)) = CSng(Val(astrValues(iField)))
                Next

                iCell += 1
            End While

            tr.Close()

            Array.Sort(astrFields)

            Me.m_cmbRow.Items.AddRange(astrFields) : Me.m_cmbRow.SelectedIndex = Me.m_cmbRow.FindString("Row")
            Me.m_cmbCol.Items.AddRange(astrFields) : Me.m_cmbCol.SelectedIndex = Me.m_cmbCol.FindString("Col")
            Me.m_grid.Fields = astrFields

            Return True

        End Function

        Private Function LoadMappedLayers() As Boolean

            Dim dtMappings As Dictionary(Of cLayer, String) = Me.m_grid.Mappings()
            Dim bm As cEcospaceBasemap = Me.m_uic.Core.EcospaceBasemap
            Dim layer As cLayer = Nothing
            Dim strField As String = ""
            Dim iRow As Integer = 0
            Dim iCol As Integer = 0
            Dim iCell As Integer = 0

            cApplicationStatusNotifier.StartProgress(Me.m_uic.Core, My.Resources.STATUS_APPLYVALUES)

            ' For each mapped field
            For Each layer In dtMappings.Keys
                strField = dtMappings(layer)
                If Not String.IsNullOrEmpty(strField.Trim) Then

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
                            ' Obtain row, col field values from data
                            iRow = CInt(Me.m_data.Value(iCell, Me.RowField()))
                            iCol = CInt(Me.m_data.Value(iCell, Me.ColField()))
                        End If
                        layer.Value(iRow, iCol) = Me.m_data.Value(iCell, strField)
                    Next

                    layer.IsModified = True
                    layer.Update(cLayer.eChangeFlags.Map)

                End If
            Next layer

            cApplicationStatusNotifier.EndProgress(Me.m_uic.Core)

            Return True

        End Function

        Private Property RowField() As String
            Get
                Return Me.m_cmbRow.Text
            End Get
            Set(ByVal value As String)
                Me.m_cmbRow.Text = value
            End Set
        End Property

        Private Property ColField() As String
            Get
                Return Me.m_cmbCol.Text
            End Get
            Set(ByVal value As String)
                Me.m_cmbCol.Text = value
            End Set
        End Property

        Private Sub UpdateControls()

            Dim bHasFile As Boolean = File.Exists(Me.m_tbInput.Text)
            Dim bHasRowCol As Boolean = (Me.m_cmbCol.SelectedIndex >= 0) And (Me.m_cmbRow.SelectedIndex >= 0)
            Dim bHasMappings As Boolean = (Me.m_grid.HasMappings())

            Me.m_cmbRow.Enabled = (Me.m_cmbRow.Items.Count > 0)
            Me.m_cmbCol.Enabled = (Me.m_cmbCol.Items.Count > 0)

            Me.m_grid.Enabled = bHasFile
            Me.m_bntOK.Enabled = bHasFile And bHasRowCol And bHasMappings

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
            Me.m_lblSource = New System.Windows.Forms.Label()
            Me.m_tbInput = New System.Windows.Forms.TextBox()
            Me.m_btnBrowseInput = New System.Windows.Forms.Button()
            Me.m_lblMappings = New System.Windows.Forms.Label()
            Me.m_tlpOkCancel = New System.Windows.Forms.TableLayoutPanel()
            Me.m_bntOK = New System.Windows.Forms.Button()
            Me.m_btnCancel = New System.Windows.Forms.Button()
            Me.m_grid = New ScientificInterface.Ecospace.Basemap.dlgImportLayerData.gridMapLayerToField()
            Me.m_lblRow = New System.Windows.Forms.Label()
            Me.m_cmbRow = New System.Windows.Forms.ComboBox()
            Me.m_cmbCol = New System.Windows.Forms.ComboBox()
            Me.m_lblCol = New System.Windows.Forms.Label()
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
            Me.m_tbInput.AllowDrop = True
            resources.ApplyResources(Me.m_tbInput, "m_tbInput")
            Me.m_tbInput.Name = "m_tbInput"
            Me.m_tbInput.ReadOnly = True
            '
            'm_btnBrowseInput
            '
            resources.ApplyResources(Me.m_btnBrowseInput, "m_btnBrowseInput")
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
            Me.m_grid.AllowBlockSelect = True
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
            Me.m_grid.Fields = New String() {}
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
            Me.m_grid.UIContext = Nothing
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
        Private WithEvents m_grid As gridMapLayerToField
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
