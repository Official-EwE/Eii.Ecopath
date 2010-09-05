#Region " Imports "

Option Strict On
Imports EwECore
Imports EwEUtils.Core
Imports ScientificInterface.Ecospace.Basemap.Layers
Imports ScientificInterfaceShared.Controls

#End Region ' Imports

Namespace Ecospace.Advection

    ''' <summary>
    ''' Wind control for advection form.
    ''' </summary>
    Public Class ucWind
        Implements IUIElement

        Private m_uic As cUIContext = Nothing

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

        Public ReadOnly Property ZoomCtrl() As ucMapZoom
            Get
                Return Me.m_zoomctrl
            End Get
        End Property

        Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)

            If Me.m_uic Is Nothing Then Return

        End Sub

        Private Sub PopulateMap()

            Me.m_zoomctrl.Map.Basemap = Me.m_uic.Core.EcospaceBasemap
            Me.m_zoomctrl.Map.Editable = True

            Me.AddLayers(eVarNameFlags.LayerDepth, False)
            Me.AddLayers(eVarNameFlags.LayerWind, True)

        End Sub

        Private Sub ClearMap()
            Me.m_zoomctrl.Map.Clear()
            Me.m_zoomctrl.Map.Basemap = Nothing
        End Sub

        Private Sub AddLayers(ByVal vn As eVarNameFlags, ByVal bEditable As Boolean)
            For Each l As cLayer In cLayerFactory.GetLayers(Me.m_uic, vn)
                If bEditable Then
                    l.Editor.IsReadOnly = False
                    l.IsSelected = True
                Else
                    l.Editor.IsReadOnly = True
                End If
                Me.m_zoomctrl.Map.AddLayer(l)
            Next l
        End Sub

        Private Sub UpdateControls()

        End Sub

#Region " Map "

        Private Sub OnLayerChanged(ByVal l As cLayer, ByVal changeFlags As cLayer.eChangeFlags)
            If ((changeFlags And cLayer.eChangeFlags.Selected) > 0) Then Me.UpdateControls()
        End Sub

#End Region ' Map

    End Class

End Namespace
