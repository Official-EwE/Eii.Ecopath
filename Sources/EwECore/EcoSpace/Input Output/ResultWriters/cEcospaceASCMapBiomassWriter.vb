' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports EwECore.Common

''' ---------------------------------------------------------------------------
''' <summary>
''' Implementation of <see cref="IEcospaceResultsWriter"/> to write Ecospace biomass
''' distributions maps to ESRI ASCII files. 
''' </summary>
''' ---------------------------------------------------------------------------
Public Class cEcospaceASCMapBiomassWriter
    Inherits cEcospaceASCBaseResultsWriter

    Public Sub New()
        MyBase.New()
        Me.vars = New eVarNameFlags() {eVarNameFlags.EcospaceMapBiomass}
    End Sub

    Public Overrides Sub Init(theCore As Object)
        MyBase.Init(theCore)
    End Sub

    Public Overrides ReadOnly Property DisplayName As String
        Get
            Return My.Resources.CoreDefaults.ECOSPACE_WRITER_ASC_BIOMASS
        End Get
    End Property

End Class