using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace KingdomRenderer.Shared.ArchieV1.Debug
{
    /// <summary>
    /// A collection of methods for reading the Callstack
    /// </summary>
    public static class CallStackTools
    {
        /// <summary>
        /// Returns an array of Methods above the currently called method in the method calling stack.
        /// Most recently called will be at in First() position.
        /// </summary>
        /// <param name="frameSkip">The number of frames to skip. 1 means it will not include this method.</param>
        /// <returns>The methods that have been called in order.</returns>
        /// <remarks>WARNING: Runs on main thread.</remarks>
        [STAThread]
        public static IEnumerable<MethodBase> GetCallingMethods(int frameSkip = 1)
        {
            // Skips the name of this method
            StackTrace stackTrace = new StackTrace(frameSkip);
            StackFrame[] stackFrames = stackTrace.GetFrames();

            var list = new List<MethodBase>();
            if (stackFrames == null) return list;
            list.AddRange(stackFrames.Select(frame => frame.GetMethod()));

            return list;
        }
        
        /// <summary>
        /// Returns the method that called the current one.
        /// </summary>
        /// <param name="frameSkip">The number of frames to skip. 2 means it will not include this method.</param>
        /// <returns>The method that called your method.</returns>
        /// <remarks>WARNING: Runs on main thread.</remarks>
        [STAThread]
        public static MethodBase GetCallingMethod(int frameSkip = 2)
        {
            StackTrace stackTrace = new StackTrace(frameSkip);
            StackFrame[] stackFrames = stackTrace.GetFrames();

            return stackFrames.First().GetMethod();
        }

        #region Get calling names
        /// <summary>
        /// Gets the namespace of the calling method.
        /// </summary>
        /// <param name="frameSkip">The number of frames to skip. 3 means it will not include this method.</param>
        /// <returns>The namespace of the method that called this method.</returns>
        /// <remarks>WARNING: Runs on main thread.</remarks>
        public static string GetCallingNamespace(int frameSkip = 3)
        {
            return GetNamespace(GetCallingMethod(frameSkip));
        }

        /// <summary>
        /// Gets the class name of the calling method
        /// </summary>
        /// <param name="frameSkip">The number of frames to skip. 3 means it will not include this method</param>
        /// <returns></returns>
        /// <remarks>WARNING: Runs on main thread.</remarks>
        public static string GetCallingClassName(int frameSkip = 3)
        {
            return GetClassName(GetCallingMethod(frameSkip));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="frameSkip"></param>
        /// <returns></returns>
        /// <remarks>WARNING: Runs on main thread.</remarks>
        public static string GetCallingMethodName(int frameSkip = 3)
        {
            return GetMethodName(GetCallingMethod(frameSkip));
        }
        #endregion

        #region Get names 
        public static string GetMethodName(MethodBase method) => method.Name;
        
        public static string GetClassName(MethodBase method) => method.DeclaringType.Name;

        public static string GetNamespace(MethodBase method) => method.DeclaringType.Namespace;

        #endregion
        
        #region Get Callstack
        /// <summary>
        /// Returns an array of Methods above the currently called method in the method calling stack. <br />
        /// Most recently called will be at in First position. <br />
        /// Logged in the form: <c>Namespace.ClassName.MethodName</c>
        /// </summary>
        /// <param name="frameSkip">The number of frames to skip. 2 means it will not include this method.</param>
        /// <returns>The methods that have been called in the order: Most recently called first.</returns>
        /// <remarks>WARNING: Runs on main thread.</remarks>
        [STAThread]
        public static IEnumerable<string> GetCallStack(int frameSkip = 2)
        {
            return GetCallingMethods(frameSkip).Select(FormatMethodBase);
        }
        
        public static string FormatMethodBase(MethodBase method)
        {
            return $"{GetNamespace(method)}.{GetClassName(method)}.{GetMethodName(method)}";
        }
        #endregion
    }
}