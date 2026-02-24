' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

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
