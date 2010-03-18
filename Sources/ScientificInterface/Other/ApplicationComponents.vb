#Region " Imports "

Option Strict On
Imports System.Reflection
Imports System.Text

#End Region ' Imports

''' -----------------------------------------------------------------------
''' <summary>
''' Helper class, provides all loaded EwE6 components in an array of 
''' <see cref="AssemblyName">AssemblyName</see> instances.
''' </summary>
''' -----------------------------------------------------------------------
Public Class ApplicationComponents

    Private m_lanComponents As New List(Of AssemblyName)

#Region " Helper classes "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Helper class to sort an assembly name list
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Private Class AssemblyNameComparer
        Implements IComparer(Of AssemblyName)

        Public Function Compare(ByVal x As System.Reflection.AssemblyName, ByVal y As System.Reflection.AssemblyName) As Integer Implements System.Collections.Generic.IComparer(Of System.Reflection.AssemblyName).Compare
            Return String.Compare(x.Name, y.Name)
        End Function
    End Class

#End Region ' Helper classes

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Constructor.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Sub New()

        ' List of assemblies that will NOT be shown in this grid
        Dim astrExcludeAssemblies() As String = {"mscorlib", "system", "microsoft"}
        ' Scientific Interface assembly
        Dim assExecuting As Assembly = Assembly.GetExecutingAssembly()
        ' All required assemblies (not actually loaded!)
        Dim anRequired() As AssemblyName = assExecuting.GetReferencedAssemblies()

        Me.m_lanComponents.Add(Assembly.GetEntryAssembly().GetName())

        ' Figure out which assemblies to show
        For Each an As AssemblyName In anRequired
            ' Not already accepted?
            If Not Me.m_lanComponents.Contains(an) Then
                ' #Yes: this is a new assembly
                ' Assume that assembly can be added
                Dim bAddAssembly As Boolean = True
                ' Check if this a blacklisted assembly
                For Each strName As String In astrExcludeAssemblies
                    ' Does name begin with a blacklisted string?
                    If an.FullName.ToLower().IndexOf(strName) = 0 Then
                        ' #Yes: can not add assembly
                        bAddAssembly = False
                    End If
                Next
                ' So what did the jury decide?
                If bAddAssembly Then Me.m_lanComponents.Add(an)
            End If
        Next

        ' Sort accepted assembly list
        Me.m_lanComponents.Sort(New AssemblyNameComparer())

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Returns an array of required components.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Function RequiredComponents() As AssemblyName()
        Return Me.m_lanComponents.ToArray()
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Returns a formatted version of this monstrosity.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Overrides Function ToString() As String
        Dim sb As New StringBuilder()
        For Each an As AssemblyName In Me.m_lanComponents
            sb.AppendLine(String.Format("{0}={1}", an.Name, an.Version))
        Next
        Return sb.ToString()
    End Function

End Class
