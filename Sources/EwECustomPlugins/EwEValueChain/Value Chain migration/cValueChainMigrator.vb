' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports Eii.ValueChain.Storage
Imports Eii.ValueChain.Storage.Models
Imports EwECore.Database
Imports EwECore.DataSources
Imports EwEUtils.Logging
Imports Microsoft.Extensions.Logging

Namespace ValueChainMigrator.LegacyData

#Disable Warning BC40008

    ''' ===========================================================================
    ''' <summary>
    ''' Migrate the Value Chain data from the OOP database to the new SQLite database.
    ''' This also renders the old Value Chain classes obsolete, as they are being 
    ''' replaced by the value chain package. This unfortunatly leaves a double object
    ''' model in the code base, but this is a temporary situation until the OOP database 
    ''' is removed from EwE.
    ''' </summary>
    ''' ===========================================================================
    Public Class cValueChainMigrator

        Private m_db As cEwEDatabase = Nothing
        Private ReadOnly m_logger As ILogger = LoggingContext.CreateLogger(Of cValueChainMigrator)()

        Public Sub New(db As IEwEDataSource)
            If (db Is Nothing) Then Return
            Dim conn As Object = db.Connection
            If (TypeOf (conn) Is cEwEDatabase) Then
                Me.m_db = DirectCast(conn, cEwEDatabase)
            End If
        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <returns></returns>
        ''' -----------------------------------------------------------------------
        Public Function Migrate(sqliteDBFilePath As String) As Boolean

            If (Me.m_db Is Nothing) Then Return False
            If Not Me.m_db.IsConnected() Then Return False

            Dim bSucces As Boolean = True
            Dim reader = New cLegacyOOPStorage(Me.m_db)

            Dim parm As Parameter
            Dim links As New List(Of Link)()
            Dim linkLandings As New List(Of LinkLandings)()
            Dim processingUnits As New List(Of ProcessingUnit)()
            Dim consumerUnits As New List(Of ConsumerUnit)()
            Dim wholesalerUnits As New List(Of WholesalerUnit)()
            Dim retailerUnits As New List(Of RetailerUnit)()
            Dim producerUnits As New List(Of ProducerUnit)()
            Dim distributionUnits As New List(Of DistributionUnit)()
            Dim flowDiagrams As New List(Of FlowDiagram)()
            Dim flowPositions As New List(Of FlowPosition)()

            Try
                parm = DirectCopy(Of Parameter)(reader.ReadObjects(GetType(LegacyData.cParameters))(0))
            Catch ex As Exception
                bSucces = False
                m_logger.LogError(ex, "ValueChain::LoadModel - reading objects")
            End Try

            Try
                For Each obj As cOOPStorable In reader.ReadObjects(GetType(LegacyData.cProducerUnit), False)
                    producerUnits.Add(DirectCopy(Of ProducerUnit)(obj))
                Next
                For Each obj As cOOPStorable In reader.ReadObjects(GetType(LegacyData.cProcessingUnit), False)
                    processingUnits.Add(DirectCopy(Of ProcessingUnit)(obj))
                Next
                For Each obj As cOOPStorable In reader.ReadObjects(GetType(LegacyData.cDistributionUnit), False)
                    distributionUnits.Add(DirectCopy(Of DistributionUnit)(obj))
                Next
                For Each obj As cOOPStorable In reader.ReadObjects(GetType(LegacyData.cWholesalerUnit), False)
                    wholesalerUnits.Add(DirectCopy(Of WholesalerUnit)(obj))
                Next
                For Each obj As cOOPStorable In reader.ReadObjects(GetType(LegacyData.cRetailerUnit), False)
                    retailerUnits.Add(DirectCopy(Of RetailerUnit)(obj))
                Next
                For Each obj As cOOPStorable In reader.ReadObjects(GetType(LegacyData.cConsumerUnit), False)
                    consumerUnits.Add(DirectCopy(Of ConsumerUnit)(obj))
                Next
                For Each obj As cOOPStorable In reader.ReadObjects(GetType(LegacyData.cLink), False)
                    links.Add(DirectCopy(Of Link)(obj))
                Next
                For Each obj As cOOPStorable In reader.ReadObjects(GetType(LegacyData.cLinkLandings), False)
                    links.Add(DirectCopy(Of LinkLandings)(obj))
                Next
                For Each obj As cOOPStorable In reader.ReadObjects(GetType(LegacyData.cFlowDiagram), False)
                    flowDiagrams.Add(DirectCopy(Of FlowDiagram)(obj))
                Next
                For Each obj As cOOPStorable In reader.ReadObjects(GetType(LegacyData.cFlowPosition), False)
                    flowPositions.Add(DirectCopy(Of FlowPosition)(obj))
                Next

            Catch ex As Exception
                bSucces = False
                m_logger.LogError(ex, "ValueChain::LoadModel - loading individual units")
            End Try

            Dim writer As New ValueChainStorageService()

            Try
                bSucces = writer.SaveValueChain(sqliteDBFilePath, parm, links, linkLandings, consumerUnits, processingUnits, wholesalerUnits, retailerUnits, producerUnits, distributionUnits, flowDiagrams, flowPositions)
            Catch ex As Exception
                bSucces = False
                m_logger.LogError(ex, "ValueChain::LoadModel - writing to new database")
            End Try

            If (bSucces) Then
                ' Remove OOP tables from the database
                Try
                    ' Me.m_db.RemoveTable("Parameters")
                Catch ex As Exception
                    bSucces = False
                    m_logger.LogError(ex, "ValueChain::LoadModel - removing OOP tables from database")
                End Try
            End If
            Return bSucces

        End Function

        Private Function DirectCopy(Of TDest As New)(src As cOOPStorable) As TDest

            If src Is Nothing Then Return Nothing

            Dim dst As New TDest()

            Dim srcProps = src.GetType().GetProperties(Reflection.BindingFlags.Instance Or Reflection.BindingFlags.Public)

            Dim dstProps = GetType(TDest).GetProperties(Reflection.BindingFlags.Instance Or Reflection.BindingFlags.Public) _
                .Where(Function(p) p.CanWrite AndAlso p.SetMethod IsNot Nothing AndAlso p.SetMethod.IsPublic AndAlso p.GetIndexParameters().Length = 0) _
                .ToDictionary(Function(p) p.Name, StringComparer.Ordinal)

            For Each srcProp In srcProps

                If Not srcProp.CanRead OrElse srcProp.GetMethod Is Nothing OrElse Not srcProp.GetMethod.IsPublic OrElse srcProp.GetIndexParameters().Length <> 0 Then
                    Continue For
                End If

                Dim dstProp As Reflection.PropertyInfo = Nothing

                If Not dstProps.TryGetValue(srcProp.Name, dstProp) Then
                    Continue For
                End If

                Dim objValue = srcProp.GetValue(src)

                If dstProp.PropertyType IsNot srcProp.PropertyType Then
                    If (TypeOf objValue IsNot ValueChainMigrator.LegacyData.cOOPStorable) Then
                        Continue For
                    End If
                    objValue = DirectCast(objValue, ValueChainMigrator.LegacyData.cOOPStorable).DBID
                End If

                dstProp.SetValue(dst, objValue)

            Next

            Return dst

        End Function

    End Class

#Enable Warning BC40008

End Namespace
