'==============================================================================
'
' $Log: cFormPositionSetting.vb,v $
' Revision 1.1  2008/09/26 07:31:22  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.1  2008/08/14 01:50:21  jeroens
' Initial version
'
'==============================================================================

#Region " Imports directive "

Option Strict On
Imports System.Text
Imports System.Windows.Forms
Imports System.Collections.Specialized

#End Region ' Imports directive

''' ===========================================================================
''' <summary>
''' Handy-dandy class that maintains and applies form position information, and
''' allows these settings to be stored and read from Application settings.
''' </summary>
''' ===========================================================================
Public Class cFormPositionSettings

#Region " Helper classes "

    ''' =======================================================================
    ''' <summary>
    ''' Helper class, holds and applies form position information for a single form.
    ''' </summary>
    ''' =======================================================================
    Private Class cFormPosition

#Region " Private vars "

        Private m_iPosX As Integer = 0
        Private m_iPosY As Integer = 0
        Private m_iWidth As Integer = 0
        Private m_iHeight As Integer = 0
        Private m_dockState As DockStyle = DockStyle.None
        Private m_formState As FormWindowState = FormWindowState.Normal

#End Region ' Private vars

#Region " Constructor "

        Public Sub New()
        End Sub

#End Region ' Constructor

#Region " Public bits "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Position the form according to this setting.
        ''' </summary>
        ''' <param name="frm">The form to position.</param>
        ''' -------------------------------------------------------------------
        Public Sub Apply(ByVal frm As Form)
            frm.SuspendLayout()
            If frm.Parent Is Nothing Then
                frm.DesktopBounds = New Rectangle(Me.m_iPosX, Me.m_iPosY, Me.m_iWidth, Me.m_iHeight)
            Else
                frm.Location = New Point(Me.m_iPosX, Me.m_iPosY)
                frm.Width = Me.m_iWidth
                frm.Height = Me.m_iHeight
            End If
            frm.Dock = Me.m_dockState
            frm.WindowState = Me.m_formState
            frm.ResumeLayout()
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Initialize by reading position information from a given form.
        ''' </summary>
        ''' <param name="frm">The form to read.</param>
        ''' <returns>True if succesful.</returns>
        ''' -------------------------------------------------------------------
        Public Function Initialize(ByVal frm As Form) As Boolean
            Dim rc As Rectangle = Nothing

            If frm Is Nothing Then Return False

            If frm.Parent Is Nothing Then
                rc = frm.DesktopBounds
            Else
                rc = frm.RestoreBounds
            End If
            Me.m_iPosX = rc.X
            Me.m_iPosY = rc.Y
            Me.m_iWidth = rc.Width
            Me.m_iHeight = rc.Height
            Me.m_dockState = frm.Dock
            Me.m_formState = frm.WindowState
            Return True
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Initialize by reading a settings string.
        ''' </summary>
        ''' <param name="strSetting">The string to read.</param>
        ''' <returns>True if succesful.</returns>
        ''' -------------------------------------------------------------------
        Public Function Initialize(ByVal strSetting As String) As Boolean
            Dim asValue As String() = strSetting.Split(","c)
            Try
                Me.m_iPosX = CInt(Val(asValue(0)))
                Me.m_iPosY = CInt(Val(asValue(1)))
                Me.m_iWidth = CInt(Val(asValue(2)))
                Me.m_iHeight = CInt(Val(asValue(3)))
                Me.m_dockState = CType(Val(asValue(4)), DockStyle)
                Me.m_formState = CType(Val(asValue(5)), FormWindowState)
            Catch ex As Exception
                Return False
            End Try
            Return True
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Produce a settings string.
        ''' </summary>
        ''' <returns>A settings string.</returns>
        ''' -------------------------------------------------------------------
        Public Overrides Function ToString() As String
            Return CStr(Me.m_iPosX) & "," & CStr(Me.m_iPosY) & "," & CStr(Me.m_iWidth) & "," & CStr(Me.m_iHeight) & "," & CStr(CInt(Me.m_dockState)) & "," & CStr(CInt(Me.m_formState))
        End Function

#End Region ' Public bits

    End Class

#End Region ' Helper classes

#Region " Private vars "

    ''' <summary>Singleton instance.</summary>
    Private Shared __inst__ As cFormPositionSettings
    ''' <summary>All maintained form positions.</summary>
    Private m_dictFormPositions As New Dictionary(Of String, cFormPosition)

#End Region ' Private vars

#Region " Singleton "

    ''' -----------------------------------------------------------------------
    ''' <summary>Enforced singleton.</summary>
    ''' -----------------------------------------------------------------------
    Private Sub New()
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Returns the singleton instance of this class.
    ''' </summary>
    ''' <returns>The singleton instance of this class.</returns>
    ''' -----------------------------------------------------------------------
    Public Shared Function GetInstance() As cFormPositionSettings
        If __inst__ Is Nothing Then __inst__ = New cFormPositionSettings()
        Return __inst__
    End Function

#End Region ' Singleton

#Region " Public interfaces "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the setting to maintain in this class
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property Setting() As Specialized.StringCollection
        Get
            Return Me.ToCollection()
        End Get
        Set(ByVal value As Specialized.StringCollection)
            Me.Initialize(value)
        End Set
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Store a forms' position.
    ''' </summary>
    ''' <param name="frm">The form to store the position for.</param>
    ''' -----------------------------------------------------------------------
    Public Sub Store(ByVal frm As Form, Optional ByVal bIncludeFormText As Boolean = True)

        Dim fs As cFormPosition = Nothing
        Dim strFormType As String = ""

        ' Sanity check
        If frm Is Nothing Then Return
        strFormType = FormTypeString(frm, bIncludeFormText)

        ' Already has it?
        If Me.m_dictFormPositions.ContainsKey(strFormType) Then
            ' Obliterate
            Me.m_dictFormPositions.Remove(strFormType)
        End If

        ' Create form state
        fs = New cFormPosition()
        ' Able to read from form?
        If fs.Initialize(frm) Then
            ' #Yes: store it
            Me.m_dictFormPositions(strFormType) = fs
        End If

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Update a forms' position from the position information held in this class.
    ''' </summary>
    ''' <param name="frm">The form to reposition.</param>
    ''' -----------------------------------------------------------------------
    Public Sub Apply(ByVal frm As Form, Optional ByVal bIncludeFormText As Boolean = True)

        Dim strFormType As String = ""
        ' Sanity check
        If frm Is Nothing Then Return
        strFormType = FormTypeString(frm, bIncludeFormText)
        ' Get info
        If Me.m_dictFormPositions.ContainsKey(strFormType) Then
            ' Apply
            Me.m_dictFormPositions(strFormType).Apply(frm)
        End If
    End Sub

#End Region ' Public interfaces

#Region " Private bits "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Load this class from application settings.
    ''' </summary>
    ''' <param name="sc">The string collection to analyze.</param>
    ''' -----------------------------------------------------------------------
    Private Sub Initialize(ByVal sc As StringCollection)

        Dim fp As cFormPosition = Nothing
        Dim astrSetting() As String = Nothing

        ' Clear!
        Me.m_dictFormPositions.Clear()

        ' Sanity checks
        If sc Is Nothing Then Return
        If sc.Count = 0 Then Return

        ' For every form setting
        For Each strFormSetting As String In sc
            ' Is valid?
            If Not String.IsNullOrEmpty(strFormSetting) Then
                ' #Yes: process
                Try
                    ' Split in {formname}={{position} bits
                    astrSetting = strFormSetting.Split("="c)
                    ' 
                    fp = New cFormPosition()
                    ' Can read form position data?
                    If fp.Initialize(astrSetting(1)) Then
                        ' #Yes: store in local admin!
                        Me.m_dictFormPositions(astrSetting(0)) = fp
                    End If
                Catch ex As Exception
                    ' Woops - ignore malformed setting
                End Try
            End If
        Next

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Generate collection from data in local dictionary.
    ''' </summary>
    ''' <returns>A penguin. Really.</returns>
    ''' -----------------------------------------------------------------------
    Private Overloads Function ToCollection() As StringCollection
        Dim sc As New StringCollection()
        Dim strEntry As String = ""

        For Each strFormName As String In Me.m_dictFormPositions.Keys
            sc.Add(String.Format("{0}={1}", strFormName, Me.m_dictFormPositions(strFormName).ToString()))
        Next
        Return sc
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Helper method, builds a string to 'uniquely' identify a form instance.
    ''' </summary>
    ''' <param name="frm">The form to identify.</param>
    ''' <returns>A string uniquely identifying a form instance.</returns>
    ''' -----------------------------------------------------------------------
    Private Function FormTypeString(ByVal frm As Form, ByVal bIncludeFormText As Boolean) As String
        If bIncludeFormText Then
            Return frm.GetType().FullName & "_" & frm.Text
        Else
            Return frm.GetType().FullName
        End If
    End Function

#End Region ' Private bits

End Class