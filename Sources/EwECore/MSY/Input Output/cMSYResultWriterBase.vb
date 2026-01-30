' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports System.IO
Imports EwEUtils.Utilities



Namespace MSY

    ''' <summary>
    ''' Base Class for writing MSY results.
    ''' </summary>
    Public MustInherit Class cMSYResultWriterBase

#Region " Private vars "

        Protected m_core As cCore = Nothing

#End Region ' Private vars

#Region " Construction "

        Public Sub New(core As cCore)
            Me.m_core = core
        End Sub

#End Region ' Construction

#Region " Internals "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Open a CSV file writer.
        ''' </summary>
        ''' <param name="strFile">File name to open the writer for.</param>
        ''' <returns>The writer, or nothing if an error occurred.</returns>
        ''' <remarks>Close the writer with <see cref="CloseWriter"/>.</remarks>
        ''' -------------------------------------------------------------------
        Protected Function OpenWriter(strFile As String) As StreamWriter

            Dim msg As cMessage = Nothing
            Dim sw As StreamWriter = Nothing

            ' Abort if directory missing
            If cFileUtils.IsDirectoryAvailable(Path.GetDirectoryName(strFile), True) = False Then
                msg = Me.ErrorMessage(strFile, My.Resources.CoreMessages.OUTPUT_DIRECTORY_MISSING)
                Me.m_core.Messages.SendMessage(msg)
                Return Nothing
            End If

            Try
                sw = New StreamWriter(strFile)
            Catch ex As Exception
                msg = Me.ErrorMessage(strFile, ex.Message)
                Me.m_core.Messages.SendMessage(msg)
                Return Nothing
            End Try

            Return sw

        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Close a CSV file writer.
        ''' </summary>
        ''' <param name="sw">The writer to close.</param>
        ''' <param name="strPath ">The path to the file of the writer.</param>
        ''' <returns>True if successful.</returns>
        ''' -------------------------------------------------------------------
        Protected Function CloseWriter(sw As StreamWriter, strPath As String) As Boolean

            Dim msg As cMessage = Nothing
            Dim bSuccess As Boolean = True

            Try
                sw.Flush()
                sw.Close()
                msg = Me.SuccessMessage(strPath)

            Catch ex As Exception
                msg = Me.ErrorMessage(strPath, ex.Message)
                bSuccess = False
            End Try

            Me.m_core.Messages.SendMessage(msg)
            Return bSuccess

        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Write CSV header information.
        ''' </summary>
        ''' <param name="writer">Writer to write to. Yippee.</param>
        ''' <param name="ass">Type of MSY <see cref="eMSYAssessmentTypes"/>.</param>
        ''' <param name="strRun">Name of the run</param>
        ''' -------------------------------------------------------------------
        Protected Overridable Sub WriteHeader(writer As StreamWriter,
                                              ass As eMSYAssessmentTypes,
                                              strRun As String)

            If (writer Is Nothing) Then Return

            ' File
            If Me.m_core.SaveWithFileHeader Then
                writer.WriteLine(Me.m_core.DefaultFileHeader(eAutosaveTypes.MSY))
                writer.WriteLine()
                writer.WriteLine("MSY run," & cStringUtils.ToCSVField(strRun))
                writer.Write("Assessment,")
                Select Case ass
                    Case eMSYAssessmentTypes.StationarySystem
                        writer.WriteLine("stationary_stock")
                    Case eMSYAssessmentTypes.FullCompensation
                        writer.WriteLine("full_compensation")
                    Case Else
                        Debug.Assert(False)
                End Select
            End If

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Message to use to report an error.
        ''' </summary>
        ''' <param name="strPath">Output file name.</param>
        ''' <param name="strReason">Reason of failure, most likely the text obtained from an exception.</param>
        ''' <returns>The message to use to report an error.</returns>
        ''' -------------------------------------------------------------------
        Protected MustOverride Function ErrorMessage(strPath As String, strReason As String) As cMessage

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Message to use to report a succes.
        ''' </summary>
        ''' <param name="strPath">Output file name.</param>
        ''' -------------------------------------------------------------------
        Protected MustOverride Function SuccessMessage(strPath As String) As cMessage

#End Region ' Internals

    End Class

End Namespace
