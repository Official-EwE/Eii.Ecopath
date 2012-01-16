#Region " Imports "

Option Strict On
Imports EwECore
Imports EwECore.Auxiliary
Imports EwEUtils.Core
Imports ScientificInterfaceShared.Controls.Map
Imports ScientificInterfaceShared.Controls.Map.Layers
Imports ScientificInterfaceShared.Properties
Imports ScientificInterfaceShared.Style

#End Region ' Imports

Namespace Controls.Map.Layers

    ''' <summary>
    ''' Layer that wraps a collection of <see cref="cLayer"/> for bundled display in the UI.
    ''' </summary>
    Public Class cLayerBundle
        Inherits cLayer

        Private m_layers As cEcospaceLayer()
        Private m_iLayer As Integer = 0
        Private m_cc As eCoreCounterTypes = eCoreCounterTypes.NotSet

        Public Sub New(uic As cUIContext, _
                       data As cEcospaceLayer(), _
                       renderer As cLayerRenderer, _
                       editor As cLayerEditor, _
                       cc As eCoreCounterTypes, _
                       ByVal source As cCoreInputOutputBase, _
                       Optional ByVal varName As eVarNameFlags = eVarNameFlags.Name, _
                       Optional ByVal sValueSet As Single = cCore.NULL_VALUE, _
                       Optional ByVal sValueClear As Single = cCore.NULL_VALUE)

            MyBase.New(uic, data(0), renderer, editor, source, varName, sValueSet, sValueClear)
            Me.m_layers = data

        End Sub

        Public Property iLayer As Integer
            Get
                Dim i As Integer = Me.m_iLayer
                If (Me.m_cc = eCoreCounterTypes.nGroups) Then i += 1
                Return i
            End Get
            Set(value As Integer)
                If (Me.m_cc = eCoreCounterTypes.nGroups) Then value -= 1
                Me.m_iLayer = Math.Max(0, value)
            End Set
        End Property

        Public Overrides ReadOnly Property Data As EwECore.cEcospaceLayer
            Get
                Return Me.m_layers(Me.m_iLayer)
            End Get
        End Property

    End Class ' Layer

End Namespace
