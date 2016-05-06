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
'    UBC Institute for the Oceans and Fisheries, Vancouver BC, Canada, and 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'

Option Strict On
Imports EwEPlugin
Imports EwEUtils.Database
Imports System.Data
Imports EwEUtils.Core
Imports System.Xml

Imports EwEUtils.SystemUtilities.cSystemUtils


''' --------------------------------------------------------------------------
''' <summary>
''' Database update base class.
''' </summary>
''' --------------------------------------------------------------------------
Friend MustInherit Class cDBUpdate

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' This method provides the update version number that will be entered in
    ''' the update log of the database. This version number is also used to check
    ''' whether an update should run.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public MustOverride ReadOnly Property UpdateVersion() As Single

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' This method provides the text that will be entered in the update log in
    ''' the database.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public MustOverride ReadOnly Property UpdateDescription() As String
    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Apply the actual update
    ''' </summary>
    ''' <param name="db"></param>
    ''' <returns></returns>
    ''' -----------------------------------------------------------------------
    Public MustOverride Function ApplyUpdate(ByRef db As EwEUtils.Database.cEwEDatabase) As Boolean

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Write update progress to the log.
    ''' </summary>
    ''' <param name="strProgress">Progress entry to write.</param>
    ''' -----------------------------------------------------------------------
    Protected Sub LogProgress(ByVal strProgress As String, ByVal bSucces As Boolean)
        cLog.Write(String.Format("Update {0}: {1} {2}", _
                                 Me.UpdateVersion, _
                                 strProgress, _
                                 IIf(bSucces, "Succes", "Failed")))
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get whether an update should always run. It is advised to use this method
    ''' only to insert database changes into both the EwE trunk and older, already 
    ''' released versions of EwE.
    ''' </summary>
    ''' <remarks>
    ''' By default, updates will only run if their reported <see cref="UpdateVersion"/>
    ''' exceeds the <see cref="cEwEDatabase.GetVersion">version of a database</see>.
    ''' Updates that always run will, well, always run, and therefore will need to 
    ''' perform extra rigorous tests whether execution is necessary prior to 
    ''' affecting a database. Please use this flag with utmost care. Preferably do
    ''' not use it at all.
    ''' </remarks>
    ''' -----------------------------------------------------------------------
    <Obsolete("Method discontinued")> _
    Public Overridable ReadOnly Property RunAlways As Boolean
        Get
            Return False
        End Get
    End Property

End Class
