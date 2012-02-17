
Namespace Ecospace

    Public Class frmExternalDataConnections

        Public Overrides Property UIContext As ScientificInterfaceShared.Controls.cUIContext
            Get
                Return MyBase.UIContext
            End Get
            Set(value As ScientificInterfaceShared.Controls.cUIContext)
                MyBase.UIContext = value
                Me.m_ucDatasets.UIContext = value
            End Set
        End Property

    End Class

End Namespace
