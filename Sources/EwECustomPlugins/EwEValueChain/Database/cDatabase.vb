'==============================================================================
'
' $Log: cDatabase.vb,v $
' Revision 1.3  2009/04/14 20:38:06  jeroens
' Save split in grouped transactions of saving unrelated objects
'
' Revision 1.2  2009/04/13 17:44:14  jeroens
' Parameters read, stored
'
' Revision 1.1  2009/03/13 15:54:46  jeroens
' Ecost -> ValueChain
'
' Revision 1.4  2009/03/08 20:45:46  jeroens
' Removed ref to obsolete OOPEndWrite
'
' Revision 1.3  2009/01/30 17:04:27  jeroens
' Moved cEwEDatabase.eAccessTypes to shared enums
'
' Revision 1.2  2009/01/05 13:03:51  jeroens
' All diagrams saved
'
' Revision 1.1  2009/01/05 11:54:15  jeroens
' Renamed
'
' Revision 1.25  2008/12/20 06:10:00  jeroens
' no message
'
' Revision 1.24  2008/12/11 18:56:25  jeroens
' Reorganized links inheritance
'
' Revision 1.23  2008/12/10 02:16:15  jeroens
' Open, Create can force database type
'
' Revision 1.22  2008/08/16 17:47:29  jeroens
' Updated to changed EwE6 interfaces
'
' Revision 1.21  2008/06/03 12:10:05  jeroens
' Removed remaining refs to Metier
'
' Revision 1.20  2008/04/25 03:00:18  jeroens
' Fixed link reading error
'
' Revision 1.19  2008/04/18 01:18:45  jeroens
' Transactions started here, no longer on OOP logic at baseclass DB
'
' Revision 1.18  2008/04/17 02:58:12  jeroens
' Separated unit, unitdefault reading
'
' Revision 1.17  2008/04/15 18:55:42  jeroens
' Saves and loads defaults (almost)
'
' Revision 1.16  2008/04/14 00:13:29  jeroens
' Removed dead code
'
' Revision 1.15  2008/04/13 21:25:37  jeroens
' Do not reload model when loading flow
'
' Revision 1.14  2008/04/13 19:36:27  jeroens
' Fixed flow read issue
'
' Revision 1.13  2008/04/13 18:39:41  jeroens
' Added FlowPanel saving
'
' Revision 1.12  2008/04/12 21:21:00  jeroens
' Switching to OOP!
'
' Revision 1.11  2008/04/11 23:56:17  jeroens
' Units can be deleted again :p
'
' Revision 1.10  2008/04/11 19:41:16  jeroens
' Renamed cOutputLink to cLink
'
' Revision 1.9  2008/04/11 16:09:51  jeroens
' Removed cFishUnit
' Renamed cProductionUnit to cMetierUnit
'
' Revision 1.8  2008/04/10 22:03:49  jeroens
' Preparing OutputLinks for OOP storage
'
' Revision 1.7  2008/04/09 20:07:43  jeroens
' It's working
'
' Revision 1.6  2008/04/09 18:29:44  villyc
' more supply removal
'
' Revision 1.5  2008/04/09 17:56:57  jeroens
' cUnit replaced cEconomicUnit as base class
'
' Revision 1.4  2008/04/09 01:10:11  villyc
' Added supply and consumer units, most related updates done, only known problem is on flowchart where 'arrange' causes the two new units to be displayed one position down from other units. Also added more attributes in master unit
'
' Revision 1.3  2008/03/17 23:28:57  jeroens
' Fixed conflict on External field name
' Fixed link read order issue
'
' Revision 1.2  2008/03/16 22:10:56  jeroens
' Added Update mechanism
'
' Revision 1.1  2008/03/13 06:26:22  jeroens
' Initial version
'
'==============================================================================

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
                                   Optional ByVal databaseType As eDataSourceTypes = eDataSourceTypes.NotSet) As eDatasourceAccessType

        Dim result As eDatasourceAccessType = MyBase.Open(strDatabase, databaseType)
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

#Region " Modify "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Create a new Value Chain database
    ''' </summary>
    ''' <param name="strDatabase">Name of the database to create.</param>
    ''' <param name="strModelName">Name of the model the database is created for.</param>
    ''' <param name="bOverwrite">Flag to indicate whether the file on disk should be overwritten.</param>
    ''' <returns>True if succesful.</returns>
    ''' -----------------------------------------------------------------------
    Public Overrides Function Create(ByVal strDatabase As String, ByVal strModelName As String, _
                                     Optional ByVal bOverwrite As Boolean = False, _
                                     Optional ByVal databaseType As eDataSourceTypes = eDataSourceTypes.NotSet) As eDatasourceAccessType
        ' Databasetype ignored
        If cDatabase.SaveDatabaseToFile(strDatabase, bOverwrite) Then
            Return eDatasourceAccessType.Created
        Else
            Return eDatasourceAccessType.Failed_Unknown
        End If

    End Function

#End Region ' Modify

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

#Region " Helpers "

    ''' <summary>Name of the current namespace. Cached to provide quick access</summary>
    Private Shared CurrentNamespace As String = Assembly.GetExecutingAssembly().GetName().Name.ToString()

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Saves an embedded resource to a file
    ''' </summary>
    ''' <param name="strFileName">The name of the file to save the resource to</param>
    ''' <param name="bOverwrite">States whether an existing file is allowed to be overwritten</param>
    ''' <returns>True if succesful</returns>
    ''' -----------------------------------------------------------------------
    Private Shared Function SaveDatabaseToFile(ByVal strFileName As String, _
            Optional ByVal bOverwrite As Boolean = False) As Boolean

        Dim strResourceName As String = "template.ewevcmdb"
        Dim sResource As Stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(CurrentNamespace & "." & strResourceName)
        Dim sFile As FileStream = Nothing
        Dim nBufLen As Integer = 256
        Dim byBuffer(nBufLen) As Byte
        Dim nBytesRead As Integer = 0

        ' Pre
        Debug.Assert(Not String.IsNullOrEmpty(strFileName), "Required target file name missing")
        Debug.Assert(sResource IsNot Nothing, String.Format("Resource {0} not found in {1}", strResourceName, CurrentNamespace))

        ' Work with full path
        strFileName = Path.GetFullPath(strFileName)

        Try
            If (bOverwrite) Then
                ' Create the file, overwriting any existing file with the same path
                sFile = New FileStream(strFileName, FileMode.Create, FileAccess.Write)
            Else
                ' Create the file but do not overwrite
                sFile = New FileStream(strFileName, FileMode.CreateNew, FileAccess.Write)
            End If
        Catch ex As Exception
            ' Just so you know
            Debug.Print("Unable to create or overwrite file {0}", strFileName)
            ' Report failure
            Return False
        End Try

        ' Copy embedded resource to file
        nBytesRead = sResource.Read(byBuffer, 0, nBufLen)
        While (nBytesRead > 0)
            sFile.Write(byBuffer, 0, nBytesRead)
            nBytesRead = sResource.Read(byBuffer, 0, nBufLen)
        End While
        ' Done
        sFile.Close()
        Return True

    End Function

#End Region ' Helpers

End Class
