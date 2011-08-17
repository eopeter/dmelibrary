using System;
using System.Net.Sockets;
using System.Data;
using System.Collections.Generic;
using System.Text;

namespace DME.Base
{
    //delegate 定义
    public delegate void CbSimple();
    public delegate void CbGeneric<T>(T obj);
    public delegate void CbSimpleInt(int val);
    public delegate void CbSimpleBool(bool val);
    public delegate void CbSimpleStr(string str);
    public delegate void CbSimpleObj(object obj);
    public delegate void CbStream(byte[] stream);
    public delegate void CbDataRow(DataRow row);
    public delegate void CbDateTime(DateTime dt);
    public delegate void CbException(Exception ex);
    public delegate void CbNetworkStream(NetworkStream stream);

    public delegate void CbSimpleStrInt(string str, int val);
    public delegate void CbProgress(int val, int total);

    public delegate void Action<T1, T2>(T1 t1, T2 t2);
    public delegate void Action<T1, T2, T3>(T1 t1, T2 t2, T3 t3);

    public delegate void Func();
    public delegate TResult Func<TResult>();
    public delegate TResult Func<T, TResult>(T arg);
    public delegate TResult Func<T, T2, TResult>(T arg, T2 arg2);
    public delegate TResult Func<T, T2, T3, TResult>(T arg, T2 arg2, T3 arg3);
    public delegate TResult Func<T, T2, T3, T4, TResult>(T arg, T2 arg2, T3 arg3, T4 arg4);
    public delegate TResult Func<T, T2, T3, T4, T5, TResult>(T arg, T2 arg2, T3 arg3, T4 arg4, T5 arg5);

}
