' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports EwECore.ValueWrapper

Namespace MSE

    Public Class cMSEBatchTACGroup
        Inherits cCoreGroupBase

        'Private m_BLimValues() As Single
        'Private m_BBaseValues() As Single
        'Private m_FMaxValues() As Single
        Private m_BatchData As MSEBatchManager.cMSEBatchDataStructures

        Public Sub New(core As cCore, ByRef MSEBatchData As MSEBatchManager.cMSEBatchDataStructures, theGroupDBID As Integer)
            MyBase.New(core)

            Dim val As cValue

            Me.m_dataType = eDataTypes.MSEBatchFixedFInput
            Me.m_coreComponent = eCoreComponentType.MSE
            Me.AllowValidation = False
            Me.DBID = theGroupDBID

            Me.m_BatchData = MSEBatchData

            'default OK status used for setVariable
            'see comment setVariable(...)
            Me.m_ValidationStatus = New cVariableStatus(Me, eStatusFlags.OK, "", eVarNameFlags.NotSet)

            val = New cValue(core, New Single, eVarNameFlags.MSEBatchTACLower, eStatusFlags.Null, eValueTypes.Sng)
            Me.m_values.Add(val.varName, val)

            val = New cValue(core, New Single, eVarNameFlags.MSEBatchTACUpper, eStatusFlags.Null, eValueTypes.Sng)
            Me.m_values.Add(val.varName, val)

            val = New cValue(core, New Single, eVarNameFlags.MSETAC, eStatusFlags.Null, eValueTypes.Sng)
            Me.m_values.Add(val.varName, val)

            'Used
            val = New cValue(core, New Boolean, eVarNameFlags.MSEBatchTACManaged, eStatusFlags.Null, eValueTypes.Bool)
            Me.m_values.Add(val.varName, val)

            Me.AllowValidation = True

        End Sub

        Public Property TAC As Single
            Get
                Return CSng(Me.GetVariable(eVarNameFlags.MSETAC))
            End Get

            Set(value As Single)
                Me.SetVariable(eVarNameFlags.MSETAC, value)
            End Set
        End Property

        Public Property TACLower As Single
            Get
                Return CSng(Me.GetVariable(eVarNameFlags.MSEBatchTACLower))
            End Get

            Set(value As Single)
                Me.SetVariable(eVarNameFlags.MSEBatchTACLower, value)
            End Set
        End Property

        Public Property TACUpper As Single
            Get
                Return CSng(Me.GetVariable(eVarNameFlags.MSEBatchTACUpper))
            End Get

            Set(value As Single)
                Me.SetVariable(eVarNameFlags.MSEBatchTACUpper, value)
            End Set
        End Property

        Public Property isManaged As Boolean
            Get
                Return CBool(Me.GetVariable(eVarNameFlags.MSEBatchTACManaged))
            End Get

            Set(value As Boolean)
                Me.SetVariable(eVarNameFlags.MSEBatchTACManaged, value)
            End Set
        End Property

        Public Property TACValue(IterationIndex As Integer) As Single
            Get
                ' Debug.Assert(IterationIndex <= Me.m_BatchData.nTFM, Me.ToString & ".BLimValue() Index out of range!")
                If IterationIndex <= Me.m_BatchData.nTAC Then
                    Return Me.m_BatchData.TAC(IterationIndex, Me.Index)
                End If
                'OH My.....
                Return cCore.NULL_VALUE
            End Get

            Set(value As Single)
                ' Debug.Assert(IterationIndex <= Me.m_BatchData.nTFM, Me.ToString & ".BLimValue() Index out of range!")
                If IterationIndex <= Me.m_BatchData.nTAC Then
                    Me.m_BatchData.TAC(IterationIndex, Me.Index) = value
                End If
            End Set
        End Property

        Public Overrides Function GetVariable(VarName As eVarNameFlags, Optional iIndex As Integer = -9999, Optional iIndex2 As Integer = -9999, Optional iIndex3 As Integer = -9999) As Object

            Select Case VarName
                Case eVarNameFlags.MSEBatchTACValues
                    Return Me.m_BatchData.TAC(iIndex, Me.Index)

            End Select

            Return MyBase.GetVariable(VarName, iIndex, iIndex2, iIndex3)

        End Function

        Public Overrides Function SetVariable(VarName As eVarNameFlags, newValue As Object, Optional iSecondaryIndex As Integer = -9999, Optional iThirdIndex As Integer = -9999) As Boolean

            Select Case VarName

                Case eVarNameFlags.MSEBatchTACValues

                    Me.m_BatchData.TAC(iSecondaryIndex, Me.Index) = CSng(newValue)
                    Return True
            End Select

            Return MyBase.SetVariable(VarName, newValue, iSecondaryIndex)

        End Function

        Friend Overrides Function ResetStatusFlags(Optional bForceReset As Boolean = False) As Boolean
            MyBase.ResetStatusFlags(bForceReset)

            Me.AllowValidation = False
            Dim tcatch As Single

            For iflt As Integer = 1 To Me.m_core.nFleets
                Dim fleet As cEcopathFleetInput = Me.m_core.EcopathFleetInputs(iflt)
                tcatch += fleet.Landings(Me.Index) + fleet.Discards(Me.Index)
            Next

            If tcatch = 0.0! Then
                For Each var As cValue In Me.m_values.Values
                    If var.varName <> eVarNameFlags.Name And var.varName <> eVarNameFlags.Index And var.varName <> eVarNameFlags.DBID Then
                        Me.SetStatusFlags(var.varName, eStatusFlags.Null Or eStatusFlags.NotEditable)
                    End If
                Next
            End If

            Return True

        End Function

    End Class

End Namespace
