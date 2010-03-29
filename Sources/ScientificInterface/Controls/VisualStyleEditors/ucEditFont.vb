#Region " Imports "

Option Strict On
Imports EwECore
Imports EwECore.Auxiliary

#End Region ' Imports

Namespace Controls

    Public Class ucEditFont

#Region " Constructor "

        Public Sub New(ByVal uic As cUIContext, _
                       ByVal vs As cVisualStyle, _
                       ByVal style As cVisualStyle.eVisualStyleTypes)
            MyBase.New(vs, style)
            Me.InitializeComponent()
        End Sub

#End Region ' Constructor

#Region " Overrides "

        Public Overrides Function Apply(ByVal vs As cVisualStyle) As Boolean
            Return True
        End Function

#End Region ' Overrides

    End Class

End Namespace
