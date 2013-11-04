using System;
using System.Data.Objects;
using System.Diagnostics;
using System.Threading;
using PA.Data.Interfaces;

namespace PA.Data
{
    public class SafeContext<T> : IContextHandler<T> where T : ObjectContext
    {
        private static Semaphore sem;
        private static T context;
        private string name;

        public SafeContext()
        {
            if (sem == null)
            {
                sem = new Semaphore(1, 1);
            }

#if DEBUG
            StackTrace stackTrace = new StackTrace();           // get call stack
            StackFrame[] stackFrames = stackTrace.GetFrames();  // get method calls (frames)
            this.name = stackFrames[1].GetMethod().DeclaringType.ToString() + "." + stackFrames[1].GetMethod().Name;
#endif

            Debug.WriteLine("Context waiting in <" + (this.name ?? "") + ">");
            sem.WaitOne();
            Debug.WriteLine("Context in use in <" + (this.name ?? "") + ">");

            if (context is ObjectContext)
            {
                throw new AccessViolationException("Will be started twice");
            }
           
            context = Activator.CreateInstance<T>();
        }

        #region IContextHandler Membres

        public T Context
        {
            get { return context; }
        }

        #endregion

        #region IDisposable Membres

        public void Dispose()
        {
            Debug.WriteLine("Context dispose in <" + (this.name ?? "") + ">");

            if (context is T)
            {
                context.Dispose();
                context = null;
            }

            if (sem is Semaphore)
            {
                sem.Release();
            }
        }

        #endregion

    }
}
