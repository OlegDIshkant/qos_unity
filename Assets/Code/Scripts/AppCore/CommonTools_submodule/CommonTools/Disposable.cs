using System;
using System.Collections.Generic;
using System.Text;

namespace CommonTools
{
    /// <summary>
    /// For "Anonymous Disposal" pattern from the book "C# 10 in a Nutshell" (Chapter 12 - Disposal and Garbage Collection)
    /// </summary>
    public class Disposable : IDisposable
    {
        public static Disposable Create(Action onDispose)
          => new Disposable(onDispose);

        Action _onDispose;
        Disposable(Action onDispose) => _onDispose = onDispose;

        public void Dispose()
        {
            _onDispose?.Invoke();
            _onDispose = null;
        }
    }


    /// <summary>
    /// Немного расширенная версия <see cref="IDisposable"/>.
    /// </summary>
    public interface ICustomDisposable : IDisposable
    {
        bool IsDisposed { get; }
    }


    public abstract class DisposableClass : ICustomDisposable
    {
        public bool IsDisposed { get; private set; }


        public void Dispose()
        {
            if (!IsDisposed)
            {
                OnDisposed();
                IsDisposed = true;
            }
        }


        protected virtual void OnDisposed()
        {

        }


        protected void ThrowErrorIfDisposed(string errorMsg = null)
        {
            if (IsDisposed)
            {
                throw new ObjectDisposedException(errorMsg);
            }
        }
    }
}
