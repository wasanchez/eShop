using System;
namespace MicroServices.Common.General.Util
{
    /// <summary>
    /// This class executes N times an action
    /// </summary>
    public class Try
    {
        protected Action Action { get; set; }
        private Action OnFailure { get; set; }

        private int maxTries = 1;

        /// <summary>
        /// How many time will excute an Action.
        /// </summary>
        /// <returns>An instance of Try class.</returns>
        /// <param name="maxAttempts">Max attempts to exceute an action.</param>
        public Try UpTo(int maxAttempts)
        {
            maxTries = maxAttempts;
            return this;
        }

        /// <summary>
        /// To the specified action.
        /// </summary>
        /// <returns>The to.</returns>
        /// <param name="action">Action.</param>
        public static Try To(Action action)
        {
            return new Try(action);
        }

        /// <summary>
        /// Ons the failed attempt.
        /// </summary>
        /// <returns>The failed attempt.</returns>
        /// <param name="action">Action.</param>
        public Try OnFailedAttempt(Action action)
        {
            OnFailure = action;
            return this;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:MicroServices.Common.General.Util.Try"/> class.
        /// </summary>
        /// <param name="action">Action.</param>
        public Try(Action action)
        {
            Action = action;
        }

        /// <summary>
        /// Attempt this instance.
        /// </summary>
        /// <returns>The attempt.</returns>
        protected virtual TryResult Attempt()
        {
            for (int attempt = 1; attempt < maxTries; attempt++)
            {
                try
                {
                    Action();
                    break;
                }
                catch (Exception ex)
                {
                    OnFailure?.Invoke();
                    if (attempt < maxTries) continue;

                    return new TryResult(ex);
                }
            }

            return new TryResult(true);
        }
    }

    /// <summary>
    /// Try result class
    /// </summary>
    public class TryResult
    {
        public TryResult(bool success)
        {
            Success = success;
        }

        public TryResult(Exception exception)
        {
            Exception = exception;
        }

        public bool Success { get; private set; }
        public Exception Exception { get; private set; }
    }
}
