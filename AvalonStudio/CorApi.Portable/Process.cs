﻿using CorApi.Portable.Win32;
using System;
using System.Collections.Generic;
using System.Text;

namespace CorApi.Portable
{
    public class CorEventArgs : EventArgs
    {
        private Controller m_controller;

        private bool m_continue;


        private ManagedCallbackType m_callbackType;

        private Thread m_thread;

        public CorEventArgs(Controller controller)
        {
            m_controller = controller;
            m_continue = true;
        }

        public CorEventArgs(Controller controller, ManagedCallbackType callbackType)
        {
            m_controller = controller;
            m_continue = true;
            m_callbackType = callbackType;
        }

        /** The Controller of the current event. */
        public Controller Controller
        {
            get
            {
                return m_controller;
            }
        }

        /** 
         * The default behavior after an event is to Continue processing
         * after the event has been handled.  This can be changed by
         * setting this property to false.
         */
        public virtual bool Continue
        {
            get
            {
                return m_continue;
            }
            set
            {
                m_continue = value;
            }
        }

        /// <summary>
        /// The type of callback that returned this EventArgs object.
        /// </summary>
        public ManagedCallbackType CallbackType
        {
            get
            {
                return m_callbackType;
            }
        }

        /// <summary>
        /// The Thread associated with the callback event that returned
        /// this EventArgs object. If here is no such thread, Thread is null.
        /// </summary>
        public Thread Thread
        {
            get
            {
                return m_thread;
            }
            protected set
            {
                m_thread = value;
            }
        }

    }


    /**
     * This class is used for all events that only have access to the 
     * Process that is generating the event.
     */
    public class ProcessEventArgs : CorEventArgs
    {
        public ProcessEventArgs(Process process)
            : base(process)
        {
        }

        public ProcessEventArgs(Process process, ManagedCallbackType callbackType)
            : base(process, callbackType)
        {
        }

        /** The process that generated the event. */
        public Process Process
        {
            get
            {
                return (Process)Controller;
            }
        }

        public override string ToString()
        {
            switch (CallbackType)
            {
                case ManagedCallbackType.OnCreateProcess:
                    return "Process Created";
                case ManagedCallbackType.OnProcessExit:
                    return "Process Exited";
                case ManagedCallbackType.OnControlCTrap:
                    break;
            }
            return base.ToString();
        }
    }


    /**
     * The event arguments for events that contain both a Process
     * and an AppDomain.
     */
    public class AppDomainEventArgs : ProcessEventArgs
    {
        private AppDomain m_ad;

        public AppDomainEventArgs(Process process, AppDomain ad)
            : base(process)
        {
            m_ad = ad;
        }

        public AppDomainEventArgs(Process process, AppDomain ad,
                                      ManagedCallbackType callbackType)
            : base(process, callbackType)
        {
            m_ad = ad;
        }

        /** The AppDomain that generated the event. */
        public AppDomain AppDomain
        {
            get
            {
                return m_ad;
            }
        }

        public override string ToString()
        {
            switch (CallbackType)
            {
                case ManagedCallbackType.OnCreateAppDomain:
                    return "AppDomain Created: " + m_ad.Name;
                case ManagedCallbackType.OnAppDomainExit:
                    return "AppDomain Exited: " + m_ad.Name;
            }
            return base.ToString();
        }
    }


    /**
     * The base class for events which take an AppDomain as their
     * source, but not a Process.
     */
    public class AppDomainBaseEventArgs : CorEventArgs
    {
        public AppDomainBaseEventArgs(AppDomain ad)
            : base(ad)
        {
        }

        public AppDomainBaseEventArgs(AppDomain ad, ManagedCallbackType callbackType)
            : base(ad, callbackType)
        {
        }

        public AppDomain AppDomain
        {
            get
            {
                return (AppDomain)Controller;
            }
        }
    }


    /**
     * Arguments for events dealing with threads.
     */
    public class ThreadEventArgs : AppDomainBaseEventArgs
    {
        public ThreadEventArgs(AppDomain appDomain, Thread thread)
            : base(appDomain != null ? appDomain : thread.AppDomain)
        {
            Thread = thread;
        }

