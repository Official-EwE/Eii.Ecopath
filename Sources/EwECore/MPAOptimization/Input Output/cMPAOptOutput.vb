'==============================================================================
'
' $Log: cMPAOptOutput.vb,v $
' Revision 1.2  2008/11/12 18:02:34  jeroens
' Added Biomass Diversity search weight input + output
'
'==============================================================================

Option Strict On
Imports EwECore.ValueWrapper
Imports EwEUtils.Core

''' <summary>
''' Results of the current search iteration
''' </summary>
''' <remarks></remarks>
Public Class cMPAOptOutput
    Inherits cCoreGroupBase

    'Private m_cells As List(Of cMPACell)

    Sub New(ByRef theCore As cCore)
        MyBase.New(theCore)
        Dim val As cValue = Nothing

        Me.DBID = cCore.NULL_VALUE '????
        Me.Index = cCore.NULL_VALUE
        Me.m_DataType = eDataTypes.MPAOptOuput

        ' Outputs should never send out messages
        m_messageSource = eMessageSource.MPAOptimization
        'default OK status used for SetVariable
        'see comment SetVariable(...)
        m_ValidationStatus = New cVariableStatus(Me, eStatusFlags.OK, "", eVarNameFlags.NotSet, Me.m_DataType, _
                                        m_messageSource, Index, cCore.NULL_VALUE)

        val = New cValue(New Integer, eVarNameFlags.MPAOptBestCol, eStatusFlags.NotEditable, eValueTypes.Int)
        m_values.Add(val.varName, val)

        val = New cValue(New Integer, eVarNameFlags.MPAOptBestRow, eStatusFlags.NotEditable, eValueTypes.Int)
        m_values.Add(val.varName, val)

        val = New cValue(New Integer, eVarNameFlags.MPAOptCurRow, eStatusFlags.NotEditable, eValueTypes.Int)
        m_values.Add(val.varName, val)

        val = New cValue(New Integer, eVarNameFlags.MPAOptCurCol, eStatusFlags.NotEditable, eValueTypes.Int)
        m_values.Add(val.varName, val)

        val = New cValue(New Single, eVarNameFlags.MPAOptEconomicValue, eStatusFlags.NotEditable, eValueTypes.Sng)
        m_values.Add(val.varName, val)

        val = New cValue(New Single, eVarNameFlags.MPAOptEcologicalValue, eStatusFlags.NotEditable, eValueTypes.Sng)
        m_values.Add(val.varName, val)

        val = New cValue(New Single, eVarNameFlags.MPAOptMandatedValue, eStatusFlags.NotEditable, eValueTypes.Sng)
        m_values.Add(val.varName, val)

        val = New cValue(New Single, eVarNameFlags.MPAOptSocialValue, eStatusFlags.NotEditable, eValueTypes.Sng)
        m_values.Add(val.varName, val)

        val = New cValue(New Single, eVarNameFlags.MPAOptTotalValue, eStatusFlags.NotEditable, eValueTypes.Sng)
        m_values.Add(val.varName, val)

        val = New cValue(New Single, eVarNameFlags.MPAOptPercentageClosed, eStatusFlags.NotEditable, eValueTypes.Sng)
        m_values.Add(val.varName, val)

        val = New cValue(New Single, eVarNameFlags.MPAOptBiomassDiversityValue, eStatusFlags.NotEditable, eValueTypes.Sng)
        m_values.Add(val.varName, val)

    End Sub


    Friend Sub Init(ByRef EcoSeedData As cMPAOptDataStructures, ByVal SpaceData As cEcospaceDataStructures)

        Me.BestRow = EcoSeedData.bestrow
        Me.BestCol = EcoSeedData.bestcol
        Me.CurRow = EcoSeedData.CurRow
        Me.CurCol = EcoSeedData.CurCol

        Me.EcologicalValue = EcoSeedData.objFuncEcologicalValue
        Me.EconomicValue = EcoSeedData.objFuncEconomicValue
        Me.MandatedValue = EcoSeedData.objFuncMandatedValue
        Me.SocialValue = EcoSeedData.objFuncSocialValue
        Me.TotalValue = EcoSeedData.objFuncTotal
        ' ToDo_JB: Hook this up
        'Me.BiomassDiversityValue = EcoSeedData.objBiomassDiversityValue

        Dim nTotCells As Integer = SpaceData.nWaterCells
        Dim nMPACells As Integer
        For ir As Integer = 1 To SpaceData.Inrow
            For ic As Integer = 1 To SpaceData.InCol
                If SpaceData.MPA(ir, ic) = EcoSeedData.iMPAtoUse Then
                    nMPACells += 1
                End If
            Next
        Next

        Me.PercentageClosed = CSng(nMPACells / nTotCells * 100)

    End Sub

    ''' <summary>
    ''' Row of the best cell for the currrent Ecoseed evaluation
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property BestRow() As Integer
        Get
            Return CInt(GetVariable(eVarNameFlags.MPAOptBestRow))
        End Get

        Set(ByVal newValue As Integer)
            If Not m_bReadOnly Then
                SetVariable(eVarNameFlags.MPAOptBestRow, newValue)
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
            Return CInt(GetVariable(eVarNameFlags.MPAOptBestCol))
        End Get

        Set(ByVal newValue As Integer)
            If Not m_bReadOnly Then
                SetVariable(eVarNameFlags.MPAOptBestCol, newValue)
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
            Return CInt(GetVariable(eVarNameFlags.MPAOptCurRow))
        End Get

        Set(ByVal newValue As Integer)
            If Not m_bReadOnly Then
                SetVariable(eVarNameFlags.MPAOptCurRow, newValue)
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
            Return CInt(GetVariable(eVarNameFlags.MPAOptCurCol))
        End Get

        Set(ByVal newValue As Integer)
            If Not m_bReadOnly Then
                SetVariable(eVarNameFlags.MPAOptCurCol, newValue)
            End If
        End Set

    End Property


    Public Property EconomicValue() As Single
        Get
            Return CSng(GetVariable(eVarNameFlags.MPAOptEconomicValue))
        End Get

        Set(ByVal newValue As Single)
            If Not m_bReadOnly Then
                SetVariable(eVarNameFlags.MPAOptEconomicValue, newValue)
            End If
        End Set

    End Property

    Public Property EcologicalValue() As Single
        Get
            Return CSng(GetVariable(eVarNameFlags.MPAOptEcologicalValue))
        End Get

        Set(ByVal newValue As Single)
            If Not m_bReadOnly Then
                SetVariable(eVarNameFlags.MPAOptEcologicalValue, newValue)
            End If
        End Set

    End Property

    Public Property MandatedValue() As Single
        Get
            Return CSng(GetVariable(eVarNameFlags.MPAOptMandatedValue))
        End Get

        Set(ByVal newValue As Single)
            If Not m_bReadOnly Then
                SetVariable(eVarNameFlags.MPAOptMandatedValue, newValue)
            End If
        End Set

    End Property

    Public Property SocialValue() As Single
        Get
            Return CSng(GetVariable(eVarNameFlags.MPAOptSocialValue))
        End Get

        Set(ByVal newValue As Single)
            If Not m_bReadOnly Then
                SetVariable(eVarNameFlags.MPAOptSocialValue, newValue)
            End If
        End Set

    End Property

    Public Property BiomassDiversityValue() As Single
        Get
            Return CSng(GetVariable(eVarNameFlags.FPSBiomassDiversityWeight))
        End Get

        Set(ByVal newValue As Single)
            If Not m_bReadOnly Then
                SetVariable(eVarNameFlags.FPSBiomassDiversityWeight, newValue)
            End If
        End Set

    End Property

    Public Property TotalValue() As Single
        Get
            Return CSng(GetVariable(eVarNameFlags.MPAOptTotalValue))
        End Get

        Set(ByVal newValue As Single)
            If Not m_bReadOnly Then
                SetVariable(eVarNameFlags.MPAOptTotalValue, newValue)
            End If
        End Set

    End Property

    Public Property PercentageClosed() As Single
        Get
            Return CSng(GetVariable(eVarNameFlags.MPAOptPercentageClosed))
        End Get

        Set(ByVal newValue As Single)
            If Not m_bReadOnly Then
                SetVariable(eVarNameFlags.MPAOptPercentageClosed, newValue)
            End If
        End Set

    End Property

    '''' <summary>
    '''' Best cells, up to this point, from the Random search.
    '''' </summary>
    '''' <value></value>
    '''' <returns></returns>
    '''' <remarks>The objective function values of the best cells are not avalible at this time we need to figure out how to do this</remarks>
    'Public ReadOnly Property BestCells() As List(Of cMPACell)
    '    Get
    '        Return Me.m_cells
    '    End Get

    'End Property

End Class
