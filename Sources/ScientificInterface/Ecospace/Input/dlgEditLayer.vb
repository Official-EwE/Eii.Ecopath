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

Option Explicit On
Option Strict On
Imports EwECore
Imports EwECore.Auxiliary
Imports EwEUtils.Core
Imports EwEUtils.Utilities
Imports ScientificInterfaceShared.Controls.Map.Layers
Imports SharedResources = ScientificInterfaceShared.My.Resources

#End Region ' Imports

Namespace Ecospace.Basemap.Layers

    ''' =======================================================================
    ''' <summary>
    ''' Dialog, implementing the Ecospace Edit Layer user interface.
    ''' </summary>
    ''' =======================================================================
    Public Class dlgEditLayer

#Region " Private variables "

        Private m_uic As cUIContext = Nothing
        Private m_qehGrid As cQuickEditHandler = Nothing

        ''' <summary>Original layer this dialog was invoked for.</summary>
        Private m_layerOriginal As cDisplayLayerRaster = Nothing
        Private m_layerDepth As cDisplayLayerRaster = Nothing
        Private m_edittype As eLayerEditTypes

        ''' <summary>Work layer (a copy of the original) for this dialog to work on.</summary>
        Private m_layerWork As cDisplayLayerRaster = Nothing
        ''' <summary>Editor to transmogrify the representation of the layer.</summary>
        Private m_ucEditVisualStyle As ucEditVisualStyle = Nothing

        Private m_fpName As cEwEFormatProvider = Nothing
        Private m_fpUnits As cEwEFormatProvider = Nothing
        Private m_fpWeight As cEwEFormatProvider = Nothing
        Private m_fpDescription As cEwEFormatProvider = Nothing

        ' -- Hackerdihack

        Private m_bIsVectorData As Boolean = False
        Private m_iVectorData As Integer = 0

#End Region ' Private variables

