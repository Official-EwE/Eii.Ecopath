Imports LumenWorks.Framework.IO.Csv
Imports System.IO
Imports EwEUtils.Utilities
Imports ScientificInterfaceShared.Controls

Public Class frmEditSurvivabilites

    Private Core As EwECore.cCore
    Private mMSE As cMSE
    Private mSurvivability As cSurvivability

    Public Sub New(MSE As cMSE, Survivability As cSurvivability)

        Me.InitializeComponent()
        Me.mMSE = MSE
        Me.mSurvivability = Survivability

    End Sub

    Private Sub PopulateTable()

        Dim reader As StreamReader
        Dim csv As CsvReader

        reader = cMSEUtils.GetReader(mMSE.DataPath)

        reader = cMSEUtils.GetReader(cMSEUtils.MSEFile(mMSE.DataPath, cMSEUtils.eMSEPaths.DistrParams, "Survivabilities.csv"))

        If (reader IsNot Nothing) Then
            csv = New CsvReader(reader, True)
            While Not csv.EndOfStream
                ' param = ExtractSurvivabilityParameter(csv)
                ' Need to replace above line with the code that will populate the grid - this can
                ' only be done once we understand how the grid will work and has been added to the form - MP to speak to JB
            End While

        Else
            ' ToDo_JS: Diets multipliers were not read; handle error
        End If

    End Sub

    ''' <summary>
    ''' Extracts information about the distribution of a survivability
    ''' from one line in the csv file
    ''' </summary>
    ''' <param name="csv">The csv file that the parameter information
    ''' will be extracted from</param>
    ''' <returns>SurvivabilityDistributionParam object</returns>
    ''' <remarks></remarks>
    'Private Function ExtractSurvivabilityParameter(csv) As cSurvivability.cSurvivabilityDistributonParam

    '    If csv = Nothing Then Return Nothing
    '    If (Not csv.ReadNextRecord()) Then Return Nothing
    '    If (csv.FieldCount < 5) Then Return Nothing

    '    Dim TFleetNumber As Integer
    '    Dim TFleetName As String
    '    Dim TGroupNumber As Integer
    '    Dim TGroupName As String = ""
    '    Dim TAlpha As Single
    '    Dim TBeta As Double

    '    Try
    '        TFleetNumber = cStringUtils.ConvertToInteger(csv(0))
    '        TFleetName = cMSEUtils.FromCSVField(csv(1))
    '        TGroupNumber = cStringUtils.ConvertToInteger(csv(2))
    '        TGroupName = cMSEUtils.FromCSVField(csv(3))
    '        TAlpha = cStringUtils.ConvertToDouble(csv(4))
    '        TBeta = cStringUtils.ConvertToDouble(csv(5))

    '        ' JS 02Oct2013: Need to validate group number
    '        If TGroupNumber < 1 Or TGroupNumber >= Me.Core.nGroups Then
    '            ' ToDo:_JS: report error somehow
    '            Return Nothing
    '        End If

    '    Catch ex As Exception
    '        ' ToDo:_JS: report error somehow
    '        Return Nothing
    '    End Try

    '    Return New cSurvivability.cSurvivabilityDistributonParam(TFleetNumber, TFleetName, TGroupNumber, TGroupName, TAlpha, TBeta)

    'End Function

End Class