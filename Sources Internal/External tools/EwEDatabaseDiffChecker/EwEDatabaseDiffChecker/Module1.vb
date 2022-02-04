
Imports System.IO

Module Module1

    Dim exclude As String() = {"Auxillary", "ChangeLog", "Quote"}

    Sub Main()

        ' Checking newer corrupt (model 1) against good, older version (model 2)
        Dim model1 As String = "D:\Troubleshooting\2022-02-04 Ecospace corrupt Marta GSA\GSA0607EwEPelweb_04.02.22.ewemdb"
        Dim model2 As String = "D:\Troubleshooting\2022-02-04 Ecospace corrupt Marta GSA\GSA0607EwEPelweb_04.02.22.Ecospace_OK.ewemdb"

        Dim r As New cAccessReader()
        Dim c As New cDiffChecker()

        Dim tables As String() = r.TableNames(model1)

        Using sw As New StreamWriter("difflog.txt")

            sw.WriteLine("model 1: {0}", model1)
            sw.WriteLine("model 2: {0}", model2)

            For Each table As String In tables

                If Not (exclude.Contains(table) Or table.StartsWith("c") Or table.StartsWith("MSys")) Then

                    Console.WriteLine("Checking " & table)
                    Dim dt1 As DataTable = r.Read(model1, table)
                    dt1.TableName = table
                    Dim dt2 As DataTable = r.Read(model2, table)
                    dt2.TableName = table

                    If c.GetDifferences(dt1, dt2) Then
                        sw.WriteLine(">> {0}: DIFFERENCES", table)
                        For Each d As cRowDifference In c.Differences
                            Select Case d.Diff
                                Case cRowDifference.eRowDifference.Changed
                                    ' Show how older value changed to a newer value (hence {4}->{3})
                                    sw.WriteLine("   {0}.{1} {2} ('{4}'->'{3}') where {5}", table, d.Column, d.Diff.ToString, d.Values1, d.Values2, d.Filter)
                                Case cRowDifference.eRowDifference.Missing
                                    sw.WriteLine("   {0}.{1} {2} where {3}", table, d.Column, d.Diff.ToString, d.Filter)
                            End Select
                        Next
                    Else
                        sw.WriteLine("   {0}: ok", table)
                    End If
                Else
                    Console.WriteLine("Skipped " & table)
                End If
            Next
            sw.Flush()
            sw.Close()
        End Using

        Console.WriteLine("Done, press key")
        Console.ReadKey()

    End Sub

End Module
