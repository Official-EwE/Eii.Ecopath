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

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Data Adapter for forcing IBM numbers at first age
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Class cIBMAge1NumbersForcingAdapter
        Inherits cSpatialScalarDataAdapterBase

#Region " Private vars "




#End Region ' Private vars

#Region " Constructor "

        Public Sub New(ByVal core As cCore, ByVal varName As eVarNameFlags, ByVal cc As eCoreCounterTypes)
            MyBase.New(core, varName, cc)
        End Sub

#End Region ' Constructor

#Region " Overrides "

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

            If (conn.ScaleType = eScaleType.Relative) Then
                If sValueAtT <> cCore.NULL_VALUE Then

                    Dim iSt As Integer = layer.Index
                    m_core.m_Stanza.isForcedIBMRecruits(iSt) = True

                    'Cells outside the modeled area can/will be -9999
                    sValueAtT *= conn.Scale
                    Return ForceIBM(layer, conn, iRow, iCol, sValueAtT)
                    'Return MyBase.SetCell(layer, conn, iRow, iCol, sValueAtT)
                End If
            End If

            Return True

        End Function


        Private Function ForceIBM(ByVal layer As cEcospaceLayer,
                                             ByVal conn As cSpatialDataConnection,
                                             ByVal iRow As Integer,
                                             ByVal iCol As Integer,
                                             ByVal sValueAtT As Double) As Boolean
            Try

                'The layer index should be for this stanza group
                'That was setup during initialization
                Dim iSt As Integer = layer.Index
                'save the full map 
                'the IBM will figure out which packet and lifestage to put it in 
                m_core.m_Stanza.IBMForcedCells(iSt)(iRow, iCol) = CSng(sValueAtT)

            Catch ex As Exception

                Dim strMsg As String = "cSpatialDataAdapter::SetCell({0}) at ({1},{2})={3}: exception {4}"
                cLog.Write(ex, cStringUtils.Localize(strMsg, layer.ToString, iCol, iRow, sValueAtT))

                Me.m_core.SpatialOperationLog.LogOperation(cStringUtils.Localize(My.Resources.CoreMessages.STATUS_SPATIALTEMPORAL_ADAPTERROR, iRow, iCol, sValueAtT, ex.Message),
                                                           eStatusFlags.MissingParameter)
                Return False


            End Try

            Return True

        End Function


        Public Overrides Function CalculateScalar(SumOverPeriod As Double, nMapCells As Double) As Double
            Try
                'RzeroS = Number of recruits at the ecopath base in one month
                'ThabArea = total habitat area
                'SumOverPeriod = total number of recruits over the first year
                Return m_core.m_Stanza.RzeroS(iScaleLayerIndex) * m_core.m_EcoSpaceData.ThabArea * 12 / SumOverPeriod

                'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
                'NO SCALER
                'For the Age 1 forcing just return 1 
                'Return 1.0
                'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
            Catch ex As Exception
                cLog.Write(ex, "Failed to calculate map scale value")
            End Try
            Return 1.0
        End Function

#End Region ' Overrides

        Public Overrides Function RestoreForcing(SpaceData As cEcospaceDataStructures) As Boolean
            Try
                If m_core.m_Stanza.IBMForcedCells Is Nothing Then Return True
                For iSt As Integer = 1 To Me.m_core.m_Stanza.Nsplit
                    'm_core.m_Stanza.IBMForcedCells(iSt)
                    System.Array.Clear(m_core.m_Stanza.IBMForcedCells(iSt), 0, m_core.m_Stanza.IBMForcedCells(iSt).Length)
                    m_core.m_Stanza.isForcedIBMRecruits(iSt) = False
                Next

            Catch ex As Exception
                Return False
            End Try
            Return True
        End Function


        Friend Overrides Sub SaveLayerData()
            'System.Console.WriteLine(Me.ToString + ".SaveLayerData()")
        End Sub

        Friend Overrides Sub RestoreLayerData()
            'System.Console.WriteLine(Me.ToString + ".RestoreLayerData()")
        End Sub

    End Class

End Namespace
