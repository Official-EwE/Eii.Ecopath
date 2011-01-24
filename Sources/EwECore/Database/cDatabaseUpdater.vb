#Region " Imports "

Option Strict On
Imports System.Reflection
Imports System.Text
Imports EwEUtils.Core
Imports EwEUtils.Database

#End Region ' Imports

Namespace Database

    ''' =======================================================================
    ''' <summary>
    ''' Utility class to update a database across minor versions within one major version.
    ''' </summary>
    ''' =======================================================================
    <CLSCompliant(False)> _
    Friend Class cDatabaseUpdater

#Region " Private bits "

        ''' -----------------------------------------------------------------------
        ''' <summary>Helper class to sort database update plug-ins by 
        ''' <see cref="cDBUpdate.UpdateVersion">Version</see>, in
        ''' ascending order.</summary>
        ''' -----------------------------------------------------------------------
        Private Class cDBUpdatePluginContextSort
            Implements IComparer(Of cDBUpdate)

            Public Function Compare(ByVal x As cDBUpdate, ByVal y As cDBUpdate) As Integer _
                    Implements IComparer(Of cDBUpdate).Compare
                Return CInt(IIf(x.UpdateVersion < y.UpdateVersion, -1, 1))
            End Function

        End Class

        ''' <summary>Core to operate onto.</summary>
        Private m_core As cCore = Nothing
        ''' <summary>The baseline database version that this updater can update from</summary>
        Private m_sBaselineVersion As Single = 0.0

#End Region ' Private bits

