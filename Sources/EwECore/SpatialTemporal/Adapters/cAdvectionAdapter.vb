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
'    UBC Institute for the Oceans and Fisheries, Vancouver BC, Canada, and 
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
    Public Class cAdvectionAdapter
        Inherits cSpatialDataAdapter


        Private m_spaceData As cEcospaceDataStructures
        Private m_iMonthIndex As Integer
        Private m_orgXData()(,) As Single
        Private m_orgYData()(,) As Single


        Public Sub New(ByVal core As cCore, ByVal varName As eVarNameFlags, ByVal cc As eCoreCounterTypes)
            MyBase.New(core, varName, cc)

        End Sub

        Friend Overrides Sub SaveLayerData()
            Try

                'saving and restoring values via the base SaveLayerData() RestoreLayerData
                'would require modifications to that code
                'this is a lot simpler
                Me.m_orgXData = New Single(11)(,) {}
                Me.m_orgYData = New Single(11)(,) {}

                For i As Integer = 0 To 11
                    Me.m_orgXData(i) = New Single(Me.m_spaceData.InRow + 1, Me.m_spaceData.InCol + 1) {}
                    Me.m_orgYData(i) = New Single(Me.m_spaceData.InRow + 1, Me.m_spaceData.InCol + 1) {}
                    For ir As Integer = 0 To Me.m_spaceData.InRow + 1
                        For ic As Integer = 0 To Me.m_spaceData.InCol + 1
                            Me.m_orgXData(i)(ir, ic) = Me.m_spaceData.MonthlyXvel(i + 1)(ir, ic)
                            Me.m_orgYData(i)(ir, ic) = Me.m_spaceData.MonthlyYvel(i + 1)(ir, ic)
                        Next
                    Next

                Next
            Catch ex As Exception
                Debug.Assert(False, ex.Message)
            End Try

        End Sub

        Friend Overrides Sub RestoreLayerData()
            Try
                For i As Integer = 0 To 11
                    For ir As Integer = 0 To Me.m_spaceData.InRow + 1
                        For ic As Integer = 0 To Me.m_spaceData.InCol + 1
                            Me.m_spaceData.MonthlyXvel(i + 1)(ir, ic) = Me.m_orgXData(i)(ir, ic)
                            Me.m_spaceData.MonthlyYvel(i + 1)(ir, ic) = Me.m_orgYData(i)(ir, ic)
                        Next
                    Next
                Next
            Catch ex As Exception
                Debug.Assert(False, ex.Message)
            End Try
        End Sub

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="cSpatialScalarDataAdapter.Initialize"/>.
        ''' -------------------------------------------------------------------
        Public Overrides Sub Initialize()

            MyBase.Initialize()
            Me.m_spaceData = Me.m_core.m_EcoSpaceData

        End Sub


        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="cSpatialDataAdapter.SetCell"/>.
        ''' <remarks>Overridden to scale values prior to being set in the 
        ''' Ecospace data structures.</remarks>
        ''' -------------------------------------------------------------------
        Protected Overrides Function SetCell(ByVal layer As cEcospaceLayer,
                                             ByVal conn As cSpatialDataConnection,
                                             ByVal iRow As Integer,
                                             ByVal iCol As Integer,
                                             ByVal sValueAtT As Double) As Boolean

            Try
                'MonthNow is the current month set Ecospace 1-12
                'Advection layer are stored by month
                layer.Cell(iRow, iCol, Me.m_spaceData.MonthNow) = sValueAtT
            Catch ex As Exception

                Dim strMsg As String = "cSpatialDataAdapter::SetCell({0}) at ({1},{2})={3}: exception {4}"
                cLog.Write(ex, cStringUtils.Localize(strMsg, layer.ToString, iCol, iRow, sValueAtT))

                Me.m_core.SpatialOperationLog.LogOperation(cStringUtils.Localize(My.Resources.CoreMessages.STATUS_SPATIALTEMPORAL_ADAPTERROR, iRow, iCol, sValueAtT, ex.Message),
                                                        eStatusFlags.MissingParameter)
                Return False
            End Try

            Return True


        End Function

    End Class

End Namespace
