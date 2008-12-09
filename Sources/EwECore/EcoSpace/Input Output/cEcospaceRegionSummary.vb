'==============================================================================
'
' $Log: cEcospaceRegionSummary.vb,v $
' Revision 1.2  2008/12/09 19:48:56  joeb
' Ouput objects now use core data instead of buffering data
'
' Revision 1.1  2008/09/26 07:30:22  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.8  2008/09/23 14:58:42  joeb
' Added Resize to resize arrays when being loaded
'
' Revision 1.7  2008/09/22 19:50:13  joeb
' Rename GameManager.DataType to CoreData
'
' Revision 1.6  2008/09/15 16:58:20  joeb
' Added more Ecospace output for Game Server
'
' Revision 1.5  2008/05/29 22:22:45  jeroens
' Moved eVarNameFlags to EwEUtils
'
' Revision 1.4  2007/12/07 21:44:04  jeroens
' * Solved 'strict on' bug
'
' Revision 1.3  2007/12/07 21:03:35  jeroens
' Added header
'
'==============================================================================

Option Strict On
Imports EwECore.ValueWrapper
Imports EwEUtils.Core

Public Class cEcospaceRegionSummary
    Inherits cCoreInputOutputBase

    'Private m_data(,,) As Single
    'Private m_biomByTime(,) As Single
    Private m_spacedata As cEcospaceDataStructures
    Private m_Vars As New Dictionary(Of eVarNameFlags, IResultsWrapper)

#Region "Constructor"

    Public Sub New(ByRef TheCore As cCore, ByVal EcospaceData As cEcospaceDataStructures, ByVal iRegion As Integer)
        MyBase.New(TheCore)

        Me.m_spacedata = EcospaceData

        Me.DBID = iRegion '????
        Me.Index = iRegion
        Me.m_DataType = eDataTypes.EcospaceRegionResults

    End Sub


    Public Sub Init()

        m_Vars.Clear()
        m_Vars.Add(eVarNameFlags.EcospaceRegionBiomass, New c3DResultsWrapper(m_spacedata.BiomassRegionGroup, Me.Index))

        'BiomassByRegion(summaryperiod(fixed),region(fixed),group(varies))
        m_Vars.Add(eVarNameFlags.EcospaceRegionBiomassStart, New c3DResultsWrapper2Fixed(m_spacedata.SumBiomassRegion, 0, Me.Index))
        m_Vars.Add(eVarNameFlags.EcospaceRegionBiomassEnd, New c3DResultsWrapper2Fixed(m_spacedata.SumBiomassRegion, 1, Me.Index))

        'CatchGearGroupRegion(var(fixed),region(fixed),fleet(varies),group(varies)) vartype and region fixed
        m_Vars.Add(eVarNameFlags.EcospaceRegionCatchStart, New c4DResultsWrapper(m_spacedata.CatchGearGroupRegion, 0, Me.Index))
        m_Vars.Add(eVarNameFlags.EcospaceRegionCatchEnd, New c4DResultsWrapper(m_spacedata.CatchGearGroupRegion, 1, Me.Index))

    End Sub

#End Region

#Region "Implementation of GetVariable() SetVariable() GetStatus() SetStatus()"

    Public Overrides Function GetVariable(ByVal varName As eVarNameFlags, Optional ByVal iFirstIndex As Integer = cCore.NULL_VALUE, Optional ByVal iSecondIndex As Integer = cCore.NULL_VALUE) As Object
        Try

            If Not m_Vars.ContainsKey(varName) Then
                'NOT in list of sim vars so get the value from the base class GetVariable(...)
                Return MyBase.GetVariable(varName, iFirstIndex, iSecondIndex)
            Else
                'Varname is access directly via the core data
                Return m_Vars.Item(varName).Value(iFirstIndex, iSecondIndex)
            End If

        Catch ex As Exception
            Debug.Assert(False, ex.Message)
        End Try

        Return cCore.NULL_VALUE

    End Function

    Public Overloads Function GetStatus(ByVal varName As eVarNameFlags, ByVal iFleet As Integer, ByVal iGroup As Integer) As eStatusFlags
        Return eStatusFlags.OK 'Oh Yeah 
    End Function

    Public Overloads Function SetStatus(ByVal varName As eVarNameFlags, ByVal newValue As eStatusFlags, ByVal iFleet As Integer, ByVal iGroup As Integer) As Boolean
        Debug.Assert(False, "Not implemented yet.")
    End Function


#End Region

