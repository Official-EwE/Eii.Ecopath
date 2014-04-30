Option Strict On
Option Explicit On

Imports System.IO
Imports EwECore
Imports EwEUtils.Utilities
Imports System.Text
Imports EwEUtils.Core

Public Class cBiomassLimits
    Implements IList(Of cBiomassLimit)

    Public lstBiomassLimits As List(Of cBiomassLimit)
    Public mPlugin As cMSEPluginPoint
    Public mMSE As cMSE
    Public mCore As cCore
    Private mFileName As String
    Const mFileNameOnly As String = "BiomassLimits.csv"

#Region "Internal Class"

    Public Class cBiomassLimit

        Public mGroup As cEcoPathGroupInput
        Public mLowerLimit As Double
        Public mUpperLimit As Double

        Private mCore As cCore

        Private Function isIndexInBounds(group As cEcoPathGroupInput) As Boolean
            If (group Is Nothing) Then Return False
            Return group.IsFished
        End Function

        Public Function isValid(ByRef ValidationString As String) As Boolean

            ' ToDo_JS: Globalize this method
            Dim sb As New StringBuilder()
            Dim breturn As Boolean = True
            Debug.Assert(Me.mCore IsNot Nothing, Me.ToString + ".isValid() cCore has not been set. Validation cannot be run.")

            Try
                If Not Me.isIndexInBounds(Me.mGroup) Then
                    breturn = False
                    sb.AppendLine("Group number is not valid.")
                End If

            Catch ex As Exception
                breturn = False
                Debug.Assert(False, Me.ToString + ".isValid() Exception: " + ex.Message)
            End Try

            ValidationString = sb.ToString()

            Return breturn

        End Function

        Public Sub New(Core As cCore)
            'mPlugin = Plugin
            'mMSE = mPlugin.MSE
            mCore = Core
            'mFileName = cMSEUtils.MSEFile(mMSE.DataPath, cMSEUtils.eMSEPaths.BiomassLimits, mFileNameOnly)
        End Sub

    End Class