        public ThreadEventArgs(AppDomain appDomain, Thread thread,
            ManagedCallbackType callbackType)
            : base(appDomain != null ? appDomain : thread.AppDomain, callbackType)
        {
            Thread = thread;
        }

        public override string ToString()
        {
            switch (CallbackType)
            {
                case ManagedCallbackType.OnBreak:
                    return "Break";
                case ManagedCallbackType.OnCreateThread:
                    return "Thread Created";
                case ManagedCallbackType.OnThreadExit:
                    return "Thread Exited";
                case ManagedCallbackType.OnNameChange:
                    return "Name Changed";
            }
            return base.ToString();
        }
    }


    /**
     * Arguments for events involving breakpoints.
     */
    public class BreakpointEventArgs : ThreadEventArgs
    {
        private Breakpoint m_break;

        public BreakpointEventArgs(AppDomain appDomain,
                                       Thread thread,
                                       Breakpoint managedBreakpoint)
            : base(appDomain, thread)
        {
            m_break = managedBreakpoint;
        }

        public BreakpointEventArgs(AppDomain appDomain,
                                       Thread thread,
                                       Breakpoint managedBreakpoint,
                                       ManagedCallbackType callbackType)
            : base(appDomain, thread, callbackType)
        {
            m_break = managedBreakpoint;
        }

        /** The breakpoint involved. */
        public Breakpoint Breakpoint
        {
            get
            {
                return m_break;
            }
        }

        public override string ToString()
        {
            if (CallbackType == ManagedCallbackType.OnBreakpoint)
            {
                return "Breakpoint Hit";
            }
            return base.ToString();
        }
    }


    /**
     * Arguments for when a Step operation has completed.
     */
    public class StepCompleteEventArgs : ThreadEventArgs
    {
        private Stepper m_stepper;
        private CorDebugStepReason m_stepReason;

        [CLSCompliant(false)]
        public StepCompleteEventArgs(AppDomain appDomain, Thread thread,
                                         Stepper stepper, CorDebugStepReason stepReason)
            : base(appDomain, thread)
        {
            m_stepper = stepper;
            m_stepReason = stepReason;
        }

        [CLSCompliant(false)]
        public StepCompleteEventArgs(AppDomain appDomain, Thread thread,
                                         Stepper stepper, CorDebugStepReason stepReason,
                                         ManagedCallbackType callbackType)
            : base(appDomain, thread, callbackType)
        {
            m_stepper = stepper;
            m_stepReason = stepReason;
        }

        public Stepper Stepper
        {
            get
            {
                return m_stepper;
            }
        }

        [CLSCompliant(false)]
        public CorDebugStepReason StepReason
        {
            get
            {
                return m_stepReason;
            }
        }

        public override string ToString()
        {
            if (CallbackType == ManagedCallbackType.OnStepComplete)
            {
                return "Step Complete";
            }
            return base.ToString();
        }
    }


    /**
     * For events dealing with exceptions.
     */
    public class ExceptionEventArgs : ThreadEventArgs
    {
        bool m_unhandled;

        public ExceptionEventArgs(AppDomain appDomain,
                                      Thread thread,
                                      bool unhandled)
            : base(appDomain, thread)
        {
            m_unhandled = unhandled;
        }

        public ExceptionEventArgs(AppDomain appDomain,
                                      Thread thread,
                                      bool unhandled,
                                      ManagedCallbackType callbackType)
            : base(appDomain, thread, callbackType)
        {
            m_unhandled = unhandled;
        }

        /** Has the exception been handled yet? */
        public bool Unhandled
        {
            get
            {
                return m_unhandled;
            }
        }
    }


    /**
     * For events dealing the evaluation of something...
     */
    public class EvalEventArgs : ThreadEventArgs
    {
        Eval m_eval;

        public EvalEventArgs(AppDomain appDomain, Thread thread,
                                 Eval eval)
            : base(appDomain, thread)
        {
            m_eval = eval;
        }

        public EvalEventArgs(AppDomain appDomain, Thread thread,
                                 Eval eval, ManagedCallbackType callbackType)
            : base(appDomain, thread, callbackType)
        {
            m_eval = eval;
        }

        /** The object being evaluated. */
        public Eval Eval
        {
            get
            {
                return m_eval;
            }
        }

