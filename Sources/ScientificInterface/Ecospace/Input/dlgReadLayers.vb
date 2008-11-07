'==============================================================================
'
' $Log: dlgReadLayers.vb,v $
' Revision 1.2  2008/11/07 23:52:52  jeroens
' Functional v1 - still quite blunt
'
' Revision 1.1  2008/11/07 08:15:18  jeroens
' Initial version
'
'==============================================================================

#Region " Imports "

Option Strict On

Imports EwECore
Imports EwEUtils.Commands
Imports SAUPUtil.SAUPData
Imports SAUPUtil.SAUPFile
Imports ScientificInterface.Ecospace.Basemap.Layers
Imports System.IO

#End Region ' Imports

Namespace Ecospace

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' 
    ''' </summary>
    ''' ---------------------------------------------------------------------------
    Public Class dlgReadLayers
        Inherits Form

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
        Private m_lData As New List(Of SpatialData)
        Private m_bDataValid As Boolean = False

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

            Me.m_lLayers.AddRange(cLayerFactory.GetLayers(Me.m_core, EwEUtils.Core.eVarNameFlags.LayerImportance))
            Me.m_lLayers.AddRange(cLayerFactory.GetLayers(Me.m_core, EwEUtils.Core.eVarNameFlags.LayerDepth))
            Me.m_lLayers.AddRange(cLayerFactory.GetLayers(Me.m_core, EwEUtils.Core.eVarNameFlags.LayerHabitat))
            Me.m_lLayers.AddRange(cLayerFactory.GetLayers(Me.m_core, EwEUtils.Core.eVarNameFlags.LayerMPA))
            Me.m_lLayers.AddRange(cLayerFactory.GetLayers(Me.m_core, EwEUtils.Core.eVarNameFlags.LayerRelPP))
            Me.m_lLayers.AddRange(cLayerFactory.GetLayers(Me.m_core, EwEUtils.Core.eVarNameFlags.LayerRelCin))
            Me.m_grid.Layers = Me.m_lLayers.ToArray()

            Me.UpdateControls()

        End Sub

        Private Sub OnBrowseInput(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_btnBrowseInput.Click

            ' ToDo_JS: Globalize this

            ' Browse via EwE6 open file dialog 
            Dim cmdh As CommandHandler = CommandHandler.GetInstance()
            Dim foc As FileOpenCommand = TryCast(cmdh.GetCommand(FileOpenCommand.COMMAND_NAME), FileOpenCommand)
            Dim strFileFilter As String = "Shapefile (*.shp)|*.shp"

            ' Sanity check
            If foc Is Nothing Then Return

            If String.IsNullOrEmpty(Me.m_tbInput.Text) Then
                foc.Invoke(strFileFilter)
            Else
                foc.Invoke(Path.GetFileName(Me.m_tbInput.Text), Path.GetDirectoryName(Me.m_tbInput.Text), strFileFilter)
            End If

            If (foc.Result = Windows.Forms.DialogResult.OK) Then
                Me.m_tbInput.Text = foc.FileName

                Select Case Me.ReadShapeFile(Me.m_tbInput.Text)
                    Case eSpatialFileCompatibility.Compatible
                        ' NOP

                    Case eSpatialFileCompatibility.Unreadable
                        MsgBox("The selected file could not be read.", MsgBoxStyle.Exclamation Or MsgBoxStyle.OkOnly)
                        Return

                    Case eSpatialFileCompatibility.IncompatibleFormat
                        MsgBox("The selected shape file is not compatible with Ecospace; required attributes 'row' and 'column_' could not be found.", MsgBoxStyle.Exclamation Or MsgBoxStyle.OkOnly)
                        Return

                    Case eSpatialFileCompatibility.IncompatibleEmpty
                        MsgBox("The selected shape file did not contain any data.", MsgBoxStyle.Exclamation Or MsgBoxStyle.OkOnly)
                        Return

                End Select

                Me.UpdateControls()

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
                    Me.m_bDataValid = (MsgBox("The selected shape file is not compatible with the current Ecospace basemap dimensions." & _
                              "Use shape file anyway?", MsgBoxStyle.Exclamation Or MsgBoxStyle.YesNo) = MsgBoxResult.Yes)
            End Select

            Me.UpdateControls()
        End Sub

#End Region ' Events

#Region " Internals "

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Read the content of the shape file into cData.
        ''' </summary>
        ''' <returns></returns>
        ''' -----------------------------------------------------------------------
        Private Function ReadShapeFile(ByVal strFile As String) As eSpatialFileCompatibility

            Dim sfio As New ShapeFileIO()

            Dim lstrAttributes As New List(Of String)

            m_lData.Clear()

            If Not sfio.Read(strFile, Me.m_lData) Then Return eSpatialFileCompatibility.Unreadable
            If (Me.m_lData.Count = 0) Then Return eSpatialFileCompatibility.IncompatibleEmpty

            For Each strAttribute As String In sfio.AttributeDefintions.Keys
                lstrAttributes.Add(strAttribute)
            Next strAttribute
            lstrAttributes.Sort()

            Me.m_cmbRow.Items.AddRange(lstrAttributes.ToArray())
            Me.m_cmbCol.Items.AddRange(lstrAttributes.ToArray())

            lstrAttributes.Insert(0, " ")
            Me.m_grid.Attributes = lstrAttributes.ToArray()

            Return eSpatialFileCompatibility.Compatible

        End Function

        Private Function ValidateData() As eSpatialFileCompatibility

            Dim bm As cEcospaceBasemap = Me.m_core.EcospaceBasemap
            Dim sd As SpatialData = Nothing
            Dim iInRow As Integer = 0
            Dim iInCol As Integer = 0

            If String.IsNullOrEmpty(Me.RowAttribute) Then Return eSpatialFileCompatibility.Unreadable
            If String.IsNullOrEmpty(Me.ColAttribute) Then Return eSpatialFileCompatibility.Unreadable

            Try
                ' Validate shapefile dimensions
                For Each sd In Me.m_lData
                    iInRow = Math.Max(CInt(sd.GetAttribute(Me.RowAttribute)), iInRow)
                    iInCol = Math.Max(CInt(sd.GetAttribute(Me.ColAttribute)), iInCol)
                Next
            Catch ex As Exception
                Return eSpatialFileCompatibility.IncompatibleFormat
            End Try

            ' Validate dimensions
            If iInRow = 0 Or iInCol = 0 Then Return eSpatialFileCompatibility.IncompatibleFormat

            If (bm.InRow <> iInRow) Then Return eSpatialFileCompatibility.IncompatibleDimensions
            If (bm.InCol <> iInCol) Then Return eSpatialFileCompatibility.IncompatibleDimensions

            Return eSpatialFileCompatibility.Compatible

        End Function

        Private Function LoadMappedLayers() As Boolean

            Dim bm As cEcospaceBasemap = Me.m_core.EcospaceBasemap
            Dim dtMappings As Dictionary(Of cLayer, String) = Me.m_grid.Mappings()
            Dim layer As cLayer = Nothing
            Dim strAttribute As String = ""
            Dim iRow As Integer = 0
            Dim iCol As Integer = 0
            Dim sValue As Single = 0.0!

            ' For each mapped attribute
            For Each layer In dtMappings.Keys
                strAttribute = dtMappings(layer)
                If Not String.IsNullOrEmpty(strAttribute.Trim) Then

                    ' Clear layer
                    For iRow = 1 To bm.InRow
                        For iCol = 1 To bm.InCol
                            layer.Value(New Point(iRow, iCol)) = 0.0!
                        Next
                    Next

                    ' For each shape
                    For Each sd As SpatialData In Me.m_lData

                        iRow = CInt(sd.GetAttribute(Me.RowAttribute))
                        iCol = CInt(sd.GetAttribute(Me.ColAttribute))

                        Try
                            sValue = CSng(Val(sd.GetAttribute(strAttribute)))
                            layer.Value(New Point(iRow, iCol)) = sValue
                        Catch ex As Exception

                        End Try

                    Next sd
                End If
            Next layer

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
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(dlgReadLayers))
            Me.m_lblSource = New System.Windows.Forms.Label
            Me.m_tbInput = New System.Windows.Forms.TextBox
            Me.m_btnBrowseInput = New System.Windows.Forms.Button
            Me.m_lblMappings = New System.Windows.Forms.Label
            Me.m_tlpOkCancel = New System.Windows.Forms.TableLayoutPanel
            Me.m_bntOK = New System.Windows.Forms.Button
            Me.m_btnCancel = New System.Windows.Forms.Button
            Me.m_grid = New ScientificInterface.Ecospace.gridReadLayers
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
            'dlgReadLayers
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
            Me.Name = "dlgReadLayers"
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
        Private WithEvents m_grid As gridReadLayers
        Private WithEvents m_tlpOkCancel As System.Windows.Forms.TableLayoutPanel
        Private WithEvents m_bntOK As System.Windows.Forms.Button
        Private WithEvents m_btnCancel As System.Windows.Forms.Button
        Private WithEvents m_lblRow As System.Windows.Forms.Label
        Private WithEvents m_cmbRow As System.Windows.Forms.ComboBox
        Private WithEvents m_cmbCol As System.Windows.Forms.ComboBox
        Private WithEvents m_lblCol As System.Windows.Forms.Label

#End Region ' DevStudio generated surprises

    End Class

End Namespace ' Ecospace
