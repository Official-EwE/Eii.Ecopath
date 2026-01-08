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
''' The definition of a Marine Protected Area in Ecospace.
''' </summary>
''' <seealso cref="EwECore.cCoreInputOutputBase" />
''' <seealso cref="EwECore.cEcospaceBasemap.LayerMPA(Integer)"/>
Public Class cEcospaceMPA
    Inherits cCoreInputOutputBase

    Private ReadOnly m_logger As ILogger = LoggingContext.CreateLogger(Of cEcospaceMPA)()

#Region "Constructor"

    Sub New(core As cCore, iDBID As Integer)
        MyBase.New(core)

        Dim val As cValue = Nothing

        Try

            Me.m_dataType = eDataTypes.EcospaceMPA
            Me.m_coreComponent = eCoreComponentType.Ecospace
            Me.DBID = iDBID

            Me.m_ValidationStatus = New cVariableStatus(Me, eStatusFlags.OK, "", eVarNameFlags.NotSet)

            Me.ResetStatusFlags()

            ' MPAMonth
            val = New cValueArray(core, eValueTypes.BoolArray, eVarNameFlags.MPAMonth, eStatusFlags.OK, eCoreCounterTypes.nMonths)
            Me.m_values.Add(val.varName, val)

        Catch ex As Exception
            Debug.Assert(False, "Error creating new cEcospaceMPA.")
            m_logger.LogError(ex, Me.ToString & ".New(nGroups) Error creating new cEcospaceMPA. Error: " & ex.Message)
        End Try

    End Sub

#End Region

#Region " Variables by dot '.' operator "

    ''' <summary>
    ''' Get/set if an MPA is OPEN for fishing for a given month.
    ''' </summary>
    ''' <param name="iMonth">The one-based month index to access the 
    ''' MPA open state for.</param>
    Public Property MPAMonth(iMonth As Integer) As Boolean
        Get
            Return CBool(Me.GetVariable(eVarNameFlags.MPAMonth, iMonth))
        End Get

        Set(value As Boolean)
            Me.SetVariable(eVarNameFlags.MPAMonth, value, iMonth)
        End Set
    End Property

#End Region ' Variables by dot '.' operator

#Region " Status by dot (.) operator "

    Public Property MPAMonthStatus(iMonth As Integer) As eStatusFlags
        Get
            Return Me.GetStatus(eVarNameFlags.MPAMonth, iMonth)
        End Get

        Friend Set(value As eStatusFlags)
            Me.SetStatus(eVarNameFlags.MPAMonth, value, iMonth)
        End Set
    End Property

#End Region ' Status by dot (.) operator

#Region " Quick accessors "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get the number of cells in a MPA.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property NumCells() As Integer
        Get
            Dim bm As cEcospaceBasemap = Me.m_core.EcospaceBasemap
            Dim l As cEcospaceLayerMPA = bm.LayerMPA(Me.Index)
            Return l.NumValueCells
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Returns whether the MPA is actually imposing fishing limitations on any fleet.
    ''' </summary>
    ''' <param name="iMonth">The month to check the MPA status for, or -1 if the assesment is to be made for an entire year.</param>
    ''' <returns>True if the MPA is actually imposing fishing limitations on any fleet.</returns>
    ''' <seealso cref="cEcospaceFleet.MPAFishery(Integer)"/>
    ''' <seealso cref="cEcospaceMPA.IsOpen(Integer)"/>
    ''' <seealso cref="cEcospaceMPA.IsClosed(Integer)"/>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property IsActive(Optional iMonth As Integer = -1) As Boolean
        Get
            Dim bIsClosed As Boolean = False
            Dim bIsApplied As Boolean = False
            Dim iMin As Integer = If(iMonth < 1 Or iMonth > cCore.N_MONTHS, 1, iMonth)
            Dim iMax As Integer = If(iMonth < 1 Or iMonth > cCore.N_MONTHS, 1, iMonth)
            For i As Integer = iMin To iMax : bIsClosed = bIsClosed Or Me.IsClosed(i) : Next
            For i As Integer = 1 To Me.m_core.nFleets : bIsApplied = bIsApplied Or (Me.m_core.EcospaceFleetInputs(i).MPAFishery(Me.Index) = False) : Next
            Return bIsClosed And bIsApplied
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set whether this MPA is open to fishing at a given month.
    ''' </summary>
    ''' <param name="iMonth">The month to check.</param>
    ''' <returns>True if the MPA is not enforced, and fishing is allowed.</returns>
    ''' <seealso cref="cEcospaceFleet.MPAFishery(Integer)"/>
    ''' <seealso cref="cEcospaceMPA.IsActive(Integer)"/>
    ''' <seealso cref="cEcospaceMPA.IsClosed(Integer)"/>
    ''' -----------------------------------------------------------------------
    Public Property IsOpen(iMonth As Integer) As Boolean
        Get
            Return Me.MPAMonth(iMonth) = True
        End Get
        Set(value As Boolean)
            Me.MPAMonth(iMonth) = (value = True)
        End Set
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set whether this MPA is enforced, and imposes fishing limitations at a given month.
    ''' </summary>
    ''' <param name="iMonth">The month to check.</param>
    ''' <returns>True if the MPA is enforced, and some or all fishing is now allowed.</returns>
    ''' <seealso cref="cEcospaceFleet.MPAFishery(Integer)"/>
    ''' <seealso cref="cEcospaceMPA.IsOpen(Integer)"/>
    ''' <seealso cref="cEcospaceMPA.IsActive(Integer)"/>
    ''' -----------------------------------------------------------------------
    Public Property IsClosed(iMonth As Integer) As Boolean
        Get
            Return Me.MPAMonth(iMonth) = False
        End Get
        Set(value As Boolean)
            Me.MPAMonth(iMonth) = (value = False)
        End Set
    End Property

#End Region ' Quick accessors

End Class