        public override string ToString()
        {
            switch (CallbackType)
            {
                case ManagedCallbackType.OnEvalComplete:
                    return "Eval Complete";
                case ManagedCallbackType.OnEvalException:
                    return "Eval Exception";
            }
            return base.ToString();
        }
    }


    /**
     * For events dealing with module loading/unloading.
     */
    public class ModuleEventArgs : AppDomainBaseEventArgs
    {
        Module m_managedModule;

        public ModuleEventArgs(AppDomain appDomain, Module managedModule)
            : base(appDomain)
        {
            m_managedModule = managedModule;
        }

        public ModuleEventArgs(AppDomain appDomain, Module managedModule,
            ManagedCallbackType callbackType)
            : base(appDomain, callbackType)
        {
            m_managedModule = managedModule;
        }

        public Module Module
        {
            get
            {
                return m_managedModule;
            }
        }

        public override string ToString()
        {
            switch (CallbackType)
            {
                case ManagedCallbackType.OnModuleLoad:
                    return "Module loaded: " + m_managedModule.Name;
                case ManagedCallbackType.OnModuleUnload:
                    return "Module unloaded: " + m_managedModule.Name;
            }
            return base.ToString();
        }
    }


    /**
     * For events dealing with class loading/unloading.
     */
    public class ClassEventArgs : AppDomainBaseEventArgs
    {
        Class m_class;

        public ClassEventArgs(AppDomain appDomain, Class managedClass)
            : base(appDomain)
        {
            m_class = managedClass;
        }

        public ClassEventArgs(AppDomain appDomain, Class managedClass,
            ManagedCallbackType callbackType)
            : base(appDomain, callbackType)
        {
            m_class = managedClass;
        }

        public Class Class
        {
            get
            {
                return m_class;
            }
        }

        public override string ToString()
        {
            switch (CallbackType)
            {
                case ManagedCallbackType.OnClassLoad:
                    return "Class loaded: " + m_class;
                case ManagedCallbackType.OnClassUnload:
                    return "Class unloaded: " + m_class;
            }
            return base.ToString();
        }
    }


    /**
     * For events dealing with debugger errors.
     */
    public class DebuggerErrorEventArgs : ProcessEventArgs
    {
        int m_hresult;
        int m_errorCode;

        public DebuggerErrorEventArgs(Process process, int hresult,
                                          int errorCode)
            : base(process)
        {
            m_hresult = hresult;
            m_errorCode = errorCode;
        }

        public DebuggerErrorEventArgs(Process process, int hresult,
                                          int errorCode, ManagedCallbackType callbackType)
            : base(process, callbackType)
        {
            m_hresult = hresult;
            m_errorCode = errorCode;
        }

        public int HResult
        {
            get
            {
                return m_hresult;
            }
        }

        public int ErrorCode
        {
            get
            {
                return m_errorCode;
            }
        }

        public override string ToString()
        {
            if (CallbackType == ManagedCallbackType.OnDebuggerError)
            {
                return "Debugger Error";
            }
            return base.ToString();
        }
    }


    /**
     * For events dealing with Assemblies.
     */
    public class AssemblyEventArgs : AppDomainBaseEventArgs
    {
        private Assembly m_assembly;
        public AssemblyEventArgs(AppDomain appDomain,
                                     Assembly assembly)
            : base(appDomain)
        {
            m_assembly = assembly;
        }

        public AssemblyEventArgs(AppDomain appDomain,
                                     Assembly assembly, ManagedCallbackType callbackType)
            : base(appDomain, callbackType)
        {
            m_assembly = assembly;
        }

        /** The Assembly of interest. */
        public Assembly Assembly
        {
            get
            {
                return m_assembly;
            }
        }

        public override string ToString()
        {
            switch (CallbackType)
            {
                case ManagedCallbackType.OnAssemblyLoad:
                    return "Assembly loaded: " + m_assembly.Name;
                case ManagedCallbackType.OnAssemblyUnload:
                    return "Assembly unloaded: " + m_assembly.Name;
            }
            return base.ToString();
        }
    }


    /**
     * For events dealing with logged messages.
     */
    public class LogMessageEventArgs : ThreadEventArgs
    {
        int m_level;
        string m_logSwitchName;
        string m_message;

