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
' Copyright 1991-2012 UBC Fisheries Centre, Vancouver BC, Canada.
' ===============================================================================
'
#Region " Imports "

Option Strict On

Imports EwEUtils.SpatialData
Imports EwECore.SpatialData
Imports SharedResources = ScientificInterfaceShared.My.Resources
Imports EwECore
Imports ScientificInterface.Ecospace.Basemap.Layers
Imports EwECore.Ecospace

#End Region ' Imports

Namespace Ecospace

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Dialog for linking external data to Ecospace layers.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Class dlgExternalData

#Region " Private classes "

        Private Class cLayerLink
            Private m_adt As cSpatialDataAdapter
            Private m_layer As cEcospaceLayer

            Public Sub New(adt As cSpatialDataAdapter, layer As cEcospaceLayer)
                Me.m_adt = adt
                Me.m_layer = layer
            End Sub

            Public ReadOnly Property Adapter As cSpatialDataAdapter
                Get
                    Return Me.m_adt
                End Get
            End Property

            Public ReadOnly Property Layer As cEcospaceLayer
                Get
                    Return Me.m_layer
                End Get
            End Property
        End Class

#End Region ' Private classes

#Region " Private vars "

        ''' <summary>UI context to operate onto.</summary>
        Private m_uic As cUIContext = Nothing
        ''' <summary>Ecospace message handler to respond to.</summary>
        Private m_mhEcospace As cMessageHandler = Nothing
        Private m_layerStartup As cEcospaceLayer = Nothing

#End Region ' Private vars

#Region " Construction / destruction "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Constructor.
        ''' </summary>
        ''' <param name="uic"><see cref="cUIContext"/> to operate onto.</param>
        ''' <param name="layer"><see cref="cEcospaceLayer">Ecospace data layer</see> to configure, if any.</param>
        ''' -------------------------------------------------------------------
        Public Sub New(ByVal uic As cUIContext, Optional layer As cEcospaceLayer = Nothing)
            MyBase.New()
            Me.SetStyle(ControlStyles.OptimizedDoubleBuffer Or ControlStyles.AllPaintingInWmPaint, True)
            Me.InitializeComponent()
            Me.UIContext = uic
            Me.m_layerStartup = layer
        End Sub

#End Region ' Construction / destruction

#Region " Form overrides "

        Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)
            MyBase.OnLoad(e)

            ' Sanity checks
            If (Me.UIContext Is Nothing) Then Return

            Debug.Assert(Me.UIContext.Core.StateMonitor.HasEcospaceLoaded)

            Dim man As cSpatialDataConnectionManager = Me.m_uic.Core.SpatialDataConnectionManager
            Dim bm As cEcospaceBasemap = Me.m_uic.Core.EcospaceBasemap
            Dim ecospaceModelParams As cEcospaceModelParameters = Me.UIContext.Core.EcospaceModelParameters()
            Dim fact As New cLayerFactoryInternal()
            Dim strAdapter As String = ""
            Dim dtGroupNodes As New Dictionary(Of String, TreeNode)
            Dim tnAdapter As TreeNode = Nothing
            Dim tnLayer As TreeNode = Nothing
            Dim layers() As cEcospaceLayer = Nothing
            Dim bHasExternalLayer As Boolean = False
            Dim tnSelect As TreeNode = Nothing

            Debug.Assert(man IsNot Nothing)

            ' Build data adapter treeview
            For Each adt As cSpatialDataAdapter In man.Adapters

                ' Get group name for the adapter
                strAdapter = fact.GetLayerGroup(adt.VarName)
                ' Get layers for the adapter
                layers = bm.Layers(adt.VarName)

                ' Get the node for this group, which may already exist
                If Not dtGroupNodes.ContainsKey(strAdapter) Then
                    dtGroupNodes(strAdapter) = New TreeNode(strAdapter)
                End If
                tnAdapter = dtGroupNodes(strAdapter)

                ' If adapter has layers
                If (layers.Length > 0) Then
                    ' Assume layers are not connected yet
                    bHasExternalLayer = False
                    ' For all layers in the adapter
                    For Each layer As cEcospaceLayer In layers
                        ' Create tree node
                        tnLayer = New TreeNode(layer.Name)
                        ' Store data link
                        tnLayer.Tag = New cLayerLink(adt, layer)
                        ' Add to parent
                        tnAdapter.Nodes.Add(tnLayer)
                        ' Check whether any layer is connected to external data
                        bHasExternalLayer = bHasExternalLayer Or layer.IsExternalData

                        ' Selection
                        If (Me.m_layerStartup IsNot Nothing) Then
                            If (Object.ReferenceEquals(Me.m_layerStartup, layer)) Then
                                tnSelect = tnLayer
                            End If
                        End If
                    Next

                    ' Group layer not added yet?
                    If (Not Me.m_tvAdapters.Nodes.Contains(tnAdapter)) Then
                        ' #Yes: add the node
                        Me.m_tvAdapters.Nodes.Add(tnAdapter)
                        ' Expand node if any of its layers is connected to external data
                        If bHasExternalLayer Then tnAdapter.Expand()
                    End If

                End If
            Next

            ' Create image list
            Me.m_ilConnections.Images.Add(SharedResources.database_NA)
            Me.m_ilConnections.Images.Add(SharedResources.Database)
            Me.m_ilConnections.Images.Add(SharedResources.database_warning)
            Me.m_ilConnections.Images.Add(SharedResources.ani_loader)

            ' Connect image list
            Me.m_tvAdapters.ImageList = Me.m_ilConnections

            ' Update displayed images
            Me.UpdateNodeImages()

            ' Initialize selection
            If (tnSelect IsNot Nothing) Then
                Me.m_tvAdapters.SelectedNode = tnSelect
            End If

            ' Ooh!
            Me.CenterToParent()

        End Sub

        Protected Overrides Sub OnFormClosed(e As System.Windows.Forms.FormClosedEventArgs)

            ' Release config screen
            Me.m_tvAdapters.Nodes.Clear()
            Me.UIContext = Nothing

            ' Dome
            MyBase.OnFormClosed(e)

        End Sub

