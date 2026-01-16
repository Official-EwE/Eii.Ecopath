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


Imports EwECore.ValueWrapper
Imports Microsoft.Extensions.Logging
Imports Debug = System.Diagnostics.Debug

''' <summary>
''' This class wraps the underlying particle size distribution data structures
''' </summary>
Public Class cPSDParameters
    Inherits cCoreInputOutputBase

    Private ReadOnly m_logger As ILogger = LoggingContext.CreateLogger(Of cPSDParameters)()

#Region "Constructor"

    Public Sub New(core As cCore)
        MyBase.New(core)

        Me.m_coreComponent = eCoreComponentType.Ecopath
        Me.m_dataType = eDataTypes.ParticleSizeDistribution

        Try

            Dim val As cValue

            Me.m_ValidationStatus = New cVariableStatus(Me, eStatusFlags.OK, "", eVarNameFlags.NotSet)

            'no data validation at this time
            Me.AllowValidation = False

            'PSDEnabled
            val = New cValue(core, New Boolean, eVarNameFlags.PSDEnabled, eStatusFlags.OK, eValueTypes.Bool)
            val.Stored = False
            val.AffectsRunState = False
            Me.m_values.Add(val.varName, val)

            'PSDComputed
            val = New cValue(core, New Boolean, eVarNameFlags.PSDComputed, eStatusFlags.OK, eValueTypes.Bool)
            val.Stored = False
            val.AffectsRunState = False
            Me.m_values.Add(val.varName, val)

            'PSDNumWeightClasses
            val = New cValue(core, New Integer, eVarNameFlags.PSDNumWeightClasses, eStatusFlags.Null, eValueTypes.Int)
            Me.m_values.Add(val.varName, val)

            'PSDMortalityType
            val = New cValue(core, New Integer, eVarNameFlags.PSDMortalityType, eStatusFlags.Null, eValueTypes.Int)
            Me.m_values.Add(val.varName, val)

            'PSDFirstWeightClass
            val = New cValue(core, New Single, eVarNameFlags.PSDFirstWeightClass, eStatusFlags.Null, eValueTypes.Sng)
            Me.m_values.Add(val.varName, val)

            'ClimateType
            ' To unify with Ecobase enumerated types?
            val = New cValue(core, New Integer, eVarNameFlags.ClimateType, eStatusFlags.Null, eValueTypes.Int)
            Me.m_values.Add(val.varName, val)

            'Number of points used in moving average
            val = New cValue(core, New Integer, eVarNameFlags.NumPtsMovAvg, eStatusFlags.Null, eValueTypes.Int)
            Me.m_values.Add(val.varName, val)

            ' == ARRAY VARS ==
            'PSDIncluded
            val = New cValueArray(core, eValueTypes.BoolArray, eVarNameFlags.PSDIncluded, eStatusFlags.Null, eCoreCounterTypes.nGroups)
            val.Stored = False
            Me.m_values.Add(val.varName, val)

            Me.AllowValidation = True

        Catch ex As Exception

            Debug.Assert(False, ex.Message)
            m_logger.LogError(".New() Error: " & ex.Message)

        End Try

    End Sub

#End Region

#Region "Variables via dot (.) operator"

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set whether the PSD model is enabled in EwE.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property PSDEnabled() As Boolean
        Get
            Return CBool(Me.GetVariable(eVarNameFlags.PSDEnabled))
        End Get

        Set(value As Boolean)
            Me.SetVariable(eVarNameFlags.PSDEnabled, value)
        End Set
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set whether the PSD results have been computed.
    ''' </summary>
    ''' <remarks>
    ''' This *should* have been reported by the core state monitor.
    ''' </remarks>
    ''' -----------------------------------------------------------------------
    Public Property PSDComputed() As Boolean
        Get
            Return CBool(Me.GetVariable(eVarNameFlags.PSDComputed))
        End Get
        Set(value As Boolean)
            Me.SetVariable(eVarNameFlags.PSDComputed, value)
        End Set
    End Property

    Public Property MortalityType() As ePSDMortalityTypes
        Get
            Return DirectCast(Me.GetVariable(eVarNameFlags.PSDMortalityType), ePSDMortalityTypes)
        End Get

        Set(value As ePSDMortalityTypes)
            Me.SetVariable(eVarNameFlags.PSDMortalityType, value)
        End Set
    End Property

    Public Property NumWeightClasses() As Integer
        Get
            Return CInt(Me.GetVariable(eVarNameFlags.PSDNumWeightClasses))
        End Get

        Set(value As Integer)
            Me.SetVariable(eVarNameFlags.PSDNumWeightClasses, value)
        End Set
    End Property

    Public Property FirstWeightClass() As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.PSDFirstWeightClass))
        End Get

        Set(value As Single)
            Me.SetVariable(eVarNameFlags.PSDFirstWeightClass, value)
        End Set
    End Property

    Public Property ClimateType() As eClimateTypes
        Get
            Return DirectCast(Me.GetVariable(eVarNameFlags.ClimateType), eClimateTypes)
        End Get

        Set(value As eClimateTypes)
            Me.SetVariable(eVarNameFlags.ClimateType, value)
        End Set
    End Property

    Public Property NumPtsMovAvg() As Integer
        Get
            Return CInt(Me.GetVariable(eVarNameFlags.NumPtsMovAvg))
        End Get

        Set(value As Integer)
            Me.SetVariable(eVarNameFlags.NumPtsMovAvg, value)
        End Set
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set whether a given group is included in the PSD analysis.
    ''' </summary>
    ''' <param name="iGroup">Index of the group.</param>
    ''' -----------------------------------------------------------------------
    Public Property GroupIncluded(iGroup As Integer) As Boolean
        Get
            Return CBool(Me.GetVariable(eVarNameFlags.PSDIncluded, iGroup))
        End Get

        Set(value As Boolean)
            Me.SetVariable(eVarNameFlags.PSDIncluded, value, iGroup)
        End Set
    End Property

#End Region

End Class
