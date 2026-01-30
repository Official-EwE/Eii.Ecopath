' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Module XAlglib

    '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    '    Callback definitions for optimizers/fitters/solvers.
    '    
    '    Callbacks for unparameterized (general) functions:
    '    * ndimensional_func         calculates f(arg), stores result to func
    '    * ndimensional_grad         calculates func = f(arg), 
    '                                grad[i] = df(arg)/d(arg[i])
    '    * ndimensional_hess         calculates func = f(arg),
    '                                grad[i] = df(arg)/d(arg[i]),
    '                                hess[i,j] = d2f(arg)/(d(arg[i])*d(arg[j]))
    '    
    '    Callbacks for systems of functions:
    '    * ndimensional_fvec         calculates vector function f(arg),
    '                                stores result to fi
    '    * ndimensional_jac          calculates f[i] = fi(arg)
    '                                jac[i,j] = df[i](arg)/d(arg[j])
    '                                
    '    Callbacks for  parameterized  functions,  i.e.  for  functions  which 
    '    depend on two vectors: P and Q.  Gradient  and Hessian are calculated 
    '    with respect to P only.
    '    * ndimensional_pfunc        calculates f(p,q),
    '                                stores result to func
    '    * ndimensional_pgrad        calculates func = f(p,q),
    '                                grad[i] = df(p,q)/d(p[i])
    '    * ndimensional_phess        calculates func = f(p,q),
    '                                grad[i] = df(p,q)/d(p[i]),
    '                                hess[i,j] = d2f(p,q)/(d(p[i])*d(p[j]))
    '
    '    Callbacks for progress reports:
    '    * ndimensional_rep          reports current position of optimization algo    
    '    
    '    Callbacks for ODE solvers:
    '    * ndimensional_ode_rp       calculates dy/dx for given y[] and x
    '    
    '    Callbacks for integrators:
    '    * integrator1_func          calculates f(x) for given x
    '                                (additional parameters xminusa and bminusx
    '                                contain x-a and b-x)
    '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    Public Delegate Sub ndimensional_func(arg As Double(), ByRef func As Double, obj As Object)
    Public Delegate Sub ndimensional_grad(arg As Double(), ByRef func As Double, grad As Double(), obj As Object)
    Public Delegate Sub ndimensional_hess(arg As Double(), ByRef func As Double, grad As Double(), hess As Double(,), obj As Object)

    Public Delegate Sub ndimensional_fvec(arg As Double(), fi As Double(), obj As Object)
    Public Delegate Sub ndimensional_jac(arg As Double(), fi As Double(), jac As Double(,), obj As Object)

    Public Delegate Sub ndimensional_pfunc(p As Double(), q As Double(), ByRef func As Double, obj As Object)
    Public Delegate Sub ndimensional_pgrad(p As Double(), q As Double(), ByRef func As Double, grad As Double(), obj As Object)
    Public Delegate Sub ndimensional_phess(p As Double(), q As Double(), ByRef func As Double, grad As Double(), hess As Double(,), obj As Object)

    Public Delegate Sub ndimensional_rep(arg As Double(), func As Double, obj As Object)

    Public Delegate Sub ndimensional_ode_rp(y As Double(), x As Double, dy As Double(), obj As Object)

    Public Delegate Sub integrator1_func(x As Double, xminusa As Double, bminusx As Double, ByRef f As Double, obj As Object)

    '
    ' ALGLIB exception
    '
    Public Class AlglibException
        Inherits System.ApplicationException
        Public Sub New(message As String)
            MyBase.New(message)
        End Sub
    End Class


    Public Class hqrndstate
        Public csobj As alglib.hqrndstate
    End Class


    Public Sub hqrndrandomize(ByRef state As hqrndstate)
        Try
            state = New hqrndstate()
            alglib.hqrndrandomize(state.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub hqrndseed(s1 As Integer, s2 As Integer, ByRef state As hqrndstate)
        Try
            state = New hqrndstate()
            alglib.hqrndseed(s1, s2, state.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Function hqrnduniformr(state As hqrndstate) As Double
        Try
            hqrnduniformr = alglib.hqrnduniformr(state.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Function


    Public Function hqrnduniformi(state As hqrndstate, n As Integer) As Integer
        Try
            hqrnduniformi = alglib.hqrnduniformi(state.csobj, n)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Function


    Public Function hqrndnormal(state As hqrndstate) As Double
        Try
            hqrndnormal = alglib.hqrndnormal(state.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Function


    Public Sub hqrndunit2(state As hqrndstate, ByRef x As Double, ByRef y As Double)
        Try
            alglib.hqrndunit2(state.csobj, x, y)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub hqrndnormal2(state As hqrndstate, ByRef x1 As Double, ByRef x2 As Double)
        Try
            alglib.hqrndnormal2(state.csobj, x1, x2)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Function hqrndexponential(state As hqrndstate, lambdav As Double) As Double
        Try
            hqrndexponential = alglib.hqrndexponential(state.csobj, lambdav)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Function


    Public Function hqrnddiscrete(state As hqrndstate, x() As Double, n As Integer) As Double
        Try
            hqrnddiscrete = alglib.hqrnddiscrete(state.csobj, x, n)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Function


    Public Function hqrndcontinuous(state As hqrndstate, x() As Double, n As Integer) As Double
        Try
            hqrndcontinuous = alglib.hqrndcontinuous(state.csobj, x, n)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Function

    Public Class kdtree
        Public csobj As alglib.kdtree
    End Class
    Public Sub kdtreeserialize(obj As kdtree, ByRef s_out As String)
        Try
            alglib.kdtreeserialize(obj.csobj, s_out)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub

    Public Sub kdtreeunserialize(s_in As String, ByRef obj As kdtree)
        Try
            alglib.kdtreeunserialize(s_in, obj.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub kdtreebuild(xy(,) As Double, n As Integer, nx As Integer, ny As Integer, normtype As Integer, ByRef kdt As kdtree)
        Try
            kdt = New kdtree()
            alglib.kdtreebuild(xy, n, nx, ny, normtype, kdt.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub kdtreebuild(xy(,) As Double, nx As Integer, ny As Integer, normtype As Integer, ByRef kdt As kdtree)
        Try
            kdt = New kdtree()
            alglib.kdtreebuild(xy, nx, ny, normtype, kdt.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub kdtreebuildtagged(xy(,) As Double, tags() As Integer, n As Integer, nx As Integer, ny As Integer, normtype As Integer, ByRef kdt As kdtree)
        Try
            kdt = New kdtree()
            alglib.kdtreebuildtagged(xy, tags, n, nx, ny, normtype, kdt.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub kdtreebuildtagged(xy(,) As Double, tags() As Integer, nx As Integer, ny As Integer, normtype As Integer, ByRef kdt As kdtree)
        Try
            kdt = New kdtree()
            alglib.kdtreebuildtagged(xy, tags, nx, ny, normtype, kdt.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Function kdtreequeryknn(kdt As kdtree, x() As Double, k As Integer, selfmatch As Boolean) As Integer
        Try
            kdtreequeryknn = alglib.kdtreequeryknn(kdt.csobj, x, k, selfmatch)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Function


    Public Function kdtreequeryknn(kdt As kdtree, x() As Double, k As Integer) As Integer
        Try
            kdtreequeryknn = alglib.kdtreequeryknn(kdt.csobj, x, k)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Function


    Public Function kdtreequeryrnn(kdt As kdtree, x() As Double, r As Double, selfmatch As Boolean) As Integer
        Try
            kdtreequeryrnn = alglib.kdtreequeryrnn(kdt.csobj, x, r, selfmatch)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Function


    Public Function kdtreequeryrnn(kdt As kdtree, x() As Double, r As Double) As Integer
        Try
            kdtreequeryrnn = alglib.kdtreequeryrnn(kdt.csobj, x, r)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Function


    Public Function kdtreequeryaknn(kdt As kdtree, x() As Double, k As Integer, selfmatch As Boolean, eps As Double) As Integer
        Try
            kdtreequeryaknn = alglib.kdtreequeryaknn(kdt.csobj, x, k, selfmatch, eps)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Function


    Public Function kdtreequeryaknn(kdt As kdtree, x() As Double, k As Integer, eps As Double) As Integer
        Try
            kdtreequeryaknn = alglib.kdtreequeryaknn(kdt.csobj, x, k, eps)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Function


    Public Sub kdtreequeryresultsx(kdt As kdtree, ByRef x(,) As Double)
        Try
            alglib.kdtreequeryresultsx(kdt.csobj, x)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub kdtreequeryresultsxy(kdt As kdtree, ByRef xy(,) As Double)
        Try
            alglib.kdtreequeryresultsxy(kdt.csobj, xy)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub kdtreequeryresultstags(kdt As kdtree, ByRef tags() As Integer)
        Try
            alglib.kdtreequeryresultstags(kdt.csobj, tags)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub kdtreequeryresultsdistances(kdt As kdtree, ByRef r() As Double)
        Try
            alglib.kdtreequeryresultsdistances(kdt.csobj, r)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub kdtreequeryresultsxi(kdt As kdtree, ByRef x(,) As Double)
        Try
            alglib.kdtreequeryresultsxi(kdt.csobj, x)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub kdtreequeryresultsxyi(kdt As kdtree, ByRef xy(,) As Double)
        Try
            alglib.kdtreequeryresultsxyi(kdt.csobj, xy)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub kdtreequeryresultstagsi(kdt As kdtree, ByRef tags() As Integer)
        Try
            alglib.kdtreequeryresultstagsi(kdt.csobj, tags)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub kdtreequeryresultsdistancesi(kdt As kdtree, ByRef r() As Double)
        Try
            alglib.kdtreequeryresultsdistancesi(kdt.csobj, r)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub




    Public Sub cmatrixtranspose(m As Integer, n As Integer, a(,) As alglib.complex, ia As Integer, ja As Integer, ByRef b(,) As alglib.complex, ib As Integer, jb As Integer)
        Try
            alglib.cmatrixtranspose(m, n, a, ia, ja, b, ib, jb)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub rmatrixtranspose(m As Integer, n As Integer, a(,) As Double, ia As Integer, ja As Integer, ByRef b(,) As Double, ib As Integer, jb As Integer)
        Try
            alglib.rmatrixtranspose(m, n, a, ia, ja, b, ib, jb)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub cmatrixcopy(m As Integer, n As Integer, a(,) As alglib.complex, ia As Integer, ja As Integer, ByRef b(,) As alglib.complex, ib As Integer, jb As Integer)
        Try
            alglib.cmatrixcopy(m, n, a, ia, ja, b, ib, jb)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub rmatrixcopy(m As Integer, n As Integer, a(,) As Double, ia As Integer, ja As Integer, ByRef b(,) As Double, ib As Integer, jb As Integer)
        Try
            alglib.rmatrixcopy(m, n, a, ia, ja, b, ib, jb)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub cmatrixrank1(m As Integer, n As Integer, ByRef a(,) As alglib.complex, ia As Integer, ja As Integer, ByRef u() As alglib.complex, iu As Integer, ByRef v() As alglib.complex, iv As Integer)
        Try
            alglib.cmatrixrank1(m, n, a, ia, ja, u, iu, v, iv)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub rmatrixrank1(m As Integer, n As Integer, ByRef a(,) As Double, ia As Integer, ja As Integer, ByRef u() As Double, iu As Integer, ByRef v() As Double, iv As Integer)
        Try
            alglib.rmatrixrank1(m, n, a, ia, ja, u, iu, v, iv)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub cmatrixmv(m As Integer, n As Integer, a(,) As alglib.complex, ia As Integer, ja As Integer, opa As Integer, x() As alglib.complex, ix As Integer, ByRef y() As alglib.complex, iy As Integer)
        Try
            alglib.cmatrixmv(m, n, a, ia, ja, opa, x, ix, y, iy)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub rmatrixmv(m As Integer, n As Integer, a(,) As Double, ia As Integer, ja As Integer, opa As Integer, x() As Double, ix As Integer, ByRef y() As Double, iy As Integer)
        Try
            alglib.rmatrixmv(m, n, a, ia, ja, opa, x, ix, y, iy)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub cmatrixrighttrsm(m As Integer, n As Integer, a(,) As alglib.complex, i1 As Integer, j1 As Integer, isupper As Boolean, isunit As Boolean, optype As Integer, ByRef x(,) As alglib.complex, i2 As Integer, j2 As Integer)
        Try
            alglib.cmatrixrighttrsm(m, n, a, i1, j1, isupper, isunit, optype, x, i2, j2)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub cmatrixlefttrsm(m As Integer, n As Integer, a(,) As alglib.complex, i1 As Integer, j1 As Integer, isupper As Boolean, isunit As Boolean, optype As Integer, ByRef x(,) As alglib.complex, i2 As Integer, j2 As Integer)
        Try
            alglib.cmatrixlefttrsm(m, n, a, i1, j1, isupper, isunit, optype, x, i2, j2)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub rmatrixrighttrsm(m As Integer, n As Integer, a(,) As Double, i1 As Integer, j1 As Integer, isupper As Boolean, isunit As Boolean, optype As Integer, ByRef x(,) As Double, i2 As Integer, j2 As Integer)
        Try
            alglib.rmatrixrighttrsm(m, n, a, i1, j1, isupper, isunit, optype, x, i2, j2)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub rmatrixlefttrsm(m As Integer, n As Integer, a(,) As Double, i1 As Integer, j1 As Integer, isupper As Boolean, isunit As Boolean, optype As Integer, ByRef x(,) As Double, i2 As Integer, j2 As Integer)
        Try
            alglib.rmatrixlefttrsm(m, n, a, i1, j1, isupper, isunit, optype, x, i2, j2)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub cmatrixsyrk(n As Integer, k As Integer, alpha As Double, a(,) As alglib.complex, ia As Integer, ja As Integer, optypea As Integer, beta As Double, ByRef c(,) As alglib.complex, ic As Integer, jc As Integer, isupper As Boolean)
        Try
            alglib.cmatrixsyrk(n, k, alpha, a, ia, ja, optypea, beta, c, ic, jc, isupper)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub rmatrixsyrk(n As Integer, k As Integer, alpha As Double, a(,) As Double, ia As Integer, ja As Integer, optypea As Integer, beta As Double, ByRef c(,) As Double, ic As Integer, jc As Integer, isupper As Boolean)
        Try
            alglib.rmatrixsyrk(n, k, alpha, a, ia, ja, optypea, beta, c, ic, jc, isupper)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub cmatrixgemm(m As Integer, n As Integer, k As Integer, alpha As alglib.complex, a(,) As alglib.complex, ia As Integer, ja As Integer, optypea As Integer, b(,) As alglib.complex, ib As Integer, jb As Integer, optypeb As Integer, beta As alglib.complex, ByRef c(,) As alglib.complex, ic As Integer, jc As Integer)
        Try
            alglib.cmatrixgemm(m, n, k, alpha, a, ia, ja, optypea, b, ib, jb, optypeb, beta, c, ic, jc)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub rmatrixgemm(m As Integer, n As Integer, k As Integer, alpha As Double, a(,) As Double, ia As Integer, ja As Integer, optypea As Integer, b(,) As Double, ib As Integer, jb As Integer, optypeb As Integer, beta As Double, ByRef c(,) As Double, ic As Integer, jc As Integer)
        Try
            alglib.rmatrixgemm(m, n, k, alpha, a, ia, ja, optypea, b, ib, jb, optypeb, beta, c, ic, jc)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub




    Public Sub samplemoments(x() As Double, n As Integer, ByRef mean As Double, ByRef variance As Double, ByRef skewness As Double, ByRef kurtosis As Double)
        Try
            alglib.samplemoments(x, n, mean, variance, skewness, kurtosis)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub samplemoments(x() As Double, ByRef mean As Double, ByRef variance As Double, ByRef skewness As Double, ByRef kurtosis As Double)
        Try
            alglib.samplemoments(x, mean, variance, skewness, kurtosis)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Function samplemean(x() As Double, n As Integer) As Double
        Try
            samplemean = alglib.samplemean(x, n)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Function


    Public Function samplemean(x() As Double) As Double
        Try
            samplemean = alglib.samplemean(x)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Function


    Public Function samplevariance(x() As Double, n As Integer) As Double
        Try
            samplevariance = alglib.samplevariance(x, n)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Function


    Public Function samplevariance(x() As Double) As Double
        Try
            samplevariance = alglib.samplevariance(x)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Function


    Public Function sampleskewness(x() As Double, n As Integer) As Double
        Try
            sampleskewness = alglib.sampleskewness(x, n)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Function


    Public Function sampleskewness(x() As Double) As Double
        Try
            sampleskewness = alglib.sampleskewness(x)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Function


    Public Function samplekurtosis(x() As Double, n As Integer) As Double
        Try
            samplekurtosis = alglib.samplekurtosis(x, n)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Function


    Public Function samplekurtosis(x() As Double) As Double
        Try
            samplekurtosis = alglib.samplekurtosis(x)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Function


    Public Sub sampleadev(x() As Double, n As Integer, ByRef adev As Double)
        Try
            alglib.sampleadev(x, n, adev)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub sampleadev(x() As Double, ByRef adev As Double)
        Try
            alglib.sampleadev(x, adev)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub samplemedian(x() As Double, n As Integer, ByRef median As Double)
        Try
            alglib.samplemedian(x, n, median)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub samplemedian(x() As Double, ByRef median As Double)
        Try
            alglib.samplemedian(x, median)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub samplepercentile(x() As Double, n As Integer, p As Double, ByRef v As Double)
        Try
            alglib.samplepercentile(x, n, p, v)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub samplepercentile(x() As Double, p As Double, ByRef v As Double)
        Try
            alglib.samplepercentile(x, p, v)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Function cov2(x() As Double, y() As Double, n As Integer) As Double
        Try
            cov2 = alglib.cov2(x, y, n)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Function


    Public Function cov2(x() As Double, y() As Double) As Double
        Try
            cov2 = alglib.cov2(x, y)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Function


    Public Function pearsoncorr2(x() As Double, y() As Double, n As Integer) As Double
        Try
            pearsoncorr2 = alglib.pearsoncorr2(x, y, n)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Function


    Public Function pearsoncorr2(x() As Double, y() As Double) As Double
        Try
            pearsoncorr2 = alglib.pearsoncorr2(x, y)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Function


    Public Function spearmancorr2(x() As Double, y() As Double, n As Integer) As Double
        Try
            spearmancorr2 = alglib.spearmancorr2(x, y, n)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Function


    Public Function spearmancorr2(x() As Double, y() As Double) As Double
        Try
            spearmancorr2 = alglib.spearmancorr2(x, y)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Function


    Public Sub covm(x(,) As Double, n As Integer, m As Integer, ByRef c(,) As Double)
        Try
            alglib.covm(x, n, m, c)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub covm(x(,) As Double, ByRef c(,) As Double)
        Try
            alglib.covm(x, c)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub pearsoncorrm(x(,) As Double, n As Integer, m As Integer, ByRef c(,) As Double)
        Try
            alglib.pearsoncorrm(x, n, m, c)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub pearsoncorrm(x(,) As Double, ByRef c(,) As Double)
        Try
            alglib.pearsoncorrm(x, c)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub spearmancorrm(x(,) As Double, n As Integer, m As Integer, ByRef c(,) As Double)
        Try
            alglib.spearmancorrm(x, n, m, c)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub spearmancorrm(x(,) As Double, ByRef c(,) As Double)
        Try
            alglib.spearmancorrm(x, c)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub covm2(x(,) As Double, y(,) As Double, n As Integer, m1 As Integer, m2 As Integer, ByRef c(,) As Double)
        Try
            alglib.covm2(x, y, n, m1, m2, c)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub covm2(x(,) As Double, y(,) As Double, ByRef c(,) As Double)
        Try
            alglib.covm2(x, y, c)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub pearsoncorrm2(x(,) As Double, y(,) As Double, n As Integer, m1 As Integer, m2 As Integer, ByRef c(,) As Double)
        Try
            alglib.pearsoncorrm2(x, y, n, m1, m2, c)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub pearsoncorrm2(x(,) As Double, y(,) As Double, ByRef c(,) As Double)
        Try
            alglib.pearsoncorrm2(x, y, c)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub spearmancorrm2(x(,) As Double, y(,) As Double, n As Integer, m1 As Integer, m2 As Integer, ByRef c(,) As Double)
        Try
            alglib.spearmancorrm2(x, y, n, m1, m2, c)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub spearmancorrm2(x(,) As Double, y(,) As Double, ByRef c(,) As Double)
        Try
            alglib.spearmancorrm2(x, y, c)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Function pearsoncorrelation(x() As Double, y() As Double, n As Integer) As Double
        Try
            pearsoncorrelation = alglib.pearsoncorrelation(x, y, n)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Function


    Public Function spearmanrankcorrelation(x() As Double, y() As Double, n As Integer) As Double
        Try
            spearmanrankcorrelation = alglib.spearmanrankcorrelation(x, y, n)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Function




    Public Sub dsoptimalsplit2(a() As Double, c() As Integer, n As Integer, ByRef info As Integer, ByRef threshold As Double, ByRef pal As Double, ByRef pbl As Double, ByRef par As Double, ByRef pbr As Double, ByRef cve As Double)
        Try
            alglib.dsoptimalsplit2(a, c, n, info, threshold, pal, pbl, par, pbr, cve)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub dsoptimalsplit2fast(ByRef a() As Double, ByRef c() As Integer, ByRef tiesbuf() As Integer, ByRef cntbuf() As Integer, ByRef bufr() As Double, ByRef bufi() As Integer, n As Integer, nc As Integer, alpha As Double, ByRef info As Integer, ByRef threshold As Double, ByRef rms As Double, ByRef cvrms As Double)
        Try
            alglib.dsoptimalsplit2fast(a, c, tiesbuf, cntbuf, bufr, bufi, n, nc, alpha, info, threshold, rms, cvrms)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub

    Public Class decisionforest
        Public csobj As alglib.decisionforest
    End Class
    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    '
    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    Public Class dfreport
        Public Property relclserror() As Double
            Get
                Return Me.csobj.relclserror
            End Get
            Set(Value As Double)
                Me.csobj.relclserror = Value
            End Set
        End Property
        Public Property avgce() As Double
            Get
                Return Me.csobj.avgce
            End Get
            Set(Value As Double)
                Me.csobj.avgce = Value
            End Set
        End Property
        Public Property rmserror() As Double
            Get
                Return Me.csobj.rmserror
            End Get
            Set(Value As Double)
                Me.csobj.rmserror = Value
            End Set
        End Property
        Public Property avgerror() As Double
            Get
                Return Me.csobj.avgerror
            End Get
            Set(Value As Double)
                Me.csobj.avgerror = Value
            End Set
        End Property
        Public Property avgrelerror() As Double
            Get
                Return Me.csobj.avgrelerror
            End Get
            Set(Value As Double)
                Me.csobj.avgrelerror = Value
            End Set
        End Property
        Public Property oobrelclserror() As Double
            Get
                Return Me.csobj.oobrelclserror
            End Get
            Set(Value As Double)
                Me.csobj.oobrelclserror = Value
            End Set
        End Property
        Public Property oobavgce() As Double
            Get
                Return Me.csobj.oobavgce
            End Get
            Set(Value As Double)
                Me.csobj.oobavgce = Value
            End Set
        End Property
        Public Property oobrmserror() As Double
            Get
                Return Me.csobj.oobrmserror
            End Get
            Set(Value As Double)
                Me.csobj.oobrmserror = Value
            End Set
        End Property
        Public Property oobavgerror() As Double
            Get
                Return Me.csobj.oobavgerror
            End Get
            Set(Value As Double)
                Me.csobj.oobavgerror = Value
            End Set
        End Property
        Public Property oobavgrelerror() As Double
            Get
                Return Me.csobj.oobavgrelerror
            End Get
            Set(Value As Double)
                Me.csobj.oobavgrelerror = Value
            End Set
        End Property
        Public csobj As alglib.dfreport
    End Class
    Public Sub dfserialize(obj As decisionforest, ByRef s_out As String)
        Try
            alglib.dfserialize(obj.csobj, s_out)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub

    Public Sub dfunserialize(s_in As String, ByRef obj As decisionforest)
        Try
            alglib.dfunserialize(s_in, obj.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub dfbuildrandomdecisionforest(xy(,) As Double, npoints As Integer, nvars As Integer, nclasses As Integer, ntrees As Integer, r As Double, ByRef info As Integer, ByRef df As decisionforest, ByRef rep As dfreport)
        Try
            df = New decisionforest()
            rep = New dfreport()
            alglib.dfbuildrandomdecisionforest(xy, npoints, nvars, nclasses, ntrees, r, info, df.csobj, rep.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub dfbuildrandomdecisionforestx1(xy(,) As Double, npoints As Integer, nvars As Integer, nclasses As Integer, ntrees As Integer, nrndvars As Integer, r As Double, ByRef info As Integer, ByRef df As decisionforest, ByRef rep As dfreport)
        Try
            df = New decisionforest()
            rep = New dfreport()
            alglib.dfbuildrandomdecisionforestx1(xy, npoints, nvars, nclasses, ntrees, nrndvars, r, info, df.csobj, rep.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub dfprocess(df As decisionforest, x() As Double, ByRef y() As Double)
        Try
            alglib.dfprocess(df.csobj, x, y)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub dfprocessi(df As decisionforest, x() As Double, ByRef y() As Double)
        Try
            alglib.dfprocessi(df.csobj, x, y)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Function dfrelclserror(df As decisionforest, xy(,) As Double, npoints As Integer) As Double
        Try
            dfrelclserror = alglib.dfrelclserror(df.csobj, xy, npoints)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Function


    Public Function dfavgce(df As decisionforest, xy(,) As Double, npoints As Integer) As Double
        Try
            dfavgce = alglib.dfavgce(df.csobj, xy, npoints)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Function


    Public Function dfrmserror(df As decisionforest, xy(,) As Double, npoints As Integer) As Double
        Try
            dfrmserror = alglib.dfrmserror(df.csobj, xy, npoints)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Function


    Public Function dfavgerror(df As decisionforest, xy(,) As Double, npoints As Integer) As Double
        Try
            dfavgerror = alglib.dfavgerror(df.csobj, xy, npoints)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Function


    Public Function dfavgrelerror(df As decisionforest, xy(,) As Double, npoints As Integer) As Double
        Try
            dfavgrelerror = alglib.dfavgrelerror(df.csobj, xy, npoints)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Function




    Public Function gammafunction(x As Double) As Double
        Try
            gammafunction = alglib.gammafunction(x)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Function


    Public Function lngamma(x As Double, ByRef sgngam As Double) As Double
        Try
            lngamma = alglib.lngamma(x, sgngam)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Function




    Public Function errorfunction(x As Double) As Double
        Try
            errorfunction = alglib.errorfunction(x)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Function


    Public Function errorfunctionc(x As Double) As Double
        Try
            errorfunctionc = alglib.errorfunctionc(x)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Function


    Public Function normaldistribution(x As Double) As Double
        Try
            normaldistribution = alglib.normaldistribution(x)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Function


    Public Function inverf(e As Double) As Double
        Try
            inverf = alglib.inverf(e)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Function


    Public Function invnormaldistribution(y0 As Double) As Double
        Try
            invnormaldistribution = alglib.invnormaldistribution(y0)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Function




    Public Function incompletegamma(a As Double, x As Double) As Double
        Try
            incompletegamma = alglib.incompletegamma(a, x)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Function


    Public Function incompletegammac(a As Double, x As Double) As Double
        Try
            incompletegammac = alglib.incompletegammac(a, x)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Function


    Public Function invincompletegammac(a As Double, y0 As Double) As Double
        Try
            invincompletegammac = alglib.invincompletegammac(a, y0)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Function




    Public Sub rmatrixqr(ByRef a(,) As Double, m As Integer, n As Integer, ByRef tau() As Double)
        Try
            alglib.rmatrixqr(a, m, n, tau)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub rmatrixlq(ByRef a(,) As Double, m As Integer, n As Integer, ByRef tau() As Double)
        Try
            alglib.rmatrixlq(a, m, n, tau)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub cmatrixqr(ByRef a(,) As alglib.complex, m As Integer, n As Integer, ByRef tau() As alglib.complex)
        Try
            alglib.cmatrixqr(a, m, n, tau)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub cmatrixlq(ByRef a(,) As alglib.complex, m As Integer, n As Integer, ByRef tau() As alglib.complex)
        Try
            alglib.cmatrixlq(a, m, n, tau)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub rmatrixqrunpackq(a(,) As Double, m As Integer, n As Integer, tau() As Double, qcolumns As Integer, ByRef q(,) As Double)
        Try
            alglib.rmatrixqrunpackq(a, m, n, tau, qcolumns, q)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub rmatrixqrunpackr(a(,) As Double, m As Integer, n As Integer, ByRef r(,) As Double)
        Try
            alglib.rmatrixqrunpackr(a, m, n, r)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub rmatrixlqunpackq(a(,) As Double, m As Integer, n As Integer, tau() As Double, qrows As Integer, ByRef q(,) As Double)
        Try
            alglib.rmatrixlqunpackq(a, m, n, tau, qrows, q)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub rmatrixlqunpackl(a(,) As Double, m As Integer, n As Integer, ByRef l(,) As Double)
        Try
            alglib.rmatrixlqunpackl(a, m, n, l)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub cmatrixqrunpackq(a(,) As alglib.complex, m As Integer, n As Integer, tau() As alglib.complex, qcolumns As Integer, ByRef q(,) As alglib.complex)
        Try
            alglib.cmatrixqrunpackq(a, m, n, tau, qcolumns, q)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub cmatrixqrunpackr(a(,) As alglib.complex, m As Integer, n As Integer, ByRef r(,) As alglib.complex)
        Try
            alglib.cmatrixqrunpackr(a, m, n, r)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub cmatrixlqunpackq(a(,) As alglib.complex, m As Integer, n As Integer, tau() As alglib.complex, qrows As Integer, ByRef q(,) As alglib.complex)
        Try
            alglib.cmatrixlqunpackq(a, m, n, tau, qrows, q)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub cmatrixlqunpackl(a(,) As alglib.complex, m As Integer, n As Integer, ByRef l(,) As alglib.complex)
        Try
            alglib.cmatrixlqunpackl(a, m, n, l)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub rmatrixbd(ByRef a(,) As Double, m As Integer, n As Integer, ByRef tauq() As Double, ByRef taup() As Double)
        Try
            alglib.rmatrixbd(a, m, n, tauq, taup)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub rmatrixbdunpackq(qp(,) As Double, m As Integer, n As Integer, tauq() As Double, qcolumns As Integer, ByRef q(,) As Double)
        Try
            alglib.rmatrixbdunpackq(qp, m, n, tauq, qcolumns, q)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub rmatrixbdmultiplybyq(qp(,) As Double, m As Integer, n As Integer, tauq() As Double, ByRef z(,) As Double, zrows As Integer, zcolumns As Integer, fromtheright As Boolean, dotranspose As Boolean)
        Try
            alglib.rmatrixbdmultiplybyq(qp, m, n, tauq, z, zrows, zcolumns, fromtheright, dotranspose)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub rmatrixbdunpackpt(qp(,) As Double, m As Integer, n As Integer, taup() As Double, ptrows As Integer, ByRef pt(,) As Double)
        Try
            alglib.rmatrixbdunpackpt(qp, m, n, taup, ptrows, pt)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub rmatrixbdmultiplybyp(qp(,) As Double, m As Integer, n As Integer, taup() As Double, ByRef z(,) As Double, zrows As Integer, zcolumns As Integer, fromtheright As Boolean, dotranspose As Boolean)
        Try
            alglib.rmatrixbdmultiplybyp(qp, m, n, taup, z, zrows, zcolumns, fromtheright, dotranspose)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub rmatrixbdunpackdiagonals(b(,) As Double, m As Integer, n As Integer, ByRef isupper As Boolean, ByRef d() As Double, ByRef e() As Double)
        Try
            alglib.rmatrixbdunpackdiagonals(b, m, n, isupper, d, e)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub rmatrixhessenberg(ByRef a(,) As Double, n As Integer, ByRef tau() As Double)
        Try
            alglib.rmatrixhessenberg(a, n, tau)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub rmatrixhessenbergunpackq(a(,) As Double, n As Integer, tau() As Double, ByRef q(,) As Double)
        Try
            alglib.rmatrixhessenbergunpackq(a, n, tau, q)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub rmatrixhessenbergunpackh(a(,) As Double, n As Integer, ByRef h(,) As Double)
        Try
            alglib.rmatrixhessenbergunpackh(a, n, h)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub smatrixtd(ByRef a(,) As Double, n As Integer, isupper As Boolean, ByRef tau() As Double, ByRef d() As Double, ByRef e() As Double)
        Try
            alglib.smatrixtd(a, n, isupper, tau, d, e)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub smatrixtdunpackq(a(,) As Double, n As Integer, isupper As Boolean, tau() As Double, ByRef q(,) As Double)
        Try
            alglib.smatrixtdunpackq(a, n, isupper, tau, q)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub hmatrixtd(ByRef a(,) As alglib.complex, n As Integer, isupper As Boolean, ByRef tau() As alglib.complex, ByRef d() As Double, ByRef e() As Double)
        Try
            alglib.hmatrixtd(a, n, isupper, tau, d, e)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub hmatrixtdunpackq(a(,) As alglib.complex, n As Integer, isupper As Boolean, tau() As alglib.complex, ByRef q(,) As alglib.complex)
        Try
            alglib.hmatrixtdunpackq(a, n, isupper, tau, q)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub




    Public Function rmatrixbdsvd(ByRef d() As Double, e() As Double, n As Integer, isupper As Boolean, isfractionalaccuracyrequired As Boolean, ByRef u(,) As Double, nru As Integer, ByRef c(,) As Double, ncc As Integer, ByRef vt(,) As Double, ncvt As Integer) As Boolean
        Try
            rmatrixbdsvd = alglib.rmatrixbdsvd(d, e, n, isupper, isfractionalaccuracyrequired, u, nru, c, ncc, vt, ncvt)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Function




    Public Function rmatrixsvd(a(,) As Double, m As Integer, n As Integer, uneeded As Integer, vtneeded As Integer, additionalmemory As Integer, ByRef w() As Double, ByRef u(,) As Double, ByRef vt(,) As Double) As Boolean
        Try
            rmatrixsvd = alglib.rmatrixsvd(a, m, n, uneeded, vtneeded, additionalmemory, w, u, vt)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Function

    Public Class linearmodel
        Public csobj As alglib.linearmodel
    End Class
    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    'LRReport structure contains additional information about linear model:
    '* C             -   covariation matrix,  array[0..NVars,0..NVars].
    '                    C[i,j] = Cov(A[i],A[j])
    '* RMSError      -   root mean square error on a training set
    '* AvgError      -   average error on a training set
    '* AvgRelError   -   average relative error on a training set (excluding
    '                    observations with zero function value).
    '* CVRMSError    -   leave-one-out cross-validation estimate of
    '                    generalization error. Calculated using fast algorithm
    '                    with O(NVars*NPoints) complexity.
    '* CVAvgError    -   cross-validation estimate of average error
    '* CVAvgRelError -   cross-validation estimate of average relative error
    '
    'All other fields of the structure are intended for internal use and should
    'not be used outside ALGLIB.
    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    Public Class lrreport
        Public Property c() As Double(,)
            Get
                Return Me.csobj.c
            End Get
            Set(Value As Double(,))
                Me.csobj.c = Value
            End Set
        End Property
        Public Property rmserror() As Double
            Get
                Return Me.csobj.rmserror
            End Get
            Set(Value As Double)
                Me.csobj.rmserror = Value
            End Set
        End Property
        Public Property avgerror() As Double
            Get
                Return Me.csobj.avgerror
            End Get
            Set(Value As Double)
                Me.csobj.avgerror = Value
            End Set
        End Property
        Public Property avgrelerror() As Double
            Get
                Return Me.csobj.avgrelerror
            End Get
            Set(Value As Double)
                Me.csobj.avgrelerror = Value
            End Set
        End Property
        Public Property cvrmserror() As Double
            Get
                Return Me.csobj.cvrmserror
            End Get
            Set(Value As Double)
                Me.csobj.cvrmserror = Value
            End Set
        End Property
        Public Property cvavgerror() As Double
            Get
                Return Me.csobj.cvavgerror
            End Get
            Set(Value As Double)
                Me.csobj.cvavgerror = Value
            End Set
        End Property
        Public Property cvavgrelerror() As Double
            Get
                Return Me.csobj.cvavgrelerror
            End Get
            Set(Value As Double)
                Me.csobj.cvavgrelerror = Value
            End Set
        End Property
        Public Property ncvdefects() As Integer
            Get
                Return Me.csobj.ncvdefects
            End Get
            Set(Value As Integer)
                Me.csobj.ncvdefects = Value
            End Set
        End Property
        Public Property cvdefects() As Integer()
            Get
                Return Me.csobj.cvdefects
            End Get
            Set(Value As Integer())
                Me.csobj.cvdefects = Value
            End Set
        End Property
        Public csobj As alglib.lrreport
    End Class


    Public Sub lrbuild(xy(,) As Double, npoints As Integer, nvars As Integer, ByRef info As Integer, ByRef lm As linearmodel, ByRef ar As lrreport)
        Try
            lm = New linearmodel()
            ar = New lrreport()
            alglib.lrbuild(xy, npoints, nvars, info, lm.csobj, ar.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub lrbuilds(xy(,) As Double, s() As Double, npoints As Integer, nvars As Integer, ByRef info As Integer, ByRef lm As linearmodel, ByRef ar As lrreport)
        Try
            lm = New linearmodel()
            ar = New lrreport()
            alglib.lrbuilds(xy, s, npoints, nvars, info, lm.csobj, ar.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub lrbuildzs(xy(,) As Double, s() As Double, npoints As Integer, nvars As Integer, ByRef info As Integer, ByRef lm As linearmodel, ByRef ar As lrreport)
        Try
            lm = New linearmodel()
            ar = New lrreport()
            alglib.lrbuildzs(xy, s, npoints, nvars, info, lm.csobj, ar.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub lrbuildz(xy(,) As Double, npoints As Integer, nvars As Integer, ByRef info As Integer, ByRef lm As linearmodel, ByRef ar As lrreport)
        Try
            lm = New linearmodel()
            ar = New lrreport()
            alglib.lrbuildz(xy, npoints, nvars, info, lm.csobj, ar.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub lrunpack(lm As linearmodel, ByRef v() As Double, ByRef nvars As Integer)
        Try
            alglib.lrunpack(lm.csobj, v, nvars)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub lrpack(v() As Double, nvars As Integer, ByRef lm As linearmodel)
        Try
            lm = New linearmodel()
            alglib.lrpack(v, nvars, lm.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Function lrprocess(lm As linearmodel, x() As Double) As Double
        Try
            lrprocess = alglib.lrprocess(lm.csobj, x)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Function


    Public Function lrrmserror(lm As linearmodel, xy(,) As Double, npoints As Integer) As Double
        Try
            lrrmserror = alglib.lrrmserror(lm.csobj, xy, npoints)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Function


    Public Function lravgerror(lm As linearmodel, xy(,) As Double, npoints As Integer) As Double
        Try
            lravgerror = alglib.lravgerror(lm.csobj, xy, npoints)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Function


    Public Function lravgrelerror(lm As linearmodel, xy(,) As Double, npoints As Integer) As Double
        Try
            lravgrelerror = alglib.lravgrelerror(lm.csobj, xy, npoints)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Function




    Public Sub filtersma(ByRef x() As Double, n As Integer, k As Integer)
        Try
            alglib.filtersma(x, n, k)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub filtersma(ByRef x() As Double, k As Integer)
        Try
            alglib.filtersma(x, k)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub filterema(ByRef x() As Double, n As Integer, alpha As Double)
        Try
            alglib.filterema(x, n, alpha)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub filterema(ByRef x() As Double, alpha As Double)
        Try
            alglib.filterema(x, alpha)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub filterlrma(ByRef x() As Double, n As Integer, k As Integer)
        Try
            alglib.filterlrma(x, n, k)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub filterlrma(ByRef x() As Double, k As Integer)
        Try
            alglib.filterlrma(x, k)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub




    Public Sub kmeansgenerate(xy(,) As Double, npoints As Integer, nvars As Integer, k As Integer, restarts As Integer, ByRef info As Integer, ByRef c(,) As Double, ByRef xyc() As Integer)
        Try
            alglib.kmeansgenerate(xy, npoints, nvars, k, restarts, info, c, xyc)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub




    Public Function smatrixevd(a(,) As Double, n As Integer, zneeded As Integer, isupper As Boolean, ByRef d() As Double, ByRef z(,) As Double) As Boolean
        Try
            smatrixevd = alglib.smatrixevd(a, n, zneeded, isupper, d, z)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Function


    Public Function smatrixevdr(a(,) As Double, n As Integer, zneeded As Integer, isupper As Boolean, b1 As Double, b2 As Double, ByRef m As Integer, ByRef w() As Double, ByRef z(,) As Double) As Boolean
        Try
            smatrixevdr = alglib.smatrixevdr(a, n, zneeded, isupper, b1, b2, m, w, z)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Function


    Public Function smatrixevdi(a(,) As Double, n As Integer, zneeded As Integer, isupper As Boolean, i1 As Integer, i2 As Integer, ByRef w() As Double, ByRef z(,) As Double) As Boolean
        Try
            smatrixevdi = alglib.smatrixevdi(a, n, zneeded, isupper, i1, i2, w, z)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Function


    Public Function hmatrixevd(a(,) As alglib.complex, n As Integer, zneeded As Integer, isupper As Boolean, ByRef d() As Double, ByRef z(,) As alglib.complex) As Boolean
        Try
            hmatrixevd = alglib.hmatrixevd(a, n, zneeded, isupper, d, z)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Function


    Public Function hmatrixevdr(a(,) As alglib.complex, n As Integer, zneeded As Integer, isupper As Boolean, b1 As Double, b2 As Double, ByRef m As Integer, ByRef w() As Double, ByRef z(,) As alglib.complex) As Boolean
        Try
            hmatrixevdr = alglib.hmatrixevdr(a, n, zneeded, isupper, b1, b2, m, w, z)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Function


    Public Function hmatrixevdi(a(,) As alglib.complex, n As Integer, zneeded As Integer, isupper As Boolean, i1 As Integer, i2 As Integer, ByRef w() As Double, ByRef z(,) As alglib.complex) As Boolean
        Try
            hmatrixevdi = alglib.hmatrixevdi(a, n, zneeded, isupper, i1, i2, w, z)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Function


    Public Function smatrixtdevd(ByRef d() As Double, e() As Double, n As Integer, zneeded As Integer, ByRef z(,) As Double) As Boolean
        Try
            smatrixtdevd = alglib.smatrixtdevd(d, e, n, zneeded, z)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Function


    Public Function smatrixtdevdr(ByRef d() As Double, e() As Double, n As Integer, zneeded As Integer, a As Double, b As Double, ByRef m As Integer, ByRef z(,) As Double) As Boolean
        Try
            smatrixtdevdr = alglib.smatrixtdevdr(d, e, n, zneeded, a, b, m, z)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Function


    Public Function smatrixtdevdi(ByRef d() As Double, e() As Double, n As Integer, zneeded As Integer, i1 As Integer, i2 As Integer, ByRef z(,) As Double) As Boolean
        Try
            smatrixtdevdi = alglib.smatrixtdevdi(d, e, n, zneeded, i1, i2, z)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Function


    Public Function rmatrixevd(a(,) As Double, n As Integer, vneeded As Integer, ByRef wr() As Double, ByRef wi() As Double, ByRef vl(,) As Double, ByRef vr(,) As Double) As Boolean
        Try
            rmatrixevd = alglib.rmatrixevd(a, n, vneeded, wr, wi, vl, vr)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Function




    Public Sub rmatrixrndorthogonal(n As Integer, ByRef a(,) As Double)
        Try
            alglib.rmatrixrndorthogonal(n, a)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub rmatrixrndcond(n As Integer, c As Double, ByRef a(,) As Double)
        Try
            alglib.rmatrixrndcond(n, c, a)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub cmatrixrndorthogonal(n As Integer, ByRef a(,) As alglib.complex)
        Try
            alglib.cmatrixrndorthogonal(n, a)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub cmatrixrndcond(n As Integer, c As Double, ByRef a(,) As alglib.complex)
        Try
            alglib.cmatrixrndcond(n, c, a)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub smatrixrndcond(n As Integer, c As Double, ByRef a(,) As Double)
        Try
            alglib.smatrixrndcond(n, c, a)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub spdmatrixrndcond(n As Integer, c As Double, ByRef a(,) As Double)
        Try
            alglib.spdmatrixrndcond(n, c, a)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub hmatrixrndcond(n As Integer, c As Double, ByRef a(,) As alglib.complex)
        Try
            alglib.hmatrixrndcond(n, c, a)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub hpdmatrixrndcond(n As Integer, c As Double, ByRef a(,) As alglib.complex)
        Try
            alglib.hpdmatrixrndcond(n, c, a)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub rmatrixrndorthogonalfromtheright(ByRef a(,) As Double, m As Integer, n As Integer)
        Try
            alglib.rmatrixrndorthogonalfromtheright(a, m, n)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub rmatrixrndorthogonalfromtheleft(ByRef a(,) As Double, m As Integer, n As Integer)
        Try
            alglib.rmatrixrndorthogonalfromtheleft(a, m, n)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub cmatrixrndorthogonalfromtheright(ByRef a(,) As alglib.complex, m As Integer, n As Integer)
        Try
            alglib.cmatrixrndorthogonalfromtheright(a, m, n)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub cmatrixrndorthogonalfromtheleft(ByRef a(,) As alglib.complex, m As Integer, n As Integer)
        Try
            alglib.cmatrixrndorthogonalfromtheleft(a, m, n)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub smatrixrndmultiply(ByRef a(,) As Double, n As Integer)
        Try
            alglib.smatrixrndmultiply(a, n)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub hmatrixrndmultiply(ByRef a(,) As alglib.complex, n As Integer)
        Try
            alglib.hmatrixrndmultiply(a, n)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub




    Public Sub rmatrixlu(ByRef a(,) As Double, m As Integer, n As Integer, ByRef pivots() As Integer)
        Try
            alglib.rmatrixlu(a, m, n, pivots)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub cmatrixlu(ByRef a(,) As alglib.complex, m As Integer, n As Integer, ByRef pivots() As Integer)
        Try
            alglib.cmatrixlu(a, m, n, pivots)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Function hpdmatrixcholesky(ByRef a(,) As alglib.complex, n As Integer, isupper As Boolean) As Boolean
        Try
            hpdmatrixcholesky = alglib.hpdmatrixcholesky(a, n, isupper)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Function


    Public Function spdmatrixcholesky(ByRef a(,) As Double, n As Integer, isupper As Boolean) As Boolean
        Try
            spdmatrixcholesky = alglib.spdmatrixcholesky(a, n, isupper)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Function




    Public Function rmatrixrcond1(a(,) As Double, n As Integer) As Double
        Try
            rmatrixrcond1 = alglib.rmatrixrcond1(a, n)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Function


    Public Function rmatrixrcondinf(a(,) As Double, n As Integer) As Double
        Try
            rmatrixrcondinf = alglib.rmatrixrcondinf(a, n)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Function


    Public Function spdmatrixrcond(a(,) As Double, n As Integer, isupper As Boolean) As Double
        Try
            spdmatrixrcond = alglib.spdmatrixrcond(a, n, isupper)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Function


    Public Function rmatrixtrrcond1(a(,) As Double, n As Integer, isupper As Boolean, isunit As Boolean) As Double
        Try
            rmatrixtrrcond1 = alglib.rmatrixtrrcond1(a, n, isupper, isunit)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Function


    Public Function rmatrixtrrcondinf(a(,) As Double, n As Integer, isupper As Boolean, isunit As Boolean) As Double
        Try
            rmatrixtrrcondinf = alglib.rmatrixtrrcondinf(a, n, isupper, isunit)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Function


    Public Function hpdmatrixrcond(a(,) As alglib.complex, n As Integer, isupper As Boolean) As Double
        Try
            hpdmatrixrcond = alglib.hpdmatrixrcond(a, n, isupper)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Function


    Public Function cmatrixrcond1(a(,) As alglib.complex, n As Integer) As Double
        Try
            cmatrixrcond1 = alglib.cmatrixrcond1(a, n)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Function


    Public Function cmatrixrcondinf(a(,) As alglib.complex, n As Integer) As Double
        Try
            cmatrixrcondinf = alglib.cmatrixrcondinf(a, n)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Function


    Public Function rmatrixlurcond1(lua(,) As Double, n As Integer) As Double
        Try
            rmatrixlurcond1 = alglib.rmatrixlurcond1(lua, n)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Function


    Public Function rmatrixlurcondinf(lua(,) As Double, n As Integer) As Double
        Try
            rmatrixlurcondinf = alglib.rmatrixlurcondinf(lua, n)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Function


    Public Function spdmatrixcholeskyrcond(a(,) As Double, n As Integer, isupper As Boolean) As Double
        Try
            spdmatrixcholeskyrcond = alglib.spdmatrixcholeskyrcond(a, n, isupper)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Function


    Public Function hpdmatrixcholeskyrcond(a(,) As alglib.complex, n As Integer, isupper As Boolean) As Double
        Try
            hpdmatrixcholeskyrcond = alglib.hpdmatrixcholeskyrcond(a, n, isupper)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Function


    Public Function cmatrixlurcond1(lua(,) As alglib.complex, n As Integer) As Double
        Try
            cmatrixlurcond1 = alglib.cmatrixlurcond1(lua, n)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Function


    Public Function cmatrixlurcondinf(lua(,) As alglib.complex, n As Integer) As Double
        Try
            cmatrixlurcondinf = alglib.cmatrixlurcondinf(lua, n)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Function


    Public Function cmatrixtrrcond1(a(,) As alglib.complex, n As Integer, isupper As Boolean, isunit As Boolean) As Double
        Try
            cmatrixtrrcond1 = alglib.cmatrixtrrcond1(a, n, isupper, isunit)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Function


    Public Function cmatrixtrrcondinf(a(,) As alglib.complex, n As Integer, isupper As Boolean, isunit As Boolean) As Double
        Try
            cmatrixtrrcondinf = alglib.cmatrixtrrcondinf(a, n, isupper, isunit)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Function

    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    'Matrix inverse report:
    '* R1    reciprocal of condition number in 1-norm
    '* RInf  reciprocal of condition number in inf-norm
    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    Public Class matinvreport
        Public Property r1() As Double
            Get
                Return Me.csobj.r1
            End Get
            Set(Value As Double)
                Me.csobj.r1 = Value
            End Set
        End Property
        Public Property rinf() As Double
            Get
                Return Me.csobj.rinf
            End Get
            Set(Value As Double)
                Me.csobj.rinf = Value
            End Set
        End Property
        Public csobj As alglib.matinvreport
    End Class


    Public Sub rmatrixluinverse(ByRef a(,) As Double, pivots() As Integer, n As Integer, ByRef info As Integer, ByRef rep As matinvreport)
        Try
            rep = New matinvreport()
            alglib.rmatrixluinverse(a, pivots, n, info, rep.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub rmatrixluinverse(ByRef a(,) As Double, pivots() As Integer, ByRef info As Integer, ByRef rep As matinvreport)
        Try
            rep = New matinvreport()
            alglib.rmatrixluinverse(a, pivots, info, rep.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub rmatrixinverse(ByRef a(,) As Double, n As Integer, ByRef info As Integer, ByRef rep As matinvreport)
        Try
            rep = New matinvreport()
            alglib.rmatrixinverse(a, n, info, rep.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub rmatrixinverse(ByRef a(,) As Double, ByRef info As Integer, ByRef rep As matinvreport)
        Try
            rep = New matinvreport()
            alglib.rmatrixinverse(a, info, rep.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub cmatrixluinverse(ByRef a(,) As alglib.complex, pivots() As Integer, n As Integer, ByRef info As Integer, ByRef rep As matinvreport)
        Try
            rep = New matinvreport()
            alglib.cmatrixluinverse(a, pivots, n, info, rep.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub cmatrixluinverse(ByRef a(,) As alglib.complex, pivots() As Integer, ByRef info As Integer, ByRef rep As matinvreport)
        Try
            rep = New matinvreport()
            alglib.cmatrixluinverse(a, pivots, info, rep.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub cmatrixinverse(ByRef a(,) As alglib.complex, n As Integer, ByRef info As Integer, ByRef rep As matinvreport)
        Try
            rep = New matinvreport()
            alglib.cmatrixinverse(a, n, info, rep.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub cmatrixinverse(ByRef a(,) As alglib.complex, ByRef info As Integer, ByRef rep As matinvreport)
        Try
            rep = New matinvreport()
            alglib.cmatrixinverse(a, info, rep.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub spdmatrixcholeskyinverse(ByRef a(,) As Double, n As Integer, isupper As Boolean, ByRef info As Integer, ByRef rep As matinvreport)
        Try
            rep = New matinvreport()
            alglib.spdmatrixcholeskyinverse(a, n, isupper, info, rep.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub spdmatrixcholeskyinverse(ByRef a(,) As Double, ByRef info As Integer, ByRef rep As matinvreport)
        Try
            rep = New matinvreport()
            alglib.spdmatrixcholeskyinverse(a, info, rep.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub spdmatrixinverse(ByRef a(,) As Double, n As Integer, isupper As Boolean, ByRef info As Integer, ByRef rep As matinvreport)
        Try
            rep = New matinvreport()
            alglib.spdmatrixinverse(a, n, isupper, info, rep.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub spdmatrixinverse(ByRef a(,) As Double, ByRef info As Integer, ByRef rep As matinvreport)
        Try
            rep = New matinvreport()
            alglib.spdmatrixinverse(a, info, rep.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub hpdmatrixcholeskyinverse(ByRef a(,) As alglib.complex, n As Integer, isupper As Boolean, ByRef info As Integer, ByRef rep As matinvreport)
        Try
            rep = New matinvreport()
            alglib.hpdmatrixcholeskyinverse(a, n, isupper, info, rep.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub hpdmatrixcholeskyinverse(ByRef a(,) As alglib.complex, ByRef info As Integer, ByRef rep As matinvreport)
        Try
            rep = New matinvreport()
            alglib.hpdmatrixcholeskyinverse(a, info, rep.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub hpdmatrixinverse(ByRef a(,) As alglib.complex, n As Integer, isupper As Boolean, ByRef info As Integer, ByRef rep As matinvreport)
        Try
            rep = New matinvreport()
            alglib.hpdmatrixinverse(a, n, isupper, info, rep.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub hpdmatrixinverse(ByRef a(,) As alglib.complex, ByRef info As Integer, ByRef rep As matinvreport)
        Try
            rep = New matinvreport()
            alglib.hpdmatrixinverse(a, info, rep.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub rmatrixtrinverse(ByRef a(,) As Double, n As Integer, isupper As Boolean, isunit As Boolean, ByRef info As Integer, ByRef rep As matinvreport)
        Try
            rep = New matinvreport()
            alglib.rmatrixtrinverse(a, n, isupper, isunit, info, rep.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub rmatrixtrinverse(ByRef a(,) As Double, isupper As Boolean, ByRef info As Integer, ByRef rep As matinvreport)
        Try
            rep = New matinvreport()
            alglib.rmatrixtrinverse(a, isupper, info, rep.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub cmatrixtrinverse(ByRef a(,) As alglib.complex, n As Integer, isupper As Boolean, isunit As Boolean, ByRef info As Integer, ByRef rep As matinvreport)
        Try
            rep = New matinvreport()
            alglib.cmatrixtrinverse(a, n, isupper, isunit, info, rep.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub cmatrixtrinverse(ByRef a(,) As alglib.complex, isupper As Boolean, ByRef info As Integer, ByRef rep As matinvreport)
        Try
            rep = New matinvreport()
            alglib.cmatrixtrinverse(a, isupper, info, rep.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub




    Public Sub fisherlda(xy(,) As Double, npoints As Integer, nvars As Integer, nclasses As Integer, ByRef info As Integer, ByRef w() As Double)
        Try
            alglib.fisherlda(xy, npoints, nvars, nclasses, info, w)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub fisherldan(xy(,) As Double, npoints As Integer, nvars As Integer, nclasses As Integer, ByRef info As Integer, ByRef w(,) As Double)
        Try
            alglib.fisherldan(xy, npoints, nvars, nclasses, info, w)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub

    Public Class multilayerperceptron
        Public csobj As alglib.multilayerperceptron
    End Class
    Public Sub mlpserialize(obj As multilayerperceptron, ByRef s_out As String)
        Try
            alglib.mlpserialize(obj.csobj, s_out)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub

    Public Sub mlpunserialize(s_in As String, ByRef obj As multilayerperceptron)
        Try
            alglib.mlpunserialize(s_in, obj.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub mlpcreate0(nin As Integer, nout As Integer, ByRef network As multilayerperceptron)
        Try
            network = New multilayerperceptron()
            alglib.mlpcreate0(nin, nout, network.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub mlpcreate1(nin As Integer, nhid As Integer, nout As Integer, ByRef network As multilayerperceptron)
        Try
            network = New multilayerperceptron()
            alglib.mlpcreate1(nin, nhid, nout, network.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub mlpcreate2(nin As Integer, nhid1 As Integer, nhid2 As Integer, nout As Integer, ByRef network As multilayerperceptron)
        Try
            network = New multilayerperceptron()
            alglib.mlpcreate2(nin, nhid1, nhid2, nout, network.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub mlpcreateb0(nin As Integer, nout As Integer, b As Double, d As Double, ByRef network As multilayerperceptron)
        Try
            network = New multilayerperceptron()
            alglib.mlpcreateb0(nin, nout, b, d, network.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub mlpcreateb1(nin As Integer, nhid As Integer, nout As Integer, b As Double, d As Double, ByRef network As multilayerperceptron)
        Try
            network = New multilayerperceptron()
            alglib.mlpcreateb1(nin, nhid, nout, b, d, network.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub mlpcreateb2(nin As Integer, nhid1 As Integer, nhid2 As Integer, nout As Integer, b As Double, d As Double, ByRef network As multilayerperceptron)
        Try
            network = New multilayerperceptron()
            alglib.mlpcreateb2(nin, nhid1, nhid2, nout, b, d, network.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub mlpcreater0(nin As Integer, nout As Integer, a As Double, b As Double, ByRef network As multilayerperceptron)
        Try
            network = New multilayerperceptron()
            alglib.mlpcreater0(nin, nout, a, b, network.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub mlpcreater1(nin As Integer, nhid As Integer, nout As Integer, a As Double, b As Double, ByRef network As multilayerperceptron)
        Try
            network = New multilayerperceptron()
            alglib.mlpcreater1(nin, nhid, nout, a, b, network.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub mlpcreater2(nin As Integer, nhid1 As Integer, nhid2 As Integer, nout As Integer, a As Double, b As Double, ByRef network As multilayerperceptron)
        Try
            network = New multilayerperceptron()
            alglib.mlpcreater2(nin, nhid1, nhid2, nout, a, b, network.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub mlpcreatec0(nin As Integer, nout As Integer, ByRef network As multilayerperceptron)
        Try
            network = New multilayerperceptron()
            alglib.mlpcreatec0(nin, nout, network.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub mlpcreatec1(nin As Integer, nhid As Integer, nout As Integer, ByRef network As multilayerperceptron)
        Try
            network = New multilayerperceptron()
            alglib.mlpcreatec1(nin, nhid, nout, network.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub mlpcreatec2(nin As Integer, nhid1 As Integer, nhid2 As Integer, nout As Integer, ByRef network As multilayerperceptron)
        Try
            network = New multilayerperceptron()
            alglib.mlpcreatec2(nin, nhid1, nhid2, nout, network.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub mlprandomize(network As multilayerperceptron)
        Try
            alglib.mlprandomize(network.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub mlprandomizefull(network As multilayerperceptron)
        Try
            alglib.mlprandomizefull(network.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub mlpproperties(network As multilayerperceptron, ByRef nin As Integer, ByRef nout As Integer, ByRef wcount As Integer)
        Try
            alglib.mlpproperties(network.csobj, nin, nout, wcount)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Function mlpgetinputscount(network As multilayerperceptron) As Integer
        Try
            mlpgetinputscount = alglib.mlpgetinputscount(network.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Function


    Public Function mlpgetoutputscount(network As multilayerperceptron) As Integer
        Try
            mlpgetoutputscount = alglib.mlpgetoutputscount(network.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Function


    Public Function mlpgetweightscount(network As multilayerperceptron) As Integer
        Try
            mlpgetweightscount = alglib.mlpgetweightscount(network.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Function


    Public Function mlpissoftmax(network As multilayerperceptron) As Boolean
        Try
            mlpissoftmax = alglib.mlpissoftmax(network.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Function


    Public Function mlpgetlayerscount(network As multilayerperceptron) As Integer
        Try
            mlpgetlayerscount = alglib.mlpgetlayerscount(network.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Function


    Public Function mlpgetlayersize(network As multilayerperceptron, k As Integer) As Integer
        Try
            mlpgetlayersize = alglib.mlpgetlayersize(network.csobj, k)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Function


    Public Sub mlpgetinputscaling(network As multilayerperceptron, i As Integer, ByRef mean As Double, ByRef sigma As Double)
        Try
            alglib.mlpgetinputscaling(network.csobj, i, mean, sigma)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub mlpgetoutputscaling(network As multilayerperceptron, i As Integer, ByRef mean As Double, ByRef sigma As Double)
        Try
            alglib.mlpgetoutputscaling(network.csobj, i, mean, sigma)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub mlpgetneuroninfo(network As multilayerperceptron, k As Integer, i As Integer, ByRef fkind As Integer, ByRef threshold As Double)
        Try
            alglib.mlpgetneuroninfo(network.csobj, k, i, fkind, threshold)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Function mlpgetweight(network As multilayerperceptron, k0 As Integer, i0 As Integer, k1 As Integer, i1 As Integer) As Double
        Try
            mlpgetweight = alglib.mlpgetweight(network.csobj, k0, i0, k1, i1)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Function


    Public Sub mlpsetinputscaling(network As multilayerperceptron, i As Integer, mean As Double, sigma As Double)
        Try
            alglib.mlpsetinputscaling(network.csobj, i, mean, sigma)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub mlpsetoutputscaling(network As multilayerperceptron, i As Integer, mean As Double, sigma As Double)
        Try
            alglib.mlpsetoutputscaling(network.csobj, i, mean, sigma)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub mlpsetneuroninfo(network As multilayerperceptron, k As Integer, i As Integer, fkind As Integer, threshold As Double)
        Try
            alglib.mlpsetneuroninfo(network.csobj, k, i, fkind, threshold)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub mlpsetweight(network As multilayerperceptron, k0 As Integer, i0 As Integer, k1 As Integer, i1 As Integer, w As Double)
        Try
            alglib.mlpsetweight(network.csobj, k0, i0, k1, i1, w)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub mlpactivationfunction(net As Double, k As Integer, ByRef f As Double, ByRef df As Double, ByRef d2f As Double)
        Try
            alglib.mlpactivationfunction(net, k, f, df, d2f)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub mlpprocess(network As multilayerperceptron, x() As Double, ByRef y() As Double)
        Try
            alglib.mlpprocess(network.csobj, x, y)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub mlpprocessi(network As multilayerperceptron, x() As Double, ByRef y() As Double)
        Try
            alglib.mlpprocessi(network.csobj, x, y)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Function mlperror(network As multilayerperceptron, xy(,) As Double, ssize As Integer) As Double
        Try
            mlperror = alglib.mlperror(network.csobj, xy, ssize)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Function


    Public Function mlperrorn(network As multilayerperceptron, xy(,) As Double, ssize As Integer) As Double
        Try
            mlperrorn = alglib.mlperrorn(network.csobj, xy, ssize)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Function


    Public Function mlpclserror(network As multilayerperceptron, xy(,) As Double, ssize As Integer) As Integer
        Try
            mlpclserror = alglib.mlpclserror(network.csobj, xy, ssize)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Function


    Public Function mlprelclserror(network As multilayerperceptron, xy(,) As Double, npoints As Integer) As Double
        Try
            mlprelclserror = alglib.mlprelclserror(network.csobj, xy, npoints)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Function


    Public Function mlpavgce(network As multilayerperceptron, xy(,) As Double, npoints As Integer) As Double
        Try
            mlpavgce = alglib.mlpavgce(network.csobj, xy, npoints)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Function


    Public Function mlprmserror(network As multilayerperceptron, xy(,) As Double, npoints As Integer) As Double
        Try
            mlprmserror = alglib.mlprmserror(network.csobj, xy, npoints)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Function


    Public Function mlpavgerror(network As multilayerperceptron, xy(,) As Double, npoints As Integer) As Double
        Try
            mlpavgerror = alglib.mlpavgerror(network.csobj, xy, npoints)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Function


    Public Function mlpavgrelerror(network As multilayerperceptron, xy(,) As Double, npoints As Integer) As Double
        Try
            mlpavgrelerror = alglib.mlpavgrelerror(network.csobj, xy, npoints)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Function


    Public Sub mlpgrad(network As multilayerperceptron, x() As Double, desiredy() As Double, ByRef e As Double, ByRef grad() As Double)
        Try
            alglib.mlpgrad(network.csobj, x, desiredy, e, grad)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub mlpgradn(network As multilayerperceptron, x() As Double, desiredy() As Double, ByRef e As Double, ByRef grad() As Double)
        Try
            alglib.mlpgradn(network.csobj, x, desiredy, e, grad)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub mlpgradbatch(network As multilayerperceptron, xy(,) As Double, ssize As Integer, ByRef e As Double, ByRef grad() As Double)
        Try
            alglib.mlpgradbatch(network.csobj, xy, ssize, e, grad)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub mlpgradnbatch(network As multilayerperceptron, xy(,) As Double, ssize As Integer, ByRef e As Double, ByRef grad() As Double)
        Try
            alglib.mlpgradnbatch(network.csobj, xy, ssize, e, grad)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub mlphessiannbatch(network As multilayerperceptron, xy(,) As Double, ssize As Integer, ByRef e As Double, ByRef grad() As Double, ByRef h(,) As Double)
        Try
            alglib.mlphessiannbatch(network.csobj, xy, ssize, e, grad, h)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub mlphessianbatch(network As multilayerperceptron, xy(,) As Double, ssize As Integer, ByRef e As Double, ByRef grad() As Double, ByRef h(,) As Double)
        Try
            alglib.mlphessianbatch(network.csobj, xy, ssize, e, grad, h)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub

    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    '
    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    Public Class densesolverreport
        Public Property r1() As Double
            Get
                Return Me.csobj.r1
            End Get
            Set(Value As Double)
                Me.csobj.r1 = Value
            End Set
        End Property
        Public Property rinf() As Double
            Get
                Return Me.csobj.rinf
            End Get
            Set(Value As Double)
                Me.csobj.rinf = Value
            End Set
        End Property
        Public csobj As alglib.densesolverreport
    End Class
    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    '
    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    Public Class densesolverlsreport
        Public Property r2() As Double
            Get
                Return Me.csobj.r2
            End Get
            Set(Value As Double)
                Me.csobj.r2 = Value
            End Set
        End Property
        Public Property cx() As Double(,)
            Get
                Return Me.csobj.cx
            End Get
            Set(Value As Double(,))
                Me.csobj.cx = Value
            End Set
        End Property
        Public Property n() As Integer
            Get
                Return Me.csobj.n
            End Get
            Set(Value As Integer)
                Me.csobj.n = Value
            End Set
        End Property
        Public Property k() As Integer
            Get
                Return Me.csobj.k
            End Get
            Set(Value As Integer)
                Me.csobj.k = Value
            End Set
        End Property
        Public csobj As alglib.densesolverlsreport
    End Class


    Public Sub rmatrixsolve(a(,) As Double, n As Integer, b() As Double, ByRef info As Integer, ByRef rep As densesolverreport, ByRef x() As Double)
        Try
            rep = New densesolverreport()
            alglib.rmatrixsolve(a, n, b, info, rep.csobj, x)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub rmatrixsolvem(a(,) As Double, n As Integer, b(,) As Double, m As Integer, rfs As Boolean, ByRef info As Integer, ByRef rep As densesolverreport, ByRef x(,) As Double)
        Try
            rep = New densesolverreport()
            alglib.rmatrixsolvem(a, n, b, m, rfs, info, rep.csobj, x)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub rmatrixlusolve(lua(,) As Double, p() As Integer, n As Integer, b() As Double, ByRef info As Integer, ByRef rep As densesolverreport, ByRef x() As Double)
        Try
            rep = New densesolverreport()
            alglib.rmatrixlusolve(lua, p, n, b, info, rep.csobj, x)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub rmatrixlusolvem(lua(,) As Double, p() As Integer, n As Integer, b(,) As Double, m As Integer, ByRef info As Integer, ByRef rep As densesolverreport, ByRef x(,) As Double)
        Try
            rep = New densesolverreport()
            alglib.rmatrixlusolvem(lua, p, n, b, m, info, rep.csobj, x)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub rmatrixmixedsolve(a(,) As Double, lua(,) As Double, p() As Integer, n As Integer, b() As Double, ByRef info As Integer, ByRef rep As densesolverreport, ByRef x() As Double)
        Try
            rep = New densesolverreport()
            alglib.rmatrixmixedsolve(a, lua, p, n, b, info, rep.csobj, x)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub rmatrixmixedsolvem(a(,) As Double, lua(,) As Double, p() As Integer, n As Integer, b(,) As Double, m As Integer, ByRef info As Integer, ByRef rep As densesolverreport, ByRef x(,) As Double)
        Try
            rep = New densesolverreport()
            alglib.rmatrixmixedsolvem(a, lua, p, n, b, m, info, rep.csobj, x)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub cmatrixsolvem(a(,) As alglib.complex, n As Integer, b(,) As alglib.complex, m As Integer, rfs As Boolean, ByRef info As Integer, ByRef rep As densesolverreport, ByRef x(,) As alglib.complex)
        Try
            rep = New densesolverreport()
            alglib.cmatrixsolvem(a, n, b, m, rfs, info, rep.csobj, x)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub cmatrixsolve(a(,) As alglib.complex, n As Integer, b() As alglib.complex, ByRef info As Integer, ByRef rep As densesolverreport, ByRef x() As alglib.complex)
        Try
            rep = New densesolverreport()
            alglib.cmatrixsolve(a, n, b, info, rep.csobj, x)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub cmatrixlusolvem(lua(,) As alglib.complex, p() As Integer, n As Integer, b(,) As alglib.complex, m As Integer, ByRef info As Integer, ByRef rep As densesolverreport, ByRef x(,) As alglib.complex)
        Try
            rep = New densesolverreport()
            alglib.cmatrixlusolvem(lua, p, n, b, m, info, rep.csobj, x)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub cmatrixlusolve(lua(,) As alglib.complex, p() As Integer, n As Integer, b() As alglib.complex, ByRef info As Integer, ByRef rep As densesolverreport, ByRef x() As alglib.complex)
        Try
            rep = New densesolverreport()
            alglib.cmatrixlusolve(lua, p, n, b, info, rep.csobj, x)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub cmatrixmixedsolvem(a(,) As alglib.complex, lua(,) As alglib.complex, p() As Integer, n As Integer, b(,) As alglib.complex, m As Integer, ByRef info As Integer, ByRef rep As densesolverreport, ByRef x(,) As alglib.complex)
        Try
            rep = New densesolverreport()
            alglib.cmatrixmixedsolvem(a, lua, p, n, b, m, info, rep.csobj, x)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub cmatrixmixedsolve(a(,) As alglib.complex, lua(,) As alglib.complex, p() As Integer, n As Integer, b() As alglib.complex, ByRef info As Integer, ByRef rep As densesolverreport, ByRef x() As alglib.complex)
        Try
            rep = New densesolverreport()
            alglib.cmatrixmixedsolve(a, lua, p, n, b, info, rep.csobj, x)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub spdmatrixsolvem(a(,) As Double, n As Integer, isupper As Boolean, b(,) As Double, m As Integer, ByRef info As Integer, ByRef rep As densesolverreport, ByRef x(,) As Double)
        Try
            rep = New densesolverreport()
            alglib.spdmatrixsolvem(a, n, isupper, b, m, info, rep.csobj, x)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub spdmatrixsolve(a(,) As Double, n As Integer, isupper As Boolean, b() As Double, ByRef info As Integer, ByRef rep As densesolverreport, ByRef x() As Double)
        Try
            rep = New densesolverreport()
            alglib.spdmatrixsolve(a, n, isupper, b, info, rep.csobj, x)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub spdmatrixcholeskysolvem(cha(,) As Double, n As Integer, isupper As Boolean, b(,) As Double, m As Integer, ByRef info As Integer, ByRef rep As densesolverreport, ByRef x(,) As Double)
        Try
            rep = New densesolverreport()
            alglib.spdmatrixcholeskysolvem(cha, n, isupper, b, m, info, rep.csobj, x)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub spdmatrixcholeskysolve(cha(,) As Double, n As Integer, isupper As Boolean, b() As Double, ByRef info As Integer, ByRef rep As densesolverreport, ByRef x() As Double)
        Try
            rep = New densesolverreport()
            alglib.spdmatrixcholeskysolve(cha, n, isupper, b, info, rep.csobj, x)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub hpdmatrixsolvem(a(,) As alglib.complex, n As Integer, isupper As Boolean, b(,) As alglib.complex, m As Integer, ByRef info As Integer, ByRef rep As densesolverreport, ByRef x(,) As alglib.complex)
        Try
            rep = New densesolverreport()
            alglib.hpdmatrixsolvem(a, n, isupper, b, m, info, rep.csobj, x)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub hpdmatrixsolve(a(,) As alglib.complex, n As Integer, isupper As Boolean, b() As alglib.complex, ByRef info As Integer, ByRef rep As densesolverreport, ByRef x() As alglib.complex)
        Try
            rep = New densesolverreport()
            alglib.hpdmatrixsolve(a, n, isupper, b, info, rep.csobj, x)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub hpdmatrixcholeskysolvem(cha(,) As alglib.complex, n As Integer, isupper As Boolean, b(,) As alglib.complex, m As Integer, ByRef info As Integer, ByRef rep As densesolverreport, ByRef x(,) As alglib.complex)
        Try
            rep = New densesolverreport()
            alglib.hpdmatrixcholeskysolvem(cha, n, isupper, b, m, info, rep.csobj, x)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub hpdmatrixcholeskysolve(cha(,) As alglib.complex, n As Integer, isupper As Boolean, b() As alglib.complex, ByRef info As Integer, ByRef rep As densesolverreport, ByRef x() As alglib.complex)
        Try
            rep = New densesolverreport()
            alglib.hpdmatrixcholeskysolve(cha, n, isupper, b, info, rep.csobj, x)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub rmatrixsolvels(a(,) As Double, nrows As Integer, ncols As Integer, b() As Double, threshold As Double, ByRef info As Integer, ByRef rep As densesolverlsreport, ByRef x() As Double)
        Try
            rep = New densesolverlsreport()
            alglib.rmatrixsolvels(a, nrows, ncols, b, threshold, info, rep.csobj, x)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub

    Public Class logitmodel
        Public csobj As alglib.logitmodel
    End Class
    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    'MNLReport structure contains information about training process:
    '* NGrad     -   number of gradient calculations
    '* NHess     -   number of Hessian calculations
    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    Public Class mnlreport
        Public Property ngrad() As Integer
            Get
                Return Me.csobj.ngrad
            End Get
            Set(Value As Integer)
                Me.csobj.ngrad = Value
            End Set
        End Property
        Public Property nhess() As Integer
            Get
                Return Me.csobj.nhess
            End Get
            Set(Value As Integer)
                Me.csobj.nhess = Value
            End Set
        End Property
        Public csobj As alglib.mnlreport
    End Class


    Public Sub mnltrainh(xy(,) As Double, npoints As Integer, nvars As Integer, nclasses As Integer, ByRef info As Integer, ByRef lm As logitmodel, ByRef rep As mnlreport)
        Try
            lm = New logitmodel()
            rep = New mnlreport()
            alglib.mnltrainh(xy, npoints, nvars, nclasses, info, lm.csobj, rep.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub mnlprocess(lm As logitmodel, x() As Double, ByRef y() As Double)
        Try
            alglib.mnlprocess(lm.csobj, x, y)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub mnlprocessi(lm As logitmodel, x() As Double, ByRef y() As Double)
        Try
            alglib.mnlprocessi(lm.csobj, x, y)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub mnlunpack(lm As logitmodel, ByRef a(,) As Double, ByRef nvars As Integer, ByRef nclasses As Integer)
        Try
            alglib.mnlunpack(lm.csobj, a, nvars, nclasses)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub mnlpack(a(,) As Double, nvars As Integer, nclasses As Integer, ByRef lm As logitmodel)
        Try
            lm = New logitmodel()
            alglib.mnlpack(a, nvars, nclasses, lm.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Function mnlavgce(lm As logitmodel, xy(,) As Double, npoints As Integer) As Double
        Try
            mnlavgce = alglib.mnlavgce(lm.csobj, xy, npoints)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Function


    Public Function mnlrelclserror(lm As logitmodel, xy(,) As Double, npoints As Integer) As Double
        Try
            mnlrelclserror = alglib.mnlrelclserror(lm.csobj, xy, npoints)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Function


    Public Function mnlrmserror(lm As logitmodel, xy(,) As Double, npoints As Integer) As Double
        Try
            mnlrmserror = alglib.mnlrmserror(lm.csobj, xy, npoints)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Function


    Public Function mnlavgerror(lm As logitmodel, xy(,) As Double, npoints As Integer) As Double
        Try
            mnlavgerror = alglib.mnlavgerror(lm.csobj, xy, npoints)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Function


    Public Function mnlavgrelerror(lm As logitmodel, xy(,) As Double, ssize As Integer) As Double
        Try
            mnlavgrelerror = alglib.mnlavgrelerror(lm.csobj, xy, ssize)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Function


    Public Function mnlclserror(lm As logitmodel, xy(,) As Double, npoints As Integer) As Integer
        Try
            mnlclserror = alglib.mnlclserror(lm.csobj, xy, npoints)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Function




    Public Class mincgstate
        Public csobj As alglib.mincgstate
    End Class
    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    '
    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    Public Class mincgreport
        Public Property iterationscount() As Integer
            Get
                Return Me.csobj.iterationscount
            End Get
            Set(Value As Integer)
                Me.csobj.iterationscount = Value
            End Set
        End Property
        Public Property nfev() As Integer
            Get
                Return Me.csobj.nfev
            End Get
            Set(Value As Integer)
                Me.csobj.nfev = Value
            End Set
        End Property
        Public Property varidx() As Integer
            Get
                Return Me.csobj.varidx
            End Get
            Set(Value As Integer)
                Me.csobj.varidx = Value
            End Set
        End Property
        Public Property terminationtype() As Integer
            Get
                Return Me.csobj.terminationtype
            End Get
            Set(Value As Integer)
                Me.csobj.terminationtype = Value
            End Set
        End Property
        Public csobj As alglib.mincgreport
    End Class


    Public Sub mincgcreate(n As Integer, x() As Double, ByRef state As mincgstate)
        Try
            state = New mincgstate()
            alglib.mincgcreate(n, x, state.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub mincgcreate(x() As Double, ByRef state As mincgstate)
        Try
            state = New mincgstate()
            alglib.mincgcreate(x, state.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub mincgcreatef(n As Integer, x() As Double, diffstep As Double, ByRef state As mincgstate)
        Try
            state = New mincgstate()
            alglib.mincgcreatef(n, x, diffstep, state.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub mincgcreatef(x() As Double, diffstep As Double, ByRef state As mincgstate)
        Try
            state = New mincgstate()
            alglib.mincgcreatef(x, diffstep, state.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub mincgsetcond(state As mincgstate, epsg As Double, epsf As Double, epsx As Double, maxits As Integer)
        Try
            alglib.mincgsetcond(state.csobj, epsg, epsf, epsx, maxits)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub mincgsetscale(state As mincgstate, s() As Double)
        Try
            alglib.mincgsetscale(state.csobj, s)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub mincgsetxrep(state As mincgstate, needxrep As Boolean)
        Try
            alglib.mincgsetxrep(state.csobj, needxrep)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub mincgsetcgtype(state As mincgstate, cgtype As Integer)
        Try
            alglib.mincgsetcgtype(state.csobj, cgtype)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub mincgsetstpmax(state As mincgstate, stpmax As Double)
        Try
            alglib.mincgsetstpmax(state.csobj, stpmax)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub mincgsuggeststep(state As mincgstate, stp As Double)
        Try
            alglib.mincgsuggeststep(state.csobj, stp)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub mincgsetprecdefault(state As mincgstate)
        Try
            alglib.mincgsetprecdefault(state.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub mincgsetprecdiag(state As mincgstate, d() As Double)
        Try
            alglib.mincgsetprecdiag(state.csobj, d)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub mincgsetprecscale(state As mincgstate)
        Try
            alglib.mincgsetprecscale(state.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Function mincgiteration(state As mincgstate) As Boolean
        Try
            mincgiteration = alglib.mincgiteration(state.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Function
    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    ' This family of functions is used to launcn iterations of nonlinear optimizer
    ' 
    ' These functions accept following parameters:
    '     func    -   callback which calculates function (or merit function)
    '                 value func at given point x
    '     grad    -   callback which calculates function (or merit function)
    '                 value func and gradient grad at given point x
    '     rep     -   optional callback which is called after each iteration
    '                 can be null
    '     obj     -   optional object which is passed to func/grad/hess/jac/rep
    '                 can be null
    ' 
    ' 
    ' NOTES:
    ' 
    ' 1. This function has two different implementations: one which  uses  exact
    '    (analytical) user-supplied  gradient, and one which uses function value
    '    only  and  numerically  differentiates  function  in  order  to  obtain
    '    gradient.
    ' 
    '    Depending  on  the  specific  function  used to create optimizer object
    '    (either MinCGCreate()  for analytical gradient  or  MinCGCreateF()  for
    '    numerical differentiation) you should  choose  appropriate  variant  of
    '    MinCGOptimize() - one which accepts function AND gradient or one  which
    '    accepts function ONLY.
    ' 
    '    Be careful to choose variant of MinCGOptimize()  which  corresponds  to
    '    your optimization scheme! Table below lists different  combinations  of
    '    callback (function/gradient) passed  to  MinCGOptimize()  and  specific
    '    function used to create optimizer.
    ' 
    ' 
    '                   |         USER PASSED TO MinCGOptimize()
    '    CREATED WITH   |  function only   |  function and gradient
    '    ------------------------------------------------------------
    '    MinCGCreateF() |     work                FAIL
    '    MinCGCreate()  |     FAIL                work
    ' 
    '    Here "FAIL" denotes inappropriate combinations  of  optimizer  creation
    '    function and MinCGOptimize() version. Attemps to use  such  combination
    '    (for  example,  to create optimizer with  MinCGCreateF()  and  to  pass
    '    gradient information to MinCGOptimize()) will lead to  exception  being
    '    thrown. Either  you  did  not  pass  gradient when it WAS needed or you
    '    passed gradient when it was NOT needed.
    ' 
    '   -- ALGLIB --
    '      Copyright 20.04.2009 by Bochkanov Sergey
    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    Public Sub mincgoptimize(state As mincgstate, func As ndimensional_func, rep As ndimensional_rep, obj As Object)
        Dim innerobj As alglib.mincg.mincgstate = state.csobj.innerobj
        If func Is Nothing Then
            Throw New AlglibException("ALGLIB: error in 'mincgoptimize()' (func is null)")
        End If
        Try
            While alglib.mincg.mincgiteration(innerobj)
                If innerobj.needf Then
                    func(innerobj.x, innerobj.f, obj)
                    Continue While
                End If
                If innerobj.xupdated Then
                    If rep IsNot Nothing Then
                        rep(innerobj.x, innerobj.f, obj)
                    End If
                    Continue While
                End If
                Throw New AlglibException("ALGLIB: error in 'mincgoptimize' (some derivatives were not provided?)")
            End While
        Catch E As alglib.alglibexception
            Throw New AlglibException(E.msg)
        End Try
    End Sub


    Public Sub mincgoptimize(state As mincgstate, grad As ndimensional_grad, rep As ndimensional_rep, obj As Object)
        Dim innerobj As alglib.mincg.mincgstate = state.csobj.innerobj
        If grad Is Nothing Then
            Throw New AlglibException("ALGLIB: error in 'mincgoptimize()' (grad is null)")
        End If
        Try
            While alglib.mincg.mincgiteration(innerobj)
                If innerobj.needfg Then
                    grad(innerobj.x, innerobj.f, innerobj.g, obj)
                    Continue While
                End If
                If innerobj.xupdated Then
                    If rep IsNot Nothing Then
                        rep(innerobj.x, innerobj.f, obj)
                    End If
                    Continue While
                End If
                Throw New AlglibException("ALGLIB: error in 'mincgoptimize' (some derivatives were not provided?)")
            End While
        Catch E As alglib.alglibexception
            Throw New AlglibException(E.msg)
        End Try
    End Sub




    Public Sub mincgresults(state As mincgstate, ByRef x() As Double, ByRef rep As mincgreport)
        Try
            rep = New mincgreport()
            alglib.mincgresults(state.csobj, x, rep.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub mincgresultsbuf(state As mincgstate, ByRef x() As Double, ByRef rep As mincgreport)
        Try
            alglib.mincgresultsbuf(state.csobj, x, rep.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub mincgrestartfrom(state As mincgstate, x() As Double)
        Try
            alglib.mincgrestartfrom(state.csobj, x)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub mincgsetgradientcheck(state As mincgstate, teststep As Double)
        Try
            alglib.mincgsetgradientcheck(state.csobj, teststep)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub

    Public Class minbleicstate
        Public csobj As alglib.minbleicstate
    End Class
    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    'This structure stores optimization report:
    '* InnerIterationsCount      number of inner iterations
    '* OuterIterationsCount      number of outer iterations
    '* NFEV                      number of gradient evaluations
    '* TerminationType           termination type (see below)
    '
    'TERMINATION CODES
    '
    'TerminationType field contains completion code, which can be:
    '  -10   unsupported combination of algorithm settings:
    '        1) StpMax is set to non-zero value,
    '        AND 2) non-default preconditioner is used.
    '        You can't use both features at the same moment,
    '        so you have to choose one of them (and to turn
    '        off another one).
    '  -7    gradient verification failed.
    '        See MinBLEICSetGradientCheck() for more information.
    '  -3    inconsistent constraints. Feasible point is
    '        either nonexistent or too hard to find. Try to
    '        restart optimizer with better initial
    '        approximation
    '   4    conditions on constraints are fulfilled
    '        with error less than or equal to EpsC
    '   5    MaxIts steps was taken
    '   7    stopping conditions are too stringent,
    '        further improvement is impossible,
    '        X contains best point found so far.
    '
    'ADDITIONAL FIELDS
    '
    'There are additional fields which can be used for debugging:
    '* DebugEqErr                error in the equality constraints (2-norm)
    '* DebugFS                   f, calculated at projection of initial point
    '                            to the feasible set
    '* DebugFF                   f, calculated at the final point
    '* DebugDX                   |X_start-X_final|
    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    Public Class minbleicreport
        Public Property inneriterationscount() As Integer
            Get
                Return Me.csobj.inneriterationscount
            End Get
            Set(Value As Integer)
                Me.csobj.inneriterationscount = Value
            End Set
        End Property
        Public Property outeriterationscount() As Integer
            Get
                Return Me.csobj.outeriterationscount
            End Get
            Set(Value As Integer)
                Me.csobj.outeriterationscount = Value
            End Set
        End Property
        Public Property nfev() As Integer
            Get
                Return Me.csobj.nfev
            End Get
            Set(Value As Integer)
                Me.csobj.nfev = Value
            End Set
        End Property
        Public Property varidx() As Integer
            Get
                Return Me.csobj.varidx
            End Get
            Set(Value As Integer)
                Me.csobj.varidx = Value
            End Set
        End Property
        Public Property terminationtype() As Integer
            Get
                Return Me.csobj.terminationtype
            End Get
            Set(Value As Integer)
                Me.csobj.terminationtype = Value
            End Set
        End Property
        Public Property debugeqerr() As Double
            Get
                Return Me.csobj.debugeqerr
            End Get
            Set(Value As Double)
                Me.csobj.debugeqerr = Value
            End Set
        End Property
        Public Property debugfs() As Double
            Get
                Return Me.csobj.debugfs
            End Get
            Set(Value As Double)
                Me.csobj.debugfs = Value
            End Set
        End Property
        Public Property debugff() As Double
            Get
                Return Me.csobj.debugff
            End Get
            Set(Value As Double)
                Me.csobj.debugff = Value
            End Set
        End Property
        Public Property debugdx() As Double
            Get
                Return Me.csobj.debugdx
            End Get
            Set(Value As Double)
                Me.csobj.debugdx = Value
            End Set
        End Property
        Public Property debugfeasqpits() As Integer
            Get
                Return Me.csobj.debugfeasqpits
            End Get
            Set(Value As Integer)
                Me.csobj.debugfeasqpits = Value
            End Set
        End Property
        Public Property debugfeasgpaits() As Integer
            Get
                Return Me.csobj.debugfeasgpaits
            End Get
            Set(Value As Integer)
                Me.csobj.debugfeasgpaits = Value
            End Set
        End Property
        Public csobj As alglib.minbleicreport
    End Class


    Public Sub minbleiccreate(n As Integer, x() As Double, ByRef state As minbleicstate)
        Try
            state = New minbleicstate()
            alglib.minbleiccreate(n, x, state.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub minbleiccreate(x() As Double, ByRef state As minbleicstate)
        Try
            state = New minbleicstate()
            alglib.minbleiccreate(x, state.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub minbleiccreatef(n As Integer, x() As Double, diffstep As Double, ByRef state As minbleicstate)
        Try
            state = New minbleicstate()
            alglib.minbleiccreatef(n, x, diffstep, state.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub minbleiccreatef(x() As Double, diffstep As Double, ByRef state As minbleicstate)
        Try
            state = New minbleicstate()
            alglib.minbleiccreatef(x, diffstep, state.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub minbleicsetbc(state As minbleicstate, bndl() As Double, bndu() As Double)
        Try
            alglib.minbleicsetbc(state.csobj, bndl, bndu)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub minbleicsetlc(state As minbleicstate, c(,) As Double, ct() As Integer, k As Integer)
        Try
            alglib.minbleicsetlc(state.csobj, c, ct, k)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub minbleicsetlc(state As minbleicstate, c(,) As Double, ct() As Integer)
        Try
            alglib.minbleicsetlc(state.csobj, c, ct)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub minbleicsetinnercond(state As minbleicstate, epsg As Double, epsf As Double, epsx As Double)
        Try
            alglib.minbleicsetinnercond(state.csobj, epsg, epsf, epsx)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub minbleicsetoutercond(state As minbleicstate, epsx As Double, epsi As Double)
        Try
            alglib.minbleicsetoutercond(state.csobj, epsx, epsi)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub minbleicsetscale(state As minbleicstate, s() As Double)
        Try
            alglib.minbleicsetscale(state.csobj, s)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub minbleicsetprecdefault(state As minbleicstate)
        Try
            alglib.minbleicsetprecdefault(state.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub minbleicsetprecdiag(state As minbleicstate, d() As Double)
        Try
            alglib.minbleicsetprecdiag(state.csobj, d)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub minbleicsetprecscale(state As minbleicstate)
        Try
            alglib.minbleicsetprecscale(state.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub minbleicsetmaxits(state As minbleicstate, maxits As Integer)
        Try
            alglib.minbleicsetmaxits(state.csobj, maxits)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub minbleicsetxrep(state As minbleicstate, needxrep As Boolean)
        Try
            alglib.minbleicsetxrep(state.csobj, needxrep)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub minbleicsetstpmax(state As minbleicstate, stpmax As Double)
        Try
            alglib.minbleicsetstpmax(state.csobj, stpmax)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Function minbleiciteration(state As minbleicstate) As Boolean
        Try
            minbleiciteration = alglib.minbleiciteration(state.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Function
    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    ' This family of functions is used to launcn iterations of nonlinear optimizer
    ' 
    ' These functions accept following parameters:
    '     func    -   callback which calculates function (or merit function)
    '                 value func at given point x
    '     grad    -   callback which calculates function (or merit function)
    '                 value func and gradient grad at given point x
    '     rep     -   optional callback which is called after each iteration
    '                 can be null
    '     obj     -   optional object which is passed to func/grad/hess/jac/rep
    '                 can be null
    ' 
    ' 
    ' NOTES:
    ' 
    ' 1. This function has two different implementations: one which  uses  exact
    '    (analytical) user-supplied gradient,  and one which uses function value
    '    only  and  numerically  differentiates  function  in  order  to  obtain
    '    gradient.
    ' 
    '    Depending  on  the  specific  function  used to create optimizer object
    '    (either  MinBLEICCreate() for analytical gradient or  MinBLEICCreateF()
    '    for numerical differentiation) you should choose appropriate variant of
    '    MinBLEICOptimize() - one  which  accepts  function  AND gradient or one
    '    which accepts function ONLY.
    ' 
    '    Be careful to choose variant of MinBLEICOptimize() which corresponds to
    '    your optimization scheme! Table below lists different  combinations  of
    '    callback (function/gradient) passed to MinBLEICOptimize()  and specific
    '    function used to create optimizer.
    ' 
    ' 
    '                      |         USER PASSED TO MinBLEICOptimize()
    '    CREATED WITH      |  function only   |  function and gradient
    '    ------------------------------------------------------------
    '    MinBLEICCreateF() |     work                FAIL
    '    MinBLEICCreate()  |     FAIL                work
    ' 
    '    Here "FAIL" denotes inappropriate combinations  of  optimizer  creation
    '    function  and  MinBLEICOptimize()  version.   Attemps   to   use   such
    '    combination (for  example,  to  create optimizer with MinBLEICCreateF()
    '    and  to  pass  gradient  information  to  MinCGOptimize()) will lead to
    '    exception being thrown. Either  you  did  not pass gradient when it WAS
    '    needed or you passed gradient when it was NOT needed.
    ' 
    '   -- ALGLIB --
    '      Copyright 28.11.2010 by Bochkanov Sergey
    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    Public Sub minbleicoptimize(state As minbleicstate, func As ndimensional_func, rep As ndimensional_rep, obj As Object)
        Dim innerobj As alglib.minbleic.minbleicstate = state.csobj.innerobj
        If func Is Nothing Then
            Throw New AlglibException("ALGLIB: error in 'minbleicoptimize()' (func is null)")
        End If
        Try
            While alglib.minbleic.minbleiciteration(innerobj)
                If innerobj.needf Then
                    func(innerobj.x, innerobj.f, obj)
                    Continue While
                End If
                If innerobj.xupdated Then
                    If rep IsNot Nothing Then
                        rep(innerobj.x, innerobj.f, obj)
                    End If
                    Continue While
                End If
                Throw New AlglibException("ALGLIB: error in 'minbleicoptimize' (some derivatives were not provided?)")
            End While
        Catch E As alglib.alglibexception
            Throw New AlglibException(E.msg)
        End Try
    End Sub


    Public Sub minbleicoptimize(state As minbleicstate, grad As ndimensional_grad, rep As ndimensional_rep, obj As Object)
        Dim innerobj As alglib.minbleic.minbleicstate = state.csobj.innerobj
        If grad Is Nothing Then
            Throw New AlglibException("ALGLIB: error in 'minbleicoptimize()' (grad is null)")
        End If
        Try
            While alglib.minbleic.minbleiciteration(innerobj)
                If innerobj.needfg Then
                    grad(innerobj.x, innerobj.f, innerobj.g, obj)
                    Continue While
                End If
                If innerobj.xupdated Then
                    If rep IsNot Nothing Then
                        rep(innerobj.x, innerobj.f, obj)
                    End If
                    Continue While
                End If
                Throw New AlglibException("ALGLIB: error in 'minbleicoptimize' (some derivatives were not provided?)")
            End While
        Catch E As alglib.alglibexception
            Throw New AlglibException(E.msg)
        End Try
    End Sub




    Public Sub minbleicresults(state As minbleicstate, ByRef x() As Double, ByRef rep As minbleicreport)
        Try
            rep = New minbleicreport()
            alglib.minbleicresults(state.csobj, x, rep.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub minbleicresultsbuf(state As minbleicstate, ByRef x() As Double, ByRef rep As minbleicreport)
        Try
            alglib.minbleicresultsbuf(state.csobj, x, rep.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub minbleicrestartfrom(state As minbleicstate, x() As Double)
        Try
            alglib.minbleicrestartfrom(state.csobj, x)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub minbleicsetgradientcheck(state As minbleicstate, teststep As Double)
        Try
            alglib.minbleicsetgradientcheck(state.csobj, teststep)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub

    Public Class mcpdstate
        Public csobj As alglib.mcpdstate
    End Class
    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    'This structure is a MCPD training report:
    '    InnerIterationsCount    -   number of inner iterations of the
    '                                underlying optimization algorithm
    '    OuterIterationsCount    -   number of outer iterations of the
    '                                underlying optimization algorithm
    '    NFEV                    -   number of merit function evaluations
    '    TerminationType         -   termination type
    '                                (same as for MinBLEIC optimizer, positive
    '                                values denote success, negative ones -
    '                                failure)
    '
    '  -- ALGLIB --
    '     Copyright 23.05.2010 by Bochkanov Sergey
    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    Public Class mcpdreport
        Public Property inneriterationscount() As Integer
            Get
                Return Me.csobj.inneriterationscount
            End Get
            Set(Value As Integer)
                Me.csobj.inneriterationscount = Value
            End Set
        End Property
        Public Property outeriterationscount() As Integer
            Get
                Return Me.csobj.outeriterationscount
            End Get
            Set(Value As Integer)
                Me.csobj.outeriterationscount = Value
            End Set
        End Property
        Public Property nfev() As Integer
            Get
                Return Me.csobj.nfev
            End Get
            Set(Value As Integer)
                Me.csobj.nfev = Value
            End Set
        End Property
        Public Property terminationtype() As Integer
            Get
                Return Me.csobj.terminationtype
            End Get
            Set(Value As Integer)
                Me.csobj.terminationtype = Value
            End Set
        End Property
        Public csobj As alglib.mcpdreport
    End Class


    Public Sub mcpdcreate(n As Integer, ByRef s As mcpdstate)
        Try
            s = New mcpdstate()
            alglib.mcpdcreate(n, s.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub mcpdcreateentry(n As Integer, entrystate As Integer, ByRef s As mcpdstate)
        Try
            s = New mcpdstate()
            alglib.mcpdcreateentry(n, entrystate, s.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub mcpdcreateexit(n As Integer, exitstate As Integer, ByRef s As mcpdstate)
        Try
            s = New mcpdstate()
            alglib.mcpdcreateexit(n, exitstate, s.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub mcpdcreateentryexit(n As Integer, entrystate As Integer, exitstate As Integer, ByRef s As mcpdstate)
        Try
            s = New mcpdstate()
            alglib.mcpdcreateentryexit(n, entrystate, exitstate, s.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub mcpdaddtrack(s As mcpdstate, xy(,) As Double, k As Integer)
        Try
            alglib.mcpdaddtrack(s.csobj, xy, k)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub mcpdaddtrack(s As mcpdstate, xy(,) As Double)
        Try
            alglib.mcpdaddtrack(s.csobj, xy)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub mcpdsetec(s As mcpdstate, ec(,) As Double)
        Try
            alglib.mcpdsetec(s.csobj, ec)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub mcpdaddec(s As mcpdstate, i As Integer, j As Integer, c As Double)
        Try
            alglib.mcpdaddec(s.csobj, i, j, c)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub mcpdsetbc(s As mcpdstate, bndl(,) As Double, bndu(,) As Double)
        Try
            alglib.mcpdsetbc(s.csobj, bndl, bndu)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub mcpdaddbc(s As mcpdstate, i As Integer, j As Integer, bndl As Double, bndu As Double)
        Try
            alglib.mcpdaddbc(s.csobj, i, j, bndl, bndu)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub mcpdsetlc(s As mcpdstate, c(,) As Double, ct() As Integer, k As Integer)
        Try
            alglib.mcpdsetlc(s.csobj, c, ct, k)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub mcpdsetlc(s As mcpdstate, c(,) As Double, ct() As Integer)
        Try
            alglib.mcpdsetlc(s.csobj, c, ct)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub mcpdsettikhonovregularizer(s As mcpdstate, v As Double)
        Try
            alglib.mcpdsettikhonovregularizer(s.csobj, v)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub mcpdsetprior(s As mcpdstate, pp(,) As Double)
        Try
            alglib.mcpdsetprior(s.csobj, pp)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub mcpdsetpredictionweights(s As mcpdstate, pw() As Double)
        Try
            alglib.mcpdsetpredictionweights(s.csobj, pw)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub mcpdsolve(s As mcpdstate)
        Try
            alglib.mcpdsolve(s.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub mcpdresults(s As mcpdstate, ByRef p(,) As Double, ByRef rep As mcpdreport)
        Try
            rep = New mcpdreport()
            alglib.mcpdresults(s.csobj, p, rep.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub




    Public Class minlbfgsstate
        Public csobj As alglib.minlbfgsstate
    End Class
    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    '
    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    Public Class minlbfgsreport
        Public Property iterationscount() As Integer
            Get
                Return Me.csobj.iterationscount
            End Get
            Set(Value As Integer)
                Me.csobj.iterationscount = Value
            End Set
        End Property
        Public Property nfev() As Integer
            Get
                Return Me.csobj.nfev
            End Get
            Set(Value As Integer)
                Me.csobj.nfev = Value
            End Set
        End Property
        Public Property varidx() As Integer
            Get
                Return Me.csobj.varidx
            End Get
            Set(Value As Integer)
                Me.csobj.varidx = Value
            End Set
        End Property
        Public Property terminationtype() As Integer
            Get
                Return Me.csobj.terminationtype
            End Get
            Set(Value As Integer)
                Me.csobj.terminationtype = Value
            End Set
        End Property
        Public csobj As alglib.minlbfgsreport
    End Class


    Public Sub minlbfgscreate(n As Integer, m As Integer, x() As Double, ByRef state As minlbfgsstate)
        Try
            state = New minlbfgsstate()
            alglib.minlbfgscreate(n, m, x, state.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub minlbfgscreate(m As Integer, x() As Double, ByRef state As minlbfgsstate)
        Try
            state = New minlbfgsstate()
            alglib.minlbfgscreate(m, x, state.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub minlbfgscreatef(n As Integer, m As Integer, x() As Double, diffstep As Double, ByRef state As minlbfgsstate)
        Try
            state = New minlbfgsstate()
            alglib.minlbfgscreatef(n, m, x, diffstep, state.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub minlbfgscreatef(m As Integer, x() As Double, diffstep As Double, ByRef state As minlbfgsstate)
        Try
            state = New minlbfgsstate()
            alglib.minlbfgscreatef(m, x, diffstep, state.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub minlbfgssetcond(state As minlbfgsstate, epsg As Double, epsf As Double, epsx As Double, maxits As Integer)
        Try
            alglib.minlbfgssetcond(state.csobj, epsg, epsf, epsx, maxits)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub minlbfgssetxrep(state As minlbfgsstate, needxrep As Boolean)
        Try
            alglib.minlbfgssetxrep(state.csobj, needxrep)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub minlbfgssetstpmax(state As minlbfgsstate, stpmax As Double)
        Try
            alglib.minlbfgssetstpmax(state.csobj, stpmax)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub minlbfgssetscale(state As minlbfgsstate, s() As Double)
        Try
            alglib.minlbfgssetscale(state.csobj, s)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub minlbfgssetprecdefault(state As minlbfgsstate)
        Try
            alglib.minlbfgssetprecdefault(state.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub minlbfgssetpreccholesky(state As minlbfgsstate, p(,) As Double, isupper As Boolean)
        Try
            alglib.minlbfgssetpreccholesky(state.csobj, p, isupper)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub minlbfgssetprecdiag(state As minlbfgsstate, d() As Double)
        Try
            alglib.minlbfgssetprecdiag(state.csobj, d)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub minlbfgssetprecscale(state As minlbfgsstate)
        Try
            alglib.minlbfgssetprecscale(state.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Function minlbfgsiteration(state As minlbfgsstate) As Boolean
        Try
            minlbfgsiteration = alglib.minlbfgsiteration(state.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Function
    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    ' This family of functions is used to launcn iterations of nonlinear optimizer
    ' 
    ' These functions accept following parameters:
    '     func    -   callback which calculates function (or merit function)
    '                 value func at given point x
    '     grad    -   callback which calculates function (or merit function)
    '                 value func and gradient grad at given point x
    '     rep     -   optional callback which is called after each iteration
    '                 can be null
    '     obj     -   optional object which is passed to func/grad/hess/jac/rep
    '                 can be null
    ' 
    ' 
    ' NOTES:
    ' 
    ' 1. This function has two different implementations: one which  uses  exact
    '    (analytical) user-supplied gradient,  and one which uses function value
    '    only  and  numerically  differentiates  function  in  order  to  obtain
    '    gradient.
    ' 
    '    Depending  on  the  specific  function  used to create optimizer object
    '    (either MinLBFGSCreate() for analytical gradient  or  MinLBFGSCreateF()
    '    for numerical differentiation) you should choose appropriate variant of
    '    MinLBFGSOptimize() - one  which  accepts  function  AND gradient or one
    '    which accepts function ONLY.
    ' 
    '    Be careful to choose variant of MinLBFGSOptimize() which corresponds to
    '    your optimization scheme! Table below lists different  combinations  of
    '    callback (function/gradient) passed to MinLBFGSOptimize()  and specific
    '    function used to create optimizer.
    ' 
    ' 
    '                      |         USER PASSED TO MinLBFGSOptimize()
    '    CREATED WITH      |  function only   |  function and gradient
    '    ------------------------------------------------------------
    '    MinLBFGSCreateF() |     work                FAIL
    '    MinLBFGSCreate()  |     FAIL                work
    ' 
    '    Here "FAIL" denotes inappropriate combinations  of  optimizer  creation
    '    function  and  MinLBFGSOptimize()  version.   Attemps   to   use   such
    '    combination (for example, to create optimizer with MinLBFGSCreateF() and
    '    to pass gradient information to MinCGOptimize()) will lead to exception
    '    being thrown. Either  you  did  not pass gradient when it WAS needed or
    '    you passed gradient when it was NOT needed.
    ' 
    '   -- ALGLIB --
    '      Copyright 20.03.2009 by Bochkanov Sergey
    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    Public Sub minlbfgsoptimize(state As minlbfgsstate, func As ndimensional_func, rep As ndimensional_rep, obj As Object)
        Dim innerobj As alglib.minlbfgs.minlbfgsstate = state.csobj.innerobj
        If func Is Nothing Then
            Throw New AlglibException("ALGLIB: error in 'minlbfgsoptimize()' (func is null)")
        End If
        Try
            While alglib.minlbfgs.minlbfgsiteration(innerobj)
                If innerobj.needf Then
                    func(innerobj.x, innerobj.f, obj)
                    Continue While
                End If
                If innerobj.xupdated Then
                    If rep IsNot Nothing Then
                        rep(innerobj.x, innerobj.f, obj)
                    End If
                    Continue While
                End If
                Throw New AlglibException("ALGLIB: error in 'minlbfgsoptimize' (some derivatives were not provided?)")
            End While
        Catch E As alglib.alglibexception
            Throw New AlglibException(E.msg)
        End Try
    End Sub


    Public Sub minlbfgsoptimize(state As minlbfgsstate, grad As ndimensional_grad, rep As ndimensional_rep, obj As Object)
        Dim innerobj As alglib.minlbfgs.minlbfgsstate = state.csobj.innerobj
        If grad Is Nothing Then
            Throw New AlglibException("ALGLIB: error in 'minlbfgsoptimize()' (grad is null)")
        End If
        Try
            While alglib.minlbfgs.minlbfgsiteration(innerobj)
                If innerobj.needfg Then
                    grad(innerobj.x, innerobj.f, innerobj.g, obj)
                    Continue While
                End If
                If innerobj.xupdated Then
                    If rep IsNot Nothing Then
                        rep(innerobj.x, innerobj.f, obj)
                    End If
                    Continue While
                End If
                Throw New AlglibException("ALGLIB: error in 'minlbfgsoptimize' (some derivatives were not provided?)")
            End While
        Catch E As alglib.alglibexception
            Throw New AlglibException(E.msg)
        End Try
    End Sub




    Public Sub minlbfgsresults(state As minlbfgsstate, ByRef x() As Double, ByRef rep As minlbfgsreport)
        Try
            rep = New minlbfgsreport()
            alglib.minlbfgsresults(state.csobj, x, rep.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub minlbfgsresultsbuf(state As minlbfgsstate, ByRef x() As Double, ByRef rep As minlbfgsreport)
        Try
            alglib.minlbfgsresultsbuf(state.csobj, x, rep.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub minlbfgsrestartfrom(state As minlbfgsstate, x() As Double)
        Try
            alglib.minlbfgsrestartfrom(state.csobj, x)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub minlbfgssetgradientcheck(state As minlbfgsstate, teststep As Double)
        Try
            alglib.minlbfgssetgradientcheck(state.csobj, teststep)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub

    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    'Training report:
    '    * NGrad     - number of gradient calculations
    '    * NHess     - number of Hessian calculations
    '    * NCholesky - number of Cholesky decompositions
    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    Public Class mlpreport
        Public Property ngrad() As Integer
            Get
                Return Me.csobj.ngrad
            End Get
            Set(Value As Integer)
                Me.csobj.ngrad = Value
            End Set
        End Property
        Public Property nhess() As Integer
            Get
                Return Me.csobj.nhess
            End Get
            Set(Value As Integer)
                Me.csobj.nhess = Value
            End Set
        End Property
        Public Property ncholesky() As Integer
            Get
                Return Me.csobj.ncholesky
            End Get
            Set(Value As Integer)
                Me.csobj.ncholesky = Value
            End Set
        End Property
        Public csobj As alglib.mlpreport
    End Class
    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    'Cross-validation estimates of generalization error
    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    Public Class mlpcvreport
        Public Property relclserror() As Double
            Get
                Return Me.csobj.relclserror
            End Get
            Set(Value As Double)
                Me.csobj.relclserror = Value
            End Set
        End Property
        Public Property avgce() As Double
            Get
                Return Me.csobj.avgce
            End Get
            Set(Value As Double)
                Me.csobj.avgce = Value
            End Set
        End Property
        Public Property rmserror() As Double
            Get
                Return Me.csobj.rmserror
            End Get
            Set(Value As Double)
                Me.csobj.rmserror = Value
            End Set
        End Property
        Public Property avgerror() As Double
            Get
                Return Me.csobj.avgerror
            End Get
            Set(Value As Double)
                Me.csobj.avgerror = Value
            End Set
        End Property
        Public Property avgrelerror() As Double
            Get
                Return Me.csobj.avgrelerror
            End Get
            Set(Value As Double)
                Me.csobj.avgrelerror = Value
            End Set
        End Property
        Public csobj As alglib.mlpcvreport
    End Class


    Public Sub mlptrainlm(network As multilayerperceptron, xy(,) As Double, npoints As Integer, decay As Double, restarts As Integer, ByRef info As Integer, ByRef rep As mlpreport)
        Try
            rep = New mlpreport()
            alglib.mlptrainlm(network.csobj, xy, npoints, decay, restarts, info, rep.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub mlptrainlbfgs(network As multilayerperceptron, xy(,) As Double, npoints As Integer, decay As Double, restarts As Integer, wstep As Double, maxits As Integer, ByRef info As Integer, ByRef rep As mlpreport)
        Try
            rep = New mlpreport()
            alglib.mlptrainlbfgs(network.csobj, xy, npoints, decay, restarts, wstep, maxits, info, rep.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub mlptraines(network As multilayerperceptron, trnxy(,) As Double, trnsize As Integer, valxy(,) As Double, valsize As Integer, decay As Double, restarts As Integer, ByRef info As Integer, ByRef rep As mlpreport)
        Try
            rep = New mlpreport()
            alglib.mlptraines(network.csobj, trnxy, trnsize, valxy, valsize, decay, restarts, info, rep.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub mlpkfoldcvlbfgs(network As multilayerperceptron, xy(,) As Double, npoints As Integer, decay As Double, restarts As Integer, wstep As Double, maxits As Integer, foldscount As Integer, ByRef info As Integer, ByRef rep As mlpreport, ByRef cvrep As mlpcvreport)
        Try
            rep = New mlpreport()
            cvrep = New mlpcvreport()
            alglib.mlpkfoldcvlbfgs(network.csobj, xy, npoints, decay, restarts, wstep, maxits, foldscount, info, rep.csobj, cvrep.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub mlpkfoldcvlm(network As multilayerperceptron, xy(,) As Double, npoints As Integer, decay As Double, restarts As Integer, foldscount As Integer, ByRef info As Integer, ByRef rep As mlpreport, ByRef cvrep As mlpcvreport)
        Try
            rep = New mlpreport()
            cvrep = New mlpcvreport()
            alglib.mlpkfoldcvlm(network.csobj, xy, npoints, decay, restarts, foldscount, info, rep.csobj, cvrep.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub

    Public Class mlpensemble
        Public csobj As alglib.mlpensemble
    End Class
    Public Sub mlpeserialize(obj As mlpensemble, ByRef s_out As String)
        Try
            alglib.mlpeserialize(obj.csobj, s_out)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub

    Public Sub mlpeunserialize(s_in As String, ByRef obj As mlpensemble)
        Try
            alglib.mlpeunserialize(s_in, obj.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub mlpecreate0(nin As Integer, nout As Integer, ensemblesize As Integer, ByRef ensemble As mlpensemble)
        Try
            ensemble = New mlpensemble()
            alglib.mlpecreate0(nin, nout, ensemblesize, ensemble.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub mlpecreate1(nin As Integer, nhid As Integer, nout As Integer, ensemblesize As Integer, ByRef ensemble As mlpensemble)
        Try
            ensemble = New mlpensemble()
            alglib.mlpecreate1(nin, nhid, nout, ensemblesize, ensemble.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub mlpecreate2(nin As Integer, nhid1 As Integer, nhid2 As Integer, nout As Integer, ensemblesize As Integer, ByRef ensemble As mlpensemble)
        Try
            ensemble = New mlpensemble()
            alglib.mlpecreate2(nin, nhid1, nhid2, nout, ensemblesize, ensemble.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub mlpecreateb0(nin As Integer, nout As Integer, b As Double, d As Double, ensemblesize As Integer, ByRef ensemble As mlpensemble)
        Try
            ensemble = New mlpensemble()
            alglib.mlpecreateb0(nin, nout, b, d, ensemblesize, ensemble.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub mlpecreateb1(nin As Integer, nhid As Integer, nout As Integer, b As Double, d As Double, ensemblesize As Integer, ByRef ensemble As mlpensemble)
        Try
            ensemble = New mlpensemble()
            alglib.mlpecreateb1(nin, nhid, nout, b, d, ensemblesize, ensemble.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub mlpecreateb2(nin As Integer, nhid1 As Integer, nhid2 As Integer, nout As Integer, b As Double, d As Double, ensemblesize As Integer, ByRef ensemble As mlpensemble)
        Try
            ensemble = New mlpensemble()
            alglib.mlpecreateb2(nin, nhid1, nhid2, nout, b, d, ensemblesize, ensemble.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub mlpecreater0(nin As Integer, nout As Integer, a As Double, b As Double, ensemblesize As Integer, ByRef ensemble As mlpensemble)
        Try
            ensemble = New mlpensemble()
            alglib.mlpecreater0(nin, nout, a, b, ensemblesize, ensemble.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub mlpecreater1(nin As Integer, nhid As Integer, nout As Integer, a As Double, b As Double, ensemblesize As Integer, ByRef ensemble As mlpensemble)
        Try
            ensemble = New mlpensemble()
            alglib.mlpecreater1(nin, nhid, nout, a, b, ensemblesize, ensemble.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub mlpecreater2(nin As Integer, nhid1 As Integer, nhid2 As Integer, nout As Integer, a As Double, b As Double, ensemblesize As Integer, ByRef ensemble As mlpensemble)
        Try
            ensemble = New mlpensemble()
            alglib.mlpecreater2(nin, nhid1, nhid2, nout, a, b, ensemblesize, ensemble.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub mlpecreatec0(nin As Integer, nout As Integer, ensemblesize As Integer, ByRef ensemble As mlpensemble)
        Try
            ensemble = New mlpensemble()
            alglib.mlpecreatec0(nin, nout, ensemblesize, ensemble.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub mlpecreatec1(nin As Integer, nhid As Integer, nout As Integer, ensemblesize As Integer, ByRef ensemble As mlpensemble)
        Try
            ensemble = New mlpensemble()
            alglib.mlpecreatec1(nin, nhid, nout, ensemblesize, ensemble.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub mlpecreatec2(nin As Integer, nhid1 As Integer, nhid2 As Integer, nout As Integer, ensemblesize As Integer, ByRef ensemble As mlpensemble)
        Try
            ensemble = New mlpensemble()
            alglib.mlpecreatec2(nin, nhid1, nhid2, nout, ensemblesize, ensemble.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub mlpecreatefromnetwork(network As multilayerperceptron, ensemblesize As Integer, ByRef ensemble As mlpensemble)
        Try
            ensemble = New mlpensemble()
            alglib.mlpecreatefromnetwork(network.csobj, ensemblesize, ensemble.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub mlperandomize(ensemble As mlpensemble)
        Try
            alglib.mlperandomize(ensemble.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub mlpeproperties(ensemble As mlpensemble, ByRef nin As Integer, ByRef nout As Integer)
        Try
            alglib.mlpeproperties(ensemble.csobj, nin, nout)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Function mlpeissoftmax(ensemble As mlpensemble) As Boolean
        Try
            mlpeissoftmax = alglib.mlpeissoftmax(ensemble.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Function


    Public Sub mlpeprocess(ensemble As mlpensemble, x() As Double, ByRef y() As Double)
        Try
            alglib.mlpeprocess(ensemble.csobj, x, y)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub mlpeprocessi(ensemble As mlpensemble, x() As Double, ByRef y() As Double)
        Try
            alglib.mlpeprocessi(ensemble.csobj, x, y)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Function mlperelclserror(ensemble As mlpensemble, xy(,) As Double, npoints As Integer) As Double
        Try
            mlperelclserror = alglib.mlperelclserror(ensemble.csobj, xy, npoints)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Function


    Public Function mlpeavgce(ensemble As mlpensemble, xy(,) As Double, npoints As Integer) As Double
        Try
            mlpeavgce = alglib.mlpeavgce(ensemble.csobj, xy, npoints)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Function


    Public Function mlpermserror(ensemble As mlpensemble, xy(,) As Double, npoints As Integer) As Double
        Try
            mlpermserror = alglib.mlpermserror(ensemble.csobj, xy, npoints)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Function


    Public Function mlpeavgerror(ensemble As mlpensemble, xy(,) As Double, npoints As Integer) As Double
        Try
            mlpeavgerror = alglib.mlpeavgerror(ensemble.csobj, xy, npoints)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Function


    Public Function mlpeavgrelerror(ensemble As mlpensemble, xy(,) As Double, npoints As Integer) As Double
        Try
            mlpeavgrelerror = alglib.mlpeavgrelerror(ensemble.csobj, xy, npoints)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Function


    Public Sub mlpebagginglm(ensemble As mlpensemble, xy(,) As Double, npoints As Integer, decay As Double, restarts As Integer, ByRef info As Integer, ByRef rep As mlpreport, ByRef ooberrors As mlpcvreport)
        Try
            rep = New mlpreport()
            ooberrors = New mlpcvreport()
            alglib.mlpebagginglm(ensemble.csobj, xy, npoints, decay, restarts, info, rep.csobj, ooberrors.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub mlpebagginglbfgs(ensemble As mlpensemble, xy(,) As Double, npoints As Integer, decay As Double, restarts As Integer, wstep As Double, maxits As Integer, ByRef info As Integer, ByRef rep As mlpreport, ByRef ooberrors As mlpcvreport)
        Try
            rep = New mlpreport()
            ooberrors = New mlpcvreport()
            alglib.mlpebagginglbfgs(ensemble.csobj, xy, npoints, decay, restarts, wstep, maxits, info, rep.csobj, ooberrors.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub mlpetraines(ensemble As mlpensemble, xy(,) As Double, npoints As Integer, decay As Double, restarts As Integer, ByRef info As Integer, ByRef rep As mlpreport)
        Try
            rep = New mlpreport()
            alglib.mlpetraines(ensemble.csobj, xy, npoints, decay, restarts, info, rep.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub




    Public Sub pcabuildbasis(x(,) As Double, npoints As Integer, nvars As Integer, ByRef info As Integer, ByRef s2() As Double, ByRef v(,) As Double)
        Try
            alglib.pcabuildbasis(x, npoints, nvars, info, s2, v)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub

    Public Class odesolverstate
        Public csobj As alglib.odesolverstate
    End Class
    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    '
    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    Public Class odesolverreport
        Public Property nfev() As Integer
            Get
                Return Me.csobj.nfev
            End Get
            Set(Value As Integer)
                Me.csobj.nfev = Value
            End Set
        End Property
        Public Property terminationtype() As Integer
            Get
                Return Me.csobj.terminationtype
            End Get
            Set(Value As Integer)
                Me.csobj.terminationtype = Value
            End Set
        End Property
        Public csobj As alglib.odesolverreport
    End Class


    Public Sub odesolverrkck(y() As Double, n As Integer, x() As Double, m As Integer, eps As Double, h As Double, ByRef state As odesolverstate)
        Try
            state = New odesolverstate()
            alglib.odesolverrkck(y, n, x, m, eps, h, state.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub odesolverrkck(y() As Double, x() As Double, eps As Double, h As Double, ByRef state As odesolverstate)
        Try
            state = New odesolverstate()
            alglib.odesolverrkck(y, x, eps, h, state.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Function odesolveriteration(state As odesolverstate) As Boolean
        Try
            odesolveriteration = alglib.odesolveriteration(state.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Function
    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    ' This function is used to launcn iterations of ODE solver
    '
    ' It accepts following parameters:
    '     diff    -   callback which calculates dy/dx for given y and x
    '     obj     -   optional object which is passed to diff; can be NULL
    '
    ' 
    '   -- ALGLIB --
    '      Copyright 01.09.2009 by Bochkanov Sergey
    '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''/
    Public Sub odesolversolve(state As odesolverstate, diff As ndimensional_ode_rp, obj As Object)
        If diff Is Nothing Then
            Throw New AlglibException("ALGLIB: error in 'odesolversolve()' (diff is null)")
        End If
        Dim innerobj As alglib.odesolver.odesolverstate = state.csobj.innerobj
        Try
            While alglib.odesolver.odesolveriteration(innerobj)
                If innerobj.needdy Then
                    diff(innerobj.y, innerobj.x, innerobj.dy, obj)
                    Continue While
                End If
                Throw New AlglibException("ALGLIB: unexpected error in 'odesolversolve'")
            End While
        Catch E As alglib.alglibexception
            Throw New AlglibException(E.msg)
        End Try
    End Sub




    Public Sub odesolverresults(state As odesolverstate, ByRef m As Integer, ByRef xtbl() As Double, ByRef ytbl(,) As Double, ByRef rep As odesolverreport)
        Try
            rep = New odesolverreport()
            alglib.odesolverresults(state.csobj, m, xtbl, ytbl, rep.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub




    Public Sub fftc1d(ByRef a() As alglib.complex, n As Integer)
        Try
            alglib.fftc1d(a, n)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub fftc1d(ByRef a() As alglib.complex)
        Try
            alglib.fftc1d(a)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub fftc1dinv(ByRef a() As alglib.complex, n As Integer)
        Try
            alglib.fftc1dinv(a, n)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub fftc1dinv(ByRef a() As alglib.complex)
        Try
            alglib.fftc1dinv(a)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub fftr1d(a() As Double, n As Integer, ByRef f() As alglib.complex)
        Try
            alglib.fftr1d(a, n, f)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub fftr1d(a() As Double, ByRef f() As alglib.complex)
        Try
            alglib.fftr1d(a, f)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub fftr1dinv(f() As alglib.complex, n As Integer, ByRef a() As Double)
        Try
            alglib.fftr1dinv(f, n, a)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub fftr1dinv(f() As alglib.complex, ByRef a() As Double)
        Try
            alglib.fftr1dinv(f, a)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub




    Public Sub convc1d(a() As alglib.complex, m As Integer, b() As alglib.complex, n As Integer, ByRef r() As alglib.complex)
        Try
            alglib.convc1d(a, m, b, n, r)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub convc1dinv(a() As alglib.complex, m As Integer, b() As alglib.complex, n As Integer, ByRef r() As alglib.complex)
        Try
            alglib.convc1dinv(a, m, b, n, r)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub convc1dcircular(s() As alglib.complex, m As Integer, r() As alglib.complex, n As Integer, ByRef c() As alglib.complex)
        Try
            alglib.convc1dcircular(s, m, r, n, c)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub convc1dcircularinv(a() As alglib.complex, m As Integer, b() As alglib.complex, n As Integer, ByRef r() As alglib.complex)
        Try
            alglib.convc1dcircularinv(a, m, b, n, r)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub convr1d(a() As Double, m As Integer, b() As Double, n As Integer, ByRef r() As Double)
        Try
            alglib.convr1d(a, m, b, n, r)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub convr1dinv(a() As Double, m As Integer, b() As Double, n As Integer, ByRef r() As Double)
        Try
            alglib.convr1dinv(a, m, b, n, r)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub convr1dcircular(s() As Double, m As Integer, r() As Double, n As Integer, ByRef c() As Double)
        Try
            alglib.convr1dcircular(s, m, r, n, c)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub convr1dcircularinv(a() As Double, m As Integer, b() As Double, n As Integer, ByRef r() As Double)
        Try
            alglib.convr1dcircularinv(a, m, b, n, r)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub




    Public Sub corrc1d(signal() As alglib.complex, n As Integer, pattern() As alglib.complex, m As Integer, ByRef r() As alglib.complex)
        Try
            alglib.corrc1d(signal, n, pattern, m, r)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub corrc1dcircular(signal() As alglib.complex, m As Integer, pattern() As alglib.complex, n As Integer, ByRef c() As alglib.complex)
        Try
            alglib.corrc1dcircular(signal, m, pattern, n, c)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub corrr1d(signal() As Double, n As Integer, pattern() As Double, m As Integer, ByRef r() As Double)
        Try
            alglib.corrr1d(signal, n, pattern, m, r)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub corrr1dcircular(signal() As Double, m As Integer, pattern() As Double, n As Integer, ByRef c() As Double)
        Try
            alglib.corrr1dcircular(signal, m, pattern, n, c)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub




    Public Sub fhtr1d(ByRef a() As Double, n As Integer)
        Try
            alglib.fhtr1d(a, n)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub fhtr1dinv(ByRef a() As Double, n As Integer)
        Try
            alglib.fhtr1dinv(a, n)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub




    Public Sub gqgeneraterec(alpha() As Double, beta() As Double, mu0 As Double, n As Integer, ByRef info As Integer, ByRef x() As Double, ByRef w() As Double)
        Try
            alglib.gqgeneraterec(alpha, beta, mu0, n, info, x, w)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub gqgenerategausslobattorec(alpha() As Double, beta() As Double, mu0 As Double, a As Double, b As Double, n As Integer, ByRef info As Integer, ByRef x() As Double, ByRef w() As Double)
        Try
            alglib.gqgenerategausslobattorec(alpha, beta, mu0, a, b, n, info, x, w)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub gqgenerategaussradaurec(alpha() As Double, beta() As Double, mu0 As Double, a As Double, n As Integer, ByRef info As Integer, ByRef x() As Double, ByRef w() As Double)
        Try
            alglib.gqgenerategaussradaurec(alpha, beta, mu0, a, n, info, x, w)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub gqgenerategausslegendre(n As Integer, ByRef info As Integer, ByRef x() As Double, ByRef w() As Double)
        Try
            alglib.gqgenerategausslegendre(n, info, x, w)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub gqgenerategaussjacobi(n As Integer, alpha As Double, beta As Double, ByRef info As Integer, ByRef x() As Double, ByRef w() As Double)
        Try
            alglib.gqgenerategaussjacobi(n, alpha, beta, info, x, w)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub gqgenerategausslaguerre(n As Integer, alpha As Double, ByRef info As Integer, ByRef x() As Double, ByRef w() As Double)
        Try
            alglib.gqgenerategausslaguerre(n, alpha, info, x, w)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub gqgenerategausshermite(n As Integer, ByRef info As Integer, ByRef x() As Double, ByRef w() As Double)
        Try
            alglib.gqgenerategausshermite(n, info, x, w)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub




    Public Sub gkqgeneraterec(alpha() As Double, beta() As Double, mu0 As Double, n As Integer, ByRef info As Integer, ByRef x() As Double, ByRef wkronrod() As Double, ByRef wgauss() As Double)
        Try
            alglib.gkqgeneraterec(alpha, beta, mu0, n, info, x, wkronrod, wgauss)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub gkqgenerategausslegendre(n As Integer, ByRef info As Integer, ByRef x() As Double, ByRef wkronrod() As Double, ByRef wgauss() As Double)
        Try
            alglib.gkqgenerategausslegendre(n, info, x, wkronrod, wgauss)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub gkqgenerategaussjacobi(n As Integer, alpha As Double, beta As Double, ByRef info As Integer, ByRef x() As Double, ByRef wkronrod() As Double, ByRef wgauss() As Double)
        Try
            alglib.gkqgenerategaussjacobi(n, alpha, beta, info, x, wkronrod, wgauss)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub gkqlegendrecalc(n As Integer, ByRef info As Integer, ByRef x() As Double, ByRef wkronrod() As Double, ByRef wgauss() As Double)
        Try
            alglib.gkqlegendrecalc(n, info, x, wkronrod, wgauss)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub gkqlegendretbl(n As Integer, ByRef x() As Double, ByRef wkronrod() As Double, ByRef wgauss() As Double, ByRef eps As Double)
        Try
            alglib.gkqlegendretbl(n, x, wkronrod, wgauss, eps)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub

    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    'Integration report:
    '* TerminationType = completetion code:
    '    * -5    non-convergence of Gauss-Kronrod nodes
    '            calculation subroutine.
    '    * -1    incorrect parameters were specified
    '    *  1    OK
    '* Rep.NFEV countains number of function calculations
    '* Rep.NIntervals contains number of intervals [a,b]
    '  was partitioned into.
    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    Public Class autogkreport
        Public Property terminationtype() As Integer
            Get
                Return Me.csobj.terminationtype
            End Get
            Set(Value As Integer)
                Me.csobj.terminationtype = Value
            End Set
        End Property
        Public Property nfev() As Integer
            Get
                Return Me.csobj.nfev
            End Get
            Set(Value As Integer)
                Me.csobj.nfev = Value
            End Set
        End Property
        Public Property nintervals() As Integer
            Get
                Return Me.csobj.nintervals
            End Get
            Set(Value As Integer)
                Me.csobj.nintervals = Value
            End Set
        End Property
        Public csobj As alglib.autogkreport
    End Class
    Public Class autogkstate
        Public csobj As alglib.autogkstate
    End Class


    Public Sub autogksmooth(a As Double, b As Double, ByRef state As autogkstate)
        Try
            state = New autogkstate()
            alglib.autogksmooth(a, b, state.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub autogksmoothw(a As Double, b As Double, xwidth As Double, ByRef state As autogkstate)
        Try
            state = New autogkstate()
            alglib.autogksmoothw(a, b, xwidth, state.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub autogksingular(a As Double, b As Double, alpha As Double, beta As Double, ByRef state As autogkstate)
        Try
            state = New autogkstate()
            alglib.autogksingular(a, b, alpha, beta, state.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Function autogkiteration(state As autogkstate) As Boolean
        Try
            autogkiteration = alglib.autogkiteration(state.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Function


    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    ' This function is used to launcn iterations of ODE solver
    '
    ' It accepts following parameters:
    '     diff    -   callback which calculates dy/dx for given y and x
    '     obj     -   optional object which is passed to diff; can be NULL
    '
    ' 
    '   -- ALGLIB --
    '      Copyright 07.05.2009 by Bochkanov Sergey
    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    Public Sub autogkintegrate(state As autogkstate, func As integrator1_func, obj As Object)
        If func Is Nothing Then
            Throw New AlglibException("ALGLIB: error in 'autogkintegrate()' (func is null)")
        End If
        Dim innerobj As alglib.autogk.autogkstate = state.csobj.innerobj
        Try
            While alglib.autogk.autogkiteration(innerobj)
                If innerobj.needf Then
                    func(innerobj.x, innerobj.xminusa, innerobj.bminusx, innerobj.f, obj)
                    Continue While
                End If
                Throw New AlglibException("ALGLIB: unexpected error in 'autogksolve'")
            End While
        Catch E As alglib.alglibexception
            Throw New AlglibException(E.msg)
        End Try
    End Sub


    Public Sub autogkresults(state As autogkstate, ByRef v As Double, ByRef rep As autogkreport)
        Try
            rep = New autogkreport()
            alglib.autogkresults(state.csobj, v, rep.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub

    Public Class idwinterpolant
        Public csobj As alglib.idwinterpolant
    End Class


    Public Function idwcalc(z As idwinterpolant, x() As Double) As Double
        Try
            idwcalc = alglib.idwcalc(z.csobj, x)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Function


    Public Sub idwbuildmodifiedshepard(xy(,) As Double, n As Integer, nx As Integer, d As Integer, nq As Integer, nw As Integer, ByRef z As idwinterpolant)
        Try
            z = New idwinterpolant()
            alglib.idwbuildmodifiedshepard(xy, n, nx, d, nq, nw, z.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub idwbuildmodifiedshepardr(xy(,) As Double, n As Integer, nx As Integer, r As Double, ByRef z As idwinterpolant)
        Try
            z = New idwinterpolant()
            alglib.idwbuildmodifiedshepardr(xy, n, nx, r, z.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub idwbuildnoisy(xy(,) As Double, n As Integer, nx As Integer, d As Integer, nq As Integer, nw As Integer, ByRef z As idwinterpolant)
        Try
            z = New idwinterpolant()
            alglib.idwbuildnoisy(xy, n, nx, d, nq, nw, z.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub

    Public Class barycentricinterpolant
        Public csobj As alglib.barycentricinterpolant
    End Class


    Public Function barycentriccalc(b As barycentricinterpolant, t As Double) As Double
        Try
            barycentriccalc = alglib.barycentriccalc(b.csobj, t)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Function


    Public Sub barycentricdiff1(b As barycentricinterpolant, t As Double, ByRef f As Double, ByRef df As Double)
        Try
            alglib.barycentricdiff1(b.csobj, t, f, df)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub barycentricdiff2(b As barycentricinterpolant, t As Double, ByRef f As Double, ByRef df As Double, ByRef d2f As Double)
        Try
            alglib.barycentricdiff2(b.csobj, t, f, df, d2f)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub barycentriclintransx(b As barycentricinterpolant, ca As Double, cb As Double)
        Try
            alglib.barycentriclintransx(b.csobj, ca, cb)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub barycentriclintransy(b As barycentricinterpolant, ca As Double, cb As Double)
        Try
            alglib.barycentriclintransy(b.csobj, ca, cb)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub barycentricunpack(b As barycentricinterpolant, ByRef n As Integer, ByRef x() As Double, ByRef y() As Double, ByRef w() As Double)
        Try
            alglib.barycentricunpack(b.csobj, n, x, y, w)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub barycentricbuildxyw(x() As Double, y() As Double, w() As Double, n As Integer, ByRef b As barycentricinterpolant)
        Try
            b = New barycentricinterpolant()
            alglib.barycentricbuildxyw(x, y, w, n, b.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub barycentricbuildfloaterhormann(x() As Double, y() As Double, n As Integer, d As Integer, ByRef b As barycentricinterpolant)
        Try
            b = New barycentricinterpolant()
            alglib.barycentricbuildfloaterhormann(x, y, n, d, b.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub




    Public Sub polynomialbar2cheb(p As barycentricinterpolant, a As Double, b As Double, ByRef t() As Double)
        Try
            alglib.polynomialbar2cheb(p.csobj, a, b, t)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub polynomialcheb2bar(t() As Double, n As Integer, a As Double, b As Double, ByRef p As barycentricinterpolant)
        Try
            p = New barycentricinterpolant()
            alglib.polynomialcheb2bar(t, n, a, b, p.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub polynomialcheb2bar(t() As Double, a As Double, b As Double, ByRef p As barycentricinterpolant)
        Try
            p = New barycentricinterpolant()
            alglib.polynomialcheb2bar(t, a, b, p.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub polynomialbar2pow(p As barycentricinterpolant, c As Double, s As Double, ByRef a() As Double)
        Try
            alglib.polynomialbar2pow(p.csobj, c, s, a)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub polynomialbar2pow(p As barycentricinterpolant, ByRef a() As Double)
        Try
            alglib.polynomialbar2pow(p.csobj, a)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub polynomialpow2bar(a() As Double, n As Integer, c As Double, s As Double, ByRef p As barycentricinterpolant)
        Try
            p = New barycentricinterpolant()
            alglib.polynomialpow2bar(a, n, c, s, p.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub polynomialpow2bar(a() As Double, ByRef p As barycentricinterpolant)
        Try
            p = New barycentricinterpolant()
            alglib.polynomialpow2bar(a, p.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub polynomialbuild(x() As Double, y() As Double, n As Integer, ByRef p As barycentricinterpolant)
        Try
            p = New barycentricinterpolant()
            alglib.polynomialbuild(x, y, n, p.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub polynomialbuild(x() As Double, y() As Double, ByRef p As barycentricinterpolant)
        Try
            p = New barycentricinterpolant()
            alglib.polynomialbuild(x, y, p.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub polynomialbuildeqdist(a As Double, b As Double, y() As Double, n As Integer, ByRef p As barycentricinterpolant)
        Try
            p = New barycentricinterpolant()
            alglib.polynomialbuildeqdist(a, b, y, n, p.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub polynomialbuildeqdist(a As Double, b As Double, y() As Double, ByRef p As barycentricinterpolant)
        Try
            p = New barycentricinterpolant()
            alglib.polynomialbuildeqdist(a, b, y, p.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub polynomialbuildcheb1(a As Double, b As Double, y() As Double, n As Integer, ByRef p As barycentricinterpolant)
        Try
            p = New barycentricinterpolant()
            alglib.polynomialbuildcheb1(a, b, y, n, p.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub polynomialbuildcheb1(a As Double, b As Double, y() As Double, ByRef p As barycentricinterpolant)
        Try
            p = New barycentricinterpolant()
            alglib.polynomialbuildcheb1(a, b, y, p.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub polynomialbuildcheb2(a As Double, b As Double, y() As Double, n As Integer, ByRef p As barycentricinterpolant)
        Try
            p = New barycentricinterpolant()
            alglib.polynomialbuildcheb2(a, b, y, n, p.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub polynomialbuildcheb2(a As Double, b As Double, y() As Double, ByRef p As barycentricinterpolant)
        Try
            p = New barycentricinterpolant()
            alglib.polynomialbuildcheb2(a, b, y, p.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Function polynomialcalceqdist(a As Double, b As Double, f() As Double, n As Integer, t As Double) As Double
        Try
            polynomialcalceqdist = alglib.polynomialcalceqdist(a, b, f, n, t)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Function


    Public Function polynomialcalceqdist(a As Double, b As Double, f() As Double, t As Double) As Double
        Try
            polynomialcalceqdist = alglib.polynomialcalceqdist(a, b, f, t)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Function


    Public Function polynomialcalccheb1(a As Double, b As Double, f() As Double, n As Integer, t As Double) As Double
        Try
            polynomialcalccheb1 = alglib.polynomialcalccheb1(a, b, f, n, t)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Function


    Public Function polynomialcalccheb1(a As Double, b As Double, f() As Double, t As Double) As Double
        Try
            polynomialcalccheb1 = alglib.polynomialcalccheb1(a, b, f, t)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Function


    Public Function polynomialcalccheb2(a As Double, b As Double, f() As Double, n As Integer, t As Double) As Double
        Try
            polynomialcalccheb2 = alglib.polynomialcalccheb2(a, b, f, n, t)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Function


    Public Function polynomialcalccheb2(a As Double, b As Double, f() As Double, t As Double) As Double
        Try
            polynomialcalccheb2 = alglib.polynomialcalccheb2(a, b, f, t)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Function

    Public Class spline1dinterpolant
        Public csobj As alglib.spline1dinterpolant
    End Class


    Public Sub spline1dbuildlinear(x() As Double, y() As Double, n As Integer, ByRef c As spline1dinterpolant)
        Try
            c = New spline1dinterpolant()
            alglib.spline1dbuildlinear(x, y, n, c.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub spline1dbuildlinear(x() As Double, y() As Double, ByRef c As spline1dinterpolant)
        Try
            c = New spline1dinterpolant()
            alglib.spline1dbuildlinear(x, y, c.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub spline1dbuildcubic(x() As Double, y() As Double, n As Integer, boundltype As Integer, boundl As Double, boundrtype As Integer, boundr As Double, ByRef c As spline1dinterpolant)
        Try
            c = New spline1dinterpolant()
            alglib.spline1dbuildcubic(x, y, n, boundltype, boundl, boundrtype, boundr, c.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub spline1dbuildcubic(x() As Double, y() As Double, ByRef c As spline1dinterpolant)
        Try
            c = New spline1dinterpolant()
            alglib.spline1dbuildcubic(x, y, c.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub spline1dgriddiffcubic(x() As Double, y() As Double, n As Integer, boundltype As Integer, boundl As Double, boundrtype As Integer, boundr As Double, ByRef d() As Double)
        Try
            alglib.spline1dgriddiffcubic(x, y, n, boundltype, boundl, boundrtype, boundr, d)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub spline1dgriddiffcubic(x() As Double, y() As Double, ByRef d() As Double)
        Try
            alglib.spline1dgriddiffcubic(x, y, d)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub spline1dgriddiff2cubic(x() As Double, y() As Double, n As Integer, boundltype As Integer, boundl As Double, boundrtype As Integer, boundr As Double, ByRef d1() As Double, ByRef d2() As Double)
        Try
            alglib.spline1dgriddiff2cubic(x, y, n, boundltype, boundl, boundrtype, boundr, d1, d2)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub spline1dgriddiff2cubic(x() As Double, y() As Double, ByRef d1() As Double, ByRef d2() As Double)
        Try
            alglib.spline1dgriddiff2cubic(x, y, d1, d2)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub spline1dconvcubic(x() As Double, y() As Double, n As Integer, boundltype As Integer, boundl As Double, boundrtype As Integer, boundr As Double, x2() As Double, n2 As Integer, ByRef y2() As Double)
        Try
            alglib.spline1dconvcubic(x, y, n, boundltype, boundl, boundrtype, boundr, x2, n2, y2)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub spline1dconvcubic(x() As Double, y() As Double, x2() As Double, ByRef y2() As Double)
        Try
            alglib.spline1dconvcubic(x, y, x2, y2)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub spline1dconvdiffcubic(x() As Double, y() As Double, n As Integer, boundltype As Integer, boundl As Double, boundrtype As Integer, boundr As Double, x2() As Double, n2 As Integer, ByRef y2() As Double, ByRef d2() As Double)
        Try
            alglib.spline1dconvdiffcubic(x, y, n, boundltype, boundl, boundrtype, boundr, x2, n2, y2, d2)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub spline1dconvdiffcubic(x() As Double, y() As Double, x2() As Double, ByRef y2() As Double, ByRef d2() As Double)
        Try
            alglib.spline1dconvdiffcubic(x, y, x2, y2, d2)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub spline1dconvdiff2cubic(x() As Double, y() As Double, n As Integer, boundltype As Integer, boundl As Double, boundrtype As Integer, boundr As Double, x2() As Double, n2 As Integer, ByRef y2() As Double, ByRef d2() As Double, ByRef dd2() As Double)
        Try
            alglib.spline1dconvdiff2cubic(x, y, n, boundltype, boundl, boundrtype, boundr, x2, n2, y2, d2, dd2)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub spline1dconvdiff2cubic(x() As Double, y() As Double, x2() As Double, ByRef y2() As Double, ByRef d2() As Double, ByRef dd2() As Double)
        Try
            alglib.spline1dconvdiff2cubic(x, y, x2, y2, d2, dd2)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub spline1dbuildcatmullrom(x() As Double, y() As Double, n As Integer, boundtype As Integer, tension As Double, ByRef c As spline1dinterpolant)
        Try
            c = New spline1dinterpolant()
            alglib.spline1dbuildcatmullrom(x, y, n, boundtype, tension, c.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub spline1dbuildcatmullrom(x() As Double, y() As Double, ByRef c As spline1dinterpolant)
        Try
            c = New spline1dinterpolant()
            alglib.spline1dbuildcatmullrom(x, y, c.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub spline1dbuildhermite(x() As Double, y() As Double, d() As Double, n As Integer, ByRef c As spline1dinterpolant)
        Try
            c = New spline1dinterpolant()
            alglib.spline1dbuildhermite(x, y, d, n, c.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub spline1dbuildhermite(x() As Double, y() As Double, d() As Double, ByRef c As spline1dinterpolant)
        Try
            c = New spline1dinterpolant()
            alglib.spline1dbuildhermite(x, y, d, c.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub spline1dbuildakima(x() As Double, y() As Double, n As Integer, ByRef c As spline1dinterpolant)
        Try
            c = New spline1dinterpolant()
            alglib.spline1dbuildakima(x, y, n, c.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub spline1dbuildakima(x() As Double, y() As Double, ByRef c As spline1dinterpolant)
        Try
            c = New spline1dinterpolant()
            alglib.spline1dbuildakima(x, y, c.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Function spline1dcalc(c As spline1dinterpolant, x As Double) As Double
        Try
            spline1dcalc = alglib.spline1dcalc(c.csobj, x)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Function


    Public Sub spline1ddiff(c As spline1dinterpolant, x As Double, ByRef s As Double, ByRef ds As Double, ByRef d2s As Double)
        Try
            alglib.spline1ddiff(c.csobj, x, s, ds, d2s)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub spline1dunpack(c As spline1dinterpolant, ByRef n As Integer, ByRef tbl(,) As Double)
        Try
            alglib.spline1dunpack(c.csobj, n, tbl)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub spline1dlintransx(c As spline1dinterpolant, a As Double, b As Double)
        Try
            alglib.spline1dlintransx(c.csobj, a, b)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub spline1dlintransy(c As spline1dinterpolant, a As Double, b As Double)
        Try
            alglib.spline1dlintransy(c.csobj, a, b)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Function spline1dintegrate(c As spline1dinterpolant, x As Double) As Double
        Try
            spline1dintegrate = alglib.spline1dintegrate(c.csobj, x)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Function


    Public Sub spline1dbuildmonotone(x() As Double, y() As Double, n As Integer, ByRef c As spline1dinterpolant)
        Try
            c = New spline1dinterpolant()
            alglib.spline1dbuildmonotone(x, y, n, c.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub spline1dbuildmonotone(x() As Double, y() As Double, ByRef c As spline1dinterpolant)
        Try
            c = New spline1dinterpolant()
            alglib.spline1dbuildmonotone(x, y, c.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub

    Public Class sparsematrix
        Public csobj As alglib.sparsematrix
    End Class


    Public Sub sparsecreate(m As Integer, n As Integer, k As Integer, ByRef s As sparsematrix)
        Try
            s = New sparsematrix()
            alglib.sparsecreate(m, n, k, s.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub sparsecreate(m As Integer, n As Integer, ByRef s As sparsematrix)
        Try
            s = New sparsematrix()
            alglib.sparsecreate(m, n, s.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub sparsecreatecrs(m As Integer, n As Integer, ner() As Integer, ByRef s As sparsematrix)
        Try
            s = New sparsematrix()
            alglib.sparsecreatecrs(m, n, ner, s.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub sparsecopy(s0 As sparsematrix, ByRef s1 As sparsematrix)
        Try
            s1 = New sparsematrix()
            alglib.sparsecopy(s0.csobj, s1.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub sparseadd(s As sparsematrix, i As Integer, j As Integer, v As Double)
        Try
            alglib.sparseadd(s.csobj, i, j, v)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub sparseset(s As sparsematrix, i As Integer, j As Integer, v As Double)
        Try
            alglib.sparseset(s.csobj, i, j, v)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Function sparseget(s As sparsematrix, i As Integer, j As Integer) As Double
        Try
            sparseget = alglib.sparseget(s.csobj, i, j)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Function


    Public Sub sparseconverttocrs(s As sparsematrix)
        Try
            alglib.sparseconverttocrs(s.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub sparsemv(s As sparsematrix, x() As Double, ByRef y() As Double)
        Try
            alglib.sparsemv(s.csobj, x, y)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub sparsemtv(s As sparsematrix, x() As Double, ByRef y() As Double)
        Try
            alglib.sparsemtv(s.csobj, x, y)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub sparsemv2(s As sparsematrix, x() As Double, ByRef y0() As Double, ByRef y1() As Double)
        Try
            alglib.sparsemv2(s.csobj, x, y0, y1)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub sparsesmv(s As sparsematrix, isupper As Boolean, x() As Double, ByRef y() As Double)
        Try
            alglib.sparsesmv(s.csobj, isupper, x, y)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub sparsemm(s As sparsematrix, a(,) As Double, k As Integer, ByRef b(,) As Double)
        Try
            alglib.sparsemm(s.csobj, a, k, b)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub sparsemtm(s As sparsematrix, a(,) As Double, k As Integer, ByRef b(,) As Double)
        Try
            alglib.sparsemtm(s.csobj, a, k, b)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub sparsemm2(s As sparsematrix, a(,) As Double, k As Integer, ByRef b0(,) As Double, ByRef b1(,) As Double)
        Try
            alglib.sparsemm2(s.csobj, a, k, b0, b1)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub sparsesmm(s As sparsematrix, isupper As Boolean, a(,) As Double, k As Integer, ByRef b(,) As Double)
        Try
            alglib.sparsesmm(s.csobj, isupper, a, k, b)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub sparseresizematrix(s As sparsematrix)
        Try
            alglib.sparseresizematrix(s.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Function sparseenumerate(s As sparsematrix, ByRef t0 As Integer, ByRef t1 As Integer, ByRef i As Integer, ByRef j As Integer, ByRef v As Double) As Boolean
        Try
            sparseenumerate = alglib.sparseenumerate(s.csobj, t0, t1, i, j, v)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Function


    Public Function sparserewriteexisting(s As sparsematrix, i As Integer, j As Integer, v As Double) As Boolean
        Try
            sparserewriteexisting = alglib.sparserewriteexisting(s.csobj, i, j, v)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Function

    Public Class normestimatorstate
        Public csobj As alglib.normestimatorstate
    End Class


    Public Sub normestimatorcreate(m As Integer, n As Integer, nstart As Integer, nits As Integer, ByRef state As normestimatorstate)
        Try
            state = New normestimatorstate()
            alglib.normestimatorcreate(m, n, nstart, nits, state.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub normestimatorsetseed(state As normestimatorstate, seedval As Integer)
        Try
            alglib.normestimatorsetseed(state.csobj, seedval)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub normestimatorestimatesparse(state As normestimatorstate, a As sparsematrix)
        Try
            alglib.normestimatorestimatesparse(state.csobj, a.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub normestimatorresults(state As normestimatorstate, ByRef nrm As Double)
        Try
            alglib.normestimatorresults(state.csobj, nrm)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub




    Public Class minqpstate
        Public csobj As alglib.minqpstate
    End Class
    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    'This structure stores optimization report:
    '* InnerIterationsCount      number of inner iterations
    '* OuterIterationsCount      number of outer iterations
    '* NCholesky                 number of Cholesky decomposition
    '* NMV                       number of matrix-vector products
    '                            (only products calculated as part of iterative
    '                            process are counted)
    '* TerminationType           completion code (see below)
    '
    'Completion codes:
    '* -5    inappropriate solver was used:
    '        * Cholesky solver for semidefinite or indefinite problems
    '        * Cholesky solver for problems with non-boundary constraints
    '* -3    inconsistent constraints (or, maybe, feasible point is
    '        too hard to find). If you are sure that constraints are feasible,
    '        try to restart optimizer with better initial approximation.
    '* -1    solver error
    '*  4    successful completion
    '*  5    MaxIts steps was taken
    '*  7    stopping conditions are too stringent,
    '        further improvement is impossible,
    '        X contains best point found so far.
    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    Public Class minqpreport
        Public Property inneriterationscount() As Integer
            Get
                Return Me.csobj.inneriterationscount
            End Get
            Set(Value As Integer)
                Me.csobj.inneriterationscount = Value
            End Set
        End Property
        Public Property outeriterationscount() As Integer
            Get
                Return Me.csobj.outeriterationscount
            End Get
            Set(Value As Integer)
                Me.csobj.outeriterationscount = Value
            End Set
        End Property
        Public Property nmv() As Integer
            Get
                Return Me.csobj.nmv
            End Get
            Set(Value As Integer)
                Me.csobj.nmv = Value
            End Set
        End Property
        Public Property ncholesky() As Integer
            Get
                Return Me.csobj.ncholesky
            End Get
            Set(Value As Integer)
                Me.csobj.ncholesky = Value
            End Set
        End Property
        Public Property terminationtype() As Integer
            Get
                Return Me.csobj.terminationtype
            End Get
            Set(Value As Integer)
                Me.csobj.terminationtype = Value
            End Set
        End Property
        Public csobj As alglib.minqpreport
    End Class


    Public Sub minqpcreate(n As Integer, ByRef state As minqpstate)
        Try
            state = New minqpstate()
            alglib.minqpcreate(n, state.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub minqpsetlinearterm(state As minqpstate, b() As Double)
        Try
            alglib.minqpsetlinearterm(state.csobj, b)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub minqpsetquadraticterm(state As minqpstate, a(,) As Double, isupper As Boolean)
        Try
            alglib.minqpsetquadraticterm(state.csobj, a, isupper)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub minqpsetquadraticterm(state As minqpstate, a(,) As Double)
        Try
            alglib.minqpsetquadraticterm(state.csobj, a)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub minqpsetstartingpoint(state As minqpstate, x() As Double)
        Try
            alglib.minqpsetstartingpoint(state.csobj, x)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub minqpsetorigin(state As minqpstate, xorigin() As Double)
        Try
            alglib.minqpsetorigin(state.csobj, xorigin)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub minqpsetalgocholesky(state As minqpstate)
        Try
            alglib.minqpsetalgocholesky(state.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub minqpsetbc(state As minqpstate, bndl() As Double, bndu() As Double)
        Try
            alglib.minqpsetbc(state.csobj, bndl, bndu)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub minqpsetlc(state As minqpstate, c(,) As Double, ct() As Integer, k As Integer)
        Try
            alglib.minqpsetlc(state.csobj, c, ct, k)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub minqpsetlc(state As minqpstate, c(,) As Double, ct() As Integer)
        Try
            alglib.minqpsetlc(state.csobj, c, ct)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub minqpoptimize(state As minqpstate)
        Try
            alglib.minqpoptimize(state.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub minqpresults(state As minqpstate, ByRef x() As Double, ByRef rep As minqpreport)
        Try
            rep = New minqpreport()
            alglib.minqpresults(state.csobj, x, rep.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub minqpresultsbuf(state As minqpstate, ByRef x() As Double, ByRef rep As minqpreport)
        Try
            alglib.minqpresultsbuf(state.csobj, x, rep.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub

    Public Class minlmstate
        Public csobj As alglib.minlmstate
    End Class
    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    'Optimization report, filled by MinLMResults() function
    '
    'FIELDS:
    '* TerminationType, completetion code:
    '    * -7    derivative correctness check failed;
    '            see Rep.WrongNum, Rep.WrongI, Rep.WrongJ for
    '            more information.
    '    *  1    relative function improvement is no more than
    '            EpsF.
    '    *  2    relative step is no more than EpsX.
    '    *  4    gradient is no more than EpsG.
    '    *  5    MaxIts steps was taken
    '    *  7    stopping conditions are too stringent,
    '            further improvement is impossible
    '* IterationsCount, contains iterations count
    '* NFunc, number of function calculations
    '* NJac, number of Jacobi matrix calculations
    '* NGrad, number of gradient calculations
    '* NHess, number of Hessian calculations
    '* NCholesky, number of Cholesky decomposition calculations
    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    Public Class minlmreport
        Public Property iterationscount() As Integer
            Get
                Return Me.csobj.iterationscount
            End Get
            Set(Value As Integer)
                Me.csobj.iterationscount = Value
            End Set
        End Property
        Public Property terminationtype() As Integer
            Get
                Return Me.csobj.terminationtype
            End Get
            Set(Value As Integer)
                Me.csobj.terminationtype = Value
            End Set
        End Property
        Public Property funcidx() As Integer
            Get
                Return Me.csobj.funcidx
            End Get
            Set(Value As Integer)
                Me.csobj.funcidx = Value
            End Set
        End Property
        Public Property varidx() As Integer
            Get
                Return Me.csobj.varidx
            End Get
            Set(Value As Integer)
                Me.csobj.varidx = Value
            End Set
        End Property
        Public Property nfunc() As Integer
            Get
                Return Me.csobj.nfunc
            End Get
            Set(Value As Integer)
                Me.csobj.nfunc = Value
            End Set
        End Property
        Public Property njac() As Integer
            Get
                Return Me.csobj.njac
            End Get
            Set(Value As Integer)
                Me.csobj.njac = Value
            End Set
        End Property
        Public Property ngrad() As Integer
            Get
                Return Me.csobj.ngrad
            End Get
            Set(Value As Integer)
                Me.csobj.ngrad = Value
            End Set
        End Property
        Public Property nhess() As Integer
            Get
                Return Me.csobj.nhess
            End Get
            Set(Value As Integer)
                Me.csobj.nhess = Value
            End Set
        End Property
        Public Property ncholesky() As Integer
            Get
                Return Me.csobj.ncholesky
            End Get
            Set(Value As Integer)
                Me.csobj.ncholesky = Value
            End Set
        End Property
        Public csobj As alglib.minlmreport
    End Class


    Public Sub minlmcreatevj(n As Integer, m As Integer, x() As Double, ByRef state As minlmstate)
        Try
            state = New minlmstate()
            alglib.minlmcreatevj(n, m, x, state.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub minlmcreatevj(m As Integer, x() As Double, ByRef state As minlmstate)
        Try
            state = New minlmstate()
            alglib.minlmcreatevj(m, x, state.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub minlmcreatev(n As Integer, m As Integer, x() As Double, diffstep As Double, ByRef state As minlmstate)
        Try
            state = New minlmstate()
            alglib.minlmcreatev(n, m, x, diffstep, state.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub minlmcreatev(m As Integer, x() As Double, diffstep As Double, ByRef state As minlmstate)
        Try
            state = New minlmstate()
            alglib.minlmcreatev(m, x, diffstep, state.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub minlmcreatefgh(n As Integer, x() As Double, ByRef state As minlmstate)
        Try
            state = New minlmstate()
            alglib.minlmcreatefgh(n, x, state.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub minlmcreatefgh(x() As Double, ByRef state As minlmstate)
        Try
            state = New minlmstate()
            alglib.minlmcreatefgh(x, state.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub minlmsetcond(state As minlmstate, epsg As Double, epsf As Double, epsx As Double, maxits As Integer)
        Try
            alglib.minlmsetcond(state.csobj, epsg, epsf, epsx, maxits)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub minlmsetxrep(state As minlmstate, needxrep As Boolean)
        Try
            alglib.minlmsetxrep(state.csobj, needxrep)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub minlmsetstpmax(state As minlmstate, stpmax As Double)
        Try
            alglib.minlmsetstpmax(state.csobj, stpmax)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub minlmsetscale(state As minlmstate, s() As Double)
        Try
            alglib.minlmsetscale(state.csobj, s)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub minlmsetbc(state As minlmstate, bndl() As Double, bndu() As Double)
        Try
            alglib.minlmsetbc(state.csobj, bndl, bndu)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub minlmsetacctype(state As minlmstate, acctype As Integer)
        Try
            alglib.minlmsetacctype(state.csobj, acctype)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Function minlmiteration(state As minlmstate) As Boolean
        Try
            minlmiteration = alglib.minlmiteration(state.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Function
    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    ' This family of functions is used to launcn iterations of nonlinear optimizer
    ' 
    ' These functions accept following parameters:
    '     func    -   callback which calculates function (or merit function)
    '                 value func at given point x
    '     grad    -   callback which calculates function (or merit function)
    '                 value func and gradient grad at given point x
    '     hess    -   callback which calculates function (or merit function)
    '                 value func, gradient grad and Hessian hess at given point x
    '     fvec    -   callback which calculates function vector fi[]
    '                 at given point x
    '     jac     -   callback which calculates function vector fi[]
    '                 and Jacobian jac at given point x
    '     rep     -   optional callback which is called after each iteration
    '                 can be null
    '     obj     -   optional object which is passed to func/grad/hess/jac/rep
    '                 can be null
    ' 
    ' 
    ' NOTES:
    ' 
    ' 1. Depending on function used to create state  structure,  this  algorithm
    '    may accept Jacobian and/or Hessian and/or gradient.  According  to  the
    '    said above, there ase several versions of this function,  which  accept
    '    different sets of callbacks.
    ' 
    '    This flexibility opens way to subtle errors - you may create state with
    '    MinLMCreateFGH() (optimization using Hessian), but call function  which
    '    does not accept Hessian. So when algorithm will request Hessian,  there
    '    will be no callback to call. In this case exception will be thrown.
    ' 
    '    Be careful to avoid such errors because there is no way to find them at
    '    compile time - you can see them at runtime only.
    ' 
    '   -- ALGLIB --
    '      Copyright 10.03.2009 by Bochkanov Sergey
    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    Public Sub minlmoptimize(state As minlmstate, fvec As ndimensional_fvec, rep As ndimensional_rep, obj As Object)
        Dim innerobj As alglib.minlm.minlmstate = state.csobj.innerobj
        If fvec Is Nothing Then
            Throw New AlglibException("ALGLIB: error in 'minlmoptimize()' (fvec is null)")
        End If
        Try
            While alglib.minlm.minlmiteration(innerobj)
                If innerobj.needfi Then
                    fvec(innerobj.x, innerobj.fi, obj)
                    Continue While
                End If
                If innerobj.xupdated Then
                    If rep IsNot Nothing Then
                        rep(innerobj.x, innerobj.f, obj)
                    End If
                    Continue While
                End If
                Throw New AlglibException("ALGLIB: error in 'minlmoptimize' (some derivatives were not provided?)")
            End While
        Catch E As alglib.alglibexception
            Throw New AlglibException(E.msg)
        End Try
    End Sub


    Public Sub minlmoptimize(state As minlmstate, fvec As ndimensional_fvec, jac As ndimensional_jac, rep As ndimensional_rep, obj As Object)
        Dim innerobj As alglib.minlm.minlmstate = state.csobj.innerobj
        If fvec Is Nothing Then
            Throw New AlglibException("ALGLIB: error in 'minlmoptimize()' (fvec is null)")
        End If
        If jac Is Nothing Then
            Throw New AlglibException("ALGLIB: error in 'minlmoptimize()' (jac is null)")
        End If
        Try
            While alglib.minlm.minlmiteration(innerobj)
                If innerobj.needfi Then
                    fvec(innerobj.x, innerobj.fi, obj)
                    Continue While
                End If
                If innerobj.needfij Then
                    jac(innerobj.x, innerobj.fi, innerobj.j, obj)
                    Continue While
                End If
                If innerobj.xupdated Then
                    If rep IsNot Nothing Then
                        rep(innerobj.x, innerobj.f, obj)
                    End If
                    Continue While
                End If
                Throw New AlglibException("ALGLIB: error in 'minlmoptimize' (some derivatives were not provided?)")
            End While
        Catch E As alglib.alglibexception
            Throw New AlglibException(E.msg)
        End Try
    End Sub


    Public Sub minlmoptimize(state As minlmstate, func As ndimensional_func, grad As ndimensional_grad, hess As ndimensional_hess, rep As ndimensional_rep, obj As Object)
        Dim innerobj As alglib.minlm.minlmstate = state.csobj.innerobj
        If func Is Nothing Then
            Throw New AlglibException("ALGLIB: error in 'minlmoptimize()' (func is null)")
        End If
        If grad Is Nothing Then
            Throw New AlglibException("ALGLIB: error in 'minlmoptimize()' (grad is null)")
        End If
        If hess Is Nothing Then
            Throw New AlglibException("ALGLIB: error in 'minlmoptimize()' (hess is null)")
        End If
        Try
            While alglib.minlm.minlmiteration(innerobj)
                If innerobj.needf Then
                    func(innerobj.x, innerobj.f, obj)
                    Continue While
                End If
                If innerobj.needfg Then
                    grad(innerobj.x, innerobj.f, innerobj.g, obj)
                    Continue While
                End If
                If innerobj.needfgh Then
                    hess(innerobj.x, innerobj.f, innerobj.g, innerobj.h, obj)
                    Continue While
                End If
                If innerobj.xupdated Then
                    If rep IsNot Nothing Then
                        rep(innerobj.x, innerobj.f, obj)
                    End If
                    Continue While
                End If
                Throw New AlglibException("ALGLIB: error in 'minlmoptimize' (some derivatives were not provided?)")
            End While
        Catch E As alglib.alglibexception
            Throw New AlglibException(E.msg)
        End Try
    End Sub


    Public Sub minlmoptimize(state As minlmstate, func As ndimensional_func, jac As ndimensional_jac, rep As ndimensional_rep, obj As Object)
        Dim innerobj As alglib.minlm.minlmstate = state.csobj.innerobj
        If func Is Nothing Then
            Throw New AlglibException("ALGLIB: error in 'minlmoptimize()' (func is null)")
        End If
        If jac Is Nothing Then
            Throw New AlglibException("ALGLIB: error in 'minlmoptimize()' (jac is null)")
        End If
        Try
            While alglib.minlm.minlmiteration(innerobj)
                If innerobj.needf Then
                    func(innerobj.x, innerobj.f, obj)
                    Continue While
                End If
                If innerobj.needfij Then
                    jac(innerobj.x, innerobj.fi, innerobj.j, obj)
                    Continue While
                End If
                If innerobj.xupdated Then
                    If rep IsNot Nothing Then
                        rep(innerobj.x, innerobj.f, obj)
                    End If
                    Continue While
                End If
                Throw New AlglibException("ALGLIB: error in 'minlmoptimize' (some derivatives were not provided?)")
            End While
        Catch E As alglib.alglibexception
            Throw New AlglibException(E.msg)
        End Try
    End Sub


    Public Sub minlmoptimize(state As minlmstate, func As ndimensional_func, grad As ndimensional_grad, jac As ndimensional_jac, rep As ndimensional_rep, obj As Object)
        Dim innerobj As alglib.minlm.minlmstate = state.csobj.innerobj
        If func Is Nothing Then
            Throw New AlglibException("ALGLIB: error in 'minlmoptimize()' (func is null)")
        End If
        If grad Is Nothing Then
            Throw New AlglibException("ALGLIB: error in 'minlmoptimize()' (grad is null)")
        End If
        If jac Is Nothing Then
            Throw New AlglibException("ALGLIB: error in 'minlmoptimize()' (jac is null)")
        End If
        Try
            While alglib.minlm.minlmiteration(innerobj)
                If innerobj.needf Then
                    func(innerobj.x, innerobj.f, obj)
                    Continue While
                End If
                If innerobj.needfg Then
                    grad(innerobj.x, innerobj.f, innerobj.g, obj)
                    Continue While
                End If
                If innerobj.needfij Then
                    jac(innerobj.x, innerobj.fi, innerobj.j, obj)
                    Continue While
                End If
                If innerobj.xupdated Then
                    If rep IsNot Nothing Then
                        rep(innerobj.x, innerobj.f, obj)
                    End If
                    Continue While
                End If
                Throw New AlglibException("ALGLIB: error in 'minlmoptimize' (some derivatives were not provided?)")
            End While
        Catch E As alglib.alglibexception
            Throw New AlglibException(E.msg)
        End Try
    End Sub




    Public Sub minlmresults(state As minlmstate, ByRef x() As Double, ByRef rep As minlmreport)
        Try
            rep = New minlmreport()
            alglib.minlmresults(state.csobj, x, rep.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub minlmresultsbuf(state As minlmstate, ByRef x() As Double, ByRef rep As minlmreport)
        Try
            alglib.minlmresultsbuf(state.csobj, x, rep.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub minlmrestartfrom(state As minlmstate, x() As Double)
        Try
            alglib.minlmrestartfrom(state.csobj, x)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub minlmcreatevgj(n As Integer, m As Integer, x() As Double, ByRef state As minlmstate)
        Try
            state = New minlmstate()
            alglib.minlmcreatevgj(n, m, x, state.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub minlmcreatevgj(m As Integer, x() As Double, ByRef state As minlmstate)
        Try
            state = New minlmstate()
            alglib.minlmcreatevgj(m, x, state.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub minlmcreatefgj(n As Integer, m As Integer, x() As Double, ByRef state As minlmstate)
        Try
            state = New minlmstate()
            alglib.minlmcreatefgj(n, m, x, state.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub minlmcreatefgj(m As Integer, x() As Double, ByRef state As minlmstate)
        Try
            state = New minlmstate()
            alglib.minlmcreatefgj(m, x, state.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub minlmcreatefj(n As Integer, m As Integer, x() As Double, ByRef state As minlmstate)
        Try
            state = New minlmstate()
            alglib.minlmcreatefj(n, m, x, state.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub minlmcreatefj(m As Integer, x() As Double, ByRef state As minlmstate)
        Try
            state = New minlmstate()
            alglib.minlmcreatefj(m, x, state.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub minlmsetgradientcheck(state As minlmstate, teststep As Double)
        Try
            alglib.minlmsetgradientcheck(state.csobj, teststep)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub

    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    'Polynomial fitting report:
    '    TaskRCond       reciprocal of task's condition number
    '    RMSError        RMS error
    '    AvgError        average error
    '    AvgRelError     average relative error (for non-zero Y[I])
    '    MaxError        maximum error
    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    Public Class polynomialfitreport
        Public Property taskrcond() As Double
            Get
                Return Me.csobj.taskrcond
            End Get
            Set(Value As Double)
                Me.csobj.taskrcond = Value
            End Set
        End Property
        Public Property rmserror() As Double
            Get
                Return Me.csobj.rmserror
            End Get
            Set(Value As Double)
                Me.csobj.rmserror = Value
            End Set
        End Property
        Public Property avgerror() As Double
            Get
                Return Me.csobj.avgerror
            End Get
            Set(Value As Double)
                Me.csobj.avgerror = Value
            End Set
        End Property
        Public Property avgrelerror() As Double
            Get
                Return Me.csobj.avgrelerror
            End Get
            Set(Value As Double)
                Me.csobj.avgrelerror = Value
            End Set
        End Property
        Public Property maxerror() As Double
            Get
                Return Me.csobj.maxerror
            End Get
            Set(Value As Double)
                Me.csobj.maxerror = Value
            End Set
        End Property
        Public csobj As alglib.polynomialfitreport
    End Class
    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    'Barycentric fitting report:
    '    RMSError        RMS error
    '    AvgError        average error
    '    AvgRelError     average relative error (for non-zero Y[I])
    '    MaxError        maximum error
    '    TaskRCond       reciprocal of task's condition number
    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    Public Class barycentricfitreport
        Public Property taskrcond() As Double
            Get
                Return Me.csobj.taskrcond
            End Get
            Set(Value As Double)
                Me.csobj.taskrcond = Value
            End Set
        End Property
        Public Property dbest() As Integer
            Get
                Return Me.csobj.dbest
            End Get
            Set(Value As Integer)
                Me.csobj.dbest = Value
            End Set
        End Property
        Public Property rmserror() As Double
            Get
                Return Me.csobj.rmserror
            End Get
            Set(Value As Double)
                Me.csobj.rmserror = Value
            End Set
        End Property
        Public Property avgerror() As Double
            Get
                Return Me.csobj.avgerror
            End Get
            Set(Value As Double)
                Me.csobj.avgerror = Value
            End Set
        End Property
        Public Property avgrelerror() As Double
            Get
                Return Me.csobj.avgrelerror
            End Get
            Set(Value As Double)
                Me.csobj.avgrelerror = Value
            End Set
        End Property
        Public Property maxerror() As Double
            Get
                Return Me.csobj.maxerror
            End Get
            Set(Value As Double)
                Me.csobj.maxerror = Value
            End Set
        End Property
        Public csobj As alglib.barycentricfitreport
    End Class
    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    'Spline fitting report:
    '    RMSError        RMS error
    '    AvgError        average error
    '    AvgRelError     average relative error (for non-zero Y[I])
    '    MaxError        maximum error
    '
    'Fields  below are  filled  by   obsolete    functions   (Spline1DFitCubic,
    'Spline1DFitHermite). Modern fitting functions do NOT fill these fields:
    '    TaskRCond       reciprocal of task's condition number
    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    Public Class spline1dfitreport
        Public Property taskrcond() As Double
            Get
                Return Me.csobj.taskrcond
            End Get
            Set(Value As Double)
                Me.csobj.taskrcond = Value
            End Set
        End Property
        Public Property rmserror() As Double
            Get
                Return Me.csobj.rmserror
            End Get
            Set(Value As Double)
                Me.csobj.rmserror = Value
            End Set
        End Property
        Public Property avgerror() As Double
            Get
                Return Me.csobj.avgerror
            End Get
            Set(Value As Double)
                Me.csobj.avgerror = Value
            End Set
        End Property
        Public Property avgrelerror() As Double
            Get
                Return Me.csobj.avgrelerror
            End Get
            Set(Value As Double)
                Me.csobj.avgrelerror = Value
            End Set
        End Property
        Public Property maxerror() As Double
            Get
                Return Me.csobj.maxerror
            End Get
            Set(Value As Double)
                Me.csobj.maxerror = Value
            End Set
        End Property
        Public csobj As alglib.spline1dfitreport
    End Class
    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    'Least squares fitting report:
    '    TaskRCond       reciprocal of task's condition number
    '    IterationsCount number of internal iterations
    '
    '    RMSError        RMS error
    '    AvgError        average error
    '    AvgRelError     average relative error (for non-zero Y[I])
    '    MaxError        maximum error
    '
    '    WRMSError       weighted RMS error
    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    Public Class lsfitreport
        Public Property taskrcond() As Double
            Get
                Return Me.csobj.taskrcond
            End Get
            Set(Value As Double)
                Me.csobj.taskrcond = Value
            End Set
        End Property
        Public Property iterationscount() As Integer
            Get
                Return Me.csobj.iterationscount
            End Get
            Set(Value As Integer)
                Me.csobj.iterationscount = Value
            End Set
        End Property
        Public Property varidx() As Integer
            Get
                Return Me.csobj.varidx
            End Get
            Set(Value As Integer)
                Me.csobj.varidx = Value
            End Set
        End Property
        Public Property rmserror() As Double
            Get
                Return Me.csobj.rmserror
            End Get
            Set(Value As Double)
                Me.csobj.rmserror = Value
            End Set
        End Property
        Public Property avgerror() As Double
            Get
                Return Me.csobj.avgerror
            End Get
            Set(Value As Double)
                Me.csobj.avgerror = Value
            End Set
        End Property
        Public Property avgrelerror() As Double
            Get
                Return Me.csobj.avgrelerror
            End Get
            Set(Value As Double)
                Me.csobj.avgrelerror = Value
            End Set
        End Property
        Public Property maxerror() As Double
            Get
                Return Me.csobj.maxerror
            End Get
            Set(Value As Double)
                Me.csobj.maxerror = Value
            End Set
        End Property
        Public Property wrmserror() As Double
            Get
                Return Me.csobj.wrmserror
            End Get
            Set(Value As Double)
                Me.csobj.wrmserror = Value
            End Set
        End Property
        Public csobj As alglib.lsfitreport
    End Class
    Public Class lsfitstate
        Public csobj As alglib.lsfitstate
    End Class


    Public Sub polynomialfit(x() As Double, y() As Double, n As Integer, m As Integer, ByRef info As Integer, ByRef p As barycentricinterpolant, ByRef rep As polynomialfitreport)
        Try
            p = New barycentricinterpolant()
            rep = New polynomialfitreport()
            alglib.polynomialfit(x, y, n, m, info, p.csobj, rep.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub polynomialfit(x() As Double, y() As Double, m As Integer, ByRef info As Integer, ByRef p As barycentricinterpolant, ByRef rep As polynomialfitreport)
        Try
            p = New barycentricinterpolant()
            rep = New polynomialfitreport()
            alglib.polynomialfit(x, y, m, info, p.csobj, rep.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub polynomialfitwc(x() As Double, y() As Double, w() As Double, n As Integer, xc() As Double, yc() As Double, dc() As Integer, k As Integer, m As Integer, ByRef info As Integer, ByRef p As barycentricinterpolant, ByRef rep As polynomialfitreport)
        Try
            p = New barycentricinterpolant()
            rep = New polynomialfitreport()
            alglib.polynomialfitwc(x, y, w, n, xc, yc, dc, k, m, info, p.csobj, rep.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub polynomialfitwc(x() As Double, y() As Double, w() As Double, xc() As Double, yc() As Double, dc() As Integer, m As Integer, ByRef info As Integer, ByRef p As barycentricinterpolant, ByRef rep As polynomialfitreport)
        Try
            p = New barycentricinterpolant()
            rep = New polynomialfitreport()
            alglib.polynomialfitwc(x, y, w, xc, yc, dc, m, info, p.csobj, rep.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub barycentricfitfloaterhormannwc(x() As Double, y() As Double, w() As Double, n As Integer, xc() As Double, yc() As Double, dc() As Integer, k As Integer, m As Integer, ByRef info As Integer, ByRef b As barycentricinterpolant, ByRef rep As barycentricfitreport)
        Try
            b = New barycentricinterpolant()
            rep = New barycentricfitreport()
            alglib.barycentricfitfloaterhormannwc(x, y, w, n, xc, yc, dc, k, m, info, b.csobj, rep.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub barycentricfitfloaterhormann(x() As Double, y() As Double, n As Integer, m As Integer, ByRef info As Integer, ByRef b As barycentricinterpolant, ByRef rep As barycentricfitreport)
        Try
            b = New barycentricinterpolant()
            rep = New barycentricfitreport()
            alglib.barycentricfitfloaterhormann(x, y, n, m, info, b.csobj, rep.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub spline1dfitpenalized(x() As Double, y() As Double, n As Integer, m As Integer, rho As Double, ByRef info As Integer, ByRef s As spline1dinterpolant, ByRef rep As spline1dfitreport)
        Try
            s = New spline1dinterpolant()
            rep = New spline1dfitreport()
            alglib.spline1dfitpenalized(x, y, n, m, rho, info, s.csobj, rep.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub spline1dfitpenalized(x() As Double, y() As Double, m As Integer, rho As Double, ByRef info As Integer, ByRef s As spline1dinterpolant, ByRef rep As spline1dfitreport)
        Try
            s = New spline1dinterpolant()
            rep = New spline1dfitreport()
            alglib.spline1dfitpenalized(x, y, m, rho, info, s.csobj, rep.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub spline1dfitpenalizedw(x() As Double, y() As Double, w() As Double, n As Integer, m As Integer, rho As Double, ByRef info As Integer, ByRef s As spline1dinterpolant, ByRef rep As spline1dfitreport)
        Try
            s = New spline1dinterpolant()
            rep = New spline1dfitreport()
            alglib.spline1dfitpenalizedw(x, y, w, n, m, rho, info, s.csobj, rep.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub spline1dfitpenalizedw(x() As Double, y() As Double, w() As Double, m As Integer, rho As Double, ByRef info As Integer, ByRef s As spline1dinterpolant, ByRef rep As spline1dfitreport)
        Try
            s = New spline1dinterpolant()
            rep = New spline1dfitreport()
            alglib.spline1dfitpenalizedw(x, y, w, m, rho, info, s.csobj, rep.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub spline1dfitcubicwc(x() As Double, y() As Double, w() As Double, n As Integer, xc() As Double, yc() As Double, dc() As Integer, k As Integer, m As Integer, ByRef info As Integer, ByRef s As spline1dinterpolant, ByRef rep As spline1dfitreport)
        Try
            s = New spline1dinterpolant()
            rep = New spline1dfitreport()
            alglib.spline1dfitcubicwc(x, y, w, n, xc, yc, dc, k, m, info, s.csobj, rep.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub spline1dfitcubicwc(x() As Double, y() As Double, w() As Double, xc() As Double, yc() As Double, dc() As Integer, m As Integer, ByRef info As Integer, ByRef s As spline1dinterpolant, ByRef rep As spline1dfitreport)
        Try
            s = New spline1dinterpolant()
            rep = New spline1dfitreport()
            alglib.spline1dfitcubicwc(x, y, w, xc, yc, dc, m, info, s.csobj, rep.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub spline1dfithermitewc(x() As Double, y() As Double, w() As Double, n As Integer, xc() As Double, yc() As Double, dc() As Integer, k As Integer, m As Integer, ByRef info As Integer, ByRef s As spline1dinterpolant, ByRef rep As spline1dfitreport)
        Try
            s = New spline1dinterpolant()
            rep = New spline1dfitreport()
            alglib.spline1dfithermitewc(x, y, w, n, xc, yc, dc, k, m, info, s.csobj, rep.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub spline1dfithermitewc(x() As Double, y() As Double, w() As Double, xc() As Double, yc() As Double, dc() As Integer, m As Integer, ByRef info As Integer, ByRef s As spline1dinterpolant, ByRef rep As spline1dfitreport)
        Try
            s = New spline1dinterpolant()
            rep = New spline1dfitreport()
            alglib.spline1dfithermitewc(x, y, w, xc, yc, dc, m, info, s.csobj, rep.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub spline1dfitcubic(x() As Double, y() As Double, n As Integer, m As Integer, ByRef info As Integer, ByRef s As spline1dinterpolant, ByRef rep As spline1dfitreport)
        Try
            s = New spline1dinterpolant()
            rep = New spline1dfitreport()
            alglib.spline1dfitcubic(x, y, n, m, info, s.csobj, rep.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub spline1dfitcubic(x() As Double, y() As Double, m As Integer, ByRef info As Integer, ByRef s As spline1dinterpolant, ByRef rep As spline1dfitreport)
        Try
            s = New spline1dinterpolant()
            rep = New spline1dfitreport()
            alglib.spline1dfitcubic(x, y, m, info, s.csobj, rep.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub spline1dfithermite(x() As Double, y() As Double, n As Integer, m As Integer, ByRef info As Integer, ByRef s As spline1dinterpolant, ByRef rep As spline1dfitreport)
        Try
            s = New spline1dinterpolant()
            rep = New spline1dfitreport()
            alglib.spline1dfithermite(x, y, n, m, info, s.csobj, rep.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub spline1dfithermite(x() As Double, y() As Double, m As Integer, ByRef info As Integer, ByRef s As spline1dinterpolant, ByRef rep As spline1dfitreport)
        Try
            s = New spline1dinterpolant()
            rep = New spline1dfitreport()
            alglib.spline1dfithermite(x, y, m, info, s.csobj, rep.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub lsfitlinearw(y() As Double, w() As Double, fmatrix(,) As Double, n As Integer, m As Integer, ByRef info As Integer, ByRef c() As Double, ByRef rep As lsfitreport)
        Try
            rep = New lsfitreport()
            alglib.lsfitlinearw(y, w, fmatrix, n, m, info, c, rep.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub lsfitlinearw(y() As Double, w() As Double, fmatrix(,) As Double, ByRef info As Integer, ByRef c() As Double, ByRef rep As lsfitreport)
        Try
            rep = New lsfitreport()
            alglib.lsfitlinearw(y, w, fmatrix, info, c, rep.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub lsfitlinearwc(y() As Double, w() As Double, fmatrix(,) As Double, cmatrix(,) As Double, n As Integer, m As Integer, k As Integer, ByRef info As Integer, ByRef c() As Double, ByRef rep As lsfitreport)
        Try
            rep = New lsfitreport()
            alglib.lsfitlinearwc(y, w, fmatrix, cmatrix, n, m, k, info, c, rep.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub lsfitlinearwc(y() As Double, w() As Double, fmatrix(,) As Double, cmatrix(,) As Double, ByRef info As Integer, ByRef c() As Double, ByRef rep As lsfitreport)
        Try
            rep = New lsfitreport()
            alglib.lsfitlinearwc(y, w, fmatrix, cmatrix, info, c, rep.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub lsfitlinear(y() As Double, fmatrix(,) As Double, n As Integer, m As Integer, ByRef info As Integer, ByRef c() As Double, ByRef rep As lsfitreport)
        Try
            rep = New lsfitreport()
            alglib.lsfitlinear(y, fmatrix, n, m, info, c, rep.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub lsfitlinear(y() As Double, fmatrix(,) As Double, ByRef info As Integer, ByRef c() As Double, ByRef rep As lsfitreport)
        Try
            rep = New lsfitreport()
            alglib.lsfitlinear(y, fmatrix, info, c, rep.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub lsfitlinearc(y() As Double, fmatrix(,) As Double, cmatrix(,) As Double, n As Integer, m As Integer, k As Integer, ByRef info As Integer, ByRef c() As Double, ByRef rep As lsfitreport)
        Try
            rep = New lsfitreport()
            alglib.lsfitlinearc(y, fmatrix, cmatrix, n, m, k, info, c, rep.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub lsfitlinearc(y() As Double, fmatrix(,) As Double, cmatrix(,) As Double, ByRef info As Integer, ByRef c() As Double, ByRef rep As lsfitreport)
        Try
            rep = New lsfitreport()
            alglib.lsfitlinearc(y, fmatrix, cmatrix, info, c, rep.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub lsfitcreatewf(x(,) As Double, y() As Double, w() As Double, c() As Double, n As Integer, m As Integer, k As Integer, diffstep As Double, ByRef state As lsfitstate)
        Try
            state = New lsfitstate()
            alglib.lsfitcreatewf(x, y, w, c, n, m, k, diffstep, state.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub lsfitcreatewf(x(,) As Double, y() As Double, w() As Double, c() As Double, diffstep As Double, ByRef state As lsfitstate)
        Try
            state = New lsfitstate()
            alglib.lsfitcreatewf(x, y, w, c, diffstep, state.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub lsfitcreatef(x(,) As Double, y() As Double, c() As Double, n As Integer, m As Integer, k As Integer, diffstep As Double, ByRef state As lsfitstate)
        Try
            state = New lsfitstate()
            alglib.lsfitcreatef(x, y, c, n, m, k, diffstep, state.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub lsfitcreatef(x(,) As Double, y() As Double, c() As Double, diffstep As Double, ByRef state As lsfitstate)
        Try
            state = New lsfitstate()
            alglib.lsfitcreatef(x, y, c, diffstep, state.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub lsfitcreatewfg(x(,) As Double, y() As Double, w() As Double, c() As Double, n As Integer, m As Integer, k As Integer, cheapfg As Boolean, ByRef state As lsfitstate)
        Try
            state = New lsfitstate()
            alglib.lsfitcreatewfg(x, y, w, c, n, m, k, cheapfg, state.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub lsfitcreatewfg(x(,) As Double, y() As Double, w() As Double, c() As Double, cheapfg As Boolean, ByRef state As lsfitstate)
        Try
            state = New lsfitstate()
            alglib.lsfitcreatewfg(x, y, w, c, cheapfg, state.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub lsfitcreatefg(x(,) As Double, y() As Double, c() As Double, n As Integer, m As Integer, k As Integer, cheapfg As Boolean, ByRef state As lsfitstate)
        Try
            state = New lsfitstate()
            alglib.lsfitcreatefg(x, y, c, n, m, k, cheapfg, state.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub lsfitcreatefg(x(,) As Double, y() As Double, c() As Double, cheapfg As Boolean, ByRef state As lsfitstate)
        Try
            state = New lsfitstate()
            alglib.lsfitcreatefg(x, y, c, cheapfg, state.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub lsfitcreatewfgh(x(,) As Double, y() As Double, w() As Double, c() As Double, n As Integer, m As Integer, k As Integer, ByRef state As lsfitstate)
        Try
            state = New lsfitstate()
            alglib.lsfitcreatewfgh(x, y, w, c, n, m, k, state.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub lsfitcreatewfgh(x(,) As Double, y() As Double, w() As Double, c() As Double, ByRef state As lsfitstate)
        Try
            state = New lsfitstate()
            alglib.lsfitcreatewfgh(x, y, w, c, state.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub lsfitcreatefgh(x(,) As Double, y() As Double, c() As Double, n As Integer, m As Integer, k As Integer, ByRef state As lsfitstate)
        Try
            state = New lsfitstate()
            alglib.lsfitcreatefgh(x, y, c, n, m, k, state.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub lsfitcreatefgh(x(,) As Double, y() As Double, c() As Double, ByRef state As lsfitstate)
        Try
            state = New lsfitstate()
            alglib.lsfitcreatefgh(x, y, c, state.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub lsfitsetcond(state As lsfitstate, epsf As Double, epsx As Double, maxits As Integer)
        Try
            alglib.lsfitsetcond(state.csobj, epsf, epsx, maxits)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub lsfitsetstpmax(state As lsfitstate, stpmax As Double)
        Try
            alglib.lsfitsetstpmax(state.csobj, stpmax)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub lsfitsetxrep(state As lsfitstate, needxrep As Boolean)
        Try
            alglib.lsfitsetxrep(state.csobj, needxrep)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub lsfitsetscale(state As lsfitstate, s() As Double)
        Try
            alglib.lsfitsetscale(state.csobj, s)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub lsfitsetbc(state As lsfitstate, bndl() As Double, bndu() As Double)
        Try
            alglib.lsfitsetbc(state.csobj, bndl, bndu)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Function lsfititeration(state As lsfitstate) As Boolean
        Try
            lsfititeration = alglib.lsfititeration(state.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Function
    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    ' This family of functions is used to launcn iterations of nonlinear fitter
    ' 
    ' These functions accept following parameters:
    '     func    -   callback which calculates function (or merit function)
    '                 value func at given point x
    '     grad    -   callback which calculates function (or merit function)
    '                 value func and gradient grad at given point x
    '     hess    -   callback which calculates function (or merit function)
    '                 value func, gradient grad and Hessian hess at given point x
    '     rep     -   optional callback which is called after each iteration
    '                 can be null
    '     obj     -   optional object which is passed to func/grad/hess/jac/rep
    '                 can be null
    ' 
    ' 
    ' NOTES:
    ' 
    ' 1. this algorithm is somewhat unusual because it works with  parameterized
    '    function f(C,X), where X is a function argument (we  have  many  points
    '    which are characterized by different  argument  values),  and  C  is  a
    '    parameter to fit.
    ' 
    '    For example, if we want to do linear fit by f(c0,c1,x) = c0*x+c1,  then
    '    x will be argument, and {c0,c1} will be parameters.
    ' 
    '    It is important to understand that this algorithm finds minimum in  the
    '    space of function PARAMETERS (not arguments), so it  needs  derivatives
    '    of f() with respect to C, not X.
    ' 
    '    In the example above it will need f=c0*x+c1 and {df/dc0,df/dc1} = {x,1}
    '    instead of {df/dx} = {c0}.
    ' 
    ' 2. Callback functions accept C as the first parameter, and X as the second
    ' 
    ' 3. If  state  was  created  with  LSFitCreateFG(),  algorithm  needs  just
    '    function   and   its   gradient,   but   if   state   was  created with
    '    LSFitCreateFGH(), algorithm will need function, gradient and Hessian.
    ' 
    '    According  to  the  said  above,  there  ase  several  versions of this
    '    function, which accept different sets of callbacks.
    ' 
    '    This flexibility opens way to subtle errors - you may create state with
    '    LSFitCreateFGH() (optimization using Hessian), but call function  which
    '    does not accept Hessian. So when algorithm will request Hessian,  there
    '    will be no callback to call. In this case exception will be thrown.
    ' 
    '    Be careful to avoid such errors because there is no way to find them at
    '    compile time - you can see them at runtime only.
    ' 
    '   -- ALGLIB --
    '      Copyright 17.08.2009 by Bochkanov Sergey
    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    Public Sub lsfitfit(state As lsfitstate, func As ndimensional_pfunc, rep As ndimensional_rep, obj As Object)
        Dim innerobj As alglib.lsfit.lsfitstate = state.csobj.innerobj
        If func Is Nothing Then
            Throw New AlglibException("ALGLIB: error in 'lsfitfit()' (func is null)")
        End If
        Try
            While alglib.lsfit.lsfititeration(innerobj)
                If innerobj.needf Then
                    func(innerobj.c, innerobj.x, innerobj.f, obj)
                    Continue While
                End If
                If innerobj.xupdated Then
                    If rep IsNot Nothing Then
                        rep(innerobj.c, innerobj.f, obj)
                    End If
                    Continue While
                End If
                Throw New AlglibException("ALGLIB: error in 'lsfitfit' (some derivatives were not provided?)")
            End While
        Catch E As alglib.alglibexception
            Throw New AlglibException(E.msg)
        End Try
    End Sub


    Public Sub lsfitfit(state As lsfitstate, func As ndimensional_pfunc, grad As ndimensional_pgrad, rep As ndimensional_rep, obj As Object)
        Dim innerobj As alglib.lsfit.lsfitstate = state.csobj.innerobj
        If func Is Nothing Then
            Throw New AlglibException("ALGLIB: error in 'lsfitfit()' (func is null)")
        End If
        If grad Is Nothing Then
            Throw New AlglibException("ALGLIB: error in 'lsfitfit()' (grad is null)")
        End If
        Try
            While alglib.lsfit.lsfititeration(innerobj)
                If innerobj.needf Then
                    func(innerobj.c, innerobj.x, innerobj.f, obj)
                    Continue While
                End If
                If innerobj.needfg Then
                    grad(innerobj.c, innerobj.x, innerobj.f, innerobj.g, obj)
                    Continue While
                End If
                If innerobj.xupdated Then
                    If rep IsNot Nothing Then
                        rep(innerobj.c, innerobj.f, obj)
                    End If
                    Continue While
                End If
                Throw New AlglibException("ALGLIB: error in 'lsfitfit' (some derivatives were not provided?)")
            End While
        Catch E As alglib.alglibexception
            Throw New AlglibException(E.msg)
        End Try
    End Sub


    Public Sub lsfitfit(state As lsfitstate, func As ndimensional_pfunc, grad As ndimensional_pgrad, hess As ndimensional_phess, rep As ndimensional_rep, obj As Object)
        Dim innerobj As alglib.lsfit.lsfitstate = state.csobj.innerobj
        If func Is Nothing Then
            Throw New AlglibException("ALGLIB: error in 'lsfitfit()' (func is null)")
        End If
        If grad Is Nothing Then
            Throw New AlglibException("ALGLIB: error in 'lsfitfit()' (grad is null)")
        End If
        If hess Is Nothing Then
            Throw New AlglibException("ALGLIB: error in 'lsfitfit()' (hess is null)")
        End If
        Try
            While alglib.lsfit.lsfititeration(innerobj)
                If innerobj.needf Then
                    func(innerobj.c, innerobj.x, innerobj.f, obj)
                    Continue While
                End If
                If innerobj.needfg Then
                    grad(innerobj.c, innerobj.x, innerobj.f, innerobj.g, obj)
                    Continue While
                End If
                If innerobj.needfgh Then
                    hess(innerobj.c, innerobj.x, innerobj.f, innerobj.g, innerobj.h, obj)
                    Continue While
                End If
                If innerobj.xupdated Then
                    If rep IsNot Nothing Then
                        rep(innerobj.c, innerobj.f, obj)
                    End If
                    Continue While
                End If
                Throw New AlglibException("ALGLIB: error in 'lsfitfit' (some derivatives were not provided?)")
            End While
        Catch E As alglib.alglibexception
            Throw New AlglibException(E.msg)
        End Try
    End Sub




    Public Sub lsfitresults(state As lsfitstate, ByRef info As Integer, ByRef c() As Double, ByRef rep As lsfitreport)
        Try
            rep = New lsfitreport()
            alglib.lsfitresults(state.csobj, info, c, rep.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub lsfitsetgradientcheck(state As lsfitstate, teststep As Double)
        Try
            alglib.lsfitsetgradientcheck(state.csobj, teststep)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub

    Public Class pspline2interpolant
        Public csobj As alglib.pspline2interpolant
    End Class
    Public Class pspline3interpolant
        Public csobj As alglib.pspline3interpolant
    End Class


    Public Sub pspline2build(xy(,) As Double, n As Integer, st As Integer, pt As Integer, ByRef p As pspline2interpolant)
        Try
            p = New pspline2interpolant()
            alglib.pspline2build(xy, n, st, pt, p.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub pspline3build(xy(,) As Double, n As Integer, st As Integer, pt As Integer, ByRef p As pspline3interpolant)
        Try
            p = New pspline3interpolant()
            alglib.pspline3build(xy, n, st, pt, p.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub pspline2buildperiodic(xy(,) As Double, n As Integer, st As Integer, pt As Integer, ByRef p As pspline2interpolant)
        Try
            p = New pspline2interpolant()
            alglib.pspline2buildperiodic(xy, n, st, pt, p.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub pspline3buildperiodic(xy(,) As Double, n As Integer, st As Integer, pt As Integer, ByRef p As pspline3interpolant)
        Try
            p = New pspline3interpolant()
            alglib.pspline3buildperiodic(xy, n, st, pt, p.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub pspline2parametervalues(p As pspline2interpolant, ByRef n As Integer, ByRef t() As Double)
        Try
            alglib.pspline2parametervalues(p.csobj, n, t)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub pspline3parametervalues(p As pspline3interpolant, ByRef n As Integer, ByRef t() As Double)
        Try
            alglib.pspline3parametervalues(p.csobj, n, t)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub pspline2calc(p As pspline2interpolant, t As Double, ByRef x As Double, ByRef y As Double)
        Try
            alglib.pspline2calc(p.csobj, t, x, y)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub pspline3calc(p As pspline3interpolant, t As Double, ByRef x As Double, ByRef y As Double, ByRef z As Double)
        Try
            alglib.pspline3calc(p.csobj, t, x, y, z)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub pspline2tangent(p As pspline2interpolant, t As Double, ByRef x As Double, ByRef y As Double)
        Try
            alglib.pspline2tangent(p.csobj, t, x, y)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub pspline3tangent(p As pspline3interpolant, t As Double, ByRef x As Double, ByRef y As Double, ByRef z As Double)
        Try
            alglib.pspline3tangent(p.csobj, t, x, y, z)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub pspline2diff(p As pspline2interpolant, t As Double, ByRef x As Double, ByRef dx As Double, ByRef y As Double, ByRef dy As Double)
        Try
            alglib.pspline2diff(p.csobj, t, x, dx, y, dy)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub pspline3diff(p As pspline3interpolant, t As Double, ByRef x As Double, ByRef dx As Double, ByRef y As Double, ByRef dy As Double, ByRef z As Double, ByRef dz As Double)
        Try
            alglib.pspline3diff(p.csobj, t, x, dx, y, dy, z, dz)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub pspline2diff2(p As pspline2interpolant, t As Double, ByRef x As Double, ByRef dx As Double, ByRef d2x As Double, ByRef y As Double, ByRef dy As Double, ByRef d2y As Double)
        Try
            alglib.pspline2diff2(p.csobj, t, x, dx, d2x, y, dy, d2y)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub pspline3diff2(p As pspline3interpolant, t As Double, ByRef x As Double, ByRef dx As Double, ByRef d2x As Double, ByRef y As Double, ByRef dy As Double, ByRef d2y As Double, ByRef z As Double, ByRef dz As Double, ByRef d2z As Double)
        Try
            alglib.pspline3diff2(p.csobj, t, x, dx, d2x, y, dy, d2y, z, dz, d2z)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Function pspline2arclength(p As pspline2interpolant, a As Double, b As Double) As Double
        Try
            pspline2arclength = alglib.pspline2arclength(p.csobj, a, b)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Function


    Public Function pspline3arclength(p As pspline3interpolant, a As Double, b As Double) As Double
        Try
            pspline3arclength = alglib.pspline3arclength(p.csobj, a, b)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Function

    Public Class linlsqrstate
        Public csobj As alglib.linlsqrstate
    End Class
    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    '
    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    Public Class linlsqrreport
        Public Property iterationscount() As Integer
            Get
                Return Me.csobj.iterationscount
            End Get
            Set(Value As Integer)
                Me.csobj.iterationscount = Value
            End Set
        End Property
        Public Property nmv() As Integer
            Get
                Return Me.csobj.nmv
            End Get
            Set(Value As Integer)
                Me.csobj.nmv = Value
            End Set
        End Property
        Public Property terminationtype() As Integer
            Get
                Return Me.csobj.terminationtype
            End Get
            Set(Value As Integer)
                Me.csobj.terminationtype = Value
            End Set
        End Property
        Public csobj As alglib.linlsqrreport
    End Class


    Public Sub linlsqrcreate(m As Integer, n As Integer, ByRef state As linlsqrstate)
        Try
            state = New linlsqrstate()
            alglib.linlsqrcreate(m, n, state.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub linlsqrsetlambdai(state As linlsqrstate, lambdai As Double)
        Try
            alglib.linlsqrsetlambdai(state.csobj, lambdai)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub linlsqrsolvesparse(state As linlsqrstate, a As sparsematrix, b() As Double)
        Try
            alglib.linlsqrsolvesparse(state.csobj, a.csobj, b)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub linlsqrsetcond(state As linlsqrstate, epsa As Double, epsb As Double, maxits As Integer)
        Try
            alglib.linlsqrsetcond(state.csobj, epsa, epsb, maxits)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub linlsqrresults(state As linlsqrstate, ByRef x() As Double, ByRef rep As linlsqrreport)
        Try
            rep = New linlsqrreport()
            alglib.linlsqrresults(state.csobj, x, rep.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub linlsqrsetxrep(state As linlsqrstate, needxrep As Boolean)
        Try
            alglib.linlsqrsetxrep(state.csobj, needxrep)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub

    Public Class rbfmodel
        Public csobj As alglib.rbfmodel
    End Class
    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    'RBF solution report:
    '* TerminationType   -   termination type, positive values - success,
    '                        non-positive - failure.
    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    Public Class rbfreport
        Public Property arows() As Integer
            Get
                Return Me.csobj.arows
            End Get
            Set(Value As Integer)
                Me.csobj.arows = Value
            End Set
        End Property
        Public Property acols() As Integer
            Get
                Return Me.csobj.acols
            End Get
            Set(Value As Integer)
                Me.csobj.acols = Value
            End Set
        End Property
        Public Property annz() As Integer
            Get
                Return Me.csobj.annz
            End Get
            Set(Value As Integer)
                Me.csobj.annz = Value
            End Set
        End Property
        Public Property iterationscount() As Integer
            Get
                Return Me.csobj.iterationscount
            End Get
            Set(Value As Integer)
                Me.csobj.iterationscount = Value
            End Set
        End Property
        Public Property nmv() As Integer
            Get
                Return Me.csobj.nmv
            End Get
            Set(Value As Integer)
                Me.csobj.nmv = Value
            End Set
        End Property
        Public Property terminationtype() As Integer
            Get
                Return Me.csobj.terminationtype
            End Get
            Set(Value As Integer)
                Me.csobj.terminationtype = Value
            End Set
        End Property
        Public csobj As alglib.rbfreport
    End Class
    Public Sub rbfserialize(obj As rbfmodel, ByRef s_out As String)
        Try
            alglib.rbfserialize(obj.csobj, s_out)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub

    Public Sub rbfunserialize(s_in As String, ByRef obj As rbfmodel)
        Try
            alglib.rbfunserialize(s_in, obj.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub rbfcreate(nx As Integer, ny As Integer, ByRef s As rbfmodel)
        Try
            s = New rbfmodel()
            alglib.rbfcreate(nx, ny, s.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub rbfsetpoints(s As rbfmodel, xy(,) As Double, n As Integer)
        Try
            alglib.rbfsetpoints(s.csobj, xy, n)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub rbfsetpoints(s As rbfmodel, xy(,) As Double)
        Try
            alglib.rbfsetpoints(s.csobj, xy)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub rbfsetalgoqnn(s As rbfmodel, q As Double, z As Double)
        Try
            alglib.rbfsetalgoqnn(s.csobj, q, z)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub rbfsetalgoqnn(s As rbfmodel)
        Try
            alglib.rbfsetalgoqnn(s.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub rbfsetalgomultilayer(s As rbfmodel, rbase As Double, nlayers As Integer, lambdav As Double)
        Try
            alglib.rbfsetalgomultilayer(s.csobj, rbase, nlayers, lambdav)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub rbfsetalgomultilayer(s As rbfmodel, rbase As Double, nlayers As Integer)
        Try
            alglib.rbfsetalgomultilayer(s.csobj, rbase, nlayers)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub rbfsetlinterm(s As rbfmodel)
        Try
            alglib.rbfsetlinterm(s.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub rbfsetconstterm(s As rbfmodel)
        Try
            alglib.rbfsetconstterm(s.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub rbfsetzeroterm(s As rbfmodel)
        Try
            alglib.rbfsetzeroterm(s.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub rbfbuildmodel(s As rbfmodel, ByRef rep As rbfreport)
        Try
            rep = New rbfreport()
            alglib.rbfbuildmodel(s.csobj, rep.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Function rbfcalc2(s As rbfmodel, x0 As Double, x1 As Double) As Double
        Try
            rbfcalc2 = alglib.rbfcalc2(s.csobj, x0, x1)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Function


    Public Function rbfcalc3(s As rbfmodel, x0 As Double, x1 As Double, x2 As Double) As Double
        Try
            rbfcalc3 = alglib.rbfcalc3(s.csobj, x0, x1, x2)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Function


    Public Sub rbfcalc(s As rbfmodel, x() As Double, ByRef y() As Double)
        Try
            alglib.rbfcalc(s.csobj, x, y)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub rbfcalcbuf(s As rbfmodel, x() As Double, ByRef y() As Double)
        Try
            alglib.rbfcalcbuf(s.csobj, x, y)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub rbfgridcalc2(s As rbfmodel, x0() As Double, n0 As Integer, x1() As Double, n1 As Integer, ByRef y(,) As Double)
        Try
            alglib.rbfgridcalc2(s.csobj, x0, n0, x1, n1, y)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub rbfunpack(s As rbfmodel, ByRef nx As Integer, ByRef ny As Integer, ByRef xwr(,) As Double, ByRef nc As Integer, ByRef v(,) As Double)
        Try
            alglib.rbfunpack(s.csobj, nx, ny, xwr, nc, v)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub

    Public Class spline2dinterpolant
        Public csobj As alglib.spline2dinterpolant
    End Class


    Public Function spline2dcalc(c As spline2dinterpolant, x As Double, y As Double) As Double
        Try
            spline2dcalc = alglib.spline2dcalc(c.csobj, x, y)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Function


    Public Sub spline2ddiff(c As spline2dinterpolant, x As Double, y As Double, ByRef f As Double, ByRef fx As Double, ByRef fy As Double, ByRef fxy As Double)
        Try
            alglib.spline2ddiff(c.csobj, x, y, f, fx, fy, fxy)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub spline2dlintransxy(c As spline2dinterpolant, ax As Double, bx As Double, ay As Double, by As Double)
        Try
            alglib.spline2dlintransxy(c.csobj, ax, bx, ay, by)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub spline2dlintransf(c As spline2dinterpolant, a As Double, b As Double)
        Try
            alglib.spline2dlintransf(c.csobj, a, b)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub spline2dcopy(c As spline2dinterpolant, ByRef cc As spline2dinterpolant)
        Try
            cc = New spline2dinterpolant()
            alglib.spline2dcopy(c.csobj, cc.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub spline2dresamplebicubic(a(,) As Double, oldheight As Integer, oldwidth As Integer, ByRef b(,) As Double, newheight As Integer, newwidth As Integer)
        Try
            alglib.spline2dresamplebicubic(a, oldheight, oldwidth, b, newheight, newwidth)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub spline2dresamplebilinear(a(,) As Double, oldheight As Integer, oldwidth As Integer, ByRef b(,) As Double, newheight As Integer, newwidth As Integer)
        Try
            alglib.spline2dresamplebilinear(a, oldheight, oldwidth, b, newheight, newwidth)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub spline2dbuildbilinearv(x() As Double, n As Integer, y() As Double, m As Integer, f() As Double, d As Integer, ByRef c As spline2dinterpolant)
        Try
            c = New spline2dinterpolant()
            alglib.spline2dbuildbilinearv(x, n, y, m, f, d, c.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub spline2dbuildbicubicv(x() As Double, n As Integer, y() As Double, m As Integer, f() As Double, d As Integer, ByRef c As spline2dinterpolant)
        Try
            c = New spline2dinterpolant()
            alglib.spline2dbuildbicubicv(x, n, y, m, f, d, c.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub spline2dcalcvbuf(c As spline2dinterpolant, x As Double, y As Double, ByRef f() As Double)
        Try
            alglib.spline2dcalcvbuf(c.csobj, x, y, f)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub spline2dcalcv(c As spline2dinterpolant, x As Double, y As Double, ByRef f() As Double)
        Try
            alglib.spline2dcalcv(c.csobj, x, y, f)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub spline2dunpackv(c As spline2dinterpolant, ByRef m As Integer, ByRef n As Integer, ByRef d As Integer, ByRef tbl(,) As Double)
        Try
            alglib.spline2dunpackv(c.csobj, m, n, d, tbl)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub spline2dbuildbilinear(x() As Double, y() As Double, f(,) As Double, m As Integer, n As Integer, ByRef c As spline2dinterpolant)
        Try
            c = New spline2dinterpolant()
            alglib.spline2dbuildbilinear(x, y, f, m, n, c.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub spline2dbuildbicubic(x() As Double, y() As Double, f(,) As Double, m As Integer, n As Integer, ByRef c As spline2dinterpolant)
        Try
            c = New spline2dinterpolant()
            alglib.spline2dbuildbicubic(x, y, f, m, n, c.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub spline2dunpack(c As spline2dinterpolant, ByRef m As Integer, ByRef n As Integer, ByRef tbl(,) As Double)
        Try
            alglib.spline2dunpack(c.csobj, m, n, tbl)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub

    Public Class spline3dinterpolant
        Public csobj As alglib.spline3dinterpolant
    End Class


    Public Function spline3dcalc(c As spline3dinterpolant, x As Double, y As Double, z As Double) As Double
        Try
            spline3dcalc = alglib.spline3dcalc(c.csobj, x, y, z)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Function


    Public Sub spline3dlintransxyz(c As spline3dinterpolant, ax As Double, bx As Double, ay As Double, by As Double, az As Double, bz As Double)
        Try
            alglib.spline3dlintransxyz(c.csobj, ax, bx, ay, by, az, bz)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub spline3dlintransf(c As spline3dinterpolant, a As Double, b As Double)
        Try
            alglib.spline3dlintransf(c.csobj, a, b)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub spline3dresampletrilinear(a() As Double, oldzcount As Integer, oldycount As Integer, oldxcount As Integer, newzcount As Integer, newycount As Integer, newxcount As Integer, ByRef b() As Double)
        Try
            alglib.spline3dresampletrilinear(a, oldzcount, oldycount, oldxcount, newzcount, newycount, newxcount, b)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub spline3dbuildtrilinearv(x() As Double, n As Integer, y() As Double, m As Integer, z() As Double, l As Integer, f() As Double, d As Integer, ByRef c As spline3dinterpolant)
        Try
            c = New spline3dinterpolant()
            alglib.spline3dbuildtrilinearv(x, n, y, m, z, l, f, d, c.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub spline3dcalcvbuf(c As spline3dinterpolant, x As Double, y As Double, z As Double, ByRef f() As Double)
        Try
            alglib.spline3dcalcvbuf(c.csobj, x, y, z, f)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub spline3dcalcv(c As spline3dinterpolant, x As Double, y As Double, z As Double, ByRef f() As Double)
        Try
            alglib.spline3dcalcv(c.csobj, x, y, z, f)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub spline3dunpackv(c As spline3dinterpolant, ByRef n As Integer, ByRef m As Integer, ByRef l As Integer, ByRef d As Integer, ByRef stype As Integer, ByRef tbl(,) As Double)
        Try
            alglib.spline3dunpackv(c.csobj, n, m, l, d, stype, tbl)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub




    Public Function rmatrixludet(a(,) As Double, pivots() As Integer, n As Integer) As Double
        Try
            rmatrixludet = alglib.rmatrixludet(a, pivots, n)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Function


    Public Function rmatrixludet(a(,) As Double, pivots() As Integer) As Double
        Try
            rmatrixludet = alglib.rmatrixludet(a, pivots)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Function


    Public Function rmatrixdet(a(,) As Double, n As Integer) As Double
        Try
            rmatrixdet = alglib.rmatrixdet(a, n)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Function


    Public Function rmatrixdet(a(,) As Double) As Double
        Try
            rmatrixdet = alglib.rmatrixdet(a)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Function


    Public Function cmatrixludet(a(,) As alglib.complex, pivots() As Integer, n As Integer) As alglib.complex
        Try
            cmatrixludet = alglib.cmatrixludet(a, pivots, n)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Function


    Public Function cmatrixludet(a(,) As alglib.complex, pivots() As Integer) As alglib.complex
        Try
            cmatrixludet = alglib.cmatrixludet(a, pivots)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Function


    Public Function cmatrixdet(a(,) As alglib.complex, n As Integer) As alglib.complex
        Try
            cmatrixdet = alglib.cmatrixdet(a, n)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Function


    Public Function cmatrixdet(a(,) As alglib.complex) As alglib.complex
        Try
            cmatrixdet = alglib.cmatrixdet(a)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Function


    Public Function spdmatrixcholeskydet(a(,) As Double, n As Integer) As Double
        Try
            spdmatrixcholeskydet = alglib.spdmatrixcholeskydet(a, n)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Function


    Public Function spdmatrixcholeskydet(a(,) As Double) As Double
        Try
            spdmatrixcholeskydet = alglib.spdmatrixcholeskydet(a)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Function


    Public Function spdmatrixdet(a(,) As Double, n As Integer, isupper As Boolean) As Double
        Try
            spdmatrixdet = alglib.spdmatrixdet(a, n, isupper)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Function


    Public Function spdmatrixdet(a(,) As Double) As Double
        Try
            spdmatrixdet = alglib.spdmatrixdet(a)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Function




    Public Function smatrixgevd(a(,) As Double, n As Integer, isuppera As Boolean, b(,) As Double, isupperb As Boolean, zneeded As Integer, problemtype As Integer, ByRef d() As Double, ByRef z(,) As Double) As Boolean
        Try
            smatrixgevd = alglib.smatrixgevd(a, n, isuppera, b, isupperb, zneeded, problemtype, d, z)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Function


    Public Function smatrixgevdreduce(ByRef a(,) As Double, n As Integer, isuppera As Boolean, b(,) As Double, isupperb As Boolean, problemtype As Integer, ByRef r(,) As Double, ByRef isupperr As Boolean) As Boolean
        Try
            smatrixgevdreduce = alglib.smatrixgevdreduce(a, n, isuppera, b, isupperb, problemtype, r, isupperr)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Function




    Public Sub rmatrixinvupdatesimple(ByRef inva(,) As Double, n As Integer, updrow As Integer, updcolumn As Integer, updval As Double)
        Try
            alglib.rmatrixinvupdatesimple(inva, n, updrow, updcolumn, updval)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub rmatrixinvupdaterow(ByRef inva(,) As Double, n As Integer, updrow As Integer, v() As Double)
        Try
            alglib.rmatrixinvupdaterow(inva, n, updrow, v)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub rmatrixinvupdatecolumn(ByRef inva(,) As Double, n As Integer, updcolumn As Integer, u() As Double)
        Try
            alglib.rmatrixinvupdatecolumn(inva, n, updcolumn, u)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub rmatrixinvupdateuv(ByRef inva(,) As Double, n As Integer, u() As Double, v() As Double)
        Try
            alglib.rmatrixinvupdateuv(inva, n, u, v)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub




    Public Function rmatrixschur(ByRef a(,) As Double, n As Integer, ByRef s(,) As Double) As Boolean
        Try
            rmatrixschur = alglib.rmatrixschur(a, n, s)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Function

    Public Class minasastate
        Public csobj As alglib.minasastate
    End Class
    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    '
    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    Public Class minasareport
        Public Property iterationscount() As Integer
            Get
                Return Me.csobj.iterationscount
            End Get
            Set(Value As Integer)
                Me.csobj.iterationscount = Value
            End Set
        End Property
        Public Property nfev() As Integer
            Get
                Return Me.csobj.nfev
            End Get
            Set(Value As Integer)
                Me.csobj.nfev = Value
            End Set
        End Property
        Public Property terminationtype() As Integer
            Get
                Return Me.csobj.terminationtype
            End Get
            Set(Value As Integer)
                Me.csobj.terminationtype = Value
            End Set
        End Property
        Public Property activeconstraints() As Integer
            Get
                Return Me.csobj.activeconstraints
            End Get
            Set(Value As Integer)
                Me.csobj.activeconstraints = Value
            End Set
        End Property
        Public csobj As alglib.minasareport
    End Class


    Public Sub minlbfgssetdefaultpreconditioner(state As minlbfgsstate)
        Try
            alglib.minlbfgssetdefaultpreconditioner(state.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub minlbfgssetcholeskypreconditioner(state As minlbfgsstate, p(,) As Double, isupper As Boolean)
        Try
            alglib.minlbfgssetcholeskypreconditioner(state.csobj, p, isupper)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub minbleicsetbarrierwidth(state As minbleicstate, mu As Double)
        Try
            alglib.minbleicsetbarrierwidth(state.csobj, mu)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub minbleicsetbarrierdecay(state As minbleicstate, mudecay As Double)
        Try
            alglib.minbleicsetbarrierdecay(state.csobj, mudecay)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub minasacreate(n As Integer, x() As Double, bndl() As Double, bndu() As Double, ByRef state As minasastate)
        Try
            state = New minasastate()
            alglib.minasacreate(n, x, bndl, bndu, state.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub minasacreate(x() As Double, bndl() As Double, bndu() As Double, ByRef state As minasastate)
        Try
            state = New minasastate()
            alglib.minasacreate(x, bndl, bndu, state.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub minasasetcond(state As minasastate, epsg As Double, epsf As Double, epsx As Double, maxits As Integer)
        Try
            alglib.minasasetcond(state.csobj, epsg, epsf, epsx, maxits)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub minasasetxrep(state As minasastate, needxrep As Boolean)
        Try
            alglib.minasasetxrep(state.csobj, needxrep)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub minasasetalgorithm(state As minasastate, algotype As Integer)
        Try
            alglib.minasasetalgorithm(state.csobj, algotype)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub minasasetstpmax(state As minasastate, stpmax As Double)
        Try
            alglib.minasasetstpmax(state.csobj, stpmax)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Function minasaiteration(state As minasastate) As Boolean
        Try
            minasaiteration = alglib.minasaiteration(state.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Function
    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    ' This family of functions is used to launcn iterations of nonlinear optimizer
    ' 
    ' These functions accept following parameters:
    '     grad    -   callback which calculates function (or merit function)
    '                 value func and gradient grad at given point x
    '     rep     -   optional callback which is called after each iteration
    '                 can be null
    '     obj     -   optional object which is passed to func/grad/hess/jac/rep
    '                 can be null
    ' 
    ' 
    ' 
    '   -- ALGLIB --
    '      Copyright 20.03.2009 by Bochkanov Sergey
    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    Public Sub minasaoptimize(state As minasastate, grad As ndimensional_grad, rep As ndimensional_rep, obj As Object)
        Dim innerobj As alglib.mincomp.minasastate = state.csobj.innerobj
        If grad Is Nothing Then
            Throw New AlglibException("ALGLIB: error in 'minasaoptimize()' (grad is null)")
        End If
        Try
            While alglib.mincomp.minasaiteration(innerobj)
                If innerobj.needfg Then
                    grad(innerobj.x, innerobj.f, innerobj.g, obj)
                    Continue While
                End If
                If innerobj.xupdated Then
                    If rep IsNot Nothing Then
                        rep(innerobj.x, innerobj.f, obj)
                    End If
                    Continue While
                End If
                Throw New AlglibException("ALGLIB: error in 'minasaoptimize' (some derivatives were not provided?)")
            End While
        Catch E As alglib.alglibexception
            Throw New AlglibException(E.msg)
        End Try
    End Sub




    Public Sub minasaresults(state As minasastate, ByRef x() As Double, ByRef rep As minasareport)
        Try
            rep = New minasareport()
            alglib.minasaresults(state.csobj, x, rep.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub minasaresultsbuf(state As minasastate, ByRef x() As Double, ByRef rep As minasareport)
        Try
            alglib.minasaresultsbuf(state.csobj, x, rep.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub minasarestartfrom(state As minasastate, x() As Double, bndl() As Double, bndu() As Double)
        Try
            alglib.minasarestartfrom(state.csobj, x, bndl, bndu)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub

    Public Class lincgstate
        Public csobj As alglib.lincgstate
    End Class
    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    '
    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    Public Class lincgreport
        Public Property iterationscount() As Integer
            Get
                Return Me.csobj.iterationscount
            End Get
            Set(Value As Integer)
                Me.csobj.iterationscount = Value
            End Set
        End Property
        Public Property nmv() As Integer
            Get
                Return Me.csobj.nmv
            End Get
            Set(Value As Integer)
                Me.csobj.nmv = Value
            End Set
        End Property
        Public Property terminationtype() As Integer
            Get
                Return Me.csobj.terminationtype
            End Get
            Set(Value As Integer)
                Me.csobj.terminationtype = Value
            End Set
        End Property
        Public Property r2() As Double
            Get
                Return Me.csobj.r2
            End Get
            Set(Value As Double)
                Me.csobj.r2 = Value
            End Set
        End Property
        Public csobj As alglib.lincgreport
    End Class


    Public Sub lincgcreate(n As Integer, ByRef state As lincgstate)
        Try
            state = New lincgstate()
            alglib.lincgcreate(n, state.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub lincgsetstartingpoint(state As lincgstate, x() As Double)
        Try
            alglib.lincgsetstartingpoint(state.csobj, x)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub lincgsetcond(state As lincgstate, epsf As Double, maxits As Integer)
        Try
            alglib.lincgsetcond(state.csobj, epsf, maxits)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub lincgsolvesparse(state As lincgstate, a As sparsematrix, isupper As Boolean, b() As Double)
        Try
            alglib.lincgsolvesparse(state.csobj, a.csobj, isupper, b)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub lincgresults(state As lincgstate, ByRef x() As Double, ByRef rep As lincgreport)
        Try
            rep = New lincgreport()
            alglib.lincgresults(state.csobj, x, rep.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub lincgsetrestartfreq(state As lincgstate, srf As Integer)
        Try
            alglib.lincgsetrestartfreq(state.csobj, srf)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub lincgsetrupdatefreq(state As lincgstate, freq As Integer)
        Try
            alglib.lincgsetrupdatefreq(state.csobj, freq)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub lincgsetxrep(state As lincgstate, needxrep As Boolean)
        Try
            alglib.lincgsetxrep(state.csobj, needxrep)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub

    Public Class nleqstate
        Public csobj As alglib.nleqstate
    End Class
    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    '
    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    Public Class nleqreport
        Public Property iterationscount() As Integer
            Get
                Return Me.csobj.iterationscount
            End Get
            Set(Value As Integer)
                Me.csobj.iterationscount = Value
            End Set
        End Property
        Public Property nfunc() As Integer
            Get
                Return Me.csobj.nfunc
            End Get
            Set(Value As Integer)
                Me.csobj.nfunc = Value
            End Set
        End Property
        Public Property njac() As Integer
            Get
                Return Me.csobj.njac
            End Get
            Set(Value As Integer)
                Me.csobj.njac = Value
            End Set
        End Property
        Public Property terminationtype() As Integer
            Get
                Return Me.csobj.terminationtype
            End Get
            Set(Value As Integer)
                Me.csobj.terminationtype = Value
            End Set
        End Property
        Public csobj As alglib.nleqreport
    End Class


    Public Sub nleqcreatelm(n As Integer, m As Integer, x() As Double, ByRef state As nleqstate)
        Try
            state = New nleqstate()
            alglib.nleqcreatelm(n, m, x, state.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub nleqcreatelm(m As Integer, x() As Double, ByRef state As nleqstate)
        Try
            state = New nleqstate()
            alglib.nleqcreatelm(m, x, state.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub nleqsetcond(state As nleqstate, epsf As Double, maxits As Integer)
        Try
            alglib.nleqsetcond(state.csobj, epsf, maxits)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub nleqsetxrep(state As nleqstate, needxrep As Boolean)
        Try
            alglib.nleqsetxrep(state.csobj, needxrep)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub nleqsetstpmax(state As nleqstate, stpmax As Double)
        Try
            alglib.nleqsetstpmax(state.csobj, stpmax)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Function nleqiteration(state As nleqstate) As Boolean
        Try
            nleqiteration = alglib.nleqiteration(state.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Function
    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    ' This family of functions is used to launcn iterations of nonlinear solver
    ' 
    ' These functions accept following parameters:
    '     func    -   callback which calculates function (or merit function)
    '                 value func at given point x
    '     jac     -   callback which calculates function vector fi[]
    '                 and Jacobian jac at given point x
    '     rep     -   optional callback which is called after each iteration
    '                 can be null
    '     obj     -   optional object which is passed to func/grad/hess/jac/rep
    '                 can be null
    ' 
    ' 
    ' 
    '   -- ALGLIB --
    '      Copyright 20.03.2009 by Bochkanov Sergey
    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    Public Sub nleqsolve(state As nleqstate, func As ndimensional_func, jac As ndimensional_jac, rep As ndimensional_rep, obj As Object)
        Dim innerobj As alglib.nleq.nleqstate = state.csobj.innerobj
        If func Is Nothing Then
            Throw New AlglibException("ALGLIB: error in 'nleqsolve()' (func is null)")
        End If
        If jac Is Nothing Then
            Throw New AlglibException("ALGLIB: error in 'nleqsolve()' (jac is null)")
        End If
        Try
            While alglib.nleq.nleqiteration(innerobj)
                If innerobj.needf Then
                    func(innerobj.x, innerobj.f, obj)
                    Continue While
                End If
                If innerobj.needfij Then
                    jac(innerobj.x, innerobj.fi, innerobj.j, obj)
                    Continue While
                End If
                If innerobj.xupdated Then
                    If rep IsNot Nothing Then
                        rep(innerobj.x, innerobj.f, obj)
                    End If
                    Continue While
                End If
                Throw New AlglibException("ALGLIB: error in 'nleqsolve' (some derivatives were not provided?)")
            End While
        Catch E As alglib.alglibexception
            Throw New AlglibException(E.msg)
        End Try
    End Sub




    Public Sub nleqresults(state As nleqstate, ByRef x() As Double, ByRef rep As nleqreport)
        Try
            rep = New nleqreport()
            alglib.nleqresults(state.csobj, x, rep.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub nleqresultsbuf(state As nleqstate, ByRef x() As Double, ByRef rep As nleqreport)
        Try
            alglib.nleqresultsbuf(state.csobj, x, rep.csobj)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub nleqrestartfrom(state As nleqstate, x() As Double)
        Try
            alglib.nleqrestartfrom(state.csobj, x)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub




    Public Sub airy(x As Double, ByRef ai As Double, ByRef aip As Double, ByRef bi As Double, ByRef bip As Double)
        Try
            alglib.airy(x, ai, aip, bi, bip)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub




    Public Function besselj0(x As Double) As Double
        Try
            besselj0 = alglib.besselj0(x)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Function


    Public Function besselj1(x As Double) As Double
        Try
            besselj1 = alglib.besselj1(x)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Function


    Public Function besseljn(n As Integer, x As Double) As Double
        Try
            besseljn = alglib.besseljn(n, x)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Function


    Public Function bessely0(x As Double) As Double
        Try
            bessely0 = alglib.bessely0(x)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Function


    Public Function bessely1(x As Double) As Double
        Try
            bessely1 = alglib.bessely1(x)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Function


    Public Function besselyn(n As Integer, x As Double) As Double
        Try
            besselyn = alglib.besselyn(n, x)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Function


    Public Function besseli0(x As Double) As Double
        Try
            besseli0 = alglib.besseli0(x)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Function


    Public Function besseli1(x As Double) As Double
        Try
            besseli1 = alglib.besseli1(x)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Function


    Public Function besselk0(x As Double) As Double
        Try
            besselk0 = alglib.besselk0(x)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Function


    Public Function besselk1(x As Double) As Double
        Try
            besselk1 = alglib.besselk1(x)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Function


    Public Function besselkn(nn As Integer, x As Double) As Double
        Try
            besselkn = alglib.besselkn(nn, x)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Function




    Public Function beta(a As Double, b As Double) As Double
        Try
            beta = alglib.beta(a, b)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Function




    Public Function incompletebeta(a As Double, b As Double, x As Double) As Double
        Try
            incompletebeta = alglib.incompletebeta(a, b, x)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Function


    Public Function invincompletebeta(a As Double, b As Double, y As Double) As Double
        Try
            invincompletebeta = alglib.invincompletebeta(a, b, y)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Function




    Public Function binomialdistribution(k As Integer, n As Integer, p As Double) As Double
        Try
            binomialdistribution = alglib.binomialdistribution(k, n, p)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Function


    Public Function binomialcdistribution(k As Integer, n As Integer, p As Double) As Double
        Try
            binomialcdistribution = alglib.binomialcdistribution(k, n, p)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Function


    Public Function invbinomialdistribution(k As Integer, n As Integer, y As Double) As Double
        Try
            invbinomialdistribution = alglib.invbinomialdistribution(k, n, y)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Function




    Public Function chebyshevcalculate(r As Integer, n As Integer, x As Double) As Double
        Try
            chebyshevcalculate = alglib.chebyshevcalculate(r, n, x)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Function


    Public Function chebyshevsum(c() As Double, r As Integer, n As Integer, x As Double) As Double
        Try
            chebyshevsum = alglib.chebyshevsum(c, r, n, x)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Function


    Public Sub chebyshevcoefficients(n As Integer, ByRef c() As Double)
        Try
            alglib.chebyshevcoefficients(n, c)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub fromchebyshev(a() As Double, n As Integer, ByRef b() As Double)
        Try
            alglib.fromchebyshev(a, n, b)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub




    Public Function chisquaredistribution(v As Double, x As Double) As Double
        Try
            chisquaredistribution = alglib.chisquaredistribution(v, x)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Function


    Public Function chisquarecdistribution(v As Double, x As Double) As Double
        Try
            chisquarecdistribution = alglib.chisquarecdistribution(v, x)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Function


    Public Function invchisquaredistribution(v As Double, y As Double) As Double
        Try
            invchisquaredistribution = alglib.invchisquaredistribution(v, y)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Function




    Public Function dawsonintegral(x As Double) As Double
        Try
            dawsonintegral = alglib.dawsonintegral(x)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Function




    Public Function ellipticintegralk(m As Double) As Double
        Try
            ellipticintegralk = alglib.ellipticintegralk(m)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Function


    Public Function ellipticintegralkhighprecision(m1 As Double) As Double
        Try
            ellipticintegralkhighprecision = alglib.ellipticintegralkhighprecision(m1)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Function


    Public Function incompleteellipticintegralk(phi As Double, m As Double) As Double
        Try
            incompleteellipticintegralk = alglib.incompleteellipticintegralk(phi, m)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Function


    Public Function ellipticintegrale(m As Double) As Double
        Try
            ellipticintegrale = alglib.ellipticintegrale(m)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Function


    Public Function incompleteellipticintegrale(phi As Double, m As Double) As Double
        Try
            incompleteellipticintegrale = alglib.incompleteellipticintegrale(phi, m)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Function




    Public Function exponentialintegralei(x As Double) As Double
        Try
            exponentialintegralei = alglib.exponentialintegralei(x)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Function


    Public Function exponentialintegralen(x As Double, n As Integer) As Double
        Try
            exponentialintegralen = alglib.exponentialintegralen(x, n)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Function




    Public Function fdistribution(a As Integer, b As Integer, x As Double) As Double
        Try
            fdistribution = alglib.fdistribution(a, b, x)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Function


    Public Function fcdistribution(a As Integer, b As Integer, x As Double) As Double
        Try
            fcdistribution = alglib.fcdistribution(a, b, x)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Function


    Public Function invfdistribution(a As Integer, b As Integer, y As Double) As Double
        Try
            invfdistribution = alglib.invfdistribution(a, b, y)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Function




    Public Sub fresnelintegral(x As Double, ByRef c As Double, ByRef s As Double)
        Try
            alglib.fresnelintegral(x, c, s)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub




    Public Function hermitecalculate(n As Integer, x As Double) As Double
        Try
            hermitecalculate = alglib.hermitecalculate(n, x)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Function


    Public Function hermitesum(c() As Double, n As Integer, x As Double) As Double
        Try
            hermitesum = alglib.hermitesum(c, n, x)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Function


    Public Sub hermitecoefficients(n As Integer, ByRef c() As Double)
        Try
            alglib.hermitecoefficients(n, c)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub




    Public Sub jacobianellipticfunctions(u As Double, m As Double, ByRef sn As Double, ByRef cn As Double, ByRef dn As Double, ByRef ph As Double)
        Try
            alglib.jacobianellipticfunctions(u, m, sn, cn, dn, ph)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub




    Public Function laguerrecalculate(n As Integer, x As Double) As Double
        Try
            laguerrecalculate = alglib.laguerrecalculate(n, x)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Function


    Public Function laguerresum(c() As Double, n As Integer, x As Double) As Double
        Try
            laguerresum = alglib.laguerresum(c, n, x)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Function


    Public Sub laguerrecoefficients(n As Integer, ByRef c() As Double)
        Try
            alglib.laguerrecoefficients(n, c)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub




    Public Function legendrecalculate(n As Integer, x As Double) As Double
        Try
            legendrecalculate = alglib.legendrecalculate(n, x)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Function


    Public Function legendresum(c() As Double, n As Integer, x As Double) As Double
        Try
            legendresum = alglib.legendresum(c, n, x)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Function


    Public Sub legendrecoefficients(n As Integer, ByRef c() As Double)
        Try
            alglib.legendrecoefficients(n, c)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub




    Public Function poissondistribution(k As Integer, m As Double) As Double
        Try
            poissondistribution = alglib.poissondistribution(k, m)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Function


    Public Function poissoncdistribution(k As Integer, m As Double) As Double
        Try
            poissoncdistribution = alglib.poissoncdistribution(k, m)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Function


    Public Function invpoissondistribution(k As Integer, y As Double) As Double
        Try
            invpoissondistribution = alglib.invpoissondistribution(k, y)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Function




    Public Function psi(x As Double) As Double
        Try
            psi = alglib.psi(x)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Function




    Public Function studenttdistribution(k As Integer, t As Double) As Double
        Try
            studenttdistribution = alglib.studenttdistribution(k, t)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Function


    Public Function invstudenttdistribution(k As Integer, p As Double) As Double
        Try
            invstudenttdistribution = alglib.invstudenttdistribution(k, p)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Function




    Public Sub sinecosineintegrals(x As Double, ByRef si As Double, ByRef ci As Double)
        Try
            alglib.sinecosineintegrals(x, si, ci)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub hyperbolicsinecosineintegrals(x As Double, ByRef shi As Double, ByRef chi As Double)
        Try
            alglib.hyperbolicsinecosineintegrals(x, shi, chi)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub




    Public Sub pearsoncorrelationsignificance(r As Double, n As Integer, ByRef bothtails As Double, ByRef lefttail As Double, ByRef righttail As Double)
        Try
            alglib.pearsoncorrelationsignificance(r, n, bothtails, lefttail, righttail)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub spearmanrankcorrelationsignificance(r As Double, n As Integer, ByRef bothtails As Double, ByRef lefttail As Double, ByRef righttail As Double)
        Try
            alglib.spearmanrankcorrelationsignificance(r, n, bothtails, lefttail, righttail)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub




    Public Sub jarqueberatest(x() As Double, n As Integer, ByRef p As Double)
        Try
            alglib.jarqueberatest(x, n, p)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub




    Public Sub mannwhitneyutest(x() As Double, n As Integer, y() As Double, m As Integer, ByRef bothtails As Double, ByRef lefttail As Double, ByRef righttail As Double)
        Try
            alglib.mannwhitneyutest(x, n, y, m, bothtails, lefttail, righttail)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub




    Public Sub onesamplesigntest(x() As Double, n As Integer, median As Double, ByRef bothtails As Double, ByRef lefttail As Double, ByRef righttail As Double)
        Try
            alglib.onesamplesigntest(x, n, median, bothtails, lefttail, righttail)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub




    Public Sub studentttest1(x() As Double, n As Integer, mean As Double, ByRef bothtails As Double, ByRef lefttail As Double, ByRef righttail As Double)
        Try
            alglib.studentttest1(x, n, mean, bothtails, lefttail, righttail)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub studentttest2(x() As Double, n As Integer, y() As Double, m As Integer, ByRef bothtails As Double, ByRef lefttail As Double, ByRef righttail As Double)
        Try
            alglib.studentttest2(x, n, y, m, bothtails, lefttail, righttail)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub unequalvariancettest(x() As Double, n As Integer, y() As Double, m As Integer, ByRef bothtails As Double, ByRef lefttail As Double, ByRef righttail As Double)
        Try
            alglib.unequalvariancettest(x, n, y, m, bothtails, lefttail, righttail)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub




    Public Sub ftest(x() As Double, n As Integer, y() As Double, m As Integer, ByRef bothtails As Double, ByRef lefttail As Double, ByRef righttail As Double)
        Try
            alglib.ftest(x, n, y, m, bothtails, lefttail, righttail)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub


    Public Sub onesamplevariancetest(x() As Double, n As Integer, variance As Double, ByRef bothtails As Double, ByRef lefttail As Double, ByRef righttail As Double)
        Try
            alglib.onesamplevariancetest(x, n, variance, bothtails, lefttail, righttail)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub




    Public Sub wilcoxonsignedranktest(x() As Double, n As Integer, e As Double, ByRef bothtails As Double, ByRef lefttail As Double, ByRef righttail As Double)
        Try
            alglib.wilcoxonsignedranktest(x, n, e, bothtails, lefttail, righttail)
        Catch _E_Alglib As alglib.alglibexception
            Throw New AlglibException(_E_Alglib.msg)
        End Try
    End Sub




End Module
