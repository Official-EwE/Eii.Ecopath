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



Namespace Ecospace.Advection

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Inputs for Ecospace Advection calculations.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Class cAdvectionParameters
        Inherits cCoreInputOutputBase

        Public Sub New(core As cCore, DBID As Integer)
            MyBase.New(core)

            Me.AllowValidation = False
            Me.DBID = DBID
            Me.m_dataType = eDataTypes.EcospaceAdvectionParameters
            Me.m_coreComponent = eCoreComponentType.Ecospace
            Me.AllowValidation = False

            'default OK status used for setVariable
            'see comment setVariable(...)
            Me.m_ValidationStatus = New cVariableStatus(Me, eStatusFlags.OK, "", eVarNameFlags.NotSet)

            Dim val As cValue

            ' XVel
            val = New cValue(core, New Single, eVarNameFlags.XVelocity, eStatusFlags.Null, eValueTypes.Sng)
            val.Stored = False
            Me.m_values.Add(val.varName, val)

            ' YVel
            val = New cValue(core, New Single, eVarNameFlags.YVelocity, eStatusFlags.Null, eValueTypes.Sng)
            val.Stored = False
            Me.m_values.Add(val.varName, val)

            ' Coriolis
            val = New cValue(core, New Single, eVarNameFlags.Coriolis, eStatusFlags.Null, eValueTypes.Sng)
            val.Stored = False
            Me.m_values.Add(val.varName, val)

            ' SorWv
            val = New cValue(core, New Single, eVarNameFlags.SorWv, eStatusFlags.Null, eValueTypes.Sng)
            val.Stored = False
            Me.m_values.Add(val.varName, val)

            val = New cValue(core, New Single, eVarNameFlags.AdvectionUpwellingThreshold, eStatusFlags.Null, eValueTypes.Sng)
            val.Stored = False
            Me.m_values.Add(val.varName, val)

            val = New cValue(core, New Single, eVarNameFlags.AdvectionUpwellingPPMultiplier, eStatusFlags.Null, eValueTypes.Sng)
            val.Stored = False
            Me.m_values.Add(val.varName, val)

            Me.ResetStatusFlags()

            Me.AllowValidation = True

        End Sub


        Public Property UpwellingThreshold() As Single
            Get
                Return CSng(Me.GetVariable(eVarNameFlags.AdvectionUpwellingThreshold))
            End Get

            Set(value As Single)
                Me.SetVariable(eVarNameFlags.AdvectionUpwellingThreshold, value)
            End Set
        End Property


        'UpwellingPPMultiplier
        Public Property UpwellingPPMultiplier() As Single
            Get
                Return CSng(Me.GetVariable(eVarNameFlags.AdvectionUpwellingPPMultiplier))
            End Get

            Set(value As Single)
                Me.SetVariable(eVarNameFlags.AdvectionUpwellingPPMultiplier, value)
            End Set
        End Property

    End Class

End Namespace
