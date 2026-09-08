' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports EwECore.Common
Imports EwECore.Database
Imports EwECore.DataSources
Imports EwECore.Plugins
Imports EwECore.Plugins.Database

''' ===========================================================================
''' <summary>
''' Factory class; builds a <see cref="cEwE5ModelImporter">EwE5 model importer</see>.
''' </summary>
''' ===========================================================================
Public Class cModelImporterFactory

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Factory method; builds a <see cref="cEwE5ModelImporter">EwE5 model importer</see>
    ''' from a path to an EwE5 source document. 
    ''' </summary>
    ''' <param name="core">The core to associate the importer with.</param>
    ''' <param name="strSource">Path to data source to build the importer for.</param>
    ''' <returns>A <see cref="cEwE5ModelImporter">EwE5 model importer</see>, if
    ''' all went well, or Nothing otherwise.</returns>
    ''' -----------------------------------------------------------------------
    Public Shared Function GetModelImporter(core As cCore,
                                            strSource As String,
                                            pm As cPluginManager) As IModelImporter

        If (strSource.ToLower().StartsWith("ewe-ecobase:")) Then
            Return New cEcobaseImporter(core)
        End If

        Select Case cDataSourceFactory.GetSupportedType(strSource)

            Case eDataSourceTypes.Access2007, eDataSourceTypes.Access2003, eDataSourceTypes.AccessVsSqlite
                Return New cEwE5DatabaseImporter(core)

            Case eDataSourceTypes.EII
                Return New cEwE5EIIImporter(core)

        End Select

        ' Explore if a plug-in is provided that can do this too
        If (pm IsNot Nothing) Then
            For Each pi As IPlugin In pm.GetPlugins(GetType(IModelImportPlugin))
                Dim imp As IModelImportPlugin = DirectCast(pi, IModelImportPlugin)
                If imp.CanImportFrom(strSource) Then
                    Return imp
                End If
            Next
        End If

        Return Nothing

    End Function

End Class
