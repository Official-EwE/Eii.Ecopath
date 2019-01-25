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

#End Region ' Imports

Namespace UI

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Helper class to validate if Ecosim contains conflicting fishing patterns. These
    ''' patterns will need removing from Ecosim, as fisheries dynamics will be replaced by
    ''' MSP player actions.
    ''' </summary>
    ''' <seealso cref="cRequirementChecker" />
    ''' ---------------------------------------------------------------------------
    Public Class cEcosimFishingChecker
        Inherits cRequirementChecker

        ''' ---------------------------------------------------------------------------
        ''' <summary>
        ''' Initializes a new instance of the <see cref="cEcosimFishingChecker"/> class.
        ''' </summary>
        ''' <param name="core">The core containing Ecosim to validate.</param>
        ''' ---------------------------------------------------------------------------
        Public Sub New(core As cCore)
            MyBase.New(core)
        End Sub

        ''' ---------------------------------------------------------------------------
        ''' <summary>
        ''' Core message handler, implemented to automatically trigger a requirement
        ''' check when the user changes Ecosim fishing dynamics.
        ''' </summary>
        ''' <param name="msg">The message to respond to.</param>
        ''' ---------------------------------------------------------------------------
        Public Overrides Sub OnCoreMessage(msg As cMessage)
            If (msg.Source = eCoreComponentType.ShapesManager) Then Me.CheckRequirements()
        End Sub

        ''' ---------------------------------------------------------------------------
        ''' <summary>
        ''' The requirement check for conflicting Ecosim fishing patterns, both for
        ''' fishing effort and fishing mortality.
        ''' </summary>
        ''' ---------------------------------------------------------------------------
        Protected Overrides Sub CheckRequirements()
            Me.RequirementsMet = IsFlat(Me.m_core.FishingEffortShapeManager) And IsFlat(Me.m_core.FishMortShapeManager)
        End Sub

        ''' ---------------------------------------------------------------------------
        ''' <summary>
        ''' Determines whether the specified <see cref="cBaseShapeManager">shape manager
        ''' </see> contains only flat shapes, e.g., shapes with constant values.
        ''' </summary>
        ''' <param name="man">The manager to check.</param>
        ''' <returns>
        '''   <c>true</c> if the specified manager is flat; otherwise, <c>false</c>.
        ''' </returns>
        ''' ---------------------------------------------------------------------------
        Private Function IsFlat(man As cBaseShapeManager) As Boolean

            Dim bFlat As Boolean = True
            For Each shp As cForcingFunction In man
                Dim i As Integer = 1
                Dim d As Single() = shp.ShapeData
                Dim s As Single = d(1)
                While i < shp.nPoints And bFlat
                    bFlat = (d(i) = s) : i += 1
                End While
            Next
            Return bFlat

        End Function

    End Class

End Namespace