        public LogMessageEventArgs(AppDomain appDomain, Thread thread,
                                       int level, string logSwitchName, string message)
            : base(appDomain, thread)
        {
            m_level = level;
            m_logSwitchName = logSwitchName;
            m_message = message;
        }

        public LogMessageEventArgs(AppDomain appDomain, Thread thread,
                                       int level, string logSwitchName, string message,
                                       ManagedCallbackType callbackType)
            : base(appDomain, thread, callbackType)
        {
            m_level = level;
            m_logSwitchName = logSwitchName;
            m_message = message;
        }

        public int Level
        {
            get
            {
                return m_level;
            }
        }

        public string LogSwitchName
        {
            get
            {
                return m_logSwitchName;
            }
        }

        public string Message
        {
            get
            {
                return m_message;
            }
        }

        public override string ToString()
        {
            if (CallbackType == ManagedCallbackType.OnLogMessage)
            {
                return "Log message(" + m_logSwitchName + ")";
            }
            return base.ToString();
        }
    }


    /**
     * For events dealing with logged messages.
     */
    public class LogSwitchEventArgs : ThreadEventArgs
    {
        int m_level;

        int m_reason;

        string m_logSwitchName;

        string m_parentName;

        public LogSwitchEventArgs(AppDomain appDomain, Thread thread,
                                      int level, int reason, string logSwitchName, string parentName)
            : base(appDomain, thread)
        {
            m_level = level;
            m_reason = reason;
            m_logSwitchName = logSwitchName;
            m_parentName = parentName;
        }

        public LogSwitchEventArgs(AppDomain appDomain, Thread thread,
                                      int level, int reason, string logSwitchName, string parentName,
                                      ManagedCallbackType callbackType)
            : base(appDomain, thread, callbackType)
        {
            m_level = level;
            m_reason = reason;
            m_logSwitchName = logSwitchName;
            m_parentName = parentName;
        }

        public int Level
        {
            get
            {
                return m_level;
            }
        }

        public int Reason
        {
            get
            {
                return m_reason;
            }
        }

        public string LogSwitchName
        {
            get
            {
                return m_logSwitchName;
            }
        }

        public string ParentName
        {
            get
            {
                return m_parentName;
            }
        }

        public override string ToString()
        {
            if (CallbackType == ManagedCallbackType.OnLogSwitch)
            {
                return "Log Switch" + "\n" +
                    "Level: " + m_level + "\n" +
                    "Log Switch Name: " + m_logSwitchName;
            }
            return base.ToString();
        }
    }


    /**
     * For events dealing with MDA messages.
     */
    public class MDAEventArgs : ProcessEventArgs
    {
        // Thread may be null.
        public MDAEventArgs(MDA mda, Thread thread, Process proc)
            : base(proc)
        {
            m_mda = mda;
            Thread = thread;
            //m_proc = proc;
        }

        public MDAEventArgs(MDA mda, Thread thread, Process proc,
            ManagedCallbackType callbackType)
            : base(proc, callbackType)
        {
            m_mda = mda;
            Thread = thread;
            //m_proc = proc;
        }

        MDA m_mda;
        public MDA MDA { get { return m_mda; } }

        public override string ToString()
        {
            if (CallbackType == ManagedCallbackType.OnMDANotification)
            {
                return "MDANotification" + "\n" +
                    "Name=" + m_mda.Name + "\n" +
                    "XML=" + m_mda.XML;
            }
            return base.ToString();
        }

        //Process m_proc;
        //Process Process { get { return m_proc; } }
    }


    /**
     * For events dealing module symbol updates.
     */
    public class UpdateModuleSymbolsEventArgs : ModuleEventArgs
    {
        IStream m_stream;

        [CLSCompliant(false)]
        public UpdateModuleSymbolsEventArgs(AppDomain appDomain,
                                                Module managedModule,
                                                IStream stream)
            : base(appDomain, managedModule)
        {
            m_stream = stream;
        }

        [CLSCompliant(false)]
        public UpdateModuleSymbolsEventArgs(AppDomain appDomain,
                                                Module managedModule,
                                                IStream stream,
                                                ManagedCallbackType callbackType)
            : base(appDomain, managedModule, callbackType)
        {
            m_stream = stream;
        }

