Imports ScientificInterfaceShared
Imports System.Windows.Forms

Public Class ucPlot

    Public Sub New()
        Me.InitializeComponent()
    End Sub

    ReadOnly Property Content As Control
        Get
            Return Me.m_plContent
        End Get
    End Property

End Class
