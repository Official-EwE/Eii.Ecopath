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
' Copyright 1991-2012 UBC Fisheries Centre, Vancouver BC, Canada.
' ===============================================================================
'
#Region " Imports "

Option Strict On
Imports EwEUtils.Core
Imports EwEUtils.SpatialData
Imports EwEUtils.Utilities

#End Region ' Imports

Namespace SpatialData

    ''' <summary>
    ''' Base spatial data adapter to insert external spatial/temporal map data into
    ''' the Ecospace data structures at any given moment.
    ''' </summary>
    Public Class cSpatialScalarDataAdapter
        Inherits cSpatialDataAdapter

        Public Enum eScaleType As Byte
            Absolute = 0
            Relative
        End Enum

        Private m_scales() As Single
        Private m_scaleType() As eScaleType
        Private m_spaceData As cEcospaceDataStructures
#Region " Constructor "

        Public Sub New(ByVal core As cCore, ByVal varName As eVarNameFlags, ByVal cc As eCoreCounterTypes)
            MyBase.New(core, varName, cc)
        End Sub

#End Region ' Constructor

#Region " Public access "

        Public Property DataScaleType(iIndex As Integer) As eScaleType
            Get
                Debug.Assert(iIndex < Me.Length, "Index out of range")
                Return Me.m_scaleType(Math.Max(0, iIndex))
            End Get
            Set(value As eScaleType)
                Debug.Assert(iIndex < Me.Length, "Index out of range")
                Me.m_scaleType(Math.Max(0, iIndex)) = value
            End Set
        End Property

        Public Property DataScale(iIndex As Integer) As Single
            Get
                Debug.Assert(iIndex < Me.Length, "Index out of range")
                Return Me.m_scales(Math.Max(0, iIndex))
            End Get
            Set(value As Single)
                Debug.Assert(iIndex < Me.Length, "Index out of range")
                Me.m_scales(Math.Max(0, iIndex)) = value
            End Set
        End Property

#End Region ' Public access

#Region " Overrides "

        Friend Overrides Sub Initialize()

            MyBase.Initialize()

            Dim iNumItems As Integer = Math.Max(0, Me.m_core.GetCoreCounter(Me.m_coreCounter))

            ReDim Me.m_scales(iNumItems)
            ReDim Me.m_scaleType(iNumItems)

            For i As Integer = 0 To iNumItems - 1
                Me.m_scales(i) = 1.0!
                Me.m_scaleType(i) = eScaleType.Relative
            Next

            Me.m_spaceData = Me.m_core.m_EcoSpaceData

        End Sub

        Protected Overrides Sub InitRun()
            MyBase.InitRun()
            
            If Me.VarName = eVarNameFlags.LayerRelPP Then
                'At the start of each Ecospace run it calculates the relative PP scaler as the mean PP (cEcospaceDataStructures.PPScale)
                'From the current relPP(,) map/array see ScaleRelativePrimaryProductivityToEcopathLevel() 
                'Then uses PPScale to scale the values in relPP(row,col) as they are passed into derivtRed()
                'This is unavoidable
                'We need to set PPScale to have no effect, then scale the spatial temporal data as it passes through the adapter 
                'Alternativley 
                'We could set PPScale to the scale set for the adapter and just let the data pass through
                Me.m_spaceData.PPScale = 1
            End If
        End Sub

        Protected Overrides Function SetCell(ByVal layer As cEcospaceLayer, ByVal iRow As Integer, ByVal iCol As Integer, ByVal ValueAtT As Single) As Boolean
            If (Me.m_scaleType(layer.Index) = eScaleType.Relative) Then
                ValueAtT *= Me.DataScale(layer.Index)
            End If
            Return MyBase.SetCell(layer, iRow, iCol, ValueAtT)
        End Function

#End Region

    End Class

End Namespace
