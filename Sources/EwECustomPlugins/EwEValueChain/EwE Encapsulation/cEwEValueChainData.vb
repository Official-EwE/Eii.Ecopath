Imports EwECore
Imports ValueChain

Public Class cEwEValueChainData
    Inherits cValueChainData

    Public ReadOnly Property Core As cCore

    Public Sub New(core As cCore)
        Me.Core = core
    End Sub

    Public Sub Close()
        ' TODO_JS: figure out what was done here previously and if it is still needed
        Console.WriteLine("-click-")
    End Sub


#Region " EwE mappings"

    Public Function EwEItemToID(item As cCoreInputOutputBase) As String
        If (item Is Nothing) Then Return String.Empty
        If (TypeOf item Is cEcoPathGroupInput) Then Return EwEGroupToID(item)
        Return EwEFleetToID(item)
    End Function

    Public Function EwEGroupToID(grp As cCoreInputOutputBase) As String
        If (grp Is Nothing) Then Return String.Empty
        Return EwEiGroupToID(grp.Index)
    End Function

    Public Function EwEiGroupToID(iGroup As Integer) As String
        ' Use the same logic. Whah!
        Dim ds = Me.Core.EcopathDataStructures
        If (iGroup <= 0) Then Return String.Empty
        Return CStr(ds.GroupDBID(iGroup))
    End Function

    Public Function EwEFleetToID(fleet As cCoreInputOutputBase) As String
        If (fleet Is Nothing) Then Return String.Empty
        Return EwEiFleetToID(fleet.Index)
    End Function

    Public Function EwEiFleetToID(iFleet As Integer) As String
        ' Use the same logic. Whah!
        Dim ds = Me.Core.EcopathDataStructures
        If (iFleet <= 0) Then Return String.Empty
        Return CStr(ds.FleetDBID(iFleet))
    End Function

    Public Function IDtoEwEiGroup(id As String) As Integer
        If (String.IsNullOrEmpty(id)) Then Return Nothing
        Dim ds = Me.Core.EcopathDataStructures
        Dim iDBID As Integer = CInt(id)
        Return Array.IndexOf(ds.GroupDBID, iDBID)
    End Function

    Public Function IDtoEwEGroup(id As String) As cEcoPathGroupInput
        Dim iGroup As Integer = IDtoEwEiGroup(id)
        If (iGroup < 1) Then Return Nothing
        Return Me.Core.EcopathGroupInputs(iGroup)
    End Function

    Public Function IDtoEwEiFleet(id As String) As Integer
        If (String.IsNullOrEmpty(id)) Then Return Nothing
        Dim ds = Me.Core.EcopathDataStructures
        Dim iDBID As Integer = CInt(id)
        Return Array.IndexOf(ds.FleetDBID, iDBID)
    End Function

    Public Function IDtoEwEFleet(id As String) As cEcopathFleetInput
        Dim iFleet As Integer = IDtoEwEiFleet(id)
        If (iFleet < 1) Then Return Nothing
        Return Me.Core.EcopathFleetInputs(iFleet)
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Return all units that operate (directly or indirectly) onto a given 
    ''' fleet and/or group.
    ''' </summary>
    ''' <param name="item"></param>
    ''' <returns></returns>
    ''' -----------------------------------------------------------------------
    Public Overloads Function GetUnits(item As cCoreInputOutputBase) As cUnit()

        Dim lUnits As New List(Of cUnit)
        Dim pu As cProducerUnit = Nothing
        Dim bUseUnit As Boolean = False

        If (item Is Nothing) Then Return Me.GetUnits(cUnitFactory.eUnitType.All)

        ' * Determine for all producers whether the fleet and group filter matches
        ' * For all matching producers add all related units in the flow

        ' Iterate over producers
        For Each unit As cUnit In Me.GetUnits(cUnitFactory.eUnitType.Producer)
            ' Get producer unit
            pu = DirectCast(unit, cProducerUnit)

            ' Filtering by fleet?
            If (TypeOf item Is cEcopathFleetInput) Then
                ' #Yes: include producer if it uses this fleet
                bUseUnit = (pu.GearCode = Me.EwEFleetToID(item))
            Else
                ' #Yes: include producer if its fleet lands or discards a group
                Dim fleet As cEcopathFleetInput = Me.IDtoEwEFleet(pu.GearCode)
                If (fleet IsNot Nothing) Then
                    bUseUnit = (fleet.Landings(item.Index) > 0) Or (fleet.Discards(item.Index) > 0)
                End If
            End If


            If bUseUnit Then
                ' Add producer
                lUnits.Add(pu)
                ' Get all flow units linked to this producer
                Me.GetTargetUnits(pu, lUnits)
            End If

        Next unit

        Return lUnits.ToArray()

    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get all units that serve as source units to a given unit.
    ''' </summary>
    ''' <param name="unit">The unit to test incoming links for.</param>
    ''' <param name="lUnits">The list that will receive the linked units.</param>
    ''' -----------------------------------------------------------------------
    Private Sub GetSourceUnits(unit As cUnit, lUnits As List(Of cUnit))
        Dim unitSource As cUnit = Nothing
        For iLink As Integer = 0 To unit.LinkInCount - 1
            unitSource = unit.LinkIn(iLink).Source
            If lUnits.IndexOf(unitSource) = -1 Then
                lUnits.Add(unitSource)
                Me.GetSourceUnits(unitSource, lUnits)
            End If
        Next
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get all units that link out of a given unit
    ''' </summary>
    ''' <param name="unit">The unit to test outgoing links for.</param>
    ''' <param name="lUnits">The list that will receive the linked units.</param>
    ''' -----------------------------------------------------------------------
    Private Sub GetTargetUnits(unit As cUnit, lUnits As List(Of cUnit))
        Dim unitTarget As cUnit = Nothing
        For iLink As Integer = 0 To unit.LinkOutCount - 1
            unitTarget = unit.LinkOut(iLink).Target
            If lUnits.IndexOf(unitTarget) = -1 Then
                lUnits.Add(unitTarget)
                Me.GetTargetUnits(unitTarget, lUnits)
            End If
        Next
    End Sub

#End Region ' EwE mappings

End Class