#End Region ' Form overrides

#Region " Event handlers "

        Private Sub OnOK(ByVal sender As Object, ByVal e As System.EventArgs) _
            Handles m_btnOK.Click
            Me.DialogResult = DialogResult.OK
            Me.Close()
        End Sub

        Private Sub OnNodeSelected(sender As System.Object, e As TreeViewEventArgs) _
            Handles m_tvAdapters.AfterSelect

            Try
                Me.m_config.SetConnection(Me.SelectedAdapter, Me.SelectedLayer)
            Catch ex As Exception
                ' Whoopy
                Debug.Assert(False, ex.Message)
            End Try

        End Sub

        Private Sub OnEcospaceMessage(ByRef msg As cMessage)
            If (msg.Type = eMessageType.DataModified) Then
                Me.UpdateNodeImages()
            End If
        End Sub

#End Region ' Event handlers

#Region " Internals "

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="IUIElement.UIContext"/>
        ''' -------------------------------------------------------------------
        Private Property UIContext As ScientificInterfaceShared.Controls.cUIContext _
            Implements ScientificInterfaceShared.Controls.IUIElement.UIContext
            Get
                Return Me.m_uic
            End Get
            Set(ByVal value As ScientificInterfaceShared.Controls.cUIContext)

                ' Clean up
                If (Me.m_uic IsNot Nothing) Then
                    Me.m_uic.Core.Messages.RemoveMessageHandler(Me.m_mhEcospace)
                    Me.m_mhEcospace.Dispose()
                    Me.m_mhEcospace = Nothing
                    Me.m_config.UIContext = Nothing
                End If

                Me.m_uic = value

                ' Set new
                If (Me.m_uic IsNot Nothing) Then
                    Me.m_config.UIContext = Me.m_uic
                    Me.m_mhEcospace = New cMessageHandler(AddressOf OnEcospaceMessage, EwEUtils.Core.eCoreComponentType.EcoSpace, eMessageType.DataModified, Me.m_uic.SyncObject)
                    Me.m_uic.Core.Messages.AddMessageHandler(Me.m_mhEcospace)

#If DEBUG Then
                    Me.m_mhEcospace.Name = "dlgExternalData::m_mhEcospace"
#End If
                End If
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get the <see cref="cSpatialDataAdapter"/> selected in the 
        ''' data adapter treeview.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Private ReadOnly Property SelectedAdapter As cSpatialDataAdapter
            Get
                Dim tag As Object = Me.m_tvAdapters.SelectedNode.Tag

                If (tag Is Nothing) Then Return Nothing
                If (Not TypeOf tag Is cLayerLink) Then Return Nothing

                Return DirectCast(tag, cLayerLink).Adapter
            End Get
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get the <see cref="cEcospaceLayer"/> selected in the 
        ''' data adapter treeview.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Private ReadOnly Property SelectedLayer As cEcospaceLayer
            Get
                Dim tag As Object = Me.m_tvAdapters.SelectedNode.Tag

                If (tag Is Nothing) Then Return Nothing
                If (Not TypeOf tag Is cLayerLink) Then Return Nothing

                Return DirectCast(tag, cLayerLink).Layer
            End Get
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Refresh node images in the data adapter treeview, based on the current 
        ''' external data configuration.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Private Sub UpdateNodeImages()

            For Each ndAdt As TreeNode In Me.m_tvAdapters.Nodes
                Dim iNumConnected As Integer = 0
                Dim iImg As Integer = 0

                For Each ndLayer As TreeNode In ndAdt.Nodes
                    Dim link As cLayerLink = DirectCast(ndLayer.Tag, cLayerLink)
                    Dim l As cEcospaceLayer = link.Layer

                    ' Calc display image
                    iImg = 0
                    If l.IsExternalData Then
                        iNumConnected += 1
                        Dim ds As ISpatialDataSet = link.Adapter.Dataset(l.Index)
                        Dim comp As New cDatasetCompatilibity(Me.m_uic.Core, ds)
                        Select Case comp.Compatibility
                            Case cDatasetCompatilibity.eCompatibilityTypes.NoTemporal, _
                                cDatasetCompatilibity.eCompatibilityTypes.NoSpatial, _
                                cDatasetCompatilibity.eCompatibilityTypes.Errors
                                iImg = 2
                            Case cDatasetCompatilibity.eCompatibilityTypes.PartialSpatial, _
                                cDatasetCompatilibity.eCompatibilityTypes.TotalOverlap
                                iImg = 1
                            Case Else
                                ' NOP
                        End Select
                    End If

                    ' Update image
                    If (iImg <> ndLayer.ImageIndex) Then
                        ndLayer.ImageIndex = iImg
                        ndLayer.SelectedImageIndex = iImg
                    End If

                Next

                ' Calc display image
                iImg = Math.Min(1, iNumConnected)
                ' Img has changed?
                If (iImg <> ndAdt.ImageIndex) Then
                    ' Update image
                    ndAdt.ImageIndex = iImg
                    ndAdt.SelectedImageIndex = iImg
                End If

            Next
        End Sub

#End Region ' Internals

    End Class

End Namespace