#Region " Constructors "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="uic"></param>
        ''' <param name="layer"></param>
        ''' <param name="edittype"></param>
        ''' -------------------------------------------------------------------
        Public Sub New(uic As cUIContext,
                       ByRef layer As cDisplayLayerRaster,
                       edittype As eLayerEditTypes)

            Debug.Assert(layer IsNot Nothing)

            Me.InitializeComponent()

            ' Set the references
            Me.m_uic = uic
            Me.m_grid.UIContext = Me.m_uic
            Me.m_zoommap.UIContext = Me.m_uic

            Me.m_layerOriginal = layer

            ' Resolve depth layer
            If Not (TypeOf layer.Data Is cEcospaceLayerDepth) Then
                Dim fact As New cLayerFactoryInternal()
                Me.m_layerDepth = fact.GetLayers(uic, eVarNameFlags.LayerDepth)(0)
            End If
            Me.m_edittype = edittype

            Me.m_layerWork = New cDisplayLayerRaster(uic, layer) ' Work on a clone
            Me.m_layerWork.AllowValidation = False
            Me.m_layerWork.IsSelected = True ' Select layer, otherwise its content may not be rendered

            ' First set default index, then make vector stuff 'live' if need be ;)
            Me.m_tscmbVectorData.SelectedIndex = 0
            Me.m_bIsVectorData = (TypeOf Me.m_layerWork.Data Is cEcospaceLayerVelocity)

        End Sub

#End Region ' Constructors

#Region " Overrides "

        Protected Overrides Sub OnLoad(e As System.EventArgs)
            MyBase.OnLoad(e)

            Me.m_grid.DataName = Me.m_layerOriginal.Name

            Me.m_qehGrid = New cQuickEditHandler()
            'Me.m_qehGrid.ShowImportExport = False
            Me.m_qehGrid.Attach(Me.m_grid, Me.m_uic, Me.m_tsGrid)
            Me.m_qehGrid.IsOutputGrid = Me.m_layerWork.Editor.IsReadOnly

            ' Show your stuff
            Me.m_zoommap.Map.AddLayer(Me.m_layerWork)

            ' Do not add depth layer if already showing depth layer
            If ((Not ReferenceEquals(Me.m_layerOriginal, Me.m_layerDepth)) And
                (Me.m_layerDepth IsNot Nothing)) Then
                Me.m_zoommap.Map.AddLayer(Me.m_layerDepth)
            End If

            Select Case Me.m_edittype
                Case eLayerEditTypes.EditVisuals
                    Me.m_tcLayerView.SelectedTab = Me.m_tpAppearance
                Case eLayerEditTypes.EditData
                    Me.m_tcLayerView.SelectedTab = Me.m_tpData
            End Select

            ' Set up format providers
            Me.m_fpName = New cEwEFormatProvider(Me.m_uic, Me.m_tbNameValue, GetType(String))
            Me.m_fpUnits = New cEwEFormatProvider(Me.m_uic, Me.m_tbUnits, GetType(String))
            Me.m_fpWeight = New cEwEFormatProvider(Me.m_uic, Me.m_nudWeight, GetType(Single))
            Me.m_fpDescription = New cEwEFormatProvider(Me.m_uic, Me.m_tbDescription, GetType(String))

            ' Set up other layers to copy style from
            Dim fact As New cLayerFactoryInternal()
            Dim others As cDisplayLayerRaster() = fact.GetLayers(Me.m_uic, Me.m_layerOriginal.Data.VarName)
            For Each dl As cDisplayLayerRaster In others
                If (TypeOf dl Is cDisplayLayerRasterBundle) Then
                    Me.m_tlpImportStyle.Enabled = False
                Else
                    If dl.Data.Index <> Me.m_layerOriginal.Data.Index Then
                        Me.m_cmbCopyStyleFrom.Items.Add(dl)
                    End If
                End If
            Next

            Me.LoadLayer()
            Me.UpdateControls()
            Me.DrawPreview()

            If (Me.m_ucEditVisualStyle IsNot Nothing) Then
                AddHandler Me.m_ucEditVisualStyle.OnVisualStyleChanged, AddressOf Me.OnVisualStyleChanged
            End If
            AddHandler Me.m_fpName.OnValueChanged, AddressOf Me.OnPropChanged
            AddHandler Me.m_fpUnits.OnValueChanged, AddressOf Me.OnPropChanged

        End Sub

        Protected Overrides Sub OnFormClosed(e As System.Windows.Forms.FormClosedEventArgs)

            If (Me.m_ucEditVisualStyle IsNot Nothing) Then
                RemoveHandler Me.m_ucEditVisualStyle.OnVisualStyleChanged, AddressOf Me.OnVisualStyleChanged
            End If

            Me.m_qehGrid.Detach()
            Me.m_qehGrid = Nothing
            Me.m_grid.UIContext = Nothing

            RemoveHandler Me.m_fpName.OnValueChanged, AddressOf Me.OnPropChanged
            RemoveHandler Me.m_fpUnits.OnValueChanged, AddressOf Me.OnPropChanged

            Me.m_fpName.Release()
            Me.m_fpWeight.Release()
            Me.m_fpDescription.Release()

            Me.m_layerDepth = Nothing
            Me.m_layerOriginal = Nothing
            Me.m_layerWork.Dispose()
            Me.m_layerWork = Nothing

            MyBase.OnFormClosed(e)

        End Sub

#End Region ' Overrides

#Region " Local events "

        Private Sub OnOK(sender As System.Object, e As System.EventArgs) _
            Handles OK_Button.Click

            If Not Me.ApplyChanges() Then Return
            Me.DialogResult = System.Windows.Forms.DialogResult.OK
            Me.Close()

        End Sub

        Private Sub OnCancel(sender As System.Object, e As System.EventArgs) _
            Handles Cancel_Button.Click

            Me.DialogResult = System.Windows.Forms.DialogResult.Cancel
            Me.Close()

        End Sub

        Private Sub OnApply(sender As Object, e As System.EventArgs) _
            Handles Apply_Button.Click

            Me.ApplyChanges()

        End Sub

        Private Sub OnVisualStyleChanged(sender As ucEditVisualStyle)

            ' Update work layer Visual Style
            Me.m_ucEditVisualStyle.Apply(Me.m_layerWork.Renderer.VisualStyle)
            Me.m_layerWork.Update(cDisplayLayer.eChangeFlags.VisualStyle)

        End Sub

#Region " Import "

        Private Sub OnImportCSV(sender As System.Object, e As System.EventArgs) _
            Handles m_tsmiImportCSV.Click
            Try
                Me.m_qehGrid.ImportGridFromCSV()
            Catch ex As Exception

            End Try
        End Sub

        Private Sub OnImportXYZ(sender As System.Object, e As System.EventArgs) _
            Handles m_tsmiImportXYZ.Click

            Try
                Dim cmd As cImportLayerCommand = DirectCast(Me.m_uic.CommandHandler.GetCommand(cImportLayerCommand.cCOMMAND_NAME), cImportLayerCommand)
                Dim l As cEcospaceLayer = Me.m_layerWork.Data
                If (TypeOf l Is cEcospaceLayerVelocity) Then
                    cmd.Invoke(DirectCast(l, cEcospaceLayerVelocity).VelocityLayers, eNativeLayerFileFormatTypes.XYZ)
                Else
                    cmd.Invoke(New cEcospaceLayer() {l}, eNativeLayerFileFormatTypes.XYZ)
                End If
                Me.m_layerWork.Update(cDisplayLayer.eChangeFlags.Map)
                Me.m_grid.RefreshContent()
            Catch ex As Exception

            End Try

        End Sub

        Private Sub OnImportAscii(sender As System.Object, e As System.EventArgs) _
            Handles m_tsmiImportAsc.Click

            Try
                Dim cmd As cImportLayerCommand = DirectCast(Me.m_uic.CommandHandler.GetCommand(cImportLayerCommand.cCOMMAND_NAME), cImportLayerCommand)
                Dim l As cEcospaceLayer = Me.m_layerWork.Data
                If (TypeOf l Is cEcospaceLayerVelocity) Then
                    l = DirectCast(l, cEcospaceLayerVelocity).VelocityLayers(Me.m_tscmbVectorData.SelectedIndex)
                End If
                cmd.Invoke(New cEcospaceLayer() {l}, eNativeLayerFileFormatTypes.ASCII)
                Me.m_layerWork.Update(cDisplayLayer.eChangeFlags.Map)
                Me.m_grid.RefreshContent()
            Catch ex As Exception

            End Try
        End Sub

#End Region ' Import

#Region " Export "

        Private Sub OnExportCSV(sender As System.Object, e As System.EventArgs) _
            Handles m_tsmiExportCSV.Click
            Try
                Me.m_qehGrid.ExportGridToCSV()
            Catch ex As Exception

            End Try
        End Sub

        Private Sub OnExportAsc(sender As System.Object, e As System.EventArgs) _
            Handles m_tsmiExportAsc.Click

            Try
                Dim cmd As cExportLayerCommand = DirectCast(Me.m_uic.CommandHandler.GetCommand(cExportLayerCommand.cCOMMAND_NAME), cExportLayerCommand)
                Dim l As cEcospaceLayer = Me.m_layerWork.Data
                If (TypeOf l Is cEcospaceLayerVelocity) Then
                    l = DirectCast(l, cEcospaceLayerVelocity).VelocityLayers(Me.m_tscmbVectorData.SelectedIndex)
                End If
                cmd.Invoke(New cEcospaceLayer() {l}, eNativeLayerFileFormatTypes.ASCII)
                Me.UpdateControls()
            Catch ex As Exception

            End Try

        End Sub

        Private Sub OnExportXYZ(sender As System.Object, e As System.EventArgs) Handles m_tsmiExportXYZ.Click

            Try
                Dim cmd As cExportLayerCommand = DirectCast(Me.m_uic.CommandHandler.GetCommand(cExportLayerCommand.cCOMMAND_NAME), cExportLayerCommand)
                Dim l As cEcospaceLayer = Me.m_layerWork.Data
                If (TypeOf l Is cEcospaceLayerVelocity) Then
                    cmd.Invoke(DirectCast(l, cEcospaceLayerVelocity).VelocityLayers, eNativeLayerFileFormatTypes.XYZ)
                Else
                    cmd.Invoke(New cEcospaceLayer() {l}, eNativeLayerFileFormatTypes.XYZ)
                End If
                Me.UpdateControls()
            Catch ex As Exception

            End Try

        End Sub

#End Region ' Export

        Private Sub OnPropChanged(sender As Object, e As System.EventArgs)
            Try
                Me.UpdateControls()
            Catch ex As Exception

            End Try
        End Sub

        Private Sub OnSelectData(sender As System.Object, e As System.EventArgs) _
            Handles m_tscmbVectorData.SelectedIndexChanged

            If (Me.m_bIsVectorData) Then
                Me.m_grid.VectorFieldIndex = Me.m_tscmbVectorData.SelectedIndex
                Me.m_grid.RefreshContent()
            End If

        End Sub

#End Region ' Local events

#Region " Internal implementation "

        Private Sub LoadLayer()

            Dim vs As cVisualStyle = Me.m_layerWork.Renderer.VisualStyle
            Dim src As cCoreInputOutputBase = Me.m_layerWork.Data

            Me.m_lblWeight.Visible = False
            Me.m_nudWeight.Visible = False
            Me.m_lblDescription.Visible = False
            Me.m_tbDescription.Visible = False

            Me.m_fpUnits.Enabled = (Me.m_layerWork.UnitStatus And eStatusFlags.NotEditable) = 0
            Me.m_fpUnits.Value = Me.m_layerWork.Units

            Me.m_fpName.Enabled = (Me.m_layerWork.NameStatus And eStatusFlags.NotEditable) = 0
            Me.m_fpName.Value = Me.m_layerWork.Name

            If TypeOf src Is cEcospaceLayerDriver Then
                Me.m_lblDescription.Visible = True
                Me.m_tbDescription.Visible = True
                Me.m_fpDescription.Value = src.GetVariable(eVarNameFlags.Description)
            End If

            If TypeOf src Is cEcospaceLayerImportance Then
                Me.m_lblWeight.Visible = True
                Me.m_nudWeight.Visible = True
                Me.m_lblDescription.Visible = True
                Me.m_tbDescription.Visible = True

                Me.m_fpWeight.Value = src.GetVariable(eVarNameFlags.ImportanceWeight)
                Me.m_fpDescription.Value = src.GetVariable(eVarNameFlags.Description)
            End If

            Me.m_ucEditVisualStyle = ucEditVisualStyle.GetEditor(Me.m_uic, vs, Me.m_layerWork.Renderer.VisualStyleFlags)

            If (Me.m_ucEditVisualStyle IsNot Nothing) Then
                Me.m_plAppearance.Height = Me.m_ucEditVisualStyle.Height
                Me.m_ucEditVisualStyle.Dock = DockStyle.Fill
                Me.m_plAppearance.Controls.Add(Me.m_ucEditVisualStyle)
            End If

            Me.m_grid.Layer = Me.m_layerWork
            Me.m_grid.VectorFieldIndex = Me.m_iVectorData
            Me.m_grid.RefreshContent()

            Me.m_tlpDetails.PerformLayout()
            Me.m_tlpBits.PerformLayout()

        End Sub

        Private Sub DrawPreview()
            Me.m_zoommap.Map.Refresh()
        End Sub

        Private Sub UpdateControls()

            Dim bEditable As Boolean = True

            If (Me.m_layerOriginal.Editor IsNot Nothing) Then
                bEditable = (Me.m_layerOriginal.Editor.IsReadOnly = False)
            End If

            Me.m_tsddImport.Enabled = bEditable
            Me.Text = cStringUtils.Localize(My.Resources.ECOSPACE_CAPTION_EDITLAYER, Me.m_tbNameValue.Text)

            Me.m_tscmbVectorData.Visible = Me.m_bIsVectorData

            Dim data As cEcospaceLayer = Me.m_layerWork.Data
            Dim sg As cStyleGuide = Me.m_uic.StyleGuide

            Me.m_tbxMaxValue.Text = sg.FormatNumber(data.MaxValue)
            Me.m_tbxMinValue.Text = sg.FormatNumber(data.MinValue)
            Me.m_tbxNoCellsValue.Text = sg.FormatNumber(data.NumValueCells)

            If (TypeOf data Is cEcospaceLayerSingle) Then
                Me.m_tbxMeanValue.Visible = True : Me.m_lblMeanValue.Visible = True
                Me.m_tbxMeanValue.Text = sg.FormatNumber(DirectCast(data, cEcospaceLayerSingle).MeanValue)
            Else
                Me.m_tbxMeanValue.Visible = False : Me.m_lblMeanValue.Visible = False
            End If

        End Sub

        Private Function ApplyChanges() As Boolean

            Dim cf As cDisplayLayer.eChangeFlags = 0
            Dim src As cCoreInputOutputBase = Me.m_layerOriginal.Source

            If Me.m_fpName.Enabled Then
                Me.m_layerOriginal.Name = CStr(Me.m_fpName.Value)
            End If

            If (TypeOf src Is cEcospaceLayerDriver) Then
                src.SetVariable(eVarNameFlags.UnitEnvDriver, Me.m_fpUnits.Value)
                src.SetVariable(eVarNameFlags.Description, Me.m_fpDescription.Value)
            End If

            If (TypeOf src Is cEcospaceLayerImportance) Then
                src.SetVariable(eVarNameFlags.ImportanceWeight, Me.m_fpWeight.Value)
                src.SetVariable(eVarNameFlags.Description, Me.m_fpDescription.Value)
            End If

            If (Me.m_ucEditVisualStyle IsNot Nothing) Then
                ' Apply changes
                Me.m_ucEditVisualStyle.Apply(Me.m_layerOriginal.Renderer.VisualStyle)
                cf = cf Or cDisplayLayer.eChangeFlags.VisualStyle
            End If

            Me.m_grid.Apply(Me.m_layerOriginal)
            cf = cf Or cDisplayLayer.eChangeFlags.Map

            ' Fire layer changed notification
            Me.m_layerOriginal.Update(cf)

            Return True

        End Function

        Private Sub m_cmbCopyStyleFrom_Format(sender As Object, e As ListControlConvertEventArgs) Handles m_cmbCopyStyleFrom.Format
            Dim item As cDisplayLayerRaster = CType(e.ListItem, cDisplayLayerRaster)
            If (item Is Nothing) Then
                e.Value = SharedResources.GENERIC_VALUE_NONE
            Else
                Dim layer As cEcospaceLayer = item.Data
                If (layer.Index < 1) Then
                    e.Value = layer.Name
                Else
                    e.Value = cStringUtils.Localize(SharedResources.GENERIC_LABEL_INDEXED, layer.Index, layer.Name)
                End If
            End If
        End Sub

        Private Sub m_cmbCopyStyleFrom_SelectedIndexChanged(sender As Object, e As EventArgs) Handles m_cmbCopyStyleFrom.SelectedIndexChanged

            If (Me.m_ucEditVisualStyle Is Nothing) Then Return
            Dim item As cDisplayLayerRaster = CType(Me.m_cmbCopyStyleFrom.SelectedItem, cDisplayLayerRaster)
            If (item Is Nothing) Then Return

            Me.m_ucEditVisualStyle.VisualStyle = item.Renderer.VisualStyle

        End Sub

        Private Sub m_cmbCopyStyleFrom_DrawItem(sender As Object, e As DrawItemEventArgs) Handles m_cmbCopyStyleFrom.DrawItem

            Dim fmt As New StringFormat(StringFormatFlags.NoWrap)
            fmt.LineAlignment = StringAlignment.Center
            Dim bIsSelected As Boolean = ((e.State And DrawItemState.Selected) > 0)

            e.DrawBackground()

            If (e.Index >= 0) Then
                Dim item As cDisplayLayerRaster = CType(Me.m_cmbCopyStyleFrom.Items(e.Index), cDisplayLayerRaster)
                If (item IsNot Nothing) Then
                    ' ToDo: add R2L support
                    Dim rc As New Rectangle(2, e.Bounds.Top, e.Bounds.Width - 30, e.Bounds.Height)
                    e.Graphics.DrawString(item.DisplayText, Me.Font, If(bIsSelected, SystemBrushes.HighlightText, SystemBrushes.ControlText), rc, fmt)
                    rc.X = rc.Width + 2
                    rc.Width = 22
                    rc.Y += 2
                    rc.Height -= 4
                    item.Renderer.RenderPreview(e.Graphics, rc)
                    e.Graphics.DrawRectangle(Pens.Black, rc)
                End If
            End If

            e.DrawFocusRectangle()

        End Sub

#End Region ' Internal implementation

    End Class

End Namespace