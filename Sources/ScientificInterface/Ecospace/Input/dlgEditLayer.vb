#Region " Imports "

Option Explicit On
Option Strict On

Imports System.Windows.Forms
Imports System.IO
Imports System.Text
Imports SAUPUtil.SAUPFile
Imports SAUPUtil.SAUPData.Mapping
Imports SAUPUtil.SAUPData
Imports EwECore
Imports EwEUtils.Core
Imports EwEUtils.Commands
Imports EwECore.Auxiliary
Imports ScientificInterfaceShared.Controls.Map.Layers
Imports ScientificInterfaceShared.Controls.Map
Imports ScientificInterfaceShared.Commands

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
        Private m_layerOriginal As cLayer = Nothing
        Private m_layerDepth As cLayer = Nothing
        Private m_edittype As eLayerEditTypes

        ''' <summary>Work layer (a copy of the original) for this dialog to work on.</summary>
        Private m_layerWork As cLayer = Nothing
        ''' <summary>Editor to transmogrify the representation of the layer.</summary>
        Private m_ucEditVisualStyle As ucEditVisualStyle = Nothing

        Private m_fpName As cEwEFormatProvider = Nothing
        Private m_fpWeight As cEwEFormatProvider = Nothing
        Private m_fpDescription As cEwEFormatProvider = Nothing

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
        Public Sub New(ByVal uic As cUIContext, _
                       ByRef layer As cLayer, _
                       ByVal edittype As eLayerEditTypes)

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

            Me.m_layerWork = New cLayer(uic, layer) ' Work on a clone
            Me.m_layerWork.AllowValidation = False

        End Sub

#End Region ' Constructors