        [CLSCompliant(false)]
        public IStream Stream
        {
            get
            {
                return m_stream;
            }
        }

        public override string ToString()
        {
            if (CallbackType == ManagedCallbackType.OnUpdateModuleSymbols)
            {
                return "Module Symbols Updated";
            }
            return base.ToString();
        }
    }

    public sealed class ExceptionInCallbackEventArgs : CorEventArgs
    {
        public ExceptionInCallbackEventArgs(Controller controller, Exception exceptionThrown)
            : base(controller)
        {
            m_exceptionThrown = exceptionThrown;
        }

        public ExceptionInCallbackEventArgs(Controller controller, Exception exceptionThrown,
            ManagedCallbackType callbackType)
            : base(controller, callbackType)
        {
            m_exceptionThrown = exceptionThrown;
        }

        public Exception ExceptionThrown
        {
            get
            {
                return m_exceptionThrown;
            }
        }

        public override string ToString()
        {
            if (CallbackType == ManagedCallbackType.OnExceptionInCallback)
            {
                return "Callback Exception: " + m_exceptionThrown.Message;
            }
            return base.ToString();
        }

        private Exception m_exceptionThrown;
    }


    /**
     * Edit and Continue callbacks
     */
    public class EditAndContinueRemapEventArgs : ThreadEventArgs
    {
        public EditAndContinueRemapEventArgs(AppDomain appDomain,
                                        Thread thread,
                                        Function managedFunction,
                                        int accurate)
            : base(appDomain, thread)
        {
            m_managedFunction = managedFunction;
            m_accurate = accurate;
        }

        public EditAndContinueRemapEventArgs(AppDomain appDomain,
                                        Thread thread,
                                        Function managedFunction,
                                        int accurate,
                                        ManagedCallbackType callbackType)
            : base(appDomain, thread, callbackType)
        {
            m_managedFunction = managedFunction;
            m_accurate = accurate;
        }

        public Function Function
        {
            get
            {
                return m_managedFunction;
            }
        }

        public bool IsAccurate
        {
            get
            {
                return m_accurate != 0;
            }
        }

        private Function m_managedFunction;
        private int m_accurate;
    }


    public class BreakpointSetErrorEventArgs : ThreadEventArgs
    {
        public BreakpointSetErrorEventArgs(AppDomain appDomain,
                                        Thread thread,
                                        Breakpoint breakpoint,
                                        int errorCode)
            : base(appDomain, thread)
        {
            m_breakpoint = breakpoint;
            m_errorCode = errorCode;
        }

        public BreakpointSetErrorEventArgs(AppDomain appDomain,
                                        Thread thread,
                                        Breakpoint breakpoint,
                                        int errorCode,
                                        ManagedCallbackType callbackType)
            : base(appDomain, thread, callbackType)
        {
            m_breakpoint = breakpoint;
            m_errorCode = errorCode;
        }

        public Breakpoint Breakpoint
        {
            get
            {
                return m_breakpoint;
            }
        }

        public int ErrorCode
        {
            get
            {
                return m_errorCode;
            }
        }

        public override string ToString()
        {
            if (CallbackType == ManagedCallbackType.OnBreakpointSetError)
            {
                return "Error Setting Breakpoint";
            }
            return base.ToString();
        }

        private Breakpoint m_breakpoint;
        private int m_errorCode;
    }


    public sealed class FunctionRemapOpportunityEventArgs : ThreadEventArgs
    {
        public FunctionRemapOpportunityEventArgs(AppDomain appDomain,
                                           Thread thread,
                                           Function oldFunction,
                                           Function newFunction,
                                           int oldILoffset
                                           )
            : base(appDomain, thread)
        {
            m_oldFunction = oldFunction;
            m_newFunction = newFunction;
            m_oldILoffset = oldILoffset;
        }

        public FunctionRemapOpportunityEventArgs(AppDomain appDomain,
                                           Thread thread,
                                           Function oldFunction,
                                           Function newFunction,
                                           int oldILoffset,
                                           ManagedCallbackType callbackType
                                           )
            : base(appDomain, thread, callbackType)
        {
            m_oldFunction = oldFunction;
            m_newFunction = newFunction;
            m_oldILoffset = oldILoffset;
        }

