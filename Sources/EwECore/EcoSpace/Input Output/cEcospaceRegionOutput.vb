'==============================================================================
'
' $Log: cEcospaceRegionOutput.vb,v $
' Revision 1.3  2009/01/20 22:30:45  joeb
' Added Catch Region Fleet Group
'
' Revision 1.2  2009/01/16 18:30:24  jeroens
' eMessageSource renamed to eCoreComponentTypes
'
' Revision 1.1  2009/01/13 21:13:59  joeb
' Merged Summary objects into Output objects
'
' Revision 1.3  2009/01/12 22:52:19  joeb
' Changed how Ecospace stores it results all data is now stored over time
'
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

Public Class cEcospaceRegionOutput
    Inherits cCoreInputOutputBase

    Private m_spacedata As cEcospaceDataStructures
    Private m_CoreArrays As New Dictionary(Of eVarNameFlags, IResultsWrapper)
    Private m_CatchFleetGroup(,,) As Single

#Region "Constructor"

    Public Sub New(ByRef TheCore As cCore, ByVal EcospaceData As cEcospaceDataStructures, ByVal iRegion As Integer)
        MyBase.New(TheCore)

        Me.m_spacedata = EcospaceData

        Me.DBID = iRegion '????
        Me.Index = iRegion
        Me.m_dataType = eDataTypes.EcospaceRegionResults

        Dim val As cValue

        'Weirdness
        'There are three ways of managing data
        'If the data has a core array then use that directly via the m_CoreArrays dictionary
        'If no core array and the data can fit into a cValue object then use that, only one variable index
        'If no core array and the data contains more then one variable index then use a local buffer

        'cValue objects
        val = New cValueArray(eValueTypes.SingleArray, eVarNameFlags.EcospaceRegionBiomassStart, eStatusFlags.OK, eCoreCounterTypes.nGroups, AddressOf TheCore.GetCoreCounter)
        m_values.Add(val.varName, val)

        val = New cValueArray(eValueTypes.SingleArray, eVarNameFlags.EcospaceRegionBiomassEnd, eStatusFlags.OK, eCoreCounterTypes.nGroups, AddressOf TheCore.GetCoreCounter)
        m_values.Add(val.varName, val)

    End Sub


    Public Sub Init()

        Try
            m_CoreArrays.Clear()
            m_CoreArrays.Add(eVarNameFlags.EcospaceRegionBiomass, New c3DResultsWrapper(m_spacedata.ResultsRegionGroup, Me.Index))
            m_CoreArrays.Add(eVarNameFlags.EcospaceRegionFleetGroupCatch, New c4DResultsWrapperFirstFixed(m_spacedata.ResultsCatchRegionGearGroup, Me.Index))
        Catch ex As Exception
            Debug.Assert(False, Me.ToString & ".Init() Error: " & ex.Message)
            cLog.Write(ex)
        End Try

    End Sub

#End Region

#Region "Implementation of GetVariable() SetVariable() GetStatus() SetStatus()"

    Public Overrides Function GetVariable(ByVal varName As eVarNameFlags, Optional ByVal iFirstIndex As Integer = cCore.NULL_VALUE, Optional ByVal iSecondIndex As Integer = cCore.NULL_VALUE, Optional ByVal iIndex3 As Integer = cCore.NULL_VALUE) As Object
        Try

            If Not m_CoreArrays.ContainsKey(varName) Then
                Debug.Assert(iSecondIndex = cCore.NULL_VALUE, Me.ToString & ".GetVariable() called with optional argument iSecondIndex for variable " & varName.ToString & " this can not be handled for this variable.")
                'NOT in list of sim vars so get the value from the base class GetVariable(...)
                Return MyBase.GetVariable(varName, iFirstIndex)
            Else
                'Varname is access directly via the core data
                Return m_CoreArrays.Item(varName).Value(iFirstIndex, iSecondIndex, iIndex3)
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

    Friend Overrides Function Resize() As Boolean
        MyBase.Resize()

        'resize local buffer
        ReDim Me.m_CatchFleetGroup(1, Me.m_core.nFleets, Me.m_core.nGroups)
        Return True
    End Function


#End Region

#Region "Variable via dot '.' operator"

    Public Property BiomassStart(ByVal iGroup As Integer) As Single
        Get
            Return CSng(GetVariable(eVarNameFlags.EcospaceRegionBiomassStart, iGroup))
        End Get

        Set(ByVal value As Single)
            SetVariable(eVarNameFlags.EcospaceRegionBiomassStart, value, iGroup)
        End Set
    End Property

    Public Property BiomassEnd(ByVal iGroup As Integer) As Single
        Get
            Return CSng(GetVariable(eVarNameFlags.EcospaceRegionBiomassEnd, iGroup))
        End Get

        Set(ByVal value As Single)
            SetVariable(eVarNameFlags.EcospaceRegionBiomassEnd, value, iGroup)
        End Set
    End Property


    Public Property CatchFleetGroupStart(ByVal iFleet As Integer, ByVal iGroup As Integer) As Single
        Get
            Try
                Return Me.m_CatchFleetGroup(0, iFleet, iGroup)
            Catch ex As Exception
                Debug.Assert(False, ex.Message)
                Return cCore.NULL_VALUE
            End Try

        End Get

        Set(ByVal value As Single)
            Try
                Me.m_CatchFleetGroup(0, iFleet, iGroup) = value
            Catch ex As Exception
                Debug.Assert(False, ex.Message)
            End Try
        End Set
    End Property


    Public Property CatchFleetGroupEnd(ByVal iFleet As Integer, ByVal iGroup As Integer) As Single
        Get
            Try
                Return Me.m_CatchFleetGroup(1, iFleet, iGroup)
            Catch ex As Exception
                Debug.Assert(False, ex.Message)
                Return cCore.NULL_VALUE
            End Try

        End Get

        Set(ByVal value As Single)
            Try
                Me.m_CatchFleetGroup(1, iFleet, iGroup) = value
            Catch ex As Exception
                Debug.Assert(False, ex.Message)
            End Try
        End Set

    End Property

    Public ReadOnly Property BiomassByTime(ByVal iGroup As Integer, ByVal iTime As Integer) As Single
        Get
            Try
                Return DirectCast(GetVariable(eVarNameFlags.EcospaceRegionBiomass, iGroup, iTime), Single)
            Catch ex As Exception
                Debug.Assert(False, ex.Message)
                Return cCore.NULL_VALUE
            End Try

        End Get

    End Property


    Public ReadOnly Property CatchFleetGroupTime(ByVal FleetIndex As Integer, ByVal GroupIndex As Integer, ByVal TimeIndex As Integer) As Single

        Get
            Try
                Return DirectCast(GetVariable(eVarNameFlags.EcospaceRegionFleetGroupCatch, FleetIndex, GroupIndex, TimeIndex), Single)
            Catch ex As Exception
                Debug.Assert(False, ex.Message)
                Return cCore.NULL_VALUE
            End Try

        End Get

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
