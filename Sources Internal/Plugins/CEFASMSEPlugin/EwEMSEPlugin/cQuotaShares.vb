Option Strict On
Option Explicit On

Imports System.IO
Imports LumenWorks.Framework.IO.Csv
Imports EwEUtils.Utilities
Imports EwECore

Public Class cQuotaShares

#Region " Internal Structure "

    Public Structure QuotaShare

        Public mGroupNo As Integer
        Public mFleetNo As Integer
        Public mShare As Double

        Public Sub New(GroupNo As Integer, FleetNo As Integer, Share As Double)
            mGroupNo = GroupNo
            mFleetNo = FleetNo
            mShare = Share
        End Sub

        Public ReadOnly Property IsNull() As Boolean
            Get
                If mGroupNo = 0 And mFleetNo = 0 And mShare = 0 Then Return True
                Return False
            End Get
        End Property

    End Structure

#End Region

#Region " Internal Variables "
    Private mlstQuotaShares As New List(Of QuotaShare)
    Private mcore As cCore
    Private mMSE As cMSE
    Private mQuotaShareFileExists As Boolean
    Private mQuotaShareFileValid As Boolean
#End Region

#Region " Construction "

    Public Sub New(core As EwECore.cCore, MSE As cMSE)
        mcore = core
        mMSE = MSE
        mlstQuotaShares = New List(Of QuotaShare)
        SetDefault()
End Sub

#End Region

