using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NamedPipeWrapper.Threading
{
    #region Delegates

    internal delegate void WorkerExceptionEventHandler(Exception exception);

    internal delegate void WorkerSucceededEventHandler();

    #endregion

    internal class Worker
    {
        #region Fields

        private readonly TaskScheduler _callbackThread;

        #endregion

        #region Constructors

        public Worker() : this(CurrentTaskScheduler)
        {
        }

        public Worker(TaskScheduler callbackThread)
        {
            _callbackThread = callbackThread;
        }

        #endregion

        #region Events

        public event WorkerExceptionEventHandler Error;

        public event WorkerSucceededEventHandler Succeeded;

        #endregion

        #region Properties

        private static TaskScheduler CurrentTaskScheduler
        {
            get
            {
                return (SynchronizationContext.Current != null
                            ? TaskScheduler.FromCurrentSynchronizationContext()
                            : TaskScheduler.Default);
            }
        }

        #endregion

        #region Methods

        public void DoWork(Action action)
        {
            new Task(DoWorkImpl, action, CancellationToken.None, TaskCreationOptions.LongRunning).Start();
        }

        private void Callback(Action action)
        {
            Task.Factory.StartNew(action, CancellationToken.None, TaskCreationOptions.None, _callbackThread);
        }

        private void DoWorkImpl(object oAction)
        {
            var action = (Action)oAction;
            try
            {
                action();
                Callback(Succeed);
            }
            catch (Exception e)
            {
                Callback(() => Fail(e));
            }
        }

        private void Fail(Exception exception)
        {
            if (Error != null)
                Error(exception);
        }

        private void Succeed()
        {
            if (Succeeded != null)
                Succeeded();
        }

        #endregion
    }
}
