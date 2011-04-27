#Region " Imports "

Option Strict On

#End Region ' Imports

Namespace Controls

    ''' <summary>
    ''' <see cref="ListView"/>-derived class for rendering without flickering.
    ''' </summary>
    Public Class cSmoothListView
        : Inherits ListView

        ''' <summary>
        ''' Constructor.
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub New()
            MyBase.New()
            Me.SetStyle(ControlStyles.OptimizedDoubleBuffer Or ControlStyles.AllPaintingInWmPaint Or ControlStyles.UserPaint, True)
        End Sub

    End Class

End Namespace ' Controls
