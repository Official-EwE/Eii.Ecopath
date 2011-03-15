Namespace Controls

    Public Class ucSmoothPanel
        Inherits Panel

        Public Sub New()
            MyBase.New()
            Me.SetStyle(ControlStyles.AllPaintingInWmPaint Or _
                        ControlStyles.OptimizedDoubleBuffer Or _
                        ControlStyles.UserPaint, True)
        End Sub

    End Class

End Namespace ' Controls
