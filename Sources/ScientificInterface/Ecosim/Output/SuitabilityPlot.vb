
Namespace Ecosim

    Public Class SuitabilityPlot

        Public Sub New()
            Me.InitializeComponent()
        End Sub

        Public Overrides Property UIContext() As cUIContext
            Get
                Return MyBase.UIContext
            End Get
            Set(ByVal value As cUIContext)
                MyBase.UIContext = value
                Me.m_plot.UIContext = value
            End Set
        End Property

        Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)
            MyBase.OnLoad(e)
        End Sub
    End Class

End Namespace