#End Region

    Public Sub New(Plugin As cMSEPluginPoint)
        mPlugin = Plugin
        mMSE = mPlugin.MSE
        mCore = mMSE.Core
        mFileName = cMSEUtils.MSEFile(mMSE.DataPath, cMSEUtils.eMSEPaths.BiomassLimits, mFileNameOnly)
    End Sub

    Private Function ResolveGroup(strName As String, iIndex As Integer) As cEcoPathGroupInput
        If (iIndex < 1) Or (iIndex > Me.mCore.nGroups) Then Return Nothing
        Dim grp As cEcoPathGroupInput = Me.mCore.EcoPathGroupInputs(iIndex)
        Dim grpName As String = cMSEUtils.FromCSVField(strName)
        If String.Compare(grp.Name, grpName, True) <> 0 Then
            Return Nothing
        End If
        Return grp
    End Function

    Public Sub Add(item As cBiomassLimit) Implements System.Collections.Generic.ICollection(Of cBiomassLimit).Add
        If Not Me.Contains(item) Then
            Me.lstBiomassLimits.Add(item)
        End If
    End Sub

    Public Function LoadLimitsFromCSV() As Boolean

        Dim datadir As String = cMSEUtils.MSEFolder(mMSE.DataPath, cMSEUtils.eMSEPaths.BiomassLimits)
        Dim strVal As String = ""
        Dim StratCounter As Integer = 1
        Dim lstFailedFiles As New List(Of String)
        Dim buff As String
        Dim recs() As String
        Dim breturn As Boolean = False

        'Strategy = New Strategy(Path.GetFileNameWithoutExtension(StrategyFile), StratCounter, StrategyFile, mCore, mMSE)

        'Save the Strategy to the file pass into its constructor

        If Not File.Exists(Me.mFileName) Then
            'message of some sort
            Return False
        End If

        Try

            Dim reader As StreamReader = cMSEUtils.GetReader(Me.mFileName)
            If (reader IsNot Nothing) Then

                buff = reader.ReadLine()
                Do Until reader.EndOfStream

                    recs = buff.Split(","c)

                    Dim tempBiomassLimit As cBiomassLimit
                    'Each HCR Group needs to be a new object
                    tempBiomassLimit = New cBiomassLimit(mCore)

                    tempBiomassLimit.mGroup = mCore.EcoPathGroupInputs(cStringUtils.ConvertToInteger(recs(0)))
                    tempBiomassLimit.mLowerLimit = cStringUtils.ConvertToDouble(recs(1))
                    tempBiomassLimit.mUpperLimit = cStringUtils.ConvertToDouble(recs(2))

                    Dim strMsg As String = ""
                    ' Only add valid BiomassLimits!
                    If tempBiomassLimit.isValid(strMsg) Then
                        Me.Add(tempBiomassLimit)
                    End If

                    breturn = True
                    buff = reader.ReadLine()
                Loop

                cMSEUtils.ReleaseReader(reader)

            End If 'reader IsNot Nothing

        Catch ex As Exception
            System.Console.WriteLine(Me.ToString + ".Read() Exception: " + ex.Message)
        End Try

        'for debugging
        Debug.Assert(breturn, Me.ToString + ".Read() Failed to read biomass limits from file.")

        'Warn the user if anything failed
        If breturn = False Then
            Me.mCore.Messages.SetMessageLock()
            Me.mCore.Messages.SendMessage(New cMessage("Cefas MSE Failed to read the biomass limits file",
                                                          eMessageType.ErrorEncountered, eCoreComponentType.Plugin, eMessageImportance.Information))
            Me.mCore.Messages.RemoveMessageLock()
        End If

        Return True


    End Function

    Public Function SaveLimitsToCSV() As Boolean
        Dim csvStrategyFile As StreamWriter = Nothing
        Dim strFile As String = ""
        Dim strPath As String = ""
        Dim msg As cMessage = Nothing
        Dim breturn As Boolean = True
        Try


            If msg Is Nothing Then
                strPath = Path.GetDirectoryName(Me.mFileName)
                msg = New cMessage(String.Format(My.Resources.STATUS_SAVED_BIOMASSLIMITS, My.Resources.CAPTION, strPath), eMessageType.DataExport, eCoreComponentType.External, eMessageImportance.Information)
                msg.Hyperlink = strPath
            End If
            'Save the Strategy to file
            'The filename was passed into the Strategy in its constructor
            Me.Save()

        Catch ex As Exception
            breturn = False
            'Me.Save() will throw exceptions out to here
            Me.mCore.Messages.SendMessage(New cMessage("Exception saving Biomass Limits to file.", eMessageType.ErrorEncountered, eCoreComponentType.Plugin, eMessageImportance.Warning))
        End Try

        If msg IsNot Nothing Then
            Me.mCore.Messages.SendMessage(msg)
        End If

        Return breturn
    End Function

    Private Function Save() As Boolean
        Dim strm As StreamWriter
        'Create a new file
        strm = cMSEUtils.GetWriter(Me.mFileName, False)
        If (strm IsNot Nothing) Then

            strm.WriteLine("GroupIndex, LowerLimit, UpperLimit")
            For Each iBiomassLimit In Me.lstBiomassLimits
                strm.WriteLine(cStringUtils.ToCSVField(iBiomassLimit.mGroup.Name) & "," & _
                                          cStringUtils.ToCSVField(iBiomassLimit.mLowerLimit) & "," & _
                                          cStringUtils.ToCSVField(iBiomassLimit.mUpperLimit))
            Next
            cMSEUtils.ReleaseWriter(strm)
        End If

        Return True
    End Function


    Public Function GetUpperLimit(iGrp As Integer) As Double
        For Each iBiomassLimit In lstBiomassLimits
            If iBiomassLimit.mGroup.Index = iGrp Then Return iBiomassLimit.mUpperLimit
        Next

        Return 100000
    End Function

    Public Function GetLowerLimit(iGrp As Integer) As Double
        For Each iBiomassLimit In lstBiomassLimits
            If iBiomassLimit.mGroup.Index = iGrp Then Return iBiomassLimit.mLowerLimit
        Next
        Return 0
    End Function

    Public Function Exist(iGrp As Integer) As Boolean
        For Each iBiomassLimit In lstBiomassLimits
            If iBiomassLimit.mGroup.Index = iGrp Then Return True
        Next
        Return False
    End Function

    Public Sub Clear() Implements System.Collections.Generic.ICollection(Of cBiomassLimit).Clear
        Me.lstBiomassLimits.Clear()
    End Sub

    Public Function Contains(item As cBiomassLimit) As Boolean Implements System.Collections.Generic.ICollection(Of cBiomassLimit).Contains
        For Each iLimit As cBiomassLimit In Me.lstBiomassLimits
            If Object.ReferenceEquals(item.mGroup, iLimit.mGroup) Then
                Return True
            End If
        Next
        Return False
    End Function

    Public Sub CopyTo(array() As cBiomassLimit, arrayIndex As Integer) Implements System.Collections.Generic.ICollection(Of cBiomassLimit).CopyTo
        ' NOP
    End Sub

    Public ReadOnly Property Count As Integer Implements System.Collections.Generic.ICollection(Of cBiomassLimit).Count
        Get
            Return lstBiomassLimits.Count
        End Get
    End Property

    Public ReadOnly Property IsReadOnly As Boolean Implements System.Collections.Generic.ICollection(Of cBiomassLimit).IsReadOnly
        Get
            Return False
        End Get
    End Property

    Public Function Remove(item As cBiomassLimit) As Boolean Implements System.Collections.Generic.ICollection(Of cBiomassLimit).Remove
        Return Me.lstBiomassLimits.Remove(item)
    End Function

    Public Function IndexOf(item As cBiomassLimit) As Integer Implements System.Collections.Generic.IList(Of cBiomassLimit).IndexOf
        Return Me.lstBiomassLimits.IndexOf(item)
    End Function

    Public Sub Insert(index As Integer, item As cBiomassLimit) Implements System.Collections.Generic.IList(Of cBiomassLimit).Insert
        Me.lstBiomassLimits.Insert(index, item)
    End Sub

    Default Public Property Item(index As Integer) As cBiomassLimit Implements System.Collections.Generic.IList(Of cBiomassLimit).Item
        Get
            Return Me.lstBiomassLimits.Item(index)
        End Get
        Set(value As cBiomassLimit)
            Me.lstBiomassLimits(index) = value
        End Set
    End Property

    Public Sub RemoveAt(index As Integer) Implements System.Collections.Generic.IList(Of cBiomassLimit).RemoveAt
        Me.lstBiomassLimits.RemoveAt(index)
    End Sub

    Public Function GetEnumerator() As System.Collections.Generic.IEnumerator(Of cBiomassLimit) Implements System.Collections.Generic.IEnumerable(Of cBiomassLimit).GetEnumerator
        Return Me.lstBiomassLimits.GetEnumerator()
    End Function

    Public Function Bogus() As System.Collections.IEnumerator Implements System.Collections.IEnumerable.GetEnumerator
        'NOP
    End Function


End Class
