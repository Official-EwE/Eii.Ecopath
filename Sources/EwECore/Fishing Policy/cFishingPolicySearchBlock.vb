' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports EwECore.ValueWrapper

''' <summary>
''' Blocks for Fishing Policy Search
''' </summary>
''' <remarks>This provides the CodeBlocks(iTimeIndex) interface</remarks>
Public Class cFishingPolicySearchBlock
    Inherits cCoreGroupBase

    Public Sub New(core As cCore, DBID As Integer)
        MyBase.New(core)

        Dim val As cValue

        Me.m_dataType = eDataTypes.FishingPolicySearchBlocks
        Me.m_coreComponent = eCoreComponentType.FishingPolicySearch
        Me.AllowValidation = False
        Me.DBID = DBID

        'default OK status used for setVariable
        'see comment setVariable(...)
        Me.m_ValidationStatus = New cVariableStatus(Me, eStatusFlags.OK, "", eVarNameFlags.NotSet)

        val = New cValueArray(core, eValueTypes.IntArray, eVarNameFlags.SearchBlock, eStatusFlags.Null, eCoreCounterTypes.nEcosimYears)
        Me.m_values.Add(val.varName, val)

        Me.AllowValidation = True

    End Sub


    Public Property SearchBlocks(iTimeIndex As Integer) As Integer

        Get
            Return CInt(Me.GetVariable(eVarNameFlags.SearchBlock, iTimeIndex))
        End Get

        Set(value As Integer)
            Me.SetVariable(eVarNameFlags.SearchBlock, value, iTimeIndex)
        End Set

    End Property

    ''' <summary>
    ''' Edit the SearchBlocks in batch mode no messages are sent out when BatchEdit = True when BatchEdit is toggled to False then the core is notified.
    ''' </summary>
    ''' <remarks>This turns off the AllowValidation flag which stops the object from calling core.OnValidate() vastly speeding up the editing</remarks>
    Public Property BatchEdit() As Boolean
        Get
            Return Not Me.AllowValidation
        End Get

        Set(value As Boolean)

            'if turning the BatchEdit On after it has been OFF tell the core that the values has been edited
            'this will allow the core to update the underlying data and send out a datamodified message
            If Me.BatchEdit = True And value = False Then
                Me.m_core.OnValidated(Me.m_values.Item(eVarNameFlags.SearchBlock), Me)
            End If
            Me.AllowValidation = Not value

        End Set

    End Property


End Class
