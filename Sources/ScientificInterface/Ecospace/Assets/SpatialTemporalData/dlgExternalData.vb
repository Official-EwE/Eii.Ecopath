#Region " Imports "

Option Strict On

Imports EwEUtils.SpatialData
Imports EwECore.SpatialData
Imports SharedResources = ScientificInterfaceShared.My.Resources
Imports EwECore
Imports ScientificInterface.Ecospace.Basemap.Layers

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

#End Region ' Private vars

#Region " Construction / destruction "

        Public Sub New(ByVal uic As cUIContext)
            MyBase.New()
            Me.SetStyle(ControlStyles.OptimizedDoubleBuffer, True)
            Me.InitializeComponent()
            Me.UIContext = uic
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

            Debug.Assert(man IsNot Nothing)

            ' For all adapters
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
            Me.m_tvAdapters.ImageList = Me.m_ilConnections

            ' Update images
            Me.UpdateNodeImages()

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
                Debug.Assert(False, "Bing!")
            End Try

        End Sub

        Private Sub OnEcospaceMessage(ByRef msg As cMessage)
            If msg.Type = eMessageType.DataModified Then
                Me.UpdateNodeImages()
            End If
        End Sub

#End Region ' Event handlers

#Region " Internals "

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
                End If
            End Set
        End Property

        Private ReadOnly Property SelectedAdapter As cSpatialDataAdapter
            Get
                Dim tag As Object = Me.m_tvAdapters.SelectedNode.Tag

                If (tag Is Nothing) Then Return Nothing
                If (Not TypeOf tag Is cLayerLink) Then Return Nothing

                Return DirectCast(tag, cLayerLink).Adapter
            End Get
        End Property

        Private ReadOnly Property SelectedLayer As cEcospaceLayer
            Get
                Dim tag As Object = Me.m_tvAdapters.SelectedNode.Tag

                If (tag Is Nothing) Then Return Nothing
                If (Not TypeOf tag Is cLayerLink) Then Return Nothing

                Return DirectCast(tag, cLayerLink).Layer
            End Get
        End Property

        Private Sub UpdateNodeImages()

            For Each ndAdt As TreeNode In Me.m_tvAdapters.Nodes
                Dim iNumConnected As Integer = 0
                For Each ndLayer As TreeNode In ndAdt.Nodes
                    Dim link As cLayerLink = DirectCast(ndLayer.Tag, cLayerLink)
                    Dim l As cEcospaceLayer = link.Layer
                    If l.IsExternalData Then
                        iNumConnected += 1
                        ndLayer.ImageIndex = 1
                    Else
                        ndLayer.ImageIndex = 0
                    End If
                    ndLayer.SelectedImageIndex = ndLayer.ImageIndex
                Next
                If (iNumConnected > 0) Then
                    ndAdt.ImageIndex = 1
                Else
                    ndAdt.ImageIndex = 0
                End If
                ndAdt.SelectedImageIndex = ndAdt.ImageIndex
            Next
        End Sub

#End Region ' Internals

    End Class

End Namespace

