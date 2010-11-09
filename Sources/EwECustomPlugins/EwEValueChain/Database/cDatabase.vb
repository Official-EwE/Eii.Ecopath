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
            Return eDatasourceAccessType.Success
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
