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

#Region " Imports "

Option Strict On
Imports EwEUtils.Core
Imports EwEUtils.SpatialData
Imports EwEUtils.Utilities

#End Region ' Imports

Namespace SpatialData

    ''' <summary>
    ''' Adapter to populate the Advection monthly maps
    ''' </summary>
    ''' <remarks>
    ''' A scalar is needed to perform unit conversions, direction swapping, etc
    ''' </remarks>
    Public Class cAdvectionAdapter
        Inherits cSpatialScalarDataAdapter

        Private m_spaceData As cEcospaceDataStructures

        Public Sub New(core As cCore, varName As eVarNameFlags, cc As eCoreCounterTypes)
            MyBase.New(core, varName, cc)
        End Sub

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="cSpatialScalarDataAdapter.Initialize"/>.
        ''' -------------------------------------------------------------------
        Public Overrides Sub Initialize()

            MyBase.Initialize()
            Me.m_spaceData = Me.m_core.m_EcospaceData

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' States that this adapter cannot scale to a base value, as there is
        ''' no base velocity to scale to.
        ''' </summary>
        ''' <returns></returns>
        ''' <seealso cref="CalculateScalar(Double, Double)" />
        ''' -------------------------------------------------------------------
        Public Overrides Function CanCalculateScalar() As Boolean
            Return False
        End Function

    End Class

End Namespace
