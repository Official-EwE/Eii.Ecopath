#Region " Imports "

Option Strict On

Imports EwECore
Imports EwEUtils.Commands
Imports ScientificInterface.Ecospace.Basemap.Layers

#End Region ' Imports

Namespace Ecospace

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Container for <see cref="ucLayerGroup"/>s.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Class ucLayersControl

        Private m_uic As cUIContext = Nothing
        Private m_dtGroups As New Dictionary(Of String, ucLayerGroup)
        Private m_dtLayerToGroup As New Dictionary(Of cLayer, String)

        Public Sub New(ByVal uic As cUIContext)
            Me.InitializeComponent()
            Me.Dock = DockStyle.Fill
            Me.m_uic = uic
        End Sub

#Region " Item access "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Add a layer to this control.
        ''' </summary>
        ''' <param name="layer">The <see cref="cLayer">layer</see> to add.</param>
        ''' <param name="layerPosition">Layer to position this layer before, if any</param>
        ''' <remarks>A layer can only be added once.</remarks>
        ''' -------------------------------------------------------------------
        Public Sub AddLayer(ByVal layer As cLayer, ByVal strGroup As String, Optional ByVal layerPosition As cLayer = Nothing)

            Dim ucg As ucLayerGroup = Me.FindGroup(strGroup)

            If (ucg Is Nothing) Then
                ' Add group
                Me.AddGroup(strGroup)
                ucg = Me.FindGroup(strGroup)
            End If

            ' Add layer
            ucg.AddLayer(layer, layerPosition)
            Me.m_dtLayerToGroup.Add(layer, strGroup)
            AddHandler layer.LayerChanged, AddressOf OnLayerChanged

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Remove a layer from this control.
        ''' </summary>
        ''' <param name="layer">The <see cref="cLayer">layer</see> to remove.</param>
        ''' <remarks>A layer can only be removed once.</remarks>
        ''' -------------------------------------------------------------------
        Public Sub RemoveLayer(ByVal layer As cLayer)

            Dim ucg As ucLayerGroup = Me.FindGroup(layer)

            If Object.ReferenceEquals(ucg, Nothing) Then Return

            ' Remove layer
            RemoveHandler layer.LayerChanged, AddressOf OnLayerChanged
            ucg.RemoveLayer(layer)
            Me.m_dtLayerToGroup.Remove(layer)

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Add a layer group to this control.
        ''' </summary>
        ''' <param name="strGroup">Name of the group to add.</param>
        ''' -------------------------------------------------------------------
        Public Sub AddGroup(ByVal strGroup As String, _
                            Optional ByVal bVisible As Boolean = True, _
                            Optional ByVal bClearGroup As Boolean = True)
            Dim ucg As ucLayerGroup = Nothing

            ' Group already exists?
            If Me.m_dtGroups.ContainsKey(strGroup) Then
                ' #Yes: get group layer control
                ucg = Me.FindGroup(strGroup)
                ' Must clear?
                If bClearGroup Then
                    ' #Yes: clear it
                    ucg.RemoveAllLayers()
                End If
            Else
                ' #No: create new group layer control
                ucg = New ucLayerGroup(Me.m_uic, strGroup)
                Me.fpItems.Controls.Add(ucg)
                Me.m_dtGroups(strGroup) = ucg
            End If

            ' Configure group layer control
            ucg.ShowAllLayers(bVisible)
            ucg.SetCollapsed(Not bVisible)

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Remove a layer group from this control.
        ''' </summary>
        ''' <param name="strGroup">Name of the group to remove.</param>
        ''' -------------------------------------------------------------------
        Public Sub RemoveGroup(ByVal strGroup As String)

            If Not Me.m_dtGroups.ContainsKey(strGroup) Then Return

            Dim ucg As ucLayerGroup = Me.FindGroup(strGroup)

            If (ucg Is Nothing) Then Return

            Me.fpItems.Controls.Remove(ucg)
            Me.m_dtGroups.Remove(strGroup)

        End Sub

        Public Sub ShowGroup(ByVal strGroup As String, ByVal bShow As Boolean, Optional ByVal bShowGroupControl As Boolean = True)
            If Not Me.m_dtGroups.ContainsKey(strGroup) Then Return

            Dim ucg As ucLayerGroup = Me.FindGroup(strGroup)
            If (ucg Is Nothing) Then Return

            ucg.ShowAllLayers(bShow)
            ucg.Visible = bShowGroupControl
        End Sub

        Public Sub EnableGroup(ByVal strGroup As String, ByVal bEditable As Boolean)
            If Not Me.m_dtGroups.ContainsKey(strGroup) Then Return

            Dim ucg As ucLayerGroup = Me.FindGroup(strGroup)
            If (ucg Is Nothing) Then Return

            ucg.EnableAllLayers(bEditable)
        End Sub

        Private m_iLockCount As Integer = 0

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Pause item reorganization on this control.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Sub LockUpdates()

            If (Me.m_iLockCount = 0) Then
                Me.fpItems.SuspendLayout()
                Application.DoEvents()
                For Each uc As UserControl In Me.fpItems.Controls
                    DirectCast(uc, ucLayerGroup).LockUpdates()
                Next
            End If

            Me.m_iLockCount += 1

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Resume item reorganization on this control.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Sub UnlockUpdates()

            Me.m_iLockCount -= 1

            If (Me.m_iLockCount = 0) Then
                For Each uc As UserControl In Me.fpItems.Controls
                    DirectCast(uc, ucLayerGroup).UnlockUpdates()
                Next
                Me.ResizeControls()
                Me.fpItems.ResumeLayout()
            End If

        End Sub

#End Region ' Item access

#Region " Events "

        Private Sub OnLayerChanged(ByVal l As cLayer, ByVal updateType As cLayer.eChangeFlags)
            If ((updateType And cLayer.eChangeFlags.Selected) = cLayer.eChangeFlags.Selected) Then
                ' Make sure only one layer is selected at the time
                Me.UpdateSelectedLayer(l)
            End If
        End Sub

        Protected Overrides Sub OnResize(ByVal e As System.EventArgs)
            Me.fpItems.Width = Me.Width - Me.Margin.Horizontal
        End Sub

        Private Sub fpItems_Resize(ByVal sender As Object, ByVal e As System.EventArgs) Handles fpItems.Resize
            Me.ResizeControls()
        End Sub

#End Region ' Events

#Region " Implementation "

        Private Sub ResizeControls()

            Dim iWidth As Integer = Me.fpItems.ClientRectangle.Width - Me.fpItems.Margin.Horizontal
            Me.fpItems.SuspendLayout()
            For Each uc As UserControl In Me.fpItems.Controls
                uc.Width = iWidth
            Next uc
            Me.fpItems.ResumeLayout()

        End Sub

        ''' <summary>Flag to prevent selection update recursion.</summary>
        Private m_bInSelectionUpdate As Boolean = False

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Make sure only one layer is selected.
        ''' </summary>
        ''' <param name="layerSelect"><see cref="cLayer">Layer</see> that has been 
        ''' selected.</param>
        ''' -------------------------------------------------------------------
        Private Sub UpdateSelectedLayer(ByVal layerSelect As cLayer)

            ' Abort if already busy
            If Me.m_bInSelectionUpdate = True Then Return

            ' Flag as busy
            Me.m_bInSelectionUpdate = True

            ' First call: fire selection command
            Me.FireSelectionCommand(layerSelect)

            ' Clean selection state of all other layer
            For Each layerTest As cLayer In Me.m_dtLayerToGroup.Keys
                ' #Yes: is it selected?
                If ((Not Object.ReferenceEquals(layerTest, layerSelect)) And (layerTest.IsSelected() = True)) Then
                    ' #Yes: clear its selection state
                    layerTest.IsSelected = False
                    ' Make the world respond to this. Note that this call will call
                    ' OnLayerChanged, which in turn will call this method, UpdateSelectedLayer.
                    ' To prevent this from causing endless loops, the flag m_bInSelectionUpdate
                    ' allows only the first layer update (which is most likely a user-triggered
                    ' change in selection) to cause a deselect of all other layers.
                    layerTest.Update(cLayer.eChangeFlags.Selected)
                End If
            Next layerTest

            ' Done
            Me.m_bInSelectionUpdate = False

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Fire global selection command to allow users to manage remarks for
        ''' the <see cref="cLayer.Source">source attached a layer</see>.
        ''' </summary>
        ''' <param name="layer"><see cref="cLayer">Layer</see> that has been 
        ''' selected.</param>
        ''' -------------------------------------------------------------------
        Private Sub FireSelectionCommand(ByVal layer As cLayer)

            Dim cmdh As cCommandHandler = Me.m_uic.CommandHandler
            Dim cmd As cCommand = cmdh.GetCommand(cPropertySelectionCommand.COMMAND_NAME)
            Dim sc As cPropertySelectionCommand = Nothing
            Dim pm As cPropertyManager = Me.m_uic.PropertyManager
            Dim prop As cProperty = Nothing

            If Not Object.ReferenceEquals(layer, Nothing) Then
                prop = pm.GetProperty(layer.Source, layer.VarName)
            End If

            If cmd IsNot Nothing Then
                If (TypeOf cmd Is cPropertySelectionCommand) Then
                    sc = DirectCast(cmd, cPropertySelectionCommand)
                    sc.Invoke(prop)
                End If
            End If

        End Sub

        Private Function FindGroup(ByVal strGroup As String) As ucLayerGroup
            Return Me.m_dtGroups(strGroup)
        End Function

        Private Function FindGroup(ByVal l As cLayer) As ucLayerGroup
            If Me.m_dtLayerToGroup.ContainsKey(l) Then Return Me.FindGroup(Me.m_dtLayerToGroup(l))
            Return Nothing
        End Function

#End Region ' Implementation

    End Class

End Namespace