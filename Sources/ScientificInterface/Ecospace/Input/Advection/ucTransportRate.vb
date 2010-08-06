#Region " Imports "

Option Strict On
Imports EwECore
Imports EwEUtils.Core
Imports ScientificInterface.Ecospace.Basemap.Layers
Imports ScientificInterfaceShared.Controls

#End Region ' Imports

Namespace Ecospace.Advection

    Public Class ucTransportRate
        Implements IUIElement

        Private m_uic As cUIContext = Nothing

        Public Property UIContext() As cUIContext _
            Implements IUIElement.UIContext
            Get
                Return Me.m_uic
            End Get
            Set(ByVal value As cUIContext)
                Me.m_uic = value
            End Set
        End Property

        Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)

            If Me.m_uic Is Nothing Then Return

            Me.m_map.Basemap = Me.m_uic.Core.EcospaceBasemap
            Me.AddLayers(eVarNameFlags.LayerDepth, False)
            Me.AddLayers(eVarNameFlags.LayerAdvection, True)

        End Sub

        Private Sub AddLayers(ByVal vn As eVarNameFlags, ByVal bEditable As Boolean)
            For Each l As cLayer In cLayerFactory.GetLayers(Me.m_uic, vn)
                l.Editor.IsReadOnly = Not bEditable
                Me.m_map.AddLayer(l)
            Next l
        End Sub

    End Class

End Namespace
