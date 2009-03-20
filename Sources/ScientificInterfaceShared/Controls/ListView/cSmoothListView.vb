'==============================================================================
'
' $Log: cSmoothListView.vb,v $
' Revision 1.1  2009/03/20 18:00:44  jeroens
' Initial version
'
'==============================================================================

#Region " Imports "

Option Strict On

#End Region ' Imports

Namespace Controls

    Public Class cSmoothListView
        : Inherits ListView

        Public Sub New()
            MyBase.New()
            Me.SetStyle(ControlStyles.OptimizedDoubleBuffer, True)
        End Sub

    End Class

End Namespace ' Controls
