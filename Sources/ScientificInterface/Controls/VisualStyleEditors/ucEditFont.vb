'==============================================================================
'
' $Log: ucEditFont.vb,v $
' Revision 1.1  2008/09/26 07:31:25  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.1  2008/01/01 19:51:17  jeroens
' New and/or moved
'
'==============================================================================

Imports EwECore

Namespace Controls

    Public Class ucEditFont

#Region " Constructor "

        Public Sub New(ByVal vs As cVisualStyle, ByVal style As cVisualStyle.eVisualStyleTypes)
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