        public Function OldFunction
        {
            get
            {
                return m_oldFunction;
            }
        }

        public Function NewFunction
        {
            get
            {
                return m_newFunction;
            }
        }

        public int OldILOffset
        {
            get
            {
                return m_oldILoffset;
            }
        }

        public override string ToString()
        {
            if (CallbackType == ManagedCallbackType.OnFunctionRemapOpportunity)
            {
                return "Function Remap Opportunity";
            }
            return base.ToString();
        }

        private Function m_oldFunction, m_newFunction;
        private int m_oldILoffset;
    }

    public sealed class FunctionRemapCompleteEventArgs : ThreadEventArgs
    {
        public FunctionRemapCompleteEventArgs(AppDomain appDomain,
                                           Thread thread,
                                           Function managedFunction
                                           )
            : base(appDomain, thread)
        {
            m_managedFunction = managedFunction;
        }

        public FunctionRemapCompleteEventArgs(AppDomain appDomain,
                                           Thread thread,
                                           Function managedFunction,
                                           ManagedCallbackType callbackType
                                           )
            : base(appDomain, thread, callbackType)
        {
            m_managedFunction = managedFunction;
        }

        public Function Function
        {
            get
            {
                return m_managedFunction;
            }
        }

        private Function m_managedFunction;
    }


    public class ExceptionUnwind2EventArgs : ThreadEventArgs
    {

        [CLSCompliant(false)]
        public ExceptionUnwind2EventArgs(AppDomain appDomain, Thread thread,
                                            CorDebugExceptionUnwindCallbackType eventType,
                                            int flags)
            : base(appDomain, thread)
        {
            m_eventType = eventType;
            m_flags = flags;
        }

        [CLSCompliant(false)]
        public ExceptionUnwind2EventArgs(AppDomain appDomain, Thread thread,
                                            CorDebugExceptionUnwindCallbackType eventType,
                                            int flags,
                                            ManagedCallbackType callbackType)
            : base(appDomain, thread, callbackType)
        {
            m_eventType = eventType;
            m_flags = flags;
        }

        [CLSCompliant(false)]
        public CorDebugExceptionUnwindCallbackType EventType
        {
            get
            {
                return m_eventType;
            }
        }

        public int Flags
        {
            get
            {
                return m_flags;
            }
        }

        public override string ToString()
        {
            if (CallbackType == ManagedCallbackType.OnExceptionUnwind2)
            {
                return "Exception unwind\n" +
                    "EventType: " + m_eventType;
            }
            return base.ToString();
        }

        CorDebugExceptionUnwindCallbackType m_eventType;
        int m_flags;
    }


    public class Exception2EventArgs : ThreadEventArgs
    {

        [CLSCompliant(false)]
        public Exception2EventArgs(AppDomain appDomain,
                                      Thread thread,
                                      Frame frame,
                                      int offset,
                                      CorDebugExceptionCallbackType eventType,
                                      int flags)
            : base(appDomain, thread)
        {
            m_frame = frame;
            m_offset = offset;
            m_eventType = eventType;
            m_flags = flags;
        }

        [CLSCompliant(false)]
        public Exception2EventArgs(AppDomain appDomain,
                                      Thread thread,
                                      Frame frame,
                                      int offset,
                                      CorDebugExceptionCallbackType eventType,
                                      int flags,
                                      ManagedCallbackType callbackType)
            : base(appDomain, thread, callbackType)
        {
            m_frame = frame;
            m_offset = offset;
            m_eventType = eventType;
            m_flags = flags;
        }

        public Frame Frame
        {
            get
            {
                return m_frame;
            }
        }

        public int Offset
        {
            get
            {
                return m_offset;
            }
        }

        [CLSCompliant(false)]
        public CorDebugExceptionCallbackType EventType
        {
            get
            {
                return m_eventType;
            }
        }

        public int Flags
        {
            get
            {
                return m_flags;
            }
        }

        public override string ToString()
        {
            if (CallbackType == ManagedCallbackType.OnException2)
            {
                return "Exception Thrown";
            }
            return base.ToString();
        }

