'==============================================================================
'
' $Log: cEcospaceRegionSummary.vb,v $
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

    Private m_data(,,) As Single
    Private m_biomByTime(,) As Single

#Region "Constructor"

    Public Sub New(ByRef TheCore As cCore, ByVal iRegion As Integer)
        MyBase.New(TheCore)

        Dim val As cValue

        Me.DBID = iRegion '????
        Me.Index = iRegion
        Me.m_DataType = eDataTypes.EcospaceRegionResults
        'no validators
        'Biomass

        val = New cValueArray(eValueTypes.SingleArray, eVarNameFlags.EcospaceRegionBiomassStart, eStatusFlags.OK, eCoreCounterTypes.nGroups, AddressOf TheCore.GetCoreCounter)
        m_values.Add(val.varName, val)

        val = New cValueArray(eValueTypes.SingleArray, eVarNameFlags.EcospaceRegionBiomassEnd, eStatusFlags.OK, eCoreCounterTypes.nGroups, AddressOf TheCore.GetCoreCounter)
        m_values.Add(val.varName, val)

        Me.dimPrivateArrays()

        'Dim nGrps As Integer = TheCore.GetCoreCounter(eCoreCounterTypes.nGroups)
        'Dim nFlts As Integer = TheCore.GetCoreCounter(eCoreCounterTypes.nFleets)
        'Dim nTs As Integer = TheCore.GetCoreCounter(eCoreCounterTypes.nEcospaceTimeSteps)

        'ReDim m_data(1, nFlts, nGrps)
        'ReDim Me.m_biomByTime(nGrps, nTs)

        ''no validators
        ''Catch
        'val = New cValueArray(eValueTypes.SingleArray, eVarNameFlags.EcospaceRegionCatchStart, eStatusFlags.OK, eCoreCounterTypes.nFleets, AddressOf TheCore.getCoreCounter)
        'm_values.Add(val.varName, val)

        'val = New cValueArray(eValueTypes.SingleArray, eVarNameFlags.EcospaceRegionCatchEnd, eStatusFlags.OK, eCoreCounterTypes.nFleets, AddressOf TheCore.getCoreCounter)
        'm_values.Add(val.varName, val)


    End Sub

#End Region

#Region "Implementation of GetVariable() SetVariable() GetStatus() SetStatus()"

    Public Overrides Function GetVariable(ByVal varName As eVarNameFlags, Optional ByVal iFirstIndex As Integer = cCore.NULL_VALUE, Optional ByVal iSecondIndex As Integer = cCore.NULL_VALUE) As Object
        Try

            If iSecondIndex = cCore.NULL_VALUE Then
                Return MyBase.GetVariable(varName, iFirstIndex)
            End If

            Select Case varName
                Case eVarNameFlags.EcospaceRegionCatchStart
                    Return m_data(0, iFirstIndex, iSecondIndex)
                Case eVarNameFlags.EcospaceRegionCatchEnd
                    Return m_data(1, iFirstIndex, iSecondIndex)

                Case eVarNameFlags.EcospaceRegionBiomass
                    Return m_biomByTime(iFirstIndex, iSecondIndex)
            End Select
        Catch ex As Exception
            Debug.Assert(False, ex.Message)
        End Try

        Return cCore.NULL_VALUE

    End Function

    Public Overloads Function SetVariable(ByVal varName As eVarNameFlags, ByVal newValue As Single, ByVal iFirstIndex As Integer, ByVal iSecondIndex As Integer) As Boolean
        Try
            Select Case varName
                Case eVarNameFlags.EcospaceRegionCatchStart
                    m_data(0, iFirstIndex, iSecondIndex) = newValue
                Case eVarNameFlags.EcospaceRegionCatchEnd
                    m_data(1, iFirstIndex, iSecondIndex) = newValue

                Case eVarNameFlags.EcospaceRegionBiomass
                    m_biomByTime(iFirstIndex, iSecondIndex) = newValue

            End Select

            Return True

        Catch ex As Exception
            Debug.Assert(False, ex.Message)
            Return False
        End Try


    End Function

    Public Overloads Function GetStatus(ByVal varName As eVarNameFlags, ByVal iFleet As Integer, ByVal iGroup As Integer) As eStatusFlags
        Return eStatusFlags.OK 'Oh Yeah 
    End Function

    Public Overloads Function SetStatus(ByVal varName As eVarNameFlags, ByVal newValue As eStatusFlags, ByVal iFleet As Integer, ByVal iGroup As Integer) As Boolean
        Debug.Assert(False, "Not implemented yet.")
    End Function


    Friend Overrides Function Resize() As Boolean
        MyBase.Resize()

        dimPrivateArrays()

    End Function


    Private Sub dimPrivateArrays()
        Dim nGrps As Integer = Me.m_core.GetCoreCounter(eCoreCounterTypes.nGroups)
        Dim nFlts As Integer = Me.m_core.GetCoreCounter(eCoreCounterTypes.nFleets)
        Dim nTs As Integer = Me.m_core.GetCoreCounter(eCoreCounterTypes.nEcospaceTimeSteps)

        ReDim m_data(1, nFlts, nGrps)
        ReDim Me.m_biomByTime(nGrps, nTs)

    End Sub
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
                Return DirectCast(GetVariable(eVarNameFlags.EcospaceRegionCatchStart, iFleet, iGroup), Single)
            Catch ex As Exception
                Debug.Assert(False, ex.Message)
                Return cCore.NULL_VALUE
            End Try

        End Get

        Set(ByVal value As Single)
            Try
                SetVariable(eVarNameFlags.EcospaceRegionCatchStart, value, iFleet, iGroup)
            Catch ex As Exception
                Debug.Assert(False, ex.Message)
            End Try
        End Set
    End Property


    Public Property CatchFleetGroupEnd(ByVal iFleet As Integer, ByVal iGroup As Integer) As Single
        Get
            Try
                Return DirectCast(GetVariable(eVarNameFlags.EcospaceRegionCatchEnd, iFleet, iGroup), Single)
            Catch ex As Exception
                Debug.Assert(False, ex.Message)
                Return cCore.NULL_VALUE
            End Try

        End Get

        Set(ByVal value As Single)
            Try
                SetVariable(eVarNameFlags.EcospaceRegionCatchEnd, value, iFleet, iGroup)
            Catch ex As Exception
                Debug.Assert(False, ex.Message)
            End Try
        End Set

    End Property


    Public Property BiomassByTime(ByVal IGroup As Integer, ByVal iTime As Integer) As Single
        Get
            Try
                Return DirectCast(GetVariable(eVarNameFlags.EcospaceRegionBiomass, IGroup, iTime), Single)
            Catch ex As Exception
                Debug.Assert(False, ex.Message)
                Return cCore.NULL_VALUE
            End Try

        End Get

        Set(ByVal value As Single)
            Try
                SetVariable(eVarNameFlags.EcospaceRegionBiomass, value, IGroup, iTime)
            Catch ex As Exception
                Debug.Assert(False, ex.Message)
            End Try
        End Set

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
