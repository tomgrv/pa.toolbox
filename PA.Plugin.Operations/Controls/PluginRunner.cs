using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using PA.Plugin.Operations;
using PA.Plugin.Operations.Core;
using System.Threading.Tasks;

namespace PA.Plugin.Operations.Controls
{
    public partial class PluginRunner : Component
    {
        public class RunCompletedEventArgs : EventArgs
        {
            public DataMap Result { get; private set; }

            public RunCompletedEventArgs(DataMap r)
            {
                this.Result = r;
            }
        }


        internal class AsyncWrapper
        {
            private IJobPlugin p;
            internal Context Context { get; private set; }

            public AsyncWrapper(IJobPlugin p, Context c)
            {
                this.p = p;
                this.Context = c;
            }

            public AsyncWrapper(IJobPlugin p, object o)
            {
                this.p = p;
                this.Context = new Context(new Dictionary<string, object>() { { "", o } });
            }

            public AsyncWrapper(IJobPlugin p, IDictionary<string, object> map)
            {
                this.p = p;
                this.Context = new Context(map);
            }

            public async Task<DataMap> ExecuteAsync()
            {
                return await this.p.Execute(this.Context);
            }

            public DataMap Execute()
            {
                using (Task<DataMap> map = this.p.Execute(this.Context))
                {
                    map.Start();
                    map.Wait();
                    return map.Result;
                }
            }
        }

        public PluginRunner()
        {
            InitializeComponent();
            this.DelayedCalls = new Queue<AsyncWrapper>();

        }

        #region IPluginRunner Membres

        [Browsable(false)]
        public bool IsBusy { get { return this.DelayedCalls.Count > 0; } }

        [Category("Plugin Management")]
        public event EventHandler<RunCompletedEventArgs> Done;

        [Category("Plugin Management")]
        public event EventHandler Started;

        [Category("Plugin Management")]
        public bool ContinueOnError { get; set; }

        private Task<DataMap> current;

        public void RunAsync(IJobPlugin p, IDictionary<string, object> map)
        {
            this.DelayedCalls.Enqueue(new AsyncWrapper(p, map));
            this.RunNext();
        }

        public async Task<DataMap> Run(IJobPlugin p, IDictionary<string, object> map)
        {
            if (this.current is Task<DataMap> && !this.current.IsCompleted && !this.current.IsCanceled &&  !this.current.IsFaulted)
            {
                this.current.Wait();
            }

            this.current = p.Execute(new Context(map));
            return await this.current;
        }

        public virtual void Cancel()
        {
            this.DelayedCalls.Clear();
        }

        #endregion

        #region Async

        private Queue<AsyncWrapper> DelayedCalls;

        public void RunNext()
        {
            if (this.Started != null)
            {
                this.Started(this, EventArgs.Empty);
            }

            while (this.DelayedCalls.Count > 0)
            {
                DataMap map  = await this.DelayedCalls.Dequeue().ExecuteAsync();
            }

            foreach (DataMap d in 
            {
                if (this.Done != null)
                {
                    this.Done(this, new RunCompletedEventArgs(d));
                }
            }
        }

      
        #endregion

    }
}
