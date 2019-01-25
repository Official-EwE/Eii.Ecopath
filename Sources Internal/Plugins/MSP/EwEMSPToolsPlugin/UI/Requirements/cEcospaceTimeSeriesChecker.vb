' ===============================================================================
' This file is part of Ecopath with Ecosim (EwE)
'
' EwE is free software: you can redistribute it and/or modify it under the terms
' of the GNU General Public License version 2 as published by the Free Software 
' Foundation.
'
' EwE is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; 
' without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR 
' PURPOSE. See the GNU General Public License for more details.
'
' You should have received a copy of the GNU General Public License along with EwE.
' If not, see <http://www.gnu.org/licenses/gpl-2.0.html>. 
'
' Copyright 2016- 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'

#Region " Imports "

Option Strict On
Imports EwECore
Imports EwEUtils.Core

#End Region

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
            If (msg.Source = eCoreComponentType.EcoSpace) Then Me.CheckRequirements()
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
