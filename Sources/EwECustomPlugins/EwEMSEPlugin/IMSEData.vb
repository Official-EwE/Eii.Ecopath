' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports EwECore



''' <summary>
''' Foundation interface for user-supplied data in the MSE plug-in.
''' </summary>
Public Interface IMSEData

    ''' <summary>
    ''' Load data from a file.
    ''' </summary>
    ''' <param name="strFilename">Optional file name to load data from.</param>
    ''' <returns>True if successful.</returns>
    Function Load(Optional msg As cMessage = Nothing,
                  Optional strFilename As String = "") As Boolean

    ''' <summary>
    ''' Load data to a file.
    ''' </summary>
    ''' <param name="strFilename">Optional file name to save data to.</param>
    ''' <returns>True if successful.</returns>
    Function Save(Optional strFilename As String = "") As Boolean

    ''' <summary>
    ''' Returns whether the data has been changed since the last time it was loaded.
    ''' </summary>
    ''' <returns>True if the data has been changed since the last time it was loaded.</returns>
    Function IsChanged() As Boolean

    ''' <summary>
    ''' Set the data to default values.
    ''' </summary>
    Sub Defaults()

    ''' <summary>
    ''' Returns whether the file(s) for the data exist
    ''' </summary>
    ''' <param name="strFilename"></param>
    ''' <returns></returns>
    Function FileExists(Optional strFilename As String = "") As Boolean

End Interface