#Region " Public interfaces "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Constructor.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Sub New(ByVal core As cCore, ByVal sBaselineVersion As Single)
            ' Lemembel the cole
            Me.m_core = core
            ' Store baseline version number
            Me.m_sBaselineVersion = sBaselineVersion
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' States if there are updates available for a given database.
        ''' </summary>
        ''' <param name="db"></param>
        ''' <returns></returns>
        ''' -------------------------------------------------------------------
        Public Function HasUpdates(ByVal db As cEwEDatabase) As Boolean
            Return Me.HasDatabaseUpdates(db, Me.m_sBaselineVersion)
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Perform a database update
        ''' </summary>
        ''' <param name="db">The <see cref="cEwEDatabase">database</see> to update</param>
        ''' <returns>True if succesful</returns>
        ''' <remarks>
        ''' More elaborate status info may be required to populate a tracking GUI.
        ''' This could be implemented either via a public accessible status object 
        ''' that gets populated during every update step, or via delegates.
        ''' </remarks>
        ''' -------------------------------------------------------------------
        Public Function UpdateDatabase(ByVal db As cEwEDatabase) As Boolean
            Return Me.UpdateDatabase(db, Me.m_sBaselineVersion)
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Returns the max supported core version of the database.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Shared Function MaxSupportedVersion() As Single
            Dim sVersion As Single = 6.0! ' Should obtain this from cEwEDatabase, but ok
            Dim upd As cDBUpdate() = cDatabaseUpdater.GetUpdates()
            ' Has updates?
            If upd.Length > 0 Then
                ' #Yes: return version of last update (updates are sorted by version ASC)
                sVersion = upd(upd.Length - 1).UpdateVersion
            End If
            Return sVersion
        End Function

#End Region ' Updating

#Region " Internals "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Returns all available update objects in this assembly.
        ''' </summary>
        ''' <returns>An array of available updates.</returns>
        ''' -------------------------------------------------------------------
        Private Shared Function GetUpdates() As cDBUpdate()

            Dim lUpdates As New List(Of cDBUpdate)
            Dim clsType As Type = Nothing
            Dim clsAssembly As Assembly
            Dim upd As cDBUpdate = Nothing

            Try
                ' Get assembly that declared the database updater class
                clsAssembly = Assembly.GetAssembly(GetType(cDatabaseUpdater))
                ' For every class in this assembly
                For Each clsType In clsAssembly.GetTypes
                    ' Is cDBUpdate derived?
                    If GetType(cDBUpdate).IsAssignableFrom(clsType) Then
                        ' #Yes: Is NOT cDBUpdate itself ('cause this is an abstract class)?
                        If Not Type.Equals(clsType, GetType(cDBUpdate)) Then
                            ' #Yes: Create update instance
                            upd = DirectCast(Activator.CreateInstance(clsType, New Object() {}), cDBUpdate)
                            ' Add to the list of updates
                            lUpdates.Add(upd)
                        End If
                    End If
                Next
            Catch ex As Exception
            End Try

            ' Sort list, ascending by update number
            lUpdates.Sort(New cDBUpdatePluginContextSort())
            ' Done
            Return lUpdates.ToArray()

        End Function

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Returns whether plug-ins have been found that can upgrade an
        ''' <see cref="cEwEDatabase">EwE database</see> to a newer version that
        ''' exceeds a requested <paramref name="sBaselineVersion">baseline version</paramref>.
        ''' </summary>
        ''' <param name="db">The EwE database to test for upgrades.</param>
        ''' <param name="sBaselineVersion">The baseline database version required 
        ''' by the EwE software.</param>
        ''' <returns>True if updates are available.</returns>
        ''' -----------------------------------------------------------------------
        Public Function HasDatabaseUpdates(ByVal db As cEwEDatabase, ByVal sBaselineVersion As Single) As Boolean

            Dim sDBVersion As Single = db.GetVersion()

            ' Sanity checks
            If db Is Nothing Then Return False
            If sDBVersion < sBaselineVersion Then Return False

            For Each update As cDBUpdate In cDatabaseUpdater.GetUpdates()
                If (update.UpdateVersion > sDBVersion) Then
                    Return True
                End If
            Next
            Return False

        End Function

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Run available database update plug-ins.
        ''' </summary>
        ''' <param name="db">The database to update.</param>
        ''' <param name="sBaselineVersion">Database version to start updating from.</param>
        ''' <remarks>
        ''' This method does not attempt to cross thread boundaries.
        ''' </remarks>
        ''' -----------------------------------------------------------------------
        Public Function UpdateDatabase(ByVal db As cEwEDatabase, ByVal sBaselineVersion As Single) As Boolean

            Dim sDBVersion As Single = 0.0!
            Dim iUpdate As Integer = 0
            Dim update As cDBUpdate = Nothing
            Dim aUpdates As cDBUpdate() = cDatabaseUpdater.GetUpdates()
            Dim bSucces As Boolean = True

            ' Sanity check
            If (db Is Nothing) Then Return False

            ' Get DB version
            sDBVersion = db.GetVersion()

            ' Abort if no need to run updates
            If (sDBVersion < sBaselineVersion) Then Return True

            ' For all updates
            While (iUpdate < aUpdates.Length) And (bSucces = True)

                ' Get update
                update = aUpdates(iUpdate)

                ' Version ok?
                If (update.UpdateVersion > sDBVersion) Then
                    ' #Yes: able to start transaction?
                    If db.BeginTransaction() Then
                        Try
                            ' #Yes: run the update
                            bSucces = update.ApplyUpdate(db)
                            ' Update ran successful?
                            If bSucces Then
                                ' #Yes: Update database version
                                db.SetVersion(update.UpdateVersion, Me.ToShortDescription(update.UpdateDescription))
                            Else
                                ' #No: report a generic error
                                Me.ReportUpdateError(String.Format(My.Resources.CoreMessages.DATABASE_UPDATE_FAILED, update.UpdateVersion))
                            End If

                        Catch ex As Exception
                            ' Woops!
                            Me.ReportUpdateError(String.Format(My.Resources.CoreMessages.DATABASE_UPDATE_FAILED_DETAIL, update.UpdateVersion, ex.Message))
                            bSucces = False
                        End Try

                        ' Update ran succesfully?
                        If bSucces Then
                            ' #Yes: commit changes
                            bSucces = db.CommitTransaction(True)
                        Else
                            ' #No: rollback changes
                            db.RollbackTransaction()
                        End If

                    Else
                        ' #No: failed to start transaction - an update did not clean up well
                        Debug.Assert(False, "Database version " & sDBVersion & " update sequence failed for update " & update.UpdateVersion)
                        bSucces = False
                    End If

                End If

                ' Next
                iUpdate += 1
            End While

            Return bSucces

        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Convert a database update description into a short description.
        ''' </summary>
        ''' <param name="strDescription"></param>
        ''' <returns></returns>
        ''' -------------------------------------------------------------------
        Private Function ToShortDescription(ByVal strDescription As String) As String

            Dim sbDescription As New StringBuilder()
            Dim strBit As String = ""
            Dim iBit As Integer = 0

            For Each strBit In strDescription.Split(New String() {"." & vbNewLine, vbNewLine}, StringSplitOptions.RemoveEmptyEntries)
                strBit = strBit.Trim
                If Not String.IsNullOrEmpty(strBit) Then
                    If iBit > 0 Then sbDescription.Append("; ")
                    sbDescription.Append(strBit)
                    iBit += 1
                End If
            Next
            Return sbDescription.ToString()

        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Send an error message to the core.
        ''' </summary>
        ''' <param name="strError"></param>
        ''' -------------------------------------------------------------------
        Private Sub ReportUpdateError(ByVal strError As String)

            Dim msg As cMessage = New cMessage(strError, _
                                               eMessageType.DataImport, _
                                               eCoreComponentType.DataSource, _
                                               eMessageImportance.Critical)

            Try
                Me.m_core.Messages.SendMessage(msg)
                cLog.Write("Database update failure: " & strError)
            Catch ex As Exception

            End Try

        End Sub

#End Region ' Internals

    End Class

End Namespace
