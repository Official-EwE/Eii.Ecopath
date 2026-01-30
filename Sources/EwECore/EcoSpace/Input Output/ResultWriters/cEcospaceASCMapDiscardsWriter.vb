' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports EwECore.Common




''' ---------------------------------------------------------------------------
''' <summary>
''' Implementation of <see cref="IEcospaceResultsWriter">IEcospaceResultsWriter</see> 
''' and <see cref="cEcospaceBaseResultsWriter">cEcospaceBaseResultsWriter</see> 
''' to write Ecospace output to ESRI ASCII files. 
''' </summary>
''' <remarks>Each ASCII file will contain an Ecospace value for a given group and time step</remarks>
''' ---------------------------------------------------------------------------
Public Class cEcospaceASCMapDiscardsWriter
    Inherits cEcospaceASCBaseResultsWriter

    Public Sub New()
        MyBase.New()

        Me.vars = New eVarNameFlags() {eVarNameFlags.EcospaceMapDiscards}
    End Sub

    Public Overrides Sub Init(theCore As Object)
        MyBase.Init(theCore)
        Me.SetCatchSelected()
    End Sub

    Public Overrides ReadOnly Property DisplayName As String
        Get
            Return My.Resources.CoreDefaults.ECOSPACE_WRITER_ASC_DISCARDS
        End Get
    End Property

End Class
