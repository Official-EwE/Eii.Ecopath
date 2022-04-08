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
Imports EwECore.ValueWrapper
Imports EwEUtils.Core

#End Region ' Imports

Public Class cEcosimFleetInput
    Inherits cCoreInputOutputBase

    Public Sub New(core As cCore, iFleet As Integer)
        MyBase.New(core)

        Dim val As cValue
        Dim simdata As cEcosimDatastructures = Me.m_core.m_EcoSimData

        Me.m_dataType = eDataTypes.EcosimFleetInput
        Me.m_coreComponent = eCoreComponentType.Ecosim

        Me.AllowValidation = False

        Me.Index = iFleet
        Me.DBID = simdata.FleetDBID(iFleet)
        Me.m_ValidationStatus = New cVariableStatus(Me, eStatusFlags.OK, "", eVarNameFlags.NotSet)

        'EPower
        val = New cValue(core, New Single, eVarNameFlags.EPower, eStatusFlags.Null, eValueTypes.Sng)
        Me.m_values.Add(val.varName, val)

        'PcapBase
        val = New cValue(core, New Single, eVarNameFlags.PcapBase, eStatusFlags.Null, eValueTypes.Sng)
        Me.m_values.Add(val.varName, val)

        'CapDepreciate
        val = New cValue(core, New Single, eVarNameFlags.CapDepreciate, eStatusFlags.Null, eValueTypes.Sng)
        Me.m_values.Add(val.varName, val)

        'CapBaseGrowth
        val = New cValue(core, New Single, eVarNameFlags.CapBaseGrowth, eStatusFlags.Null, eValueTypes.Sng)
        Me.m_values.Add(val.varName, val)

        ' FleetEffortConversion
        val = New cValue(core, New Single, eVarNameFlags.FleetEffortConversion, eStatusFlags.Null, eValueTypes.Sng)
        Me.m_values.Add(val.varName, val)

        Me.AllowValidation = True

    End Sub

#Region " Variable via dot '.' operator "

    ''' <summary>
    ''' Effort response pow.fi
    ''' </summary>
    Public Property EPower() As Single

        Get
            Return CSng(Me.GetVariable(eVarNameFlags.EPower))
        End Get

        Set(value As Single)
            Me.SetVariable(eVarNameFlags.EPower, value)
        End Set

    End Property

    ''' <summary>
    ''' capital depreciation rate
    ''' </summary>
    Public Property CapDepreciateRate() As Single

        Get
            Return CSng(Me.GetVariable(eVarNameFlags.CapDepreciate))
        End Get

        Set(value As Single)
            Me.SetVariable(eVarNameFlags.CapDepreciate, value)
        End Set

    End Property

    ''' <summary>
    ''' Initial effort / capital capacity
    ''' </summary>
    Public Property PcapBase() As Single

        Get
            Return CSng(Me.GetVariable(eVarNameFlags.PcapBase))
        End Get

        Set(value As Single)
            Me.SetVariable(eVarNameFlags.PcapBase, value)
        End Set

    End Property

    ''' <summary>
    ''' initial capitial growth
    ''' </summary>
    Public Property CapBaseGrowth() As Single

        Get
            Return CSng(Me.GetVariable(eVarNameFlags.CapBaseGrowth))
        End Get

        Set(value As Single)
            Me.SetVariable(eVarNameFlags.CapBaseGrowth, value)
        End Set

    End Property


    ''' <summary>
    ''' Effort conversion factor used for summing maps into a single Effort value
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property EffortConversionFactor() As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.FleetEffortConversion))
        End Get
        Set(value As Single)
            Me.SetVariable(eVarNameFlags.FleetEffortConversion, value)
        End Set
    End Property

#End Region ' Variable via dot '.' operator

#Region " Status via dot '.' operator "

    Public Property CapBaseGrowthStatus() As eStatusFlags

        Get
            Return Me.GetStatus(eVarNameFlags.CapBaseGrowth)
        End Get

        Friend Set(value As eStatusFlags)
            Me.SetStatus(eVarNameFlags.CapBaseGrowth, value)
        End Set

    End Property

    Public Property CapDepreciateRateStatus() As eStatusFlags

        Get
            Return Me.GetStatus(eVarNameFlags.CapDepreciate)
        End Get

        Friend Set(value As eStatusFlags)
            Me.SetStatus(eVarNameFlags.CapDepreciate, value)
        End Set

    End Property

    Public Property PcapBaseStatus() As eStatusFlags

        Get
            Return Me.GetStatus(eVarNameFlags.PcapBase)
        End Get

        Friend Set(value As eStatusFlags)
            Me.SetStatus(eVarNameFlags.PcapBase, value)
        End Set

    End Property

    Public Property EPowerStatus() As eStatusFlags
        Get
            Return Me.GetStatus(eVarNameFlags.EPower)
        End Get

        Friend Set(value As eStatusFlags)
            Me.SetStatus(eVarNameFlags.EPower, value)
        End Set
    End Property

    Public Property EffortConversionFactorStatus() As eStatusFlags
        Get
            Return Me.GetStatus(eVarNameFlags.FleetEffortConversion)
        End Get
        Set(value As eStatusFlags)
            Me.SetStatus(eVarNameFlags.FleetEffortConversion, value)
        End Set
    End Property

#End Region ' Status via dot '.' operator

End Class
