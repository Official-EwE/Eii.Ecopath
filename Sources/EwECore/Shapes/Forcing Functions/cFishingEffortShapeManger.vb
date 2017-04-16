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
' Copyright 1991- 
'    UBC Fisheries Centre, Vancouver BC, Canada, and 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'

Option Strict On
Imports EwEUtils.Core

Public Class cFishingEffortShapeManger
    Inherits cFishingBaseShapeManager

    Friend Sub New(ByRef EcoSimData As cEcosimDatastructures, ByRef theCore As cCore, DataType As eDataTypes)
        MyBase.New(EcoSimData, theCore, DataType)
    End Sub

    ''' <summary>
    ''' Fishing rate shapes can not be dynamically created; they are part of the fleet setup.
    ''' </summary>
    ''' <returns>Always Nothing.</returns>
    Public Overrides Function CreateNewShape(strName As String, asData() As Single,
                                             Optional shapeType As Long = eShapeFunctionType.NotSet,
                                             Optional params As Single() = Nothing) As cForcingFunction
        Return Nothing
    End Function

    Friend Overrides Function Init() As Boolean
        Dim shape As cFishingRateShape = Nothing
        Dim iFleet As Integer

        'clear out any existing data
        m_shapes.Clear()
        For iFleet = 1 To m_SimData.nGear ' number of fishing fleets

            shape = New cFishingRateShape(m_SimData, Me, m_SimData.FishRateGearDBID(iFleet), m_core.m_EcoPathData.FleetName(iFleet))
            'keep the index of this forcing function in the list in the function itself
            'it will be used later to return the list item for a given EcoSim array index
            shape.ID = m_shapes.Count
            shape.Index = iFleet
            shape.Load()
            m_shapes.Add(shape)

        Next iFleet

        If m_SimData.nGear > 0 Then
            'Add the Combined Gear types shape to the end of the list
            'Its iFleet index is m_Data.nGear + 1 
            'this is critical as that is how the shape itself decides that it the Combined Fleets shape
            'the Combined Fleets shape updates all the other fleets as well as the FishMort shapes
            shape = New cFishingRateShape(m_SimData, Me, cCore.NULL_VALUE, My.Resources.CoreDefaults.CORE_ALL_FLEETS)
            shape.ID = m_shapes.Count
            shape.Index = m_SimData.nGear + 1
            shape.Load()
            m_shapes.Add(shape)
        End If

        Me.Load()

    End Function

    Public Overrides Sub ResetToDefaults()
        Me.m_SimData.DefaultFishingRates()
        Me.Load()
        Me.ShapeChanged()
    End Sub

    Public Overrides Function EcopathBaseValue(iShape As Integer) As Single
        Return 1
    End Function

End Class

