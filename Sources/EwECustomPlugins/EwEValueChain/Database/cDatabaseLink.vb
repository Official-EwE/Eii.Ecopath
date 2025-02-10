' ===============================================================================
' This file is part of Ecopath with Ecosim (EwE)
'
' EwE is free software: you can redistribute it and/or modify it under the terms
' of the GNU General Public License version 2 as published by the Free Software 
' Foundation.
'
' EwE is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; 
' without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR 
' PURPOSE. See the GNU General Public License for more details.
'
' You should have received a copy of the GNU General Public License along with EwE.
' If not, see <http://www.gnu.org/licenses/gpl-2.0.html>. 
'
' Copyright 1991- 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'

#Region " Imports "

Option Strict On
Imports System.Reflection
Imports EwEUtils.Core
Imports EwEUtils.Database
Imports EwEUtils.Database.cEwEDatabase

#End Region ' Imports

''' ===========================================================================
''' <summary>
''' 
''' </summary>
''' ===========================================================================
Public Class cDatabaseLink

    Private m_db As cEwEDatabase = Nothing

#Region " Load "

    Public Function Attach(conn As Object) As eDatasourceAccessType

        If (TypeOf conn Is cEwEDatabase) Then
            Me.m_db = DirectCast(conn, cEwEDatabase)
            Me.m_db.OOPEnabled = True
            Return eDatasourceAccessType.Opened
        End If

        Me.m_db = Nothing
        Return eDatasourceAccessType.Failed_Unknown

    End Function

    Public Sub Detach()
        Me.m_db = Nothing
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="data"></param>
    ''' <returns></returns>
    ''' -----------------------------------------------------------------------
    Public Function LoadModel(data As cData) As Boolean

        If Not Me.IsConnected() Then Return False

        Dim aObjects As cOOPStorable() = Nothing
        Dim bSucces As Boolean = True

        Me.m_db.OOPFlushObjectCache()
        data.Clear()

        Try
            aObjects = Me.m_db.ReadObjects(GetType(cParameters))
        Catch ex As Exception
            bSucces = False
            cLog.Write(ex, "ValueChain::LoadModel - reading objects")
        End Try

        If (aObjects.Length = 0) Then
            data.AddParameters(New cParameters())
            ' If no parameters found there is little need to continue...
            Return True
        Else
            data.AddParameters(DirectCast(aObjects(0), cParameters))
        End If

        Try

            ' Load default units
            aObjects = Me.m_db.ReadObjects(GetType(cProducerUnitDefault), False)
            For Each obj As cOOPStorable In aObjects : data.AddUnitDefault(DirectCast(obj, cUnit)) : Next
            aObjects = Me.m_db.ReadObjects(GetType(cProcessingUnitDefault), False)
            For Each obj As cOOPStorable In aObjects : data.AddUnitDefault(DirectCast(obj, cUnit)) : Next
            aObjects = Me.m_db.ReadObjects(GetType(cDistributionUnitDefault), False)
            For Each obj As cOOPStorable In aObjects : data.AddUnitDefault(DirectCast(obj, cUnit)) : Next
            aObjects = Me.m_db.ReadObjects(GetType(cWholesalerUnitDefault), False)
            For Each obj As cOOPStorable In aObjects : data.AddUnitDefault(DirectCast(obj, cUnit)) : Next
            aObjects = Me.m_db.ReadObjects(GetType(cRetailerUnitDefault), False)
            For Each obj As cOOPStorable In aObjects : data.AddUnitDefault(DirectCast(obj, cUnit)) : Next
            aObjects = Me.m_db.ReadObjects(GetType(cConsumerUnitDefault), False)
            For Each obj As cOOPStorable In aObjects : data.AddUnitDefault(DirectCast(obj, cUnit)) : Next

            ' Load default links
            aObjects = Me.m_db.ReadObjects(GetType(cLinkDefault), False)
            For Each obj As cOOPStorable In aObjects : data.AddLinkDefault(DirectCast(obj, cLinkDefault)) : Next

            ' Load units
            aObjects = Me.m_db.ReadObjects(GetType(cProducerUnit), False)
            For Each obj As cOOPStorable In aObjects
                data.AddUnit(DirectCast(obj, cUnit))
            Next
            aObjects = Me.m_db.ReadObjects(GetType(cProcessingUnit), False)
            For Each obj As cOOPStorable In aObjects
                data.AddUnit(DirectCast(obj, cUnit))
            Next
            aObjects = Me.m_db.ReadObjects(GetType(cDistributionUnit), False)
            For Each obj As cOOPStorable In aObjects
                data.AddUnit(DirectCast(obj, cUnit))
            Next
            aObjects = Me.m_db.ReadObjects(GetType(cWholesalerUnit), False)
            For Each obj As cOOPStorable In aObjects
                data.AddUnit(DirectCast(obj, cUnit))
            Next
            aObjects = Me.m_db.ReadObjects(GetType(cRetailerUnit), False)
            For Each obj As cOOPStorable In aObjects
                data.AddUnit(DirectCast(obj, cUnit))
            Next
            aObjects = Me.m_db.ReadObjects(GetType(cConsumerUnit), False)
            For Each obj As cOOPStorable In aObjects
                data.AddUnit(DirectCast(obj, cUnit))
            Next

            ' Load imported (new) links first
            aObjects = Me.m_db.ReadObjects(GetType(cLinkLandings), False)
            For Each obj As cOOPStorable In aObjects
                Dim ll As cLinkLandings = DirectCast(obj, cLinkLandings)
                ll.Group = data.FindEcopathGroupByID(ll.EcopathGroupID)
                If ll.Group IsNot Nothing Then
                    data.AddLink(ll)
                Else
                    ll = ll
                End If
            Next
            ' Import old-fashioned links after and convert them to cLinkLandings
            aObjects = Me.m_db.ReadObjects(GetType(cLink), False)
            For Each obj As cOOPStorable In aObjects
                ' Is old-fashioned producer link?
                Dim l As cLink = DirectCast(obj, cLink)
                Dim bError As Boolean = False
                If (l.Source.GetType Is GetType(cProducerUnit)) Then
                    ' #Yes: dump it
                    l.Source.RemoveLink(l)
                    Try
                        For iGroup As Integer = 1 To data.Core.nGroups
                            Dim ll As cLinkLandings = data.CreateLandingsLink(DirectCast(l.Source, cProducerUnit), l.Target, data.Core.EcopathGroupInputs(iGroup), bError, True)
                            If (ll IsNot Nothing) Then
                                ll.BiomassRatio = l.BiomassRatio
                                ll.ValueRatio = l.ValueRatio
                                ll.ValuePerTon = l.ValuePerTon
                            End If
                        Next
                    Catch ex As Exception

                    End Try
                Else
                    ' #No: use it
                    data.AddLink(DirectCast(obj, cLink))
                End If
            Next


            ' Load flow diagrams
            aObjects = Me.m_db.ReadObjects(GetType(cFlowDiagram), False)
            For Each obj As cOOPStorable In aObjects : data.CreateFlowDiagram(DirectCast(obj, cFlowDiagram)) : Next

            ' Load flow positions
            aObjects = Me.m_db.ReadObjects(GetType(cFlowPosition), False)
            For Each obj As cOOPStorable In aObjects
                data.AddFlowPosition(DirectCast(obj, cFlowPosition))
            Next

        Catch ex As Exception
            bSucces = False
            cLog.Write(ex, "ValueChain::LoadModel - loading individual units")
        End Try

        Return bSucces

    End Function

#End Region ' Load

#Region " Save "

    Public Function SaveModel(data As cData) As Boolean

        If Not Me.IsConnected() Then Return False

        Dim bSucces As Boolean = True
        Dim ass As Assembly = Assembly.GetAssembly(GetType(cUnit))

        Me.m_db.OOPFlushObjectCache()
        Me.m_db.OOPFlushSchemaCache()

        ' JS 17Nov11: Use OOP transaction to minimize time on getting and releasing adapters
        If Me.m_db.OOPBeginTransaction(ass, True) Then

            Try
                ' Store model parameters
                bSucces = bSucces And Me.m_db.WriteObject(data.Parameters)

                ' Store default units
                bSucces = bSucces And Me.m_db.WriteObject(data.GetUnitDefault(cUnitFactory.eUnitType.Producer))
                bSucces = bSucces And Me.m_db.WriteObject(data.GetUnitDefault(cUnitFactory.eUnitType.Processing))
                bSucces = bSucces And Me.m_db.WriteObject(data.GetUnitDefault(cUnitFactory.eUnitType.Distribution))
                bSucces = bSucces And Me.m_db.WriteObject(data.GetUnitDefault(cUnitFactory.eUnitType.Wholesaler))
                bSucces = bSucces And Me.m_db.WriteObject(data.GetUnitDefault(cUnitFactory.eUnitType.Retailer))
                bSucces = bSucces And Me.m_db.WriteObject(data.GetUnitDefault(cUnitFactory.eUnitType.Consumer))

                ' Store units
                For i As Integer = 0 To data.UnitCount - 1
                    bSucces = bSucces And Me.m_db.WriteObject(data.Unit(i))
                Next

                ' Store flow diagrams
                For i As Integer = 0 To data.FlowDiagramCount - 1
                    bSucces = bSucces And Me.m_db.WriteObject(data.FlowDiagram(i))
                Next i

            Catch ex As Exception
                bSucces = False
            End Try

            Try

                ' Store default links
                bSucces = bSucces And Me.m_db.WriteObject(data.GetLinkDefault(cLinkFactory.eLinkType.ProducerToProcessing))
                bSucces = bSucces And Me.m_db.WriteObject(data.GetLinkDefault(cLinkFactory.eLinkType.ProcessingToDistribution))
                bSucces = bSucces And Me.m_db.WriteObject(data.GetLinkDefault(cLinkFactory.eLinkType.DistributionToWholeseller))
                bSucces = bSucces And Me.m_db.WriteObject(data.GetLinkDefault(cLinkFactory.eLinkType.WholesellerToRetailer))
                bSucces = bSucces And Me.m_db.WriteObject(data.GetLinkDefault(cLinkFactory.eLinkType.RetailerToConsumer))

                ' Store links
                For i As Integer = 0 To data.LinkCount - 1
                    bSucces = bSucces And Me.m_db.WriteObject(data.Link(i))
                Next

            Catch ex As Exception
                bSucces = False
            End Try

            ' Store flow positions
            For i As Integer = 0 To data.FlowPositionCount - 1
                bSucces = bSucces And Me.m_db.WriteObject(data.FlowPosition(i))
            Next

            If bSucces Then
                bSucces = Me.m_db.OOPCommitTransaction(True)
            Else
                Me.m_db.OOPRollbackTransaction()
            End If

        End If

        Return bSucces
    End Function

#End Region ' Save

#Region " Pass through "

    Public Function IsConnected() As Boolean
        Return (Me.m_db IsNot Nothing)
    End Function

    Public Sub BeginTransaction()
        If Me.IsConnected() Then
            Me.m_db.BeginTransaction()
        End If
    End Sub

    Public Sub CommitTransaction(bCommit As Boolean)
        If Me.IsConnected() Then
            Me.m_db.CommitTransaction(bCommit)
        End If
    End Sub

    Public Sub RollbackTransaction()
        If Me.IsConnected() Then
            Me.m_db.RollbackTransaction()
        End If
    End Sub

    Public Function DeleteObject(obj As cOOPStorable) As Boolean
        If Not Me.IsConnected() Then Return False
        Return Me.m_db.DeleteObject(obj)
    End Function

#End Region ' Pass through

End Class
