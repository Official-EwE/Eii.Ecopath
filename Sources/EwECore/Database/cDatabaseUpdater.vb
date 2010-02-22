Option Strict On
Imports System.IO
Imports System.Data
Imports System.Data.OleDb
Imports EwEPlugin
Imports EwEUtils.Database
Imports System.Reflection

Namespace Database

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Utility class to update a database across minor versions within one major version.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    <CLSCompliant(False)> _
    Public Class cDatabaseUpdater

#Region " Private helper classes "

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

#End Region ' Private helper classes

#Region " Public interfaces "

        ''' <summary>Core to operate onto.</summary>
        Private m_core As cCore = Nothing
        ''' <summary>The baseline database version that this updater can update from</summary>
        Private m_sBaselineVersion As Single = 0.0
        ''' <summary>All available DB updates.</summary>
        Private m_lUpdates As New List(Of cDBUpdate)

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Constructor
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Sub New(ByVal core As cCore, ByVal sBaselineVersion As Single)
            ' Lemembel the cole
            Me.m_core = core
            ' Store baseline version number
            Me.m_sBaselineVersion = sBaselineVersion
            ' Get updates
            Me.m_lUpdates = Me.GetUpdates()
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

#End Region ' Updating

#Region " Internals "

        Private Function GetUpdates() As List(Of cDBUpdate)

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
                        End If
                    End If
                Next
            Catch ex As Exception
            End Try

            ' Sort list, ascending by update number
            lUpdates.Sort(New cDBUpdatePluginContextSort())
            ' Done
            Return lUpdates

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

            Dim sVerDB As Single = db.GetVersion()

            ' Sanity checks
            If db Is Nothing Then Return False
            If sVerDB < sBaselineVersion Then Return False

            For Each ip As cDBUpdate In Me.m_lUpdates
                If (ip.UpdateVersion > sVerDB) Then
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

            Dim strDescription As String = ""
            Dim bSucces As Boolean = True
            Dim msg As cMessage = Nothing

            ' Sanity checks
            If db Is Nothing Then Return False
            If db.GetVersion() < sBaselineVersion Then Return True

            For Each ip As cDBUpdate In Me.m_lUpdates
                ' Check
                If (ip.UpdateVersion > db.GetVersion()) Then
                    Try
                        If db.BeginTransaction() Then
                            If ip.ApplyUpdate(db) Then

                                Dim sbDescription As New System.Text.StringBuilder()
                                Dim iBit As Integer = 0
                                For Each strBit As String In ip.UpdateDescription.Split(New String() {"." & vbNewLine, vbNewLine}, StringSplitOptions.RemoveEmptyEntries)
                                    strBit = strBit.Trim
                                    If Not String.IsNullOrEmpty(strBit) Then
                                        If iBit > 0 Then sbDescription.Append("; ")
                                        sbDescription.Append(strBit)
                                        iBit += 1
                                    End If
                                Next
                                db.SetVersion(ip.UpdateVersion, sbDescription.ToString())
                            Else
                                msg = New cMessage("Database update " & ip.UpdateVersion & " failed", eMessageType.Any, EwEUtils.Core.eCoreComponentType.DataSource, eMessageImportance.Critical)
                                Me.m_core.Messages.SendMessage(msg)
                                bSucces = False
                            End If

                            ' Terminate transaction
                            If bSucces Then
                                bSucces = db.CommitTransaction(True)
                            Else
                                db.RollbackTransaction()
                            End If
                        End If

                    Catch ex As Exception
                        msg = New cMessage("Database update " & ip.UpdateVersion & " failed: " & ex.Message, _
                                           eMessageType.Any, EwEUtils.Core.eCoreComponentType.DataSource, eMessageImportance.Critical)
                        Me.m_core.Messages.SendMessage(msg)
                        bSucces = False
                    End Try

                End If
            Next
            Return bSucces

        End Function

#End Region ' Internals

    End Class

End Namespace
