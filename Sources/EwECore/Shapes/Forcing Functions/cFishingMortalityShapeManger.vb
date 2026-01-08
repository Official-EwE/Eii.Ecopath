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
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'

Option Strict On

Public Class cFishingMortalityShapeManger
    Inherits cFishingBaseShapeManager

    Friend Sub New(ByRef EcoSimData As cEcosimDatastructures, ByRef theCore As cCore, DataType As eDataTypes)
        MyBase.New(EcoSimData, theCore, DataType)
    End Sub

    ''' <summary>
    ''' Fishing mortality rate shapes can not be dynamically created; they are part of the fleet setup.
    ''' </summary>
    ''' <returns>Always Nothing.</returns>
    Public Overrides Function CreateNewShape(strName As String, asData() As Single, Optional shapeType As Long = 0, Optional params() As Single = Nothing) As cForcingFunction
        Return Nothing
    End Function

    Friend Overrides Function Init() As Boolean
        Dim shape As cFishingMortShape

        'clear out any existing data
        Me.m_shapes.Clear()
        For iGroup As Integer = 1 To Me.m_SimData.nGroups ' one shape for each group 
            ' Fishing rate shapes are no longer loaded from the DB
            Me.m_SimData.FishRateNoDBID(iGroup) = Me.m_core.m_EcoSimData.GroupDBID(iGroup)

            shape = New cFishingMortShape(Me.m_SimData, Me, Me.m_SimData.FishRateNoDBID(iGroup), Me.m_core.m_EcopathData.GroupName(iGroup))
            'keep the index of this forcing function in the list in the function itself
            'it will be used later to return the list item for a given EcoSim array index
            shape.ID = Me.m_shapes.Count
            shape.Index = iGroup
            shape.Load()
            Me.m_shapes.Add(shape)

        Next iGroup
        Me.Load()

    End Function

    Public Overrides Sub ResetToDefaults()
        Me.m_SimData.DefaultFishMortalityRates()
        Me.Load()
        Me.ShapeChanged()
    End Sub

    Public Overrides Function EcopathBaseValue(iShape As Integer) As Single
        Return Me.m_core.m_EcoSimData.Fish1(iShape)
    End Function

End Class
