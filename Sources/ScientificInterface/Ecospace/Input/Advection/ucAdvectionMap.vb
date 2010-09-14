#Region " Imports "

Option Strict On
Imports EwECore
Imports EwEUtils.Core
Imports ScientificInterface.Ecospace.Basemap.Layers
Imports ScientificInterfaceShared.Controls
Imports System.ComponentModel

#End Region ' Imports

Namespace Ecospace.Advection

    ''' <summary>
    ''' Base control for implementing maps on the advection form.
    ''' </summary>
    Public Class ucAdvectionMap
        Implements IUIElement

#Region " Private vars "

        ''' <summary>UI context to operate on.</summary>
        Private m_uic As cUIContext = Nothing
        ''' <summary>The layer that can be edited in this map, if any.</summary>
        Private m_layerEditable As cLayer = Nothing
        ''' <summary>The name of the map to display in the header.</summary>
        Private m_strMapName As String = "<header>"

#End Region ' Private vars

#Region " Public access "

        ''' -------------------------------------------------------------------
        ''' <inheritdoc cref="IUIElement.UIContext"/>
        ''' -------------------------------------------------------------------
        Public Property UIContext() As cUIContext _
            Implements IUIElement.UIContext
            Get
                Return Me.m_uic
            End Get
            Set(ByVal value As cUIContext)
                If Me.m_uic IsNot Nothing Then
                    Me.ClearMap()
                End If
                Me.m_uic = value
                If Me.m_uic IsNot Nothing Then
                    Me.PopulateMap()
                End If
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get the <see cref="ucMapZoom">Zoom control</see> that wraps the
        ''' embedded <see cref="Map">Map</see>.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public ReadOnly Property ZoomCtrl() As ucMapZoom
            Get
                Return Me.m_zoomctrl
            End Get
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get the <see cref="Ecospace.ucMap">Map control</see> displayed here.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public ReadOnly Property Map() As Ecospace.ucMap
            Get
                Return Me.m_zoomctrl.Map
            End Get
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get the layer that the user can edit in this map.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public ReadOnly Property LayerEdit() As cLayer
            Get
                Return Me.m_layerEditable
            End Get
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the name of the map to display in the header.
        ''' </summary>
        ''' -------------------------------------------------------------------
        <Browsable(True)> _
        Public Property MapName() As String
            Get
                Return Me.m_strMapName
            End Get
            Set(ByVal value As String)
                Me.m_strMapName = value
                Me.m_hdrTitle.Text = Me.m_strMapName
            End Set
        End Property

#End Region ' Public access

#Region " Overridables "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Specify the <see cref="eVarNameFlags">var name</see> identifying the
        ''' one editable layer in the attached <see cref="Map">map</see>.
        ''' </summary>
        ''' <returns>A variable name, or <see cref="eVarNameFlags.NotSet">NotSet</see>
        ''' if the user cannot edit this map.</returns>
        ''' -------------------------------------------------------------------
        Protected Overridable Function EditableLayer() As eVarNameFlags
            Return eVarNameFlags.NotSet
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Specify the <see cref="eVarNameFlags">var names</see> identifying the
        ''' background layers in the attached <see cref="Map">map</see>.
        ''' </summary>
        ''' <returns>
        ''' A list of variable names to show on top of the already present
        ''' <see cref="eVarNameFlags.LayerDepth">Ecospace depth layer</see>.
        ''' </returns>
        ''' -------------------------------------------------------------------
        Protected Overridable Function BackgroundLayers() As eVarNameFlags()
            Return Nothing
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Update the state and content of local controls.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Protected Overridable Sub UpdateControls()
            Dim img As Image = Nothing

            If Me.Enabled Then
                If Me.LayerEdit IsNot Nothing Then
                    If Me.LayerEdit.Editor.IsEditable Then
                        img = My.Resources.Editable
                    Else
                        img = My.Resources.NotEditable
                    End If
                End If
            End If
            Me.m_hdrTitle.Image = img

        End Sub

#End Region ' Overridables

#Region " Events "

        Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)
            MyBase.OnLoad(e)
            Me.UpdateControls()
        End Sub

        Protected Overrides Sub OnEnabledChanged(ByVal e As System.EventArgs)
            MyBase.OnEnabledChanged(e)
            Me.UpdateControls()
        End Sub

        Protected Overridable Sub OnLayerChanged(ByVal l As cLayer, ByVal changeFlags As cLayer.eChangeFlags)
            If ((changeFlags And cLayer.eChangeFlags.Editable) > 0) Then Me.UpdateControls()
        End Sub

#End Region ' Events

#Region " Internal implementation "

        Private Sub PopulateMap()

            Me.m_zoomctrl.Map.Basemap = Me.m_uic.Core.EcospaceBasemap

            ' Always show depth layer
            Me.AddLayer(eVarNameFlags.LayerDepth, False)
            ' Add optional background layers
            If (Me.BackgroundLayers IsNot Nothing) Then
                For Each vn As eVarNameFlags In Me.BackgroundLayers
                    If vn <> eVarNameFlags.NotSet Then
                        Me.AddLayer(vn, False)
                    End If
                Next
            End If

            If Me.EditableLayer <> eVarNameFlags.NotSet Then
                Me.m_layerEditable = Me.AddLayer(Me.EditableLayer, True)
                Me.m_zoomctrl.Map.Editable = True
            Else
                Me.m_zoomctrl.Map.Editable = False
            End If

            ' Start observing layer changes
            If (Me.m_layerEditable IsNot Nothing) Then
                AddHandler Me.m_layerEditable.LayerChanged, AddressOf OnLayerChanged
            End If

        End Sub

        Protected Overridable Sub ClearMap()

            If (Me.m_layerEditable IsNot Nothing) Then
                RemoveHandler Me.m_layerEditable.LayerChanged, AddressOf OnLayerChanged
                Me.m_layerEditable = Nothing
            End If

            Me.m_zoomctrl.Map.Clear()
            Me.m_zoomctrl.Map.Basemap = Nothing

        End Sub

        Private Function AddLayer(ByVal vn As eVarNameFlags, ByVal bEditable As Boolean) As cLayer

            Dim layers() As cLayer = cLayerFactory.GetLayers(Me.m_uic, vn)
            Dim l As cLayer = Nothing

            If (layers Is Nothing) Then Return Nothing
            If (layers.Length <> 1) Then
                Debug.Assert(False, "No such layers found")
                Return Nothing
            End If

            l = layers(0)

            If bEditable Then
                l.Editor.IsEditable = True
                l.IsSelected = True
            Else
                l.Editor.IsEditable = False
            End If
            Me.m_zoomctrl.Map.AddLayer(l)
            Return l

        End Function

#End Region ' Internal implementation

    End Class

End Namespace
