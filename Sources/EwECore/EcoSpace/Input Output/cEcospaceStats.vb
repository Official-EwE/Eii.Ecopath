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
Imports EwECore.ValueWrapper
Imports Microsoft.Extensions.Logging
Imports Debug = System.Diagnostics.Debug

''' <summary>
''' Statistics for the last Ecospace run.
''' </summary>
''' <remarks>One object for all the groups and stats</remarks>
Public Class cEcospaceStats
    Inherits cCoreInputOutputBase

    Private ReadOnly m_logger As ILogger = LoggingContext.CreateLogger(Of cEcospaceStats)()

    Sub New(core As cCore, DBID As Integer)
        MyBase.New(core)

        Me.DBID = DBID
        Me.m_dataType = eDataTypes.EcospaceStatistics
        Me.m_coreComponent = eCoreComponentType.Ecospace

        Dim val As cValue

        Try

            Me.m_ValidationStatus = New cVariableStatus(Me, eStatusFlags.OK, "", eVarNameFlags.NotSet)

            'SS
            val = New cValue(core, New Single, eVarNameFlags.EcospaceSS, eStatusFlags.NotEditable, eValueTypes.Sng)
            Me.m_values.Add(val.varName, val)


            val = New cValueArray(core, eValueTypes.SingleArray, eVarNameFlags.EcospaceSSGroup, eStatusFlags.NotEditable, eCoreCounterTypes.nGroups)
            Me.m_values.Add(val.varName, val)

            val = New cValue(core, New Boolean, eVarNameFlags.EcospaceSSCalculated, eStatusFlags.NotEditable, eValueTypes.Bool)
            Me.m_values.Add(val.varName, val)

            ''Region SS
            'val = New cValueArray(core, eValueTypes.SingleArray, eVarNameFlags.EcospaceRegionSS, eStatusFlags.NotEditable, eCoreCounterTypes.nRegions, _
            '             AddressOf m_core.GetCoreCounter)
            'm_values.Add(val.varName, val)

            'set status flags to their default values
            Me.ResetStatusFlags()

        Catch ex As Exception
            Debug.Assert(False, "Error creating new cEcospaceStats.")
            m_logger.LogError(ex, Me.ToString & ".New(nGroups) Error creating new cEcospaceStats. Error: " & ex.Message)
        End Try

    End Sub

    Friend Overrides Function ResetStatusFlags(Optional bForceReset As Boolean = False) As Boolean
        Dim i As Integer

        'tell the base class to do the default values
        MyBase.ResetStatusFlags(bForceReset)

        Dim keyvalue As KeyValuePair(Of eVarNameFlags, cValue)
        Dim value As cValue
        For Each keyvalue In Me.m_values
            Try
                value = keyvalue.Value

                Select Case value.varType
                    Case eValueTypes.SingleArray, eValueTypes.IntArray, eValueTypes.BoolArray
                        For i = 0 To value.Length
                            value.Status(i) = eStatusFlags.NotEditable Or eStatusFlags.ValueComputed
                        Next i

                    Case eValueTypes.Sng, eValueTypes.Int
                        value.Status = eStatusFlags.NotEditable Or eStatusFlags.ValueComputed

                End Select
            Catch ex As Exception
                Debug.Assert(False, ex.Message)
                Return False
            End Try
        Next keyvalue
        Return True

    End Function

    ''' <summary>
    ''' SS sumed across all groups and variables
    ''' </summary>
    ''' <returns>sumof(log(observed/predicted))</returns>
    Public Property SS() As Double
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.EcospaceSS))
        End Get
        Set(value As Double)
            Me.SetVariable(eVarNameFlags.EcospaceSS, value)
        End Set
    End Property



    Public Property SSStatus() As eStatusFlags
        Get
            Return Me.GetStatus(eVarNameFlags.EcospaceSS)
        End Get
        Set(value As eStatusFlags)
            Me.SetStatus(eVarNameFlags.EcospaceSS, value)
        End Set
    End Property


    ''' <summary>
    ''' SS by group
    ''' </summary>
    ''' <param name="iGrp"></param>
    ''' <returns>sumof(log(observed(igroup)/predicted(igroup))</returns>
    Public Property SSGroup(iGrp As Integer) As Double
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.EcospaceSSGroup, iGrp))
        End Get
        Set(value As Double)
            Me.SetVariable(eVarNameFlags.EcospaceSSGroup, value, iGrp)
        End Set
    End Property



    Public Property SSGroupStatus() As eStatusFlags
        Get
            Return Me.GetStatus(eVarNameFlags.EcospaceSSGroup)
        End Get
        Set(value As eStatusFlags)
            Me.SetStatus(eVarNameFlags.EcospaceSSGroup, value)
        End Set
    End Property

    Public Property isSSCalculated As Boolean
        Get
            Return CBool(Me.GetVariable(eVarNameFlags.EcospaceSSCalculated))
        End Get

        Set(value As Boolean)
            Me.SetVariable(eVarNameFlags.EcospaceSSCalculated, value)
        End Set
    End Property

    Public ReadOnly Property isSSCalculatedStatus As eStatusFlags
        Get
            Return Me.GetStatus(eVarNameFlags.EcospaceSSCalculated)
        End Get
    End Property

    'EcospaceSSCalculated

#Region "No longer implemented"

#If DEADCODE Then
    
    Public Property RegionSS(iRegion As Integer) As Single
        Get
            Return CSng(GetVariable(eVarNameFlags.EcospaceRegionSS, iRegion))
        End Get
        Set(value As Single)
            SetVariable(eVarNameFlags.EcospaceRegionSS, value, iRegion)
        End Set
    End Property



    Public Property RegionSSStatus(iRegion As Integer) As eStatusFlags
        Get
            Return GetStatus(eVarNameFlags.EcospaceRegionSS, iRegion)
        End Get
        Set(value As eStatusFlags)
            SetStatus(eVarNameFlags.EcospaceRegionSS, value, iRegion)
        End Set
    End Property
        
#End If

#End Region



End Class
