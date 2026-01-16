' ===============================================================================
' This file is part of Ecopath with Ecosim (EwE)
'
' EwE is free software: you can redistribute it and/or modify it under the terms
' of the GNU General Public License version 3 as published by the Free Software 
' Foundation.
'
' EwE is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; 
' without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR 
' PURPOSE. See the GNU General Public License for more details.
'
' You should have received a copy of the GNU General Public License along with EwE.
' If not, see https://www.gnu.org/licenses/gpl-3.0.html>. 
'
' Copyright 1991- 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'
Imports EwECore.ValueWrapper

Namespace MSE

    Public Class cMSEBatchTFMGroup
        Inherits cCoreGroupBase

        'Private m_BLimValues() As Single
        'Private m_BBaseValues() As Single
        'Private m_FMaxValues() As Single
        Private m_BatchData As MSEBatchManager.cMSEBatchDataStructures

        Public Sub New(core As cCore, ByRef MSEBatchData As MSEBatchManager.cMSEBatchDataStructures, theGroupDBID As Integer)
            MyBase.New(core)

            Dim val As cValue

            Me.m_dataType = eDataTypes.MSEBatchTFMInput
            Me.m_coreComponent = eCoreComponentType.MSE
            Me.AllowValidation = False
            Me.DBID = theGroupDBID

            Me.m_BatchData = MSEBatchData

            'default OK status used for setVariable
            'see comment setVariable(...)
            Me.m_ValidationStatus = New cVariableStatus(Me, eStatusFlags.OK, "", eVarNameFlags.NotSet)

            val = New cValue(core, New Single, eVarNameFlags.MSETFMBLimLower, eStatusFlags.Null, eValueTypes.Sng)
            Me.m_values.Add(val.varName, val)

            val = New cValue(core, New Single, eVarNameFlags.MSETFMBLimUpper, eStatusFlags.Null, eValueTypes.Sng)
            Me.m_values.Add(val.varName, val)

            val = New cValue(core, New Single, eVarNameFlags.MSETFMBBaseLower, eStatusFlags.Null, eValueTypes.Sng)
            Me.m_values.Add(val.varName, val)

            val = New cValue(core, New Single, eVarNameFlags.MSETFMBBaseUpper, eStatusFlags.Null, eValueTypes.Sng)
            Me.m_values.Add(val.varName, val)

            val = New cValue(core, New Single, eVarNameFlags.MSETFMFOptLower, eStatusFlags.Null, eValueTypes.Sng)
            Me.m_values.Add(val.varName, val)

            val = New cValue(core, New Single, eVarNameFlags.MSETFMFOptUpper, eStatusFlags.Null, eValueTypes.Sng)
            Me.m_values.Add(val.varName, val)

            'bBase
            val = New cValue(core, New Single, eVarNameFlags.MSEBBase, eStatusFlags.Null, eValueTypes.Sng)
            Me.m_values.Add(val.varName, val)
            'bLim
            val = New cValue(core, New Single, eVarNameFlags.MSEBLim, eStatusFlags.Null, eValueTypes.Sng)
            Me.m_values.Add(val.varName, val)
            'FOpt
            val = New cValue(core, New Single, eVarNameFlags.MSEFmax, eStatusFlags.Null, eValueTypes.Sng)
            Me.m_values.Add(val.varName, val)
            'Fmin
            val = New cValue(core, New Single, eVarNameFlags.MSEFmin, eStatusFlags.Null, eValueTypes.Sng)
            Me.m_values.Add(val.varName, val)

            'Used
            val = New cValue(core, New Boolean, eVarNameFlags.MSEBatchTFMManaged, eStatusFlags.Null, eValueTypes.Bool)
            Me.m_values.Add(val.varName, val)

            'Iteration values 
            val = New cValueArray(core, eValueTypes.SingleArray, eVarNameFlags.MSETFMFOptValues, eStatusFlags.Null, eCoreCounterTypes.nMSEBatchTFM)
            Me.m_values.Add(val.varName, val)

            val = New cValueArray(core, eValueTypes.SingleArray, eVarNameFlags.MSETFMBBaseValues, eStatusFlags.Null, eCoreCounterTypes.nMSEBatchTFM)
            Me.m_values.Add(val.varName, val)

            val = New cValueArray(core, eValueTypes.SingleArray, eVarNameFlags.MSETFMBLimValues, eStatusFlags.Null, eCoreCounterTypes.nMSEBatchTFM)
            Me.m_values.Add(val.varName, val)

            Me.AllowValidation = True

        End Sub


        Public Property BLim As Single
            Get
                Return CSng(Me.GetVariable(eVarNameFlags.MSEBLim))
            End Get

            Set(value As Single)
                Me.SetVariable(eVarNameFlags.MSEBLim, value)
            End Set
        End Property

        Public Property BLimLower As Single
            Get
                Return CSng(Me.GetVariable(eVarNameFlags.MSETFMBLimLower))
            End Get

            Set(value As Single)
                Me.SetVariable(eVarNameFlags.MSETFMBLimLower, value)
            End Set
        End Property

        Public Property BLimUpper As Single
            Get
                Return CSng(Me.GetVariable(eVarNameFlags.MSETFMBLimUpper))
            End Get

            Set(value As Single)
                Me.SetVariable(eVarNameFlags.MSETFMBLimUpper, value)
            End Set
        End Property



        Public Property BBase As Single
            Get
                Return CSng(Me.GetVariable(eVarNameFlags.MSEBBase))
            End Get

            Set(value As Single)
                Me.SetVariable(eVarNameFlags.MSEBBase, value)
            End Set
        End Property

        Public Property BBaseLower As Single
            Get
                Return CSng(Me.GetVariable(eVarNameFlags.MSETFMBBaseLower))
            End Get

            Set(value As Single)
                Me.SetVariable(eVarNameFlags.MSETFMBBaseLower, value)
            End Set
        End Property

        Public Property BBaseUpper As Single
            Get
                Return CSng(Me.GetVariable(eVarNameFlags.MSETFMBBaseUpper))
            End Get

            Set(value As Single)
                Me.SetVariable(eVarNameFlags.MSETFMBBaseUpper, value)
            End Set
        End Property

        Public Property FMax As Single
            Get
                Return CSng(Me.GetVariable(eVarNameFlags.MSEFmax))
            End Get

            Set(value As Single)
                Me.SetVariable(eVarNameFlags.MSEFmax, value)
            End Set
        End Property

        Public Property FMaxLower As Single
            Get
                Return CSng(Me.GetVariable(eVarNameFlags.MSETFMFOptLower))
            End Get

            Set(value As Single)
                Me.SetVariable(eVarNameFlags.MSETFMFOptLower, value)
            End Set
        End Property

        Public Property isManaged As Boolean
            Get
                Return CBool(Me.GetVariable(eVarNameFlags.MSEBatchTFMManaged))
            End Get

            Set(value As Boolean)
                Me.SetVariable(eVarNameFlags.MSEBatchTFMManaged, value)
            End Set
        End Property

        Public Property FMaxUpper As Single
            Get
                Return CSng(Me.GetVariable(eVarNameFlags.MSETFMFOptUpper))
            End Get

            Set(value As Single)
                Me.SetVariable(eVarNameFlags.MSETFMFOptUpper, value)
            End Set
        End Property

        Public Property FMaxValue(IterationIndex As Integer) As Single
            Get
                Return CSng(Me.GetVariable(eVarNameFlags.MSETFMFOptValues, IterationIndex))
            End Get

            Set(value As Single)
                Me.SetVariable(eVarNameFlags.MSETFMFOptValues, value, IterationIndex)
            End Set

        End Property

        Public Property BLimValue(IterationIndex As Integer) As Single

            Get
                Return CSng(Me.GetVariable(eVarNameFlags.MSETFMBLimValues, IterationIndex))
            End Get

            Set(value As Single)
                Me.SetVariable(eVarNameFlags.MSETFMBLimValues, value, IterationIndex)
            End Set
            'Get
            '    ' Debug.Assert(IterationIndex <= Me.m_BatchData.nTFM, Me.ToString & ".BLimValue() Index out of range!")
            '    If IterationIndex <= Me.m_BatchData.nTFM Then
            '        Return Me.m_BatchData.tfmBlim(IterationIndex, Me.Index)
            '    End If
            '    'OH My.....
            '    Return cCore.NULL_VALUE
            'End Get

            'Set(value As Single)
            '    'Debug.Assert(IterationIndex <= Me.m_BatchData.nTFM, Me.ToString & ".BLimValue() Index out of range!")
            '    If IterationIndex <= Me.m_BatchData.nTFM Then
            '        Me.m_BatchData.tfmBlim(IterationIndex, Me.Index) = value
            '    End If
            'End Set
        End Property


        Public Property BBaseValue(IterationIndex As Integer) As Single

            Get
                Return CSng(Me.GetVariable(eVarNameFlags.MSETFMBBaseValues, IterationIndex))
            End Get

            Set(value As Single)
                Me.SetVariable(eVarNameFlags.MSETFMBBaseValues, value, IterationIndex)
            End Set

            'Get
            '    'Debug.Assert(IterationIndex <= Me.m_BatchData.nTFM, Me.ToString & ".BLimValue() Index out of range!")
            '    If IterationIndex <= Me.m_BatchData.nTFM Then
            '        Return Me.m_BatchData.tfmBbase(IterationIndex, Me.Index)
            '    End If
            '    'OH My.....
            '    Return cCore.NULL_VALUE
            'End Get

            'Set(value As Single)
            '    'Debug.Assert(IterationIndex <= Me.m_BatchData.nTFM, Me.ToString & ".BLimValue() Index out of range!")
            '    If IterationIndex <= Me.m_BatchData.nTFM Then
            '        Me.m_BatchData.tfmBbase(IterationIndex, Me.Index) = value
            '    End If
            'End Set
        End Property

        'Public Overrides Function GetVariable(VarName As eVarNameFlags, Optional iIndex As Integer = -9999, Optional iIndex2 As Integer = -9999, Optional iIndex3 As Integer = -9999) As Object

        '    Select Case VarName
        '        Case eVarNameFlags.MSETFMBLimValues
        '            Return Me.BLimValue(Index)
        '        Case eVarNameFlags.MSETFMBBaseValues
        '            Return Me.BBaseValue(Index)
        '            'Case eVarNameFlags.MSETFMFOptValues
        '            '    Return Me.FMaxValue(Index)
        '    End Select

        '    Return MyBase.GetVariable(VarName, iIndex, iIndex2, iIndex3)

        'End Function


        'Public Overrides Function SetVariable(VarName As eVarNameFlags, newValue As Object, Optional iSecondaryIndex As Integer = -9999) As Boolean
        '    Dim bdone As Boolean
        '    Select Case VarName
        '        Case eVarNameFlags.MSETFMBLimValues
        '            Me.BLimValue(iSecondaryIndex) = CSng(newValue)
        '            bdone = True
        '        Case eVarNameFlags.MSETFMBBaseValues
        '            Me.BBaseValue(Index) = CSng(newValue)
        '            bdone = True
        '            'Case eVarNameFlags.MSETFMFOptValues
        '            '    Me.m_BatchData.tfmFmax(Index, iSecondaryIndex) = CSng(newValue)
        '            '    bdone = True
        '    End Select

        '    If bdone Then
        '        Me.m_core.Messages.SendMessage(New cMessage("Values update.", eMessageType.DataModified, eCoreComponentType.MSE, _
        '                                                eMessageImportance.Maintenance, eDataTypes.MSEBatchTFMInput))
        '        Return True
        '    Else
        '        Return MyBase.SetVariable(VarName, newValue, iSecondaryIndex)
        '    End If


        'End Function


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