#Region " Properties "

    ''' <summary>
    ''' Checks whether the quota file is valid
    ''' </summary>
    Private Function QuotaFileValid() As Boolean
        Throw New NotImplementedException("QuotaFileValid not implemented")
        Return False
    End Function

    Public ReadOnly Property GetLstGrpShares As List(Of QuotaShare)
        Get
            Return mlstQuotaShares
        End Get
    End Property

    ''' <summary>
    ''' Returns whether the Quota share file exists
    ''' </summary>
    Public Property QuotaFileExists() As Boolean
        Get
            Return mQuotaShareFileExists
        End Get
        Set(ByVal value As Boolean)
            mQuotaShareFileExists = value
        End Set
    End Property

    ''' <summary>
    ''' Returns the number of elements in the quota share list
    ''' </summary>
    Public ReadOnly Property CountDist() As Integer
        Get
            Return mlstQuotaShares.Count
        End Get
    End Property

#End Region

#Region " Functions "

    ''' <summary>
    ''' Checks whether the quota share file is valid
    ''' </summary>
    Private Function QuotaShareFileValid() As Boolean
        'TODO MP add validation code to check whether distribution file is okay
        'what checks need doing?
        Throw New NotImplementedException("QuotaShareFileValid not implemented")
        Return False
    End Function

    ''' <summary>
    ''' Adds a quota share value to the list of quota shares
    ''' and if it can't returns FALSE
    ''' </summary>
    Public Function AddQuotaShare(GroupNo As Integer, FleetNo As Integer, Share As Double) As Boolean

        'Check Fleet Number
        If FleetNo < 0 Or FleetNo > mcore.nFleets Then Return False

        'Check GroupNo
        If GroupNo < 0 Or GroupNo > mcore.nGroups Then Return False

        'Check Alpha and Beta
        If Share < 0 Or Share > 1 Then Return False

        'Add it to the list
        mlstQuotaShares.Add(New QuotaShare(GroupNo, FleetNo, Share))

        Return True

    End Function

    ''' <summary>
    ''' Reads the iRow_th from the list of quotas
    ''' </summary>
    ''' <param name="iRow"></param>
    ''' <returns></returns>
    ''' <remarks>iRow is zero-based</remarks>
    Public Function ReadRowDist(iRow As Integer) As QuotaShare
        If iRow < 0 Then Return Nothing
        If iRow > mlstQuotaShares.Count - 1 Then Return Nothing
        Return mlstQuotaShares(iRow)
    End Function

    ''' <summary>
    ''' Reads the quota share that iFleet has of iGroup
    ''' </summary>
    ''' <param name="iFleet"></param>
    ''' <param name="iGroup"></param>
    ''' <returns></returns>
    ''' <remarks>iFleet and iGroup are zero-based</remarks>
    Public Function ReadiFleetiGroupQuota(iFleet As Integer, iGroup As Integer) As QuotaShare

        If iFleet < 1 Or iFleet > mcore.nFleets Then Return Nothing
        If iGroup < 1 Or iGroup > mcore.nGroups Then Return Nothing

        For iRow As Integer = 0 To mlstQuotaShares.Count - 1
            If mlstQuotaShares(iRow).mFleetNo = iFleet And mlstQuotaShares(iRow).mGroupNo = iGroup Then Return mlstQuotaShares(iRow)
        Next

        Return Nothing

    End Function

    ''' <summary>
    ''' Loads the quota shares from CSV
    ''' </summary>
    ''' <returns>True if successful, false otherwise</returns>
    ''' <remarks></remarks>
    Public Function LoadQuotaFromCSV() As Boolean

        Dim reader As StreamReader = Nothing
        Dim csv As CsvReader = Nothing
        Dim iQuotaShare As QuotaShare
        Dim bSuccess As Boolean = True
        Dim filePath As String = cMSEUtils.MSEFile(mMSE.DataPath, cMSEUtils.eMSEPaths.Fleet, "QuotaShares.csv")

        If File.Exists(filePath) Then

            reader = cMSEUtils.GetReader(filePath)
            If (reader IsNot Nothing) Then
                Try
                    mlstQuotaShares.Clear()
                    csv = New CsvReader(reader, True)
                    mQuotaShareFileExists = True
                    While Not csv.EndOfStream
                        iQuotaShare = ExtractQuotaShare(csv)
                        If Not iQuotaShare.IsNull Then
                            'TODO Ask Jeroen - how do I check whether iQuotaShare is equal to nothing
                            AddQuotaShare(iQuotaShare.mGroupNo, iQuotaShare.mFleetNo, iQuotaShare.mShare)
                        End If
                    End While
                    csv.Dispose()

                Catch ex As Exception
                    'Debug.Assert(False, Me.ToString & ".LoadEcosimParameters() Exception: " & ex.Message)
                    bSuccess = False
                End Try
                cMSEUtils.ReleaseReader(reader)
            End If
        Else
            bSuccess = False
        End If

        Return bSuccess

    End Function

    ''' <summary>
    ''' Extracts a single quota share from the csv file
    ''' </summary>
    ''' <param name="csv">The CSV object linking to the quota share file</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function ExtractQuotaShare(ByVal csv As CsvReader) As QuotaShare

        ' Sanity checks
        If (csv Is Nothing) Then Return Nothing
        If (Not csv.ReadNextRecord()) Then Return Nothing

        Dim TGroupNumber As Integer
        Dim TFleetNumber As Integer
        Dim TShare As Double

        Try
            TGroupNumber = cStringUtils.ConvertToInteger(csv(0))
            TFleetNumber = cStringUtils.ConvertToInteger(csv(2))
            TShare = cStringUtils.ConvertToDouble(csv(4))

        Catch ex As Exception
            ' ToDo_JS: respond to error
            Return Nothing
        End Try

        Return New QuotaShare(TGroupNumber, TFleetNumber, TShare)

    End Function

    Public Sub CreateDefaultCSV()
        SetDefault()
        SaveQuotaSharesToCSV()
    End Sub

    ''' <summary>
    ''' Saves quota shares to CSV
    ''' </summary>
    ''' <returns>False if there was an error</returns>
    ''' <remarks></remarks>
    Public Function SaveQuotaSharesToCSV() As Boolean

        Dim writer As StreamWriter = cMSEUtils.GetWriter(cMSEUtils.MSEFile(mMSE.DataPath, cMSEUtils.eMSEPaths.Fleet, "QuotaShares.csv"), False)
        Dim bSuccess As Boolean = False

        If (writer Is Nothing) Then Return bSuccess

        Try
            writer.WriteLine("GroupNumber,GroupName,FleetNumber,FleetName,QuotaShare")

            For Each entry As QuotaShare In mlstQuotaShares
                writer.WriteLine(cStringUtils.ToCSVField(entry.mGroupNo) & "," & _
                                 cStringUtils.ToCSVField(mcore.EcoPathGroupInputs(entry.mGroupNo).Name) & "," & _
                                 cStringUtils.ToCSVField(entry.mFleetNo) & "," & _
                                 cStringUtils.ToCSVField(mcore.FleetInputs(entry.mFleetNo).Name) & "," & _
                                 cStringUtils.ToCSVField(entry.mShare))
            Next

            bSuccess = True

        Catch ex As Exception

        End Try
        cMSEUtils.ReleaseWriter(writer)
        Return bSuccess

    End Function

#End Region

#Region " Subroutines "

    ''' <summary>
    ''' Runs when the MSE plugin has been loaded up
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub PluginLoaded()
        Dim reader As StreamReader = Nothing

        'Todo MP
        ' check file exists for surivability distribution parameters
        If Not File.Exists(cMSEUtils.MSEFile(mMSE.DataPath, cMSEUtils.eMSEPaths.Fleet, "QuotaShares.csv")) Then
            QuotaFileExists = False
            mQuotaShareFileValid = False
        Else
            ' check file is correct
            If Not QuotaShareFileValid() Then
                mQuotaShareFileValid = False
            Else
                'If it is load the file into memory
                mQuotaShareFileValid = True
                LoadQuotaFromCSV()
            End If
        End If

        If Not File.Exists(cMSEUtils.MSEFile(mMSE.DataPath, cMSEUtils.eMSEPaths.ParamsOut, "Survivabilites_out.csv")) Then
            QuotaFileExists = False
            mQuotaShareFileValid = False
        Else
            If Not QuotaFileValid() Then
                mQuotaShareFileValid = False
            Else
                mQuotaShareFileValid = True
                LoadQuotaFromCSV()
            End If
        End If

    End Sub

#End Region

    'Something that might only be used for testing purposes
    Public Sub SetDefault()
        Dim nFleetsCatch As Integer

        If Not mlstQuotaShares Is Nothing Then mlstQuotaShares.Clear()

        For iGroup = 1 To mcore.nLivingGroups

            'Count how many fleets catch this group
            nFleetsCatch = 0
            For iFleet = 1 To mcore.nFleets
                If mcore.FleetInputs(iFleet).Landings(iGroup) > 0 Then nFleetsCatch += 1
            Next

            For iFleet = 1 To mcore.nFleets
                If mcore.FleetInputs(iFleet).Landings(iGroup) > 0 Then
                    AddQuotaShare(iGroup, iFleet, 1 / nFleetsCatch)
                End If
            Next

        Next

    End Sub

End Class
