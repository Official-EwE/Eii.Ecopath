' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports EwECore

Namespace UI

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Helper class to validate if Ecospace is forced with spatial-temporal data. 
    ''' Any forcing patterns will need removing from Eocspace as they will be replaced
    ''' with MSP player interactions.
    ''' </summary>
    ''' <seealso cref="cRequirementChecker" />
    ''' ---------------------------------------------------------------------------
    Public Class cEcospaceTimeSeriesChecker
        Inherits cRequirementChecker

        ''' ---------------------------------------------------------------------------
        ''' <summary>
        ''' Initializes a new instance of the <see cref="cEcospaceTimeSeriesChecker"/> class.
        ''' </summary>
        ''' <param name="core">The core containing Ecosim to validate.</param>
        ''' ---------------------------------------------------------------------------
        Public Sub New(core As cCore)
            MyBase.New(core)
        End Sub

        ''' ---------------------------------------------------------------------------
        ''' <summary>
        ''' Core message handler, implemented to automatically trigger a requirement
        ''' check when the user changes Ecospace time series.
        ''' </summary>
        ''' <param name="msg">The message to respond to.</param>
        ''' ---------------------------------------------------------------------------
        Public Overrides Sub OnCoreMessage(msg As cMessage)
            If (msg.Source = eCoreComponentType.Ecospace) Then Me.CheckRequirements()
        End Sub

        ''' ---------------------------------------------------------------------------
        ''' <summary>
        ''' The requirement check for applied Ecospace external data connections. There
        ''' should be none.
        ''' </summary>
        ''' ---------------------------------------------------------------------------
        Protected Overrides Sub CheckRequirements()
            Dim man As SpatialData.cSpatialDataConnectionManager = Me.m_core.SpatialDataConnectionManager
            Me.RequirementsMet = (man.NumConnectedAdapters <= 0)
        End Sub

    End Class

End Namespace
