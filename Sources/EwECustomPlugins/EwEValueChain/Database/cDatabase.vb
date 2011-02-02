#Region " Imports "

Option Strict On
Imports EwEUtils.Database
Imports EwEUtils.Core
Imports System.IO
Imports System.Reflection

#End Region ' Imports

''' ===========================================================================
''' <summary>
''' 
''' </summary>
''' ===========================================================================
Public Class cDatabase
    Inherits EwECore.Database.cEwEAccessDatabase

#Region " Load "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Overridden to update the database when opened
    ''' </summary>
    ''' <param name="strDatabase"></param>
    ''' <param name="databaseType">Type to use to open the database. Set this
    ''' to 'NotSet' to auto-detect the database type.</param>
    ''' <returns>True if connected succesfully.</returns>
    ''' -------------------------------------------------------------------
    Public Overrides Function Open(ByVal strDatabase As String, _
                                   Optional ByVal databaseType As eDataSourceTypes = eDataSourceTypes.NotSet, _
                                   Optional ByVal bReadOnly As Boolean = False) As eDatasourceAccessType

        Dim result As eDatasourceAccessType = MyBase.Open(strDatabase, databaseType, bReadOnly)
        If result = eDatasourceAccessType.Opened Then
            Me.OOPEnabled = True
            Me.UpdateDatabase()
        End If
        Return result

    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="data"></param>
    ''' <returns></returns>
    ''' -----------------------------------------------------------------------
    Public Function LoadModel(ByVal data As cData) As Boolean

        Dim aObjects As cOOPStorable() = Nothing
        Dim bSucces As Boolean = True

        Me.OOPFlushObjectCache()

        Try
            data.Clear()

            aObjects = Me.ReadObjects(GetType(cParameters))
            If aObjects.Length = 0 Then
                data.AddParameters(New cParameters())
            Else
                data.AddParameters(DirectCast(aObjects(0), cParameters))
            End If

            ' Load default units
            aObjects = Me.ReadObjects(GetType(cProducerUnitDefault), False)
            For Each obj As cOOPStorable In aObjects : data.AddUnitDefault(DirectCast(obj, cUnit)) : Next
            aObjects = Me.ReadObjects(GetType(cProcessingUnitDefault), False)
            For Each obj As cOOPStorable In aObjects : data.AddUnitDefault(DirectCast(obj, cUnit)) : Next
            aObjects = Me.ReadObjects(GetType(cDistributionUnitDefault), False)
            For Each obj As cOOPStorable In aObjects : data.AddUnitDefault(DirectCast(obj, cUnit)) : Next
            aObjects = Me.ReadObjects(GetType(cMarketUnitDefault), False)
            For Each obj As cOOPStorable In aObjects : data.AddUnitDefault(DirectCast(obj, cUnit)) : Next
            aObjects = Me.ReadObjects(GetType(cConsumerUnitDefault), False)
            For Each obj As cOOPStorable In aObjects : data.AddUnitDefault(DirectCast(obj, cUnit)) : Next

            ' Load default links
            aObjects = Me.ReadObjects(GetType(cLinkDefault), False)
            For Each obj As cOOPStorable In aObjects : data.AddLinkDefault(DirectCast(obj, cLinkDefault)) : Next

            ' Load units
            aObjects = Me.ReadObjects(GetType(cProducerUnit), False)
            For Each obj As cOOPStorable In aObjects
                data.AddUnit(DirectCast(obj, cUnit))
            Next
            aObjects = Me.ReadObjects(GetType(cProcessingUnit), False)
            For Each obj As cOOPStorable In aObjects
                data.AddUnit(DirectCast(obj, cUnit))
            Next
            aObjects = Me.ReadObjects(GetType(cDistributionUnit), False)
            For Each obj As cOOPStorable In aObjects
                data.AddUnit(DirectCast(obj, cUnit))
            Next
            aObjects = Me.ReadObjects(GetType(cMarketUnit), False)
            For Each obj As cOOPStorable In aObjects
                data.AddUnit(DirectCast(obj, cUnit))
            Next
            aObjects = Me.ReadObjects(GetType(cConsumerUnit), False)
            For Each obj As cOOPStorable In aObjects
                data.AddUnit(DirectCast(obj, cUnit))
            Next

            ' Load links
            aObjects = Me.ReadObjects(GetType(cLink), False)
            For Each obj As cOOPStorable In aObjects
                data.AddLink(DirectCast(obj, cLink))
            Next

            ' Load flow diagrams
            aObjects = Me.ReadObjects(GetType(cFlowDiagram), False)
            For Each obj As cOOPStorable In aObjects : data.CreateFlowDiagram(DirectCast(obj, cFlowDiagram)) : Next

            ' Load flow positions
            aObjects = Me.ReadObjects(GetType(cFlowPosition), False)
            For Each obj As cOOPStorable In aObjects : data.AddFlowPosition(DirectCast(obj, cFlowPosition)) : Next

        Catch ex As Exception
            bSucces = False
        End Try

        Return True

    End Function

#End Region ' Load

#Region " Save "

    Public Function SaveModel(ByVal data As cData) As Boolean

        Dim bSucces As Boolean = True

        Me.OOPFlushObjectCache()
        Me.OOPFlushSchemaCache()

        ' JS 14apr09: save logic broken up in separate transactions. Each transaction saves
        '             a set of unrelated objects (objects that are not linked). This 
        '
        If Me.BeginTransaction() Then

            Try
                ' Store model parameters
                bSucces = bSucces And Me.WriteObject(data.Parameters)

                ' Store default units
                bSucces = bSucces And Me.WriteObject(data.GetUnitDefault(cUnitFactory.eUnitType.Producer))
                bSucces = bSucces And Me.WriteObject(data.GetUnitDefault(cUnitFactory.eUnitType.Processing))
                bSucces = bSucces And Me.WriteObject(data.GetUnitDefault(cUnitFactory.eUnitType.Distribution))
                bSucces = bSucces And Me.WriteObject(data.GetUnitDefault(cUnitFactory.eUnitType.Market))
                bSucces = bSucces And Me.WriteObject(data.GetUnitDefault(cUnitFactory.eUnitType.Consumer))

                ' Store units
                For i As Integer = 0 To data.UnitCount - 1
                    bSucces = bSucces And Me.WriteObject(data.Unit(i))
                Next

                ' Store flow diagrams
                For i As Integer = 0 To data.FlowDiagramCount - 1
                    bSucces = bSucces And Me.WriteObject(data.FlowDiagram(i))
                Next i

            Catch ex As Exception
                bSucces = False
            End Try

            If bSucces Then
                bSucces = Me.CommitTransaction(True)
            Else
                Me.RollbackTransaction()
            End If

        End If

        If Me.BeginTransaction() Then

            Try

                ' Store default links
                bSucces = bSucces And Me.WriteObject(data.GetLinkDefault(cLinkFactory.eLinkType.ProducerToProcessing))
                bSucces = bSucces And Me.WriteObject(data.GetLinkDefault(cLinkFactory.eLinkType.ProcessingToDistribution))
                bSucces = bSucces And Me.WriteObject(data.GetLinkDefault(cLinkFactory.eLinkType.DistributionToMarket))
                bSucces = bSucces And Me.WriteObject(data.GetLinkDefault(cLinkFactory.eLinkType.MarketToConsumer))

                ' Store links
                For i As Integer = 0 To data.LinkCount - 1
                    bSucces = bSucces And Me.WriteObject(data.Link(i))
                Next

            Catch ex As Exception
                bSucces = False
            End Try

            If bSucces Then
                bSucces = Me.CommitTransaction(True)
            Else
                Me.RollbackTransaction()
            End If
        End If

        If Me.BeginTransaction() Then

            Try

            Catch ex As Exception
                bSucces = False
            End Try

            ' Store flow positions
            For i As Integer = 0 To data.FlowPositionCount - 1
                bSucces = bSucces And Me.WriteObject(data.FlowPosition(i))
            Next

            If bSucces Then
                bSucces = Me.CommitTransaction(True)
            Else
                Me.RollbackTransaction()
            End If

        End If

        Return bSucces
    End Function

#End Region ' Save

#Region " Updates "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Run consecutive updates to bring the database schema up to date.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Private Function UpdateDatabase() As Boolean

        Dim sVersion As Single = Me.GetVersion()
        Dim bSucces As Boolean = True

        Me.BeginTransaction()

        If bSucces Then
            Me.CommitTransaction()
        Else
            Me.RollbackTransaction()
        End If

        Return bSucces
    End Function

#End Region ' Updates

End Class