        Frame m_frame;
        int m_offset;
        CorDebugExceptionCallbackType m_eventType;
        int m_flags;
    }


    public enum ManagedCallbackType
    {
        OnBreakpoint,
        OnStepComplete,
        OnBreak,
        OnException,
        OnEvalComplete,
        OnEvalException,
        OnCreateProcess,
        OnProcessExit,
        OnCreateThread,
        OnThreadExit,
        OnModuleLoad,
        OnModuleUnload,
        OnClassLoad,
        OnClassUnload,
        OnDebuggerError,
        OnLogMessage,
        OnLogSwitch,
        OnCreateAppDomain,
        OnAppDomainExit,
        OnAssemblyLoad,
        OnAssemblyUnload,
        OnControlCTrap,
        OnNameChange,
        OnUpdateModuleSymbols,
        OnFunctionRemapOpportunity,
        OnFunctionRemapComplete,
        OnBreakpointSetError,
        OnException2,
        OnExceptionUnwind2,
        OnMDANotification,
        OnExceptionInCallback,
    }

    public partial class Process
    {
        public delegate void DebugEventHandler<in TArgs>(Object sender, TArgs args) where TArgs : EventArgs;

        public event DebugEventHandler<BreakpointEventArgs> OnBreakpoint = delegate { };
        public event DebugEventHandler<BreakpointEventArgs> OnBreakpointSetError = delegate { };
        public event DebugEventHandler<StepCompleteEventArgs> OnStepComplete = delegate { };
        public event DebugEventHandler<ThreadEventArgs> OnBreak = delegate { };
        public event DebugEventHandler<ExceptionEventArgs> OnException = delegate { };
        public event DebugEventHandler<EvalEventArgs> OnEvalComplete = delegate { };
        public event DebugEventHandler<EvalEventArgs> OnEvalException = delegate { };
        public event DebugEventHandler<ProcessEventArgs> OnCreateProcess = delegate { };
        public event DebugEventHandler<ProcessEventArgs> OnProcessExit = delegate { };
        public event DebugEventHandler<ThreadEventArgs> OnCreateThread = delegate { };
        public event DebugEventHandler<ThreadEventArgs> OnThreadExit = delegate { };
        public event DebugEventHandler<ModuleEventArgs> OnModuleLoad = delegate { };
        public event DebugEventHandler<ModuleEventArgs> OnModuleUnload = delegate { };
        public event DebugEventHandler<ClassEventArgs> OnClassLoad = delegate { };
        public event DebugEventHandler<ClassEventArgs> OnClassUnload = delegate { };
        public event DebugEventHandler<DebuggerErrorEventArgs> OnDebuggerError = delegate { };
        public event DebugEventHandler<MDAEventArgs> OnMDANotification = delegate { };
        public event DebugEventHandler<LogMessageEventArgs> OnLogMessage = delegate { };
        public event DebugEventHandler<LogSwitchEventArgs> OnLogSwitch = delegate { };
        public event DebugEventHandler<AppDomainEventArgs> OnCreateAppDomain = delegate { };
        public event DebugEventHandler<AppDomainEventArgs> OnAppDomainExit = delegate { };
        public event DebugEventHandler<AssemblyEventArgs> OnAssemblyLoad = delegate { };
        public event DebugEventHandler<AssemblyEventArgs> OnAssemblyUnload = delegate { };
        public event DebugEventHandler<ProcessEventArgs> OnControlCTrap = delegate { };
        public event DebugEventHandler<ThreadEventArgs> OnNameChange = delegate { };
        public event DebugEventHandler<UpdateModuleSymbolsEventArgs> OnUpdateModuleSymbols = delegate { };
        public event DebugEventHandler<FunctionRemapOpportunityEventArgs> OnFunctionRemapOpportunity = delegate { };
        public event DebugEventHandler<FunctionRemapCompleteEventArgs> OnFunctionRemapComplete = delegate { };
        public event DebugEventHandler<Exception2EventArgs> OnException2 = delegate { };
        public event DebugEventHandler<ExceptionUnwind2EventArgs> OnExceptionUnwind2 = delegate { };
        public event DebugEventHandler<ExceptionInCallbackEventArgs> OnExceptionInCallback = delegate { };
    }
}