#Region " Overrides "

        Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)
            MyBase.OnLoad(e)

            Me.m_qehGrid = New cQuickEditHandler()
            Me.m_qehGrid.Attach(Me.m_grid, Me.m_uic, Me.m_tsGrid, Me.m_layerOriginal.Name)

            ' Show your stuff
            Me.m_zoommap.Map.AddLayer(Me.m_layerWork)
            If ((Not Object.ReferenceEquals(Me.m_layerOriginal, Me.m_layerDepth)) And _
                (Not Object.ReferenceEquals(Me.m_layerDepth, Nothing))) Then
                Me.m_zoommap.Map.AddLayer(Me.m_layerDepth)
            End If

            Me.m_tcLayerView.SelectedIndex = CInt(Me.m_edittype)

            Me.LoadLayer()
            Me.UpdateControls()
            Me.DrawPreview()

            If (Me.m_ucEditVisualStyle IsNot Nothing) Then
                AddHandler Me.m_ucEditVisualStyle.OnVisualStyleChanged, AddressOf OnVisualStyleChanged
            End If

        End Sub

        Protected Overrides Sub OnFormClosed(ByVal e As System.Windows.Forms.FormClosedEventArgs)

            If (Me.m_ucEditVisualStyle IsNot Nothing) Then
                RemoveHandler Me.m_ucEditVisualStyle.OnVisualStyleChanged, AddressOf OnVisualStyleChanged
            End If

            Me.m_qehGrid.Detach()
            Me.m_qehGrid = Nothing
            Me.m_grid.UIContext = Nothing

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

        Private Sub OnOK(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles OK_Button.Click

            If Not Me.ApplyChanges() Then Return
            Me.DialogResult = System.Windows.Forms.DialogResult.OK
            Me.Close()

        End Sub

        Private Sub OnCancel(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles Cancel_Button.Click

            Me.DialogResult = System.Windows.Forms.DialogResult.Cancel
            Me.Close()

        End Sub

        Private Sub OnApply(ByVal sender As Object, ByVal e As System.EventArgs) _
            Handles Apply_Button.Click

            Me.ApplyChanges()

        End Sub

        Private Sub OnVisualStyleChanged(ByVal sender As ucEditVisualStyle)

            ' Update work layer Visual Style
            Me.m_ucEditVisualStyle.Apply(Me.m_layerWork.Renderer.VisualStyle)
            Me.m_layerWork.Update(cLayer.eChangeFlags.VisualStyle)

        End Sub

        Private Sub OnImportLayer(sender As System.Object, e As System.EventArgs) _
            Handles m_tsbnImport.Click
            Try
                Dim cmd As cImportLayerCommand = DirectCast(Me.m_uic.CommandHandler.GetCommand(cImportLayerCommand.cCOMMAND_NAME), cImportLayerCommand)
                cmd.Invoke(New cLayer() {Me.m_layerWork})
            Catch ex As Exception

            End Try
        End Sub

        Private Sub OnExportLayer(sender As System.Object, e As System.EventArgs) _
            Handles m_tsbnExport.Click
            Try
                Dim cmd As cExportLayerCommand = DirectCast(Me.m_uic.CommandHandler.GetCommand(cExportLayerCommand.cCOMMAND_NAME), cExportLayerCommand)
                cmd.Invoke(New cLayer() {Me.m_layerWork})
            Catch ex As Exception

            End Try
        End Sub

        Private Sub OnNameChanged(sender As Object, e As System.EventArgs) _
            Handles m_tbNameValue.TextChanged
            Try
                Me.UpdateControls()
            Catch ex As Exception

            End Try
        End Sub

#End Region ' Local events

#Region " Internal implementation "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Diagnostic method, states if a layer has a unique core variable 
        ''' link. Layers with unique sources support extra's that can be stored
        ''' in the database such as remarks and visual styles.
        ''' </summary>
        ''' <returns></returns>
        ''' -------------------------------------------------------------------
        Private Function HasUniqueSource() As Boolean
            If (Me.m_layerOriginal.Source Is Nothing) Then Return False
            If (TypeOf Me.m_layerOriginal.Source Is cEcospaceBasemap) Then Return False
            Return True
        End Function

        Private Sub LoadLayer()

            Dim vs As cVisualStyle = Me.m_layerWork.Renderer.VisualStyle
            Dim src As cCoreInputOutputBase = Me.m_layerWork.Source

            Me.m_lblWeight.Visible = False
            Me.m_nudWeight.Visible = False
            Me.m_lblDescription.Visible = False
            Me.m_tbDescription.Visible = False

            Me.m_fpName = New cEwEFormatProvider(Me.m_uic, Me.m_tbNameValue, GetType(String))
            Me.m_fpWeight = New cEwEFormatProvider(Me.m_uic, Me.m_nudWeight, GetType(Single))
            Me.m_fpDescription = New cEwEFormatProvider(Me.m_uic, Me.m_tbNameValue, GetType(String))

            If (Me.HasUniqueSource()) Then
                Me.m_fpName.Enabled = True
                Me.m_tbRemarks.Text = src.Remark
                Me.m_tbRemarks.Enabled = True

                If TypeOf src Is cEcospaceLayerImportance Then
                    Me.m_lblWeight.Visible = True
                    Me.m_nudWeight.Visible = True
                    Me.m_lblDescription.Visible = True
                    Me.m_tbDescription.Visible = True

                    Me.m_fpWeight.Value = src.GetVariable(eVarNameFlags.ImportanceWeight)
                    Me.m_fpDescription.Value = src.GetVariable(eVarNameFlags.Description)
                End If
            Else
                Me.m_fpName.Enabled = False
                ' ToDo: globalize this
                Me.m_tbRemarks.Text = "Remarks not supported for this layer"
                Me.m_tbRemarks.Enabled = False
            End If

            ' Do not use display text; user may want to edit this
            Me.m_fpName.Value = m_layerWork.Name

            Me.m_ucEditVisualStyle = ucEditVisualStyle.GetEditor(Me.m_uic, vs, Me.m_layerWork.Renderer.VisualStyleFlags)

            If (Me.m_ucEditVisualStyle IsNot Nothing) Then
                Me.m_plAppearance.Height = Me.m_ucEditVisualStyle.Height
                Me.m_ucEditVisualStyle.Dock = DockStyle.Fill
                Me.m_plAppearance.Controls.Add(Me.m_ucEditVisualStyle)
            End If

            Me.m_grid.Layer = Me.m_layerWork
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

            Me.m_tsbnImport.Enabled = bEditable
            ' ToDo: globalize this
            Me.Text = String.Format("Edit layer '{0}'", Me.m_tbNameValue.Text)

        End Sub

        Private Function ApplyChanges() As Boolean

            Dim cf As cLayer.eChangeFlags = 0
            Dim src As cCoreInputOutputBase = Me.m_layerOriginal.Source

            If (HasUniqueSource()) Then

                Dim pm As cPropertyManager = Me.m_uic.PropertyManager
                Dim p As cProperty = pm.GetProperty(Me.m_layerOriginal.Source, eVarNameFlags.Name)

                If (p IsNot Nothing) Then
                    p.SetRemark(Me.m_tbRemarks.Text)
                    p.SetValue(CStr(Me.m_fpName.Value))
                End If

                If TypeOf Me.m_layerOriginal.Source Is cEcospaceLayerImportance Then
                    src.SetVariable(eVarNameFlags.ImportanceWeight, Me.m_fpWeight.Value)
                    src.SetVariable(eVarNameFlags.Description, Me.m_fpDescription.Value)
                End If

            End If

            If (Me.m_ucEditVisualStyle IsNot Nothing) Then
                ' Apply changes
                Me.m_ucEditVisualStyle.Apply(Me.m_layerOriginal.Renderer.VisualStyle)
                cf = cf Or cLayer.eChangeFlags.VisualStyle
            End If

            If Me.m_grid.Apply(Me.m_layerOriginal) Then
                cf = cf Or cLayer.eChangeFlags.Map
            End If

            ' Fire layer changed notification
            Me.m_layerOriginal.Update(cf)
            Return True

        End Function

#End Region ' Internal implementation

    End Class

End Namespace