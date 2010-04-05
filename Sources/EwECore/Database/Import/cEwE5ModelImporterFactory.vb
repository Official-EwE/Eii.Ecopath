#Region " Imports "

Option Strict On
Imports EwECore.Database
Imports EwECore.DataSources
Imports EwEUtils.Core
Imports EwEUtils.Database

#End Region ' Imports 

''' ===========================================================================
''' <summary>
''' Factory class; builds a <see cref="IEwE5ModelImporter">EwE5 model importer</see>.
''' </summary>
''' ===========================================================================
Public Class cEwE5ModelImporterFactory

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Factory method; builds a <see cref="IEwE5ModelImporter">EwE5 model importer</see>
    ''' from a path to an EwE5 source document. 
    ''' </summary>
    ''' <param name="core">The core to associate the importer with.</param>
    ''' <param name="strFilename">Path to the EwE5 source document to build the
    ''' importer for.</param>
    ''' <returns>A <see cref="IEwE5ModelImporter">EwE5 model importer</see>, if
    ''' all went well, or Nothing otherwise.</returns>
    ''' -----------------------------------------------------------------------
    Public Shared Function GetEwE5ModelImporter(ByVal core As cCore, _
                                               ByVal strFilename As String) As cEwE5ModelImporter

        Select Case cDataSourceFactory.GetSupportedType(strFilename)

            Case eDataSourceTypes.ACCDB, eDataSourceTypes.MDB
                Return New cDBImporter(core, strFilename)

            Case eDataSourceTypes.EII
                Return New cEwE5EIIImporter(core, strFilename)

        End Select

        Return Nothing

    End Function

End Class
