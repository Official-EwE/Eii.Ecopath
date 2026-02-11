' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports EwECore.ValueWrapper

''' <summary>
''' Results of the current search iteration
''' </summary>
''' <remarks></remarks>
Public Class cMPAOptOutput
    Inherits cCoreGroupBase

    'Private m_cells As List(Of cMPACell)

    Sub New(core As cCore)
        MyBase.New(core)
        Dim val As cValue = Nothing

        Me.DBID = cCore.NULL_VALUE '????
        Me.Index = cCore.NULL_VALUE
        Me.m_dataType = eDataTypes.MPAOptOutput

        ' Outputs should never send out messages
        Me.m_coreComponent = eCoreComponentType.MPAOptimization
        'default OK status used for SetVariable
        'see comment SetVariable(...)
        Me.m_ValidationStatus = New cVariableStatus(Me, eStatusFlags.OK Or eStatusFlags.NotEditable, "", eVarNameFlags.NotSet)

        val = New cValue(core, New Integer, eVarNameFlags.MPAOptBestCol, eStatusFlags.NotEditable, eValueTypes.Int)
        Me.m_values.Add(val.varName, val)

        val = New cValue(core, New Integer, eVarNameFlags.MPAOptBestRow, eStatusFlags.NotEditable, eValueTypes.Int)
        Me.m_values.Add(val.varName, val)

        val = New cValue(core, New Integer, eVarNameFlags.MPAOptCurRow, eStatusFlags.NotEditable, eValueTypes.Int)
        Me.m_values.Add(val.varName, val)

        val = New cValue(core, New Integer, eVarNameFlags.MPAOptCurCol, eStatusFlags.NotEditable, eValueTypes.Int)
        Me.m_values.Add(val.varName, val)

        val = New cValue(core, New Single, eVarNameFlags.MPAOptEconomicValue, eStatusFlags.NotEditable, eValueTypes.Sng)
        Me.m_values.Add(val.varName, val)

        val = New cValue(core, New Single, eVarNameFlags.MPAOptEcologicalValue, eStatusFlags.NotEditable, eValueTypes.Sng)
        Me.m_values.Add(val.varName, val)

        val = New cValue(core, New Single, eVarNameFlags.MPAOptMandatedValue, eStatusFlags.NotEditable, eValueTypes.Sng)
        Me.m_values.Add(val.varName, val)

        val = New cValue(core, New Single, eVarNameFlags.MPAOptSocialValue, eStatusFlags.NotEditable, eValueTypes.Sng)
        Me.m_values.Add(val.varName, val)

        val = New cValue(core, New Single, eVarNameFlags.MPAOptTotalValue, eStatusFlags.NotEditable, eValueTypes.Sng)
        Me.m_values.Add(val.varName, val)

        val = New cValue(core, New Integer, eVarNameFlags.MPAOptPercentageClosed, eStatusFlags.NotEditable, eValueTypes.Int)
        Me.m_values.Add(val.varName, val)

        val = New cValue(core, New Single, eVarNameFlags.MPAOptBiomassDiversityValue, eStatusFlags.NotEditable, eValueTypes.Sng)
        Me.m_values.Add(val.varName, val)

        val = New cValue(core, New Single, eVarNameFlags.MPAOptAreaBoundary, eStatusFlags.NotEditable, eValueTypes.Sng)
        Me.m_values.Add(val.varName, val)

    End Sub

    Friend Sub Init(ByRef mpaData As cMPAOptDataStructures, SpaceData As cEcospaceDataStructures)

        Me.BestRow = mpaData.bestrow
        Me.BestCol = mpaData.bestcol
        Me.CurRow = mpaData.CurRow
        Me.CurCol = mpaData.CurCol

        Me.EcologicalValue = mpaData.objFuncEcologicalValue
        Me.EconomicValue = mpaData.objFuncEconomicValue
        Me.MandatedValue = mpaData.objFuncMandatedValue
        Me.SocialValue = mpaData.objFuncSocialValue
        Me.TotalValue = mpaData.objFuncTotal
        Me.BiomassDiversityValue = mpaData.objFuncBiodiversity
        Me.AreaBoundaryValue = mpaData.objFuncAreaBorder

        Dim nTotCells As Integer = SpaceData.nWaterCells
        Dim nMPACells As Integer
        For ir As Integer = 1 To SpaceData.InRow
            For ic As Integer = 1 To SpaceData.InCol
                If SpaceData.Depth(ir, ic) > 0 Then
                    If SpaceData.MPA(mpaData.iMPAtoUse)(ir, ic) > 0 Then
                        nMPACells += 1
                    End If
                End If
            Next
        Next

        Me.PercentageClosed = CInt(nMPACells / nTotCells * 100)

    End Sub

    ''' <summary>
    ''' Row of the best cell for the currrent Ecoseed evaluation
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property BestRow() As Integer
        Get
            Return CInt(Me.GetVariable(eVarNameFlags.MPAOptBestRow))
        End Get

        Set(newValue As Integer)
            If Not Me.m_bReadOnly Then
                Me.SetVariable(eVarNameFlags.MPAOptBestRow, newValue)
            End If
        End Set

    End Property

    ''' <summary>
    ''' Col of the best cell for the currrent Ecoseed evaluation
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property BestCol() As Integer
        Get
            Return CInt(Me.GetVariable(eVarNameFlags.MPAOptBestCol))
        End Get

        Set(newValue As Integer)
            If Not Me.m_bReadOnly Then
                Me.SetVariable(eVarNameFlags.MPAOptBestCol, newValue)
            End If
        End Set

    End Property

    ''' <summary>
    ''' Row of current cell being evaluated by Ecoseed
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property CurRow() As Integer
        Get
            Return CInt(Me.GetVariable(eVarNameFlags.MPAOptCurRow))
        End Get

        Set(newValue As Integer)
            If Not Me.m_bReadOnly Then
                Me.SetVariable(eVarNameFlags.MPAOptCurRow, newValue)
            End If
        End Set

    End Property

    ''' <summary>
    '''  Col of current cell being evaluated by Ecoseed
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property CurCol() As Integer
        Get
            Return CInt(Me.GetVariable(eVarNameFlags.MPAOptCurCol))
        End Get

        Set(newValue As Integer)
            If Not Me.m_bReadOnly Then
                Me.SetVariable(eVarNameFlags.MPAOptCurCol, newValue)
            End If
        End Set

    End Property

    Public Property EconomicValue() As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.MPAOptEconomicValue))
        End Get

        Set(newValue As Single)
            If Not Me.m_bReadOnly Then
                Me.SetVariable(eVarNameFlags.MPAOptEconomicValue, newValue)
            End If
        End Set

    End Property

    Public Property EcologicalValue() As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.MPAOptEcologicalValue))
        End Get

        Set(newValue As Single)
            If Not Me.m_bReadOnly Then
                Me.SetVariable(eVarNameFlags.MPAOptEcologicalValue, newValue)
            End If
        End Set

    End Property

    Public Property MandatedValue() As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.MPAOptMandatedValue))
        End Get

        Set(newValue As Single)
            If Not Me.m_bReadOnly Then
                Me.SetVariable(eVarNameFlags.MPAOptMandatedValue, newValue)
            End If
        End Set

    End Property

    Public Property SocialValue() As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.MPAOptSocialValue))
        End Get

        Set(newValue As Single)
            If Not Me.m_bReadOnly Then
                Me.SetVariable(eVarNameFlags.MPAOptSocialValue, newValue)
            End If
        End Set

    End Property

    Public Property BiomassDiversityValue() As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.MPAOptBiomassDiversityValue))
        End Get

        Set(newValue As Single)
            If Not Me.m_bReadOnly Then
                Me.SetVariable(eVarNameFlags.MPAOptBiomassDiversityValue, newValue)
            End If
        End Set

    End Property

    Public Property TotalValue() As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.MPAOptTotalValue))
        End Get

        Set(newValue As Single)
            If Not Me.m_bReadOnly Then
                Me.SetVariable(eVarNameFlags.MPAOptTotalValue, newValue)
            End If
        End Set

    End Property

    Public Property AreaBoundaryValue() As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.MPAOptAreaBoundary))
        End Get

        Set(newValue As Single)
            If Not Me.m_bReadOnly Then
                Me.SetVariable(eVarNameFlags.MPAOptAreaBoundary, newValue)
            End If
        End Set

    End Property

    Public Property PercentageClosed() As Integer
        Get
            Return CInt(Me.GetVariable(eVarNameFlags.MPAOptPercentageClosed))
        End Get

        Set(newValue As Integer)
            If Not Me.m_bReadOnly Then
                Me.SetVariable(eVarNameFlags.MPAOptPercentageClosed, newValue)
            End If
        End Set

    End Property

End Class
