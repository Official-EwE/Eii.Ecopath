
Imports System.IO

''' <summary>
''' Class to wrap a list of Strategies into an object
''' </summary>
''' <remarks>Strategies "Is A" list of Strategy objects</remarks>
Public Class Strategies
    Inherits List(Of Strategy)
    'ToDo All the code to read and save Strategies could go here instead of scattered around.
    'So the Strategies could load and save them selves

    Private m_dataDir As String

    Public Property DataDirectory As String
        Get
            Return m_dataDir
        End Get
        Set(value As String)
            Me.m_dataDir = value
        End Set
    End Property

    ''' <summary>
    ''' Overwrite default behaviour to delete the Strategy file when removing a Strategy from the list
    ''' </summary>
    ''' <param name="ZeroBasedIndex">Zero based index of the Strategy to remove</param>
    ''' <remarks></remarks>
    Public Shadows Sub RemoveAt(ByVal ZeroBasedIndex As Integer)
        Try
            Dim strategy As Strategy = Me.Item(ZeroBasedIndex)
            MyBase.RemoveAt(ZeroBasedIndex)

            If File.Exists(strategy.FileName) Then
                File.Delete(strategy.FileName)
            End If

        Catch ex As Exception
            Debug.Assert(False, Me.ToString + ".RemoveAt() Exception: " + ex.Message)
        End Try
    End Sub


    Public Shadows Sub Add(StrategyToAdd As Strategy)

        If Not Me.Contains(StrategyToAdd) Then
            MyBase.Add(StrategyToAdd)
        End If


    End Sub


    Public Shadows Function Contains(Item As Strategy) As Boolean

        For Each Strategy As Strategy In Me
            If Item.Name = Strategy.Name And Item.FileName = Strategy.FileName Then
                Return True
            End If
        Next
        Return False

    End Function

End Class
