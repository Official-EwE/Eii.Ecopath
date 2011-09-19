#Region " Imports "

Option Strict On
Imports EwECore

#End Region ' Imports

''' ---------------------------------------------------------------------------
''' <summary>
''' Base class grid for showing <see cref="cMediationBaseFunction">mediation</see>-derived
''' functions.
''' </summary>
''' ---------------------------------------------------------------------------
Public MustInherit Class gridMediation
    Inherits gridForcingBase

    Protected Overrides Function TimeLabel(ByVal iPoint As Integer) As String
        Return CStr(iPoint + 1)
    End Function

End Class
