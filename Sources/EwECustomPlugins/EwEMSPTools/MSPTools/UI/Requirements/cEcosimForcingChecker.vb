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
    ''' Helper class to validate if Ecosim contains conflicting forcing patterns among
    ''' applied forcing functions. These patterns will need removing from Ecosim, as 
    ''' temporal forcing dynamics will be replaced by MSP player actions.
    ''' </summary>
    ''' <seealso cref="cRequirementChecker" />
    ''' <remarks>
    ''' This class still needs to consider applied environmental forcing functions.
    ''' </remarks>
    ''' ---------------------------------------------------------------------------
    Public Class cEcosimForcingChecker
        Inherits cRequirementChecker

        ''' ---------------------------------------------------------------------------
        ''' <summary>
        ''' Initializes a new instance of the <see cref="cEcosimForcingChecker"/> class.
        ''' </summary>
        ''' <param name="core">The core containing Ecosim to validate.</param>
        ''' ---------------------------------------------------------------------------
        Public Sub New(core As cCore)
            MyBase.New(core)
        End Sub

        ''' ---------------------------------------------------------------------------
        ''' <summary>
        ''' Core message handler, implemented to automatically trigger a requirement
        ''' check when the user changes Ecosim forcing functions.
        ''' </summary>
        ''' <param name="msg">The message to respond to.</param>
        ''' ---------------------------------------------------------------------------
        Public Overrides Sub OnCoreMessage(msg As cMessage)
            If (msg.Source = eCoreComponentType.ShapesManager) Then Me.CheckRequirements()
        End Sub

        ''' ---------------------------------------------------------------------------
        ''' <summary>
        ''' The requirement check for existing forcing patterns among applied forcing 
        ''' functions.
        ''' </summary>
        ''' ---------------------------------------------------------------------------
        Protected Overrides Sub CheckRequirements()

            Dim parms As cEcoSimModelParameters = Me.m_core.EcoSimModelParameters
            Dim interactions As cMediatedInteractionManager = Me.m_core.MediatedInteractionManager
            Dim shapes As cForcingFunctionShapeManager = Me.m_core.ForcingShapeManager
            Dim bFlat As Boolean = True
            Dim bMustCheck As Boolean = False

            For Each shape As cForcingFunction In Me.m_core.ForcingShapeManager
                For j As Integer = 1 To Me.m_core.nGroups
                    For k As Integer = 1 To Me.m_core.nGroups
                        If interactions.isPredPrey(j, k) Then
                            Dim ppi As cPredPreyInteraction = interactions.PredPreyInteraction(j, k)
                            For l As Integer = 1 To ppi.MaxNumShapes
                                Dim shapeTest As cForcingFunction = Nothing
                                Dim appl As eForcingFunctionApplication
                                If ppi.getShape(l, shapeTest, appl) Then
                                    If ReferenceEquals(shape, shapeTest) Then
                                        bMustCheck = True
                                    End If
                                End If
                            Next l
                        End If
                    Next k
                Next j

                If (shape.Index = parms.NutForceFunctionNumber) Then
                    bMustCheck = True
                End If

                ' ToDo: evaluate applied env forcing functions

                If (bMustCheck) Then
                    bFlat = bFlat And Me.IsFlat(shape)
                End If

            Next shape

            Me.RequirementsMet = bFlat

        End Sub

        ''' ---------------------------------------------------------------------------
        ''' <summary>
        ''' Determines whether the specified <see cref="cBaseShapeManager">shape manager
        ''' </see> contains only flat shapes, e.g., shapes with constant values of 1.
        ''' </summary>
        ''' <param name="shape">The forcing function to check.</param>
        ''' <returns>
        '''   <c>true</c> if the specified manager is flat; otherwise, <c>false</c>.
        ''' </returns>
        ''' ---------------------------------------------------------------------------
        Private Function IsFlat(shape As cForcingFunction) As Boolean

            Dim bFlat As Boolean = True
            Dim i As Integer = 0
            Dim d As Single() = shape.ShapeData
            Dim s As Single = 1
            While i < shape.nPoints And bFlat
                bFlat = (d(i) = s) : i += 1
            End While
            Return bFlat

        End Function

    End Class

End Namespace
