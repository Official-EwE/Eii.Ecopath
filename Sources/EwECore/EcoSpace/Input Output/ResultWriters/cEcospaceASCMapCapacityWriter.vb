' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports EwECore.Common



''' ---------------------------------------------------------------------------
''' <summary>
''' Implementation of <see cref="IEcospaceResultsWriter"/> to write Ecospace 
''' computed habitat capacity maps to ESRI ASCII files. 
''' </summary>
''' ---------------------------------------------------------------------------
Public Class cEcospaceASCMapCapacityWriter
    Inherits cEcospaceASCBaseResultsWriter

    Public Sub New()
        MyBase.New()
        Me.vars = New eVarNameFlags() {eVarNameFlags.LayerHabitatCapacity}
    End Sub

    Public Overrides Sub Init(theCore As Object)
        MyBase.Init(theCore)
        Me.SetAllGroupsSelected()
    End Sub

    Public Overrides ReadOnly Property DisplayName As String
        Get
            Return My.Resources.CoreDefaults.ECOSAPCE_WRITER_ASC_COMPUTEDCAPACITY
        End Get
    End Property

End Class