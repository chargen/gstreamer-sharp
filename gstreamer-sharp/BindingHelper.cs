//
// BindingHelper.cs: Utility methods to make creating
//   element bindings by hand an easier task
//
// Authors:
//   Aaron Bockover (abockover@novell.com)
//
// Copyright (C) 2006 Novell, Inc.
//

using System;
using GLib;

namespace Gst {
  public static class BindingHelper {
    public static Delegate AddProxySignalDelegate (GLib.Object o, string signal,
        DynamicSignalHandler baseHandler, Delegate existingHandler, Delegate addHandler) {
      if (existingHandler == null) {
        DynamicSignal.Connect (o, signal, baseHandler);
      }

      return Delegate.Combine (existingHandler, addHandler);
    }

    public static Delegate RemoveProxySignalDelegate (GLib.Object o, string signal,
        DynamicSignalHandler baseHandler, Delegate existingHandler, Delegate removeHandler) {
      Delegate temp_delegate = Delegate.Remove (existingHandler, removeHandler);
      if (temp_delegate == null) {
        DynamicSignal.Disconnect (o, signal, baseHandler);
      }

      return temp_delegate;
    }

    public static void InvokeProxySignalDelegate (Delegate raiseDelegate, Type type,
        object o, GLib.SignalArgs args) {
      if (!type.IsSubclassOf (typeof (GLib.SignalArgs))) {
        throw new ArgumentException ("Args type must derive SignalArgs");
      }

      if (raiseDelegate != null) {
        GLib.SignalArgs new_args = (GLib.SignalArgs) Activator.CreateInstance (type);
        new_args.RetVal = args.RetVal;
        new_args.Args = args.Args;

        raiseDelegate.DynamicInvoke (new object [] { o, new_args });
      }
    }
  }
}
