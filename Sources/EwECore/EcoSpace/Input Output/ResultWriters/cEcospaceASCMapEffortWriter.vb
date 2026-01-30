' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports EwECore.Common



''' ---------------------------------------------------------------------------
''' <summary>
''' Implementation of <see cref="IEcospaceResultsWriter"/> to write Ecospace effort
''' distributions maps to ESRI ASCII files. 
''' </summary>
''' ---------------------------------------------------------------------------
Public Class cEcospaceASCMapEffortWriter
    Inherits cEcospaceASCBaseResultsWriter

    Public Sub New()
        MyBase.New()
        Me.vars = New eVarNameFlags() {eVarNameFlags.EcospaceMapEffort}
    End Sub

    Public Overrides Sub Init(theCore As Object)
        MyBase.Init(theCore)
    End Sub

    Protected Overrides Function FirstMap() As Integer
        Return 1
    End Function

    Protected Overrides Function LastMap() As Integer
        Return Me.EcopathData.NumFleet
    End Function

    Protected Overrides Function IsItemSelected(iIndex As Integer) As Boolean
        Return Me.SelectedFleets(iIndex)
    End Function

    Public Overrides ReadOnly Property DisplayName As String
        Get
            Return My.Resources.CoreDefaults.ECOSPACE_WRITER_ASC_EFFORT
        End Get
    End Property

    Protected Overrides Function GetFileName(varname As eVarNameFlags,
                                             iIndex As Integer,
                                             strExt As String,
                                             Optional iModelTimeStep As Integer = cCore.NULL_VALUE) As String
        Return MyBase.GetFleetFileName(varname, iIndex, strExt, iModelTimeStep)
    End Function

End Class