#Region "Variable via dot '.' operator"

    Public ReadOnly Property BiomassStart(ByVal iGroup As Integer) As Single
        Get
            Return CSng(GetVariable(eVarNameFlags.EcospaceRegionBiomassStart, iGroup))
        End Get

        'Set(ByVal value As Single)
        '    SetVariable(eVarNameFlags.EcospaceRegionBiomassStart, value, iGroup)
        'End Set
    End Property

    Public ReadOnly Property BiomassEnd(ByVal iGroup As Integer) As Single
        Get
            Return CSng(GetVariable(eVarNameFlags.EcospaceRegionBiomassEnd, iGroup))
        End Get

        'Set(ByVal value As Single)
        '    SetVariable(eVarNameFlags.EcospaceRegionBiomassEnd, value, iGroup)
        'End Set
    End Property


    Public ReadOnly Property CatchFleetGroupStart(ByVal iFleet As Integer, ByVal iGroup As Integer) As Single
        Get
            Try
                Return DirectCast(GetVariable(eVarNameFlags.EcospaceRegionCatchStart, iFleet, iGroup), Single)
            Catch ex As Exception
                Debug.Assert(False, ex.Message)
                Return cCore.NULL_VALUE
            End Try

        End Get

        'Set(ByVal value As Single)
        '    Try
        '        SetVariable(eVarNameFlags.EcospaceRegionCatchStart, value, iFleet, iGroup)
        '    Catch ex As Exception
        '        Debug.Assert(False, ex.Message)
        '    End Try
        'End Set
    End Property


    Public ReadOnly Property CatchFleetGroupEnd(ByVal iFleet As Integer, ByVal iGroup As Integer) As Single
        Get
            Try
                Return DirectCast(GetVariable(eVarNameFlags.EcospaceRegionCatchEnd, iFleet, iGroup), Single)
            Catch ex As Exception
                Debug.Assert(False, ex.Message)
                Return cCore.NULL_VALUE
            End Try

        End Get

        'Set(ByVal value As Single)
        '    Try
        '        SetVariable(eVarNameFlags.EcospaceRegionCatchEnd, value, iFleet, iGroup)
        '    Catch ex As Exception
        '        Debug.Assert(False, ex.Message)
        '    End Try
        'End Set

    End Property


    Public ReadOnly Property BiomassByTime(ByVal IGroup As Integer, ByVal iTime As Integer) As Single
        Get
            Try
                Return DirectCast(GetVariable(eVarNameFlags.EcospaceRegionBiomass, IGroup, iTime), Single)
            Catch ex As Exception
                Debug.Assert(False, ex.Message)
                Return cCore.NULL_VALUE
            End Try

        End Get

        'Set(ByVal value As Single)
        '    Try
        '        SetVariable(eVarNameFlags.EcospaceRegionBiomass, value, IGroup, iTime)
        '    Catch ex As Exception
        '        Debug.Assert(False, ex.Message)
        '    End Try
        'End Set

    End Property

#End Region

#Region "Status Flags via dot '.' operator"

    Public Property BiomassStartStatus(ByVal iGroup As Integer) As eStatusFlags
        Get
            Return GetStatus(eVarNameFlags.EcospaceRegionBiomassStart, iGroup)
        End Get

        Friend Set(ByVal value As eStatusFlags)
            SetStatus(eVarNameFlags.EcospaceRegionBiomassStart, value, iGroup)
        End Set
    End Property

    Public Property BiomassEndStatus(ByVal iGroup As Integer) As eStatusFlags
        Get
            Return GetStatus(eVarNameFlags.EcospaceRegionBiomassEnd, iGroup)
        End Get

        Friend Set(ByVal value As eStatusFlags)
            SetStatus(eVarNameFlags.EcospaceRegionBiomassEnd, value, iGroup)
        End Set
    End Property


    Public Property CatchFleetGroupStartStatus(ByVal iGroup As Integer, ByVal iFleet As Integer) As eStatusFlags
        Get
            Return GetStatus(eVarNameFlags.EcospaceRegionCatchStart, iGroup, iFleet)
        End Get

        Friend Set(ByVal value As eStatusFlags)
            SetStatus(eVarNameFlags.EcospaceRegionCatchStart, value, iGroup, iFleet)
        End Set
    End Property


    Public Property CatchFleetGroupEndStatus(ByVal iGroup As Integer, ByVal iFleet As Integer) As eStatusFlags
        Get
            Return GetStatus(eVarNameFlags.EcospaceRegionCatchEnd, iGroup, iFleet)
        End Get

        Friend Set(ByVal value As eStatusFlags)
            SetStatus(eVarNameFlags.EcospaceRegionCatchEnd, value, iGroup, iFleet)
        End Set
    End Property

#End Region

End Class
