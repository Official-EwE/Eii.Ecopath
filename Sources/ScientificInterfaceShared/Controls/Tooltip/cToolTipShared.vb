#Region " Imports "

Option Strict On

#End Region ' Imports

Namespace Controls

    ''' =======================================================================
    ''' <summary>
    ''' Public accessible but shared tooltip instance for homogenous application
    ''' behaviour and styling. Yeah.
    ''' </summary>
    ''' =======================================================================
    Public Class cToolTipShared
        Inherits ToolTip

#Region " Privates "

        ''' <summary>Singleton instance.</summary>
        Private Shared __inst__ As cToolTipShared

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Singleton enforced constructor
        ''' </summary>
        ''' -------------------------------------------------------------------
        Private Sub New()
            ' Yoho
        End Sub

#End Region ' Privates

#Region " Public interfaces "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Zhe van einzterfeiz to get zhe tuhltipp.
        ''' </summary>
        ''' <returns>Zhe tuhltipp inschtanz.</returns>
        ''' -------------------------------------------------------------------
        Public Shared Function GetInstance() As cToolTipShared
            If Object.ReferenceEquals(cToolTipShared.__inst__, Nothing) Then
                cToolTipShared.__inst__ = New cToolTipShared
                cToolTipShared.__inst__.Active = True
            End If
            Return cToolTipShared.__inst__
        End Function

#End Region ' Public interfaces

    End Class

End Namespace ' Controls
