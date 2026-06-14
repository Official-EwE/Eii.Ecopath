' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports System.IO
Imports EwECore.Database
Imports EwEUtils.NetUtilities
Imports EwEUtils.SystemUtilities

Namespace DataSources

    ''' =======================================================================
    ''' <summary>
    ''' Factory for creating data sources
    ''' </summary>
    ''' =======================================================================
    Public Class cDataSourceFactory

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Returns an EwE <see cref="eDataSourceTypes">datasource type</see> that
        ''' will be able to interact with the provided file name.
        ''' </summary>
        ''' <param name="strFile">Name of the file.</param>
        ''' <returns>A <see cref="eDataSourceTypes">datasource type</see>
        ''' indicating what type of EwE datasource will be able to interact with
        ''' the provided file name.</returns>
        ''' -------------------------------------------------------------------
        Public Shared Function GetSupportedType(strFile As String) As eDataSourceTypes

            Select Case Path.GetExtension(strFile).ToLower

                Case ".eii"
                    Return eDataSourceTypes.EII

                Case ".accdb", ".eweaccdb"
                    ' check if alongside this file there is a .sqlite file
                    Dim dir As String = Path.GetDirectoryName(strFile)
                    Dim baseName As String = Path.GetFileNameWithoutExtension(strFile)

                    Dim sqliteFile As String = Path.Combine(dir, baseName & ".sqlite")
                    ' Check if sqlite file is present,
                    '   but also check if WebSocket server is running, needed for Access-Sqlite data source comparison,
                    '   otherwise it will be a waist of resources
                    If File.Exists(sqliteFile) Then             ' Rik disabled And cWebsocketHelper.IsRunning() Then
                        Return eDataSourceTypes.AccessVsSqlite
                    End If

                    Return eDataSourceTypes.Access2007

                Case ".mdb", ".ewemdb"
                    If cSystemUtils.Is64BitProcess() Then
                        Return eDataSourceTypes.Access2007
                    Else
                        Return eDataSourceTypes.Access2003
                    End If
                Case ".sqlite"
                    Return eDataSourceTypes.Sqlite
                Case ".eiixml"
                    Return eDataSourceTypes.EIIXML

            End Select

            ' Explore URL protocols
            Dim i As Integer = strFile.IndexOf(":"c)

            ' Is probably a URL protocol?
            If (i > 0) Then
                Select Case strFile.Substring(0, i)
                    Case "ewe-ecobase"
                        Return eDataSourceTypes.EcoBase
                End Select
            End If

            Return eDataSourceTypes.NotSet

        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Returns the default extension for a given <see cref="eDataSourceTypes">datasource type</see>.
        ''' </summary>
        ''' <param name="dst">The <see cref="eDataSourceTypes">datasource type</see> to query.</param>
        ''' <returns>A string providing a file extension, or an empty string if
        ''' the given datasource type is not supported.</returns>
        ''' -------------------------------------------------------------------
        Public Shared Function GetDefaultExtension(dst As eDataSourceTypes) As String
            Select Case dst
                Case eDataSourceTypes.Access2003 : Return ".ewemdb"
                Case eDataSourceTypes.EII : Return ".eii"
                Case eDataSourceTypes.Access2007, eDataSourceTypes.AccessVsSqlite : Return ".eweaccdb"
                Case eDataSourceTypes.EIIXML : Return ".eiixml"
            End Select
            Return ""
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Returns the compatibility of a given database with the current code.
        ''' </summary>
        ''' <param name="strDatabase"></param>
        ''' <param name="access">Flag that must state whether the database can be accessed.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ''' -------------------------------------------------------------------
        Public Shared Function GetCompatibility(strDatabase As String, ByRef access As eDatasourceAccessType) As cEwEDatabase.eCompatibilityTypes

            Dim comp As cEwEDatabase.eCompatibilityTypes = cEwEDatabase.eCompatibilityTypes.Unknown
            Dim dst As eDataSourceTypes = cDataSourceFactory.GetSupportedType(strDatabase)

            ' Detect file type
            Select Case dst
                Case eDataSourceTypes.Access2007, eDataSourceTypes.Access2003, eDataSourceTypes.AccessVsSqlite

                    If File.Exists(strDatabase) Then
                        Dim db As New cEwEAccessDatabase()
                        access = db.Open(strDatabase)
                        If (access = eDatasourceAccessType.Opened) Then
                            comp = db.Compatibility
                            db.Close()
                        End If
                    Else
                        access = eDatasourceAccessType.Failed_FileNotFound
                    End If

                Case eDataSourceTypes.EII, eDataSourceTypes.EIIXML

                    If File.Exists(strDatabase) Then
                        comp = cEwEDatabase.eCompatibilityTypes.EwE6
                        access = eDatasourceAccessType.Opened
                    Else
                        access = eDatasourceAccessType.Failed_FileNotFound
                    End If

                Case eDataSourceTypes.EcoBase

                    If cSystemUtils.IsConnectedToInternet("https://ecobase.ecopath.org") Then
                        comp = cEwEDatabase.eCompatibilityTypes.Importable
                        access = eDatasourceAccessType.Success
                    Else
                        ' ToDo: create explicit enum value Failed_NoInternet
                        access = eDatasourceAccessType.Failed_FileNotFound
                    End If

                Case eDataSourceTypes.Sqlite
                    Dim db As New cEwEEFDatabase()
                    access = db.Open(strDatabase)
                    If (access = eDatasourceAccessType.Opened) Then
                        comp = db.Compatibility
                        db.Close()
                    Else
                        access = eDatasourceAccessType.Failed_FileNotFound
                    End If

                Case eDataSourceTypes.NotSet

                    comp = cEwEDatabase.eCompatibilityTypes.Unknown
                    access = eDatasourceAccessType.Failed_Unknown

            End Select

            Return comp

        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Create a data source onto an existing <see cref="cEwEDatabase">EwE database</see>.
        ''' </summary>
        ''' <param name="db"><see cref="cEwEDatabase">cEwEDatabase</see> to create a datasource for.</param>
        ''' <param name="ds">The newly created datasource.</param>
        ''' <returns>A <see cref="eStatusFlags">Status flag</see> that indicates the valid</returns>
        ''' -------------------------------------------------------------------
        Public Shared Function Create(ByRef db As cEwEDatabase, ByRef ds As IEwEDataSource) As eStatusFlags

            Dim nResult As eStatusFlags = eStatusFlags.OK

            If TypeOf db Is cEwEAccessDatabase Then
                ' Create a DB datasource on a MS Access database
                ds = New cDBDataSource(db)
            End If
            Return nResult

        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Create a data source for a given <see cref="eDataSourceTypes">type of EwE datasource</see>.
        ''' </summary>
        ''' <param name="dst"><see cref="eDataSourceTypes">Type of EwE datasource</see> to create.</param>
        ''' <returns>A <see cref="IEwEDataSource">IEwEDataSource</see> or 
        ''' Nothing if creation failed</returns>
        ''' -------------------------------------------------------------------
        Public Shared Function Create(dst As eDataSourceTypes) As IEwEDataSource

            Dim nResult As eStatusFlags = eStatusFlags.OK

            Select Case dst

                Case eDataSourceTypes.EII
                    Return New cEIIDataSource()

                Case eDataSourceTypes.EIIXML
                    Return New cEIIXMLDataSource()

                Case eDataSourceTypes.Access2003,
                     eDataSourceTypes.Access2007
                    ' Create a DB datasource on a MS Access database
                    Return New cDBDataSource(New cEwEAccessDatabase())
                Case eDataSourceTypes.AccessVsSqlite
                    Return New cDBDataSource(New cEwEVersusDatabase(New cEwEAccessDatabase(), New cEwEEFDatabase()))

                Case eDataSourceTypes.Sqlite
                    Return New cDBDataSource(New cEwEEFDatabase())
                Case Else
                    '
            End Select

            'Failure
            Return Nothing

        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Create a data source for a given file name.
        ''' </summary>
        ''' <param name="strFileName">The file to create the data source for.</param>
        ''' <returns>A <see cref="IEwEDataSource">IEwEDataSource</see> or 
        ''' Nothing if creation failed</returns>
        ''' <remarks>The factory will attempt to decipher from the file name
        ''' which <see cref="eDataSourceTypes">type of EwE datasource</see>
        ''' is requred.</remarks>
        ''' -------------------------------------------------------------------
        Public Shared Function Create(strFileName As String) As IEwEDataSource

            Return Create(GetSupportedType(strFileName))

        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' States whether the operating system supports a given type of EwE 
        ''' <see cref="eDataSourceTypes">data source</see>.
        ''' </summary>
        ''' <param name="dst">The type of EwE <see cref="eDataSourceTypes">data source</see>
        ''' to test.</param>
        ''' <returns>True if the system appears to support the given type of
        ''' data source. The check is implemented by the actual data sources. 
        ''' Implementations can range from simple file checks to online driver 
        ''' validations.</returns>
        ''' -------------------------------------------------------------------
        Public Shared Function IsOSSupported(dst As eDataSourceTypes) As Boolean

            Dim ds As IEwEDataSource = cDataSourceFactory.Create(dst)
            If ds Is Nothing Then Return False
            Return ds.IsOSSupported(dst)

        End Function

    End Class

End Namespace ' DataSources
